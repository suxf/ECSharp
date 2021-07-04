using System;

namespace ES.Network.Http
{
    /// <summary>
    /// 超文本访问协议委托回调
    /// </summary>
    public interface IHttp
    {
        /// <summary>
        /// 访问回调
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <param name="response">HTTP应答</param>
        void OnRequest(HttpRequest request, HttpResponse response);

        /// <summary>
        /// 套接字异常捕获
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <param name="exception">异常对象</param>
        void HttpException(HttpRequest request, Exception exception);
    }
}
