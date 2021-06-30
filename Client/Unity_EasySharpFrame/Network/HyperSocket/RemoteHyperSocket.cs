using ES.Network.Sockets.Server;
using System;
using System.Text;

namespace ES.Network.HyperSocket
{
    /// <summary>
    /// 远程超级套接字
    /// </summary>
    public class RemoteHyperSocket : IKcpListener
    {
        /// <summary>
        /// kcp协议模块
        /// </summary>
        private readonly KcpHelper kcpHelper;
        /// <summary>
        /// 会话ID
        /// </summary>
        public ushort SessionId { get; private set; } = 0;

        internal RemoteConnection tcpConn;
        internal RemoteConnection udpConn;
        private WeakReference<HyperSocket> hyperSocketRef;
        /// <summary>
        /// 在线状态
        /// </summary>
        public bool IsAlive { internal set; get; } = false;
        /// <summary>
        /// 是否认证
        /// </summary>
        internal bool isValid = false;
        /// <summary>
        /// 是否安全连接 启用安全连接才可以使用
        /// </summary>
        internal bool isSecurityConnected = false;
        /// <summary>
        /// 与远程对象捆绑标记
        /// </summary>
        public object Tag;
        /// <summary>
        /// 心跳检测超时累计
        /// </summary>
        internal long heartCheckTimeOut = DateTime.UtcNow.Ticks;
        /// <summary>
        /// 安全传输协议
        /// </summary>
        internal SSL ssl;

        /// <summary>
        /// ip地址
        /// </summary>
        internal string ip;
        /// <summary>
        /// tcp端口
        /// </summary>
        internal int tcpPort;
        /// <summary>
        /// udp端口
        /// </summary>
        internal int udpPort;

        internal RemoteHyperSocket(ushort sessionId, HyperSocket hyperSocket, HyperSocketConfig config)
        {
            if (config.UseSSL) ssl = new SSL(SSL.SSLMode.AES);
            hyperSocketRef = new WeakReference<HyperSocket>(hyperSocket);
            SessionId = sessionId;
            kcpHelper = new KcpHelper(sessionId, (int)config.UdpReceiveSize, config.KcpWinSize, config.kcpMode, this);
        }

        /// <summary>
        /// 发送数据 TCP
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendTcp(byte[] data)
        {
            if (IsAlive && isValid && data != null)
            {
                if (hyperSocketRef.TryGetTarget(out var hyperSocket) && hyperSocket.config.UseSSL && (hyperSocket.config.SSLMode == 0 || hyperSocket.config.SSLMode == 1)) return tcpConn.Send(SessionId, ssl.AESEncrypt(data));
                else return tcpConn.Send(SessionId, data);
            }
            else return false;
        }

        /// <summary>
        /// 发送数据 TCP
        /// </summary>
        /// <returns></returns>
        internal bool SendPong()
        {
            System.Threading.Interlocked.Exchange(ref heartCheckTimeOut, DateTime.UtcNow.Ticks);
            return tcpConn.Send(SessionId, HyperSocket.HeartPongBytes);
        }

        /// <summary>
        /// 发送签名 TCP
        /// </summary>
        /// <returns></returns>
        internal bool SendSignData(byte[] signData)
        {
            return tcpConn.Send(SessionId, signData);
        }

        /// <summary>
        /// 发送数据 UDP
        /// </summary>
        /// <param name="data"></param>
        internal void SendKcp(byte[] data)
        {
            kcpHelper.Send(data);
        }

        /// <summary>
        /// 发送数据 UDP
        /// </summary>
        /// <param name="data"></param>
        public void SendUdp(byte[] data)
        {
            if (IsAlive && isValid)
            {
                if (hyperSocketRef.TryGetTarget(out var hyperSocket) && hyperSocket.config.UseSSL && (hyperSocket.config.SSLMode == 0 || hyperSocket.config.SSLMode == 2)) SendKcp(ssl.AESEncrypt(data));
                else SendKcp(data);
            }
        }

        /// <summary>
        /// 发送数据 TCP
        /// </summary>
        /// <param name="dataStr"></param>
        public void SendTcp(string dataStr)
        {
            SendTcp(Encoding.UTF8.GetBytes(dataStr));
        }

        /// <summary>
        /// 发送数据 UDP
        /// </summary>
        /// <param name="dataStr"></param>
        public void SendUdp(string dataStr)
        {
            SendUdp(Encoding.UTF8.GetBytes(dataStr));
        }

        /// <summary>
        /// 内部接受函数 KCP
        /// </summary>
        /// <param name="data"></param>
        public void OnReceive(byte[] data)
        {
            if (hyperSocketRef.TryGetTarget(out var hyperSocket)) hyperSocket.udpServer.KcpDataBackHandle(this, data);
        }

        /// <summary>
        /// 内部发送函数 KCP
        /// </summary>
        /// <param name="data"></param>
        public void OnSend(byte[] data)
        {
            udpConn.Send(SessionId, data);
        }

        /// <summary>
        /// 接受原始数据
        /// </summary>
        /// <param name="data"></param>
        internal void RecvData(byte[] data)
        {
            kcpHelper.Recv(data);
        }

        /// <summary>
        /// 检测是不是一样的远程
        /// </summary>
        /// <returns></returns>
        internal bool CheckSameRemote(RemoteConnection conn)
        {
            if (conn != null && tcpConn != null && tcpConn.socket.endPoint.Address.Equals(conn.socket.endPoint.Address) /*&& tcpConn.socket.endPoint.Port == conn.socket.endPoint.Port*/)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 检测是不是一样的远程
        /// </summary>
        /// <returns></returns>
        internal bool CheckSameRemote(System.Net.EndPoint conn)
        {
            if (conn != null && udpConn != null && udpConn.socket.endPoint.Address.Equals((conn as System.Net.IPEndPoint).Address) /*&& tcpConn.socket.endPoint.Port == conn.socket.endPoint.Port*/)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取远程IP
        /// </summary>
        public string GetRemoteIp()
        {
            return ip;
        }

        /// <summary>
        /// 获取TCP远程端口
        /// </summary>
        public int GetRemoteTcpPort()
        {
            return tcpPort;
        }

        /// <summary>
        /// 获取UDP远程端口
        /// </summary>
        public int GetRemoteUdpPort()
        {
            return udpPort;
        }

        /// <summary>
        /// 关闭套接字
        /// </summary>
        public void CloseSocket()
        {
            if (IsAlive)
            {
                tcpConn.Destroy();
                udpConn.Destroy();
                kcpHelper.CloseKcp();
            }
            if (hyperSocketRef.TryGetTarget(out var hyperSocket))
            {
                if (IsAlive && isValid) hyperSocket.svrListener.OnClose(this);
                else hyperSocket.svrListener.OnError(new Exception("Initialize Connection Fail"));
                hyperSocket.SetSocketAtIndex(SessionId, null);
            }
            IsAlive = false;
            isValid = false;
            Tag = null;
            hyperSocketRef = null;
        }
    }
}
