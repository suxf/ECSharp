using System.Net;
using System.Net.Sockets;

namespace ES.Network.Sockets
{
    /// <summary>
    /// 继承套接字
    /// </summary>
    internal class MySocketAsyncEventArgs : SocketAsyncEventArgs
    {
        internal bool isUsed = false;

        internal MySocketAsyncEventArgs(object userToken, EndPoint endPoint)
        {
            UserToken = userToken;
            RemoteEndPoint = endPoint;
        }

        internal MySocketAsyncEventArgs(object userToken, Socket socket)
        {
            UserToken = userToken;
            AcceptSocket = socket.GetSocket();
            RemoteEndPoint = socket.endPoint;
        }

        /// <summary>
        /// 重置使用状态
        /// </summary>
        internal void ResetUsedState()
        {
            isUsed = false;
        }
    }
}
