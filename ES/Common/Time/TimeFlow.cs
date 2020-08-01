namespace ES.Common.Time
{
    /// <summary>
    /// 时间流 抽象类 Update以10ms周期循环
    /// 继承此类可以实现Update实时更新功能
    /// 每次Update被调用都是Sleep过需要的时间，也就是说执行逻辑的时候已经等待了响应间隔周期了
    /// </summary>
    public abstract class TimeFlow
    {
        /// <summary>
        /// 获取时间流固定周期
        /// 刷新固定时间：10ms
        /// </summary>
        public const int timeFlowPeriod = TimeFlowManager.timeFlowPeriod;
        /// <summary>
        /// 时间流暂停开关
        /// </summary>
        internal bool isTimeFlowPause = false;
        /// <summary>
        /// 时间流暂停开关
        /// </summary>
        public bool IsTimeFlowPause { get { return isTimeFlowStop; } }
        /// <summary>
        /// 时间流停止开关
        /// </summary>
        internal bool isTimeFlowStop = false;
        /// <summary>
        /// 时间流停止开关
        /// </summary>
        public bool IsTimeFlowStop { get { return isTimeFlowStop; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TimeFlow()
        {
            TimeFlowManager.Instance.PushTimeFlow(this);
        }

        /// <summary>
        /// 构造函数 内部使用
        /// </summary>
        /// <param name="tfIndex">数组前两个线程是给框架使用，0负责数据部分 1负责文件部分</param>
        internal TimeFlow(int tfIndex)
        {
            TimeFlowManager.Instance.PushTimeFlow(this, tfIndex);
        }

        /// <summary>
        /// 设置时间流暂停
        /// </summary>
        /// <param name="pause">暂停开关 true暂停时间流 false恢复时间流</param>
        public void SetTimeFlowPause(bool pause)
        {
            isTimeFlowPause = pause;
        }

        /// <summary>
        /// 关闭时间流
        /// 关闭后无法在此对象唤醒
        /// </summary>
        public void CloseTimeFlow()
        {
            isTimeFlowStop = true;
        }

        /// <summary>
        /// 关闭程序中所有时间流
        /// 调用此函数，在此次进程中无法再次启动
        /// </summary>
        public static void CloseAllTimeFlow()
        {
            TimeFlowManager.Instance.Destroy();
        }

        /// <summary>
        /// 抽象更新
        /// </summary>
        /// <param name="dt">时间差，实际执行时间 减去 理论周期时间10ms 精度：ms</param>
        public abstract void Update(int dt);
    }
}
