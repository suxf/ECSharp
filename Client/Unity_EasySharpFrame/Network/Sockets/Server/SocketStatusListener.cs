namespace ES.Network.Sockets.Server
{
    /// <summary>
    /// 套接字状态监听回调
    /// </summary>
    public interface SocketStatusListener
    {
        /// <summary>
        /// 新连接回调
        /// </summary>
        void OnConnect(RemoteConnection connectClient);
        /// <summary>
        /// 连接关闭回调
        /// </summary>
        void OnClose(RemoteConnection removeClient);
    }
}
