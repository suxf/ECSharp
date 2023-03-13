#if !UNITY_2020_1_OR_NEWER
using System;

namespace ECSharp.Database.MySQL
{
    /// <summary>
    /// MySql数据库助手异常捕获
    /// </summary>
    public interface IMySqlDbHelper
    {
        /// <summary>
        /// 检测连接状态异常
        /// </summary>
        /// <param name="helper">数据库助手</param>
        /// <param name="exception">异常</param>
        void CheckConnectedException(MySqlDbHelper helper, Exception exception);

        /// <summary>
        /// 执行SQL异常
        /// </summary>
        /// <param name="helper">数据库助手</param>
        /// <param name="sql">sql语句</param>
        /// <param name="exception">异常</param>
        void CommandSQLException(MySqlDbHelper helper, string sql, Exception exception);
    }
}

#endif