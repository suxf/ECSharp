using ECSharp.Time;
using System;
using System.Buffers;
using System.Net.Sockets.Kcp;

namespace ECSharp.Network.Sockets.HyperSocket
{
    /// <summary>
    /// KCP助手
    /// </summary>
    internal class KcpHelper : ITimeUpdate, IKcpCallback
    {
        private readonly Kcp kcp;

        private readonly IKcp kcpListener;
        /// <summary>
        /// 下次更新时间 【kcp优化方案】
        /// </summary>
        private DateTime nextUpdateTime = DateTime.UtcNow;
        /// <summary>
        /// 无网络数据更新次数  【kcp优化方案】
        /// </summary>
        private int noNetDataCount = 0;

        private bool isClosed = false;

        private readonly BaseTimeFlow timeFlow;

        public KcpHelper(uint conv, int mtu, int winSize, KcpMode kcpMode, IKcp listener)
        {
            kcp = new Kcp(conv, this);
            if (kcpMode == KcpMode.Normal) kcp.NoDelay(0, 40, 0, 0);
            else if (kcpMode == KcpMode.Fast) kcp.NoDelay(1, 10, 2, 1);
            kcp.WndSize(winSize, winSize);
            kcp.SetMtu(mtu);

            kcpListener = listener;

            timeFlow = BaseTimeFlow.CreateTimeFlow(this/*, false, 10*/);
            timeFlow.StartTimeFlowES();
        }

        /// <summary>
        /// 上层kcp发射
        /// </summary>
        /// <param name="data"></param>
        public void Send(Span<byte> data)
        {
            if (isClosed) return;

            lock (kcp)
            {
                if (isClosed) return;
                kcp.Send(data);
                nextUpdateTime = DateTime.UtcNow;
                noNetDataCount = 0;
            }
        }

        /// <summary>
        /// 上层kcp接收
        /// </summary>
        /// <param name="data"></param>
        public void Recv(Span<byte> data)
        {
            if (isClosed) return;

            lock (kcp)
            {
                if (isClosed) return;
                kcp.Input(data);
                int len;
                // 检查接收
                while ((len = kcp.PeekSize()) > 0)
                {
                    var buffer = new byte[len];
                    if (kcp.Recv(buffer) > 0) kcpListener.OnReceive(buffer);
                    else break;
                }
                nextUpdateTime = DateTime.UtcNow;
                noNetDataCount = 0;
            }
        }

        /// <summary>
        /// kcp发射
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="avalidLength"></param>
        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            if (buffer.Memory.Length == avalidLength)
                kcpListener.OnSend(buffer.Memory.ToArray());
            else
                kcpListener.OnSend(buffer.Memory.Slice(0, avalidLength).ToArray());
        }

        public void CloseKcp()
        {
            lock (kcp)
            {
                isClosed = true;
                timeFlow.CloseTimeFlowES();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="dt"></param>
        public void Update(int dt)
        {
            if (isClosed) return;

            lock (kcp)
            {
                if (isClosed) return;
                // 更新周期10ms 此处次数大于100 则为 1s 无数据跳出
                if (noNetDataCount >= 100) return;
                ++noNetDataCount;

                DateTime utc = DateTime.UtcNow;
                if (nextUpdateTime <= utc)
                {
                    kcp.Update(utc);
                    nextUpdateTime = kcp.Check(utc);
                }
            }
        }

        /// <summary>
        /// 停止更新
        /// </summary>
        public void UpdateEnd()
        {
            lock (kcp) kcp.Dispose();
        }
    }
}
