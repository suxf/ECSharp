using ES.Database.Linq;
using ES.Time;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ES.Database.SQLServer
{
    /// <summary>
    /// SQLServer数据库访问助手
    /// <para>数据库异常可以通过 异常监听来获取</para>
    /// <para>详情参考：https://docs.microsoft.com/zh-cn/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring/ </para>
    /// </summary>
    public sealed class SqlServerDbHelper : ITimeUpdate, IDbHelper
    {
        /// <summary>
        /// 数据连接参数构造器
        /// </summary>
        private readonly SqlConnectionStringBuilder builder;
        /// <summary>
        /// sql队列用于缓存通过压入队列执行的sql对象
        /// </summary>
        private readonly Queue<string> SQLQueue = new Queue<string>();
        private readonly Queue<ProcedureCmd> ProcedureQueue = new Queue<ProcedureCmd>();
        /// <summary>
        /// 数据库异常监听
        /// </summary>
        private ISqlServerDbHelper? listener = null;

        private readonly BaseTimeFlow timeFlow;

        /// <summary>
        /// 获取数据库时间
        /// <para>如果数据库异常导致查询失败，则默认返回DateTime.Now</para>
        /// </summary>
        public DateTime Now
        {
            get
            {
                var result = CommandSQL("SELECT GETDATE()");

                if (result.EffectNum == 1)
                    return (DateTime)result.Rows![0][0];
                else
                    return DateTime.Now;
            }
        }

        /// <summary>
        /// SqlServer助手构造函数
        /// <para>存在参数不需要再次在额外配置中设置</para>
        /// <para>详情参考：https://docs.microsoft.com/zh-cn/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring/ </para>
        /// </summary>
        /// <param name="address">数据库地址，如果非默认端口需要带端口号，注意地址与端口号是以逗号分隔的</param>
        /// <param name="username">数据库账号</param>
        /// <param name="password">数据库密码</param>
        /// <param name="databaseName">数据库名称，默认为空</param>
        /// <param name="minPoolSize">数据库池连接最小值，默认为0</param>
        /// <param name="maxPoolSize">数据库池连接最大值，默认为100</param>
        /// <param name="extraConfig">数据库额外配置</param>
        public SqlServerDbHelper(string address, string username, string password, string? databaseName = null, int minPoolSize = 0, int maxPoolSize = 100, string? extraConfig = null)
        {
            if (!string.IsNullOrEmpty(extraConfig))
                builder = new SqlConnectionStringBuilder(extraConfig);
            else
                builder = new SqlConnectionStringBuilder();

            builder.DataSource = address;
            builder.UserID = username;
            builder.Password = password;

            if (!string.IsNullOrEmpty(databaseName)) builder.InitialCatalog = databaseName;

            builder.Pooling = true;
            if (minPoolSize > maxPoolSize) minPoolSize = maxPoolSize;
            builder.MinPoolSize = minPoolSize;
            builder.MaxPoolSize = maxPoolSize;
            builder.IntegratedSecurity = false;

            timeFlow = BaseTimeFlow.CreateTimeFlow(this/*, 0*/);
            timeFlow.StartTimeFlowES();
        }

        /// <summary>
        /// SqlServer助手构造函数
        /// <para>详情参考：https://docs.microsoft.com/zh-cn/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring/ </para>
        /// </summary>
        /// <param name="connectionString">连接配置</param>
        public SqlServerDbHelper(string connectionString)
        {
            builder = new SqlConnectionStringBuilder(connectionString);

            timeFlow = BaseTimeFlow.CreateTimeFlow(this/*, 0*/);
            timeFlow.StartTimeFlowES();
        }

        /// <summary>
        /// SqlServer助手构造函数
        /// <para>详情参考：https://docs.microsoft.com/zh-cn/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring/ </para>
        /// </summary>
        public SqlServerDbHelper(SqlConnectionStringBuilder sqlConnectionStringBuilder)
        {
            builder = sqlConnectionStringBuilder;

            timeFlow = BaseTimeFlow.CreateTimeFlow(this/*, 0*/);
            timeFlow.StartTimeFlowES();
        }

        /// <summary>
        /// 设置异常监听
        /// </summary>
        /// <param name="listener">异常监听器</param>
        public void SetExceptionListener(ISqlServerDbHelper listener)
        {
            this.listener = listener;
        }

        /// <summary>
        /// 检查是否连接
        /// </summary>
        /// <returns>成功连接返回true</returns>
        public bool CheckConnected()
        {
            using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
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
        /// 存储过程 1
        /// <para>返回值默认为整型，长度为4</para>
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>返回 ProcedureResult 失败为null</returns>
        public ProcedureResult Procedure(string procedure, params SqlParameter[] sqlParameters)
        {
            return Procedure(procedure, SqlDbType.Int, 4, sqlParameters);
        }

        /// <summary>
        /// 存储过程 2
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="retvalueDbType">返回值类型</param>
        /// <param name="retvalueSize">返回值大小</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>返回 ProcedureResult 失败为null</returns>
        public ProcedureResult Procedure(string procedure, SqlDbType retvalueDbType, int retvalueSize, params SqlParameter[] sqlParameters)
        {
            ProcedureResult result = new ProcedureResult();
            result.Procedure = procedure;
            result.IsCompleted = true;
            // 执行SQL语句过程
            try
            {
                using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        // 执行SQL
                        using (SqlCommand sqlCommand = new SqlCommand(procedure, conn))
                        {
                            sqlCommand.CommandType = CommandType.StoredProcedure;
                            sqlCommand.Parameters.AddRange(sqlParameters);
                            sqlCommand.Parameters.Add("@RETURN_VALUE", retvalueDbType, retvalueSize).Direction = ParameterDirection.ReturnValue;
                            SqlDataAdapter dataAdapter = new SqlDataAdapter();
                            dataAdapter.SelectCommand = sqlCommand;
                            DataSet myDataSet = new DataSet();
                            dataAdapter.Fill(myDataSet);

                            result.ReturnValue = sqlCommand.Parameters["@RETURN_VALUE"].Value;
                            result.SqlParameters = sqlCommand.Parameters;
                            result.Tables = myDataSet.Tables;

                            if (result.Tables.Count > 0) result.FirstRows = result.Tables[0].Rows;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.ReturnValue = -1;
                result.IsCompleted = false;

                if (listener != null)
                    listener.ProcedureException(this, procedure, sqlParameters, ex);
                else throw;
            }
            return result;
        }

        /// <summary>
        /// 存储过程 3
        /// <para>不需要返回任何数据</para>
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>影响数量 -1表示异常</returns>
        public int ProcedureNonQuery(string procedure, params SqlParameter[] sqlParameters)
        {
            // 执行SQL语句过程
            try
            {
                using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        // 执行SQL
                        using (SqlCommand sqlCommand = new SqlCommand(procedure, conn))
                        {
                            sqlCommand.CommandType = CommandType.StoredProcedure;
                            sqlCommand.Parameters.AddRange(sqlParameters);
                            return sqlCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (listener != null) listener.ProcedureException(this, procedure, sqlParameters, ex);
                else throw;
            }
            return -1;
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
                using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        if (obj != null && obj.Length > 0) sql = string.Format(sql, obj);
                        // 执行SQL
                        using (SqlCommand sqlCommand = new SqlCommand(sql, conn))
                        {
                            sqlCommand.CommandType = CommandType.Text;
                            SqlDataAdapter dataAdapter = new SqlDataAdapter();
                            dataAdapter.SelectCommand = sqlCommand;
                            result.DataSet = new DataSet();
                            dataAdapter.Fill(result.DataSet);

                            if (result.DataSet.Tables.Count > 0)
                            {
                                result.Tables = result.DataSet.Tables;
                                result.Rows = result.DataSet.Tables[0].Rows;
                                result.EffectNum = result.Rows.Count;
                            }
                            else
                            {
                                result.EffectNum = 0;
                            }
                        }
                    }
                    else
                    {
                        result.EffectNum = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                result.EffectNum = -1;
                if (listener != null) listener.CommandSQLException(this, sql, ex);
                else throw;
            }
            return result;
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
                using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        if (obj.Length > 0) sql = string.Format(sql, obj);
                        // 执行SQL
                        using (SqlCommand sqlCommand = new SqlCommand(sql, conn))
                        {
                            sqlCommand.CommandType = CommandType.Text;
                            return sqlCommand.ExecuteNonQuery();
                        }
                    }
                    return -1;
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
        /// 压入SQL队列，等待统一顺序执行【异步】
        /// <para>此操作适合非查询操作SQL,且对数据实时更新无要求的情况下方可使用</para>
        /// <para>脱离主线程由其他线程处理数据</para>
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>返回成功与否</returns>
        public void PushProcedure(string procedure, params SqlParameter[] sqlParameters)
        {
            lock (SQLQueue) ProcedureQueue.Enqueue(new ProcedureCmd(procedure, sqlParameters));
        }

        /// <summary>
        /// 创建一个数据实体组
        /// <para>同 DataAgent 使用相同</para>
        /// </summary>
        /// <param name="primaryKey">主键名，用于更新和寻找唯一依据字段</param>
        /// <param name="tableName">SQL表名</param>
        /// <param name="whereCondition">SQL条件判断条件【Where语句后的内容 包括排序等】</param>
        /// <param name="fieldNames">SQL字段名【默认为：*】</param>
        /// <param name="topNum">SQL取值数量【默认为：-1 无限】</param>
        /// <param name="isNoLock">是否不锁Sql，默认锁表</param>
        /// <returns></returns>
        public DataEntityRows? CreateDataEntityRows(string primaryKey, string tableName, string whereCondition, string fieldNames = "*", int topNum = -1, bool isNoLock = false)
        {
            return DataEntityRows.Load(this, primaryKey, tableName, whereCondition, fieldNames, topNum, isNoLock);
        }

        /// <summary>
        /// 创建一个配置加载器
        /// <para>此操作是利用sql查询到结果然后进行绑定</para>
        /// </summary>
        /// <param name="sql">需要查询的语句</param>
        public ConfigLoader<T> CreateConfigLoader<T>(string sql) where T : BaseConfigItem, new()
        {
            return new ConfigLoader<T>(this, sql);
        }

        /// <summary>
        /// 创建一个非关系型数据存储类
        /// </summary>
        /// <param name="keyName">数据所对应数据库中的key名</param>
        /// <param name="valueName">数据所对应数据库中的value名</param>
        /// <param name="tableName">数据所对应数据库的表名</param>
        /// <param name="syncPeriod">同步周期 用于控制写入到持久化数据库的时间 单位 毫秒 默认 1000ms</param>
        /// <param name="condition">数据查询的其他条件 如不需要则默认值即可，注意此处不需要再次写入key名所对应的条件了</param>
        public NoSqlStorage<T, U> CreateNoSqlStorage<T, U>(string keyName, string valueName, string tableName, int syncPeriod = 1000, string condition = "") where T : IComparable where U : IComparable
        {
            return new NoSqlStorage<T, U>(this, keyName, valueName, tableName, syncPeriod, condition);
        }

        /// <summary>
        /// 创建一个Sql构造器
        /// <para>需要传入一个非空数据库助手实例对象</para>
        /// </summary>
        public SqlBuilder CreateBuilder()
        {
            return SqlBuilder.Create(this);
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
                periodUpdate -= 1000;

                int runCount = TimeFlowThread.UtilMsMaxHandleCount;
                lock (SQLQueue)
                {
                    while (runCount > 0 && SQLQueue.Count > 0)
                    {
                        --runCount;
                        string sql = SQLQueue.Dequeue();
                        ExecuteSQL(sql);
                    }
                }

                lock (ProcedureQueue)
                {
                    while (runCount > 0 && ProcedureQueue.Count > 0)
                    {
                        --runCount;
                        ProcedureCmd pr = ProcedureQueue.Dequeue();
                        Procedure(pr.procedure, pr.sqlParameters);
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
