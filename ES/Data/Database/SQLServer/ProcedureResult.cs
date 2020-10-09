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
        /// </summary>
        public bool isCompleted { internal set; get; }
    }
}
