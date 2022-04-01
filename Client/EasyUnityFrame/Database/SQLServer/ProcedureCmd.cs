#if !UNITY_2020_1_OR_NEWER
using System.Data.SqlClient;

namespace ES.Database.SQLServer
{
    /// <summary>
    /// 存储过程指令
    /// </summary>
    internal class ProcedureCmd
    {
        /// <summary>
        /// 存储过程名
        /// </summary>
        public string procedure;
        /// <summary>
        /// 参数
        /// </summary>
        public SqlParameter[] sqlParameters;

        internal ProcedureCmd(string procedure, SqlParameter[] sqlParameters)
        {
            this.procedure = procedure;
            this.sqlParameters = sqlParameters;
        }
    }
}
#endif