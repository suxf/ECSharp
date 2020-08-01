using ES.Common.Time;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace ES.Data.Database.SQLServer.Linq
{
    /// <summary>
    /// Sqlserver数据缓存助理数据组[线程安全]
    /// 是一个集成化数据高速操作（查询，更新）的助手对象
    /// 使用此类可以更加有效的进行数据库数据的常规操作。
    /// 此类固定同步数据库周期为：1秒
    /// 数据助理产生的对象
    /// 用于托管数据操作的代理类
    /// 如果有一条数据不进行任何读写操作一定时间[默认300s有效]后，下一次操作必定重新读取数据库最新数据
    /// </summary>
    public class DataAgentRows : TimeFlow, IEnumerable<DataAgentRow>
    {
        /// <summary>
        /// 数据库对象
        /// </summary>
        public SQLServerDBHelper dBHelper = null;
        /// <summary>
        /// 表名
        /// </summary>
        public string tableName { internal set; get; } = null;
        /// <summary>
        /// 主键
        /// </summary>
        public string primaryKey { internal set; get; } = null;
        /// <summary>
        /// 查询的字段名
        /// </summary>
        public string fieldNames { internal set; get; } = null;
        /// <summary>
        /// 记录字典
        /// </summary>
        private ConcurrentDictionary<object, DataAgentRow> rows = new ConcurrentDictionary<object, DataAgentRow>();

        /// <summary>
        /// 实际周期
        /// </summary>
        private int realPeriod = 1000;
        /// <summary>
        /// 当前周期记录
        /// </summary>
        private int period = 0;

        /// <summary>
        /// 读取数据对
        /// </summary>
        /// <param name="dBHelper">数据库链接对象</param>
        /// <param name="primaryKey">主键名，用于更新和寻找唯一依据字段</param>
        /// <param name="tableName">SQL表名</param>
        /// <param name="whereCondition">SQL条件判断条件【Where语句后的内容 包括排序等】</param>
        /// <param name="fieldNames">SQL字段名【默认为：*】</param>
        /// <param name="topNum">SQL取值数量【默认为：-1 无限】</param>
        /// <param name="isNoLock">是否不锁Sql，默认锁表</param>
        /// <returns></returns>
        public static DataAgentRows Load(SQLServerDBHelper dBHelper, string primaryKey, string tableName, string whereCondition, string fieldNames = "*", int topNum = -1, bool isNoLock = false)
        {
            if (dBHelper != null)
            {
                CommandResult result = dBHelper.CommandSQL($"SELECT {(topNum > -1 ? ("TOP(" + topNum + ")") : "")} {fieldNames} FROM {tableName} {(isNoLock ? "WITH(NOLOCK)" : "")} {(whereCondition != null && whereCondition != "" ? ("WHERE " + whereCondition) : "")}");
                if (result != null && result.effectNum > 0)
                {
                    DataAgentRows dataPairs = new DataAgentRows(dBHelper, result.collection, primaryKey, tableName, fieldNames);
                    return dataPairs;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取数据对象数据
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns></returns>
        public DataAgentRow this[object key]
        {
            get
            {
                if (rows.TryGetValue(key, out DataAgentRow value))
                    return value;
                else return null;
            }
        }

        /// <summary>
        /// 获取记录数量
        /// </summary>
        public int Length { get { return rows.Values.Count; } }

        /// <summary>
        /// 命名空间构造函数
        /// </summary>
        internal DataAgentRows(SQLServerDBHelper dBHelper, DataRowCollection collection, string primaryKey, string tableName, string fieldNames) : base(0)
        {
            this.dBHelper = dBHelper;
            this.tableName = tableName;
            this.primaryKey = primaryKey;
            this.fieldNames = fieldNames;
            foreach (DataRow dataRow in collection)
            {
                DataAgentRow dataObject = new DataAgentRow(this);
                foreach (object column in dataRow.Table.Columns) dataObject.data.TryAdd(column.ToString(), dataRow[column.ToString()]);
                rows.TryAdd(dataRow[primaryKey], dataObject);
            }
        }

        /// <summary>
        /// 设置缓存同步周期
        /// </summary>
        /// <param name="second">同步周期，单位s【小于等于0都为1s】</param>
        public void SetSyncPeriod(int second)
        {
            if (second <= 0) second = 1;
            {
                Interlocked.Exchange(ref realPeriod, second * 1000);
                Interlocked.Exchange(ref period, 0);
            }
        }

        /// <summary>
        /// 提交至数据库
        /// 将所有缓存数据立刻写入数据库
        /// </summary>
        public void CommitDB()
        {
            // 改变周期时间 达到周期
            Interlocked.Add(ref period, realPeriod);
            // 并且立刻执行更新操作
            UpdateDBHandle(0);
        }

        /// <summary>
        /// 设置数据过期时间，过期后会从数据库中重新拉取
        /// </summary>
        /// <param name="sec">过期时间，单位s【小于等于0都为1s】</param>
        public void SetExpiredTime(int sec)
        {
            if (sec <= 0) sec = 1;
            sec *= 1000;
            foreach (var item in rows)
            {
                DataAgentRow dataItem = item.Value;
                Interlocked.Exchange(ref dataItem.realExpiredTime, sec);
                Interlocked.Exchange(ref dataItem.expiredTime, 0);
            }
        }

        /// <summary>
        /// 更新句柄
        /// </summary>
        internal void UpdateDBHandle(int dt)
        {
            foreach (var Key in rows.Keys)
            {
                var Value = rows[Key];
                DataAgentRow dataItem = Value;
                // 已改变需要更新
                if (dataItem.bChangeState)
                {
                    // 拼接sql语句
                    dataItem.bChangeState = false;
                    Interlocked.Exchange(ref dataItem.expiredTime, 0);
                    StringBuilder value = new StringBuilder();
                    lock (dataItem.listChangeColumns)
                    {
                        foreach (var data in dataItem.data)
                        {
                            if (dataItem.listChangeColumns.Contains(data.Key))
                            {
                                value.AppendFormat("{0}='{1}',", data.Key, data.Value);
                            }
                        }
                        dataItem.listChangeColumns.Clear();
                    }
                    if (value.Length > 0) dBHelper.CommandSQL("UPDATE {0} SET {1} WHERE {2}='{3}'", tableName, value.Remove(value.Length - 1, 1).ToString(), primaryKey, Key);
                }
                else if (dataItem.bReadState)
                {
                    dataItem.bReadState = false;
                    Interlocked.Exchange(ref dataItem.expiredTime, 0);
                }
                else
                {
                    Interlocked.Add(ref dataItem.expiredTime, dt);
                }
            }
        }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DataAgentRow> GetEnumerator()
        {
            return rows.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 更新句柄【不需要操作】
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(int dt)
        {
            period += TimeFlowManager.timeFlowPeriod;
            if (period >= realPeriod)
            {
                period = 0;
                // 执行更新
                {
                    UpdateDBHandle(realPeriod);
                }
            }
        }
    }
}
