namespace ES.Common.Time
{
    /// <summary>
    /// 时间流 
    /// <para>[多线程处理逻辑] Update以10ms周期循环</para>
    /// <para>继承此类可以实现Update实时更新功能</para>
    /// <para>为了方便类的部分初始和性能节省需手动调用 StartTimeFlow(); 函数</para>
    /// <para>每次Update是先执行函数体内容再睡眠等待，所以如果需要精确的时间间隔应当先判定时间再累加时间</para>
    /// <para>继承此类的对象会分配在多个线程下运行，需要单线程请使用SyncTimeFlow类</para>
    /// </summary>
    public abstract class TimeFlow : BaseTimeFlow
    {
        /// <summary>
        /// 时间流暂停开关 
        /// <para>只读 修改通过 SetTimeFlowPause 函数</para>
        /// </summary>
        public bool IsTimeFlowPause { get { return isTimeFlowStop; } }

        /// <summary>
        /// 时间流停止开关
        /// <para>只读 修改通过 CloseTimeFlow 函数</para>
        /// </summary>
        public bool IsTimeFlowStop { get { return isTimeFlowStop; } }

        /// <summary>
        /// 构造函数 多线程处理逻辑
        /// <para>继承此类的对象会分配在多个线程下运行，需要单线程请使用SyncTimeFlow类</para>
        /// </summary>
        public TimeFlow() { }

        /// <summary>
        /// 构造函数 内部使用
        /// </summary>
        /// <param name="tfIndex">数组前两个线程是给框架使用，0负责数据部分 1负责文件部分</param>
        internal TimeFlow(int tfIndex) : base(tfIndex) { }

        /// <summary>
        /// 开始时间流
        /// </summary>
        public void StartTimeFlow()
        {
            StartTimeFlowES();
        }

        /// <summary>
        /// 设置时间流暂停
        /// </summary>
        /// <param name="pause">暂停开关 true暂停时间流 false恢复时间流</param>
        public void SetTimeFlowPause(bool pause)
        {
            SetTimeFlowPauseES(pause);
        }

        /// <summary>
        /// 关闭时间流
        /// <para>关闭后无法在此对象唤醒</para>
        /// <para>如果可能尽可能在不再使用时调用此函数</para>
        /// </summary>
        public void CloseTimeFlow()
        {
            CloseTimeFlowES();
        }

        /// <summary>
        /// 关闭程序中所有时间流
        /// <para>调用此函数，在此次进程中无法再次启动</para>
        /// </summary>
        public static void CloseAllTimeFlow()
        {
            CloseAllTimeFlowES();
        }
    }
}
