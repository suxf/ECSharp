#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using ECSharp.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ECSharp.Variant
{
    /// <summary>
    /// 可变变量
    /// <para>支持Object、VarList、VarMap和所有基础类型</para>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Var
    {
        /**
         * 托管区
         */
        [FieldOffset(0)]
        private readonly string? stringValue;
        /// <summary>
        /// 列表
        /// </summary>
        [FieldOffset(0)]
        private readonly VarList? listValue;
        /// <summary>
        /// 字典
        /// </summary>
        [FieldOffset(0)]
        private readonly VarMap? mapValue;
        /// <summary>
        /// 对象
        /// </summary>
        [FieldOffset(0)]
        private readonly object? objectValue;

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
        private readonly VarType type;

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
            listValue = null;
            mapValue = null;

            type = VarType.STRUCT;
            objectValue = value;
        }

        /// <summary>
        /// 对象变量
        /// </summary>
        public Var(object value)
        {
            if (value == null)
            {
                type = VarType.NULL;

                boolValue = false;
                intValue = 0;
                longValue = 0L;
                floatValue = 0.0F;
                doubleValue = 0.0D;
                objectValue = null;
                listValue = null;
                mapValue = null;
                stringValue = null;
                return;
            }

            boolValue = false;
            intValue = 0;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            stringValue = null;
            listValue = null;
            mapValue = null;

            type = VarType.OBJECT;
            objectValue = value;
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
            objectValue = null;
            listValue = null;
            mapValue = null;

            type = VarType.INT32;
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
            objectValue = null;
            listValue = null;
            mapValue = null;

            type = VarType.INT32;
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
            objectValue = null;
            listValue = null;
            mapValue = null;

            type = VarType.INT64;
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
            objectValue = null;
            listValue = null;
            mapValue = null;

            type = VarType.FLOAT;
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
            objectValue = null;
            listValue = null;
            mapValue = null;

            type = VarType.DOUBLE;
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
            objectValue = null;
            listValue = null;
            mapValue = null;

            type = VarType.BOOL;
            boolValue = value;
        }

        /// <summary>
        /// 可变变量
        /// </summary>
        public Var(string value)
        {
            if (value == null)
            {
                type = VarType.NULL;

                boolValue = false;
                intValue = 0;
                longValue = 0L;
                floatValue = 0.0F;
                doubleValue = 0.0D;
                objectValue = null;
                listValue = null;
                mapValue = null;
                stringValue = null;
                return;
            }

            boolValue = false;
            intValue = 0;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            objectValue = null;
            listValue = null;
            mapValue = null;

            type = VarType.STRING;
            stringValue = value;
        }

        /// <summary>
        /// 可变变量
        /// </summary>
        public Var(VarList value)
        {
            if (value == null)
            {
                type = VarType.NULL;

                boolValue = false;
                intValue = 0;
                longValue = 0L;
                floatValue = 0.0F;
                doubleValue = 0.0D;
                objectValue = null;
                listValue = null;
                mapValue = null;
                stringValue = null;
                return;
            }

            boolValue = false;
            intValue = 0;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            stringValue = null;
            objectValue = null;
            mapValue = null;

            type = VarType.VARLIST;
            listValue = value;
        }

        /// <summary>
        /// 可变变量
        /// </summary>
        public Var(VarMap value)
        {
            if (value == null)
            {
                type = VarType.NULL;

                boolValue = false;
                intValue = 0;
                longValue = 0L;
                floatValue = 0.0F;
                doubleValue = 0.0D;
                objectValue = null;
                listValue = null;
                mapValue = null;
                stringValue = null;
                return;
            }

            boolValue = false;
            intValue = 0;
            longValue = 0L;
            floatValue = 0.0F;
            doubleValue = 0.0D;
            stringValue = null;
            objectValue = null;
            listValue = null;

            type = VarType.VARMAP;
            mapValue = value;
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
            switch (value.type)
            {
                case VarType.INT32:
                    return value.intValue;
                case VarType.INT64:
                    return (int)value.longValue;
                case VarType.FLOAT:
                    return (int)value.floatValue;
                case VarType.DOUBLE:
                    return (int)value.doubleValue;
                default:
                    throw VarException.CreateTypeError(value.type);
            }
        }

        /// <summary>
        /// 长整型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator long(Var value)
        {
            switch (value.type)
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
                    throw VarException.CreateTypeError(value.type);
            }
        }

        /// <summary>
        /// 单精度浮点型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator float(Var value)
        {
            switch (value.type)
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
                    throw VarException.CreateTypeError(value.type);
            }
        }

        /// <summary>
        /// 双精度浮点型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator double(Var value)
        {
            switch (value.type)
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
                    throw VarException.CreateTypeError(value.type);
            }
        }

        /// <summary>
        /// 布尔型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator bool(Var value)
        {
            if (value.type == VarType.BOOL)
                return value.boolValue;
            else
                return !value.IsNull;
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
        /// 列表变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator VarList(Var value)
        {
            if(value.type == VarType.VARLIST)
                return value.listValue!;
            else
                throw VarException.CreateTypeError(value.type);
        }

        /// <summary>
        /// 字典变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator VarMap(Var value)
        {
            if (value.type == VarType.VARMAP)
                return value.mapValue!;
            else
                throw VarException.CreateTypeError(value.type);
        }

        /// <summary>
        /// 整型变量
        /// </summary>
        public int Int32 => this;

        /// <summary>
        /// 长整型变量
        /// </summary>
        public long Int64 => this;

        /// <summary>
        /// 单精度浮点型变量
        /// </summary>
        public float Float => this;

        /// <summary>
        /// 双精度浮点型变量
        /// </summary>
        public double Double => this;

        /// <summary>
        /// 布尔型变量
        /// </summary>
        public bool Bool => this;

        /// <summary>
        /// 字符串
        /// </summary>
        public string String
        {
            get
            {
                if (type == VarType.STRING)
                    return stringValue!;

                throw VarException.CreateTypeError(type);
            }
        }

        /// <summary>
        /// 对象
        /// </summary>
        public object Object
        {
            get
            {
                if (type == VarType.OBJECT)
                    return objectValue!;

                throw VarException.CreateTypeError(type);
            }
        }

        /// <summary>
        /// 列表
        /// </summary>
        public VarList List
        {
            get
            {
                if (type == VarType.VARLIST)
                    return listValue!;

                throw VarException.CreateTypeError(type);
            }
        }

        /// <summary>
        /// 字典
        /// </summary>
        public VarMap Map
        {
            get
            {
                if (type == VarType.VARMAP)
                    return mapValue!;

                throw VarException.CreateTypeError(type);
            }
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (type)
            {
                case VarType.STRING:
                    return stringValue!;
                case VarType.NULL:
                    return "null";
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
                case VarType.STRUCT:
                case VarType.OBJECT:
                    return objectValue?.ToString() ?? "null";
                case VarType.VARLIST:
                    return listValue!.ToString();
                case VarType.VARMAP:
                    return mapValue!.ToString();
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
        /// <param name="value"></param>
        /// <returns></returns>
        public int CompareTo(Var value)
        {
            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(
#if NETCOREAPP3_1_OR_GREATER
            [NotNullWhen(true)]
#endif
        object? obj)
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
            return HashCode.Combine(type, intValue, longValue, floatValue, stringValue, objectValue, listValue, mapValue);
#else
            return type.GetHashCode() ^ intValue.GetHashCode() ^ longValue.GetHashCode() ^ floatValue.GetHashCode() ^ doubleValue.GetHashCode() ^ stringValue?.GetHashCode() ?? 0 ^ objectValue?.GetHashCode() ?? 0 ^ listValue?.GetHashCode() ?? 0 ^ mapValue?.GetHashCode() ?? 0;
#endif
        }

        /// <summary>
        /// 变量类型
        /// </summary>
        public VarType Type => type;

        /// <summary>
        /// 是否为空类型
        /// </summary>
        public bool IsNull => type == VarType.NULL;

        /// <summary>
        /// 是否为数字类型
        /// </summary>
        public bool IsNumber => type == VarType.INT32 || type == VarType.INT64 || type == VarType.FLOAT || type == VarType.DOUBLE;

        /// <summary>
        /// 是否为字符串类型
        /// </summary>
        public bool IsString => type == VarType.STRING;

        /// <summary>
        /// 是否为空的字符串类型
        /// </summary>
        public bool IsEmptyString => type == VarType.STRING && string.IsNullOrEmpty(stringValue);

        /// <summary>
        /// 是否为列表
        /// </summary>
        public bool IsList => type == VarType.VARLIST;

        /// <summary>
        /// 是否为字典
        /// </summary>
        public bool IsMap => type == VarType.VARMAP;

        /// <summary>
        /// 转字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] bytes;
            switch (type)
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
                    byte[] listBytes = listValue!.GetBytesInternal();
                    int listlen = listBytes.Length;
                    bytes = new byte[1 + listlen];
                    bytes[0] = (byte)VarType.VARLIST;
                    Buffer.BlockCopy(listBytes, 0, bytes, 1, listlen);
                    break;
                case VarType.VARMAP:
                    byte[] mapBytes = mapValue!.GetBytesInternal();
                    int maplen = mapBytes.Length;
                    bytes = new byte[1 + maplen];
                    bytes[0] = (byte)VarType.VARMAP;
                    Buffer.BlockCopy(mapBytes, 0, bytes, 1, maplen);
                    break;
                case VarType.STRUCT:
                    if (objectValue == null) bytes = new byte[1] { (byte)VarType.NULL | 0xF0 };
                    else
                    {
                        string typeName = VarObjectMgr.RegisterObjectType(objectValue);
                        int typeNameSize = typeName.Length;
                        int size = Marshal.SizeOf(objectValue);
                        IntPtr bufferIntPtr = Marshal.AllocHGlobal(size);
                        try
                        {
                            Marshal.StructureToPtr(objectValue, bufferIntPtr, false);
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
                    VarList list = VarList.ParseInternal(value, startIndex + 1, out int listLen) ?? new VarList();
                    length = listLen + 1;
                    return list;
                case VarType.VARMAP:
                    VarMap map = VarMap.ParseInternal(value, startIndex + 1, out int mapLen) ?? new VarMap();
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
            switch (left.type)
            {
                case VarType.NULL: return Null;
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.NULL: return Null;
                        case VarType.INT32: return left.intValue + right.intValue;
                        case VarType.INT64: return left.intValue + right.longValue;
                        case VarType.FLOAT: return left.intValue + right.floatValue;
                        case VarType.DOUBLE: return left.intValue + right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.NULL: return Null;
                        case VarType.INT32: return left.longValue + right.intValue;
                        case VarType.INT64: return left.longValue + right.longValue;
                        case VarType.FLOAT: return left.longValue + right.floatValue;
                        case VarType.DOUBLE: return left.longValue + right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.type)
                    {
                        case VarType.NULL: return Null;
                        case VarType.INT32: return left.floatValue + right.intValue;
                        case VarType.INT64: return left.floatValue + right.longValue;
                        case VarType.FLOAT: return left.floatValue + right.floatValue;
                        case VarType.DOUBLE: return left.floatValue + right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.NULL: return Null;
                        case VarType.INT32: return left.doubleValue + right.intValue;
                        case VarType.INT64: return left.doubleValue + right.longValue;
                        case VarType.FLOAT: return left.doubleValue + right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue + right.doubleValue;
                    }
                    break;
                case VarType.VARLIST:
                    return left.listValue!.Add(right);
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
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue - right.intValue;
                        case VarType.INT64: return left.intValue - right.longValue;
                        case VarType.FLOAT: return left.intValue - right.floatValue;
                        case VarType.DOUBLE: return left.intValue - right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue - right.intValue;
                        case VarType.INT64: return left.longValue - right.longValue;
                        case VarType.FLOAT: return left.longValue - right.floatValue;
                        case VarType.DOUBLE: return left.longValue - right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.floatValue - right.intValue;
                        case VarType.INT64: return left.floatValue - right.longValue;
                        case VarType.FLOAT: return left.floatValue - right.floatValue;
                        case VarType.DOUBLE: return left.floatValue - right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.type)
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
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue * right.intValue;
                        case VarType.INT64: return left.intValue * right.longValue;
                        case VarType.FLOAT: return left.intValue * right.floatValue;
                        case VarType.DOUBLE: return left.intValue * right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue * right.intValue;
                        case VarType.INT64: return left.longValue * right.longValue;
                        case VarType.FLOAT: return left.longValue * right.floatValue;
                        case VarType.DOUBLE: return left.longValue * right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.floatValue * right.intValue;
                        case VarType.INT64: return left.floatValue * right.longValue;
                        case VarType.FLOAT: return left.floatValue * right.floatValue;
                        case VarType.DOUBLE: return left.floatValue * right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.type)
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
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue / right.intValue;
                        case VarType.INT64: return left.intValue / right.longValue;
                        case VarType.FLOAT: return left.intValue / right.floatValue;
                        case VarType.DOUBLE: return left.intValue / right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue / right.intValue;
                        case VarType.INT64: return left.longValue / right.longValue;
                        case VarType.FLOAT: return left.longValue / right.floatValue;
                        case VarType.DOUBLE: return left.longValue / right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.floatValue / right.intValue;
                        case VarType.INT64: return left.floatValue / right.longValue;
                        case VarType.FLOAT: return left.floatValue / right.floatValue;
                        case VarType.DOUBLE: return left.floatValue / right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.type)
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
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue % right.intValue;
                        case VarType.INT64: return left.intValue % right.longValue;
                        case VarType.FLOAT: return left.intValue % right.floatValue;
                        case VarType.DOUBLE: return left.intValue % right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue % right.intValue;
                        case VarType.INT64: return left.longValue % right.longValue;
                        case VarType.FLOAT: return left.longValue % right.floatValue;
                        case VarType.DOUBLE: return left.longValue % right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.floatValue % right.intValue;
                        case VarType.INT64: return left.floatValue % right.longValue;
                        case VarType.FLOAT: return left.floatValue % right.floatValue;
                        case VarType.DOUBLE: return left.floatValue % right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.type)
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
            switch (value.type)
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
            switch (value.type)
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
            switch (value.type)
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
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue & right.intValue;
                        case VarType.INT64: return left.intValue & right.longValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue & right.intValue;
                        case VarType.INT64: return left.longValue & right.longValue;
                    }
                    break;
            }

            if (left.type == VarType.BOOL && right.type == VarType.BOOL)
            {
                return left.boolValue && right.boolValue;
            }
            else if (left.type == VarType.BOOL)
            {
                return left.boolValue && !right.IsNull;
            }
            else if(right.type == VarType.BOOL)
            {
                return right.boolValue && !left.IsNull;
            }

            return !(left.IsNull && right.IsNull);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator |(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue | right.intValue;
                        case VarType.INT64: return (long)left.intValue | right.longValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue | (long)right.intValue;
                        case VarType.INT64: return left.longValue | right.longValue;
                    }
                    break;
            }

            if (left.type == VarType.BOOL && right.type == VarType.BOOL)
            {
                return left.boolValue || right.boolValue;
            }
            else if (left.type == VarType.BOOL)
            {
                return left.boolValue || !right.IsNull;
            }
            else if (right.type == VarType.BOOL)
            {
                return right.boolValue || !left.IsNull;
            }

            return !(left.IsNull || right.IsNull);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator ^(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue ^ right.intValue;
                        case VarType.INT64: return left.intValue ^ right.longValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue ^ right.intValue;
                        case VarType.INT64: return left.longValue ^ right.longValue;
                    }
                    break;
            }

            if (left.type == VarType.BOOL && right.type == VarType.BOOL)
            {
                return left.boolValue ^ right.boolValue;
            }
            else if (left.type == VarType.BOOL)
            {
                return left.boolValue ^ !right.IsNull;
            }
            else if (right.type == VarType.BOOL)
            {
                return right.boolValue ^ !left.IsNull;
            }

            return !(left.IsNull ^ right.IsNull);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left">可变变量</param>
        /// <param name="right">位移量</param>
        public static Var operator <<(Var left, int right)
        {
            switch (left.type)
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
            switch (left.type)
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
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue < right.intValue;
                        case VarType.INT64: return left.intValue < right.longValue;
                        case VarType.FLOAT: return left.intValue < right.floatValue;
                        case VarType.DOUBLE: return left.intValue < right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
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
                    switch (right.type)
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
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue > right.intValue;
                        case VarType.INT64: return left.intValue > right.longValue;
                        case VarType.FLOAT: return left.intValue > right.floatValue;
                        case VarType.DOUBLE: return left.intValue > right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue > right.intValue;
                        case VarType.INT64: return left.longValue > right.longValue;
                        case VarType.FLOAT: return left.longValue > right.floatValue;
                        case VarType.DOUBLE: return left.longValue > right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.floatValue > right.intValue;
                        case VarType.INT64: return left.floatValue > right.longValue;
                        case VarType.FLOAT: return left.floatValue > right.floatValue;
                        case VarType.DOUBLE: return left.floatValue > right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.type)
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
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue <= right.intValue;
                        case VarType.INT64: return left.intValue <= right.longValue;
                        case VarType.FLOAT: return left.intValue <= right.floatValue;
                        case VarType.DOUBLE: return left.intValue <= right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue <= right.intValue;
                        case VarType.INT64: return left.longValue <= right.longValue;
                        case VarType.FLOAT: return left.longValue <= right.floatValue;
                        case VarType.DOUBLE: return left.longValue <= right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.floatValue <= right.intValue;
                        case VarType.INT64: return left.floatValue <= right.longValue;
                        case VarType.FLOAT: return left.floatValue <= right.floatValue;
                        case VarType.DOUBLE: return left.floatValue <= right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.type)
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
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue >= right.intValue;
                        case VarType.INT64: return left.intValue >= right.longValue;
                        case VarType.FLOAT: return left.intValue >= right.floatValue;
                        case VarType.DOUBLE: return left.intValue >= right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue >= right.intValue;
                        case VarType.INT64: return left.longValue >= right.longValue;
                        case VarType.FLOAT: return left.longValue >= right.floatValue;
                        case VarType.DOUBLE: return left.longValue >= right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.floatValue >= right.intValue;
                        case VarType.INT64: return left.floatValue >= right.longValue;
                        case VarType.FLOAT: return left.floatValue >= right.floatValue;
                        case VarType.DOUBLE: return left.floatValue >= right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.type)
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
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue == right.intValue;
                        case VarType.INT64: return left.intValue == right.longValue;
                        case VarType.FLOAT: return left.intValue == right.floatValue;
                        case VarType.DOUBLE: return left.intValue == right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue == right.intValue;
                        case VarType.INT64: return left.longValue == right.longValue;
                        case VarType.FLOAT: return left.longValue == right.floatValue;
                        case VarType.DOUBLE: return left.longValue == right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.floatValue == right.intValue;
                        case VarType.INT64: return left.floatValue == right.longValue;
                        case VarType.FLOAT: return left.floatValue == right.floatValue;
                        case VarType.DOUBLE: return left.floatValue == right.doubleValue;
                    }
                    break;
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.doubleValue == right.intValue;
                        case VarType.INT64: return left.doubleValue == right.longValue;
                        case VarType.FLOAT: return left.doubleValue == right.floatValue;
                        case VarType.DOUBLE: return left.doubleValue == right.doubleValue;
                    }
                    break;
                case VarType.BOOL:
                    if (right.type == VarType.BOOL)
                        return left.boolValue == right.boolValue;
                    else
                        return false;
                case VarType.STRING:
                    if (right.type == VarType.STRING)
                        return left.stringValue == right.stringValue;
                    else
                        return false;
                case VarType.STRUCT:
                    if (right.type == VarType.STRUCT)
                        return left.objectValue == right.objectValue;
                    else
                        return false;
                case VarType.OBJECT:
                    if (right.type == VarType.OBJECT)
                        return left.objectValue == right.objectValue;
                    else
                        return false;
                case VarType.VARLIST:
                    if (right.type == VarType.VARLIST)
                        return left.listValue == right.listValue;
                    else
                        return false;
                case VarType.VARMAP:
                    if (right.type == VarType.VARMAP)
                        return left.mapValue == right.mapValue;
                    else
                        return false;
                case VarType.NULL:
                    if (right.type == VarType.NULL)
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

        /// <summary>
        /// 
        /// </summary>
        public static bool operator true(Var v)
        {
            return (bool)v;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool operator false(Var v)
        {
            return !(bool)v;
        }

        /// <summary>
        /// 根据键名安全获取键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Var this[Var key]
        {
            get
            {
                if (type == VarType.VARMAP)
                    return mapValue![key];
                else if (type == VarType.VARLIST)
                    return listValue![key];
                else
                    return Null;
            }
            set
            {
                if (type == VarType.VARMAP)
                    mapValue![key] = value;
                else if (type == VarType.VARLIST)
                    listValue![key] = value;
                else
                    throw VarException.CreateTypeError(type);
            }
        }

        /// <summary>
        /// 设置一个值变化监听，当列表被修改就会触发监听
        /// </summary>
        /// <param name="changeListener"></param>
        public void SetChangeListener(Action<int, Var> changeListener)
        {
            if (type == VarType.VARLIST)
                listValue!.SetChangeListener(changeListener);
            else
                throw VarException.CreateTypeError(type);
        }

        /// <summary>
        /// 设置一个值变化监听，当字典被修改就会触发监听
        /// </summary>
        /// <param name="changeListener"></param>
        public void SetChangeListener(Action<Var, Var> changeListener)
        {
            if (type == VarType.VARMAP)
                mapValue!.SetChangeListener(changeListener);
            else
                throw VarException.CreateTypeError(type);
        }

        /// <summary>
        /// 合并可变变量字典
        /// <para>合并字典中已存在的则覆盖最新值</para>
        /// </summary>
        /// <param name="varmap"></param>
        /// <returns></returns>
        public Var Merge(VarMap varmap)
        {
            return mapValue!.Merge(varmap);
        }

        /// <summary>
        /// 列表插入一个值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Var Insert(int index, Var value)
        {
            return listValue!.Insert(index, value);
        }

        /// <summary>
        /// 列表插入一组值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Var InsertRange(int index, IEnumerable<Var> value)
        {
            return listValue!.InsertRange(index, value);
        }

        /// <summary>
        /// 增加一个可变变量对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Var Add(Var key, Var value)
        {
            return mapValue!.Add(key, value);
        }

        /// <summary>
        /// 添加或者更新
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Var AddOrUpdate(Var key, Var value)
        {
            return mapValue!.AddOrUpdate(key, value);
        }

#if NETCOREAPP3_1_OR_GREATER
        /// <summary>
        /// 尝试添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryAdd(Var key, Var value)
        {
            return mapValue!.TryAdd(key, value);
        }
#endif

        /// <summary>
        /// 转Json对象
        /// </summary>
        /// <returns></returns>
        public JObject ToJObject()
        {
            return mapValue!.ToJObject();
        }

        /// <summary>
        /// 合并可变变量列表
        /// </summary>
        /// <param name="varlist"></param>
        /// <returns></returns>
        public Var Merge(VarList varlist)
        {
            return listValue!.Merge(varlist);
        }

        /// <summary>
        /// 增加一个可变变量
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Var Add(Var value)
        {
            return listValue!.Add(value);
        }

        /// <summary>
        /// 增加多个可变变量
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Var MultiAdd(params Var[] value)
        {
            return listValue!.AddRange(value);
        }

        /// <summary>
        /// 增加多个可变变量
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Var AddRange(IEnumerable<Var> value)
        {
            return listValue!.AddRange(value);
        }

        /// <summary>
        /// 转json对象
        /// </summary>
        /// <returns></returns>
        public JArray ToJArray()
        {
            return listValue!.ToJArray();
        }

        /// <summary>
        /// 列表或者字典是否包含关键字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(Var key)
        {
            if (type == VarType.VARMAP)
                return mapValue!.ContainsKey(key);
            else if (type == VarType.VARLIST)
                return listValue!.Contains(key);
            else
                throw VarException.CreateTypeError(type);
        }

        /// <summary>
        /// 尝试从字典获取值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(Var key, out Var value)
        {
            return mapValue!.TryGetValue(key, out value);
        }

        /// <summary>
        /// 字符串或列表或字典的长度
        /// </summary>
        public int Count
        {
            get
            {
                if (type == VarType.VARMAP)
                    return mapValue!.Count;
                else if (type == VarType.VARLIST)
                    return listValue!.Count;
                else if (type == VarType.STRING)
                    return stringValue!.Length;
                else
                    throw VarException.CreateTypeError(type);
            }
        }

        /// <summary>
        /// 列表移除索引位节点
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (type == VarType.VARLIST)
                listValue!.RemoveAt(index);
            else
                throw VarException.CreateTypeError(type);
        }

        /// <summary>
        /// 从列表或者字典中移除对象或者关键字
        /// </summary>
        /// <param name="itemOrKey"></param>
        public bool Remove(Var itemOrKey)
        {
            if (type == VarType.VARMAP)
                return mapValue!.Remove(itemOrKey);
            else if (type == VarType.VARLIST)
                return listValue!.Remove(itemOrKey);
            else
                throw VarException.CreateTypeError(type);
        }

        /// <summary>
        /// 清空列表或者字典
        /// </summary>
        public void Clear()
        {
            if (type == VarType.VARMAP)
                mapValue!.Clear();
            else if (type == VarType.VARLIST)
                listValue!.Clear();
            else
                throw VarException.CreateTypeError(type);
        }
    }
}
