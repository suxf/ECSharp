﻿#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
namespace ECSharp.Network.Sockets
{
    /// <summary>
    /// 协议类型
    /// </summary>
    public enum ProtocolType
    {
        /// <summary>
        /// Transmission Control Protocol.
        /// </summary>
        Tcp = 6,
        /// <summary>
        /// User Datagram Protocol.
        /// </summary>
        Udp = 17
    }
}
