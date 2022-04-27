using ES.Variant;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ES.Network.Sockets.Client
{
    /// <summary>
    /// ESF客户端套接字
    /// </summary>
    public class ClientSocket : ISocketIOEvent
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
        protected byte[] buffer;

        /// <summary>
        /// 用户绑定对象
        /// </summary>
        public Var Tag = Var.Empty;

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
        protected bool isRecving = true;

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
        private readonly MySocketAsyncEventArgs readWriteEventArg;

        /// <summary>
        /// 构造函数
        /// <para>创建一个异步socket</para>
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口</param>
        /// <param name="numMaxBufferSize">接受数据大小[UDP模式以传送理论最大值/TCP模式以传送最佳合适值]</param>
        public ClientSocket(string ip, int port, int numMaxBufferSize)
        {
            clientSocket = new Socket(ip, port);
            this.numMaxBufferSize = numMaxBufferSize + SweetStream.OUTSOURCING_SIZE;
            buffer = new byte[this.numMaxBufferSize];
            RBuffer = new SweetStream();

            sendEventArgs = new SocketAsyncEventArgsEx(clientSocket, clientSocket.endPoint, this);
            readWriteEventArg = new MySocketAsyncEventArgs(clientSocket, clientSocket.endPoint, this);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="esfSocket">ESFSocket对象</param>
        /// <param name="numMaxBufferSize">接受数据大小[UDP模式以传送理论最大值/TCP模式以传送最佳合适值]</param>
        public ClientSocket(Socket esfSocket, int numMaxBufferSize)
        {
            clientSocket = esfSocket;
            this.numMaxBufferSize = numMaxBufferSize + SweetStream.OUTSOURCING_SIZE;
            buffer = new byte[this.numMaxBufferSize];
            RBuffer = new SweetStream();

            sendEventArgs = new SocketAsyncEventArgsEx(clientSocket, clientSocket.endPoint, this);
            readWriteEventArg = new MySocketAsyncEventArgs(clientSocket, clientSocket.endPoint, this);
        }

        /// <summary>
        /// 初始化套接字
        /// </summary>
        public bool Init(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, ISocket socketInvoke)
        {
            // 绑定委托
            this.socketInvoke = socketInvoke;
            // 启动服务
            bool bSuccess = clientSocket.Connect(addressFamily, socketType, protocolType);
            if (bSuccess)
            {
                if (socketType == SocketType.Stream)
                    BeginReceived();
                else if (socketType == SocketType.Dgram)
                    BeginReceivedFrom();
            }
            return bSuccess;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        public bool Send(Span<byte> buffer)
        {
            return Send(0x00, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据(utf8字符串数据)
        /// </summary>
        /// <param name="utf8str">数据</param>
        public bool Send(string utf8str)
        {
            Span<byte> buffer = Encoding.UTF8.GetBytes(utf8str);
            return Send(0x00, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <param name="buffer">数据</param>
        public bool Send(ushort sessionId, Span<byte> buffer)
        {
            return Send(sessionId, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据(utf8字符串数据)
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <param name="utf8str">数据</param>
        public bool Send(ushort sessionId, string utf8str)
        {
            Span<byte> buffer = Encoding.UTF8.GetBytes(utf8str);
            return Send(sessionId, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        public bool Send(ushort sessionId, Span<byte> buffer, int offset, int count)
        {
            if (clientSocket.SocketType == SocketType.Stream)
                return SendBuffer(/*sessionId,*/ buffer, offset, count);
            else if (clientSocket.SocketType == SocketType.Dgram)
                return SendBufferTo(sessionId, buffer, offset, count);
            return false;
        }

        /// <summary>
        /// 接受线程函数(异步)
        /// </summary>
        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                Socket? ts = result.AsyncState as Socket;
                if (ts == null)
                    return;
                int len = ts.EndReceive(result);
                if (len > 0)
                {
                    result.AsyncWaitHandle.Close();
                    RBuffer.Decode(buffer);
                    TriggerSocketInvoke();
                }
                else if (len == 0)
                {
                    // 如果等于0说明断开连接
                    return;
                }
                //清空数据，重新开始异步接收
                int blen = buffer.Length;
                Array.Clear(buffer, 0, blen);
                ts.BeginReceive(buffer, 0, blen, SocketFlags.None, new AsyncCallback(ReceiveCallback), ts);
            }
            catch (Exception ex)
            {
                socketInvoke?.SocketException(ex);
            }
        }

        /// <summary>
        /// 接受线程函数(异步)
        /// </summary>
        private void ReceiveFromCallback(IAsyncResult result)
        {
            try
            {
                Socket? ts = result.AsyncState as Socket;
                if (ts == null)
                    return;
                int len = ts.EndReceive(result);
                if (len > 0)
                {
                    result.AsyncWaitHandle.Close();
                    if ((byte)(buffer[0] + buffer[1] + 0x66) == buffer[2])
                    {
                        ushort sessionId = (ushort)(((buffer[0] & 0xFF) << 8) | (buffer[1] & 0xFF));
                        int dlen = numMaxBufferSize - 3;
                        byte[] data = new byte[dlen];
                        Buffer.BlockCopy(buffer, 3, data, 0, dlen);
                        socketInvoke?.OnReceivedCompleted(new SocketMsg(sessionId, data, this));
                    }
                    // rBuffer.Decode(buffer);
                    // TriggerSocketInvoke();
                }
                else if (len == 0)
                {
                    // 如果等于0说明断开连接
                    return;
                }
                System.Net.EndPoint endPoint = clientSocket.endPoint;
                //清空数据，重新开始异步接收
                int blen = buffer.Length;
                Array.Clear(buffer, 0, blen);
                ts.BeginReceiveFrom(buffer, 0, blen, SocketFlags.None, ref endPoint, new AsyncCallback(ReceiveFromCallback), ts);
            }
            catch (SocketException ex)
            {
                Close();
                socketInvoke?.SocketException(ex);
            }
            catch (IOException ex)
            {
                Close();
                socketInvoke?.SocketException(ex);
            }
            catch (Exception ex)
            {
                socketInvoke?.SocketException(ex);
            }
        }

        /// <summary>
        /// 触发回调委托
        /// </summary>
        private void TriggerSocketInvoke()
        {
            do
            {
                var sb = RBuffer.TakeStreamBuffer();
                if (sb == null) return;
                socketInvoke?.OnReceivedCompleted(new SocketMsg(0, sb, this));
            } while (true);
        }

        /// <summary>
        /// 发送数据
        /// <para>返回 0 为成功 -1 异常</para>
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        /// <returns>0 为成功 -1 异常</returns>
        protected bool SendBuffer(/*ushort sessionId,*/ Span<byte> buffer, int offset, int count)
        {
            try
            {
                Memory<byte> mBuffer = null;
                // 数据打包
                if (offset == 0 && buffer.Length == count)
                {
                    mBuffer = SweetStream.Encode(buffer);
                }
                else if (offset > 0 || buffer.Length < count)
                {
                    mBuffer = SweetStream.Encode(buffer.Slice(offset, count));
                }
                if (mBuffer.IsEmpty)
                    return false;
                var args = sendEventArgs.Pop();
                if (args == null)
                    return false;
#if !UNITY_2020_1_OR_NEWER
                args.SetBuffer(mBuffer);
#else
				args.SetBuffer(mBuffer.ToArray(), 0, mBuffer.Length);
#endif
                var willRaiseEvent = clientSocket.SendAsync(args);
                if (!willRaiseEvent)
                {
                    return ProcessSend(args);
                }
                else return true;
            }
            catch (Exception ex)
            {
                socketInvoke?.SocketException(ex);
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
        protected bool SendBufferTo(ushort sessionId, Span<byte> buffer, int offset, int count)
        {
            // 数据打包
            if (offset > 0 || buffer.Length < count)
            {
                buffer = buffer.Slice(offset, count);
            }

            try
            {
                if (buffer.IsEmpty)
                    return false;
                int len = buffer.Length;
                var args = sendEventArgs.Pop();
                if (args == null)
                    return false;
                int slen = 3 + len;
                byte[] sendBuffer = new byte[slen];
                /* 会话ID 高位 */
                // 1 byte
                sendBuffer[0] = (byte)((sessionId >> 8) & 0xFF);
                /* 会话ID 低位 */
                // 1 byte
                sendBuffer[1] = (byte)((sessionId) & 0xFF);
                /* 数据长度标识 指令验签（0x66混淆参数）*/
                // 1 byte
                sendBuffer[2] = (byte)(sendBuffer[0] + sendBuffer[1] + 0x66);
                Buffer.BlockCopy(buffer.ToArray(), 0, sendBuffer, 3, len);
                args.SetBuffer(sendBuffer, 0, slen);
                var willRaiseEvent = clientSocket.SendToAsync(args);
                if (!willRaiseEvent)
                    return ProcessSend(args);
                else
                    return true;
            }
            catch (Exception ex)
            {
                socketInvoke?.SocketException(ex);
                return false;
            }
        }

        /// <summary>
        /// IO完成端口
        /// </summary>
        /// <param name="e"></param>
        public void IO_Completed(SocketAsyncEventArgs e)
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
        private static bool ProcessSend(SocketAsyncEventArgs e)
        {
            ((MySocketAsyncEventArgsEx)e).Push();
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
        private void BeginReceived()
        {
            try
            {
                //开始异步接收
                clientSocket.BeginReceive(buffer, 0, numMaxBufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
            }
            catch (Exception ex)
            {
                socketInvoke?.SocketException(ex);
            }
        }

        /// <summary>
        /// 开始监听 udp
        /// </summary>
        private void BeginReceivedFrom()
        {
            try
            {
                System.Net.EndPoint endPoint = clientSocket.endPoint;
                //开始异步接收
                clientSocket.BeginReceiveFrom(buffer, 0, numMaxBufferSize, SocketFlags.None, ref endPoint, new AsyncCallback(ReceiveFromCallback), clientSocket);
            }
            catch (Exception ex)
            {
                socketInvoke?.SocketException(ex);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (!isRecving) return;
            isRecving = false;
            sendEventArgs.Destroy();
            Tag = Var.Empty;
            clientSocket.Close();
        }
    }
}
