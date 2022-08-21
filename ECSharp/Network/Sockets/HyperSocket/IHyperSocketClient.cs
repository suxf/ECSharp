using System;

namespace ECSharp.Network.Sockets.HyperSocket
{
    /// <summary>
    /// 超级套接字客户端监听器
    /// </summary>
    public interface IHyperSocketClient
    {
        /// <summary>
        /// 连接成功套接字
        /// </summary>
        /// <param name="socket"></param>
        void OnOpen(HyperSocket socket);
        /// <summary>
        /// 接受数据TCP
        /// </summary>
        void OnTcpReceive(byte[] data, HyperSocket socket);
        /// <summary>
        /// 接受数据UDP
        /// </summary>
        void OnUdpReceive(byte[] data, HyperSocket socket);
        /// <summary>
        /// 套接字错误
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="ex"></param>
        void SocketError(HyperSocket socket, Exception ex);
    }
}
