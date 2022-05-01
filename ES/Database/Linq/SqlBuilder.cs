using System;

namespace ES.Database.Linq
{
    /// <summary>
    /// SQL语句构建器
    /// <para>适合简单的语句构建</para>
    /// <para>可以帮助开发者减少拼写sql语句带来的不便</para>
    /// </summary>
    public class SqlBuilder
    {
        readonly IDbHelper dBHelper;

        int topCount = 0;
        string[] fields = Array.Empty<string>();
        object[] values = Array.Empty<object>();
        string conditions = "";
        string tableName = "";

        /// <summary>
        /// 构建函数
        /// <para>需要传入一个非空数据库助手实例对象</para>
        /// </summary>
        /// <param name="dBHelper">数据库助手实例</param>
        private SqlBuilder(IDbHelper dBHelper)
        {
            if (dBHelper == null)
            {
                Exception ex = new NullReferenceException("DBHelper Is Null");
                throw ex;
            }
            else
            {
                this.dBHelper = dBHelper;
            }
        }

        /// <summary>
        /// 创建一个构造器
        /// <para>需要传入一个非空数据库助手实例对象</para>
        /// </summary>
        /// <param name="dBHelper">数据库助手实例</param>
        /// <returns></returns>
        public static SqlBuilder Create(IDbHelper dBHelper)
        {
            return new SqlBuilder(dBHelper);
        }

        /// <summary>
        /// 顶部数量
        /// </summary>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public SqlBuilder Top(int topCount)
        {
            this.topCount = topCount;
            return this;
        }

        /// <summary>
        /// 字段
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlBuilder Fields(params string[] fields)
        {
            for (int i = 0, len = fields.Length; i < len; i++) fields[i] = $"[{fields[i]}]";
            this.fields = fields;
            return this;
        }

        /// <summary>
        /// 值
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public SqlBuilder Values(params object[] values)
        {
            for (int i = 0, len = values.Length; i < len; i++) values[i] = $"'{values[i]}'";
            this.values = values;
            return this;
        }

        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public SqlBuilder Table(string tableName)
        {
            this.tableName = tableName;
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public SqlBuilder Where(string conditions)
        {
            this.conditions = conditions;
            return this;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="isLock">是否锁（NOLOCK) 默认有锁</param>
        /// <returns></returns>
        public CommandResult Select(bool isLock = true)
        {
            return dBHelper.CommandSQL($"SELECT {(topCount > 0 ? ("TOP " + topCount) : "")} {string.Join(",", fields)} FROM {tableName} {(isLock ? "" : "WITH(NOLOCK)")} WHERE {conditions};");
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        public CommandResult Insert()
        {
            return dBHelper.CommandSQL($"INSERT {tableName} ({string.Join(",", fields)}) VALUES ({string.Join(",", values)});");
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public int Update()
        {
            if (fields.Length != 0 && values.Length != 0 && fields.Length == values.Length)
            {
                string[] kvStrs;
                kvStrs = new string[fields.Length];
                for (int i = 0, len = kvStrs.Length; i < len; i++) kvStrs[i] = $"[{fields[i]}] = '{values[i]}'";
                return dBHelper.ExecuteSQL($"UPDATE {tableName} SET {string.Join(",", kvStrs)} WHERE {conditions};");
            }
            return -1;
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public int Delete()
        {
            return dBHelper.ExecuteSQL($"DELETE FROM {tableName} WHERE {conditions};");
        }

    }
}
