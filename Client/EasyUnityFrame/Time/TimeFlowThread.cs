using System.Collections.Generic;
using System.Threading;

namespace ES.Time
{
    /// <summary>
    /// A timer that can be used to schedule events.
    /// </summary>
    internal class TimeFlowThread
    {
        private Thread? thread;
        private readonly List<BaseTimeFlow> timeFlows = new List<BaseTimeFlow>();
        private readonly List<BaseTimeFlow> waitAddTimeFlows = new List<BaseTimeFlow>();
        /// <summary>
        /// 正在更新状态值
        /// </summary>
        internal bool IsRunning = false;

        /// <summary>
        /// 同步标记
        /// </summary>
        private readonly bool isSync = false;

        /// <summary>
        /// 高精度模式
        /// </summary>
        internal static bool isHighPrecisionMode = false;

        internal TimeFlowThread(bool isSync)
        {
            this.isSync = isSync;
            IsRunning = true;
            thread = new Thread(UpdateHandle);
            thread.IsBackground = true;
            thread.Start();
        }

        internal int GetTaskCount()
        {
            return timeFlows.Count;
        }

        internal void Push(BaseTimeFlow timeFlow)
        {
            lock (waitAddTimeFlows) waitAddTimeFlows.Add(timeFlow);
        }

        /// <summary>
        /// 更新句柄
        /// 这个地方要优化，在原基础线程优化方案上改成自动增长的模式，检测线程里工作线数量与处理时长的比例是否对称和目标延迟是否对等，否则增加新的线程并且移动到新线程中
        /// 以及线程超时优化
        /// </summary>
        private void UpdateHandle()
        {
            List<BaseTimeFlow> waitRmv = new List<BaseTimeFlow>();
            // 耗时监视器
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            while (IsRunning)
            {
                // 加入新的时间流
                if (waitAddTimeFlows.Count > 0)
                {
                    BaseTimeFlow[]? tfArray;
                    lock (waitAddTimeFlows)
                    {
                        tfArray = waitAddTimeFlows.ToArray();
                        waitAddTimeFlows.Clear();
                    }
                    for (int i = 0, len = tfArray.Length; i < len; i++)
                    {
                        tfArray[i].consumeTime = watch.Elapsed.TotalSeconds;
                        timeFlows.Add(tfArray[i]);
                    }
                }
                for (int i = 0, len = timeFlows.Count; i < len; i++)
                {
                    var tf = timeFlows[i];
                    if (!tf.IsTimeUpdateActive() || tf.isTimeFlowStop)
                    {
                        waitRmv.Add(tf);
                        if (isSync) tf.UpdateSyncEndES();
                        else tf.UpdateEndES();
                        continue;
                    }
                    if (tf.isTimeFlowPause) continue;
                    if (isSync) tf.UpdateSyncES(watch.Elapsed.TotalSeconds);
                    else tf.UpdateES(watch.Elapsed.TotalSeconds);
                }
                for (int i = 0, len = waitRmv.Count; i < len; i++)
                {
                    timeFlows.Remove(waitRmv[i]);
                }
                waitRmv.Clear();
                // 精度调整
                if (isHighPrecisionMode) Thread.Yield();
                else Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 通过对象关闭时间流
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <returns></returns>
        internal bool CloseByObj(ITimeUpdate timeUpdate)
        {
            for (int i = 0, len = timeFlows.Count; i < len; i++)
            {
                if (timeFlows[i].CloseByObj(timeUpdate)) return true;
                else continue;
            }
            return false;
        }

        internal void Close()
        {
            IsRunning = false;
            try
            {
#pragma warning disable SYSLIB0006 // 类型或成员已过时
                if (thread != null && thread.IsAlive) thread.Abort();
#pragma warning restore SYSLIB0006 // 类型或成员已过时
            }
            catch { }
            thread = null;
        }
    }
}
