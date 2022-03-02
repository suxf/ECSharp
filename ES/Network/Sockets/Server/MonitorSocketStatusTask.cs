﻿using ES.Common.Time;
using System.Collections.Concurrent;
using System.Threading;

namespace ES.Network.Sockets.Server
{
    /// <summary>
    /// 监控套接字状态任务 (服务器用)
    /// <para>监控原理：只要处于队列中的用户票据都会每秒递增1，达到设定超时秒数后移除。（只要收到任何对方消息都会重置该事件）</para>
    /// </summary>
    public class MonitorSocketStatusTask : ITimeUpdate
    {
        private readonly ConcurrentQueue<RemoteConnection> remoteUserTokens = new ConcurrentQueue<RemoteConnection>();
        /// <summary>
        /// 超时时间。 单位秒，只有大于0才生效
        /// </summary>
        private int timeoutSecond = -1;

        private readonly BaseTimeFlow timeFlow;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="timeout">(超时)断线时间，单位:s</param>
        public MonitorSocketStatusTask(int timeout)
        {
            SetTimeout(timeout);

            timeFlow = BaseTimeFlow.CreateTimeFlow(this, 0);
            timeFlow.StartTimeFlowES();
        }

        /// <summary>
        /// 设置超时时间
        /// </summary>
        /// <param name="timeout">(超时)断线时间，单位:s 小于0则不生效</param>
        public void SetTimeout(int timeout)
        {
            Interlocked.Exchange(ref timeoutSecond, timeout);
        }

        /// <summary>
        /// 推送检测
        /// </summary>
        /// <param name="token">需要检测的客户端票据</param>
        internal void PushCheck(RemoteConnection token)
        {
            remoteUserTokens.Enqueue(token);
        }

        /// <summary>
        /// 回调处理
        /// </summary>
        private void TimeoutTaskCallback()
        {
            for (int i = 0, len = remoteUserTokens.Count; i < len; i++)
            {
                if (remoteUserTokens.TryDequeue(out RemoteConnection? token))
                {
                    Interlocked.Increment(ref token.timeoutCount);
                    if (token.timeoutCount >= timeoutSecond)
                    {
                        Interlocked.Exchange(ref token.timeoutCount, 0);
                        token.Destroy();
                    }

                    if (token.IsAlive)
                    {
                        remoteUserTokens.Enqueue(token);
                    }
                    else
                    {
                        // 断开回调
                        if (token.SocketSvrMgr != null && token.SocketSvrMgr.socketStatusListener != null)
                        {
                            token.SocketSvrMgr.socketStatusListener.OnClose(token);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 关闭任务
        /// </summary>
        internal void Close()
        {
            timeFlow.CloseTimeFlowES();
        }

        int periodNow = 0;
        /// <summary>
        /// 系统调用
        /// </summary>
        /// <param name="dt"></param>
        public void Update(int dt)
        {
            periodNow += dt;
            if (periodNow >= 1000)
            {
                periodNow = 0;
                TimeoutTaskCallback();
            }
        }

        /// <summary>
        /// 停止更新
        /// </summary>
        public void UpdateEnd()
        {
        }
    }
}
