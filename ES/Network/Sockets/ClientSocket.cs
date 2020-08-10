using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ES.Network.Sockets
{
    /// <summary>
    /// ESF客户端套接字（异步接受）
    /// </summary>
    public class ClientSocket : BaseClientSocket
    {

        /// <summary>
        /// 构造函数
        /// 创建一个异步socket
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
        public bool Init(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, SocketInvoke socketInvoke)
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
        public int Send(byte[] buffer)
        {
            return Send(0x01, 0xff, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据(utf8字符串数据)
        /// </summary>
        /// <param name="utf8str">数据</param>
        public int Send(string utf8str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(utf8str);
            return Send(0x01, 0xff, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="main">主指令</param>
        /// <param name="second">副指令</param>
        /// <param name="buffer">数据</param>
        public int Send(byte main, byte second, byte[] buffer)
        {
            return Send(main, second, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据(utf8字符串数据)
        /// </summary>
        /// <param name="main">主指令</param>
        /// <param name="second">副指令</param>
        /// <param name="utf8str">数据</param>
        public int Send(byte main, byte second, string utf8str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(utf8str);
            return Send(main, second, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        public int Send(byte[] buffer, int offset, int count)
        {
            return Send(0x01, 0xff, buffer, offset, count);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="main">主指令</param>
        /// <param name="second">副指令</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        public int Send(byte main, byte second, byte[] buffer, int offset, int count)
        {
            if (clientSocket.socketType == SocketType.Stream)
                return SendBuffer(main, second, buffer, offset, count);
            else if (clientSocket.socketType == SocketType.Dgram)
                return SendBufferTo(main, second, buffer, offset, count);
            return -2;
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
                    rBuffer.Decode(buffer);
                    TriggerSocketInvoke();
                }
                else if (len == 0)
                {
                    // 如果等于0说明断开连接
                    return;
                }
                //清空数据，重新开始异步接收
                Array.Clear(buffer, 0, buffer.Length);
                ts.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), ts);
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
                    rBuffer.Decode(buffer);
                    TriggerSocketInvoke();
                }
                else if (len == 0)
                {
                    // 如果等于0说明断开连接
                    return;
                }
                System.Net.EndPoint endPoint = clientSocket.endPoint;
                //清空数据，重新开始异步接收
                Array.Clear(buffer, 0, buffer.Length);
                ts.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endPoint, new AsyncCallback(ReceiveFromCallback), ts);
            }
            catch (SocketException ex)
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
            StreamBuffer sb = rBuffer.TakeStreamBuffer();
            while (sb != null)
            {
                if (socketInvoke != null)
                    socketInvoke.ReceivedCompleted(new SocketMsg(sb.main, sb.second, sb.buffer, this));
                // 提取下一个
                sb = rBuffer.TakeStreamBuffer();
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
