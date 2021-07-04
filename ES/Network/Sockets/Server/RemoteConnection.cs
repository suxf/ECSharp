using System;
using System.Net;
using System.Text;

namespace ES.Network.Sockets.Server
{
    /// <summary>
    /// 远程客户端连接
    /// <para>TCP模式下 主动推送数据直接保存</para>
    /// <para>UDP模式下 主动推送需要调用CloneUDPConnection()保存使用</para>
    /// </summary>
    public class RemoteConnection
    {
        /// <summary>
        /// 连接服务控制对象
        /// </summary>
        public ServerSocket SocketSvrMgr { get; protected set; } = null;
        /// <summary>
        /// 数据包缓存
        /// </summary>
        public SweetStream RBuffer { get; internal set; } = null;
        /// <summary>
        /// 存活情况
        /// </summary>
        internal bool IsAlive { get; set; } = false;
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool HasConnected { get { if (Socket != null) return Socket.IsConnected && !Socket.IsClosed; return false; } }
        /// <summary>
        /// 时间计时器
        /// </summary>
        internal int timeoutCount = 0;

        /// <summary>
        /// 异步接受信息委托回调
        /// </summary>
        public IRemoteSocket SocketInvoke { get; protected set; } = null;

        /// <summary>
        /// ESF.Socket
        /// </summary>
        public Socket Socket { get; protected set; } = null;

        /// <summary>
        /// 用户绑定对象(TCP模式)
        /// </summary>
        public object Target = null;

        /// <summary>
        /// 超级远程连接
        /// </summary>
        internal HyperSocket.RemoteHyperSocket hySocket = null;

        /// <summary>
        /// 接受事件参数
        /// </summary>
        public System.Net.Sockets.SocketAsyncEventArgs ReceiveEventArgs { get; protected set; } = null;

        /// <summary>
        /// 发送事件参数
        /// </summary>
        internal SocketAsyncEventArgsEx sendEventArgs;

        /// <summary>
        /// 服务分配的系统接收参数
        /// </summary>
        internal System.Net.Sockets.SocketAsyncEventArgs readWriteEventArg = null;

        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime ConnectDateTime { get; protected set; } = DateTime.MinValue;

        /// <summary>
        /// 构造函数 tcp
        /// <para>正式调用还需执行Init()</para>
        /// </summary>
        internal RemoteConnection(ServerSocket service, IRemoteSocket socketInvoke)
        {
            SocketSvrMgr = service;
            this.SocketInvoke = socketInvoke;
        }

        /// <summary>
        /// 构造函数 udp
        /// <para>正式调用还需执行Init()</para>
        /// </summary>
        internal RemoteConnection(EndPoint remoteEndPoint, ServerSocket service)
        {
            IsAlive = true;
            SocketSvrMgr = service;
            Socket = Socket.FillAsClient(remoteEndPoint);
            sendEventArgs = new SocketAsyncEventArgsEx(this, remoteEndPoint, null);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init(Socket socket, EventHandler<System.Net.Sockets.SocketAsyncEventArgs> eventHandler)
        {

            ReceiveEventArgs = new System.Net.Sockets.SocketAsyncEventArgs();
            ReceiveEventArgs.UserToken = this;

            ConnectDateTime = DateTime.Now;
            RBuffer = new SweetStream();
            IsAlive = true;
            timeoutCount = 0;

            Socket = socket;
            if (Socket != null)
            {
                sendEventArgs = new SocketAsyncEventArgsEx(this, socket, eventHandler);
                ReceiveEventArgs.AcceptSocket = Socket.GetSocket();
                ReceiveEventArgs.RemoteEndPoint = Socket.endPoint;
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        public bool Send(byte[] buffer)
        {
            return Send(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据(utf8字符串数据)
        /// </summary>
        /// <param name="utf8str">数据</param>
        public bool Send(string utf8str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(utf8str);
            return Send(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <param name="buffer">数据</param>
        public bool Send(ushort sessionId, byte[] buffer)
        {
            return Send(sessionId, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">数据偏移</param>
        /// <param name="count">数据大小</param>
        public bool Send(byte[] buffer, int offset, int count)
        {
            return Send(0x00, buffer, offset, count);
        }

        /// <summary>
        /// 发送数据(utf8字符串数据)
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <param name="utf8str">数据</param>
        public bool Send(ushort sessionId, string utf8str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(utf8str);
            return Send(sessionId, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">数据偏移</param>
        /// <param name="count">数据大小</param>
        public bool Send(ushort sessionId, byte[] buffer, int offset, int count)
        {
            if (SocketSvrMgr != null) return SocketSvrMgr.SendAsyncEvent(this, sessionId, buffer, offset, count);
            return false;
        }

        /// <summary>
        /// 触发回调委托
        /// </summary>
        internal void TriggerSocketInvoke()
        {
            byte[] sb = null;
            do
            {
                if (RBuffer != null) sb = RBuffer.TakeStreamBuffer();
                if (sb == null) return;
                if (SocketInvoke != null) SocketInvoke.OnReceivedCompleted(new RemoteSocketMsg(0, sb, this));
            } while (true);
        }

        /// <summary>
        /// 销毁当前对象并且从服务器列表中删除
        /// </summary>
        public void Destroy()
        {
            // 不存活就不存在销毁
            if (!IsAlive) return;
            IsAlive = false;
            // 移除套接字参数
            sendEventArgs.Destroy();
            if (ReceiveEventArgs != null) ReceiveEventArgs.Dispose();

            // 移除管理器
            if (SocketSvrMgr != null) SocketSvrMgr.RemoveExistClient(this);
            // 重置
            ReceiveEventArgs = null;
            Socket = null;
            RBuffer = null;
            Target = null;
            hySocket = null;
        }
    }
}
