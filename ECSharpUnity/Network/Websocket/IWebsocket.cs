#if !UNITY_2020_1_OR_NEWER
using System;

namespace ECSharp.Network.Websocket
{
    /// <summary>
    /// websocket委托
    /// </summary>
    public interface IWebsocket
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
        /// <summary>
        /// 连接发生错误
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="exception"></param>
        void OnError(RemoteConnection conn, Exception exception);
    }
}
#endif
