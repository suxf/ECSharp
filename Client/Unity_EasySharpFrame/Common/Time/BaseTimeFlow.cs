using System;
using System.Diagnostics;
using System.Threading;

namespace ES.Common.Time
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

        private readonly Stopwatch stopwatch = new Stopwatch();
        internal int lastUseTime = 0;

        private readonly WeakReference<ITimeUpdate> reference;

        /// <summary>
        /// 构造函数 多线程处理逻辑
        /// <para>继承此类的对象会分配在多个线程下运行，需要单线程请使用SyncTimeFlow类</para>
        /// </summary>
        protected BaseTimeFlow(ITimeUpdate timeUpdate)
        {
            reference = new WeakReference<ITimeUpdate>(timeUpdate);
            TimeFlowManager.Instance.PushTimeFlow(this);
        }

        /// <summary>
        /// 构造函数 内部使用
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <param name="tfIndex">数组前两个线程是给框架使用，0负责数据部分 1负责文件部分</param>
        protected BaseTimeFlow(ITimeUpdate timeUpdate, int tfIndex)
        {
            reference = new WeakReference<ITimeUpdate>(timeUpdate);
            TimeFlowManager.Instance.PushTimeFlow(this, tfIndex);
        }

        /// <summary>
        /// 创建基础时间流
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <param name="tfIndex">数组前两个线程是给框架使用，0负责数据部分 1负责文件部分</param>
        internal static BaseTimeFlow CreateTimeFlow(ITimeUpdate timeUpdate, int tfIndex = -1)
        {
            if (tfIndex == -1)
                return new BaseTimeFlow(timeUpdate);
            else
                return new BaseTimeFlow(timeUpdate, tfIndex);
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

        /// <summary>
        /// 关闭程序中所有时间流
        /// <para>调用此函数，在此次进程中无法再次启动</para>
        /// </summary>
        internal static void CloseAllTimeFlowES()
        {
            TimeFlowManager.Instance.Destroy();
        }

        /// <summary>
        /// 内部 更新
        /// </summary>
        /// <param name="dt"></param>
        internal void UpdateES(int dt)
        {
            stopwatch.Start();
            if(reference.TryGetTarget(out var iTimeUpdate)) iTimeUpdate.Update(dt);
            stopwatch.Stop();
            Interlocked.Exchange(ref lastUseTime, (int)stopwatch.Elapsed.TotalMilliseconds);
            stopwatch.Reset();
        }

        /// <summary>
        /// 内部 停止更新
        /// </summary>
        internal void UpdateEndES()
        {
            if (reference.TryGetTarget(out var iTimeUpdate)) iTimeUpdate.UpdateEnd();
        }
    }
}
