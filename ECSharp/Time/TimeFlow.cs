namespace ECSharp.Time
{
    /// <summary>
    /// 时间流 
    /// <para>继承此类可以实现Update实时更新功能</para>
    /// <para>为了方便类的部分初始和性能节省需手动调用 Start(); 函数</para>
    /// <para>每次Update是先执行函数体内容再睡眠等待，所以如果需要精确的时间间隔应当先判定时间再累加时间</para>
    /// </summary>
    public sealed class TimeFlow : BaseTimeFlow
    {
        /// <summary>
        /// 时间流暂停开关 
        /// <para>只读 通过 Pause 函数修改</para>
        /// </summary>
        public bool IsPause { get { return isTimeFlowStop; } }

        /// <summary>
        /// 时间流停止开关
        /// <para>只读 通过 Close 函数修改</para>
        /// </summary>
        public bool IsStop { get { return isTimeFlowStop; } }

#if !UNITY_2020_1_OR_NEWER
        /// <summary>
        /// 时间流更新间隔
        /// <para>默认为 TimeInterval.Interval_16ms </para>
        /// </summary>
        public static TimeInterval TimeInterval
        {
            get { return (TimeInterval)TimeFlowThread.Interval; }
            set { TimeFlowThread.Interval = (int)value; }
        }
#endif

#if !UNITY_2020_1_OR_NEWER
        /// <summary>
        /// 构造函数 内部使用
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <param name="isSync">同步标记</param>
        /// <param name="fixedTime">修正时间, 不能低于 TimeInterval </param>
#else
        /// <summary>
        /// 构造函数 内部使用
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <param name="isSync">同步标记</param>
        /// <param name="fixedTime">修正时间, 不能低于 Unity设置的更新间隔周期 </param>
#endif
        private TimeFlow(ITimeUpdate timeUpdate, bool isSync, int fixedTime) : base(timeUpdate, isSync, fixedTime) { }

#if !UNITY_2020_1_OR_NEWER
        /// <summary>
        /// 创建一个时间流
        /// </summary>
        /// <param name="timeUpdate">更新回调接口</param>
        /// <param name="period">刷新周期 单位：毫秒 [不能低于 TimeInterval]</param>
#else
        /// <summary>
        /// 创建一个时间流
        /// </summary>
        /// <param name="timeUpdate">更新回调接口</param>
        /// <param name="period">刷新周期 单位：毫秒 [不能低于 Unity设置的更新间隔周期]</param>
#endif
        public static TimeFlow Create(ITimeUpdate timeUpdate, int period = 0)
        {
            return new TimeFlow(timeUpdate, false, period);
        }

#if !UNITY_2020_1_OR_NEWER
        /// <summary>
        /// 创建一个同步时间流
        /// <para>通过此函数创建的时间流将始终都处于一个线程运行</para>
        /// </summary>
        /// <param name="timeUpdate">更新回调接口</param>
        /// <param name="period">刷新周期 单位：毫秒 [不能低于 TimeInterval]</param>
#else
        /// <summary>
        /// 创建一个同步时间流
        /// <para>通过此函数创建的时间流将始终都处于一个线程运行</para>
        /// </summary>
        /// <param name="timeUpdate">更新回调接口</param>
        /// <param name="period">刷新周期 单位：毫秒 [不能低于 Unity设置的更新间隔周期]</param>
#endif
        public static TimeFlow CreateSync(ITimeUpdate timeUpdate, int period = 0)
        {
            return new TimeFlow(timeUpdate, true, period);
        }

        /// <summary>
        /// 开始时间流
        /// </summary>
        public TimeFlow Start()
        {
            StartTimeFlowES();
            return this;
        }

        /// <summary>
        /// 时间流暂停
        /// </summary>
        public void Pause()
        {
            SetTimeFlowPauseES(true);
        }

        /// <summary>
        /// 关闭时间流
        /// <para>关闭后无法在此对象唤醒</para>
        /// <para>如果可能尽可能在不再使用时调用此函数</para>
        /// </summary>
        public void Close()
        {
            CloseTimeFlowES();
        }
    }
}
