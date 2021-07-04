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
        /// 日志捕捉空间命名
        /// </summary>
        public string spaceName;
        /// <summary>
        /// 日志发生对象类名
        /// </summary>
        public string className;
        /// <summary>
        /// 日志发生函数
        /// </summary>
        public string methodName;
        /// <summary>
        /// 日志内容
        /// </summary>
        public string data;
    }
}
