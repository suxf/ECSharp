namespace ES.Network.HyperSocket
{
    /// <summary>
    /// kcp监听器
    /// </summary>
    internal interface IKcpListener
    {
        void OnReceive(byte[] data);

        void OnSend(byte[] data);
    }
}
