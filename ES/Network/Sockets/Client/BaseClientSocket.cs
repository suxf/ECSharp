using System;
using System.Net.Sockets;

namespace ES.Network.Sockets.Client
{
    /// <summary>
    /// ESF客户端套接字 抽象基础类
    /// </summary>
    public abstract class BaseClientSocket
    {
        /// <summary>
        /// 远程客户端套接字
        /// </summary>
        protected Socket clientSocket;

        /// <summary>
        /// 接受缓存最大长度
        /// </summary>
        protected int numMaxBufferSize;

        /// <summary>
        /// 缓存
        /// </summary>
        protected byte[] buffer = Array.Empty<byte>();

        /// <summary>
        /// 用户绑定对象
        /// </summary>
        public object? Target = null;

        /// <summary>
        /// 解析缓存
        /// </summary>
        public SweetStream RBuffer { get; private set; }

        /// <summary>
        /// 消息委托
        /// </summary>
        public ISocket? socketInvoke = null;

        /// <summary>
        /// 接受状态
        /// </summary>
        protected bool isRecving = false;

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool HasConnected { get { if (clientSocket != null) return clientSocket.IsConnected && !clientSocket.IsClosed; return false; } }

        /// <summary>
        /// 发送事件参数
        /// </summary>
        internal SocketAsyncEventArgsEx sendEventArgs;
        /// <summary>
        /// 读写参数
        /// </summary>
        private readonly SocketAsyncEventArgs readWriteEventArg;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="numMaxBufferSize"></param>
        public BaseClientSocket(string ip, int port, int numMaxBufferSize)
        {
            clientSocket = new Socket(ip, port);
            this.numMaxBufferSize = numMaxBufferSize + SweetStream.OUTSOURCING_SIZE;
            RBuffer = new SweetStream();

            sendEventArgs = new SocketAsyncEventArgsEx(clientSocket, clientSocket.endPoint, IO_Completed!);
            readWriteEventArg = new SocketAsyncEventArgs() { UserToken = clientSocket, RemoteEndPoint = clientSocket.endPoint };
            readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed!);
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
            RBuffer = new SweetStream();

            sendEventArgs = new SocketAsyncEventArgsEx(clientSocket, clientSocket.endPoint, IO_Completed!);
            readWriteEventArg = new SocketAsyncEventArgs() { UserToken = clientSocket, RemoteEndPoint = clientSocket.endPoint };
            readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed!);
        }

        /// <summary>
        /// 发送数据
        /// <para>返回 0 为成功 -1 异常</para>
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        /// <returns>0 为成功 -1 异常</returns>
        protected bool SendBuffer(/*ushort sessionId,*/ byte[] buffer, int offset, int count)
        {
            // 数据打包
            byte[]? data = null;
            if (offset == 0 && buffer.Length == count)
            {
                data = RBuffer.Encode(buffer);
            }
            else if (offset > 0)
            {
                byte[] buffer2 = new byte[count];
                Array.Copy(buffer, offset, buffer2, 0, count);
                data = RBuffer.Encode(buffer);
            }

            try
            {
                if (data == null)
                    return false;
                var args = sendEventArgs.Pop();
                args.SetBuffer(data, 0, data.Length);
                var willRaiseEvent = clientSocket.SendAsync(args);
                if (!willRaiseEvent)
                {
                    return ProcessSend(args);
                }
                else return true;
            }
            catch (Exception ex)
            {
                // Log.Exception(ex, "BaseClientConnection", "SendBuffer", "Socket");
                socketInvoke!.SocketException(ex);
                return false;
            }
        }

        /// <summary>
        /// 发送数据 报文
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        protected bool SendBufferTo(ushort sessionId, byte[] buffer, int offset, int count)
        {
            // 数据打包
            byte[]? data = null;
            if (offset == 0 && buffer.Length == count)
            {
                data = buffer;
            }
            else if (offset > 0)
            {
                data = new byte[count];
                Array.Copy(buffer, offset, data, 0, count);
            }

            try
            {
                if (data == null)
                    return false;
                var args = sendEventArgs.Pop();

                byte[] sendBuffer = new byte[3 + buffer.Length];
                /* 会话ID 高位 */
                // 1 byte
                sendBuffer[0] = (byte)((sessionId >> 8) & 0xFF);
                /* 会话ID 低位 */
                // 1 byte
                sendBuffer[1] = (byte)((sessionId) & 0xFF);
                /* 数据长度标识 指令验签（0x66混淆参数）*/
                // 1 byte
                sendBuffer[2] = (byte)(sendBuffer[0] + sendBuffer[1] + 0x66);
                Buffer.BlockCopy(data, 0, sendBuffer, 3, data.Length);

                args.SetBuffer(sendBuffer, 0, sendBuffer.Length);
                var willRaiseEvent = clientSocket.SendToAsync(args);
                if (!willRaiseEvent)
                {
                    return ProcessSend(args);
                }
                else return true;
            }
            catch (Exception ex)
            {
                // Log.Exception(ex, "BaseClientConnection", "SendBufferTo", "Socket");
                socketInvoke!.SocketException(ex);
                return false;
            }
        }

        /// <summary>
        /// IO完成端口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    // Log.Info("IO_Completed The last operation completed on the socket was not a receive or send");
                    break;
            }
        }

        /// <summary>
        /// 线程发送
        /// </summary>
        private bool ProcessSend(SocketAsyncEventArgs e)
        {
            (e as MySocketAsyncEventArgs)!.ResetUsedState();
            if (e.SocketError == SocketError.Success)
            {
                // done echoing data back to the client
                // RemoteUserToken token = (RemoteUserToken)e.UserToken;
                return true;
            }
            return false;
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
                // Log.Exception(ex, "BaseClientConnection", "BeginReceived", "Socket");
                socketInvoke!.SocketException(ex);
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
                // Log.Exception(ex, "BaseClientConnection", "BeginReceivedFrom", "Socket");
                socketInvoke!.SocketException(ex);
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

            sendEventArgs.Destroy();

            Target = null;
            clientSocket.Close();
        }
    }
}
