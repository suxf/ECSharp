using ES.Linq;
using ES.Network.Sockets.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace ES.Network.Sockets.HyperSocket
{
    /// <summary>
    /// 超级服务器套接字模块
    /// </summary>
    internal class HyperSocketServerModule : ServerSocket, IRemoteSocket
    {
        private readonly HyperSocketServer server;
        /// <summary>
        /// 监听器
        /// </summary>
        private readonly IHyperSocketServer listener;

        internal HyperSocketServerModule(string ip, int port, int num, int size, HyperSocketServer hyperSocket, IHyperSocketServer listener) : base(ip, port, num, size)
        {
            server = hyperSocket;
            this.listener = listener;
        }

        public void OnReceivedCompleted(RemoteSocketMsg msg)
        {
            if (server == null || msg.data == null)
            {
                // 不能正确处理则销毁
                msg.sender?.Destroy();
                return;
            }

            if (serverSocket.ProtocolType == ProtocolType.Tcp)
            {
                if (msg.sender?.hySocket == null)
                {
                    if (msg.sender == null)
                        return;

                    // 连接握手开头验证
                    // 通信连接步骤<二> 验证握手数据包 且取出有效会话标识 地址 端口 发送给客户端
                    if (!msg.data.Compare(BaseHyperSocket.FirstConnectBytes))
                    {
                        msg.sender.Destroy();
                        return;
                    }
                    var data = server.GenerateVerifyConnection(out var sessionId);
                    if (data == null)
                    {
                        msg.sender.Destroy();
                        return;
                    }

                    var s = server.GetSocketAtIndex(sessionId);
                    if (s == null || msg.sender.Socket == null)
                    {
                        msg.sender.Destroy();
                        return;
                    }

                    msg.sender.hySocket = s;
                    // 绑定数据
                    msg.sender.hySocket.tcpConn = msg.sender;
                    msg.sender.hySocket.ip = msg.sender.hySocket.tcpConn.Socket.Ip;
                    msg.sender.hySocket.tcpPort = msg.sender.hySocket.tcpConn.Socket.Port;
                    // 发送验证数据
                    msg.sender.Send(sessionId, data);
                    return;
                }

                // 处理消息
                var remote = server.GetSocketAtIndex(msg.sender.hySocket.SessionId);
                if (remote == null)
                {
                    msg.sender.Destroy();
                    return;
                }
                if (msg.data == null || !remote.isValid || !remote.CheckSameRemote(msg.sender))
                {
                    remote.CloseSocket();
                    return;
                }

                // 通信连接步骤<八> 如果受到连接成功包 则完成连接
                if (!remote.isConnecting && msg.data.Compare(BaseHyperSocket.ConnectedClientBytes))
                {
                    remote.isConnecting = true;
                    remote.SendPong();
                    listener.OnOpen(remote);
                    return;
                }

                if (server.rsa != null && remote.aes != null && !remote.isSecurityConnected)
                {
                    // 通信连接步骤<六> 验证加密密钥 加密连接需要进一步确认加密密钥是否正确 并且进行发送签名验证包
                    var key = server.rsa.Decrypt(msg.data);
                    if (key == null)
                    {
                        remote.CloseSocket();
                        return;
                    }
                    remote.isSecurityConnected = true;
                    remote.aes.SetKey(key.AsString());
                    remote.SendSignData(server.rsa.SignData(remote.aes.Encrypt(BaseHyperSocket.SignSecurityBytes)));
                    return;
                }

                if (remote.aes != null
                    && (server.config.SSLMode == 0 || server.config.SSLMode == 1))
                {
                    var dataSSL = remote.aes.Decrypt(msg.data);
                    if (dataSSL != null)
                        listener.OnTcpReceive(dataSSL, remote);
                }
                else
                {
                    listener.OnTcpReceive(msg.data, remote);
                }
            }
            else if (serverSocket.ProtocolType == ProtocolType.Udp)
            {
                if (msg.sessionId <= ushort.MinValue)
                {
                    return;
                }

                var remote = server.GetSocketAtIndex(msg.sessionId);
                if (remote == null)
                {
                    return;
                }

                // 判断是否一样来源
                if (remote.udpConn == null)
                {
                    if (msg.remoteEndPoint == null)
                    {
                        remote.CloseSocket();
                        return;
                    }
                    remote.udpConn = new RemoteConnection(msg.remoteEndPoint, this);
                    if (remote.udpConn.Socket == null)
                    {
                        remote.CloseSocket();
                        return;
                    }
                    remote.udpPort = remote.udpConn.Socket.Port;
                }

                // 处理信息
                if (msg.remoteEndPoint == null || !remote.CheckSameRemote(msg.remoteEndPoint))
                {
                    remote.CloseSocket();
                    return;
                }

                remote.RecvData(msg.data);
            }
        }

        internal void KcpDataBackHandle(RemoteHyperSocket remote, byte[] data)
        {
            if (remote.isValid)
            {
                // 判断是否为心跳
                if (data.Compare(BaseHyperSocket.HeartPingBytes))
                {
                    remote.SendPong();
                    return;
                }

                // 是否为加密
                if (remote.aes != null && (server.config.SSLMode == 0 || server.config.SSLMode == 2))
                {
                    var dataSSL = remote.aes.Decrypt(data);
                    if (dataSSL == null)
                        return;
                    data = dataSSL;
                }

                // 处理消息
                listener.OnUdpReceive(data, remote);
                return;
            }

            // 通信连接步骤<四> 对比客户端验证数据是否正确 并且发送相关配置
            long verifyCode = remote.SessionId * (server.UdpPort / 10);
            var waitVerifyCode = Encoding.UTF8.GetString(data);
            if (verifyCode.ToString() != waitVerifyCode)
            {
                remote.CloseSocket();
                return;
            }
            remote.isValid = true;
            if (remote.tcpConn != null) remote.tcpConn.isVaildHyperSocket = true;
            var str = server.rsa != null ? $"1{server.config.SSLMode}{server.rsa.PublicKey}" : "0";
            remote.SendKcp(Encoding.UTF8.GetBytes(str));
        }

        public void SocketException(RemoteConnection? conn, Exception exception)
        {
            listener.SocketError(conn?.hySocket, exception);
        }

        internal void Update(int dt)
        {
            List<int> hashlist = new List<int>();
            foreach (var item in TcpClients)
            {
                var remote = item.Value;
                if (remote.isVaildHyperSocket) continue;
                if (remote.Tag >= server.config.HeartTimeOut)
                {
                    hashlist.Add(remote.hashCode);
                    remote.isVaildHyperSocket = false;
                }
                if (remote.Tag.IsNull()) remote.Tag = dt;
                else remote.Tag += dt;
            }
            for (int i = 0, len = hashlist.Count; i < len; i++)
            {
                if (TcpClients.TryGetValue(hashlist[i], out var remote))
                {
                    remote.Destroy();
                }
            }
        }

        internal void CloseSocket()
        {
            CloseServer();
        }
    }
}
