#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;
using System.Collections.Generic;

namespace ECSharp.Time
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
        internal readonly Action? handle = null;
        /// <summary>
        /// 带参数执行的函数
        /// </summary>
        private readonly Action<object?>? handleWithParam = null;

        private object? parameter = null;

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
        public readonly int repeatNum;
        private int repeatNumNow = 0;

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
        /// <param name="isSync">同步标记</param>
        private TimeCaller(int delayTime, int periodTime, int repeatNum, Action handle, bool isSync)
        {
            this.delayTime = delayTime;
            this.periodTime = periodTime;
            this.repeatNum = repeatNum;
            this.handle = handle;

            timeFlow = BaseTimeFlow.CreateTimeFlow(this, isSync);
        }

        /// <summary>
        /// 创建一个时间执行器
        /// </summary>
        /// <param name="delayTime">第一次开始延迟时间，单位ms</param>
        /// <param name="periodTime">每次周期时间【第二次之后开始执行的延迟时间】，单位ms</param>
        /// <param name="repeatNum">重复次数，值为 -1 时 无限循环，默认 1 次</param>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="isSync">同步标记</param>
        private TimeCaller(int delayTime, int periodTime, int repeatNum, Action<object?> handle, bool isSync)
        {
            this.delayTime = delayTime;
            this.periodTime = periodTime;
            this.repeatNum = repeatNum;
            handleWithParam = handle;

            timeFlow = BaseTimeFlow.CreateTimeFlow(this, isSync);
        }

        /// <summary>
        /// 创建一个时间执行器
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="delayTime">第一次开始延迟时间，单位ms</param>
        /// <param name="periodTime">每次周期时间【第二次之后开始执行的延迟时间】，单位ms</param>
        /// <param name="repeatNum">重复次数，值为 -1 时 无限循环，默认 1 次</param>
        public static TimeCaller Create(Action handle, int delayTime, int periodTime = 0, int repeatNum = 1)
        {
            return new TimeCaller(delayTime, periodTime, repeatNum, handle, false);
        }

        /// <summary>
        /// 创建一个时间执行器
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="delayTime">第一次开始延迟时间，单位ms</param>
        /// <param name="periodTime">每次周期时间【第二次之后开始执行的延迟时间】，单位ms</param>
        /// <param name="repeatNum">重复次数，值为 -1 时 无限循环，默认 1 次</param>
        public static TimeCaller Create(Action<object?> handle, int delayTime, int periodTime = 0, int repeatNum = 1)
        {
            return new TimeCaller(delayTime, periodTime, repeatNum, handle, false);
        }

        /// <summary>
        /// 创建一个同步时间执行器
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="delayTime">第一次开始延迟时间，单位ms</param>
        /// <param name="periodTime">每次周期时间【第二次之后开始执行的延迟时间】，单位ms</param>
        /// <param name="repeatNum">重复次数，值为 -1 时 无限循环，默认 1 次</param>
        public static TimeCaller CreateSync(Action handle, int delayTime, int periodTime = 0, int repeatNum = 1)
        {
            return new TimeCaller(delayTime, periodTime, repeatNum, handle, true);
        }

        /// <summary>
        /// 创建一个同步时间执行器
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="delayTime">第一次开始延迟时间，单位ms</param>
        /// <param name="periodTime">每次周期时间【第二次之后开始执行的延迟时间】，单位ms</param>
        /// <param name="repeatNum">重复次数，值为 -1 时 无限循环，默认 1 次</param>
        public static TimeCaller CreateSync(Action<object?> handle, int delayTime, int periodTime = 0, int repeatNum = 1)
        {
            return new TimeCaller(delayTime, periodTime, repeatNum, handle, true);
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="isDaemon">是否守护执行,守护执行后不再需要保存执行器对象</param>
        /// <returns></returns>
        public TimeCaller Start(bool isDaemon = false)
        {
            if (isDaemon)
            {
                lock (timeCallers)
                {
                    timeCallers.Add(this);
                }
            }

            timeFlow.StartTimeFlowES();
            return this;
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="parameter">需要传递的参数</param>
        /// <param name="isDaemon">是否守护执行,守护执行后不再需要保存执行器对象</param>
        /// <returns></returns>
        public TimeCaller Start(object? parameter, bool isDaemon = false)
        {
            if (isDaemon)
            {
                lock (timeCallers)
                {
                    timeCallers.Add(this);
                }
            }

            this.parameter = parameter;

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
            if (timeCallers.Count <= 0)
            {
                return;
            }

            lock (timeCallers)
            {
                for (int i = 0, len = timeCallers.Count; i < len; i++)
                {
                    timeCallers[i].Cancel();
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

                if (repeatNum != -1)
                    ++repeatNumNow;

                handle?.Invoke();
                handleWithParam?.Invoke(parameter);
                return;
            }

            periodTimeNow += deltaTime;
            int count = periodTimeNow / periodTime;
            if (count == 0)
                return;

            if (repeatNum != -1 && count + repeatNumNow >= repeatNum)
            {
                count = repeatNum - repeatNumNow;
            }

            periodTimeNow %= periodTime;
            for (int i = 0; i < count; i++)
            {
                if (repeatNum != -1)
                    ++repeatNumNow;
                handle?.Invoke();
                handleWithParam?.Invoke(parameter);
            }
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
