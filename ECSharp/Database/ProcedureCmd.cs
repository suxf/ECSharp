#if !UNITY_2020_1_OR_NEWER
using System.Data.Common;

namespace ECSharp.Database
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
        public DbParameter[] parameters;

        internal ProcedureCmd(string procedure, DbParameter[] parameters)
        {
            this.procedure = procedure;
            this.parameters = parameters;
        }
    }
}

#endif