using ES.Common.Log;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ES.Network.Sockets
{
    /// <summary>
    /// ESF客户端套接字（同步接受）
    /// 注意：主副指令在同步传输中需要双向一样才可以正确同步,而主副指令在客户端是自动生成的，服务器只需透传即可
    /// </summary>
    public class SyncClientSocket : BaseClientSocket
    {
        /// <summary>
        /// 同步线程等待超时时间 ms
        /// 默认 2000ms 超时
        /// </summary>
        protected int outTime;
        /// <summary>
        /// 同步线程等待信号量
        /// </summary>
        protected Semaphore syncRecvSignal;

        /// <summary>
        /// 主指令同步计数
        /// </summary>
        private byte mainCommandRef = 0xFA;
        /// <summary>
        /// 次指令同步计数
        /// </summary>
        private byte secondCommandRef = 0x00;

        /// <summary>
        /// 构造函数
        /// 创建一个同步socket
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口</param>
        /// <param name="numMaxBufferSize">接受数据大小[UDP模式以传送理论最大值/TCP模式以传送最佳合适值]</param>
        /// <param name="outTime">同步超时时间，单位：ms</param>
        public SyncClientSocket(string ip, int port, int numMaxBufferSize, int outTime = 2000) : base(ip, port, numMaxBufferSize)
        {
            this.outTime = outTime;
            syncRecvSignal = new Semaphore(1, 1);
        }

        /// <summary>
        /// 构造函数
        /// 创建一个同步socket
        /// </summary>
        /// <param name="esfSocket">ESF Socket 对象</param>
        /// <param name="numMaxBufferSize">接受数据大小[UDP模式以传送理论最大值/TCP模式以传送最佳合适值]</param>
        /// <param name="outTime">同步超时时间，单位：ms</param>
        public SyncClientSocket(Socket esfSocket, int numMaxBufferSize, int outTime = 2000) : base(esfSocket, numMaxBufferSize)
        {
            this.outTime = outTime;
            syncRecvSignal = new Semaphore(1, 1);
        }

        /// <summary>
        /// 同步初始化套接字 
        /// </summary>
        public bool Init(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            // 启动服务
            bool bSuccess = clientSocket.Connect(addressFamily, socketType, protocolType);
            if (bSuccess)
            {
                // Log.Info("Init Client Connecting!");
                if (socketType == SocketType.Stream)
                    BeginReceived();
                else if (socketType == SocketType.Dgram)
                    BeginReceivedFrom();
            }
            return bSuccess;
        }

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        public byte[] Send(byte[] buffer)
        {
            return Send(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 同步发送数据(utf8字符串数据)
        /// </summary>
        /// <param name="utf8str">数据</param>
        public byte[] Send(string utf8str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(utf8str);
            return Send(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">数据偏移</param>
        /// <param name="count">数据大小</param>
        public byte[] Send(byte[] buffer, int offset, int count)
        {
            // 递增主副指令
            if (++secondCommandRef >= 0xFF)
            {
                secondCommandRef = 0x00;
                if (++mainCommandRef >= 0xFF) mainCommandRef = 0xFA;
            }

            int len = -1;
            if (clientSocket.socketType == SocketType.Stream)
                len = SendBufferSync(mainCommandRef, secondCommandRef, buffer, offset, count);
            else if (clientSocket.socketType == SocketType.Dgram)
                len = SendBufferToSync(mainCommandRef, secondCommandRef, buffer, offset, count);

            if (len > 0)
            {
                int timeout = outTime;// 记录超时时间
                while (true)
                {
                    syncRecvSignal.WaitOne();
                    // 这里做同步委托
                    StreamBuffer sb = rBuffer.FindBufferByCommand(mainCommandRef, secondCommandRef);
                    syncRecvSignal.Release();
                    if (sb != null)
                        return sb.buffer;
                    else
                    {
                        if (timeout-- <= 0) return null;
                        Thread.Sleep(1);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 接受线程函数(同步) tcp
        /// </summary>
        protected override void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                Socket ts = (Socket)result.AsyncState;
                syncRecvSignal.WaitOne();
                int len = ts.EndReceive(result);
                if (len > 0)
                {
                    result.AsyncWaitHandle.Close();
                    rBuffer.Decode(buffer);
                }
                else if (len == 0)
                {
                    // 如果等于0说明断开连接
                    return;
                }
                syncRecvSignal.Release();
                //清空数据，重新开始异步接收
                Array.Clear(buffer, 0, buffer.Length);
                ts.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), ts);
            }
            catch (SocketException ex)
            {
                Close();
                Log.Exception(ex, "SyncClientConnection", "ReceiveCallback", "Socket");
            }
            catch (IOException ex)
            {
                Close();
                Log.Exception(ex, "SyncClientConnection", "ReceiveCallback", "Socket");
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "SyncClientConnection", "ReceiveCallback", "Socket");
            }
        }

        /// <summary>
        /// 接受线程函数(同步) udp
        /// </summary>
        protected override void ReceiveFromCallback(IAsyncResult result)
        {
            try
            {
                Socket ts = (Socket)result.AsyncState;
                syncRecvSignal.WaitOne();
                int len = ts.EndReceive(result);
                if (len > 0)
                {
                    result.AsyncWaitHandle.Close();
                    rBuffer.Decode(buffer);
                }
                else if (len == 0)
                {
                    // 如果等于0说明断开连接
                    return;
                }
                syncRecvSignal.Release();
                System.Net.EndPoint endPoint = clientSocket.endPoint;
                //清空数据，重新开始异步接收
                Array.Clear(buffer, 0, buffer.Length);
                ts.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endPoint, new AsyncCallback(ReceiveFromCallback), ts);
            }
            catch (SocketException ex)
            {
                Close();
                Log.Exception(ex, "SyncClientConnection", "ReceiveFromCallback", "Socket");
            }
            catch (IOException ex)
            {
                Close();
                Log.Exception(ex, "SyncClientConnection", "ReceiveFromCallback", "Socket");
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "SyncClientConnection", "ReceiveFromCallback", "Socket");
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            base.Close();
            syncRecvSignal.Release(1);
            syncRecvSignal.Dispose();
        }
    }
}
