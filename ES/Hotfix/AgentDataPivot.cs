﻿using System;
using System.Collections.Concurrent;

namespace ES.Hotfix
{
    /// <summary>
    /// 代理数据枢纽
    /// <para>所有代理的最上层数据存储都通过此类来执行存储释放</para>
    /// </summary>
    public static class AgentDataPivot
    {
        /// <summary>
        /// 引用变量
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> objects = new ConcurrentDictionary<string, object>();
        /// <summary>
        /// 值变量
        /// </summary>
        private static readonly ConcurrentDictionary<string, StructValue> structs = new ConcurrentDictionary<string, StructValue>();

        /// <summary>
        /// 增加或获取对象
        /// </summary>
        public static T AddOrGetObject<T>(string key) where T : class, new()
        {
            if (!objects.TryGetValue(key, out var value))
            {
                var obj = new T();
                objects.TryAdd(key, obj);
                return obj;
            }
            return (T)value;
        }

        /// <summary>
        /// 增加或获取对象
        /// </summary>
        public static T AddOrGetObject<T>(string key, Func<T> action) where T : class
        {
            if(!objects.TryGetValue(key, out var value))
            {
                var obj = action.Invoke();
                objects.TryAdd(key, obj);
                return obj;
            }
            return (T)value;
        }

        /// <summary>
        /// 删除对象
        /// <para>有且删除则返回true</para>
        /// </summary>
        /// <param name="key">对象名称</param>
        /// <returns></returns>
        public static bool DeleteObject(string key)
        {
            return objects.TryRemove(key, out _);
        }

        /// <summary>
        /// 增加或获取结构值
        /// </summary>
        public static StructValue AddOrGetStruct<T>(string key, T defaultValue = default) where T : struct
        {
            if (!structs.TryGetValue(key, out var value))
            {
                var obj = new StructValue{ Value = defaultValue };
                structs.TryAdd(key, obj);
                return obj;
            }
            return value;
        }

        /// <summary>
        /// 删除结构值
        /// <para>有且删除则返回true</para>
        /// </summary>
        /// <param name="key">对象名称</param>
        /// <returns></returns>
        public static bool DeleteStruct(string key)
        {
            return structs.TryRemove(key, out _);
        }
    }
}