#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using ECSharp.Time;
using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace ECSharp.Database
{
    /// <summary>
    /// 数据助手接口
    /// </summary>
    public interface IDbHelper : ISysTime
    {
        /// <summary>
        /// 检查是否连接
        /// </summary>
        /// <returns>成功连接返回true</returns>
        bool CheckConnected();

        /// <summary>
        /// 获取数据库连接地址
        /// </summary>
        /// <returns></returns>
        string GetConnectionString();

        /// <summary>
        /// 执行查询SQL语句
        /// <para>SELECT适用和部分需要更新返回的SQL</para>
        /// </summary>
        /// <param name="sql">执行SQL</param>
        /// <param name="obj">格式化参数输入【类似string.Format】</param>
        /// <returns>返回成功与否</returns>
        CommandResult CommandSQL(string sql, params object[] obj);

        /// <summary>
        /// 执行修改SQL语句
        /// <para>非SELECT适用，只需要影响行数</para>
        /// </summary>
        /// <param name="sql">执行SQL</param>
        /// <param name="obj">格式化参数输入【类似string.Format】</param>
        /// <returns>返回影响条数</returns>
        int ExecuteSQL(string sql, params object[] obj);

        /// <summary>
        /// 压入SQL队列，等待统一顺序执行【异步】
        /// <para>此操作适合非查询操作SQL,且对数据实时更新无要求的情况下方可使用</para>
        /// <para>脱离主线程由其他线程处理数据</para>
        /// </summary>
        /// <param name="sql">执行SQL</param>
        /// <param name="obj">格式化参数输入【类似string.Format】</param>
        void PushSQL(string sql, params object[] obj);

        /// <summary>
        /// 存储过程 1
        /// <para>返回值默认为整型，长度为4</para>
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>返回 ProcedureResult 失败为null</returns>
        public ProcedureResult Procedure(string procedure, params DbParameter[] sqlParameters);

        /// <summary>
        /// 存储过程 2
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="retvalueDbType">返回值类型</param>
        /// <param name="retvalueSize">返回值大小</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>返回 ProcedureResult 失败为null</returns>
        public ProcedureResult Procedure(string procedure, SqlDbType retvalueDbType, int retvalueSize, params DbParameter[] sqlParameters);

        /// <summary>
        /// 存储过程 2
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="retvalueDbType">返回值类型</param>
        /// <param name="retvalueSize">返回值大小</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>返回 ProcedureResult 失败为null</returns>
        public ProcedureResult Procedure(string procedure, MySqlDbType retvalueDbType, int retvalueSize, params DbParameter[] sqlParameters);

        /// <summary>
        /// 存储过程 3
        /// <para>不需要返回任何数据</para>
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>影响数量 -1表示异常</returns>
        public int ProcedureNonQuery(string procedure, params DbParameter[] sqlParameters);

        /// <summary>
        /// 压入SQL队列，等待统一顺序执行【异步】
        /// <para>此操作适合非查询操作SQL,且对数据实时更新无要求的情况下方可使用</para>
        /// <para>脱离主线程由其他线程处理数据</para>
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>返回成功与否</returns>
        public void PushProcedure(string procedure, params DbParameter[] sqlParameters);
    }
}
