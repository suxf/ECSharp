namespace ES.Common.Time
{
    /// <summary>
    /// <para>时间流 [单线程处理逻辑] Update以10ms周期循环</para>
    /// <para>继承此类可以实现Update实时更新功能</para>
    /// <para>每次Update被调用都是Sleep过需要的时间，也就是说执行逻辑的时候已经等待了响应间隔周期了</para>
    /// <para>继承此类的对象全部会在同一个线程中运行，需要多线程请使用TimeFlow类</para>
    /// </summary>
    public abstract class SyncTimeFlow : TimeFlow
    {
        /// <summary>
        /// 构造函数 单线程处理逻辑
        /// <para>继承此类的对象全部会在同一个线程中运行，需要多线程请使用TimeFlow类</para>
        /// </summary>
        public SyncTimeFlow() : base(2) { }
    }
}
