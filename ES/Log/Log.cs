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
        WriteLine(ES.Log.LogType.DEBUG, log);
    }

    /// <summary>
    /// 信息性 日志
    /// </summary>
    /// <param name="log">日志数据</param>
    public static void Info(string log)
    {
        WriteLine(ES.Log.LogType.INFO, log);
    }

    /// <summary>
    /// 警告性 日志
    /// </summary>
    /// <param name="log">日志数据</param>
    public static void Warn(string log)
    {
        WriteLine(ES.Log.LogType.WARN, log);
    }

    /// <summary>
    /// 错误性 日志
    /// </summary>
    /// <param name="log">日志数据</param>
    public static void Error(string log)
    {
        WriteLine(ES.Log.LogType.ERROR, log);
    }

    /// <summary>
    /// 异常 日志
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <param name="log">日志内容</param>
    public static void Exception(System.Exception ex, string log = "")
    {
        string data = string.Format("{0}\r\n   Message:{1}\r\n   Method:{2}\r\n   StackTrace:\r\n{3}", log, ex.Message, ex.TargetSite, ex.StackTrace);
        WriteLine(ES.Log.LogType.FATAL, data);
    }

    /// <summary>
    /// 写入日志
    /// </summary>
    /// <param name="type">日志类型</param>
    /// <param name="log">日志数据</param>
    private static void WriteLine(ES.Log.LogType type, string log)
    {
        ES.Log.LogManager.LogInfo logInfo = new ES.Log.LogManager.LogInfo();
        logInfo.type = type;
        logInfo.data = log;
        if (ES.Log.LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT || ES.Log.LogConfig.LOG_FILE_STACK_TRACE_OUTPUT)
        {
            var frame = new System.Diagnostics.StackTrace().GetFrame(2);
            if (frame != null)
            {
                var method = frame.GetMethod();
                logInfo.stack = (method?.DeclaringType?.FullName ?? "UnknowClassType") + ":" + (method?.Name ?? "UnknowMethod");
            }
        }
        if (!ES.Log.LogConfig.LOG_CONSOLE_ASYNC_OUTPUT)
        {
            ES.Log.LogManager.FormatLog(ref logInfo);
            ES.Log.LogManager.OutputLog(ref logInfo);
        }
        // 压入队列
        ES.Log.LogManager.Instance.logInfos.Enqueue(logInfo);
    }
}
