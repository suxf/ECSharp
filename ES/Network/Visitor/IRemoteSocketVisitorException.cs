using ES.Network.Sockets;
using System;

namespace ES.Network.Visitor
{
    /// <summary>
    /// 捕捉接受异常接口
    /// </summary>
    public interface IRemoteSocketVisitorException
    {
        /// <summary>
        /// 异常捕捉回调
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public void CatchReceivedException(RemoteSocketMsg msg, Exception ex);
    }
}
