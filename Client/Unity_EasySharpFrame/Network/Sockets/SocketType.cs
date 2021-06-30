namespace ES.Network.Sockets
{
    /// <summary>
    /// 套接字类型
    /// </summary>
    public enum SocketType
    {
        /// <summary>
        /// Supports reliable, two-way, connection-based byte streams without the duplication
        /// of data and without preservation of boundaries. A System.Net.Sockets.Socket of
        /// this type communicates with a single peer and requires a remote host connection
        /// before communication can begin. System.Net.Sockets.SocketType.Stream uses the
        /// Transmission Control Protocol (ProtocolType.System.Net.Sockets.ProtocolType.Tcp)
        /// and the AddressFamily.System.Net.Sockets.AddressFamily.InterNetwork address family.
        /// </summary>
        Stream = 1,
        /// <summary>
        /// Supports datagrams, which are connectionless, unreliable messages of a fixed
        /// (typically small) maximum length. Messages might be lost or duplicated and might
        /// arrive out of order. A System.Net.Sockets.Socket of type System.Net.Sockets.SocketType.Dgram
        /// requires no connection prior to sending and receiving data, and can communicate
        /// with multiple peers. System.Net.Sockets.SocketType.Dgram uses the Datagram Protocol
        /// (ProtocolType.System.Net.Sockets.ProtocolType.Udp) and the AddressFamily.System.Net.Sockets.AddressFamily.InterNetwork
        /// address family.
        /// </summary>
        Dgram = 2
    }
}
