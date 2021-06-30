using System;

namespace ES.Network.Sockets.Client.Linq
{
    /// <summary>
    /// 捕捉接受异常接口
    /// </summary>
    public interface ISocketVisitorException
    {
        /// <summary>
        /// 异常捕捉回调
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        void OnReceivedException(SocketMsg msg, Exception ex);
    }
}
