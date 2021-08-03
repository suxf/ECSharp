namespace ES.Common.Log
{
    /// <summary>
    /// 日志类
    /// <para>调用此日志类可以应对高速写入日志需求</para>
    /// <para>日志周期性写入目标文件路径</para>
    /// <para>配置请参考 LogConfig.cs 类</para>
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// 日志控制台输出开关 默认开启
        /// <para>配置修改建议在第一次调用Log前修改完成，避免出现奇怪的问题</para>
        /// </summary>
        public static bool LOG_CONSOLE_OUTPUT = true;
        /// <summary>
        /// 日志写入周期 单位 ms
        /// <para>配置修改建议在第一次调用Log前修改完成，避免出现奇怪的问题</para>
        /// </summary>
        public static int LOG_PERIOD = 1000;
        /// <summary>
        /// 日志写入文件后缀
        /// <para>配置修改建议在第一次调用Log前修改完成，避免出现奇怪的问题</para>
        /// </summary>
        public static string LOG_FILE_SUFFIX = ".log";
        /// <summary>
        /// 日志单个文件最多大小
        /// 单位 byte 默认 50MB大小
        /// <para>配置修改建议在第一次调用Log前修改完成，避免出现奇怪的问题</para>
        /// </summary>
        public static long LOG_UNIT_FILE_MAX_SIZE = 52428800;
        /// <summary>
        /// 日志根路径
        /// <para>配置修改建议在第一次调用Log前修改完成，避免出现奇怪的问题</para>
        /// </summary>
        public static string LOG_PATH = "./log/";
        /// <summary>
        /// 信息性 日志字体颜色
        /// </summary>
        public static System.ConsoleColor FOREGROUND_INFO_COLOR = System.ConsoleColor.Green;
        /// <summary>
        /// 调试性 日志字体颜色
        /// </summary>
        public static System.ConsoleColor FOREGROUND_DEBUG_COLOR = System.ConsoleColor.Blue;
        /// <summary>
        /// 警告性 日志字体颜色
        /// </summary>
        public static System.ConsoleColor FOREGROUND_WARN_COLOR = System.ConsoleColor.Yellow;
        /// <summary>
        /// 错误性 日志字体颜色
        /// </summary>
        public static System.ConsoleColor FOREGROUND_ERROR_COLOR = System.ConsoleColor.Red;
        /// <summary>
        /// 异常 日志字体颜色
        /// </summary>
        public static System.ConsoleColor FOREGROUND_EXCEPTION_COLOR = System.ConsoleColor.DarkYellow;
        /// <summary>
        /// 信息性 日志字体背景颜色
        /// </summary>
        public static System.ConsoleColor BACKGROUND_INFO_COLOR = System.ConsoleColor.Black;
        /// <summary>
        /// 调试性 日志字体背景颜色
        /// </summary>
        public static System.ConsoleColor BACKGROUND_DEBUG_COLOR = System.ConsoleColor.Black;
        /// <summary>
        /// 警告性 日志字体背景颜色
        /// </summary>
        public static System.ConsoleColor BACKGROUND_WARN_COLOR = System.ConsoleColor.Black;
        /// <summary>
        /// 错误性 日志字体背景颜色
        /// </summary>
        public static System.ConsoleColor BACKGROUND_ERROR_COLOR = System.ConsoleColor.Black;
        /// <summary>
        /// 异常 日志字体背景颜色
        /// </summary>
        public static System.ConsoleColor BACKGROUND_EXCEPTION_COLOR = System.ConsoleColor.DarkRed;

        /// <summary>
        /// 信息性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        public static void Info(string log)
        {
            WriteLine("INFO", log);
        }

        /// <summary>
        /// 调试性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        public static void Debug(string log)
        {
            WriteLine("DEBUG", log);
        }

        /// <summary>
        /// 警告性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        public static void Warn(string log)
        {
            WriteLine("WARN", log);
        }

        /// <summary>
        /// 错误性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        public static void Error(string log)
        {
            WriteLine("ERROR", log);
        }

        /// <summary>
        /// 异常 日志
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="log">日志内容</param>
        public static void Exception(System.Exception ex, string log = "")
        {
            string data = string.Format("{0}\r\n   Message:{1}\r\n   Method:{2}\r\n   StackTrace:\r\n{3}", log, ex.Message, ex.TargetSite, ex.StackTrace);
            WriteLine("FATAL", data);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="log">日志数据</param>
        private static void WriteLine(string type, string log)
        {
            LogInfo logInfo = new LogInfo();
            logInfo.type = type;
            logInfo.data = log;
            // 压入队列
            LogManager.Instance.logInfos.Enqueue(logInfo);
        }
    }
}
