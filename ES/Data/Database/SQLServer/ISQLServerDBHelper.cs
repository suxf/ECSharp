using System;
using System.Data.SqlClient;

namespace ES.Data.Database.SQLServer
{
    /// <summary>
    /// SQLServer数据库助手异常捕获
    /// </summary>
    public interface ISQLServerDBHelper
    {
        /// <summary>
        /// 检测连接状态异常
        /// </summary>
        /// <param name="helper">数据库助手</param>
        /// <param name="exception">异常</param>
        void CheckConnectedException(SQLServerDBHelper helper, Exception exception);

        /// <summary>
        /// 执行SQL异常
        /// </summary>
        /// <param name="helper">数据库助手</param>
        /// <param name="sql">sql语句</param>
        /// <param name="exception">异常</param>
        void CommandSQLException(SQLServerDBHelper helper, string sql, Exception exception);

        /// <summary>
        /// 存储过程异常
        /// </summary>
        /// <param name="helper">数据库助手</param>
        /// <param name="procedure">存储过程</param>
        /// <param name="sqlParameters">参数</param>
        /// <param name="exception">异常</param>
        void ProcedureException(SQLServerDBHelper helper, string procedure, SqlParameter[] sqlParameters, Exception exception);
    }
}
