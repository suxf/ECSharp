namespace ES.Time
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
        /// <para>只读 通过 Close/CloseAll 函数修改</para>
        /// </summary>
        public bool IsStop { get { return isTimeFlowStop; } }

        /// <summary>
        /// 高精度模式
        /// <para>高精度模式会侵占所有CPU性能，但运算精度。</para>
        /// </summary>
        public static bool IsHighPrecisionMode { get { return TimeFlowThread.isHighPrecisionMode; } }

        /// <summary>
        /// 构造函数 内部使用
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <param name="isSync">同步标记</param>
        /// <param name="fixedTime">修正时间</param>
        private TimeFlow(ITimeUpdate timeUpdate, bool isSync, int fixedTime) : base(timeUpdate, isSync, fixedTime) { }

        /// <summary>
        /// 创建一个时间流
        /// </summary>
        /// <param name="timeUpdate">更新回调接口</param>
        /// <param name="period">刷新周期 单位：毫秒 [值如果是0为实时刷新，将不受周期修正，需要自行平衡时间，大于0的情况刷新间隔固定为此值]</param>
        public static TimeFlow Create(ITimeUpdate timeUpdate, int period = 10)
        {
            return new TimeFlow(timeUpdate, false, period);
        }

        /// <summary>
        /// 创建一个同步时间流
        /// <para>通过此函数创建的时间流将始终都处于一个线程运行</para>
        /// </summary>
        /// <param name="timeUpdate">更新回调接口</param>
        /// <param name="period">刷新周期 单位：毫秒 [值如果是0为实时刷新，将不受周期修正，需要自行平衡时间，大于0的情况刷新间隔固定为此值]</param>
        public static TimeFlow CreateSync(ITimeUpdate timeUpdate, int period = 10)
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
        /// 设置高精度模式
        /// <para>在某些情况的服务中需要毫秒级无误差支持可打开此项</para>
        /// <para>在机器设备支持的情况下可支持毫秒无误差更新，具体视执行函数体内容而定</para>
        /// <para>此模式会占用更多的CPU资源，低精度不占用CPU资源，但会存在毫秒误差，不过结果趋向是相同的</para>
        /// <para>默认是低精度模式，可在任何时间变更精度模式，要注意精度模式是影响整个框架的所有更新模式</para>
        /// </summary>
        /// <param name="state">打开状态</param>
        public static void SetHighPrecisionMode(bool state)
        {
            TimeFlowThread.isHighPrecisionMode = state;
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

        /*
        /// <summary>
        /// 关闭程序中所有时间流
        /// <para>调用此函数，在此次进程中无法再次启动</para>
        /// </summary>
        public static void CloseAll()
        {
            CloseAllTimeFlowES();
        }
        */
    }
}
