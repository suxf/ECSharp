namespace ES.Common.Log
{
    /// <summary>
    /// 日志配置器
    /// <para>配置修改建议在第一次调用Log前修改完成，避免出现奇怪的问题</para>
    /// </summary>
    public static class LogConfig
    {
        /// <summary>
        /// 日志控制台输出开关 默认关闭
        /// </summary>
        public static bool LOG_CONSOLE_OUTPUT = false;
        /// <summary>
        /// 日志写入周期 单位 ms
        /// </summary>
        public static int LOG_PERIOD = 1000;
        /// <summary>
        /// 日志写入文件后缀
        /// </summary>
        public static string LOG_FILE_SUFFIX = ".log";
        /// <summary>
        /// 日志单个文件最多大小
        /// 单位 byte 默认 50MB大小
        /// </summary>
        public static long LOG_UNIT_FILE_MAX_SIZE = 52428800;
        /// <summary>
        /// 日志根路径
        /// </summary>
        public static string LOG_PATH = "./log/";
    }
}
