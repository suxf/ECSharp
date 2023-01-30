#if UNITY_2017_1_OR_NEWER || NET462 || NETSTANDARD2_0 
using ECSharp.Linq;
#endif
using ECSharp.Utils;
using System.Collections.Concurrent;
using System.Net;

namespace ECSharp.Network.Sockets
{
    /// <summary>
    /// 套接字异步事件参数拓展
    /// </summary>
    internal class SocketAsyncEventArgsEx
    {
        /// <summary>
        /// 参数列表
        /// </summary>
        private readonly ConcurrentBag<MySocketAsyncEventArgsEx> argsList;

        private object userToken;
        private EndPoint? endPoint = null;
        private Socket? socket = null;
        internal ISocketIOEvent eventHandler;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal SocketAsyncEventArgsEx(object userToken, EndPoint endPoint, ISocketIOEvent eventHandler)
        {
            argsList = new ConcurrentBag<MySocketAsyncEventArgsEx>();
            this.userToken = userToken;
            this.endPoint = endPoint;
            this.eventHandler = eventHandler;
            ExpandNewArgs();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        internal SocketAsyncEventArgsEx(object userToken, Socket socket, ISocketIOEvent eventHandler)
        {
            argsList = new ConcurrentBag<MySocketAsyncEventArgsEx>();
            this.userToken = userToken;
            this.socket = socket;
            this.eventHandler = eventHandler;
            ExpandNewArgs();
        }

        /// <summary>
        /// 扩容
        /// </summary>
        private bool ExpandNewArgs()
        {
            for (int i = 0; i < SystemInfo.ProcessorCount; i++)
            {
                MySocketAsyncEventArgsEx mySocketAsyncEventArgs;
                if (socket == null)
                {
                    if (endPoint != null)
                        mySocketAsyncEventArgs = new MySocketAsyncEventArgsEx(userToken, endPoint, this);
                    else
                        return false;
                }
                else
                    mySocketAsyncEventArgs = new MySocketAsyncEventArgsEx(userToken, socket, this);

                argsList.Add(mySocketAsyncEventArgs);
            }

            return true;
        }

        /// <summary>
        /// 取出值
        /// </summary>
        /// <returns></returns>
        internal MySocketAsyncEventArgsEx? Pop()
        {
            if (!argsList.TryTake(out var args) && ExpandNewArgs())
            {
                return Pop();
            }
            return args;
        }

        internal void Push(MySocketAsyncEventArgsEx args)
        {
            argsList.Add(args);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        internal void Destroy()
        {
            foreach (var item in argsList)
            {
                item.Dispose();
            }
#if !UNITY_2020_1_OR_NEWER && !NET462 && !NETSTANDARD2_0
            argsList.Clear();
#else
            argsList.ClearAll();
#endif
            endPoint = null;
            socket = null;
        }
    }
}
