using System;
using System.Data;

namespace ES.Data.Database.SQLServer
{
    /// <summary>
    /// SQL语句执行对象结果
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// 影响行数
        /// </summary>
        public int effectNum { internal set; get; }
        /// <summary>
        /// 执行数据合集
        /// </summary>
        public DataRowCollection collection { internal set; get; }
        /// <summary>
        /// 数据总合集
        /// </summary>
        public DataSet dataSet { internal set; get; }

        /// <summary>
        /// 是否有异常
        /// </summary>
        public bool hasException { internal set; get; }
        /// <summary>
        /// 异常内容
        /// </summary>
        public Exception exception { internal set; get; }
    }
}
