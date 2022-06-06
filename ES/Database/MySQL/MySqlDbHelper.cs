using ES.Time;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace ES.Database.MySQL
{
    /// <summary>
    /// MySQL数据库访问助手
    /// <para>详情参考：https://mysqlconnector.net/ </para>
    /// <para>数据库异常可以通过 异常监听来获取</para>
    /// </summary>
    public class MySqlDbHelper : ITimeUpdate, IDbHelper
    {
        /// <summary>
        /// 数据连接参数构造器
        /// </summary>
        private readonly MySqlConnectionStringBuilder builder;
        /// <summary>
        /// sql队列用于缓存通过压入队列执行的sql对象
        /// </summary>
        private readonly Queue<string> SQLQueue = new Queue<string>();
        /// <summary>
        /// 数据库异常监听
        /// </summary>
        private IMySqlDbHelper? listener = null;

        private readonly BaseTimeFlow timeFlow;

        /// <summary>
        /// 获取数据库时间
        /// <para>如果数据库异常导致查询失败，则默认返回DateTime.Now</para>
        /// </summary>
        public DateTime Now
        {
            get
            {
                var result = CommandSQL("select now()");

                if (result.EffectNum == 1)
                    return (DateTime)result.Rows![0][0];
                else
                    return DateTime.Now;
            }
        }

        /// <summary>
        /// MySQL助手构造函数
        /// <para>详情参考：https://mysqlconnector.net/ </para>
        /// <para>存在参数不需要再次在额外配置中设置</para>
        /// </summary>
        /// <param name="address">数据库地址</param>
        /// <param name="username">数据库账号</param>
        /// <param name="password">数据库密码</param>
        /// <param name="port">数据库端口 默认端口为3306</param>
        /// <param name="databaseName">数据库名称，默认为空</param>
        /// <param name="minPoolSize">数据库池连接最小值，默认为0</param>
        /// <param name="maxPoolSize">数据库池连接最大值，默认为100</param>
        /// <param name="extraConfig">数据库额外配置</param>
        public MySqlDbHelper(string address, string username, string password, uint port = 3306, string? databaseName = null, uint minPoolSize = 0, uint maxPoolSize = 100, string? extraConfig = null)
        {
            if (!string.IsNullOrEmpty(extraConfig))
#if !NET462 && !NETSTANDARD2_0
                builder = new MySqlConnectionStringBuilder(extraConfig);
#else
                builder = new MySqlConnectionStringBuilder(extraConfig!);
#endif
            else
                builder = new MySqlConnectionStringBuilder();

            builder.Server = address;
            builder.Port = port;
            builder.UserID = username;
            builder.Password = password;

            if (!string.IsNullOrEmpty(databaseName)) builder.Database = databaseName;

            builder.Pooling = true;

            if (minPoolSize > maxPoolSize)
                minPoolSize = maxPoolSize;

            builder.MinimumPoolSize = minPoolSize;
            builder.MaximumPoolSize = maxPoolSize;

            timeFlow = BaseTimeFlow.CreateTimeFlow(this/*, 0*/);
            timeFlow.StartTimeFlowES();
        }

        /// <summary>
        /// MySQL助手构造函数
        /// <para>详情参考：https://mysqlconnector.net/ </para>
        /// </summary>
        /// <param name="connectionString">连接配置</param>
        public MySqlDbHelper(string connectionString)
        {
            builder = new MySqlConnectionStringBuilder(connectionString);

            timeFlow = BaseTimeFlow.CreateTimeFlow(this/*, 0*/);
            timeFlow.StartTimeFlowES();
        }

        /// <summary>
        /// MySQL助手构造函数
        /// <para>详情参考：https://mysqlconnector.net/ </para>
        /// </summary>
        public MySqlDbHelper(MySqlConnectionStringBuilder mySqlConnectionStringBuilder)
        {
            builder = mySqlConnectionStringBuilder;

            timeFlow = BaseTimeFlow.CreateTimeFlow(this/*, 0*/);
            timeFlow.StartTimeFlowES();
        }

        /// <summary>
        /// 设置异常监听
        /// </summary>
        /// <param name="listener">异常监听器</param>
        public void SetExceptionListener(IMySqlDbHelper listener)
        {
            this.listener = listener;
        }

        /// <summary>
        /// 检查是否连接
        /// </summary>
        /// <returns>成功连接返回true</returns>
        public bool CheckConnected()
        {
            using (MySqlConnection conn = new MySqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    if (listener != null) listener.CheckConnectedException(this, ex);
                    else throw;

                    return false;
                }
                return conn.State == ConnectionState.Open;
            }
        }

        /// <summary>
        /// 获取数据库连接地址
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            return builder.ConnectionString;
        }

        /// <summary>
        /// 执行查询SQL语句
        /// <para>SELECT适用和部分需要更新返回的SQL</para>
        /// </summary>
        /// <param name="sql">执行SQL</param>
        /// <param name="obj">格式化参数输入【类似string.Format】</param>
        /// <returns>返回成功与否</returns>
        public CommandResult CommandSQL(string sql, params object[] obj)
        {
            CommandResult result = new CommandResult();
            // 执行SQL语句过程
            try
            {
                using (MySqlConnection conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        result.EffectNum = -1;
                        return result;
                    }

                    if (obj != null && obj.Length > 0) sql = string.Format(sql, obj);
                    // 执行SQL
                    using (MySqlCommand sqlCommand = new MySqlCommand(sql, conn))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        MySqlDataAdapter dataAdapter = new MySqlDataAdapter();
                        dataAdapter.SelectCommand = sqlCommand;
                        result.DataSet = new DataSet();
                        dataAdapter.Fill(result.DataSet);

                        if (result.DataSet.Tables.Count == 0)
                        {
                            result.EffectNum = 0;
                            return result;
                        }

                        result.Tables = result.DataSet.Tables;
                        result.Rows = result.DataSet.Tables[0].Rows;
                        result.EffectNum = result.Rows.Count;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.EffectNum = -1;
                if (listener != null) listener.CommandSQLException(this, sql, ex);
                else throw;
                return result;
            }
        }

        /// <summary>
        /// 执行修改SQL语句
        /// <para>非SELECT适用，只需要影响行数</para>
        /// </summary>
        /// <param name="sql">执行SQL</param>
        /// <param name="obj">格式化参数输入【类似string.Format】</param>
        /// <returns>返回影响条数</returns>
        public int ExecuteSQL(string sql, params object[] obj)
        {
            // 执行SQL语句过程
            try
            {
                using (MySqlConnection conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        return -1;
                    }

                    if (obj.Length > 0) sql = string.Format(sql, obj);
                    // 执行SQL
                    using (MySqlCommand sqlCommand = new MySqlCommand(sql, conn))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        return sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                if (listener != null) listener.CommandSQLException(this, sql, ex);
                else throw;
                return -1;
            }
        }

        /// <summary>
        /// 压入SQL队列，等待统一顺序执行【异步】
        /// <para>此操作适合非查询操作SQL,且对数据实时更新无要求的情况下方可使用</para>
        /// <para>脱离主线程由其他线程处理数据</para>
        /// </summary>
        /// <param name="sql">执行SQL</param>
        /// <param name="obj">格式化参数输入【类似string.Format】</param>
        public void PushSQL(string sql, params object[] obj)
        {
            if (obj != null && obj.Length > 0) sql = string.Format(sql, obj);
            lock (SQLQueue) SQLQueue.Enqueue(sql);
        }

        /// <summary>
        /// 同步时间周期记录
        /// </summary>
        private int periodUpdate = 0;
        /// <summary>
        /// 通过时间流来更新通过队列执行的SQL
        /// <para>固定周期为 1s</para>
        /// </summary>
        /// <param name="dt"></param>
        public void Update(int dt)
        {
            periodUpdate += dt;
            if (periodUpdate >= 1000)
            {
                periodUpdate = 0;
                lock (SQLQueue)
                {
                    while (SQLQueue.Count > 0)
                    {
                        string sql = SQLQueue.Dequeue();
                        ExecuteSQL(sql);
                    }
                }
            }
        }

        /// <summary>
        /// 停止更新
        /// </summary>
        public void UpdateEnd()
        {
        }
    }
}
