namespace ES.Network.Http
{
    /// <summary>
    /// 超文本访问协议委托回调
    /// 返回状态码：200 成功 400 访问无效 404 无此页面
    /// </summary>
    public interface HttpInvoke
    {
        /// <summary>
        /// 访问回调
        /// </summary>
        /// <param name="conn">超文本访问连接对象</param>
        void OnRequest(HttpConnection conn);
    }
}
