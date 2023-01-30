#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using ECSharp.Crypto;
using System;
using System.Collections.Concurrent;

namespace ECSharp.Network.Sockets.HyperSocket
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
        public readonly uint ConnectMaxNum;

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
        internal readonly ConcurrentDictionary<ushort, RemoteHyperSocket?> remoteSockets;

        internal RSA? rsa;

        /// <summary>
        /// 实际TCP套接字连接数量
        /// <para>此数量 ServerSocket 类中获得,为实际连接到服务中连接数量</para>
        /// </summary>
        public int ConnectedCount => TcpServer.ConnectedNum;

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
            if (connectMaxNum < ushort.MaxValue - 1)
            {
                // 如果小于最大数量，则加一保持连接最大数数量一致
                connectMaxNum += 1;
            }
            else if (connectMaxNum > ushort.MaxValue - 1)
            {
                // 如果大于最大数量，则减一保持数组不会溢出
                connectMaxNum = ushort.MaxValue - 1;
            }

            // 赋值最大连接数量 这样做的目的是因为 0 索引位被占用导致的
            ConnectMaxNum = connectMaxNum;

            remoteSockets = new ConcurrentDictionary<ushort, RemoteHyperSocket?>();
            for (ushort i = 1; i < connectMaxNum; i++)
            {
                remoteSockets.TryAdd(i, null);
            }

            if (config?.UseSSL ?? false) rsa = new RSA();

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
            if (0 <= session && session < ConnectMaxNum)
                return GetSocketAtIndex(session);

            return null;
        }

        /// <summary>
        /// 生成认证通道
        /// </summary>
        /// <returns></returns>
        internal ReadOnlySpan<byte> GenerateVerifyConnection(out ushort sessionId)
        {
            sessionId = GetUnusedSocketIndex();

            if (sessionId <= ushort.MinValue)
                return null;

            // 先加入验证
            RemoteHyperSocket remote = new RemoteHyperSocket(sessionId, this, config);
            SetSocketAtIndex(sessionId, remote);
            Span<byte> data = stackalloc byte[8];
            data[0] = (byte)((UdpPort >> 8) & 0xFF);
            data[1] = (byte)((UdpPort) & 0xFF);
            data[2] = (byte)((sessionId >> 8) & 0xFF);
            data[3] = (byte)((sessionId) & 0xFF);
            // 认证部分
            data[4] = (byte)(data[0] + data[1]);
            data[5] = (byte)(data[2] + data[3]);
            data[6] = (byte)(data[0] + data[3]);
            data[7] = (byte)(data[1] + data[2]);
            return data.ToArray();
        }

        /// <summary>
        /// 返回未使用的远程套接字索引
        /// <para>0 位占用 不使用</para>
        /// </summary>
        /// <returns></returns>
        internal ushort GetUnusedSocketIndex()
        {
            foreach (var item in remoteSockets)
            {
                if (item.Value == null) return item.Key;
            }
            return ushort.MinValue;
        }

        /// <summary>
        /// 设置远程套接字
        /// </summary>
        /// <returns></returns>
        internal void SetSocketAtIndex(ushort index, RemoteHyperSocket? socket)
        {
            if (index < 0 || index >= ConnectMaxNum)
                return;

            if (remoteSockets.TryGetValue(index, out var oldValue))
            {
                remoteSockets.TryUpdate(index, socket, oldValue);
            }
        }

        /// <summary>
        /// 获取远程套接字
        /// </summary>
        /// <returns></returns>
        internal RemoteHyperSocket? GetSocketAtIndex(ushort index)
        {
            if (remoteSockets.TryGetValue(index, out var socket))
                return socket;
            return null;
        }

        internal override void UpdateHandle(int dt)
        {
            heartCheckPeriod -= dt;
            if (heartCheckPeriod > 0)
                return;

            heartCheckPeriod += config.HeartCheckPeriod;

            long ticks1 = Time.TimeFlowManager.TotalRunTime - config.HeartTimeOut;
            long ticks2 = Time.TimeFlowManager.TotalRunTime - 3000;
            foreach (var item in remoteSockets)
            {
                var remote = item.Value;
                if (remote == null) continue;

                if (!remote.isConnecting)
                {
                    // 未初始化成功前超时情况
                    if (remote.heartCheckTimeOut < ticks2)
                        remote.CloseSocket();
                    continue;
                }

                if (remote.tcpConn != null)
                {
                    // 底层连接通道已关闭上层未检测出的情况
                    if (!remote.tcpConn.IsAlive || !remote.tcpConn.HasConnected)
                    {
                        remote.CloseSocket();
                        continue;
                    }
                }
                // 超时心跳或者发不出pong情况
                if (remote.heartCheckTimeOut < ticks1 || !remote.SendPong())
                {
                    remote.CloseSocket();
                }
            }
            // 检查底层tcp连接初始化不正确情况
            TcpServer.Update(config.HeartCheckPeriod);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void UpdateEnd()
        {
            foreach (var item in remoteSockets)
            {
                item.Value?.CloseSocket();
            }
            TcpServer.CloseSocket();
            UdpServer.CloseSocket();
        }
    }
}
