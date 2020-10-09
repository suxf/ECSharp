using ES.Common.Utils;
using ES.Network.Sockets;
using ES.Network.Sockets.Server;
using System;
using System.Text;
using System.Threading;

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
            if (hyperSocket != null)
            {
                if (serverSocket.protocolType == ProtocolType.Tcp)
                {
                    if (msg.sessionId == ushort.MinValue)
                    {
                        // 连接握手开头验证
                        if (msg.data[0] == 0x01 && msg.data[1] == 0x02)
                        {
                            var data = hyperSocket.GenerateVerifyConnection(out var sessionId);
                            if (data != null)
                            {
                                // 发送验证数据
                                hyperSocket.GetSocketAtIndex(sessionId).tcpConn = msg.sender;
                                msg.sender.Send((ushort)sessionId, data);
                            }
                        }
                        else msg.sender.Destroy();
                    }
                    else
                    {
                        // 处理消息
                        var remote = hyperSocket.GetSocketAtIndex(msg.sessionId);
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
                                else listener.OnTcpReceive(msg.data, remote);
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
                            if (remote.udpConn == null) remote.udpConn = new RemoteConnection(msg.remoteEndPoint, this);

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
                if (data.Compare(HyperSocket.HeartPingBytes)) remote.SendPong();
                else listener.OnUdpReceive(data, remote);
            }
            else
            {
                long secondTicks = DateTime.UtcNow.ToSecondTicks();
                long verifyCode = remote.SessionId * (secondTicks / 100);
                var waitVerifyCode = Encoding.UTF8.GetString(data);
                if (verifyCode.ToString() == waitVerifyCode)
                {
                    remote.isValid = true;
                    remote.SendKcp(Encoding.UTF8.GetBytes(DateTime.UtcNow.ToSecondTicks().ToString()));
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
