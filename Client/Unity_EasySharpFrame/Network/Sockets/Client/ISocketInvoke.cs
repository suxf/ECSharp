using System;

namespace ES.Network.Sockets.Client
{
    /// <summary>
    /// 套接字委托接口回调
    /// </summary>
    public interface ISocketInvoke
    {
        /// <summary>
        /// 完成接受回调
        /// </summary>
        /// <param name="msg">数据信息</param>
        void ReceivedCompleted(SocketMsg msg);

        /// <summary>
        /// 套接字异常捕获
        /// </summary>
        /// <param name="exception">异常对象</param>
        void OnSocketException(Exception exception);
    }
}
