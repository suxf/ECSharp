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
    internal class HyperSocketServerModule : ServerSocket, IRemoteSocket
    {
        private readonly HyperSocketServer hyperSocket;
        /// <summary>
        /// 监听器
        /// </summary>
        private readonly IHyperSocketServer listener;


        internal HyperSocketServerModule(string ip, int port, int num, int size, HyperSocketServer hyperSocket, IHyperSocketServer listener) : base(ip, port, num, size)
        {
            this.hyperSocket = hyperSocket;
            this.listener = listener;

        }

        public void OnReceivedCompleted(RemoteSocketMsg msg)
        {
            if (hyperSocket != null && msg.data != null)
            {
                if (serverSocket.ProtocolType == ProtocolType.Tcp)
                {
                    if (msg.sender!.hySocket == null)
                    {
                        // 连接握手开头验证
                        if (msg.data.Compare(BaseHyperSocket.FirstConnectBytes))
                        {
                            var data = hyperSocket.GenerateVerifyConnection(out var sessionId);
                            if (data != null)
                            {
                                var s = hyperSocket.GetSocketAtIndex(sessionId);
                                if (s != null)
                                {
                                    msg.sender.hySocket = s;
                                    // 绑定数据
                                    msg.sender.hySocket.tcpConn = msg.sender;
                                    msg.sender.hySocket.ip = msg.sender.hySocket.tcpConn.Socket!.Ip;
                                    msg.sender.hySocket.tcpPort = msg.sender.hySocket.tcpConn.Socket.Port;
                                    // 发送验证数据
                                    msg.sender.Send(sessionId, data);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 处理消息
                        var remote = hyperSocket.GetSocketAtIndex(msg.sender.hySocket.SessionId);
                        if (remote != null)
                        {
                            if (msg.data != null && remote.isValid && remote.CheckSameRemote(msg.sender))
                            {
                                if (!remote.IsAlive && msg.data.Compare(BaseHyperSocket.ConnectedClientBytes))
                                {
                                    remote.IsAlive = true;
                                    remote.SendPong();
                                    listener!.OnOpen(remote);
                                    return;
                                }
                                else if (hyperSocket.config.UseSSL && !remote.isSecurityConnected)
                                {
                                    var key = hyperSocket.ssl!.RSADecrypt(msg.data);
                                    if (key != null)
                                    {
                                        remote.isSecurityConnected = true;
                                        remote.ssl!.SetAESKey(key.AsString());
                                        remote.SendSignData(hyperSocket.ssl.RSASignData(remote.ssl.AESEncrypt(BaseHyperSocket.SignSecurityBytes)));
                                        return;
                                    }
                                    else remote.CloseSocket();
                                }
                                else
                                {
                                    if (hyperSocket.config.UseSSL && (hyperSocket.config.SSLMode == 0 || hyperSocket.config.SSLMode == 1)) listener!.OnTcpReceive(remote.ssl!.AESDecrypt(msg.data)!, remote);
                                    else listener!.OnTcpReceive(msg.data, remote);
                                    return;
                                }
                            }
                            else remote.CloseSocket();
                        }
                    }
                }
                else if (serverSocket.ProtocolType == ProtocolType.Udp)
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
                                remote.udpPort = remote.udpConn.Socket!.Port;
                            }

                            // 处理信息
                            if (remote.CheckSameRemote(msg.remoteEndPoint))
                            {
                                remote.RecvData(msg.data);
                                return;
                            }
                            else remote.CloseSocket();
                        }
                    }
                }
            }
            // 不能正确处理则销毁
            msg.sender?.Destroy();
        }

        internal void KcpDataBackHandle(RemoteHyperSocket remote, byte[] data)
        {
            if (remote.isValid)
            {
                if (hyperSocket!.config.UseSSL && (hyperSocket.config.SSLMode == 0 || hyperSocket.config.SSLMode == 2))
                    data = remote.ssl!.AESDecrypt(data)!;

                if (data.Compare(BaseHyperSocket.HeartPingBytes)) remote.SendPong();
                else listener!.OnUdpReceive(data, remote);
            }
            else
            {
                long verifyCode = remote.SessionId * (hyperSocket!.UdpPort / 10);
                var waitVerifyCode = Encoding.UTF8.GetString(data);
                if (verifyCode.ToString() == waitVerifyCode)
                {
                    remote.isValid = true;
                    var str = hyperSocket.config.UseSSL ? ("1" + hyperSocket.config.SSLMode + hyperSocket.ssl!.GetRSAPublicKey()) : "0";
                    remote.SendKcp(Encoding.UTF8.GetBytes(str));
                }
                else remote.CloseSocket();
            }
        }

        public void SocketException(Exception exception)
        {
            listener!.SocketError(exception);
        }

        internal void CloseSocket()
        {
            CloseServer();
            // hyperSocket = null;
            // listener = null;
        }
    }
}
