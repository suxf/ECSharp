using System;
using System.Collections.Generic;

namespace ES.Time
{
    /// <summary>
    /// 时间执行器
    /// </summary>
    public class TimeCaller : ITimeUpdate
    {
        private readonly static List<TimeCaller> timeCallers = new List<TimeCaller>();

        /// <summary>
        /// 无限次数执行
        /// </summary>
        public const int Infinite = -1;

        /// <summary>
        /// 回调执行的函数
        /// </summary>
        private readonly Action<long> handle;

        /// <summary>
        /// 延迟时间
        /// </summary>
        public readonly int delayTime;
        private int delayTimeNow = 0;
        private bool isFirstCall = true;

        /// <summary>
        /// 周期时间
        /// </summary>
        public readonly int periodTime;
        private int periodTimeNow = 0;

        /// <summary>
        /// 重复次数
        /// </summary>
        public readonly long repeatNum;
        private long repeatNumNow = 0;

        private readonly BaseTimeFlow timeFlow;

        /// <summary>
        /// 是否已取消执行
        /// </summary>
        public bool IsCancel => timeFlow.isTimeFlowStop;

        /// <summary>
        /// 创建一个时间执行器
        /// </summary>
        /// <param name="delayTime">第一次开始延迟时间，单位ms</param>
        /// <param name="periodTime">每次周期时间【第二次之后开始执行的延迟时间】，单位ms</param>
        /// <param name="repeatNum">重复次数，值为 -1 时 无限循环，默认 1 次</param>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="tfIndex">时间流索引</param>
        private TimeCaller(int delayTime, int periodTime, long repeatNum, Action<long> handle, int tfIndex = -1)
        {
            this.delayTime = delayTime;
            this.periodTime = periodTime;
            this.repeatNum = repeatNum;
            this.handle = handle;

            if (tfIndex == -1)
                timeFlow = BaseTimeFlow.CreateTimeFlow(this);
            else
                timeFlow = BaseTimeFlow.CreateTimeFlow(this, tfIndex);
        }

        /// <summary>
        /// 创建一个时间执行器
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="delayTime">第一次开始延迟时间，单位ms</param>
        /// <param name="periodTime">每次周期时间【第二次之后开始执行的延迟时间】，单位ms</param>
        /// <param name="repeatNum">重复次数，值为 -1 时 无限循环，默认 1 次</param>
        public static TimeCaller Create(Action<long> handle, int delayTime, int periodTime = 0, long repeatNum = 1)
        {
            return new TimeCaller(delayTime, periodTime, repeatNum, handle);
        }

        /// <summary>
        /// 创建一个同步时间执行器
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="delayTime">第一次开始延迟时间，单位ms</param>
        /// <param name="periodTime">每次周期时间【第二次之后开始执行的延迟时间】，单位ms</param>
        /// <param name="repeatNum">重复次数，值为 -1 时 无限循环，默认 1 次</param>
        public static TimeCaller CreateSync(Action<long> handle, int delayTime, int periodTime = 0, long repeatNum = 1)
        {
            return new TimeCaller(delayTime, periodTime, repeatNum, handle, 2);
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="isDaemon">是否守护执行,守护执行后不再需要保存执行器对象</param>
        /// <returns></returns>
        public TimeCaller Start(bool isDaemon = false)
        {
            if (isDaemon) lock (timeCallers) timeCallers.Add(this);

            timeFlow.StartTimeFlowES();
            return this;
        }

        /// <summary>
        /// 取消时间执行器任务
        /// </summary>
        public void Cancel()
        {
            timeFlow.CloseTimeFlowES();
        }

        /// <summary>
        /// 取消所有守护的时间执行器任务
        /// </summary>
        public static void CancelAllDaemonTimeCalls()
        {
            lock (timeCallers)
            {
                foreach (var t in timeCallers)
                {
                    t.Cancel();
                }
                timeCallers.Clear();
            }
        }

        /// <summary>
        /// 系统调用
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(int deltaTime)
        {
            if (repeatNum != -1 && repeatNumNow >= repeatNum)
            {
                timeFlow.CloseTimeFlowES();
                return;
            }

            if (isFirstCall)
            {
                delayTimeNow += deltaTime;
                if (delayTimeNow < delayTime)
                    return;
                isFirstCall = false;
                handle?.Invoke(++repeatNumNow);
                return;
            }

            periodTimeNow += deltaTime;
            if (periodTimeNow < periodTime)
                return;

            periodTimeNow = 0;
            handle?.Invoke(++repeatNumNow);
        }

        /// <summary>
        /// 停止更新
        /// </summary>
        public void UpdateEnd()
        {
            lock (timeCallers) timeCallers.Remove(this);
        }
    }
}
