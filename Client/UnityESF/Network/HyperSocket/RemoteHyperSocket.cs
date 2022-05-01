using ES.Network.Sockets.Server;
using ES.Utils;
using ES.Variant;
using System;
using System.Text;

namespace ES.Network.HyperSocket
{
    /// <summary>
    /// 远程超级套接字
    /// </summary>
    public class RemoteHyperSocket : IKcp
    {
        /// <summary>
        /// kcp协议模块
        /// </summary>
        private readonly KcpHelper kcpHelper;
        /// <summary>
        /// 会话ID
        /// </summary>
        public ushort SessionId { get; private set; } = 0;

        internal RemoteConnection? tcpConn;
        internal RemoteConnection? udpConn;
        private readonly HyperSocketServer server;
        /// <summary>
        /// 在线状态
        /// </summary>
        public bool IsAlive { internal set; get; } = true;
        /// <summary>
        /// 是否认证
        /// </summary>
        internal bool isValid = false;
        /// <summary>
        /// 是否正在连接
        /// </summary>
        internal bool isConnecting = false;
        /// <summary>
        /// 是否安全连接 启用安全连接才可以使用
        /// </summary>
        internal bool isSecurityConnected = false;
        /// <summary>
        /// 与远程对象捆绑标记
        /// </summary>
        public Var Tag = Var.Empty;
        /// <summary>
        /// 心跳检测超时累计
        /// </summary>
        internal long heartCheckTimeOut = Time.TimeFlowManager.TotalRunTime;

        internal AesCrypto? aes;

        /// <summary>
        /// ip地址
        /// </summary>
        internal string ip = "";
        /// <summary>
        /// tcp端口
        /// </summary>
        internal int tcpPort;
        /// <summary>
        /// udp端口
        /// </summary>
        internal int udpPort;

        internal RemoteHyperSocket(ushort sessionId, HyperSocketServer hyperSocketServer, HyperSocketConfig config)
        {
            if (config.UseSSL) aes = new AesCrypto();
            server = hyperSocketServer;
            SessionId = sessionId;
            kcpHelper = new KcpHelper(sessionId, (int)config.UdpReceiveSize, config.KcpWinSize, config.kcpMode, this);
        }

        /// <summary>
        /// 发送数据 TCP
        /// </summary>
        /// <returns></returns>
        internal bool SendPong()
        {
            if (tcpConn == null) return false;
            System.Threading.Interlocked.Exchange(ref heartCheckTimeOut, Time.TimeFlowManager.TotalRunTime);
            return tcpConn.Send(SessionId, BaseHyperSocket.HeartPongBytes);
        }

        /// <summary>
        /// 发送签名 TCP
        /// </summary>
        /// <returns></returns>
        internal bool SendSignData(ReadOnlySpan<byte> signData)
        {
            if (tcpConn == null) return false;
            return tcpConn.Send(SessionId, signData);
        }

        /// <summary>
        /// 发送数据 UDP
        /// </summary>
        /// <param name="data"></param>
        internal void SendKcp(Span<byte> data)
        {
            kcpHelper.Send(data);
        }

        /// <summary>
        /// 发送数据 TCP
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendTcp(byte[] data)
        {
            if (IsAlive && isValid)
            {
                if (aes != null
                    && (server.config.SSLMode == 0 || server.config.SSLMode == 1))
                {
                    return tcpConn?.Send(SessionId, aes.Encrypt(data)) ?? false;
                }
                else
                {
                    return tcpConn?.Send(SessionId, data) ?? false;
                }
            }
            else return false;
        }

        /// <summary>
        /// 发送数据 UDP
        /// </summary>
        /// <param name="data"></param>
        public void SendUdp(byte[] data)
        {
            if (IsAlive && isValid)
            {
                if (aes != null
                    && (server.config.SSLMode == 0 || server.config.SSLMode == 2))
                {
                    SendKcp(aes.Encrypt(data));
                }
                else
                {
                    SendKcp(data);
                }
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
        /// 通过TCP发送数据
        /// </summary>
        /// <param name="list"></param>
        public void SendTcp(VarList list)
        {
            SendTcp(list.GetBytes());
        }

        /// <summary>
        /// 通过UDP发送数据[KCP]
        /// </summary>
        /// <param name="list"></param>
        public void SendUdp(VarList list)
        {
            SendUdp(list.GetBytes());
        }

        /// <summary>
        /// 通过TCP发送数据
        /// </summary>
        /// <param name="map"></param>
        public void SendTcp(VarMap map)
        {
            SendTcp(map.GetBytes());
        }

        /// <summary>
        /// 通过UDP发送数据[KCP]
        /// </summary>
        /// <param name="map"></param>
        public void SendUdp(VarMap map)
        {
            SendUdp(map.GetBytes());
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="isTcpMode">默认tcp模式，否则udp模式</param>
        public void Send(byte[] data, bool isTcpMode = true)
        {
            if (isTcpMode) SendTcp(data);
            else SendUdp(data);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="dataStr">数据</param>
        /// <param name="isTcpMode">默认tcp模式，否则udp模式</param>
        public void Send(string dataStr, bool isTcpMode = true)
        {
            if (isTcpMode) SendTcp(dataStr);
            else SendUdp(dataStr);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="list">数据</param>
        /// <param name="isTcpMode">默认tcp模式，否则udp模式</param>
        public void Send(VarList list, bool isTcpMode = true)
        {
            if (isTcpMode) SendTcp(list);
            else SendUdp(list);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="map">数据</param>
        /// <param name="isTcpMode">默认tcp模式，否则udp模式</param>
        public void Send(VarMap map, bool isTcpMode = true)
        {
            if (isTcpMode) SendTcp(map);
            else SendUdp(map);
        }

        /// <summary>
        /// 内部接受函数 KCP
        /// </summary>
        /// <param name="data"></param>
        public void OnReceive(byte[] data)
        {
            server.UdpServer.KcpDataBackHandle(this, data);
        }

        /// <summary>
        /// 内部发送函数 KCP
        /// </summary>
        /// <param name="data"></param>
        public void OnSend(Span<byte> data)
        {
            udpConn?.Send(SessionId, data);
        }

        /// <summary>
        /// 接受原始数据
        /// </summary>
        /// <param name="data"></param>
        internal void RecvData(Span<byte> data)
        {
            kcpHelper.Recv(data);
        }

        /// <summary>
        /// 检测是不是一样的远程
        /// </summary>
        /// <returns></returns>
        internal bool CheckSameRemote(RemoteConnection conn)
        {
            if (conn != null
                && tcpConn != null
                && tcpConn.Socket!.endPoint.Address.Equals(conn.Socket!.endPoint.Address)
                /*&& tcpConn.socket.endPoint.Port == conn.socket.endPoint.Port*/)
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
            if (conn != null
                && udpConn != null
                && udpConn.Socket != null
                && udpConn.Socket.endPoint.Address.Equals(((System.Net.IPEndPoint)conn).Address)
                /*&& tcpConn.socket.endPoint.Port == conn.socket.endPoint.Port*/)
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
            if (!IsAlive) return;

            if (tcpConn != null)
            {
                tcpConn.isVaildHyperSocket = false;
                tcpConn.Destroy();
            }
            udpConn?.Destroy();
            kcpHelper.CloseKcp();
            if (IsAlive && isValid) server.svrListener.OnClose(this);
            else server.svrListener.SocketError(this, new Exception("Initialize Connection Fail"));
            server.SetSocketAtIndex(SessionId, null);
            IsAlive = false;
            isValid = false;
            isConnecting = false;
            Tag = Var.Empty;
        }
    }
}
