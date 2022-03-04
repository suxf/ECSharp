using ES.Common.Time;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace ES.Common.Log
{
    /// <summary>
    /// 日志管理器
    /// <para>周期性写入文件</para>
    /// <para>周期LOG_PERIOD、写入路径LOG_PATH和分文件大小限制LOG_UNIT_FILE_MAX_SIZE可以直接调用静态修改（程序启动时未第一次调用就应修改完成）</para>
    /// </summary>
    internal class LogManager : ITimeUpdate
    {
        /// <summary>
        /// 单例静态对象
        /// </summary>
        private static LogManager? instance = null;
        /// <summary>
        /// 获取单例
        /// </summary>
        internal static LogManager Instance { get { if (instance == null) instance = new LogManager(); return instance; } }

        /// <summary>
        /// 日志数据队列
        /// </summary>
        internal ConcurrentQueue<LogInfo> logInfos = new ConcurrentQueue<LogInfo>();
        /// <summary>
        /// 文件信息
        /// </summary>
        private FileInfo? fileInfo = null;
        /// <summary>
        /// 日志ID
        /// </summary>
        private readonly string logId;
        /// <summary>
        /// 日志索引，如果单个时间内日志太大则分开
        /// </summary>
        private int logIndex = 0;
        /// <summary>
        /// 进程名称
        /// </summary>
        private readonly string proccessName = "";

        private readonly BaseTimeFlow timeFlow;

        /// <summary>
        /// 构造函数
        /// </summary>
        private LogManager()
        {
            proccessName = Process.GetCurrentProcess().ProcessName.ToLower();
            logId = new Random().Next(100, 999).ToString();
            // 创建目录
            if (!Directory.Exists(LogConfig.LOG_PATH))
            {
                Directory.CreateDirectory(LogConfig.LOG_PATH);
            }

            timeFlow = BaseTimeFlow.CreateTimeFlow(this, 1);
            timeFlow.StartTimeFlowES();
        }

        private int periodNow = 0;
        /// <summary>
        /// 系统调用
        /// </summary>
        /// <param name="dt"></param>
        public void Update(int dt)
        {
            periodNow += dt;
            if (periodNow >= LogConfig.LOG_PERIOD)
            {
                periodNow = 0;

                // 如果没有日志则不处理
                if (logInfos.Count <= 0) return;
                // 创建当日目录
                if (!Directory.Exists(LogConfig.LOG_PATH + DateTime.Now.ToString("yyyy_MM_dd/")))
                {
                    Directory.CreateDirectory(LogConfig.LOG_PATH + DateTime.Now.ToString("yyyy_MM_dd/"));
                }
                string filename = LogConfig.LOG_PATH + string.Format(DateTime.Now.ToString("yyyy_MM_dd/{2}_HH_{0}_{1}{3}"), logIndex, logId, proccessName, LogConfig.LOG_FILE_SUFFIX);
                if (!File.Exists(filename)) fileInfo = null;
                // 检查文件
                if (fileInfo == null)
                    fileInfo = new FileInfo(filename);
                else
                    fileInfo.Refresh();
                if (fileInfo.Exists)
                {
                    if (fileInfo.Length > LogConfig.LOG_UNIT_FILE_MAX_SIZE)
                    {
                        fileInfo = new FileInfo(LogConfig.LOG_PATH + string.Format(DateTime.Now.ToString("yyyy_MM_dd/{2}_HH_{0}_{1}{3}"), ++logIndex, logId, proccessName, LogConfig.LOG_FILE_SUFFIX));
                        FileStream fs = fileInfo.Create();
                        fs.Close();
                        fileInfo.Refresh();
                    }
                }
                else
                {
                    FileStream fs = fileInfo.Create();
                    fs.Close();
                    fileInfo.Refresh();
                }

                // 写入日志
                while (logInfos.TryDequeue(out LogInfo? log))
                {
                    if (log.data == null) continue;
                    var logStr = OutputLog(log, LogConfig.LOG_CONSOLE_ASYNC_OUTPUT);
                    using (StreamWriter sw = fileInfo.AppendText()) sw.WriteLine(logStr);
                }
            }
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="log"></param>
        /// <param name="isWriteConsole"></param>
        internal static string OutputLog(LogInfo log, bool isWriteConsole)
        {
            string logType = "";
            switch (log.type)
            {
                case LogType.DEBUG:
                    if (isWriteConsole)
                    {
                        Console.ForegroundColor = LogConfig.FOREGROUND_DEBUG_COLOR;
                        Console.BackgroundColor = LogConfig.BACKGROUND_DEBUG_COLOR;
                    }
                    logType = "DEBUG";
                    break;
                case LogType.INFO:
                    if (isWriteConsole)
                    {
                        Console.ForegroundColor = LogConfig.FOREGROUND_INFO_COLOR;
                        Console.BackgroundColor = LogConfig.BACKGROUND_INFO_COLOR;
                    }
                    logType = "INFO";
                    break;
                case LogType.WARN:
                    if (isWriteConsole)
                    {
                        Console.ForegroundColor = LogConfig.FOREGROUND_WARN_COLOR;
                        Console.BackgroundColor = LogConfig.BACKGROUND_WARN_COLOR;
                    }
                    logType = "WARN";
                    break;
                case LogType.ERROR:
                    if (isWriteConsole)
                    {
                        Console.ForegroundColor = LogConfig.FOREGROUND_ERROR_COLOR;
                        Console.BackgroundColor = LogConfig.BACKGROUND_ERROR_COLOR;
                    }
                    logType = "ERROR";
                    break;
                case LogType.FATAL:
                    if (isWriteConsole)
                    {
                        Console.ForegroundColor = LogConfig.FOREGROUND_EXCEPTION_COLOR;
                        Console.BackgroundColor = LogConfig.BACKGROUND_EXCEPTION_COLOR;
                    }
                    logType = "FATAL";
                    break;
            }
            var logStr = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} [{logType}] {(logType.Length == 4 ? " " : "")}{log.data} {(log.stack == null ? "" : "@ " + log.stack)}";
            if (isWriteConsole && log.type >= LogConfig.CONSOLE_OUTPUT_LOG_TYPE)
            {
                Console.WriteLine(logStr);
                Console.ResetColor();
            }
            return logStr;
        }

        /// <summary>
        /// 停止更新
        /// </summary>
        public void UpdateEnd()
        {
        }

        /// <summary>
        /// 日志信息数据
        /// </summary>
        internal class LogInfo
        {
            /// <summary>
            /// 日志类型
            /// </summary>
            public LogType type;
            /// <summary>
            /// 日志时间
            /// </summary>
            public DateTime time = DateTime.Now;
            /// <summary>
            /// 日志内容
            /// </summary>
            public string data = "";
            /// <summary>
            /// 堆栈信息
            /// </summary>
            public string stack = "";
        }
    }
}
