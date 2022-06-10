using System.Collections.Generic;
using System.Threading;

namespace ES.Time
{
    /// <summary>
    /// A timer that can be used to schedule events.
    /// </summary>
    internal class TimeFlowThread
    {
        private readonly Thread thread;
        private readonly ManualResetEventSlim waitHandle;
        private readonly List<BaseTimeFlow> timeFlows = new List<BaseTimeFlow>();
        private readonly List<BaseTimeFlow> waitAddTimeFlows = new List<BaseTimeFlow>();

#if !UNITY_2020_1_OR_NEWER
        /// <summary>
        /// 时间间隔
        /// </summary>
        private static int interval = (int)TimeInterval.Interval_16ms;
#else
        /// <summary>
        /// 时间间隔
        /// </summary>
        private static readonly int interval = (int)(UnityEngine.Time.fixedDeltaTime * 1000);
#endif

        /// <summary>
        /// 时间间隔
        /// </summary>
        internal static int Interval
        {
            get { return interval; }
#if !UNITY_2020_1_OR_NEWER
            set { interval = value; }
#endif
        }

        /// <summary>
        /// 单位毫秒最大处理数量
        /// </summary>
        internal static int UtilMsMaxHandleCount = Interval * 10 > 0 ? Interval * 10 : 1;

        /// <summary>
        /// 同步标记
        /// </summary>
        private readonly bool isSync = false;

        internal TimeFlowThread(bool isSync)
        {
            this.isSync = isSync;
            waitHandle = new ManualResetEventSlim(false);
            thread = new Thread(UpdateHandle);
            thread.IsBackground = true;
            thread.Start(this);
        }

        internal int GetTaskCount()
        {
            return timeFlows.Count;
        }

        internal void Push(BaseTimeFlow timeFlow)
        {
            lock (waitAddTimeFlows)
                waitAddTimeFlows.Add(timeFlow);
        }

        /// <summary>
        /// 更新句柄
        /// </summary>
        private static void UpdateHandle(object? obj)
        {
            TimeFlowThread? t = obj as TimeFlowThread;
            if (t == null)
                return;

            List<BaseTimeFlow> waitRmv = new List<BaseTimeFlow>();
            while (true)
            {
                // 加入新的时间流
                if (t.waitAddTimeFlows.Count > 0)
                {
                    BaseTimeFlow[]? tfArray;
                    lock (t.waitAddTimeFlows)
                    {
                        tfArray = t.waitAddTimeFlows.ToArray();
                        t.waitAddTimeFlows.Clear();
                    }

                    for (int i = 0, len = tfArray.Length; i < len; i++)
                    {
                        tfArray[i].consumeTime = TimeFlowManager.TotalRunTime;
                        t.timeFlows.Add(tfArray[i]);
                    }
                }
                for (int i = 0, len = t.timeFlows.Count; i < len; i++)
                {
                    var tf = t.timeFlows[i];
                    if (tf.isTimeFlowStop)
                    {
                        waitRmv.Add(tf);
                        if (t.isSync) tf.UpdateSyncEndES();
                        else tf.UpdateEndES();
                        continue;
                    }

                    if (tf.isTimeFlowPause)
                        continue;

                    if (t.isSync)
                        tf.UpdateSyncES();
                    else
                        tf.UpdateES();
                }
                for (int i = 0, len = waitRmv.Count; i < len; i++)
                {
                    t.timeFlows.Remove(waitRmv[i]);

                    // 优化清空逻辑
                    if (i == len - 1)
                        waitRmv.Clear();
                }
                // 睡眠
                t.waitHandle.Wait(interval);
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
                if (timeFlows[i].CloseByObj(timeUpdate))
                    return true;
                else
                    continue;
            }
            return false;
        }
    }
}
