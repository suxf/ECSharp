using System;

namespace ES.Utils
{
    /// <summary>
    /// 系统信息
    /// </summary>
    public static class SystemInfo
    {
        /// <summary>
        /// 获取框架版本
        /// </summary>
        /// <returns></returns>
        public static string FrameVersion { get; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";
        /// <summary>
        /// 逻辑线程数
        /// </summary>
        public static int ProcessorCount { get; } = Environment.ProcessorCount;
    }
}
