#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
namespace ECSharp.Network.Sockets.HyperSocket
{
    /// <summary>
    /// KCP模式
    /// </summary>
    public enum KcpMode
    {
        /// <summary>
        /// 普通模式
        /// </summary>
        Normal,
        /// <summary>
        /// 快速模式
        /// </summary>
        Fast
    }
}
