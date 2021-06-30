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
        /// 信息性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        /// <param name="className">类型</param>
        /// <param name="methodName">方法</param>
        /// <param name="spaceName">空间命名</param>
        public static void Info(string log, string className = null, string methodName = null, string spaceName = null)
        {
            WriteLine("Info", log, className, methodName, spaceName);
        }

        /// <summary>
        /// 调试性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        /// <param name="className">类型</param>
        /// <param name="methodName">方法</param>
        /// <param name="spaceName">空间命名</param>
        public static void Debug(string log, string className = null, string methodName = null, string spaceName = null)
        {
            WriteLine("Debug", log, className, methodName, spaceName);
        }

        /// <summary>
        /// 警告性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        /// <param name="className">类型</param>
        /// <param name="methodName">方法</param>
        /// <param name="spaceName">空间命名</param>
        public static void Warn(string log, string className = null, string methodName = null, string spaceName = null)
        {
            WriteLine("Warn", log, className, methodName, spaceName);
        }

        /// <summary>
        /// 错误性 日志
        /// </summary>
        /// <param name="log">日志数据</param>
        /// <param name="className">类型</param>
        /// <param name="methodName">方法</param>
        /// <param name="spaceName">空间命名</param>
        public static void Error(string log, string className = null, string methodName = null, string spaceName = null)
        {
            WriteLine("Error", log, className, methodName, spaceName);
        }

        /// <summary>
        /// 异常 日志
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="log">日志内容</param>
        /// <param name="className">类型</param>
        /// <param name="methodName">方法</param>
        /// <param name="spaceName">空间命名</param>
        public static void Exception(System.Exception ex, string log = "", string className = null, string methodName = null, string spaceName = null)
        {
            string data = string.Format("Message:{0}\r\n   ExceptionData:{1}\r\n   Method:{2}\r\n   StackTrace:\r\n{3}", ex.Message, log, ex.TargetSite, ex.StackTrace);
            WriteLine("Exception", data, className, methodName, spaceName);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="log">日志数据</param>
        /// <param name="className">类型</param>
        /// <param name="methodName">方法</param>
        /// <param name="spaceName">空间命名</param>
        private static void WriteLine(string type, string log, string className, string methodName, string spaceName)
        {
            LogInfo logInfo = new LogInfo();
            logInfo.type = type;
            logInfo.data = log;
            logInfo.className = className;
            logInfo.methodName = methodName;
            logInfo.spaceName = spaceName;
            // 压入队列
            LogManager.Instance.logInfos.Enqueue(logInfo);
        }
    }
}
