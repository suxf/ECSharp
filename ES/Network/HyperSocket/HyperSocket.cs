using ES.Linq;
using ES.Network.Sockets;
using System;
using System.Linq;
using System.Text;

namespace ES.Network.HyperSocket
{
    /// <summary>
    /// 超级套接字 客户端
    /// <para>使用最简单方式来建立服务器或客户端连接</para>
    /// <para>可以使用可靠的TCP通信、UDP[KCP]通信</para>
    /// <para>在连接过程中因为握手是异步处理的，所以需要在接口中才能得到正确的连接对象</para>
    /// <para>如果仅仅是创建了对象后就发送消息等操作是无法正确应答的</para>
    /// </summary>
    public class HyperSocket : BaseHyperSocket
    {
        /// <summary>
        /// TCP连接客户端
        /// </summary>
        internal HyperSocketClientModule tcpClient;
        /// <summary>
        /// UDP连接客户端
        /// </summary>
        internal HyperSocketClientModule? udpClient;
        /// <summary>
        /// 客户端监听器
        /// </summary>
        protected IHyperSocketClient cntListener;

        /// <summary>
        /// 客户端会话ID
        /// </summary>
        public ushort SessionId = 0;

        /// <summary>
        /// 获取有效性
        /// </summary>
        internal bool IsValid = false;

        /// <summary>
        /// 是否安全连接 启用安全连接才可以使用
        /// </summary>
        internal bool isSecurityConnected = false;


        /// <summary>
        /// 创建一个客户端超级套接字
        /// <para>客户端会尝试连接3次，如果3次都失败则触发连接失败回调</para>
        /// </summary>
        /// <param name="ip">连接地址</param>
        /// <param name="port">连接端口</param>
        /// <param name="listener">监听器</param>
        /// <param name="config">配置</param>
        /// <returns></returns>
        public HyperSocket(string ip, uint port, IHyperSocketClient listener, HyperSocketConfig? config = null)
        : base(false, ip, port, 0, 0, config)
        {
            cntListener = listener;
            // 开始发起连接
            tcpClient = new HyperSocketClientModule(ip, (int)port, (int)this.config.TcpReceiveSize, ProtocolType.Tcp, this, listener);
        }

        /// <summary>
        /// 开启连接
        /// </summary>
        public HyperSocket Connect()
        {
            tcpClient.Init(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, tcpClient);
            // 发送验证标签
            tcpClient.Send(FirstConnectBytes);
            return this;
        }

        /// <summary>
        /// 通过TCP发送数据
        /// </summary>
        /// <param name="data"></param>
        public bool SendTcp(byte[] data)
        {
            if (IsValid && data != null)
            {
                if (config.UseSSL && (config.SSLMode == 0 || config.SSLMode == 1)) return tcpClient.Send(SessionId, ssl!.AESEncrypt(data));
                else return tcpClient.Send(SessionId, data);
            }
            else return false;
        }

        /// <summary>
        /// 通过UDP发送数据[KCP]
        /// </summary>
        /// <param name="data"></param>
        public void SendUdp(byte[] data)
        {
            if (IsValid)
            {
                if (config.UseSSL && (config.SSLMode == 0 || config.SSLMode == 2)) udpClient!.SendKcp(ssl!.AESEncrypt(data));
                else udpClient!.SendKcp(data);
            }
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
            SendUdp(Encoding.UTF8.GetBytes(dataStr));
        }

        /// <summary>
        /// 验证服务器数据
        /// </summary>
        internal void VerifyServerData(byte[] data)
        {
            var str = Encoding.UTF8.GetString(data);
            if (str.ElementAt(0) == '0')
            {
                IsValid = true;
                IsAlive = true;
                timeFlow.StartTimeFlowES();
                SendTcp(ConnectedClientBytes);
            }
            else if (str.ElementAt(0) == '1')
            {
                IsValid = true;
                IsAlive = true;
                var publicKey = str.Substring(2);
                config.UseSSL = true;
                config.SSLMode = int.Parse(str.ElementAt(1).ToString());
                ssl = new SSL(SSL.SSLMode.Both, publicKey);
                tcpClient.Send(SessionId, ssl.RSAEncrypt(ssl.GetAESKey().AsBytes()));
            }
            else Close();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void InitializeUdpClient(byte[] data)
        {
            try
            {
                if (data.Length == 8 && data[4] == (byte)(data[0] + data[1]) && data[5] == (byte)(data[2] + data[3]) && data[6] == (byte)(data[0] + data[3]) && data[7] == (byte)(data[1] + data[2]))
                {
                    ushort udpPort = (ushort)(((data[0] & 0xFF) << 8) | (data[1] & 0xFF));
                    SessionId = (ushort)(((data[2] & 0xFF) << 8) | (data[3] & 0xFF));
                    UdpPort = udpPort;
                    udpClient = new HyperSocketClientModule(ip, udpPort, (int)config.UdpReceiveSize, ProtocolType.Udp, (HyperSocket)this, cntListener);
                    // 返回验证UDP连接
                    if (udpClient.Init(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp, udpClient))
                    {
                        long verifyCode = SessionId * (udpPort / 10);
                        udpClient.SendKcp(Encoding.UTF8.GetBytes(verifyCode.ToString()));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                cntListener.SocketError(this, ex);
            }
            // 没有进入最内部逻辑则直接关闭
            Close();
        }

        internal override void UpdateHandle(int dt)
        {
            heartCheckPeriod -= dt;
            if (heartCheckPeriod > 0)
                return;
            heartCheckPeriod = config.HeartSendPeriod;
            if (tcpClient.heartCheckTimeOut < DateTime.UtcNow.AddMilliseconds(-config.HeartTimeOut).Ticks) Close();
            else SendUdp(HeartPingBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void UpdateEnd()
        {
            tcpClient.CloseSocket();
            udpClient?.CloseSocket();
        }
    }
}
