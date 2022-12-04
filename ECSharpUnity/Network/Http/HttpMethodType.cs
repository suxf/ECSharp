#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
namespace ECSharp.Network.Http
{
    /// <summary>
    /// http访问方法类型
    /// </summary>
    public enum HttpMethodType
    {
        /// <summary>
        /// 未知
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// GET请求
        /// </summary>
        GET,
        /// <summary>
        /// POST请求
        /// </summary>
        POST,
        /// <summary>
        /// PUT请求
        /// </summary>
        PUT,
        /// <summary>
        /// DELETE请求
        /// </summary>
        DELETE,
        /// <summary>
        /// HEAD请求
        /// </summary>
        HEAD,
        /// <summary>
        /// CONNECT请求
        /// </summary>
        CONNECT,
        /// <summary>
        /// OPTIONS请求
        /// </summary>
        OPTIONS,
        /// <summary>
        /// TRACE请求
        /// </summary>
        TRACE
    }
}
