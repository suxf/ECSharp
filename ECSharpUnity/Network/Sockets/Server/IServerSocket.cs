﻿#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
namespace ECSharp.Network.Sockets.Server
{
    /// <summary>
    /// 套接字状态监听回调
    /// </summary>
    public interface IServerSocket
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
