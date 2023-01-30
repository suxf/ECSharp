#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
namespace ECSharp.Network.Sockets
{
    /// <summary>
    /// 地址簇
    /// </summary>
    public enum AddressFamily
    {
        /// <summary>
        /// Address for IP version 4.
        /// </summary>
        InterNetwork = 2,
        /// <summary>
        /// Address for IP version 6.
        /// </summary>
        InterNetworkV6 = 23
    }
}
