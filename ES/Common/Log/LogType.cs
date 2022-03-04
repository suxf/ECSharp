namespace ES.Common.Log
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 调试性日志
        /// </summary>
        DEBUG = 0,
        /// <summary>
        /// 信息性日志
        /// </summary>
        INFO = 1,
        /// <summary>
        /// 警告性日志
        /// </summary>
        WARN = 2,
        /// <summary>
        /// 错误性日志
        /// </summary>
        ERROR = 3,
        /// <summary>
        /// 致命性日志
        /// </summary>
        FATAL = 4,
    }
}
