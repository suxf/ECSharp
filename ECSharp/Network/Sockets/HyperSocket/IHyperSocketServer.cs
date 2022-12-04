#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;

namespace ECSharp.Network.Sockets.HyperSocket
{
    /// <summary>
    /// 超级套接字服务端监听器
    /// </summary>
    public interface IHyperSocketServer
    {
        /// <summary>
        /// 连接成功套接字
        /// </summary>
        /// <param name="socket"></param>
        void OnOpen(RemoteHyperSocket socket);
        /// <summary>
        /// 接受数据TCP
        /// </summary>
        void OnTcpReceive(byte[] data, RemoteHyperSocket socket);
        /// <summary>
        /// 接受数据UDP
        /// </summary>
        void OnUdpReceive(byte[] data, RemoteHyperSocket socket);
        /// <summary>
        /// 关闭套接字
        /// </summary>
        /// <param name="socket"></param>
        void OnClose(RemoteHyperSocket socket);
        /// <summary>
        /// 套接字错误
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="ex"></param>
        void SocketError(RemoteHyperSocket? socket, Exception ex);
    }
}
