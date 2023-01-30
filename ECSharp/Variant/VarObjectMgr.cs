using System;
using System.Collections.Generic;

namespace ECSharp.Variant
{
    /// <summary>
    /// 可变对象管理器
    /// </summary>
    public static class VarObjectMgr
    {
        /// <summary>
        /// 通过字节转可变变量
        /// </summary>
        private readonly static Dictionary<string, Type> varObjectTypePairs = new Dictionary<string, Type>();

        /// <summary>
        /// 注册对象类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterObjectType<T>() where T : struct
        {
            Type type = typeof(T);
            string name = type.Name;
            if (!varObjectTypePairs.ContainsKey(name))
            {
                varObjectTypePairs.Add(name, type);
            }
            else
            {
                varObjectTypePairs[name] = type;
            }
        }

        /// <summary>
        /// 注册对象类型
        /// </summary>
        /// <param name="obj"></param>
        internal static string RegisterObjectType(object obj)
        {
            Type type = obj.GetType();
            string name = type.Name;
            if (!varObjectTypePairs.ContainsKey(name))
            {
                varObjectTypePairs.Add(name, type);
            }

            return name;
        }

        /// <summary>
        /// 通过名称获取类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static Type? GetTypeByName(string name)
        {
            if (!varObjectTypePairs.ContainsKey(name))
            {
                return null;
            }

            return varObjectTypePairs[name];
        }
    }
}
