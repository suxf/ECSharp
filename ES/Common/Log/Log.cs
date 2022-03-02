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
        /// 调试性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        public static void Debug(string log)
        {
            WriteLine("DEBUG", log);
        }

        /// <summary>
        /// 信息性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        public static void Info(string log)
        {
            WriteLine("INFO", log);
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
            LogManager.LogInfo logInfo = new LogManager.LogInfo();
            logInfo.type = type;
            logInfo.data = log;
            if (LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT)
            {
                var frame = new System.Diagnostics.StackTrace().GetFrame(2);
                if (frame != null)
                {
                    var method = frame.GetMethod();
                    logInfo.stack = method!.DeclaringType!.FullName + ":" + method!.Name;
                }
            }
            LogManager.OutputLog(logInfo, !LogConfig.LOG_CONSOLE_ASYNC_OUTPUT);
            // 压入队列
            LogManager.Instance.logInfos.Enqueue(logInfo);
        }
    }
}
