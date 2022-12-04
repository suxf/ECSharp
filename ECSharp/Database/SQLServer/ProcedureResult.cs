#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System.Data;
using System.Data.SqlClient;

namespace ECSharp.Database.SQLServer
{
    /// <summary>
    /// 存储过程结果集
    /// </summary>
    public class ProcedureResult
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        public object? ReturnValue { internal set; get; }

        /// <summary>
        /// 存储过程
        /// </summary>
        public string? Procedure { internal set; get; }

        /// <summary>
        /// 执行数据输出参数
        /// </summary>
        public SqlParameterCollection? SqlParameters { internal set; get; }

        /// <summary>
        /// 执行数据合集
        /// </summary>
        public DataTableCollection? Tables { internal set; get; }

        /// <summary>
        /// 首个表记录
        /// <para>如果过程返回存在一个或多个表结果，此处为第一个表的结果，否则为空</para>
        /// </summary>
        public DataRowCollection? FirstRows { internal set; get; }

        /// <summary>
        /// 是否已完成
        /// <para>为True代表存储过程完整执行成功，False表示有异常</para>
        /// </summary>
        public bool IsCompleted { internal set; get; }
    }
}
