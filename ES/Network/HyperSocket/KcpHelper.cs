using ES.Common.Time;
using System;
using System.Buffers;
using System.Net.Sockets.Kcp;
using System.Threading;

namespace ES.Network.HyperSocket
{
    /// <summary>
    /// KCP助手
    /// </summary>
    internal class KcpHelper : ITimeUpdate, IKcpCallback
    {
        private readonly Kcp kcp;

        private readonly IKcpListener kcpListener;
        /// <summary>
        /// 下次更新时间 【kcp优化方案】
        /// </summary>
        private DateTime nextUpdateTime = DateTime.UtcNow;
        /// <summary>
        /// 无网络数据更新次数  【kcp优化方案】
        /// </summary>
        private int noNetDataCount = 0;
        private readonly object m_lock = new object();

        private readonly BaseTimeFlow timeFlow;

        internal KcpHelper(uint conv, int mtu, int winSize, KcpMode kcpMode, IKcpListener listener)
        {
            kcp = new Kcp(conv, this);
            if (kcpMode == KcpMode.Normal) kcp.NoDelay(0, 40, 0, 0);
            else if (kcpMode == KcpMode.Fast) kcp.NoDelay(1, 10, 2, 1);
            kcp.WndSize(winSize, winSize);
            kcp.SetMtu(mtu);

            kcpListener = listener;

            timeFlow = BaseTimeFlow.CreateTimeFlow(this);
            timeFlow.StartTimeFlowES();
        }

        /// <summary>
        /// 上层kcp发射
        /// </summary>
        /// <param name="data"></param>
        internal void Send(byte[] data)
        {
            if (data != null)
            {
                kcp.Send(data);
                lock (m_lock) nextUpdateTime = DateTime.UtcNow;
                Interlocked.Exchange(ref noNetDataCount, 0);
            }
        }

        /// <summary>
        /// 上层kcp接收
        /// </summary>
        /// <param name="data"></param>
        internal void Recv(byte[] data)
        {
            if (data != null)
            {
                kcp.Input(data);
                CheckRecv();
                lock (m_lock) nextUpdateTime = DateTime.UtcNow;
                Interlocked.Exchange(ref noNetDataCount, 0);
            }
        }

        /// <summary>
        /// kcp发射
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="avalidLength"></param>
        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            kcpListener.OnSend(buffer.Memory.Slice(0, avalidLength).ToArray());
        }

        /// <summary>
        /// 检查接收
        /// </summary>
        private void CheckRecv()
        {
            int len;
            while ((len = kcp.PeekSize()) > 0)
            {
                var buffer = new byte[len];
                if (kcp.Recv(buffer) > 0) kcpListener.OnReceive(buffer);
            }
        }

        internal void CloseKcp()
        {
            timeFlow.CloseTimeFlowES();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="dt"></param>
        public void Update(int dt)
        {
            // 更新周期10ms 此处次数大于100 则为 1s 无数据跳出
            if (noNetDataCount >= 100) return;
            Interlocked.Increment(ref noNetDataCount);

            var utc = DateTime.UtcNow;
            lock (m_lock)
            {
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
            // kcp.Dispose();
        }
    }
}
