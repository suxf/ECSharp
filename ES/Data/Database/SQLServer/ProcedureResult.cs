using System;
using System.Data;
using System.Data.SqlClient;

namespace ES.Data.Database.SQLServer
{
    /// <summary>
    /// 存储过程结果集
    /// </summary>
    public class ProcedureResult
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        public object returnValue { internal set; get; }

        /// <summary>
        /// 存储过程
        /// </summary>
        public string procedure { internal set; get; }

        /// <summary>
        /// 执行数据输出参数
        /// </summary>
        public SqlParameterCollection SqlParameters { internal set; get; }

        /// <summary>
        /// 执行数据合集
        /// </summary>
        public DataTableCollection Tables { internal set; get; }

        /// <summary>
        /// 是否已完成
        /// 如果未完成很可能有异常 请通过 exception 对象获取
        /// </summary>
        public bool isCompleted { internal set; get; }
        /// <summary>
        /// 异常内容
        /// </summary>
        public Exception exception { internal set; get; }
    }
}
