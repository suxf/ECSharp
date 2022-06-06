using ES.Time;
using ES.Utils;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace ES
{
    /// <summary>
    /// 日志管理器
    /// <para>周期性写入文件</para>
    /// <para>周期LOG_PERIOD、写入路径LOG_PATH和分文件大小限制LOG_UNIT_FILE_MAX_SIZE可以直接调用静态修改（程序启动时未第一次调用就应修改完成）</para>
    /// </summary>
    internal static class LogManager
    {
        /// <summary>
        /// 文件信息
        /// </summary>
        private static FileInfo? fileInfo = null;
        /// <summary>
        /// 日志ID
        /// </summary>
        private static readonly string logId;
        /// <summary>
        /// 日志索引，如果单个时间内日志太大则分开
        /// </summary>
        private static int logIndex = 0;
        /// <summary>
        /// 进程名称
        /// </summary>
        private static readonly string proccessName = "";
        /// <summary>
        /// 时间流
        /// </summary>
        private static readonly BaseTimeFlow timeFlow;
        /// <summary>
        /// 日志写入线程
        /// </summary>
        private static readonly LogWriteUpdate logWriteUpdate = new LogWriteUpdate();
        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object m_lock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        static LogManager()
        {
            proccessName = Process.GetCurrentProcess().ProcessName.ToLower();
            logId = Randomizer.Random.Next(100, 999).ToString();
            timeFlow = BaseTimeFlow.CreateTimeFlow(logWriteUpdate);
            timeFlow.StartTimeFlowES();
#if !UNITY_2020_1_OR_NEWER
            SystemInfo();
#endif
        }

        /// <summary>
        /// 打印系统环境信息日志
        /// </summary>
#if UNITY_2020_1_OR_NEWER
    [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        private static void SystemInfo()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
#if !UNITY_2020_1_OR_NEWER
            sb.Append("name: ");
            sb.Append(Utils.SystemInfo.ProcessName);
            sb.Append("; version: ");
            sb.Append(Utils.SystemInfo.ProcessVersion);
            sb.Append("; es: ");
            sb.Append(Utils.SystemInfo.FrameVersion);
            sb.Append("; ");
#endif
            sb.Append("dotnet: ");
            sb.Append(Utils.SystemInfo.DotNetVersion);
            sb.Append("; path: ");
            sb.Append(Utils.SystemInfo.Path);
            sb.Append("; system: ");
            sb.Append(Utils.SystemInfo.SystemVersion);
            sb.Append("; user: ");
            sb.Append(Utils.SystemInfo.UserName);
            sb.Append("; logicthreads: ");
            sb.Append(Utils.SystemInfo.ProcessorCount);

            bool LOG_CONSOLE_STACK_TRACE_OUTPUT = LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT;
            bool LOG_FILE_STACK_TRACE_OUTPUT = LogConfig.LOG_FILE_STACK_TRACE_OUTPUT;
            LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT = false;
            LogConfig.LOG_FILE_STACK_TRACE_OUTPUT = false;
            Log.Info(sb);
            LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT = LOG_CONSOLE_STACK_TRACE_OUTPUT;
            LogConfig.LOG_FILE_STACK_TRACE_OUTPUT = LOG_FILE_STACK_TRACE_OUTPUT;
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="log">日志数据</param>
        internal static void WriteLine(LogType type, string log)
        {
            LogInfo logInfo = new LogInfo();
            logInfo.time = DateTime.Now;
            logInfo.type = type;
            logInfo.data = log;

            if (LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT || LogConfig.LOG_FILE_STACK_TRACE_OUTPUT)
            {
                var frame = new StackTrace().GetFrame(2);
                var method = frame?.GetMethod();

                if (method != null && method.DeclaringType != null)
                    logInfo.stack = $"{method.DeclaringType.FullName}:{method.Name}";
            }

            if (!LogConfig.LOG_CONSOLE_ASYNC_OUTPUT)
            {
                FormatLog(ref logInfo);
                lock (m_lock) OutputLog(ref logInfo);
            }

            // 压入队列
            logWriteUpdate.Enqueue(logInfo);
        }

        /// <summary>
        /// 格式化日志
        /// </summary>
        /// <param name="log"></param>
        private static void FormatLog(ref LogInfo log)
        {
            string logType = "";
            switch (log.type)
            {
                case LogType.DEBUG:
                    logType = "DEBUG";
                    break;
                case LogType.INFO:
                    logType = "INFO";
                    break;
                case LogType.WARN:
                    logType = "WARN";
                    break;
                case LogType.ERROR:
                    logType = "ERROR";
                    break;
                case LogType.FATAL:
                    logType = "FATAL";
                    break;
                case LogType.INPUT:
                    logType = "INPUT";
                    break;
            }
            log.log = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} [{logType}] {(logType.Length == 4 ? " " : "")}{log.data}";
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="log"></param>
        private static void OutputLog(ref LogInfo log)
        {
            if (log.type < LogConfig.CONSOLE_OUTPUT_LOG_TYPE)
                return;

            switch (log.type)
            {
                case LogType.DEBUG:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_DEBUG_COLOR;
                    if (LogConfig.BACKGROUND_DEBUG_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_DEBUG_COLOR;
#else
					UnityEngine.Debug.Log($"{log.log}{(LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT && !string.IsNullOrEmpty(log.stack) ? $" <{log.stack}>" : " ")}");
#endif
                    break;
                case LogType.INFO:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_INFO_COLOR;
                    if (LogConfig.BACKGROUND_INFO_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_INFO_COLOR;
#else
					UnityEngine.Debug.Log($"{log.log}{(LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT && !string.IsNullOrEmpty(log.stack) ? $" <{log.stack}>" : " ")}");
#endif
                    break;
                case LogType.WARN:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_WARN_COLOR;
                    if (LogConfig.BACKGROUND_WARN_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_WARN_COLOR;
#else
					UnityEngine.Debug.LogWarning($"{log.log}{(LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT && !string.IsNullOrEmpty(log.stack) ? $" <{log.stack}>" : " ")}");
#endif
                    break;
                case LogType.ERROR:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_ERROR_COLOR;
                    if (LogConfig.BACKGROUND_ERROR_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_ERROR_COLOR;
#else
					UnityEngine.Debug.LogError($"{log.log}{(LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT && !string.IsNullOrEmpty(log.stack) ? $" <{log.stack}>" : " ")}");
#endif
                    break;
                case LogType.FATAL:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_EXCEPTION_COLOR;
                    if (LogConfig.BACKGROUND_EXCEPTION_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_EXCEPTION_COLOR;
#else
					UnityEngine.Debug.LogAssertion($"{log.log}{(LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT && !string.IsNullOrEmpty(log.stack) ? $" <{log.stack}>" : " ")}");
#endif
                    break;
                case LogType.INPUT:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_INPUT_COLOR;
                    if (LogConfig.BACKGROUND_INPUT_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_INPUT_COLOR;
#else
					UnityEngine.Debug.Log($"{log.log}{(LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT && !string.IsNullOrEmpty(log.stack) ? $" <{log.stack}>" : " ")}");
#endif
                    break;
            }
#if !UNITY_2020_1_OR_NEWER
            Console.WriteLine($"{log.log}{(LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT && !string.IsNullOrEmpty(log.stack) ? $" <{log.stack}>" : " ")}");
            Console.ResetColor();
#endif
        }

        /// <summary>
        /// 日志写入线程
        /// </summary>
        private class LogWriteUpdate : ITimeUpdate
        {
            /// <summary>
            /// 日志数据队列
            /// </summary>
            private readonly ConcurrentQueue<LogInfo> logInfos = new ConcurrentQueue<LogInfo>();
            /// <summary>
            /// 周期
            /// </summary>
            private int periodNow = 0;

            /// <summary>
            /// 压入日志队列
            /// </summary>
            /// <param name="log"></param>
            public void Enqueue(LogInfo log)
            {
                logInfos.Enqueue(log);
            }

            /// <summary>
            /// 系统调用
            /// </summary>
            /// <param name="dt"></param>
            public void Update(int dt)
            {
                periodNow += dt;
                if (periodNow >= LogConfig.LOG_PERIOD)
                {
                    periodNow -= LogConfig.LOG_PERIOD;

                    // 如果没有日志则不处理
                    if (logInfos.IsEmpty)
                        return;

                    // 创建目录
                    if (!Directory.Exists(LogConfig.LOG_PATH))
                    {
                        Directory.CreateDirectory(LogConfig.LOG_PATH);
                    }

                    string dateStr = DateTime.Now.ToString("yyyy_MM_dd/");
                    // 创建当日目录
                    if (!Directory.Exists(LogConfig.LOG_PATH + dateStr))
                    {
                        Directory.CreateDirectory(LogConfig.LOG_PATH + dateStr);
                    }

                    string filename = string.Format(DateTime.Now.ToString("{4}yyyy_MM_dd/{2}_HH_{0}_{1}{3}"), logIndex, logId, proccessName, LogConfig.LOG_FILE_SUFFIX, LogConfig.LOG_PATH);

                    if (!File.Exists(filename))
                        fileInfo = null;

                    // 检查文件
                    if (fileInfo == null)
                        fileInfo = new FileInfo(filename);
                    else
                        fileInfo.Refresh();

                    if (fileInfo.Exists)
                    {
                        if (fileInfo.Length > LogConfig.LOG_UNIT_FILE_MAX_SIZE)
                        {
                            fileInfo = new FileInfo(string.Format(DateTime.Now.ToString("{4}yyyy_MM_dd/{2}_HH_{0}_{1}{3}"), ++logIndex, logId, proccessName, LogConfig.LOG_FILE_SUFFIX, LogConfig.LOG_PATH));
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

                    using (StreamWriter sw = fileInfo.AppendText())
                    {
                        // 写入日志
                        while (logInfos.TryDequeue(out LogInfo log))
                        {
                            if (LogConfig.LOG_CONSOLE_ASYNC_OUTPUT)
                            {
                                FormatLog(ref log);
                                OutputLog(ref log);
                            }

                            if (log.type >= LogConfig.FILE_OUTPUT_LOG_TYPE)
                            {
                                sw.WriteLine($"{log.log}{(LogConfig.LOG_FILE_STACK_TRACE_OUTPUT && !string.IsNullOrEmpty(log.stack) ? $" <{log.stack}>" : " ")}");
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// 停止更新
            /// </summary>
            public void UpdateEnd()
            {
            }
        }

        /// <summary>
        /// 日志信息数据
        /// </summary>
        private struct LogInfo
        {
            /// <summary>
            /// 日志类型
            /// </summary>
            internal LogType type;
            /// <summary>
            /// 日志时间
            /// </summary>
            internal DateTime time;
            /// <summary>
            /// 日志内容
            /// </summary>
            internal string data;
            /// <summary>
            /// 堆栈信息
            /// </summary>
            internal string stack;
            /// <summary>
            /// 日志字符串
            /// </summary>
            internal string log;
        }
    }
}
