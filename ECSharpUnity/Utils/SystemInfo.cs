#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif

namespace ECSharp.Utils
{
    /// <summary>
    /// 系统信息
    /// </summary>
    public static class SystemInfo
    {
        /// <summary>
        /// 系统版本
        /// </summary>
        public static string SystemVersion { get; } = System.Environment.OSVersion.ToString();
        /// <summary>
        /// DotNet版本
        /// </summary>
        public static string DotNetVersion { get; } = System.Environment.Version.ToString();
        /// <summary>
        /// 用户名称
        /// </summary>
        public static string UserName { get; } = System.Environment.UserName;
        /// <summary>
        /// 当前执行路径
        /// </summary>
        public static string Path { get; } = System.Environment.CurrentDirectory + "\\";
        /// <summary>
        /// 进程名称
        /// </summary>
        public static string ProcessName { get; } = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name?.ToString() ?? "";
        /// <summary>
        /// 进程版本
        /// </summary>
        public static string ProcessVersion { get; } = System.Text.RegularExpressions.Regex.Match(System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "", "[0-9]+\\.[0-9]+\\.[0-9]+").Value;
        /// <summary>
        /// 框架版本
        /// </summary>
        public static string FrameVersion { get; } = System.Text.RegularExpressions.Regex.Match(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "", "[0-9]+\\.[0-9]+\\.[0-9]+").Value;
        /// <summary>
        /// 逻辑线程数
        /// </summary>
        public static int ProcessorCount { get; } = System.Environment.ProcessorCount;
        /// <summary>
        /// 程序总运行时间(毫秒)
        /// <para>实际时间是从调用框架功能开始计算</para>
        /// <para>内部使用 Stopwatch 类实现</para>
        /// </summary>
        public static long TotalRunTime => LocalTime.ElapsedMilliseconds;
    }
}
