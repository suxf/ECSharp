﻿#if !NET462 && !NETSTANDARD2_0
namespace ECSharp.Hotfix
{
    /// <summary>
    /// 基础结构体值保存对象
    /// <para>用于保存和使用结构体类型的数据</para>
    /// </summary>
    public abstract class BaseStructValue
    {
        /// <summary>
        /// 结构体值
        /// </summary>
        protected object? _value;
    }
}
#endif