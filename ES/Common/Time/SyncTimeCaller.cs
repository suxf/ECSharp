namespace ES.Common.Time
{
    /// <summary>
    /// 时间执行器
    /// 此执行器是SyncTimeFlow同一个线程执行
    /// </summary>
    public class SyncTimeCaller : TimeCaller
    {
        /// <summary>
        /// 创建一个同步时间执行器
        /// 此执行器是SyncTimeFlow同一个线程执行
        /// </summary>
        /// <param name="delayTime">第一次开始延迟时间，单位ms</param>
        /// <param name="periodTime">每次周期时间【第二次之后开始执行的延迟时间】，单位ms</param>
        /// <param name="isRepeat">是否重复状态，不重复状态下周期时间无效, 默认不重复</param>
        /// <param name="repeatNum">重复次数，值为 -1时 无限循环，默认 -1</param>
        /// <param name="handle">需要被执行的函数</param>
        public SyncTimeCaller(int delayTime, int periodTime, bool isRepeat = false, long repeatNum = -1, MethodHandle handle = null) : base(delayTime, periodTime, isRepeat, repeatNum, handle, 2) { }
    }
}
