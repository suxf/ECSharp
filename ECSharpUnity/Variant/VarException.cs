#if UNITY_2020_1_OR_NEWER
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
        private VarException(string? message) : base(message)
        {
        }

        internal static VarException CreateTypeError(VarType type)
        {
            return new VarException($"Var Use [{type}] Error Type!");
        }

        internal static VarException CreateLengthError(int length)
        {
            return new VarException($"Max Length 255, Now Length Is {length}!");
        }
    }
}
