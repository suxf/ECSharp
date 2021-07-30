using ES.Common.Time;
using ES.Common.Utils;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace ES.Data.Database.SQLServer.Linq
{
    /// <summary>
    /// 非关系型存储类
    /// <para>此类设计灵感源于非关系型数据库中基础原理</para>
    /// <para>使用起来只需要知道数据库中取出值和筛选条件即可类似使用字典方式来实现高速访问改变以及同步持久化</para>
    /// </summary>
    public class NoSqlStorage<T, U> : ITimeUpdate where T : IComparable where U : IComparable
    {
        private readonly SqlServerDbHelper dBHelper;

        private readonly string tableName;
        private readonly int syncPeriod;
        private readonly string condition;

        private readonly string keyName;
        private readonly string valueName;

        private readonly ConcurrentDictionary<T, U> keyValues = new ConcurrentDictionary<T, U>();

        private readonly ConcurrentQueue<T> keyUpdateQueue = new ConcurrentQueue<T>();
        private readonly ConcurrentQueue<T> keyInsertQueue = new ConcurrentQueue<T>();
        private readonly ConcurrentQueue<T> keyDeleteQueue = new ConcurrentQueue<T>();

        private readonly BaseTimeFlow timeFlow;

        /// <summary>
        /// 创建一个非关系型数据存储类
        /// </summary>
        /// <param name="dBHelper">对应的数据对象</param>
        /// <param name="keyName">数据所对应数据库中的key名</param>
        /// <param name="valueName">数据所对应数据库中的value名</param>
        /// <param name="tableName">数据所对应数据库的表名</param>
        /// <param name="syncPeriod">同步周期 用于控制写入到持久化数据库的时间 单位 毫秒 默认 1000ms</param>
        /// <param name="condition">数据查询的其他条件 如不需要则默认值即可，注意此处不需要再次写入key名所对应的条件了</param>
        public NoSqlStorage(SqlServerDbHelper dBHelper, string keyName, string valueName, string tableName, int syncPeriod = 1000, string condition = "")
        {
            this.dBHelper = dBHelper;
            this.keyName = keyName;
            this.valueName = valueName;
            this.tableName = tableName;
            this.syncPeriod = syncPeriod;
            this.condition = !string.IsNullOrEmpty(condition) ? (condition + " AND ") : "";

            timeFlow = BaseTimeFlow.CreateTimeFlow(this, 0);
            timeFlow.StartTimeFlowES();
        }

        /// <summary>
        /// 是否包含键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>存在为真，否则为假</returns>
        public bool ContainsKey(T key)
        {
            if (keyValues.ContainsKey(key))
            {
                return true;
            }
            else
            {
                var result = dBHelper.CommandSQL($"SELECT TOP 1 [{valueName}] FROM {tableName} WHERE {condition} {keyName}='{key}'");
                if (result.EffectNum > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">获取到的value值 失败为默认值</param>
        /// <returns>成功状态</returns>
        public bool TryGetValue(T key, out U value)
        {
            if (keyValues.TryGetValue(key, out value))
            {
                return true;
            }
            else
            {
                var result = dBHelper.CommandSQL($"SELECT TOP 1 [{valueName}] FROM {tableName} WHERE {condition} {keyName}='{key}'");
                if (result.EffectNum > 0)
                {
                    var val = value = (U)result.Collection[0][valueName];
                    keyValues.AddOrUpdate(key, value, (k, v) => val);
                    return true;
                }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 增加新的数据
        /// <para>注意使用此函数 请确保该记录不受其他约束条件影响 否则可能会在持久化存储中插入失败</para>
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">value值</param>
        /// <returns>如果已存在则返回 false</returns>
        public bool TryAdd(T key, U value)
        {
            var result = dBHelper.CommandSQL($"SELECT TOP 1 [{valueName}] FROM {tableName} WHERE {condition} {keyName}='{key}'");
            if (result.EffectNum <= 0)
            {
                keyValues.TryAdd(key, value);
                if (!keyInsertQueue.Contains(key)) keyInsertQueue.Enqueue(key);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">需要写入的value</param>
        /// <returns>成功状态</returns>
        public bool SetValue(T key, U value)
        {
            if (keyValues.TryGetValue(key, out var oldValue))
            {
                keyValues.TryUpdate(key, value, oldValue);
                if (!keyUpdateQueue.Contains(key)) keyUpdateQueue.Enqueue(key);
                return true;
            }
            else
            {
                var result = dBHelper.CommandSQL($"SELECT TOP 1 [{valueName}] FROM {tableName} WHERE {condition} {keyName}='{key}'");
                if (result.EffectNum > 0)
                {
                    var val = (U)result.Collection[0][valueName];
                    keyValues.AddOrUpdate(key, value, (k, v) => value);
                    if (!keyUpdateQueue.Contains(key) && !val.Equals(value)) keyUpdateQueue.Enqueue(key);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool DeleteKey(T key)
        {
            if (keyValues.TryGetValue(key, out _))
            {
                if (!keyDeleteQueue.Contains(key)) keyDeleteQueue.Enqueue(key);
                return true;
            }
            else
            {
                var result = dBHelper.CommandSQL($"SELECT TOP 1 [{valueName}] FROM {tableName} WHERE {condition} {keyName}='{key}'");
                if (result.EffectNum > 0)
                {
                    if (!keyDeleteQueue.Contains(key)) keyDeleteQueue.Enqueue(key);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 立即更新数据库
        /// </summary>
        public void Flush()
        {
            Interlocked.Exchange(ref syncPeriodNow, syncPeriod);
            Update(0);
        }

        /// <summary>
        /// 清空所有数据
        /// </summary>
        public void Clear()
        {
            keyUpdateQueue.ClearAll();
            keyInsertQueue.ClearAll();
            keyValues.Clear();
        }

        private int syncPeriodNow = 0;
        /// <summary>
        /// 系统调用
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(int deltaTime)
        {
            syncPeriodNow += deltaTime;
            if (syncPeriodNow >= syncPeriod)
            {
                syncPeriodNow = 0;
                while (keyInsertQueue.TryDequeue(out T key)) if (keyValues.TryGetValue(key, out U value)) dBHelper.ExecuteSQL($"INSERT {tableName} ({keyName}, [{valueName}]) VALUES ('{key}', '{value}')");
                while (keyUpdateQueue.TryDequeue(out T key)) if (keyValues.TryGetValue(key, out U value)) dBHelper.ExecuteSQL($"UPDATE {tableName} SET [{valueName}] = '{value}' WHERE {condition} {keyName}='{key}'");
                while (keyDeleteQueue.TryDequeue(out T key)) if (keyValues.TryRemove(key, out _)) dBHelper.ExecuteSQL($"DELETE FROM {tableName} WHERE {condition} {keyName}='{key}'");
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
