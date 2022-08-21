using System.Runtime.InteropServices;

namespace ECSharp.Variant
{
    /// <summary>
    /// 可变变量
    /// <para>支持Object、VarList、VarMap和所有基础类型</para>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public readonly partial struct Var
    {
        /**
         * 托管区
         */
        [FieldOffset(0)]
        private readonly string? stringValue;
        [FieldOffset(0)]
        private readonly object? objectValue;
        [FieldOffset(0)]
        private readonly VarList? listValue;
        [FieldOffset(0)]
        private readonly VarMap? mapValue;

        /**
         * 非托管区
         */
        [FieldOffset(8)]
        private readonly int intValue;
        [FieldOffset(8)]
        private readonly long longValue;
        [FieldOffset(8)]
        private readonly float floatValue;
        [FieldOffset(8)]
        private readonly double doubleValue;

        /**
         * 属性区
         */
        [FieldOffset(16)]
        private readonly VarType type;
    }
}
