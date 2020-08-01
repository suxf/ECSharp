using System;

namespace ES.Data.Database.SQLServer.Linq
{
    /// <summary>
    /// SQL语句构建器
    /// 适合简单的语句构建
    /// 可以帮助开发者减少拼写sql语句带来的不便
    /// </summary>
    public class SQLBuilder
    {

        SQLServerDBHelper dBHelper = null;

        int topCount = 0;
        string[] fields = null;
        object[] values = null;
        string conditions = null;
        string tableName = null;


        /// <summary>
        /// 构建函数
        /// 需要传入一个非空数据库助手实例对象
        /// </summary>
        /// <param name="dBHelper">数据库助手实例</param>
        private SQLBuilder(SQLServerDBHelper dBHelper)
        {
            if (dBHelper == null)
            {
                Exception ex = new NullReferenceException("DBHelper Is Null");
                throw (ex);
            }
            else
            {
                this.dBHelper = dBHelper;
            }
        }

        /// <summary>
        /// 创建一个构造器
        /// 需要传入一个非空数据库助手实例对象
        /// </summary>
        /// <param name="dBHelper">数据库助手实例</param>
        /// <returns></returns>
        public static SQLBuilder Create(SQLServerDBHelper dBHelper)
        {
            return new SQLBuilder(dBHelper);
        }

        /// <summary>
        /// 顶部数量
        /// </summary>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public SQLBuilder Top(int topCount)
        {
            this.topCount = topCount;
            return this;
        }

        /// <summary>
        /// 字段
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SQLBuilder Fields(params string[] fields)
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
        public SQLBuilder Values(params object[] values)
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
        public SQLBuilder Table(string tableName)
        {
            this.tableName = tableName;
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public SQLBuilder Where(string conditions)
        {
            this.conditions = conditions;
            return this;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public CommandResult Select()
        {
            return dBHelper.CommandSQL($"SELECT {(topCount > 0 ? ("TOP " + topCount) : "")} {string.Join(',', fields)} FROM {tableName} WHERE {conditions};");
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        public CommandResult Insert()
        {
            return dBHelper.CommandSQL($"INSERT {tableName} ({string.Join(',', fields)}) VALUES ({string.Join(',', values)});");
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public int Update()
        {
            if (fields.Length == values.Length)
            {
                string[] kvStrs;
                kvStrs = new string[fields.Length];
                for (int i = 0, len = kvStrs.Length; i < len; i++) kvStrs[i] = $"[{fields[i]}] = '{values[i]}'";
                return dBHelper.ExecuteSQL($"UPDATE {tableName} SET {string.Join(',', kvStrs)} WHERE {conditions};");
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
