#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;
using System.Collections.Generic;
using System.Reflection;
#if !UNITY_2020_1_OR_NEWER
using System.Threading;
#endif

namespace ECSharp.Time
{
    /// <summary>
    /// A timer that can be used to schedule events.
    /// </summary>
    internal class TimeFlowThread
    {
#if !UNITY_2020_1_OR_NEWER
        private readonly Thread? thread;
        private readonly ManualResetEventSlim? waitHandle;
#endif
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

#if !UNITY_2020_1_OR_NEWER
        internal static Action<Exception>? exceptionListener = null;
        internal static Func<Exception, bool>? hotfixExceptionListener = null;
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
#if !UNITY_2020_1_OR_NEWER
            waitHandle = new ManualResetEventSlim(false);
            thread = new Thread(UpdateHandle);
            thread.IsBackground = true;
            thread.Start(this);
#endif
        }

        internal int GetTaskCount()
        {
            return timeFlows.Count;
        }

        internal void Push(BaseTimeFlow timeFlow)
        {
            lock (waitAddTimeFlows)
            {
                waitAddTimeFlows.Add(timeFlow);
            }
        }

#if UNITY_2020_1_OR_NEWER
        internal System.Collections.IEnumerator OnUnityUpdate(TimeFlowThread thread)
        {
            yield return UpdateHandle(thread);
        }
#endif

/// <summary>
/// 更新句柄
/// </summary>
#if !UNITY_2020_1_OR_NEWER
        private static void UpdateHandle(object? obj)
#else
        private static System.Collections.IEnumerator UpdateHandle(object? obj)
#endif
        {
            if (obj is not TimeFlowThread t)
            { 
#if !UNITY_2020_1_OR_NEWER
                return;
#else
                yield break;
#endif
            }

            for (int i = 0; i < t.timeFlows.Count; i++)
            {
                t.timeFlows[i].ResetIdle();
            }

#if UNITY_2020_1_OR_NEWER
            yield return OnUpdate(t);
#else
            try
            {
                OnUpdate(t);
            }
            catch(Exception e)
            {
                if ((hotfixExceptionListener == null || !hotfixExceptionListener.Invoke(e))
                    && exceptionListener == null)
                {
                    throw;
                }
                exceptionListener?.Invoke(e);
                UpdateHandle(obj);
            }
#endif
        }

        static readonly List<BaseTimeFlow> waitRmv = new List<BaseTimeFlow>();
#if !UNITY_2020_1_OR_NEWER
        private static void OnUpdate(TimeFlowThread t)
#else
        private static System.Collections.IEnumerator OnUpdate(TimeFlowThread t)
#endif
        {
            waitRmv.Clear();
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
                        tfArray[i].consumeTime = Utils.SystemInfo.TotalRunTime;
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
#if !UNITY_2020_1_OR_NEWER
                // 睡眠
                t.waitHandle.Wait(interval);
#else
                yield return Awaiters.Seconds(UnityEngine.Time.fixedDeltaTime);
#endif
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

        internal void DoWithAssembly(int action, Assembly assembly)
        {
            for (int i = 0, len = timeFlows.Count; i < len; i++)
            {
                timeFlows[i].DoWithAssembly(action, assembly);
            }
        }
    }
}
