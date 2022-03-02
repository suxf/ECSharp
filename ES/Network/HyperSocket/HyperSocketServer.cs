using ES.Network.Sockets;
using System;

namespace ES.Network.HyperSocket
{
    /// <summary>
    /// 超级套接字 服务器
    /// <para>使用最简单方式来建立服务器或客户端连接</para>
    /// <para>可以使用可靠的TCP通信、UDP[KCP]通信</para>
    /// <para>在连接过程中因为握手是异步处理的，所以需要在接口中才能得到正确的连接对象</para>
    /// <para>如果仅仅是创建了对象后就发送消息等操作是无法正确应答的</para>
    /// </summary>
    public class HyperSocketServer : BaseHyperSocket
    {

        /// <summary>
        /// 最大连接数
        /// </summary>
        public readonly uint connectMaxNum;

        /// <summary>
        /// TCP连接服务器
        /// </summary>
        internal HyperSocketServerModule TcpServer;
        /// <summary>
        /// UDP连接服务器
        /// </summary>
        internal HyperSocketServerModule UdpServer;
        /// <summary>
        /// 服务端监听器
        /// </summary>
        internal IHyperSocketServer svrListener;

        /// <summary>
        /// 远程连接
        /// </summary>
        internal readonly RemoteHyperSocket?[] remoteSockets;


        /// <summary>
        /// 创建一个服务器超级套接字
        /// </summary>
        /// <param name="ip">监听地址</param>
        /// <param name="port">监听端口 TCP/UDP共用相同端口</param>
        /// <param name="connectMaxNum">允许最大连接数 最大为65534个连接数</param>
        /// <param name="listener">监听器</param>
        /// <param name="config">配置</param>
        /// <returns></returns>
        public HyperSocketServer(string ip, uint port, uint connectMaxNum, IHyperSocketServer listener, HyperSocketConfig? config = null)
        : this(ip, port, port, connectMaxNum, listener, config) { }

        /// <summary>
        /// 创建一个服务器超级套接字
        /// </summary>
        /// <param name="ip">监听地址</param>
        /// <param name="tcpPort">tcp端口</param>
        /// <param name="udpPort">udp端口</param>
        /// <param name="connectMaxNum">允许最大连接数</param>
        /// <param name="listener">监听器</param>
        /// <param name="config">配置</param>
        /// <returns></returns>
        public HyperSocketServer(string ip, uint tcpPort, uint udpPort, uint connectMaxNum, IHyperSocketServer listener, HyperSocketConfig? config = null)
        : base(true, ip, tcpPort, udpPort, connectMaxNum, config)
        {
            // if (connectMaxNum < ushort.MaxValue - 1) connectMaxNum += 1;
            if (connectMaxNum > ushort.MaxValue - 1) connectMaxNum = ushort.MaxValue - 1;
            this.connectMaxNum = connectMaxNum;

            remoteSockets = new RemoteHyperSocket[connectMaxNum];
            ssl = new SSL(SSL.SSLMode.RSA);

            TcpServer = new HyperSocketServerModule(ip, (int)tcpPort, (int)connectMaxNum, (int)this.config.TcpReceiveSize, this, listener);
            UdpServer = new HyperSocketServerModule(ip, (int)udpPort, (int)connectMaxNum, (int)this.config.UdpReceiveSize, this, listener);

            svrListener = listener;
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <returns></returns>
        public HyperSocketServer StartServer()
        {
            var r1 = TcpServer.Init(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, 0, TcpServer);
            var r2 = UdpServer.Init(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp, 0, UdpServer);

            if (r1 && r2)
            {
                IsAlive = true;
                timeFlow.StartTimeFlowES();
            }

            return this;
        }

        /// <summary>
        /// 获取套接字
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public RemoteHyperSocket? GetSocket(ushort session)
        {
            if (0 <= session && session < connectMaxNum)
                return GetSocketAtIndex(session);
            return null;
        }

        /// <summary>
        /// 生成认证通道
        /// </summary>
        /// <returns></returns>
        internal byte[]? GenerateVerifyConnection(out ushort sessionId)
        {
            try
            {
                sessionId = (ushort)GetUnusedSocketIndex();
                if (sessionId > ushort.MinValue)
                {
                    // 先加入验证
                    RemoteHyperSocket remote = new RemoteHyperSocket(sessionId, this, config);
                    SetSocketAtIndex(sessionId, remote);

                    byte[] data = new byte[8];
                    data[0] = (byte)((UdpPort >> 8) & 0xFF);
                    data[1] = (byte)((UdpPort) & 0xFF);
                    data[2] = (byte)((sessionId >> 8) & 0xFF);
                    data[3] = (byte)((sessionId) & 0xFF);
                    // 认证部分
                    data[4] = (byte)(data[0] + data[1]);
                    data[5] = (byte)(data[2] + data[3]);
                    data[6] = (byte)(data[0] + data[3]);
                    data[7] = (byte)(data[1] + data[2]);
                    return data;
                }
            }
            catch (Exception ex)
            {
                svrListener.SocketError(ex);
            }
            sessionId = 0;
            return null;
        }

        /// <summary>
        /// 返回使用的远程套接字数量
        /// </summary>
        /// <returns></returns>
        public int GetUsedSocketCount()
        {
            int count = 0;
            lock (remoteSockets)
            {
                for (int i = 1; i < connectMaxNum; i++)
                {
                    if (remoteSockets[i] != null) count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 返回未使用的远程套接字索引
        /// <para>0 位占用 不使用</para>
        /// </summary>
        /// <returns></returns>
        internal int GetUnusedSocketIndex()
        {
            lock (remoteSockets)
            {
                for (int i = 1; i < connectMaxNum; i++)
                {
                    if (remoteSockets[i] == null) return i;
                }
            }
            return ushort.MinValue;
        }

        /// <summary>
        /// 设置远程套接字
        /// </summary>
        /// <returns></returns>
        internal void SetSocketAtIndex(int index, RemoteHyperSocket? socket)
        {
            lock (remoteSockets) remoteSockets[index] = socket;
        }

        /// <summary>
        /// 获取远程套接字
        /// </summary>
        /// <returns></returns>
        internal RemoteHyperSocket? GetSocketAtIndex(int index)
        {
            lock (remoteSockets) return remoteSockets[index];
        }

        internal override void UpdateHandle(int dt)
        {
            heartCheckPeriod -= dt;
            if (heartCheckPeriod > 0)
                return;
            heartCheckPeriod = config.HeartCheckPeriod;
            lock (remoteSockets)
            {
                for (int i = 0; i < connectMaxNum; i++)
                {
                    var remote = remoteSockets[i];
                    if (remote != null)
                    {
                        if (remote.IsAlive)
                        {
                            if (remote.heartCheckTimeOut < DateTime.UtcNow.AddMilliseconds(-config.HeartTimeOut).Ticks) remote.CloseSocket();
                            else if (remote.IsAlive && !remote.SendPong()) remote.CloseSocket();
                        }
                        else if (!remote.isValid)
                        {
                            if (remote.heartCheckTimeOut < DateTime.UtcNow.AddSeconds(-3).Ticks) remote.CloseSocket();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void UpdateEnd()
        {
            lock (remoteSockets)
            {
                for (int i = 0, len = remoteSockets.Length; i < len; i++)
                {
                    var remote = remoteSockets[i];
                    if (remote != null && remote.IsAlive) remote.CloseSocket();
                }
            }
            TcpServer.CloseSocket();
            UdpServer.CloseSocket();
        }
    }
}
