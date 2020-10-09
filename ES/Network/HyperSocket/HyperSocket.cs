using ES.Common.Time;
using ES.Common.Utils;
using ES.Network.Sockets;
using System;
using System.Linq;
using System.Text;

namespace ES.Network.HyperSocket
{
    /// <summary>
    /// 超级套接字
    /// <para>使用最简单方式来建立服务器或客户端连接</para>
    /// <para>可以使用可靠的TCP通信、UDP[KCP]通信</para>
    /// <para>在连接过程中因为握手是异步处理的，所以需要在接口中才能得到正确的连接对象</para>
    /// <para>如果仅仅是创建了对象后就发送消息等操作是无法正确应答的</para>
    /// </summary>
    public class HyperSocket : BaseTimeFlow
    {
        /// <summary>
        /// 是否为服务器模式
        /// </summary>
        public readonly bool IsServerMode;
        /// <summary>
        /// 配置
        /// </summary>
        public readonly HyperSocketConfig config;
        /// <summary>
        /// IP地址
        /// </summary>
        public readonly string ip;
        /// <summary>
        /// TCP模式监听端口
        /// </summary>
        public uint TcpPort { get; private set; }
        /// <summary>
        /// UDP模式监听端口
        /// </summary>
        public uint UdpPort { get; private set; }
        /// <summary>
        /// 存活状态
        /// </summary>
        public bool IsAlive { get; private set; } = false;

        /// <summary>
        /// 心跳检测周期
        /// </summary>
        private int heartCheckPeriod = 0;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="IsServerMode"></param>
        /// <param name="ip"></param>
        /// <param name="tcpPort"></param>
        /// <param name="udpPort"></param>
        /// <param name="connectMaxNum"></param>
        /// <param name="config"></param>
        private HyperSocket(bool IsServerMode, string ip, uint tcpPort, uint udpPort, uint connectMaxNum, HyperSocketConfig config)
        {
            this.IsServerMode = IsServerMode;

            this.ip = ip;
            TcpPort = tcpPort;
            UdpPort = udpPort;
            if (connectMaxNum < ushort.MaxValue - 1) connectMaxNum += 1;
            this.connectMaxNum = connectMaxNum;
            this.config = config;

            if (this.IsServerMode) remoteSockets = new RemoteHyperSocket[connectMaxNum];
        }

        /// <summary>
        /// 心跳检测
        /// </summary>
        /// <param name="dt"></param>
        protected override void Update(int dt)
        {
            if (IsAlive)
            {
                heartCheckPeriod -= timeFlowPeriod;
                if (heartCheckPeriod <= 0)
                {
                    if (IsServerMode)
                    {
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
                    else
                    {
                        heartCheckPeriod = config.HeartSendPeriod;
                        if (tcpClient.heartCheckTimeOut < DateTime.UtcNow.AddMilliseconds(-config.HeartTimeOut).Ticks) Close();
                        else SendUdp(HeartPingBytes);
                    }
                }
            }
        }

        /// <summary>
        /// 关闭套接字
        /// </summary>
        public void Close()
        {
            if (IsAlive)
            {
                IsAlive = false;
                CloseTimeFlowES();
            }
        }

        /// <summary>
        /// 停止更新
        /// </summary>
        protected override void OnUpdateEnd()
        {
            if (IsServerMode)
            {
                lock (remoteSockets)
                {
                    for (int i = 0, len = remoteSockets.Length; i < len; i++)
                    {
                        var remote = remoteSockets[i];
                        if (remote != null && remote.IsAlive) remote.CloseSocket();
                    }
                }
                if (tcpServer != null) tcpServer.CloseSocket();
                if (udpServer != null) udpServer.CloseSocket();
            }
            else
            {
                if (tcpClient != null) tcpClient.CloseSocket();
                if (udpClient != null) udpClient.CloseSocket();
            }
        }

        #region 服务端逻辑
        /// <summary>
        /// 最大连接数【服务器模式用】
        /// </summary>
        public readonly uint connectMaxNum;

        /// <summary>
        /// TCP连接服务器
        /// </summary>
        internal HyperSocketServerModule tcpServer { get; private set; }
        /// <summary>
        /// UDP连接服务器
        /// </summary>
        internal HyperSocketServerModule udpServer { get; private set; }
        /// <summary>
        /// 服务端监听器
        /// </summary>
        internal IHyperSocketServerListener svrListener;

        /// <summary>
        /// 远程连接
        /// </summary>
        private readonly RemoteHyperSocket[] remoteSockets;

        /// <summary>
        /// 心跳pong字节
        /// </summary>
        internal readonly static byte[] HeartPongBytes = new byte[] { 0x01, 0x00, 0x02 };

        /// <summary>
        /// 创建一个服务器超级套接字
        /// </summary>
        /// <param name="ip">监听地址</param>
        /// <param name="port">监听端口 TCP/UDP共用相同端口</param>
        /// <param name="connectMaxNum">允许最大连接数 最大为65534个连接数</param>
        /// <param name="listener">监听器</param>
        /// <param name="config">配置</param>
        /// <returns></returns>
        public static HyperSocket CreateServer(string ip, uint port, uint connectMaxNum, IHyperSocketServerListener listener, HyperSocketConfig config = null)
        {
            return CreateServer(ip, port, port, connectMaxNum, listener, config);
        }

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
        public static HyperSocket CreateServer(string ip, uint tcpPort, uint udpPort, uint connectMaxNum, IHyperSocketServerListener listener, HyperSocketConfig config = null)
        {
            if (config == null) config = new HyperSocketConfig();
            if (connectMaxNum > ushort.MaxValue - 1) connectMaxNum = ushort.MaxValue - 1;

            var hyperSocket = new HyperSocket(true, ip, tcpPort, udpPort, connectMaxNum, config);

            hyperSocket.tcpServer = new HyperSocketServerModule(ip, (int)tcpPort, (int)connectMaxNum, (int)config.TcpReceiveSize, hyperSocket);
            hyperSocket.udpServer = new HyperSocketServerModule(ip, (int)udpPort, (int)connectMaxNum, (int)config.UdpReceiveSize, hyperSocket);

            hyperSocket.svrListener = listener;

            hyperSocket.tcpServer.SetListener(listener);
            hyperSocket.udpServer.SetListener(listener);

            var r1 = hyperSocket.tcpServer.Init(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, 0, hyperSocket.tcpServer);
            var r2 = hyperSocket.udpServer.Init(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp, 0, hyperSocket.udpServer);

            if (r1 && r2)
            {
                hyperSocket.IsAlive = true;
                hyperSocket.StartTimeFlow();
                return hyperSocket;
            }
            else return null;
        }

        /// <summary>
        /// 生成认证通道
        /// </summary>
        /// <returns></returns>
        internal byte[] GenerateVerifyConnection(out int sessionId)
        {
            try
            {
                sessionId = GetUnusedSocketIndex();
                if (sessionId > ushort.MinValue)
                {
                    // 先加入验证
                    RemoteHyperSocket remote = new RemoteHyperSocket((ushort)sessionId, this, config);
                    SetSocketAtIndex(sessionId, remote);
                    // 返回验证码
                    long secondTicks = DateTime.UtcNow.ToSecondTicks();
                    // 刚建立连接
                    string nativeCode = RandomCode.Generate(8, RandomCode.RandomCodeType.HighLetterAndNumber, (int)secondTicks);
                    byte[] data = new byte[10];
                    Array.Copy(Encoding.UTF8.GetBytes(nativeCode), data, 8);
                    data[8] = (byte)((UdpPort >> 8) & 0xFF);
                    data[9] = (byte)((UdpPort) & 0xFF);
                    return data;
                }
            }
            catch (Exception ex)
            {
                svrListener.OnError(ex);
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
        internal void SetSocketAtIndex(int index, RemoteHyperSocket socket)
        {
            lock (remoteSockets) remoteSockets[index] = socket;
        }

        /// <summary>
        /// 获取远程套接字
        /// </summary>
        /// <returns></returns>
        internal RemoteHyperSocket GetSocketAtIndex(int index)
        {
            lock (remoteSockets) return remoteSockets[index];
        }

        #endregion

        #region 客户端逻辑
        /// <summary>
        /// TCP连接客户端
        /// </summary>
        private HyperSocketClientModule tcpClient;
        /// <summary>
        /// UDP连接客户端
        /// </summary>
        private HyperSocketClientModule udpClient;
        /// <summary>
        /// 客户端监听器
        /// </summary>
        private IHyperSocketClientListener cntListener;

        /// <summary>
        /// 心跳ping字节
        /// </summary>
        internal readonly static byte[] HeartPingBytes = new byte[] { 0x01, 0x00, 0x01 };
        /// <summary>
        /// 客户端连接成功字节
        /// </summary>
        internal readonly static byte[] ConnectedClientBytes = new byte[] { 0x01, 0x00, 0x03 };

        /// <summary>
        /// 客户端会话ID
        /// </summary>
        public ushort SessionId { get; private set; } = 0;

        /// <summary>
        /// 获取有效性
        /// </summary>
        internal bool IsValid { get; private set; } = false;

        /// <summary>
        /// 创建一个客户端超级套接字
        /// <para>客户端会尝试连接3次，如果3次都失败则触发连接失败回调</para>
        /// </summary>
        /// <param name="ip">连接地址</param>
        /// <param name="port">连接端口</param>
        /// <param name="listener">监听器</param>
        /// <param name="config">配置</param>
        /// <returns></returns>
        public static HyperSocket CreateClient(string ip, uint port, IHyperSocketClientListener listener, HyperSocketConfig config = null)
        {
            try
            {
                if (config == null) config = new HyperSocketConfig();
                var hyperSocket = new HyperSocket(false, ip, port, 0, 0, config);
                hyperSocket.cntListener = listener;
                // 开始发起连接
                hyperSocket.tcpClient = new HyperSocketClientModule(ip, (int)port, (int)config.TcpReceiveSize, ProtocolType.Tcp, hyperSocket);
                hyperSocket.tcpClient.SetListener(listener);
                hyperSocket.tcpClient.Init(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, hyperSocket.tcpClient);
                // 发送验证标签
                hyperSocket.tcpClient.Send(new byte[] { 0x01, 0x02 });

                return hyperSocket;
            }
            catch (Exception ex)
            {
                listener.OnError(null, ex);
                return null;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void InitializeUdpClient(byte[] data, ushort sessionId)
        {
            try
            {
                long secondTicks = DateTime.UtcNow.ToSecondTicks();
                // 刚建立连接
                string nativeCode = RandomCode.Generate(8, RandomCode.RandomCodeType.HighLetterAndNumber, (int)secondTicks);
                if (Encoding.UTF8.GetString(data.Take(8).ToArray()) == nativeCode)
                {
                    SessionId = sessionId;
                    ushort udpPort = (ushort)(((data[8] & 0xFF) << 8) | (data[9] & 0xFF));
                    UdpPort = udpPort;
                    udpClient = new HyperSocketClientModule(ip, udpPort, (int)config.UdpReceiveSize, ProtocolType.Udp, this);
                    udpClient.SetListener(cntListener);
                    // 返回验证UDP连接
                    if (udpClient.Init(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp, udpClient))
                    {
                        long verifyCode = sessionId * (secondTicks / 100);
                        udpClient.SendKcp(Encoding.UTF8.GetBytes(verifyCode.ToString()));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                cntListener.OnError(this, ex);
            }
            // 没有进入最内部逻辑则直接关闭
            Close();
        }

        /// <summary>
        /// 验证服务器数据
        /// </summary>
        internal void VerifyServerData(byte[] data)
        {
            if (Encoding.UTF8.GetString(data) == DateTime.UtcNow.ToSecondTicks().ToString())
            {
                IsValid = true;
                IsAlive = true;
                StartTimeFlow();
                SendTcp(ConnectedClientBytes);
            }
            else Close();
        }

        /// <summary>
        /// 通过TCP发送数据
        /// </summary>
        /// <param name="data"></param>
        public bool SendTcp(byte[] data)
        {
            if (IsValid && data != null) return tcpClient.Send(SessionId, data);
            else return false;
        }

        /// <summary>
        /// 通过UDP发送数据[KCP]
        /// </summary>
        /// <param name="data"></param>
        public void SendUdp(byte[] data)
        {
            if (IsValid) udpClient.SendKcp(data);
        }

        /// <summary>
        /// 通过TCP发送数据
        /// </summary>
        /// <param name="dataStr"></param>
        public void SendTcp(string dataStr)
        {
            SendTcp(Encoding.UTF8.GetBytes(dataStr));
        }

        /// <summary>
        /// 通过UDP发送数据[KCP]
        /// </summary>
        /// <param name="dataStr"></param>
        public void SendUdp(string dataStr)
        {
            if (IsValid) udpClient.SendKcp(Encoding.UTF8.GetBytes(dataStr));
        }
        #endregion
    }
}
