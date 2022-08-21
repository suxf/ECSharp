#if !UNITY_2020_1_OR_NEWER
namespace ECSharp.Time
{
    /// <summary>
    /// 时间间隔
    /// <para>用于控制时间流更新最小间隔 单位：毫秒</para>
    /// <para>间隔越小消耗性能越大</para>
    /// </summary>
    public enum TimeInterval
    {
        /// <summary>
        /// 间隔0毫秒
        /// <para>无间隔更新会导致CPU时间片占满，但精确度最高</para>
        /// </summary>
        Interval_0ms = 0,
        /// <summary>
        /// 间隔1毫秒
        /// <para>实际间隔时间根据系统内核分辨率决定，windows下为16ms左右</para>
        /// </summary>
        Interval_1ms = 1,
        /// <summary>
        /// 间隔2毫秒
        /// <para>实际间隔时间根据系统内核分辨率决定，windows下为16ms左右</para>
        /// </summary>
        Interval_2ms = 2,
        /// <summary>
        /// 间隔4毫秒
        /// <para>实际间隔时间根据系统内核分辨率决定，windows下为16ms左右</para>
        /// </summary>
        Interval_4ms = 4,
        /// <summary>
        /// 间隔8毫秒
        /// <para>实际间隔时间根据系统内核分辨率决定，windows下为16ms左右</para>
        /// </summary>
        Interval_8ms = 8,
        /// <summary>
        /// 间隔10毫秒
        /// <para>实际间隔时间根据系统内核分辨率决定，windows下为16ms左右</para>
        /// </summary>
        Interval_10ms = 10,
        /// <summary>
        /// 间隔16毫秒 (默认间隔)
        /// </summary>
        Interval_16ms = 16,
        /// <summary>
        /// 间隔20毫秒 (Unity默认间隔)
        /// </summary>
        Interval_20ms = 20,
        /// <summary>
        /// 间隔30毫秒
        /// </summary>
        Interval_30ms = 30,
        /// <summary>
        /// 间隔32毫秒
        /// </summary>
        Interval_32ms = 32,
        /// <summary>
        /// 间隔40毫秒
        /// </summary>
        Interval_40ms = 40,
        /// <summary>
        /// 间隔50毫秒
        /// </summary>
        Interval_50ms = 50,
        /// <summary>
        /// 间隔64毫秒
        /// </summary>
        Interval_64ms = 64,
        /// <summary>
        /// 间隔100毫秒
        /// </summary>
        Interval_100ms = 100,
        /// <summary>
        /// 间隔200毫秒
        /// </summary>
        Interval_200ms = 200,
        /// <summary>
        /// 间隔500毫秒
        /// </summary>
        Interval_500ms = 500,
        /// <summary>
        /// 间隔1000毫秒
        /// </summary>
        Interval_1000ms = 1000,
    }
}
#endif