using System.Net.Sockets;

namespace ES.Network.Sockets
{
    /// <summary>
    /// 套接字IO事件接口
    /// </summary>
    internal interface ISocketIOEvent
    {
        void IO_Completed(SocketAsyncEventArgs e);
    }
}
