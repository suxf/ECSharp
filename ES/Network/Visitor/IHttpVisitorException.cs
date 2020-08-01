using ES.Network.Http;
using System;

namespace ES.Network.Visitor
{
    /// <summary>
    /// 捕捉接受异常接口
    /// </summary>
    public interface IHttpVisitorException
    {
        /// <summary>
        /// 异常捕捉回调
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ex"></param>
        public void CatchOnRequestException(HttpConnection conn, Exception ex);
    }
}
