using ES.Common.Log;
using ES.Common.Time;
using ES.Data.Database.SQLServer.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ES.Data.Database.SQLServer
{
    /// <summary>
    /// SQLServer数据库访问助手
    /// 不可继承
    /// 详情参考：https://docs.microsoft.com/zh-cn/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring
    /// </summary>
    public sealed class SQLServerDBHelper : TimeFlow
    {
        /// <summary>
        /// 数据连接参数构造器
        /// </summary>
        private SqlConnectionStringBuilder builder = null;
        /// <summary>
        /// sql队列用于缓存通过压入队列执行的sql对象
        /// </summary>
        private Queue<string> SQLQueue = new Queue<string>();
        private Queue<ProcedureCmd> ProcedureQueue = new Queue<ProcedureCmd>();

        /// <summary>
        /// SqlServer助手构造函数
        /// 存在参数不需要再次在额外配置中设置
        /// 详情参考：https://docs.microsoft.com/zh-cn/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring
        /// </summary>
        /// <param name="address">数据库地址，如果非默认端口需要带端口号，注意地址与端口号是以逗号分隔的</param>
        /// <param name="username">数据库账号</param>
        /// <param name="password">数据库密码</param>
        /// <param name="databaseName">数据库名称，默认为空</param>
        /// <param name="minPoolSize">数据库池连接最小值，默认为0</param>
        /// <param name="maxPoolSize">数据库池连接最大值，默认为100</param>
        /// <param name="extraConfig">数据库额外配置</param>
        public SQLServerDBHelper(string address, string username, string password, string databaseName = null, int minPoolSize = 0, int maxPoolSize = 100, string extraConfig = null) : base(0)
        {
            if (extraConfig != null && extraConfig != "")
                builder = new SqlConnectionStringBuilder(extraConfig);
            else
                builder = new SqlConnectionStringBuilder();
            builder.DataSource = address;
            builder.UserID = username;
            builder.Password = password;
            if (databaseName != null) builder.InitialCatalog = databaseName;
            builder.Pooling = true;
            if (minPoolSize > maxPoolSize) minPoolSize = maxPoolSize;
            builder.MinPoolSize = minPoolSize;
            builder.MaxPoolSize = maxPoolSize;
            builder.IntegratedSecurity = false;
        }

        /// <summary>
        /// SqlServer助手构造函数
        /// 详情参考：https://docs.microsoft.com/zh-cn/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring
        /// </summary>
        /// <param name="connectionString">连接配置</param>
        public SQLServerDBHelper(string connectionString) : base(0)
        {
            builder = new SqlConnectionStringBuilder(connectionString);
        }

        /// <summary>
        /// SqlServer助手构造函数
        /// 详情参考：https://docs.microsoft.com/zh-cn/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring
        /// </summary>
        public SQLServerDBHelper(SqlConnectionStringBuilder sqlConnectionStringBuilder) : base(0)
        {
            builder = sqlConnectionStringBuilder;
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
                catch // (Exception ex)
                {
                    // Log.Exception(ex, "", "DBHelper", "CheckConnected", "SqlServer");
                    return false;
                }
                return conn.State == ConnectionState.Open;
            }
        }

        /// <summary>
        /// 存储过程 1
        /// 返回值默认为整型，长度为4
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>返回 ProcedureResult 失败为null</returns>
        public ProcedureResult Procedure(string procedure, params SqlParameter[] sqlParameters)
        {
            return Procedure(procedure, SQLServerDbType.Int, 4, sqlParameters);
        }

        /// <summary>
        /// 存储过程 2
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="retvalueDbType">返回值类型</param>
        /// <param name="retvalueSize">返回值大小</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>返回 ProcedureResult 失败为null</returns>
        public ProcedureResult Procedure(string procedure, SQLServerDbType retvalueDbType, int retvalueSize, params SqlParameter[] sqlParameters)
        {
            ProcedureResult result = new ProcedureResult();
            result.procedure = procedure;
            result.isCompleted = true;
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
                            sqlCommand.Parameters.Add("@RETURN_VALUE", (SqlDbType)retvalueDbType, retvalueSize).Direction = System.Data.ParameterDirection.ReturnValue;
                            SqlDataAdapter dataAdapter = new SqlDataAdapter();
                            dataAdapter.SelectCommand = sqlCommand;
                            DataSet myDataSet = new DataSet();
                            dataAdapter.Fill(myDataSet);
                            result.returnValue = sqlCommand.Parameters["@RETURN_VALUE"].Value;
                            result.SqlParameters = sqlCommand.Parameters;
                            result.Tables = myDataSet.Tables;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log.Exception(ex, procedure, "DBHelper", "Procedure", "SqlServer");
                result.isCompleted = false;
                result.exception = ex;
            }
            return result;
        }

        /// <summary>
        /// 存储过程 不需要返回任何数据 3
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
            catch // (Exception ex)
            {
                // Log.Exception(ex, procedure, "DBHelper", "Procedure", "SqlServer");
            }
            return -1;
        }

        /// <summary>
        /// 执行查询SQL语句（SELECT适用和部分需要更新返回的SQL）
        /// </summary>
        /// <param name="sql">执行SQL</param>
        /// <param name="obj">格式化参数输入【类似string.Format】</param>
        /// <returns>返回成功与否</returns>
        public CommandResult CommandSQL(string sql, params object[] obj)
        {
            CommandResult result = new CommandResult();
            result.hasException = false;
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
                            result.dataSet = new DataSet();
                            dataAdapter.Fill(result.dataSet);
                            if (result.dataSet.Tables.Count > 0)
                            {
                                result.collection = result.dataSet.Tables[0].Rows;
                                result.effectNum = result.collection.Count;
                            }
                            else
                            {
                                result.effectNum = 0;
                            }
                        }
                    }
                    else
                    {
                        result.effectNum = -2;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log.Exception(ex, sql, "DBHelper", "CommandSQL", "SqlServer");
                result.hasException = true;
                result.exception = ex;
                result.effectNum = -1;
            }
            return result;
        }

        /// <summary>
        /// 执行修改SQL语句（非SELECT适用，只需要影响行数）
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
                        if (obj != null && obj.Length > 0) sql = string.Format(sql, obj);
                        // 执行SQL
                        using (SqlCommand sqlCommand = new SqlCommand(sql, conn))
                        {
                            sqlCommand.CommandType = CommandType.Text;
                            return sqlCommand.ExecuteNonQuery();
                        }
                    }
                    return -2;
                }
            }
            catch // (Exception ex)
            {
                // Log.Exception(ex, sql, "DBHelper", "CommandSQL2", "SqlServer");
                return -1;
            }
        }

        /// <summary>
        /// 压入SQL队列，等待统一顺序执行【异步】
        /// 此操作适合非查询操作SQL,且对数据实时更新无要求的情况下方可使用
        /// 脱离主线程由其他线程处理数据
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
        /// 此操作适合非查询操作SQL,且对数据实时更新无要求的情况下方可使用
        /// 脱离主线程由其他线程处理数据
        /// </summary>
        /// <param name="procedure">存储过程名称</param>
        /// <param name="sqlParameters">存储过程参数 建议使用Parameter生成</param>
        /// <returns>返回成功与否</returns>
        public void PushProcedure(string procedure, params SqlParameter[] sqlParameters)
        {
            lock (SQLQueue) ProcedureQueue.Enqueue(new ProcedureCmd(procedure, sqlParameters));
        }

        /// <summary>
        /// 加载数据缓存
        /// 同 DataAgent 使用相同
        /// </summary>
        /// <param name="primaryKey">主键名，用于更新和寻找唯一依据字段</param>
        /// <param name="tableName">SQL表名</param>
        /// <param name="whereCondition">SQL条件判断条件【Where语句后的内容 包括排序等】</param>
        /// <param name="fieldNames">SQL字段名【默认为：*】</param>
        /// <param name="topNum">SQL取值数量【默认为：-1 无限】</param>
        /// <param name="isNoLock">是否不锁Sql，默认锁表</param>
        /// <returns></returns>
        public DataAgentRows LoadDataCache(string primaryKey, string tableName, string whereCondition, string fieldNames = "*", int topNum = -1, bool isNoLock = false)
        {
            return DataAgentRows.Load(this, primaryKey, tableName, whereCondition, fieldNames, topNum, isNoLock);
        }

        /// <summary>
        /// 同步时间周期记录
        /// </summary>
        private int periodUpdate = 0;
        /// <summary>
        /// 通过时间流来更新通过队列执行的SQL
        /// 固定周期为 1s
        /// </summary>
        /// <param name="dt"></param>
        protected override void Update(int dt)
        {
            periodUpdate += timeFlowPeriod;
            if (periodUpdate >= 1000)
            {
                periodUpdate = 0;
                lock (SQLQueue) while (SQLQueue.TryDequeue(out var sql)) ExecuteSQL(sql, null);
                lock (ProcedureQueue) while (ProcedureQueue.TryDequeue(out var pr)) Procedure(pr.procedure, pr.sqlParameters);
            }
        }

        /// <summary>
        /// 内部调用
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal DataSet NoDBStorageSQL(string sql)
        {
            var result = CommandSQL(sql);
            if (result.effectNum > 0) return result.dataSet;
            return null;
        }
    }
}
