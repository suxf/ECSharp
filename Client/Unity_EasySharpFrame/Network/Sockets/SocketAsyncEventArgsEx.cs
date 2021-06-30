using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ES.Network.Sockets
{
    /// <summary>
    /// 套接字异步事件参数拓展
    /// </summary>
    internal partial class SocketAsyncEventArgsEx
    {
        /// <summary>
        /// 参数列表
        /// </summary>
        private readonly List<MySocketAsyncEventArgs> argsList;
        /// <summary>
        /// 锁
        /// </summary>
        private readonly object m_lock = new object();
        /// <summary>
        /// 参数索引
        /// </summary>
        private readonly int index = 0;

        private object userToken = null;
        private EndPoint endPoint = null;
        private Socket socket = null;
        private EventHandler<SocketAsyncEventArgs> eventHandler = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal SocketAsyncEventArgsEx(object userToken, EndPoint endPoint, EventHandler<SocketAsyncEventArgs> eventHandler)
        {
            argsList = new List<MySocketAsyncEventArgs>();
            this.userToken = userToken;
            this.endPoint = endPoint;
            this.eventHandler = eventHandler;
            ExpandNewArgs();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        internal SocketAsyncEventArgsEx(object userToken, Socket socket, EventHandler<SocketAsyncEventArgs> eventHandler)
        {
            argsList = new List<MySocketAsyncEventArgs>();
            this.userToken = userToken;
            this.socket = socket;
            this.eventHandler = eventHandler;
            ExpandNewArgs();
        }

        /// <summary>
        /// 扩容
        /// </summary>
        private void ExpandNewArgs()
        {
            lock (m_lock)
            {
                for (int i = 0; i < 8; i++)
                {
                    MySocketAsyncEventArgs mySocketAsyncEventArgs;
                    if (socket == null) mySocketAsyncEventArgs = new MySocketAsyncEventArgs(userToken, endPoint);
                    else mySocketAsyncEventArgs = new MySocketAsyncEventArgs(userToken, socket);
                    mySocketAsyncEventArgs.Completed += eventHandler;
                    argsList.Add(mySocketAsyncEventArgs);
                }
            }
        }

        /// <summary>
        /// 取出值
        /// </summary>
        /// <returns></returns>
        internal MySocketAsyncEventArgs Pop()
        {
            lock (m_lock)
            {
                for (int i = index, len = argsList.Count; i < len; i++)
                {
                    if (!argsList[i].isUsed)
                    {
                        argsList[i].isUsed = true;
                        return argsList[i];
                    }
                }
                ExpandNewArgs();
                return Pop();
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        internal void Destroy()
        {
            lock (m_lock)
            {
                for (int i = 0, len = argsList.Count; i < len; i++)
                {
                    argsList[i].isUsed = true;
                    argsList[i].Dispose();
                    argsList[i] = null;
                }
            }
            userToken = null;
            endPoint = null;
            socket = null;
            eventHandler = null;
        }
    }
}
