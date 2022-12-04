#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;
using System.Data.SqlClient;

namespace ECSharp.Database.SQLServer
{
    /// <summary>
    /// SQLServer数据库助手异常捕获
    /// </summary>
    public interface ISqlServerDbHelper
    {
        /// <summary>
        /// 检测连接状态异常
        /// </summary>
        /// <param name="helper">数据库助手</param>
        /// <param name="exception">异常</param>
        void CheckConnectedException(SqlServerDbHelper helper, Exception exception);

        /// <summary>
        /// 执行SQL异常
        /// </summary>
        /// <param name="helper">数据库助手</param>
        /// <param name="sql">sql语句</param>
        /// <param name="exception">异常</param>
        void CommandSQLException(SqlServerDbHelper helper, string sql, Exception exception);

        /// <summary>
        /// 存储过程异常
        /// </summary>
        /// <param name="helper">数据库助手</param>
        /// <param name="procedure">存储过程</param>
        /// <param name="sqlParameters">参数</param>
        /// <param name="exception">异常</param>
        void ProcedureException(SqlServerDbHelper helper, string procedure, SqlParameter[] sqlParameters, Exception exception);
    }
}
