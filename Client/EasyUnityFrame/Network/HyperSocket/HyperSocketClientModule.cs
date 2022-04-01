using ES.Linq;
using ES.Network.Sockets;
using ES.Network.Sockets.Client;
using System;
using System.Threading;

namespace ES.Network.HyperSocket
{
    /// <summary>
    /// 超级套接字客户端模块
    /// </summary>
    internal class HyperSocketClientModule : ClientSocket, ISocket, IKcp
    {
        private readonly HyperSocket hyperSocket;
        /// <summary>
        /// 监听器
        /// </summary>
        private readonly IHyperSocketClient listener;
        /// <summary>
        /// kcp协议模块
        /// </summary>
        private readonly KcpHelper? kcpHelper;

        /// <summary>
        /// 心跳检测超时累计
        /// </summary>
        internal long heartCheckTimeOut = DateTime.UtcNow.Ticks;
        /// <summary>
        /// 第一次接受到pong消息
        /// </summary>
        private bool hasFirstRecvPong = false;

        internal HyperSocketClientModule(string ip, int port, int num, ProtocolType protocolType, HyperSocket hyperSocket, IHyperSocketClient listener) : base(ip, port, num)
        {
            this.hyperSocket = hyperSocket;
            this.listener = listener;
            if (protocolType == ProtocolType.Udp) kcpHelper = new KcpHelper(hyperSocket.SessionId, (int)hyperSocket.config.UdpReceiveSize, hyperSocket.config.KcpWinSize, hyperSocket.config.kcpMode, this);
        }

        internal void SendKcp(Span<byte> data)
        {
            kcpHelper?.Send(data);
        }

        internal void SendConnectData()
        {
            // 发送验证标签
            // 通信连接步骤<一> 握手数据包
            Send(BaseHyperSocket.FirstConnectBytes);
        }

        public void OnReceivedCompleted(SocketMsg msg)
        {
            if (hyperSocket == null || msg.data == null || hyperSocket.isClosed)
            {
                msg.sender.Close();
                return;
            }

            if (clientSocket.ProtocolType == ProtocolType.Tcp)
            {
                // 验证通过
                if (!hyperSocket.IsValid)
                {
                    // 通信连接步骤<三> 获取有效会话标识 地址 端口 并发送验证数据
                    hyperSocket.InitializeUdpClient(msg.data);
                    return;
                }

                if (msg.data.Compare(BaseHyperSocket.HeartPongBytes))
                {
                    Interlocked.Exchange(ref heartCheckTimeOut, DateTime.UtcNow.Ticks);
                    if (!hasFirstRecvPong)
                    {
                        hasFirstRecvPong = true;
                        listener.OnOpen(hyperSocket);
                    }
                    return;
                }
                if (hyperSocket.rsa != null && hyperSocket.aes != null && !hyperSocket.isSecurityConnected)
                {
                    // 通信连接步骤<七> 检测加密签名是否正确 并且发送 连接成功 包
                    var signOk = hyperSocket.rsa.VerifyData(hyperSocket.aes.Encrypt(BaseHyperSocket.SignSecurityBytes), msg.data);
                    if (!signOk)
                        return;

                    hyperSocket.isSecurityConnected = true;
                    hyperSocket.timeFlow.StartTimeFlowES();
                    Send(hyperSocket.SessionId, BaseHyperSocket.ConnectedClientBytes);
                    return;
                }

                if (hyperSocket.aes != null
                    && (hyperSocket.config.SSLMode == 0 || hyperSocket.config.SSLMode == 1))
                {
                    var dataSSL = hyperSocket.aes.Decrypt(msg.data!);
                    if (dataSSL != null)
                        listener.OnTcpReceive(dataSSL, hyperSocket);
                }
                else
                {
                    listener.OnTcpReceive(msg.data, hyperSocket);
                }
            }
            else if (clientSocket.ProtocolType == ProtocolType.Udp)
            {
                // 验证通过
                if (hyperSocket.SessionId == msg.sessionId)
                {
                    kcpHelper?.Recv(msg.data);
                }
            }
        }

        /// <summary>
        /// kcp转发发射接口
        /// </summary>
        /// <param name="data"></param>
        public void OnSend(Span<byte> data)
        {
            Send(hyperSocket.SessionId, data);
        }

        /// <summary>
        /// kcp转收接收接口
        /// </summary>
        /// <param name="data"></param>
        public void OnReceive(byte[] data)
        {
            if (!hyperSocket.IsValid)
            {
                // 通信连接步骤<五> 验证服务器数据配置 非加密连接直接发送 连接成功 包 完成连接 | 加密连接 发送 加密密钥 处理
                hyperSocket.VerifyServerData(data);
                return;
            }

            if (hyperSocket.aes != null
                   && (hyperSocket.config.SSLMode == 0 || hyperSocket.config.SSLMode == 2))
            {
                var dataSSL = hyperSocket.aes.Decrypt(data);
                if (dataSSL != null)
                    listener?.OnUdpReceive(dataSSL, hyperSocket);
            }
            else
            {
                listener.OnUdpReceive(data, hyperSocket);
            }
        }

        public void SocketException(Exception exception)
        {
            hyperSocket.Close(exception);
        }

        internal void CloseSocket()
        {
            Close();
            kcpHelper?.CloseKcp();
        }
    }
}
