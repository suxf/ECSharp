#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using ECSharp.Utils;
using System;
using System.Collections.Generic;
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
        /// <summary>
        /// 对象
        /// </summary>
        [FieldOffset(0)]
        public readonly object? Object;
        /// <summary>
        /// 列表
        /// </summary>
        [FieldOffset(0)]
        public readonly VarList? List;
        /// <summary>
        /// 字典
        /// </summary>
        [FieldOffset(0)]
        public readonly VarMap? Map;

        /**
         * 非托管区
         */
        [FieldOffset(8)]
        private readonly bool boolValue;
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
        /// <summary>
        /// 变量类型
        /// </summary>
        [FieldOffset(16)]
        public readonly VarType Type;


        /// <summary>
        /// 空值
        /// </summary>
        public static readonly Var Null = new Var();

        /// <summary>
        /// 值类型变量
        /// </summary>
        public Var(ValueType value)
        {
            boolValue = false;
            intValue = 0;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            stringValue = null;
            List = null;
            Map = null;

            Type = VarType.STRUCT;
            Object = value;
        }

        /// <summary>
        /// 对象变量
        /// </summary>
        public Var(object value)
        {
            boolValue = false;
            intValue = 0;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            stringValue = null;
            List = null;
            Map = null;

            Type = VarType.OBJECT;
            Object = value;
        }

        /// <summary>
        /// 枚举可变变量
        /// </summary>
        public Var(Enum value)
        {
            boolValue = false;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            stringValue = null;
            Object = null;
            List = null;
            Map = null;

            Type = VarType.INT32;
            intValue = Convert.ToInt32(value);
        }

        /// <summary>
        /// 整型可变变量
        /// </summary>
        public Var(int value)
        {
            boolValue = false;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            stringValue = null;
            Object = null;
            List = null;
            Map = null;

            Type = VarType.INT32;
            intValue = value;
        }

        /// <summary>
        /// 长整型可变变量
        /// </summary>
        public Var(long value)
        {
            boolValue = false;
            intValue = 0;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            stringValue = null;
            Object = null;
            List = null;
            Map = null;

            Type = VarType.INT64;
            longValue = value;
        }

        /// <summary>
        /// 单精度浮点型可变变量
        /// </summary>
        public Var(float value)
        {
            boolValue = false;
            intValue = 0;
            longValue = 0L;
            doubleValue = 0.0D;
            stringValue = null;
            Object = null;
            List = null;
            Map = null;

            Type = VarType.FLOAT;
            floatValue = value;
        }

        /// <summary>
        /// 双精度浮点型可变变量
        /// </summary>
        public Var(double value)
        {
            boolValue = false;
            intValue = 0;
            longValue = 0L;
            floatValue = 0.0F;
            stringValue = null;
            Object = null;
            List = null;
            Map = null;

            Type = VarType.DOUBLE;
            doubleValue = value;
        }

        /// <summary>
        /// 布尔型可变变量
        /// </summary>
        public Var(bool value)
        {
            intValue = 0;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            stringValue = null;
            Object = null;
            List = null;
            Map = null;

            Type = VarType.BOOL;
            boolValue = value;
        }

        /// <summary>
        /// 可变变量
        /// </summary>
        public Var(string value)
        {
            boolValue = false;
            intValue = 0;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            Object = null;
            List = null;
            Map = null;

            Type = VarType.STRING;
            stringValue = value;
        }

        /// <summary>
        /// 可变变量
        /// </summary>
        public Var(VarList value)
        {
            boolValue = false;
            intValue = 0;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            stringValue = null;
            Object = null;
            Map = null;

            Type = VarType.VARLIST;
            List = value;
        }

        /// <summary>
        /// 可变变量
        /// </summary>
        public Var(VarMap value)
        {
            boolValue = false;
            intValue = 0;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            stringValue = null;
            Object = null;
            List = null;

            Type = VarType.VARMAP;
            Map = value;
        }

        /// <summary>
        /// 枚举值变量
        /// </summary>
        /// <param name="value">字节型变量</param>
        public static implicit operator Var(Enum value)
            => new Var(value);
        /// <summary>
        /// 整型变量
        /// </summary>
        /// <param name="value">整型变量</param>
        public static implicit operator Var(int value)
            => new Var(value);
        /// <summary>
        /// 长整型变量
        /// </summary>
        /// <param name="value">长整型变量</param>
        public static implicit operator Var(long value)
            => new Var(value);
        /// <summary>
        /// 单精度浮点型变量
        /// </summary>
        /// <param name="value">单精度浮点型变量</param>
        public static implicit operator Var(float value)
            => new Var(value);
        /// <summary>
        /// 双精度浮点型变量
        /// </summary>
        /// <param name="value">双精度浮点型变量</param>
        public static implicit operator Var(double value)
            => new Var(value);
        /// <summary>
        /// 布尔型变量
        /// </summary>
        /// <param name="value">布尔型变量</param>
        public static implicit operator Var(bool value)
            => new Var(value);
        /// <summary>
        /// 字符串型变量
        /// </summary>
        /// <param name="value">字符串型变量</param>
        public static implicit operator Var(string value)
            => new Var(value);
        /// <summary>
        /// 可变变量列表
        /// </summary>
        /// <param name="value">字符串型变量</param>
        public static implicit operator Var(VarList value)
            => new Var(value);
        /// <summary>
        /// 可变变量字典
        /// </summary>
        /// <param name="value">字符串型变量</param>
        public static implicit operator Var(VarMap value)
            => new Var(value);

        /// <summary>
        /// 整型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator int(Var value)
        {
            switch (value.Type)
            {
                case VarType.INT32:
                    // case VarType.UINT32:
                    return value.intValue;
                case VarType.INT64:
                    // case VarType.UINT64:
                    return (int)value.longValue;
                case VarType.FLOAT:
                    return (int)value.floatValue;
                case VarType.DOUBLE:
                    return (int)value.doubleValue;
                default:
                    throw new VarException($"Var Use [{value.Type}] Error Type!");
            }
        }

        /// <summary>
        /// 长整型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator long(Var value)
        {
            switch (value.Type)
            {
                case VarType.INT32:
                    return value.intValue;
                case VarType.INT64:
                    return value.longValue;
                case VarType.FLOAT:
                    return (long)value.floatValue;
                case VarType.DOUBLE:
                    return (long)value.doubleValue;
                default:
                    throw new VarException($"Var Use [{value.Type}] Error Type!");
            }
        }

        /// <summary>
        /// 单精度浮点型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator float(Var value)
        {
            switch (value.Type)
            {
                case VarType.INT32:
                    return value.intValue;
                case VarType.INT64:
                    return value.longValue;
                case VarType.FLOAT:
                    return value.floatValue;
                case VarType.DOUBLE:
                    return (float)value.doubleValue;
                default:
                    throw new VarException($"Var Use [{value.Type}] Error Type!");
            }
        }

        /// <summary>
        /// 双精度浮点型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator double(Var value)
        {
            switch (value.Type)
            {
                case VarType.INT32:
                    return value.intValue;
                case VarType.INT64:
                    return value.longValue;
                case VarType.FLOAT:
                    return value.floatValue;
                case VarType.DOUBLE:
                    return value.doubleValue;
                default:
                    throw new VarException($"Var Use [{value.Type}] Error Type!");
            }
        }

        /// <summary>
        /// 布尔型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator bool(Var value)
        {
            return value.boolValue;
        }

        /// <summary>
        /// 字符串型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator string(Var value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Type)
            {
                case VarType.INT32:
                    return intValue.ToString();
                case VarType.INT64:
                    return longValue.ToString();
                case VarType.FLOAT:
                    return floatValue.ToString();
                case VarType.DOUBLE:
                    return doubleValue.ToString();
                case VarType.BOOL:
                    return boolValue.ToString();
                case VarType.STRING:
                    return stringValue ?? "null";
                case VarType.STRUCT:
                case VarType.OBJECT:
                    return Object?.ToString() ?? "null";
                case VarType.VARLIST:
                    return List?.ToString() ?? "null";
                case VarType.VARMAP:
                    return Map?.ToString() ?? "null";
                default:
                    return "null";
            }
        }

        /// <summary>
        /// 转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToEnum<T>() where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), intValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            return obj is Var var && var == this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
#if !UNITY_2020_1_OR_NEWER && !NET462 && !NETSTANDARD2_0
            return HashCode.Combine(Type, intValue, longValue, floatValue, stringValue, Object, List, Map);
#else
			return Type.GetHashCode() ^ intValue.GetHashCode() ^ longValue.GetHashCode() ^ floatValue.GetHashCode() ^ doubleValue.GetHashCode() ^ stringValue?.GetHashCode() ?? 0 ^ Object?.GetHashCode() ?? 0 ^ List?.GetHashCode() ?? 0 ^ Map?.GetHashCode() ?? 0;
#endif
        }

        /// <summary>
        /// 是否为空类型
        /// </summary>
        public bool IsNull()
        {
            return Type == VarType.NULL;
        }

        /// <summary>
        /// 是否为数字类型
        /// </summary>
        public bool IsNumber()
        {
            return Type == VarType.INT32 || Type == VarType.INT64 || Type == VarType.FLOAT || Type == VarType.DOUBLE;
        }

        /// <summary>
        /// 是否为字符串类型
        /// </summary>
        public bool IsString()
        {
            return Type == VarType.STRING;
        }

        /// <summary>
        /// 转字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] bytes;
            switch (Type)
            {
                case VarType.INT32:
                    if (sbyte.MinValue <= intValue && intValue <= sbyte.MaxValue)
                    { bytes = new byte[2] { (byte)VarType.SBYTE, (byte)intValue }; }
                    else if (byte.MinValue <= intValue && intValue <= byte.MaxValue)
                    { bytes = new byte[2] { (byte)VarType.BYTE, (byte)intValue }; }
                    else if (short.MinValue <= intValue && intValue <= short.MaxValue)
                    {
                        bytes = new byte[3];
                        bytes[0] = (byte)VarType.INT16;
                        Buffer.BlockCopy(ByteConverter.GetBytes((short)intValue), 0, bytes, 1, 2);
                    }
                    else if (ushort.MinValue <= intValue && intValue <= ushort.MaxValue)
                    {
                        bytes = new byte[3];
                        bytes[0] = (byte)VarType.UINT16;
                        Buffer.BlockCopy(ByteConverter.GetBytes((ushort)intValue), 0, bytes, 1, 2);
                    }
                    else
                    {
                        bytes = new byte[5];
                        bytes[0] = (byte)VarType.INT32;
                        Buffer.BlockCopy(ByteConverter.GetBytes(intValue), 0, bytes, 1, 4);
                    }
                    break;
                case VarType.INT64:
                    if (sbyte.MinValue <= longValue && longValue <= sbyte.MaxValue)
                    { bytes = new byte[2] { (byte)VarType.SBYTE, (byte)longValue }; }
                    else if (byte.MinValue <= longValue && longValue <= byte.MaxValue)
                    { bytes = new byte[2] { (byte)VarType.BYTE, (byte)longValue }; }
                    else if (short.MinValue <= longValue && longValue <= short.MaxValue)
                    {
                        bytes = new byte[3];
                        bytes[0] = (byte)VarType.INT16;
                        Buffer.BlockCopy(ByteConverter.GetBytes((short)longValue), 0, bytes, 1, 2);
                    }
                    else if (ushort.MinValue <= longValue && longValue <= ushort.MaxValue)
                    {
                        bytes = new byte[3];
                        bytes[0] = (byte)VarType.UINT16;
                        Buffer.BlockCopy(ByteConverter.GetBytes((ushort)longValue), 0, bytes, 1, 2);
                    }
                    else if (int.MinValue <= longValue && longValue <= int.MaxValue)
                    {
                        bytes = new byte[5];
                        bytes[0] = (byte)VarType.INT32;
                        Buffer.BlockCopy(ByteConverter.GetBytes((int)longValue), 0, bytes, 1, 4);
                    }
                    else if (uint.MinValue <= longValue && longValue <= uint.MaxValue)
                    {
                        bytes = new byte[5];
                        bytes[0] = (byte)VarType.UINT32;
                        Buffer.BlockCopy(ByteConverter.GetBytes((uint)longValue), 0, bytes, 1, 4);
                    }
                    else
                    {
                        bytes = new byte[9];
                        bytes[0] = (byte)VarType.INT64;
                        Buffer.BlockCopy(ByteConverter.GetBytes(longValue), 0, bytes, 1, 8);
                    }
                    break;
                case VarType.FLOAT:
                    int intVal = (int)floatValue;
                    if (intVal == doubleValue)
                    {
                        if (sbyte.MinValue <= intVal && intVal <= sbyte.MaxValue)
                        { bytes = new byte[2] { (byte)VarType.SBYTE, (byte)intVal }; }
                        else if (byte.MinValue <= intVal && intVal <= byte.MaxValue)
                        { bytes = new byte[2] { (byte)VarType.BYTE, (byte)intVal }; }
                        else if (short.MinValue <= intVal && intVal <= short.MaxValue)
                        {
                            bytes = new byte[3];
                            bytes[0] = (byte)VarType.INT16;
                            Buffer.BlockCopy(ByteConverter.GetBytes((short)intVal), 0, bytes, 1, 2);
                        }
                        else if (ushort.MinValue <= intVal && intVal <= ushort.MaxValue)
                        {
                            bytes = new byte[3];
                            bytes[0] = (byte)VarType.UINT16;
                            Buffer.BlockCopy(ByteConverter.GetBytes((ushort)intVal), 0, bytes, 1, 2);
                        }
                        else if (int.MinValue <= intVal && intVal <= int.MaxValue)
                        {
                            bytes = new byte[5];
                            bytes[0] = (byte)VarType.INT32;
                            Buffer.BlockCopy(ByteConverter.GetBytes(intVal), 0, bytes, 1, 4);
                        }
                        else
                        {
                            bytes = new byte[5];
                            bytes[0] = (byte)VarType.UINT32;
                            Buffer.BlockCopy(ByteConverter.GetBytes((uint)intVal), 0, bytes, 1, 4);
                        }
                    }
                    else
                    {
                        bytes = new byte[5];
                        bytes[0] = (byte)VarType.FLOAT;
                        Buffer.BlockCopy(ByteConverter.GetBytes((float)doubleValue), 0, bytes, 1, 4);
                    }
                    break;
                case VarType.DOUBLE:
                    long longVal = (long)doubleValue;
                    if (longVal == doubleValue)
                    {
                        if (sbyte.MinValue <= longVal && longVal <= sbyte.MaxValue)
                        { bytes = new byte[2] { (byte)VarType.SBYTE, (byte)longVal }; }
                        else if (byte.MinValue <= longVal && longVal <= byte.MaxValue)
                        { bytes = new byte[2] { (byte)VarType.BYTE, (byte)longVal }; }
                        else if (short.MinValue <= longVal && longVal <= short.MaxValue)
                        {
                            bytes = new byte[3];
                            bytes[0] = (byte)VarType.INT16;
                            Buffer.BlockCopy(ByteConverter.GetBytes((short)longVal), 0, bytes, 1, 2);
                        }
                        else if (ushort.MinValue <= longVal && longVal <= ushort.MaxValue)
                        {
                            bytes = new byte[3];
                            bytes[0] = (byte)VarType.UINT16;
                            Buffer.BlockCopy(ByteConverter.GetBytes((ushort)longVal), 0, bytes, 1, 2);
                        }
                        else if (int.MinValue <= longVal && longVal <= int.MaxValue)
                        {
                            bytes = new byte[5];
                            bytes[0] = (byte)VarType.INT32;
                            Buffer.BlockCopy(ByteConverter.GetBytes((int)longVal), 0, bytes, 1, 4);
                        }
                        else if (uint.MinValue <= longVal && longVal <= uint.MaxValue)
                        {
                            bytes = new byte[5];
                            bytes[0] = (byte)VarType.UINT32;
                            Buffer.BlockCopy(ByteConverter.GetBytes((uint)longVal), 0, bytes, 1, 4);
                        }
                        else
                        {
                            bytes = new byte[9];
                            bytes[0] = (byte)VarType.INT64;
                            Buffer.BlockCopy(ByteConverter.GetBytes(longVal), 0, bytes, 1, 8);
                        }
                    }
                    else
                    {
                        bytes = new byte[9];
                        bytes[0] = (byte)VarType.DOUBLE;
                        Buffer.BlockCopy(ByteConverter.GetBytes(doubleValue), 0, bytes, 1, 8);
                    }
                    break;
                case VarType.BOOL:
                    bytes = new byte[1] { (byte)((byte)VarType.BOOL | (boolValue ? 0x10 : 0x00)) };
                    break;
                case VarType.STRING:
                    if (stringValue == null)
                    {
                        bytes = ByteConverter.Empty;
                        break;
                    }
                    byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(stringValue);
                    int strlen = strBytes.Length;
                    if (strlen <= byte.MaxValue)
                    {
                        bytes = new byte[2 + strlen];
                        bytes[0] = (byte)VarType.STRING | 0x10;
                        bytes[1] = (byte)strlen;
                        Buffer.BlockCopy(strBytes, 0, bytes, 2, strlen);
                    }
                    else if (strlen <= short.MaxValue)
                    {
                        bytes = new byte[3 + strlen];
                        bytes[0] = (byte)VarType.STRING | 0x20;
                        bytes[1] = (byte)(strlen >> 8);
                        bytes[2] = (byte)strlen;
                        Buffer.BlockCopy(strBytes, 0, bytes, 3, strlen);
                    }
                    else
                    {
                        bytes = new byte[5 + strlen];
                        bytes[0] = (byte)VarType.STRING | 0x30;
                        bytes[1] = (byte)(strlen >> 24);
                        bytes[2] = (byte)(strlen >> 16);
                        bytes[3] = (byte)(strlen >> 8);
                        bytes[4] = (byte)strlen;
                        Buffer.BlockCopy(strBytes, 0, bytes, 5, strlen);
                    }
                    break;
                case VarType.VARLIST:
                    byte[] listBytes = (List ?? new VarList()).GetBytes();
                    int listlen = listBytes.Length;
                    bytes = new byte[1 + listlen];
                    bytes[0] = (byte)VarType.VARLIST;
                    Buffer.BlockCopy(listBytes, 0, bytes, 1, listlen);
                    break;
                case VarType.VARMAP:
                    byte[] mapBytes = (Map ?? new VarMap()).GetBytes();
                    int maplen = mapBytes.Length;
                    bytes = new byte[1 + maplen];
                    bytes[0] = (byte)VarType.VARMAP;
                    Buffer.BlockCopy(mapBytes, 0, bytes, 1, maplen);
                    break;
                case VarType.STRUCT:
                    if (Object == null) bytes = new byte[1] { (byte)VarType.NULL | 0xF0 };
                    else
                    {
                        string typeName = VarObjectMgr.RegisterObjectType(Object);
                        int typeNameSize = typeName.Length;
                        int size = Marshal.SizeOf(Object);
                        IntPtr bufferIntPtr = Marshal.AllocHGlobal(size);
                        try
                        {
                            Marshal.StructureToPtr(Object, bufferIntPtr, false);
                            if (size <= byte.MaxValue)
                            {
                                bytes = new byte[3 + size + typeNameSize];
                                bytes[0] = (byte)VarType.STRUCT | 0x10;
                                bytes[1] = (byte)size;
                                bytes[2] = (byte)typeNameSize;
                                Buffer.BlockCopy(System.Text.Encoding.UTF8.GetBytes(typeName), 0, bytes, 3, typeNameSize);
                                Marshal.Copy(bufferIntPtr, bytes, 3 + typeNameSize, size);
                            }
                            else if (size <= short.MaxValue)
                            {
                                bytes = new byte[4 + size + typeNameSize];
                                bytes[0] = (byte)VarType.STRUCT | 0x20;
                                bytes[1] = (byte)(size >> 8);
                                bytes[2] = (byte)size;
                                bytes[3] = (byte)typeNameSize;
                                Buffer.BlockCopy(System.Text.Encoding.UTF8.GetBytes(typeName), 0, bytes, 4, typeNameSize);
                                Marshal.Copy(bufferIntPtr, bytes, 4 + typeNameSize, size);
                            }
                            else
                            {
                                bytes = new byte[6 + size + typeNameSize];
                                bytes[0] = (byte)VarType.STRUCT | 0x30;
                                bytes[1] = (byte)(size >> 24);
                                bytes[2] = (byte)(size >> 16);
                                bytes[3] = (byte)(size >> 8);
                                bytes[4] = (byte)size;
                                bytes[5] = (byte)typeNameSize;
                                Buffer.BlockCopy(System.Text.Encoding.UTF8.GetBytes(typeName), 0, bytes, 6, typeNameSize);
                                Marshal.Copy(bufferIntPtr, bytes, 6 + typeNameSize, size);
                            }
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(bufferIntPtr);
                        }
                    }
                    break;
                case VarType.NULL:
                    bytes = new byte[1] { (byte)VarType.NULL | 0xF0 };
                    break;
                default:
                    bytes = ByteConverter.Empty;
                    break;
            }
            return bytes;
        }

        /// <summary>
        /// 通过字节转可变变量
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="startIndex">开始索引位</param>
        /// <param name="length">转化长度</param>
        /// <returns></returns>
        public static Var Parse(byte[] value, int startIndex, out int length)
        {
            ReadOnlySpan<byte> bytesSpan = value;
            if (bytesSpan.Length == 0)
            {
                length = 0;
                return Null;
            }
            VarType type = (VarType)(bytesSpan[startIndex] & 0x0F);
            switch (type)
            {
                case VarType.BYTE:
                    length = 2;
                    return bytesSpan[startIndex + 1];
                case VarType.SBYTE:
                    length = 2;
                    return (sbyte)bytesSpan[startIndex + 1];
                case VarType.INT16:
                    length = 3;
                    return ByteConverter.ToInt16(value, startIndex + 1);
                case VarType.UINT16:
                    length = 3;
                    return ByteConverter.ToUInt16(value, startIndex + 1);
                case VarType.INT32:
                    length = 5;
                    return ByteConverter.ToInt32(value, startIndex + 1);
                case VarType.UINT32:
                    length = 5;
                    return ByteConverter.ToUInt32(value, startIndex + 1);
                case VarType.INT64:
                    length = 9;
                    return ByteConverter.ToInt64(value, startIndex + 1);
                case VarType.FLOAT:
                    length = 5;
                    return ByteConverter.ToSingle(value, startIndex + 1);
                case VarType.DOUBLE:
                    length = 9;
                    return ByteConverter.ToDouble(value, startIndex + 1);
                case VarType.BOOL:
                    length = 1;
                    return (bytesSpan[startIndex] & 0xF0) == 0x10;
                case VarType.STRING:
                    int tag = bytesSpan[startIndex] & 0xF0;
                    int strLen;
                    int index;
                    if (tag == 0x10) { strLen = bytesSpan[startIndex + 1]; length = strLen + 2; index = 2; }
                    else if (tag == 0x20) { strLen = bytesSpan[startIndex + 1] << 8 | bytesSpan[startIndex + 2]; length = strLen + 3; index = 3; }
                    else { strLen = bytesSpan[startIndex + 1] << 24 | bytesSpan[startIndex + 2] << 16 | bytesSpan[startIndex + 3] << 8 | bytesSpan[startIndex + 4]; length = strLen + 5; index = 5; }
                    return System.Text.Encoding.UTF8.GetString(value, startIndex + index, strLen);
                case VarType.VARLIST:
                    VarList list = VarList.Parse(value, startIndex + 1, out int listLen) ?? new VarList();
                    length = listLen + 1;
                    return list;
                case VarType.VARMAP:
                    VarMap map = VarMap.Parse(value, startIndex + 1, out int mapLen) ?? new VarMap();
                    length = mapLen + 1;
                    return map;
                case VarType.STRUCT:
                    int tag2 = bytesSpan[startIndex] & 0xF0;
                    int size;
                    int typeNameSize;
                    int index2;
                    if (tag2 == 0x10) { size = bytesSpan[startIndex + 1]; typeNameSize = bytesSpan[startIndex + 2]; length = size + 3 + typeNameSize; index2 = 3; }
                    else if (tag2 == 0x20) { size = bytesSpan[startIndex + 1] << 8 | bytesSpan[startIndex + 2]; typeNameSize = bytesSpan[startIndex + 3]; length = size + 4 + typeNameSize; index2 = 4; }
                    else { size = bytesSpan[startIndex + 1] << 24 | bytesSpan[startIndex + 2] << 16 | bytesSpan[startIndex + 3] << 8 | bytesSpan[startIndex + 4]; typeNameSize = bytesSpan[startIndex + 5]; length = size + 6 + typeNameSize; index2 = 6; }
                    string typeName = System.Text.Encoding.UTF8.GetString(value, startIndex + index2, typeNameSize);
                    Type? varObjType = VarObjectMgr.GetTypeByName(typeName);
                    if (varObjType == null)
                    {
                        return Null;
                    }
                    IntPtr allocIntPtr = Marshal.AllocHGlobal(size);
                    try
                    {
                        Marshal.Copy(value, startIndex + index2 + typeNameSize, allocIntPtr, size);
                        ValueType? structure = Marshal.PtrToStructure(allocIntPtr, varObjType) as ValueType;
                        if (structure == null)
                        {
                            return Null;
                        }
                        return new Var(structure);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(allocIntPtr);
                    }
                case VarType.NULL:
                    length = ((bytesSpan[startIndex] & 0xF0) == 0xF0) ? 1 : 0;
                    return Null;
                default:
                    length = 0;
                    return Null;
            }
        }

        /// <summary>
        /// 通过字节转可变变量
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="length">转化长度</param>
        /// <returns></returns>
        public static Var Parse(byte[] value, out int length)
        {
            return Parse(value, 0, out length);
        }

        /// <summary>
        /// 通过字节转可变变量
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="startIndex">开始索引位</param>
        /// <returns></returns>
        public static Var Parse(byte[] value, int startIndex)
        {
            return Parse(value, startIndex, out _);
        }

        /// <summary>
        /// 通过字节转可变变量
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <returns></returns>
        public static Var Parse(byte[] value)
        {
            return Parse(value, 0, out _);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator +(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.NULL: return Null;
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.NULL: return Null;
                        case VarType.INT32: return left.intValue + right.intValue;
                        case VarType.INT64: return left.intValue + right.longValue;
                        case VarType.FLOAT: return left.intValue + right.floatValue;
                        case VarType.DOUBLE: return left.intValue + right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.NULL: return Null;
                        case VarType.INT32: return left.longValue + right.intValue;
                        case VarType.INT64: return left.longValue + right.longValue;
                        case VarType.FLOAT: return left.longValue + right.floatValue;
                        case VarType.DOUBLE: return left.longValue + right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.Type)
                    {
                        case VarType.NULL: return Null;
                        case VarType.INT32: return left.floatValue + right.intValue;
                        case VarType.INT64: return left.floatValue + right.longValue;
                        case VarType.FLOAT: return left.floatValue + right.floatValue;
                        case VarType.DOUBLE: return left.floatValue + right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.Type)
                    {
                        case VarType.NULL: return Null;
                        case VarType.INT32: return left.doubleValue + right.intValue;
                        case VarType.INT64: return left.doubleValue + right.longValue;
                        case VarType.FLOAT: return left.doubleValue + right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue + right.doubleValue;
                    }
                    break;
            }
            return left.ToString() + right.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator -(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue - right.intValue;
                        case VarType.INT64: return left.intValue - right.longValue;
                        case VarType.FLOAT: return left.intValue - right.floatValue;
                        case VarType.DOUBLE: return left.intValue - right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue - right.intValue;
                        case VarType.INT64: return left.longValue - right.longValue;
                        case VarType.FLOAT: return left.longValue - right.floatValue;
                        case VarType.DOUBLE: return left.longValue - right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.floatValue - right.intValue;
                        case VarType.INT64: return left.floatValue - right.longValue;
                        case VarType.FLOAT: return left.floatValue - right.floatValue;
                        case VarType.DOUBLE: return left.floatValue - right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.doubleValue - right.intValue;
                        case VarType.INT64: return left.doubleValue - right.longValue;
                        case VarType.FLOAT: return left.doubleValue - right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue - right.doubleValue;
                    }
                    break;
            }
            return Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator *(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue * right.intValue;
                        case VarType.INT64: return left.intValue * right.longValue;
                        case VarType.FLOAT: return left.intValue * right.floatValue;
                        case VarType.DOUBLE: return left.intValue * right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue * right.intValue;
                        case VarType.INT64: return left.longValue * right.longValue;
                        case VarType.FLOAT: return left.longValue * right.floatValue;
                        case VarType.DOUBLE: return left.longValue * right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.floatValue * right.intValue;
                        case VarType.INT64: return left.floatValue * right.longValue;
                        case VarType.FLOAT: return left.floatValue * right.floatValue;
                        case VarType.DOUBLE: return left.floatValue * right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.doubleValue * right.intValue;
                        case VarType.INT64: return left.doubleValue * right.longValue;
                        case VarType.FLOAT: return left.doubleValue * right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue * right.doubleValue;
                    }
                    break;
            }
            return Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator /(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue / right.intValue;
                        case VarType.INT64: return left.intValue / right.longValue;
                        case VarType.FLOAT: return left.intValue / right.floatValue;
                        case VarType.DOUBLE: return left.intValue / right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue / right.intValue;
                        case VarType.INT64: return left.longValue / right.longValue;
                        case VarType.FLOAT: return left.longValue / right.floatValue;
                        case VarType.DOUBLE: return left.longValue / right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.floatValue / right.intValue;
                        case VarType.INT64: return left.floatValue / right.longValue;
                        case VarType.FLOAT: return left.floatValue / right.floatValue;
                        case VarType.DOUBLE: return left.floatValue / right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.doubleValue / right.intValue;
                        case VarType.INT64: return left.doubleValue / right.longValue;
                        case VarType.FLOAT: return left.doubleValue / right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue / right.doubleValue;
                    }
                    break;
            }
            return Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator %(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue % right.intValue;
                        case VarType.INT64: return left.intValue % right.longValue;
                        case VarType.FLOAT: return left.intValue % right.floatValue;
                        case VarType.DOUBLE: return left.intValue % right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue % right.intValue;
                        case VarType.INT64: return left.longValue % right.longValue;
                        case VarType.FLOAT: return left.longValue % right.floatValue;
                        case VarType.DOUBLE: return left.longValue % right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.floatValue % right.intValue;
                        case VarType.INT64: return left.floatValue % right.longValue;
                        case VarType.FLOAT: return left.floatValue % right.floatValue;
                        case VarType.DOUBLE: return left.floatValue % right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.doubleValue % right.intValue;
                        case VarType.INT64: return left.doubleValue % right.longValue;
                        case VarType.FLOAT: return left.doubleValue % right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue % right.doubleValue;
                    }
                    break;
            }
            return Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">可变变量</param>
        public static Var operator ++(Var value)
        {
            switch (value.Type)
            {
                case VarType.INT32:
                    return value.intValue + 1;
                case VarType.INT64:
                    return value.longValue + 1;
                case VarType.FLOAT:
                    return value.floatValue + 1;
                case VarType.DOUBLE:
                    return value.doubleValue + 1;
                default:
                    return Null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">可变变量</param>
        public static Var operator --(Var value)
        {
            switch (value.Type)
            {
                case VarType.INT32:
                    return value.intValue - 1;
                case VarType.INT64:
                    return value.longValue - 1;
                case VarType.FLOAT:
                    return value.floatValue - 1;
                case VarType.DOUBLE:
                    return value.doubleValue - 1;
                default:
                    return Null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">可变变量</param>
        public static Var operator ~(Var value)
        {
            switch (value.Type)
            {
                case VarType.INT32: return ~value.intValue;
                case VarType.INT64: return ~value.longValue;
                default: return Null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator &(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue & right.intValue;
                        case VarType.INT64: return left.intValue & right.longValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue & right.intValue;
                        case VarType.INT64: return left.longValue & right.longValue;
                    }
                    break;
            }
            return Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator |(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue | right.intValue;
                        case VarType.INT64: return (long)left.intValue | right.longValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue | (long)right.intValue;
                        case VarType.INT64: return left.longValue | right.longValue;
                    }
                    break;
            }
            return Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator ^(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue ^ right.intValue;
                        case VarType.INT64: return left.intValue ^ right.longValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue ^ right.intValue;
                        case VarType.INT64: return left.longValue ^ right.longValue;
                    }
                    break;
            }
            return Null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left">可变变量</param>
        /// <param name="right">位移量</param>
        public static Var operator <<(Var left, int right)
        {
            switch (left.Type)
            {
                case VarType.INT32: return left.intValue << right;
                case VarType.INT64: return left.longValue << right;
                default: return Null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left">可变变量</param>
        /// <param name="right">位移量</param>
        public static Var operator >>(Var left, int right)
        {
            switch (left.Type)
            {
                case VarType.INT32: return left.intValue >> right;
                case VarType.INT64: return left.longValue >> right;
                default: return Null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator <(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue < right.intValue;
                        case VarType.INT64: return left.intValue < right.longValue;
                        case VarType.FLOAT: return left.intValue < right.floatValue;
                        case VarType.DOUBLE: return left.intValue < right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue < right.intValue;
                        // case VarType.UINT32: return left.longValue < (uint)right.intValue;
                        case VarType.INT64: return left.longValue < right.longValue;
                        // case VarType.UINT64: return right.longValue >= 0 && left.longValue < right.longValue;
                        case VarType.FLOAT: return left.longValue < right.floatValue;
                        case VarType.DOUBLE: return left.longValue < right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.doubleValue < right.intValue;
                        case VarType.INT64: return left.doubleValue < right.longValue;
                        case VarType.FLOAT: return left.doubleValue < right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue < right.doubleValue;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator >(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue > right.intValue;
                        case VarType.INT64: return left.intValue > right.longValue;
                        case VarType.FLOAT: return left.intValue > right.floatValue;
                        case VarType.DOUBLE: return left.intValue > right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue > right.intValue;
                        case VarType.INT64: return left.longValue > right.longValue;
                        case VarType.FLOAT: return left.longValue > right.floatValue;
                        case VarType.DOUBLE: return left.longValue > right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.floatValue > right.intValue;
                        case VarType.INT64: return left.floatValue > right.longValue;
                        case VarType.FLOAT: return left.floatValue > right.floatValue;
                        case VarType.DOUBLE: return left.floatValue > right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.doubleValue > right.intValue;
                        case VarType.INT64: return left.doubleValue > right.longValue;
                        case VarType.FLOAT: return left.doubleValue > right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue > right.doubleValue;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator <=(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue <= right.intValue;
                        case VarType.INT64: return left.intValue <= right.longValue;
                        case VarType.FLOAT: return left.intValue <= right.floatValue;
                        case VarType.DOUBLE: return left.intValue <= right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue <= right.intValue;
                        case VarType.INT64: return left.longValue <= right.longValue;
                        case VarType.FLOAT: return left.longValue <= right.floatValue;
                        case VarType.DOUBLE: return left.longValue <= right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.floatValue <= right.intValue;
                        case VarType.INT64: return left.floatValue <= right.longValue;
                        case VarType.FLOAT: return left.floatValue <= right.floatValue;
                        case VarType.DOUBLE: return left.floatValue <= right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.doubleValue <= right.intValue;
                        case VarType.INT64: return left.doubleValue <= right.longValue;
                        case VarType.FLOAT: return left.doubleValue <= right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue <= right.doubleValue;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator >=(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue >= right.intValue;
                        case VarType.INT64: return left.intValue >= right.longValue;
                        case VarType.FLOAT: return left.intValue >= right.floatValue;
                        case VarType.DOUBLE: return left.intValue >= right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue >= right.intValue;
                        case VarType.INT64: return left.longValue >= right.longValue;
                        case VarType.FLOAT: return left.longValue >= right.floatValue;
                        case VarType.DOUBLE: return left.longValue >= right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.floatValue >= right.intValue;
                        case VarType.INT64: return left.floatValue >= right.longValue;
                        case VarType.FLOAT: return left.floatValue >= right.floatValue;
                        case VarType.DOUBLE: return left.floatValue >= right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.doubleValue >= right.intValue;
                        case VarType.INT64: return left.doubleValue >= right.longValue;
                        case VarType.FLOAT: return left.doubleValue >= right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue >= right.doubleValue;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(Var left, Var right)
        {
            switch (left.Type)
            {
                case VarType.INT32:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.intValue == right.intValue;
                        case VarType.INT64: return left.intValue == right.longValue;
                        case VarType.FLOAT: return left.intValue == right.floatValue;
                        case VarType.DOUBLE: return left.intValue == right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.longValue == right.intValue;
                        case VarType.INT64: return left.longValue == right.longValue;
                        case VarType.FLOAT: return left.longValue == right.floatValue;
                        case VarType.DOUBLE: return left.longValue == right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.floatValue == right.intValue;
                        case VarType.INT64: return left.floatValue == right.longValue;
                        case VarType.FLOAT: return left.floatValue == right.floatValue;
                        case VarType.DOUBLE: return left.floatValue == right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.Type)
                    {
                        case VarType.INT32: return left.doubleValue == right.intValue;
                        case VarType.INT64: return left.doubleValue == right.longValue;
                        case VarType.FLOAT: return left.doubleValue == right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue == right.doubleValue;
                    }
                    break;
                case VarType.BOOL:
                    if (right.Type == VarType.BOOL)
                        return left.boolValue == right.boolValue;
                    else
                        return false;
                case VarType.STRING:
                    if (right.Type == VarType.STRING)
                        return left.stringValue == right.stringValue;
                    else
                        return false;
                case VarType.STRUCT:
                    if (right.Type == VarType.STRUCT)
                        return left.Object == right.Object;
                    else
                        return false;
                case VarType.OBJECT:
                    if (right.Type == VarType.OBJECT)
                        return left.Object == right.Object;
                    else
                        return false;
                case VarType.VARLIST:
                    if (right.Type == VarType.VARLIST)
                        return left.List == right.List;
                    else
                        return false;
                case VarType.VARMAP:
                    if (right.Type == VarType.VARMAP)
                        return left.Map == right.Map;
                    else
                        return false;
                case VarType.NULL:
                    if (right.Type == VarType.NULL)
                        return true;
                    else
                        return false;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(Var left, Var right)
        {
            return !(left == right);
        }
    }
}
