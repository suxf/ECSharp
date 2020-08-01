using Fleck;

namespace ES.Network.Websocket
{
    /// <summary>
    /// 远程连接对象
    /// </summary>
    public class RemoteConnection
    {
        /// <summary>
        /// ws连接对象
        /// </summary>
        public IWebSocketConnection conn;
        /// <summary>
        /// 标识
        /// </summary>
        public object tag;
        /// <summary>
        /// ws消息
        /// </summary>
        public string message;
        /// <summary>
        /// ws消息
        /// </summary>
        public byte[] buffer;
    }
}
