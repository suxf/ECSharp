﻿#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;

namespace ECSharp.Variant
{
    /// <summary>
    /// 可变变量异常
    /// </summary>
    public class VarException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public VarException(string? message) : base(message)
        {
        }
    }
}
