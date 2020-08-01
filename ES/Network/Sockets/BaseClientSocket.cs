using ES.Common.Log;
using System;
using System.Net.Sockets;

namespace ES.Network.Sockets
{
    /// <summary>
    /// ESF客户端套接字 抽象基础类
    /// </summary>
    public abstract class BaseClientSocket
    {
        /// <summary>
        /// 远程客户端套接字
        /// </summary>
        protected Socket clientSocket = null;

        /// <summary>
        /// 接受缓存最大长度
        /// </summary>
        protected int numMaxBufferSize;

        /// <summary>
        /// 缓存
        /// </summary>
        protected byte[] buffer = null;

        /// <summary>
        /// 用户绑定对象
        /// </summary>
        public object Target = null;

        /// <summary>
        /// 解析缓存
        /// </summary>
        public SweetStream rBuffer { get; private set; } = null;

        /// <summary>
        /// 消息委托
        /// </summary>
        public SocketInvoke socketInvoke = null;

        /// <summary>
        /// 接受状态
        /// </summary>
        protected bool isRecving = false;

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool hasConnected { get { if (clientSocket != null) return clientSocket.isConnected && !clientSocket.isClosed; return false; } }

        /// <summary>
        /// 发送事件参数
        /// </summary>
        protected SocketAsyncEventArgs sendEventArgs;
        /// <summary>
        /// 发送事件参数
        /// </summary>
        public SocketAsyncEventArgs SendEventArgs { get { return sendEventArgs; } set { sendEventArgs = value; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseClientSocket(string ip, int port, int numMaxBufferSize)
        {
            clientSocket = new Socket(ip, port);
            this.numMaxBufferSize = numMaxBufferSize + SweetStream.OUTSOURCING_SIZE;
            rBuffer = new SweetStream();

            sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.UserToken = clientSocket;
            sendEventArgs.RemoteEndPoint = clientSocket.endPoint;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="esfSocket">ESFSocket对象</param>
        /// <param name="numMaxBufferSize">接受数据最大容量</param>
        public BaseClientSocket(Socket esfSocket, int numMaxBufferSize)
        {
            clientSocket = esfSocket;
            this.numMaxBufferSize = numMaxBufferSize + SweetStream.OUTSOURCING_SIZE;
            rBuffer = new SweetStream();
        }

        /// <summary>
        /// 发送数据
        /// 返回 0 为成功 -1 异常
        /// </summary>
        /// <param name="main">主指令</param>
        /// <param name="second">副指令</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        /// <returns>0 为成功 -1 异常</returns>
        protected int SendBuffer(byte main, byte second, byte[] buffer, int offset, int count)
        {
            // 数据打包
            byte[] data = null;
            if (offset == 0 && buffer.Length == count)
            {
                data = rBuffer.Encode(main, second, buffer);
            }
            else if (offset > 0)
            {
                byte[] buffer2 = new byte[count];
                Array.Copy(buffer, offset, buffer2, 0, count);
                data = rBuffer.Encode(main, second, buffer);
            }

            try
            {
                sendEventArgs.SetBuffer(data, 0, data.Length);
                return clientSocket.SendAsync(sendEventArgs) ? 0 : -1;
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "BaseClientConnection", "SendBuffer", "Socket");
                return -1;
            }
        }

        /// <summary>
        /// 发送数据 同步阻塞
        /// </summary>
        /// <param name="main">主指令</param>
        /// <param name="second">副指令</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        protected int SendBufferSync(byte main, byte second, byte[] buffer, int offset, int count)
        {
            // 数据打包
            byte[] data = null;
            if (offset == 0 && buffer.Length == count)
            {
                data = rBuffer.Encode(main, second, buffer);
            }
            else if (offset > 0)
            {
                byte[] buffer2 = new byte[count];
                Array.Copy(buffer, offset, buffer2, 0, count);
                data = rBuffer.Encode(main, second, buffer);
            }

            try
            {
                return clientSocket.Send(data);
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "BaseClientConnection", "SendBufferSync", "Socket");
                return -1;
            }
        }

        /// <summary>
        /// 发送数据 报文
        /// </summary>
        /// <param name="main">主指令</param>
        /// <param name="second">副指令</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        protected int SendBufferTo(byte main, byte second, byte[] buffer, int offset, int count)
        {
            // 数据打包
            byte[] data = null;
            if (offset == 0 && buffer.Length == count)
            {
                data = rBuffer.Encode(main, second, buffer);
            }
            else if (offset > 0)
            {
                byte[] buffer2 = new byte[count];
                Array.Copy(buffer, offset, buffer2, 0, count);
                data = rBuffer.Encode(main, second, buffer);
            }

            try
            {
                sendEventArgs.SetBuffer(data, 0, data.Length);
                return clientSocket.SendToAsync(sendEventArgs) ? 0 : -1;
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "BaseClientConnection", "SendBufferTo", "Socket");
                return -1;
            }
        }

        /// <summary>
        /// 发送数据 报文 同步阻塞
        /// </summary>
        /// <param name="main">主指令</param>
        /// <param name="second">副指令</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        protected int SendBufferToSync(byte main, byte second, byte[] buffer, int offset, int count)
        {
            // 数据打包
            byte[] data = null;
            if (offset == 0 && buffer.Length == count)
            {
                data = rBuffer.Encode(main, second, buffer);
            }
            else if (offset > 0)
            {
                byte[] buffer2 = new byte[count];
                Array.Copy(buffer, offset, buffer2, 0, count);
                data = rBuffer.Encode(main, second, buffer);
            }

            try
            {
                return clientSocket.SendTo(data, clientSocket.endPoint);
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "BaseClientConnection", "SendBufferToSync", "Socket");
                return -1;
            }
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        protected virtual void BeginReceived()
        {
            buffer = new byte[numMaxBufferSize];
            try
            {
                //开始异步接收
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "BaseClientConnection", "BeginReceived", "Socket");
            }
        }

        /// <summary>
        /// 开始监听 udp
        /// </summary>
        protected virtual void BeginReceivedFrom()
        {
            buffer = new byte[numMaxBufferSize];
            try
            {
                System.Net.EndPoint endPoint = clientSocket.endPoint;
                //开始异步接收
                clientSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endPoint, new AsyncCallback(ReceiveFromCallback), clientSocket);
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "BaseClientConnection", "BeginReceivedFrom", "Socket");
            }
        }

        /// <summary>
        /// 接受线程函数 tcp
        /// </summary>
        protected abstract void ReceiveCallback(IAsyncResult result);
        /// <summary>
        /// 接受线程函数 udp
        /// </summary>
        protected abstract void ReceiveFromCallback(IAsyncResult result);

        /// <summary>
        /// 触发回调委托
        /// </summary>
        protected virtual void TriggerSocketInvoke() { }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void Close()
        {
            if (!isRecving) return;
            isRecving = false;

            Target = null;
            clientSocket.Close();
        }
    }
}
