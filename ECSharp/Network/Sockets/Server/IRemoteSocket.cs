using System;

namespace ECSharp.Network.Sockets.Server
{
    /// <summary>
    /// 远程套接字委托接口回调
    /// </summary>
    public interface IRemoteSocket
    {
        /// <summary>
        /// 完成接受回调
        /// </summary>
        /// <param name="msg">数据信息体</param>
        void OnReceivedCompleted(RemoteSocketMsg msg);

        /// <summary>
        /// 套接字异常捕获
        /// </summary>
        /// <param name="conn">连接对象</param>
        /// <param name="exception">异常对象</param>
        void SocketException(RemoteConnection? conn, Exception exception);
    }
}
