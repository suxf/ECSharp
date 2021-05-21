using ES.Common.Utils;
using ES.Network.Sockets;
using ES.Network.Sockets.Server;
using System;
using System.Text;

namespace ES.Network.HyperSocket
{
    /// <summary>
    /// 超级服务器套接字模块
    /// </summary>
    internal class HyperSocketServerModule : ServerSocket, IRemoteSocketInvoke
    {
        private HyperSocket hyperSocket;
        /// <summary>
        /// 监听器
        /// </summary>
        private IHyperSocketServerListener listener;


        internal HyperSocketServerModule(string ip, int port, int num, int size, HyperSocket hyperSocket) : base(ip, port, num, size)
        {
            this.hyperSocket = hyperSocket;
        }

        internal void SetListener(IHyperSocketServerListener listener)
        {
            this.listener = listener;
        }

        public void ReceivedCompleted(RemoteSocketMsg msg)
        {
            if (hyperSocket != null && msg.data != null)
            {
                if (serverSocket.protocolType == ProtocolType.Tcp)
                {
                    if (msg.sender.hySocket == null)
                    {
                        // 连接握手开头验证
                        if (msg.data.Compare(HyperSocket.FirstConnectBytes))
                        {
                            var data = hyperSocket.GenerateVerifyConnection(out var sessionId);
                            if (data != null)
                            {
                                msg.sender.hySocket = hyperSocket.GetSocketAtIndex(sessionId);
                                // 绑定数据
                                msg.sender.hySocket.tcpConn = msg.sender;
                                msg.sender.hySocket.ip = msg.sender.hySocket.tcpConn.socket.ip;
                                msg.sender.hySocket.tcpPort = msg.sender.hySocket.tcpConn.socket.port;
                                // 发送验证数据
                                msg.sender.Send(sessionId, data);
                            }
                        }
                        else msg.sender.Destroy();
                    }
                    else
                    {
                        // 处理消息
                        var remote = hyperSocket.GetSocketAtIndex(msg.sender.hySocket.SessionId);
                        if (remote != null)
                        {
                            if (msg.data != null && remote.isValid && remote.CheckSameRemote(msg.sender))
                            {
                                if (!remote.IsAlive && msg.data.Compare(HyperSocket.ConnectedClientBytes))
                                {
                                    remote.IsAlive = true;
                                    remote.SendPong();
                                    listener.OnOpen(remote);
                                }
                                else if (hyperSocket.config.UseSSL && !remote.isSecurityConnected)
                                {
                                    var key = hyperSocket.ssl.RSADecrypt(msg.data);
                                    if (key != null)
                                    {
                                        remote.isSecurityConnected = true;
                                        remote.ssl.SetAESKey(key.AsString());
                                        remote.SendSignData(hyperSocket.ssl.RSASignData(remote.ssl.AESEncrypt(HyperSocket.SignSecurityBytes)));
                                    }
                                    else remote.CloseSocket();
                                }
                                else
                                {
                                    if (hyperSocket.config.UseSSL && (hyperSocket.config.SSLMode == 0 || hyperSocket.config.SSLMode == 1)) listener.OnTcpReceive(remote.ssl.AESDecrypt(msg.data), remote);
                                    else listener.OnTcpReceive(msg.data, remote);
                                }
                            }
                            else remote.CloseSocket();
                        }
                    }
                }
                else if (serverSocket.protocolType == ProtocolType.Udp)
                {
                    if (msg.sessionId > ushort.MinValue)
                    {
                        var remote = hyperSocket.GetSocketAtIndex(msg.sessionId);
                        if (remote != null)
                        {
                            // 判断是否一样来源
                            if (remote.udpConn == null)
                            {
                                remote.udpConn = new RemoteConnection(msg.remoteEndPoint, this);
                                remote.udpPort = remote.udpConn.socket.port;
                            }

                            // 处理信息
                            if (remote.CheckSameRemote(msg.remoteEndPoint)) remote.RecvData(msg.data);
                            else remote.CloseSocket();
                        }
                    }
                }
            }
        }

        internal void KcpDataBackHandle(RemoteHyperSocket remote, byte[] data)
        {
            if (remote.isValid)
            {
                if (hyperSocket.config.UseSSL && (hyperSocket.config.SSLMode == 0 || hyperSocket.config.SSLMode == 2))
                    data = remote.ssl.AESDecrypt(data);

                if (data.Compare(HyperSocket.HeartPingBytes)) remote.SendPong();
                else listener.OnUdpReceive(data, remote);
            }
            else
            {
                long verifyCode = remote.SessionId * (hyperSocket.UdpPort / 10);
                var waitVerifyCode = Encoding.UTF8.GetString(data);
                if (verifyCode.ToString() == waitVerifyCode)
                {
                    remote.isValid = true;
                    var str = hyperSocket.config.UseSSL ? ("1" + hyperSocket.config.SSLMode + hyperSocket.ssl.GetRSAPublicKey()) : "0";
                    remote.SendKcp(Encoding.UTF8.GetBytes(str));
                }
                else remote.CloseSocket();
            }
        }

        public void OnSocketException(Exception exception)
        {
            listener.OnError(exception);
        }

        internal void CloseSocket()
        {
            CloseServer();
            hyperSocket = null;
            listener = null;
        }
    }
}
