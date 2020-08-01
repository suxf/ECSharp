namespace ES.Network.Sockets
{
    /// <summary>
    /// 套接字委托接口回调
    /// </summary>
    public interface SocketInvoke
    {
        /// <summary>
        /// 完成接受回调
        /// </summary>
        /// <param name="msg">数据信息</param>
        void ReceivedCompleted(SocketMsg msg);
    }
}
