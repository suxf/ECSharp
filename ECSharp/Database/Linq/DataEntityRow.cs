using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace ECSharp.Database.Linq
{
    /// <summary>
    /// 数据实体记录[线程安全]
    /// <para>数据不进行任何读写操作一定时间[默认300s有效]后，下一次读取必定重新读取数据库最新数据</para>
    /// </summary>
    public class DataEntityRow : IEnumerable<KeyValuePair<string, object>>
    {
        internal DataEntityRows parent;

        internal int realExpiredTime = 300000;
        internal int expiredTime = 0;
        internal bool bReadState = false;
        internal bool bChangeState = false;
        internal ConcurrentDictionary<string, object> data = new ConcurrentDictionary<string, object>();
        internal List<string> listChangeColumns = new List<string>();

        internal DataEntityRow(DataEntityRows p) { parent = p; }

        /// <summary>
        /// 获取数据对象数据
        /// <para>这个可以直接获得未转类型的对象</para>
        /// <para>但是更加推荐使用 GetObject 来获取</para>
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns></returns>
        public object? this[string key]
        {
            get
            {
                bReadState = true;

                if (expiredTime >= realExpiredTime)
                    ReloadDB();

                if (data.TryGetValue(key, out object? value))
                    return value;
                else
                    return null;
            }

            set
            {
                bChangeState = true;

                if (expiredTime >= realExpiredTime)
                    ReloadDB();

                if (!listChangeColumns.Contains(key))
                {
                    lock (listChangeColumns)
                    {
                        if (!listChangeColumns.Contains(key))
                        {
                            listChangeColumns.Add(key);
                        }
                    }
                }

                data.AddOrUpdate(key, value!, (k, v) => value!);
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">基础引用类型</typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public DataEntity<T> GetObject<T>(string name) where T : IComparable
        {
            return new DataEntity<T>(name, this);
        }

        /// <summary>
        /// 获取字段数量
        /// </summary>
        public int Count { get { return data.Keys.Count; } }

        /// <summary>
        /// 获取主键值
        /// </summary>
        public object PrimaryKeyValue { get { return data[parent.PrimaryKey]; } }

        /// <summary>
        /// 重读数据库
        /// </summary>
        private void ReloadDB()
        {
            Interlocked.Exchange(ref expiredTime, 0);
            CommandResult result = parent.DBHelper.CommandSQL("SELECT {0} FROM {1} WHERE {2}='{3}'", parent.FieldNames, parent.TableName, parent.PrimaryKey, data[parent.PrimaryKey]);

            if (result != null && result.Rows != null && result.EffectNum > 0)
            {
                DataRow row = result.Rows[0];

                data.Clear();

                foreach (object? column in row.Table.Columns)
                {
                    if (column == null) continue;
                    data.TryAdd(column.ToString()!, row[column.ToString()!]);
                }
            }
        }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }
}
