#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
namespace ECSharp
{
    /// <summary>
    /// 日志配置
    /// <para>配置修改建议在第一次调用Log前修改完成，避免出现奇怪的问题</para>
    /// </summary>
    public static class LogConfig
    {
        /// <summary>
        /// 日志控制台异步输出开关
        /// <para>异步输出采用其它线程处理，调试时可能出现意外，建议调试采用同步输出</para>
        /// <para>默认关闭</para>
        /// </summary>
        public static bool LOG_CONSOLE_ASYNC_OUTPUT = false;
        /// <summary>
        /// 日志控制台堆栈跟踪输出
        /// <para>开启后控制台中日志将写入堆栈信息</para>
        /// <para>默认关闭</para>
        /// </summary>
        public static bool LOG_CONSOLE_STACK_TRACE_OUTPUT = false;
        /// <summary>
        /// 日志文件堆栈跟踪输出
        /// <para>开启后日志文件中日志将写入堆栈信息</para>
        /// <para>默认开启</para>
        /// </summary>
        public static bool LOG_FILE_STACK_TRACE_OUTPUT = true;
        /// <summary>
        /// 控制台输出最小日志类型 默认：调试性日志
        /// </summary>
        public static LogType CONSOLE_OUTPUT_LOG_TYPE = LogType.DEBUG;
        /// <summary>
        /// 文件输出最小日志类型 默认：信息性日志
        /// </summary>
        public static LogType FILE_OUTPUT_LOG_TYPE = LogType.INFO;
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
        /// 单位 byte 默认 10MB大小
        /// </summary>
        public static long LOG_UNIT_FILE_MAX_SIZE = 1048576;
        /// <summary>
        /// 日志根路径
        /// </summary>
#if UNITY_2020_1_OR_NEWER
        public static string LOG_PATH = UnityEngine.Application.persistentDataPath + "/logs/"; 
#else
        public static string LOG_PATH = Utils.SystemInfo.Path + "logs/";
#endif
        /// <summary>
        /// 调试性 日志字体颜色
        /// </summary>
        public static System.ConsoleColor FOREGROUND_DEBUG_COLOR = System.ConsoleColor.DarkGray;
        /// <summary>
        /// 信息性 日志字体颜色
        /// </summary>
        public static System.ConsoleColor FOREGROUND_INFO_COLOR = System.ConsoleColor.Green;
        /// <summary>
        /// 警告性 日志字体颜色
        /// </summary>
        public static System.ConsoleColor FOREGROUND_WARN_COLOR = System.ConsoleColor.DarkYellow;
        /// <summary>
        /// 错误性 日志字体颜色
        /// </summary>
        public static System.ConsoleColor FOREGROUND_ERROR_COLOR = System.ConsoleColor.Red;
        /// <summary>
        /// 异常 日志字体颜色
        /// </summary>
        public static System.ConsoleColor FOREGROUND_EXCEPTION_COLOR = System.ConsoleColor.Yellow;
        /// <summary>
        /// 输入 日志字体颜色
        /// </summary>
        public static System.ConsoleColor FOREGROUND_INPUT_COLOR = System.ConsoleColor.Blue;
        /// <summary>
        /// 信息性 日志字体背景颜色
        /// </summary>
        public static System.ConsoleColor? BACKGROUND_INFO_COLOR = null;
        /// <summary>
        /// 调试性 日志字体背景颜色
        /// </summary>
        public static System.ConsoleColor? BACKGROUND_DEBUG_COLOR = null;
        /// <summary>
        /// 警告性 日志字体背景颜色
        /// </summary>
        public static System.ConsoleColor? BACKGROUND_WARN_COLOR = null;
        /// <summary>
        /// 错误性 日志字体背景颜色
        /// </summary>
        public static System.ConsoleColor? BACKGROUND_ERROR_COLOR = null;
        /// <summary>
        /// 异常 日志字体背景颜色
        /// </summary>
        public static System.ConsoleColor? BACKGROUND_EXCEPTION_COLOR = System.ConsoleColor.DarkRed;
        /// <summary>
        /// 输入 日志字体背景颜色
        /// </summary>
        public static System.ConsoleColor? BACKGROUND_INPUT_COLOR = null;
    }
}
