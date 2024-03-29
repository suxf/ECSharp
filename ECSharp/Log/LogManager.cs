﻿#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using ECSharp.Time;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace ECSharp
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
        /// 日期时间字符串
        /// </summary>
        private static string dateTimeStr = "";
        /// <summary>
        /// 时间字符串
        /// </summary>
        private static string timeStr = "";

        /// <summary>
        /// 进程名称
        /// </summary>
        private static readonly string proccessName = "program";

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
            // 创建目录
            if (!Directory.Exists(LogConfig.LOG_PATH))
            {
                Directory.CreateDirectory(LogConfig.LOG_PATH);
            }
#if !UNITY_2020_1_OR_NEWER
            proccessName = Process.GetCurrentProcess().ProcessName.ToLower();
#endif
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
#if UNITY_2020_1_OR_NEWER
            ECSharpScript.InitECSharp();
#else
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("process:");
            sb.Append(Utils.SystemInfo.ProcessName);
            sb.Append("; version:");
            sb.Append(Utils.SystemInfo.ProcessVersion);
            sb.Append("; esf:");
            sb.Append(Utils.SystemInfo.FrameVersion);
            sb.Append("; ");
            sb.Append(".net:");
            sb.Append(Utils.SystemInfo.DotNetVersion);
            sb.Append("; path:");
            sb.Append(Utils.SystemInfo.Path);
            sb.Append("; sys:");
            sb.Append(Utils.SystemInfo.SystemVersion);
            sb.Append("; user:");
            sb.Append(Utils.SystemInfo.UserName);
            sb.Append("; cores:");
            sb.Append(Utils.SystemInfo.ProcessorCount);

            bool LOG_CONSOLE_STACK_TRACE_OUTPUT = LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT;
            bool LOG_FILE_STACK_TRACE_OUTPUT = LogConfig.LOG_FILE_STACK_TRACE_OUTPUT;
            LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT = false;
            LogConfig.LOG_FILE_STACK_TRACE_OUTPUT = false;
            Log.Info(sb);
            LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT = LOG_CONSOLE_STACK_TRACE_OUTPUT;
            LogConfig.LOG_FILE_STACK_TRACE_OUTPUT = LOG_FILE_STACK_TRACE_OUTPUT;
#endif
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="log">日志数据</param>
        internal static void WriteLine(LogType type, string log)
        {
            LogInfo logInfo = new LogInfo();
            logInfo.time = LocalTime.Now;
            logInfo.type = type;
            logInfo.data = log;

            if (LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT || LogConfig.LOG_FILE_STACK_TRACE_OUTPUT)
            {
#if !UNITY_2020_1_OR_NEWER
                var frame = new StackTrace().GetFrame(2);
#else
                var frame = new StackTrace().GetFrame(4);
#endif
                var method = frame?.GetMethod();

                if (method != null && method.DeclaringType != null)
                    logInfo.stack = $"{method.DeclaringType.FullName}:{method.Name}";
            }

            if (!LogConfig.LOG_CONSOLE_ASYNC_OUTPUT)
            {
                lock (m_lock) OutputLog(ref logInfo);
            }

            // 压入队列
            logWriteUpdate.Enqueue(logInfo);
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="log"></param>
        private static void OutputLog(ref LogInfo log)
        {
            if (log.type < LogConfig.CONSOLE_OUTPUT_LOG_TYPE)
                return;

            string logStack = $"{(LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT && !string.IsNullOrEmpty(log.stack) ? $" <{log.stack}>" : " ")}";
            switch (log.type)
            {
                case LogType.DEBUG:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_DEBUG_COLOR;
                    if (LogConfig.BACKGROUND_DEBUG_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_DEBUG_COLOR;
#else
#if UNITY_EDITOR
                    string l1 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <<color='#08F629'>DEBUG</color>> {log.data}{logStack}";
#else
                    string l1 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <DEBUG> {log.data}{logStack}";
#endif
                    UnityEngine.Debug.Log(l1);
                    LogConfig.OnLog?.Invoke(log.type, l1);
#endif
                    break;
                case LogType.INFO:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_INFO_COLOR;
                    if (LogConfig.BACKGROUND_INFO_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_INFO_COLOR;
#else
#if UNITY_EDITOR
                    string l2 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <<color='#808080'>INFO</color>>  {log.data}{logStack}";
#else
                    string l2 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <INFO>  {log.data}{logStack}";
#endif
                    UnityEngine.Debug.Log(l2);
                    LogConfig.OnLog?.Invoke(log.type, l2);
#endif
                    break;
                case LogType.WARN:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_WARN_COLOR;
                    if (LogConfig.BACKGROUND_WARN_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_WARN_COLOR;
#else
#if UNITY_EDITOR
                    string l3 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <<color='#FFEE28'>WARN</color>>  {log.data}{logStack}";
#else
                    string l3 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <WARN>  {log.data}{logStack}";
#endif
                    UnityEngine.Debug.LogWarning(l3);
                    LogConfig.OnLog?.Invoke(log.type, l3);
#endif
                    break;
                case LogType.ERROR:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_ERROR_COLOR;
                    if (LogConfig.BACKGROUND_ERROR_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_ERROR_COLOR;
#else
#if UNITY_EDITOR
                    string l4 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <<color='#FF2D2D'>ERROR</color>> {log.data}{logStack}";
#else
                    string l4 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <ERROR> {log.data}{logStack}";
#endif
                    UnityEngine.Debug.LogError(l4);
                    LogConfig.OnLog?.Invoke(log.type, l4);
#endif
                    break;
                case LogType.FATAL:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_EXCEPTION_COLOR;
                    if (LogConfig.BACKGROUND_EXCEPTION_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_EXCEPTION_COLOR;
#else
#if UNITY_EDITOR
                    string l5 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <<color='#C947FF'>FATAL</color>> {log.data}{logStack}";
#else
                    string l5 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <FATAL> {log.data}{logStack}";
#endif
                    UnityEngine.Debug.LogError(l5);
                    LogConfig.OnLog?.Invoke(log.type, l5);
#endif
                    break;
                case LogType.INPUT:
#if !UNITY_2020_1_OR_NEWER
                    Console.ForegroundColor = LogConfig.FOREGROUND_INPUT_COLOR;
                    if (LogConfig.BACKGROUND_INPUT_COLOR != null) Console.BackgroundColor = (ConsoleColor)LogConfig.BACKGROUND_INPUT_COLOR;
#else
#if UNITY_EDITOR
                    string l6 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <<color='#2B45FF'>INPUT</color>> {log.data}{logStack}";
#else
                    string l6 = $"{log.time:yyyy/MM/dd HH:mm:ss.fff} <INPUT> {log.data}{logStack}";
#endif
                    UnityEngine.Debug.Log(l6);
                    LogConfig.OnLog?.Invoke(log.type, l6);
#endif
                    break;
            }
#if !UNITY_2020_1_OR_NEWER
            string logType = log.type.ToString();
            Console.WriteLine($"{log.time:yyyy/MM/dd HH:mm:ss.fff} <{logType}> {(logType.Length == 4 ? " " : "")}{log.data}{logStack}");
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

                    if(LocalTime.Now.ToString("yyyy_MM_dd") != dateTimeStr)
                    {
                        dateTimeStr = LocalTime.Now.ToString("yyyy_MM_dd");
                        timeStr = LocalTime.Now.ToString("HH_mm_ss_fff");
                    }

                    // string dateStr = datetime.ToString("yyyy_MM_dd/");
                    // // 创建当日目录
                    // if (!Directory.Exists(LogConfig.LOG_PATH + dateStr))
                    // {
                    //     Directory.CreateDirectory(LogConfig.LOG_PATH + dateStr);
                    // }

                    string filename = $"{LogConfig.LOG_PATH}{proccessName}_{dateTimeStr}_{timeStr}{LogConfig.LOG_FILE_SUFFIX}";

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
                            timeStr = LocalTime.Now.ToString("HH_mm_ss_fff");
                            fileInfo = new FileInfo($"{LogConfig.LOG_PATH}{proccessName}_{dateTimeStr}_{timeStr}{LogConfig.LOG_FILE_SUFFIX}");
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
                        int runCount = TimeFlowThread.UtilMsMaxHandleCount;
                        // 写入日志
                        while (runCount > 0 && logInfos.TryDequeue(out LogInfo log))
                        {
                            --runCount;
                            if (LogConfig.LOG_CONSOLE_ASYNC_OUTPUT)
                            {
                                OutputLog(ref log);
                            }

                            if (log.type >= LogConfig.FILE_OUTPUT_LOG_TYPE)
                            {
                                string logType = log.type.ToString();
                                sw.WriteLine($"{log.time:yyyy/MM/dd HH:mm:ss.fff} <{logType}> {(logType.Length == 4 ? " " : "")}{log.data}{(LogConfig.LOG_FILE_STACK_TRACE_OUTPUT && !string.IsNullOrEmpty(log.stack) ? $" <{log.stack}>" : " ")}");
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
        }
    }
}
