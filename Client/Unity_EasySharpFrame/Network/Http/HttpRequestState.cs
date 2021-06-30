namespace ES.Network.Http
{
    /// <summary>
    /// 超文本协议请求状态
    /// </summary>
    public enum HttpRequestState
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        Success = 200,
        /// <summary>
        /// 请求失败
        /// </summary>
        Fail = 400,
        /// <summary>
        /// 无法访问
        /// </summary>
        NonExistent = 404
    }
}
