using System;
using System.Net.Sockets;
using System.Text;

namespace ES.Network.Sockets
{
    /// <summary>
    /// 远程客户端连接
    /// TCP模式下 主动推送数据直接保存
    /// UDP模式下 主动推送需要调用CloneUDPConnection()保存使用
    /// </summary>
    public class RemoteConnection
    {
        /// <summary>
        /// 连接服务控制对象
        /// </summary>
        public ServerSocket socketSvrMgr { get; protected set; } = null;
        /// <summary>
        /// 数据包缓存
        /// </summary>
        public SweetStream rBuffer { get; internal set; } = null;
        /// <summary>
        /// 存活情况
        /// </summary>
        internal bool isAlive { get; set; } = false;
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool hasConnected { get { if (socket != null) return socket.isConnected && !socket.isClosed; return false; } }
        /// <summary>
        /// 时间计时器
        /// </summary>
        internal int timeoutCount = 0;

        /// <summary>
        /// 异步接受信息委托回调
        /// </summary>
        public RemoteSocketInvoke socketInvoke { get; protected set; } = null;

        /// <summary>
        /// ESF.Socket
        /// </summary>
        public Socket socket { get; protected set; } = null;

        /// <summary>
        /// 用户绑定对象(TCP模式)
        /// </summary>
        public object Target = null;

        /// <summary>
        /// 接受事件参数
        /// </summary>
        public SocketAsyncEventArgs receiveEventArgs { get; protected set; } = null;

        /// <summary>
        /// 发送事件参数
        /// </summary>
        public SocketAsyncEventArgs sendEventArgs { get; protected set; } = null;

        /// <summary>
        /// 服务分配的系统接收参数
        /// </summary>
        internal SocketAsyncEventArgs readWriteEventArg = null;

        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime connectDateTime { get; protected set; } = DateTime.MinValue;

        /// <summary>
        /// 构造函数
        /// 正式调用还需执行Init()
        /// </summary>
        public RemoteConnection()
        {
            socketSvrMgr = null;

            sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.UserToken = this;
            receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.UserToken = this;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(Socket socket, ServerSocket service, RemoteSocketInvoke socketInvoke)
        {
            connectDateTime = DateTime.Now;
            rBuffer = new SweetStream();
            isAlive = true;
            timeoutCount = 0;

            socketSvrMgr = service;
            this.socketInvoke = socketInvoke;
            this.socket = socket;
            if (this.socket != null)
            {
                sendEventArgs.AcceptSocket = this.socket.GetSocket();
                receiveEventArgs.AcceptSocket = this.socket.GetSocket();

                sendEventArgs.RemoteEndPoint = this.socket.endPoint;
                receiveEventArgs.RemoteEndPoint = this.socket.endPoint;
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
        /// <param name="main">主指令</param>
        /// <param name="second">副指令</param>
        /// <param name="buffer">数据</param>
        public bool Send(byte main, byte second, byte[] buffer)
        {
            return Send(main, second, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">数据偏移</param>
        /// <param name="count">数据大小</param>
        public bool Send(byte[] buffer, int offset, int count)
        {
            return Send(0x01, 0xff, buffer, offset, count);
        }

        /// <summary>
        /// 发送数据(utf8字符串数据)
        /// </summary>
        /// <param name="main">主指令</param>
        /// <param name="second">副指令</param>
        /// <param name="utf8str">数据</param>
        public bool Send(byte main, byte second, string utf8str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(utf8str);
            return Send(main, second, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="main">主指令</param>
        /// <param name="second">副指令</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">数据偏移</param>
        /// <param name="count">数据大小</param>
        public bool Send(byte main, byte second, byte[] buffer, int offset, int count)
        {
            if (socketSvrMgr != null)
            {
                return socketSvrMgr.SendAsyncEvent(this, buffer, offset, count, main, second);
            }
            return false;
        }

        /// <summary>
        /// 触发回调委托
        /// </summary>
        internal void TriggerSocketInvoke()
        {
            StreamBuffer sb = rBuffer.TakeStreamBuffer();
            while (sb != null)
            {
                if (socketInvoke != null)
                    socketInvoke.ReceivedCompleted(new RemoteSocketMsg(sb.main, sb.second, sb.buffer, this));
                // 提取下一个
                sb = rBuffer.TakeStreamBuffer();
            }
        }

        /// <summary>
        /// 获取UDP发送器
        /// 仅在UDP连接下有实际意义，其他连接下返回为NULL
        /// </summary>
        public RemoteConnection CloneUDPConnection()
        {
            if (socket.protocolType == ProtocolType.Udp)
            {
                RemoteConnection connection = new RemoteConnection();

                connection.connectDateTime = connectDateTime;
                connection.rBuffer = new SweetStream();
                connection.isAlive = isAlive;
                connection.timeoutCount = timeoutCount;

                connection.socket = socket;

                connection.socketSvrMgr = socketSvrMgr;
                connection.sendEventArgs.RemoteEndPoint = socket.endPoint;

                return connection;
            }
            return null;
        }

        /// <summary>
        /// 销毁当前对象并且从服务器列表中删除
        /// </summary>
        public void Destroy()
        {
            // 不存活就不存在销毁
            if (!isAlive) return;
            isAlive = false;

            Target = null;
            if (socketSvrMgr != null) socketSvrMgr.RemoveExistClient(this);
        }
    }
}
