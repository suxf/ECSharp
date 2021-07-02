namespace ES.Common.Time
{
    /// <summary>
    /// 时间流 
    /// <para>[多线程处理逻辑] Update以10ms周期循环</para>
    /// <para>继承此类可以实现Update实时更新功能</para>
    /// <para>为了方便类的部分初始和性能节省需手动调用 Start(); 函数</para>
    /// <para>每次Update是先执行函数体内容再睡眠等待，所以如果需要精确的时间间隔应当先判定时间再累加时间</para>
    /// </summary>
    public sealed class TimeFlow : BaseTimeFlow
    {
        /// <summary>
        /// 获取时间流固定周期
        /// <para>刷新固定时间：10ms</para>
        /// </summary>
        public const int period = TimeFlowManager.timeFlowPeriod;

        /// <summary>
        /// 时间流暂停开关 
        /// <para>只读 通过 Pause 函数修改</para>
        /// </summary>
        public bool isPause { get { return isTimeFlowStop; } }

        /// <summary>
        /// 时间流停止开关
        /// <para>只读 通过 Close/CloseAll 函数修改</para>
        /// </summary>
        public bool isStop { get { return isTimeFlowStop; } }

        /// <summary>
        /// 构造函数 多线程处理逻辑
        /// </summary>
        private TimeFlow(ITimeUpdate timeUpdate) : base(timeUpdate) { }

        /// <summary>
        /// 构造函数 内部使用
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <param name="tfIndex">数组前两个线程是给框架使用，0负责数据部分 1负责文件部分</param>
        private TimeFlow(ITimeUpdate timeUpdate, int tfIndex) : base(timeUpdate, tfIndex) { }

        /// <summary>
        /// 创建一个时间流
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <returns></returns>
        public static TimeFlow Create(ITimeUpdate timeUpdate)
        {
            return new TimeFlow(timeUpdate);
        }

        /// <summary>
        /// 创建一个同步时间流
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <returns></returns>
        public static TimeFlow CreateSync(ITimeUpdate timeUpdate)
        {
            return new TimeFlow(timeUpdate, 2);
        }

        /// <summary>
        /// 开始时间流
        /// </summary>
        public void Start()
        {
            StartTimeFlowES();
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

        /// <summary>
        /// 关闭程序中所有时间流
        /// <para>调用此函数，在此次进程中无法再次启动</para>
        /// </summary>
        public static void CloseAll()
        {
            CloseAllTimeFlowES();
        }
    }
}
