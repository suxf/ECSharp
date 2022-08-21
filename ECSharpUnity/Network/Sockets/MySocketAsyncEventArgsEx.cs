using System.Net;

namespace ECSharp.Network.Sockets
{
    /// <summary>
    /// 套接字异步事件变量
    /// </summary>
    internal class MySocketAsyncEventArgsEx : MySocketAsyncEventArgs
    {
        internal readonly SocketAsyncEventArgsEx _ex;

        internal MySocketAsyncEventArgsEx(object userToken, EndPoint endPoint, SocketAsyncEventArgsEx ex)
        : base(userToken, endPoint, ex.eventHandler)
        {
            _ex = ex;
        }

        internal MySocketAsyncEventArgsEx(object userToken, Socket socket, SocketAsyncEventArgsEx ex)
        : base(userToken, socket, ex.eventHandler)
        {
            _ex = ex;
        }

        /// <summary>
        /// 推入
        /// </summary>
        internal void Push()
        {
            _ex.Push(this);
        }
    }
}
