using ES.Common.Utils;
using ES.Network.Sockets;
using ES.Network.Sockets.Client;
using System;
using System.Threading;

namespace ES.Network.HyperSocket
{
    /// <summary>
    /// 超级套接字客户端模块
    /// </summary>
    internal class HyperSocketClientModule : ClientSocket, ISocketInvoke, IKcpListener
    {
        private HyperSocket hyperSocket;
        /// <summary>
        /// 监听器
        /// </summary>
        private IHyperSocketClientListener listener;
        /// <summary>
        /// kcp协议模块
        /// </summary>
        private readonly KcpHelper kcpHelper;

        /// <summary>
        /// 心跳检测超时累计
        /// </summary>
        internal long heartCheckTimeOut = DateTime.UtcNow.Ticks;
        /// <summary>
        /// 第一次接受到pong消息
        /// </summary>
        private bool hasFirstRecvPong = false;

        internal HyperSocketClientModule(string ip, int port, int num, ProtocolType protocolType, HyperSocket hyperSocket) : base(ip, port, num)
        {
            this.hyperSocket = hyperSocket;
            if (protocolType == ProtocolType.Udp) kcpHelper = new KcpHelper(hyperSocket.SessionId, (int)hyperSocket.config.UdpReceiveSize, hyperSocket.config.KcpWinSize, hyperSocket.config.kcpMode, this);
        }

        internal void SetListener(IHyperSocketClientListener listener)
        {
            this.listener = listener;
        }

        internal void SendKcp(byte[] data)
        {
            kcpHelper.Send(data);
        }

        public void ReceivedCompleted(SocketMsg msg)
        {
            if (hyperSocket != null && msg.data != null)
            {
                if (clientSocket.protocolType == ProtocolType.Tcp)
                {
                    // 验证通过
                    if (hyperSocket.IsValid)
                    {
                        if (msg.data != null && msg.data.Compare(HyperSocket.HeartPongBytes))
                        {
                            Interlocked.Exchange(ref heartCheckTimeOut, DateTime.UtcNow.Ticks);
                            if (!hasFirstRecvPong) { hasFirstRecvPong = true; listener.OnOpen(hyperSocket); }
                        }
                        else if (hyperSocket.config.UseSSL && !hyperSocket.isSecurityConnected)
                        {
                            if (msg.data != null)
                            {
                                var signOk = hyperSocket.ssl.RSAVerifyData(hyperSocket.ssl.AESEncrypt(HyperSocket.SignSecurityBytes), msg.data);
                                if (signOk)
                                {
                                    hyperSocket.isSecurityConnected = true;
                                    hyperSocket.StartTimeFlow();
                                    Send(hyperSocket.SessionId, HyperSocket.ConnectedClientBytes);
                                }
                            }
                        }
                        else
                        {
                            if (hyperSocket.config.UseSSL && (hyperSocket.config.SSLMode == 0 || hyperSocket.config.SSLMode == 1)) listener.OnTcpReceive(hyperSocket.ssl.AESDecrypt(msg.data), hyperSocket);
                            else listener.OnTcpReceive(msg.data, hyperSocket);
                        }
                    }
                    else hyperSocket.InitializeUdpClient(msg.data);
                }
                else if (clientSocket.protocolType == ProtocolType.Udp)
                {
                    // 验证通过
                    if (hyperSocket.SessionId == msg.sessionId) kcpHelper.Recv(msg.data);
                }
            }
        }

        /// <summary>
        /// kcp转发发射接口
        /// </summary>
        /// <param name="data"></param>
        public void OnSend(byte[] data)
        {
            if (data != null && hyperSocket != null) Send(hyperSocket.SessionId, data);
        }

        /// <summary>
        /// kcp转收接收接口
        /// </summary>
        /// <param name="data"></param>
        public void OnReceive(byte[] data)
        {
            if (hyperSocket.IsValid)
            {
                if (hyperSocket.config.UseSSL && (hyperSocket.config.SSLMode == 0 || hyperSocket.config.SSLMode == 2)) listener.OnUdpReceive(hyperSocket.ssl.AESDecrypt(data), hyperSocket);
                else listener.OnUdpReceive(data, hyperSocket);
            }
            else hyperSocket.VerifyServerData(data);
        }

        public void OnSocketException(Exception exception)
        {
            if (listener != null) listener.OnError(hyperSocket, exception);
            if (hyperSocket != null) hyperSocket.Close();
        }

        internal void CloseSocket()
        {
            Close();
            if (kcpHelper != null) kcpHelper.CloseKcp();
            hyperSocket = null;
            listener = null;
        }
    }
}
