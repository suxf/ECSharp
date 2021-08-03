using System;

namespace ES.Common.Log
{
    /// <summary>
    /// 日志信息数据
    /// </summary>
    internal class LogInfo
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        public string type;
        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime time = DateTime.Now;
        /// <summary>
        /// 日志内容
        /// </summary>
        public string data;
    }
}
