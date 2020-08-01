namespace ES.Network.Websocket
{
    /// <summary>
    /// websocket委托
    /// </summary>
    public interface WebsocketInvoke
    {
        /// <summary>
        /// 连接打开回调
        /// </summary>
        /// <param name="conn">连接对象</param>
        void OnOpen(RemoteConnection conn);
        /// <summary>
        /// 连接关闭回调
        /// </summary>
        /// <param name="conn">连接对象</param>
        void OnClose(RemoteConnection conn);
        /// <summary>
        /// 收到消息回调
        /// </summary>
        /// <param name="conn">连接对象</param>
        void OnMessage(RemoteConnection conn);
        /// <summary>
        /// 收到消息回调
        /// </summary>
        /// <param name="conn">连接对象</param>
        void OnBinary(RemoteConnection conn);
    }
}
