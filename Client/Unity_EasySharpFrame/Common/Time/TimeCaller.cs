namespace ES.Common.Time
{
    /// <summary>
    /// 时间执行器
    /// <para>此执行器多线程分配</para>
    /// <para>需要统一线程调度请使用SyncTimeCaller</para>
    /// </summary>
    public class TimeCaller : BaseTimeFlow
    {
        /// <summary>
        /// 回调执行的函数
        /// </summary>
        /// <param name="count">执行次数</param>
        public delegate void MethodHandle(long count);
        private MethodHandle handle = null;

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
        /// 是否重复
        /// </summary>
        public readonly bool isRepeat;
        /// <summary>
        /// 重复次数
        /// </summary>
        public readonly long repeatNum;
        private long repeatNumNow = 0;

        /// <summary>
        /// 创建一个时间执行器
        /// </summary>
        /// <param name="delayTime">第一次开始延迟时间，单位ms</param>
        /// <param name="periodTime">每次周期时间【第二次之后开始执行的延迟时间】，单位ms</param>
        /// <param name="isRepeat">是否重复状态，不重复状态下周期时间无效, 默认不重复</param>
        /// <param name="repeatNum">重复次数，值为 -1时 无限循环，默认 -1</param>
        /// <param name="handle">需要被执行的函数</param>
        public TimeCaller(int delayTime, int periodTime, bool isRepeat = false, long repeatNum = -1, MethodHandle handle = null)
        {
            this.delayTime = delayTime;
            this.periodTime = periodTime;
            this.isRepeat = isRepeat;
            this.repeatNum = repeatNum;
            this.handle = handle;

            StartTimeFlowES();
        }

        /// <summary>
        /// 创建一个时间执行器
        /// </summary>
        /// <param name="delayTime">第一次开始延迟时间，单位ms</param>
        /// <param name="periodTime">每次周期时间【第二次之后开始执行的延迟时间】，单位ms</param>
        /// <param name="isRepeat">是否重复状态，不重复状态下周期时间无效, 默认不重复</param>
        /// <param name="repeatNum">重复次数，值为 -1时 无限循环，默认 -1</param>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="timeFlowIndex">时间流线程索引</param>
        protected TimeCaller(int delayTime, int periodTime, bool isRepeat, long repeatNum, MethodHandle handle, int timeFlowIndex) : base(timeFlowIndex)
        {
            this.delayTime = delayTime;
            this.periodTime = periodTime;
            this.isRepeat = isRepeat;
            this.repeatNum = repeatNum;
            this.handle = handle;

            StartTimeFlowES();
        }

        /// <summary>
        /// 需要被调用的函数
        /// </summary>
        /// <param name="handle"></param>
        public void CallMethod(MethodHandle handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// 取消时间执行器任务
        /// </summary>
        public void CancelTimeCall()
        {
            CloseTimeFlowES();
        }

        /// <summary>
        /// 系统调用
        /// </summary>
        /// <param name="dt"></param>
        protected override void Update(int dt)
        {
            if (isFirstCall)
            {
                delayTimeNow += timeFlowPeriod;
                if (delayTimeNow >= delayTime)
                {
                    isFirstCall = false;
                    if (handle != null) handle.Invoke(++repeatNumNow);
                }
            }
            else
            {
                if (repeatNum == -1 || repeatNumNow < repeatNum)
                {
                    periodTimeNow += timeFlowPeriod;
                    if (periodTimeNow >= periodTime)
                    {
                        periodTimeNow = 0;
                        if (handle != null) handle.Invoke(++repeatNumNow);
                    }
                }
                if (!isRepeat || (isRepeat && repeatNum != -1 && repeatNumNow >= repeatNum)) CloseTimeFlowES();
            }
        }

        /// <summary>
        /// 停止更新
        /// </summary>
        protected override void OnUpdateEnd()
        {
        }
    }
}
