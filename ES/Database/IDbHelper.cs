#if !UNITY_2020_1_OR_NEWER
using ES.Time;

namespace ES.Database
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
    }
}
#endif