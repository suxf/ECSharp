using ES.Common.Time;
using ES.Network.Sockets;
using System;
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
    public abstract class BaseHyperSocket : ITimeUpdate
    {
        /// <summary>
        /// 心跳pong字节
        /// </summary>
        internal readonly static byte[] HeartPongBytes = new byte[] { 0x01, 0x00, 0x02 };
        /// <summary>
        /// 心跳ping字节
        /// </summary>
        internal readonly static byte[] HeartPingBytes = new byte[] { 0x01, 0x00, 0x01 };
        /// <summary>
        /// 客户端连接成功字节
        /// </summary>
        internal readonly static byte[] ConnectedClientBytes = new byte[] { 0x01, 0x00, 0x03 };
        /// <summary>
        /// 初次握手
        /// </summary>
        internal readonly static byte[] FirstConnectBytes = new byte[] { 0x01, 0x02, 0x01 };
        /// <summary>
        /// 签名握手
        /// </summary>
        internal readonly static byte[] SignSecurityBytes = new byte[] { 0x01, 0x02, 0x02 };


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
        public uint UdpPort { get; internal set; }
        /// <summary>
        /// 存活状态
        /// </summary>
        public bool IsAlive = false;

        /// <summary>
        /// 心跳检测周期
        /// </summary>
        internal int heartCheckPeriod = 0;

        /// <summary>
        /// SSL传输协议
        /// </summary>
        internal SSL? ssl;

        internal readonly BaseTimeFlow timeFlow;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="IsServerMode"></param>
        /// <param name="ip"></param>
        /// <param name="tcpPort"></param>
        /// <param name="udpPort"></param>
        /// <param name="connectMaxNum"></param>
        /// <param name="config"></param>
        internal BaseHyperSocket(bool IsServerMode, string ip, uint tcpPort, uint udpPort, uint connectMaxNum, HyperSocketConfig? config)
        {
            this.IsServerMode = IsServerMode;

            this.ip = ip;
            TcpPort = tcpPort;
            UdpPort = udpPort;
            
            if (config == null) config = new HyperSocketConfig();
            this.config = config;

            timeFlow = BaseTimeFlow.CreateTimeFlow(this);
        }

        /// <summary>
        /// 心跳检测
        /// </summary>
        /// <param name="dt"></param>
        public void Update(int dt)
        {
            if (IsAlive) UpdateHandle(dt);
        }

        internal abstract void UpdateHandle(int dt);

        /// <summary>
        /// 关闭套接字
        /// </summary>
        public void Close()
        {
            if (IsAlive)
            {
                IsAlive = false;
                timeFlow.CloseTimeFlowES();
            }
        }

        /// <summary>
        /// 停止更新
        /// </summary>
        public abstract void UpdateEnd();
    }
}
