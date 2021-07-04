using System;
using System.IO;
using System.Text;

namespace ES.Network.Sockets.Client
{
    /// <summary>
    /// ESF客户端套接字（异步接受）
    /// </summary>
    public class ClientSocket : BaseClientSocket
    {

        /// <summary>
        /// 构造函数
        /// <para>创建一个异步socket</para>
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口</param>
        /// <param name="numMaxBufferSize">接受数据大小[UDP模式以传送理论最大值/TCP模式以传送最佳合适值]</param>
        public ClientSocket(string ip, int port, int numMaxBufferSize) : base(ip, port, numMaxBufferSize) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="esfSocket">ESFSocket对象</param>
        /// <param name="numMaxBufferSize">接受数据大小[UDP模式以传送理论最大值/TCP模式以传送最佳合适值]</param>
        public ClientSocket(Socket esfSocket, int numMaxBufferSize) : base(esfSocket, numMaxBufferSize) { }

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
        public bool Send(byte[] buffer)
        {
            return Send(0x00, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据(utf8字符串数据)
        /// </summary>
        /// <param name="utf8str">数据</param>
        public bool Send(string utf8str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(utf8str);
            return Send(0x00, buffer, 0, buffer.Length);
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
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        public bool Send(ushort sessionId, byte[] buffer, int offset, int count)
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
        protected override void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                Socket ts = (Socket)result.AsyncState;
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
                Array.Clear(buffer, 0, buffer.Length);
                ts.BeginReceive(buffer, 0, buffer.Length, System.Net.Sockets.SocketFlags.None, new AsyncCallback(ReceiveCallback), ts);
            }
            catch (Exception ex)
            {
                // Log.Exception(ex, "ClientConnection", "ReceiveCallback", "Socket");
                socketInvoke.SocketException(ex);
            }
        }

        /// <summary>
        /// 接受线程函数(异步)
        /// </summary>
        protected override void ReceiveFromCallback(IAsyncResult result)
        {
            try
            {
                Socket ts = (Socket)result.AsyncState;
                int len = ts.EndReceive(result);
                if (len > 0)
                {
                    result.AsyncWaitHandle.Close();
                    if ((byte)(buffer[0] + buffer[1] + 0x66) == buffer[2])
                    {
                        ushort sessionId = (ushort)(((buffer[0] & 0xFF) << 8) | (buffer[1] & 0xFF));
                        byte[] data = new byte[buffer.Length - 3];
                        Buffer.BlockCopy(buffer, 3, data, 0, data.Length);
                        if (socketInvoke != null) socketInvoke.OnReceivedCompleted(new SocketMsg(sessionId, data, this));
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
                Array.Clear(buffer, 0, buffer.Length);
                ts.BeginReceiveFrom(buffer, 0, buffer.Length, System.Net.Sockets.SocketFlags.None, ref endPoint, new AsyncCallback(ReceiveFromCallback), ts);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Close();
                // Log.Exception(ex, "ClientConnection", "ReceiveFromCallback", "Socket");
                socketInvoke.SocketException(ex);
            }
            catch (IOException ex)
            {
                Close();
                // Log.Exception(ex, "ClientConnection", "ReceiveFromCallback", "Socket");
                socketInvoke.SocketException(ex);
            }
            catch (Exception ex)
            {
                // Log.Exception(ex, "ClientConnection", "ReceiveFromCallback", "Socket");
                socketInvoke.SocketException(ex);
            }
        }

        /// <summary>
        /// 触发回调委托
        /// </summary>
        protected override void TriggerSocketInvoke()
        {
            var sb = RBuffer.TakeStreamBuffer();
            while (sb != null)
            {
                if (socketInvoke != null)
                    socketInvoke.OnReceivedCompleted(new SocketMsg(0, sb, this));
                // 提取下一个
                sb = RBuffer.TakeStreamBuffer();
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            base.Close();
        }
    }
}
