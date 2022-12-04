#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System.Net;
using System.Net.Sockets;

namespace ECSharp.Network.Sockets
{
    /// <summary>
    /// 套接字异步事件变量
    /// </summary>
    internal class MySocketAsyncEventArgs : SocketAsyncEventArgs
    {
        private readonly ISocketIOEvent Event;

        internal MySocketAsyncEventArgs(ISocketIOEvent eventHandler)
        {
            Event = eventHandler;
        }

        internal MySocketAsyncEventArgs(object userToken, EndPoint endPoint, ISocketIOEvent eventHandler)
        {
            UserToken = userToken;
            RemoteEndPoint = endPoint;
            Event = eventHandler;
        }

        internal MySocketAsyncEventArgs(object userToken, Socket socket, ISocketIOEvent eventHandler)
        {
            UserToken = userToken;
            AcceptSocket = socket.GetSocket();
            RemoteEndPoint = socket.endPoint;
            Event = eventHandler;
        }

        protected override void OnCompleted(SocketAsyncEventArgs e)
        {
            Event.IO_Completed(e);
        }
    }
}
