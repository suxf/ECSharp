namespace ES
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
        /// 输入性 日志
        /// <para>Reads the next line of characters from the standard input stream.</para>
        /// </summary>
        /// <param name="log">日志数据</param>
        /// <returns></returns>
        public static string ReadLine(string log = "")
        {
            System.Console.ForegroundColor = System.ConsoleColor.White;
            System.Console.BackgroundColor = System.ConsoleColor.Black;
            if (log != "") System.Console.Write(log);
            string input = System.Console.ReadLine() ?? "";
            System.Console.ResetColor();
            LogManager.WriteLine(LogType.INPUT, log + input);
            return input;
        }

        /// <summary>
        /// 调试性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        public static void Debug(string log)
        {
            LogManager.WriteLine(LogType.DEBUG, log);
        }

        /// <summary>
        /// 调试性 日志
        /// </summary>
        /// <param name="logs">日志数据</param>
        public static void Debug(params string[] logs)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0, len = logs.Length; i < len; i++)
            {
                sb.Append(logs[i]);
            }
            LogManager.WriteLine(LogType.DEBUG, sb.ToString());
        }

        /// <summary>
        /// 信息性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        public static void Info(string log)
        {
            LogManager.WriteLine(LogType.INFO, log);
        }

        /// <summary>
        /// 信息性 日志
        /// </summary>
        /// <param name="logs">日志数据</param>
        public static void Info(params string[] logs)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0, len = logs.Length; i < len; i++)
            {
                sb.Append(logs[i]);
            }
            LogManager.WriteLine(LogType.INFO, sb.ToString());
        }

        /// <summary>
        /// 警告性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        public static void Warn(string log)
        {
            LogManager.WriteLine(LogType.WARN, log);
        }

        /// <summary>
        /// 警告性 日志
        /// </summary>
        /// <param name="logs">日志数据</param>
        public static void Warn(params string[] logs)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0, len = logs.Length; i < len; i++)
            {
                sb.Append(logs[i]);
            }
            LogManager.WriteLine(LogType.WARN, sb.ToString());
        }

        /// <summary>
        /// 错误性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        public static void Error(string log)
        {
            LogManager.WriteLine(LogType.ERROR, log);
        }

        /// <summary>
        /// 错误性 日志
        /// </summary>
        /// <param name="logs">日志数据</param>
        public static void Error(params string[] logs)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0, len = logs.Length; i < len; i++)
            {
                sb.Append(logs[i]);
            }
            LogManager.WriteLine(LogType.ERROR, sb.ToString());
        }

        /// <summary>
        /// 异常 日志
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="log">日志内容</param>
        public static void Exception(System.Exception ex, string log = "")
        {
            string data = string.Format("{0}\r\n   Message:{1}\r\n   Method:{2}\r\n   StackTrace:\r\n{3}", log, ex.Message, ex.TargetSite, ex.StackTrace);
            LogManager.WriteLine(LogType.FATAL, data);
        }
    }
}