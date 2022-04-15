using System;
using System.Threading;

namespace ES.Time
{
    /// <summary>
    /// 时间流基类
    /// <para>封装了时间流必要的函数 正常使用 TimeFlow 即可</para>
    /// <para>框架内部调用类</para>
    /// </summary>
    public class BaseTimeFlow
    {
        /// <summary>
        /// 时间流暂停开关
        /// </summary>
        internal bool isTimeFlowPause = true;

        /// <summary>
        /// 时间流停止开关
        /// </summary>
        internal bool isTimeFlowStop = false;

        // private readonly Stopwatch stopwatch = new Stopwatch();
        // internal int lastUseTime = 0;

        private readonly WeakReference<ITimeUpdate> reference;

        /// <summary>
        /// 耗时监视器累积时间 此处内部转换为纳秒整型
        /// </summary>
        private long consumeTime = 0;
        /// <summary>
        /// 未消耗的修正时间 此处内部转换为纳秒整型
        /// </summary>
        private long notConsumeFixedTime = 0;
        /// <summary>
        /// 修正时间 毫秒整型
        /// </summary>
        internal readonly int fixedTime = 0;
        /// <summary>
        /// 修正时间 此处内部转换为纳秒整型
        /// </summary>
        private readonly long fixedNanoTime = 0;
        /// <summary>
        /// 首次更新
        /// </summary>
        private bool firstUpdate = true;

        /// <summary>
        /// 空闲标记
        /// </summary>
        private bool IsIdle = true;

        /// <summary>
        /// 同步标记
        /// </summary>
        private readonly bool IsSync = false;

        /// <summary>
        /// 构造函数 内部使用
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <param name="isSync">同步标记</param>
        /// <param name="fixedTime">修正时间</param>
        protected BaseTimeFlow(ITimeUpdate timeUpdate, bool isSync, int fixedTime)
        {
            IsSync = isSync;
            this.fixedTime = fixedTime;
            if (this.fixedTime <= 0) this.fixedTime = 0;
            else fixedNanoTime = fixedTime * 1000000L;
            reference = new WeakReference<ITimeUpdate>(timeUpdate);
            TimeFlowManager.Instance.PushTimeFlow(this, isSync);
        }

        /// <summary>
        /// 创建基础时间流
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <param name="isSync">同步标记</param>
        /// <param name="fixedTime">修正时间</param>
        internal static BaseTimeFlow CreateTimeFlow(ITimeUpdate timeUpdate, bool isSync = false, int fixedTime = 10)
        {
            return new BaseTimeFlow(timeUpdate, isSync, fixedTime);
        }

        /// <summary>
        /// 索引是否还在
        /// </summary>
        /// <returns></returns>
        internal bool IsTimeUpdateActive()
        {
            return reference.TryGetTarget(out _);
        }

        /// <summary>
        /// 开始时间流
        /// </summary>
        internal void StartTimeFlowES()
        {
            isTimeFlowPause = false;
        }

        /// <summary>
        /// 设置时间流暂停
        /// </summary>
        /// <param name="pause">暂停开关 true暂停时间流 false恢复时间流</param>
        internal void SetTimeFlowPauseES(bool pause)
        {
            isTimeFlowPause = pause;
        }

        /// <summary>
        /// 关闭时间流
        /// <para>关闭后无法在此对象唤醒</para>
        /// </summary>
        internal void CloseTimeFlowES()
        {
            isTimeFlowPause = true;
            isTimeFlowStop = true;
        }

        /*
        /// <summary>
        /// 关闭程序中所有时间流
        /// <para>调用此函数，在此次进程中无法再次启动</para>
        /// </summary>
        internal static void CloseAllTimeFlowES()
        {
            TimeFlowManager.Instance.Destroy();
        }
        */

        /// <summary>
        /// 通过对象关闭时间流
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <returns></returns>
        internal bool CloseByObj(ITimeUpdate timeUpdate)
        {
            if (reference.TryGetTarget(out var target) && target == timeUpdate)
            {
                CloseTimeFlowES();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 内部 更新
        /// </summary>
        /// <param name="dt"></param>
        internal void UpdateES(double dt)
        {
            // stopwatch.Start();
            long nanoDt = (long)dt * 1000000000L;
            if (reference.TryGetTarget(out var iTimeUpdate))
            {
                // 首次处理忽略之前的值
                if (firstUpdate)
                {
                    firstUpdate = false;
                    consumeTime = nanoDt;
                }
                var temp = nanoDt - consumeTime;
                // 正常更新
                if (IsIdle)
                {
                    IsIdle = false;
                    if (fixedTime == 0)
                    {
                        if (IsSync)
                        {
                            iTimeUpdate.Update((int)(temp / 1000000L));
                            IsIdle = true;
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                iTimeUpdate.Update((int)(temp / 1000000L));
                                IsIdle = true;
                            });
                        }
                    }
                    else
                    {
                        // 修正更新
                        var consumeFixedTime = notConsumeFixedTime + temp;
                        notConsumeFixedTime = consumeFixedTime % fixedNanoTime;
                        var count = consumeFixedTime / fixedNanoTime;
                        if (IsSync)
                        {
                            for (int i = 0; i < count; i++)
                                iTimeUpdate.Update(fixedTime);
                            IsIdle = true;
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                for (int i = 0; i < count; i++)
                                    iTimeUpdate.Update(fixedTime);
                                IsIdle = true;
                            });
                        }
                    }
                    consumeTime = nanoDt;
                }
            }
            // Interlocked.Exchange(ref lastUseTime, (int)stopwatch.ElapsedMilliseconds);
            // stopwatch.Reset();
        }

        /// <summary>
        /// 内部 停止更新
        /// </summary>
        internal void UpdateEndES()
        {
            if (reference.TryGetTarget(out var iTimeUpdate))
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    iTimeUpdate.UpdateEnd();
                });
            }
        }
    }
}
