using ES.Utils;
using System;
using System.Collections.Generic;

namespace ES.Variant
{
    /// <summary>
    /// 可变变量
    /// <para>支持Object、VarList、VarMap和所有基础类型</para>
    /// </summary>
    public readonly partial struct Var
    {
        /// <summary>
        /// 空值
        /// </summary>
        public static readonly Var Empty = new Var();

#if !UNITY_2020_1_OR_NEWER
        private readonly VarType type = VarType.NULL;

        private readonly int intValue = 0;
        private readonly long longValue = 0L;
        private readonly double doubleValue = 0.0D;
        private readonly string stringValue = "";
        private readonly object? objectValue = null;
        private readonly VarList? listValue = null;
        private readonly VarMap? mapValue = null;
#else
        private readonly VarType type;

        private readonly int intValue;
        private readonly long longValue;
        private readonly double doubleValue;
        private readonly string stringValue;
        private readonly object? objectValue;
        private readonly VarList? listValue;
        private readonly VarMap? mapValue;
#endif

        /// <summary>
        /// 变量类型
        /// </summary>
        public readonly VarType Type => type;

#if !UNITY_2020_1_OR_NEWER
        /// <summary>
        /// 空变量
        /// </summary>
        public Var() { type = VarType.NULL; }
#endif

        /// <summary>
        /// 对象变量
        /// </summary>
        public Var(object value)
        {
            type = VarType.OBJECT;
            objectValue = value;
#if UNITY_2017_1_OR_NEWER
            intValue = 0;
            longValue = 0L;
            doubleValue = 0.0D;
            stringValue = "";
            listValue = null;
            mapValue = null;
#endif
        }

        /// <summary>
        /// 整型可变变量
        /// </summary>
        private Var(int value)
        {
            type = VarType.INT32;
            intValue = value;
#if UNITY_2017_1_OR_NEWER
            longValue = 0L;
            doubleValue = 0.0D;
            stringValue = "";
            objectValue = null;
            listValue = null;
            mapValue = null;
#endif
        }

        /// <summary>
        /// 无符号整型可变变量
        /// </summary>
        private Var(uint value)
        {
            type = VarType.UINT32;
            intValue = (int)value;
#if UNITY_2017_1_OR_NEWER
            longValue = 0L;
            doubleValue = 0.0D;
            stringValue = "";
            objectValue = null;
            listValue = null;
            mapValue = null;
#endif
        }

        /// <summary>
        /// 长整型可变变量
        /// </summary>
        private Var(long value)
        {
            type = VarType.INT64;
            longValue = value;
#if UNITY_2017_1_OR_NEWER
            intValue = 0;
            doubleValue = 0.0D;
            stringValue = "";
            objectValue = null;
            listValue = null;
            mapValue = null;
#endif
        }

        /// <summary>
        /// 无符号长整型可变变量
        /// </summary>
        private Var(ulong value)
        {
            type = VarType.UINT64;
            longValue = (long)value;
#if UNITY_2017_1_OR_NEWER
            intValue = 0;
            doubleValue = 0.0D;
            stringValue = "";
            objectValue = null;
            listValue = null;
            mapValue = null;
#endif
        }

        /// <summary>
        /// 单精度浮点型可变变量
        /// </summary>
        private Var(float value)
        {
            type = VarType.FLOAT;
            doubleValue = value;
#if UNITY_2017_1_OR_NEWER
            intValue = 0;
            longValue = 0L;
            stringValue = "";
            objectValue = null;
            listValue = null;
            mapValue = null;
#endif
        }

        /// <summary>
        /// 双精度浮点型可变变量
        /// </summary>
        private Var(double value)
        {
            type = VarType.DOUBLE;
            doubleValue = value;
#if UNITY_2017_1_OR_NEWER
            intValue = 0;
            longValue = 0L;
            stringValue = "";
            objectValue = null;
            listValue = null;
            mapValue = null;
#endif
        }

        /// <summary>
        /// 布尔型可变变量
        /// </summary>
        private Var(bool value)
        {
            type = VarType.BOOL;
            intValue = value ? 1 : 0;
#if UNITY_2017_1_OR_NEWER
            longValue = 0L;
            doubleValue = 0.0D;
            stringValue = "";
            objectValue = null;
            listValue = null;
            mapValue = null;
#endif
        }

        /// <summary>
        /// 可变变量
        /// </summary>
        private Var(string value)
        {
            type = VarType.STRING;
            stringValue = value;
#if UNITY_2017_1_OR_NEWER
            intValue = 0;
            longValue = 0L;
            doubleValue = 0.0D;
            objectValue = null;
            listValue = null;
            mapValue = null;
#endif
        }

        /// <summary>
        /// 可变变量
        /// </summary>
        private Var(VarList value)
        {
            type = VarType.VARLIST;
            listValue = value;
#if UNITY_2017_1_OR_NEWER
            intValue = 0;
            longValue = 0L;
            doubleValue = 0.0D;
            stringValue = "";
            objectValue = null;
            mapValue = null;
#endif
        }

        /// <summary>
        /// 可变变量
        /// </summary>
        private Var(VarMap value)
        {
            type = VarType.VARMAP;
            mapValue = value;
#if UNITY_2017_1_OR_NEWER
            intValue = 0;
            longValue = 0L;
            doubleValue = 0.0D;
            stringValue = "";
            objectValue = null;
            listValue = null;
#endif
        }

        /// <summary>
        /// 对象
        /// </summary>
        public object Object => objectValue ?? new object();
        /// <summary>
        /// 列表
        /// </summary>
        public VarList List => listValue ?? new VarList();
        /// <summary>
        /// 字典
        /// </summary>
        public VarMap Map => mapValue ?? new VarMap();

        /// <summary>
        /// 枚举值变量
        /// </summary>
        /// <param name="value">字节型变量</param>
        public static implicit operator Var(Enum value)
            => new Var(Convert.ToInt32(value));
        /// <summary>
        /// 字节型变量
        /// </summary>
        /// <param name="value">字节型变量</param>
        public static implicit operator Var(byte value)
            => new Var(value);
        /// <summary>
        /// 有符号字节型变量
        /// </summary>
        /// <param name="value">有符号字节型变量</param>
        public static implicit operator Var(sbyte value)
            => new Var(value);
        /// <summary>
        /// 短整型变量
        /// </summary>
        /// <param name="value">短整型变量</param>
        public static implicit operator Var(short value)
            => new Var(value);
        /// <summary>
        /// 无符号短整型变量
        /// </summary>
        /// <param name="value">无符号短整型变量</param>
        public static implicit operator Var(ushort value)
            => new Var(value);
        /// <summary>
        /// 整型变量
        /// </summary>
        /// <param name="value">整型变量</param>
        public static implicit operator Var(int value)
            => new Var(value);
        /// <summary>
        /// 无符号整型变量
        /// </summary>
        /// <param name="value">无符号整型变量</param>
        public static implicit operator Var(uint value)
            => new Var(value);
        /// <summary>
        /// 长整型变量
        /// </summary>
        /// <param name="value">长整型变量</param>
        public static implicit operator Var(long value)
            => new Var(value);
        /// <summary>
        /// 无符号长整型变量
        /// </summary>
        /// <param name="value">无符号长整型变量</param>
        public static implicit operator Var(ulong value)
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
        /// 通过字节数组转可变变量
        /// </summary>
        /// <param name="value">可变变量的字节数组</param>
        public static implicit operator Var(byte[] value)
        {
            return Parse(value, out _);
        }

        /// <summary>
        /// 字节型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator byte(Var value)
        {
            switch (value.type)
            {
                case VarType.BOOL:
                case VarType.INT32:
                case VarType.UINT32:
                    return (byte)value.intValue;
                case VarType.INT64:
                case VarType.UINT64:
                    return (byte)value.longValue;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    return (byte)value.doubleValue;
                default:
                    throw new Exception($"Var Use [{value.type}] Error Type!");
            }
        }
        /// <summary>
        /// 有符号字节型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator sbyte(Var value)
        {
            switch (value.type)
            {
                case VarType.BOOL:
                case VarType.INT32:
                case VarType.UINT32:
                    return (sbyte)value.intValue;
                case VarType.INT64:
                case VarType.UINT64:
                    return (sbyte)value.longValue;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    return (sbyte)value.doubleValue;
                default:
                    throw new Exception($"Var Use [{value.type}] Error Type!");
            }
        }
        /// <summary>
        /// 短整型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator short(Var value)
        {
            switch (value.type)
            {
                case VarType.BOOL:
                case VarType.INT32:
                case VarType.UINT32:
                    return (short)value.intValue;
                case VarType.INT64:
                case VarType.UINT64:
                    return (short)value.longValue;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    return (short)value.doubleValue;
                default:
                    throw new Exception($"Var Use [{value.type}] Error Type!");
            }
        }
        /// <summary>
        /// 无符号短整型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator ushort(Var value)
        {
            switch (value.type)
            {
                case VarType.BOOL:
                case VarType.INT32:
                case VarType.UINT32:
                    return (ushort)value.intValue;
                case VarType.INT64:
                case VarType.UINT64:
                    return (ushort)value.longValue;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    return (ushort)value.doubleValue;
                default:
                    throw new Exception($"Var Use [{value.type}] Error Type!");
            }
        }
        /// <summary>
        /// 整型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator int(Var value)
        {
            switch (value.type)
            {
                case VarType.BOOL:
                case VarType.INT32:
                case VarType.UINT32:
                    return value.intValue;
                case VarType.INT64:
                case VarType.UINT64:
                    return (int)value.longValue;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    return (int)value.doubleValue;
                default:
                    throw new Exception($"Var Use [{value.type}] Error Type!");
            }
        }
        /// <summary>
        /// 无符号整型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator uint(Var value)
        {
            switch (value.type)
            {
                case VarType.BOOL:
                case VarType.INT32:
                case VarType.UINT32:
                    return (uint)value.intValue;
                case VarType.INT64:
                case VarType.UINT64:
                    return (uint)value.longValue;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    return (uint)value.doubleValue;
                default:
                    throw new Exception($"Var Use [{value.type}] Error Type!");
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
                case VarType.BOOL:
                case VarType.INT32:
                case VarType.UINT32:
                    return value.intValue;
                case VarType.INT64:
                case VarType.UINT64:
                    return value.longValue;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    return (long)value.doubleValue;
                default:
                    throw new Exception($"Var Use [{value.type}] Error Type!");
            }
        }
        /// <summary>
        /// 无符号长整型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator ulong(Var value)
        {
            switch (value.type)
            {
                case VarType.BOOL:
                case VarType.INT32:
                case VarType.UINT32:
                    return (ulong)value.intValue;
                case VarType.INT64:
                case VarType.UINT64:
                    return (ulong)value.longValue;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    return (ulong)value.doubleValue;
                default:
                    throw new Exception($"Var Use [{value.type}] Error Type!");
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
                case VarType.BOOL:
                case VarType.INT32:
                case VarType.UINT32:
                    return value.intValue;
                case VarType.INT64:
                case VarType.UINT64:
                    return value.longValue;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    return (float)value.doubleValue;
                default:
                    throw new Exception($"Var Use [{value.type}] Error Type!");
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
                case VarType.BOOL:
                case VarType.INT32:
                case VarType.UINT32:
                    return value.intValue;
                case VarType.INT64:
                case VarType.UINT64:
                    return value.longValue;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    return value.doubleValue;
                default:
                    throw new Exception($"Var Use [{value.type}] Error Type!");
            }
        }
        /// <summary>
        /// 布尔型变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator bool(Var value)
        {
            return value.intValue > 0 || value.longValue > 0 || value.doubleValue > 0;
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
            switch (type)
            {
                case VarType.INT32:
                    return intValue.ToString();
                case VarType.UINT32:
                    return ((uint)intValue).ToString();
                case VarType.INT64:
                    return longValue.ToString();
                case VarType.UINT64:
                    return ((ulong)longValue).ToString();
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    return doubleValue.ToString();
                case VarType.BOOL:
                    return (intValue == 1).ToString();
                case VarType.STRING:
                    return stringValue;
                case VarType.OBJECT:
                    return objectValue?.ToString() ?? "";
                case VarType.VARLIST:
                    return listValue?.ToString() ?? "";
                case VarType.VARMAP:
                    return mapValue?.ToString() ?? "";
                default:
                    return "";
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
            return obj is Var var &&
                   type == var.type &&
                   intValue == var.intValue &&
                   longValue == var.longValue &&
                   doubleValue == var.doubleValue &&
                   stringValue == var.stringValue &&
                   EqualityComparer<object>.Default.Equals(objectValue, var.objectValue) &&
                   EqualityComparer<VarList>.Default.Equals(listValue, var.listValue) &&
                   EqualityComparer<VarMap>.Default.Equals(mapValue, var.mapValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
#if !UNITY_2020_1_OR_NEWER
            return HashCode.Combine(type, intValue, longValue, doubleValue, stringValue, objectValue, listValue, mapValue);
#else 
			return type.GetHashCode() ^ intValue.GetHashCode() ^ longValue.GetHashCode() ^ doubleValue.GetHashCode() ^ stringValue.GetHashCode() ^ objectValue.GetHashCode() ^ listValue.GetHashCode() ^ mapValue.GetHashCode();
#endif
		}

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
                        Buffer.BlockCopy(ByteHelper.GetBytes((short)intValue), 0, bytes, 1, 2);
                    }
                    else if (ushort.MinValue <= intValue && intValue <= ushort.MaxValue)
                    {
                        bytes = new byte[3];
                        bytes[0] = (byte)VarType.UINT16;
                        Buffer.BlockCopy(ByteHelper.GetBytes((ushort)intValue), 0, bytes, 1, 2);
                    }
                    else
                    {
                        bytes = new byte[5];
                        bytes[0] = (byte)VarType.INT32;
                        Buffer.BlockCopy(ByteHelper.GetBytes(intValue), 0, bytes, 1, 4);
                    }
                    break;
                case VarType.UINT32:
                    uint uintValue = (uint)intValue;
                    if (byte.MinValue <= uintValue && uintValue <= byte.MaxValue)
                    { bytes = new byte[2] { (byte)VarType.BYTE, (byte)uintValue }; }
                    else if (ushort.MinValue <= uintValue && uintValue <= ushort.MaxValue)
                    {
                        bytes = new byte[3];
                        bytes[0] = (byte)VarType.UINT16;
                        Buffer.BlockCopy(ByteHelper.GetBytes((ushort)uintValue), 0, bytes, 1, 2);
                    }
                    else
                    {
                        bytes = new byte[5];
                        bytes[0] = (byte)VarType.UINT32;
                        Buffer.BlockCopy(ByteHelper.GetBytes(uintValue), 0, bytes, 1, 4);
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
                        Buffer.BlockCopy(ByteHelper.GetBytes((short)longValue), 0, bytes, 1, 2);
                    }
                    else if (ushort.MinValue <= longValue && longValue <= ushort.MaxValue)
                    {
                        bytes = new byte[3];
                        bytes[0] = (byte)VarType.UINT16;
                        Buffer.BlockCopy(ByteHelper.GetBytes((ushort)longValue), 0, bytes, 1, 2);
                    }
                    else if (int.MinValue <= longValue && longValue <= int.MaxValue)
                    {
                        bytes = new byte[5];
                        bytes[0] = (byte)VarType.INT32;
                        Buffer.BlockCopy(ByteHelper.GetBytes((int)longValue), 0, bytes, 1, 4);
                    }
                    else if (uint.MinValue <= longValue && longValue <= uint.MaxValue)
                    {
                        bytes = new byte[5];
                        bytes[0] = (byte)VarType.UINT32;
                        Buffer.BlockCopy(ByteHelper.GetBytes((uint)longValue), 0, bytes, 1, 4);
                    }
                    else
                    {
                        bytes = new byte[9];
                        bytes[0] = (byte)VarType.INT64;
                        Buffer.BlockCopy(ByteHelper.GetBytes(longValue), 0, bytes, 1, 8);
                    }
                    break;
                case VarType.UINT64:
                    ulong ulongValue = (ulong)longValue;
                    if (byte.MinValue <= ulongValue && ulongValue <= byte.MaxValue)
                    { bytes = new byte[2] { (byte)VarType.BYTE, (byte)ulongValue }; }
                    else if (ushort.MinValue <= ulongValue && ulongValue <= ushort.MaxValue)
                    {
                        bytes = new byte[3];
                        bytes[0] = (byte)VarType.UINT16;
                        Buffer.BlockCopy(ByteHelper.GetBytes((ushort)ulongValue), 0, bytes, 1, 2);
                    }
                    else if (uint.MinValue <= ulongValue && ulongValue <= uint.MaxValue)
                    {
                        bytes = new byte[5];
                        bytes[0] = (byte)VarType.UINT32;
                        Buffer.BlockCopy(ByteHelper.GetBytes((uint)ulongValue), 0, bytes, 1, 4);
                    }
                    else
                    {
                        bytes = new byte[9];
                        bytes[0] = (byte)VarType.UINT64;
                        Buffer.BlockCopy(ByteHelper.GetBytes(ulongValue), 0, bytes, 1, 8);
                    }
                    break;
                case VarType.FLOAT:
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
                            Buffer.BlockCopy(ByteHelper.GetBytes((short)longVal), 0, bytes, 1, 2);
                        }
                        else if (ushort.MinValue <= longVal && longVal <= ushort.MaxValue)
                        {
                            bytes = new byte[3];
                            bytes[0] = (byte)VarType.UINT16;
                            Buffer.BlockCopy(ByteHelper.GetBytes((ushort)longVal), 0, bytes, 1, 2);
                        }
                        else if (int.MinValue <= longVal && longVal <= int.MaxValue)
                        {
                            bytes = new byte[5];
                            bytes[0] = (byte)VarType.INT32;
                            Buffer.BlockCopy(ByteHelper.GetBytes((int)longVal), 0, bytes, 1, 4);
                        }
                        else if (uint.MinValue <= longVal && longVal <= uint.MaxValue)
                        {
                            bytes = new byte[5];
                            bytes[0] = (byte)VarType.UINT32;
                            Buffer.BlockCopy(ByteHelper.GetBytes((uint)longVal), 0, bytes, 1, 4);
                        }
                        else
                        {
                            bytes = new byte[9];
                            bytes[0] = (byte)VarType.INT64;
                            Buffer.BlockCopy(ByteHelper.GetBytes(longVal), 0, bytes, 1, 8);
                        }
                    }
                    else
                    {
                        if (type == VarType.FLOAT)
                        {
                            bytes = new byte[5];
                            bytes[0] = (byte)VarType.FLOAT;
                            Buffer.BlockCopy(ByteHelper.GetBytes((float)doubleValue), 0, bytes, 1, 4);
                        }
                        else
                        {
                            bytes = new byte[9];
                            bytes[0] = (byte)VarType.DOUBLE;
                            Buffer.BlockCopy(ByteHelper.GetBytes(doubleValue), 0, bytes, 1, 8);
                        }
                    }
                    break;
                case VarType.BOOL:
                    bytes = new byte[1] { (byte)((byte)VarType.BOOL | (intValue == 1 ? 0x10 : 0x00)) };
                    break;
                case VarType.STRING:
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
                    byte[] listBytes = (listValue ?? new VarList()).GetBytes();
                    int listlen = listBytes.Length;
                    bytes = new byte[1 + listlen];
                    bytes[0] = (byte)VarType.VARLIST;
                    Buffer.BlockCopy(listBytes, 0, bytes, 1, listlen);
                    break;
                case VarType.VARMAP:
                    byte[] mapBytes = (mapValue ?? new VarMap()).GetBytes();
                    int maplen = mapBytes.Length;
                    bytes = new byte[1 + maplen];
                    bytes[0] = (byte)VarType.VARMAP;
                    Buffer.BlockCopy(mapBytes, 0, bytes, 1, maplen);
                    break;
                case VarType.NULL:
                    bytes = new byte[1] { (byte)VarType.NULL | 0xF0 };
                    break;
                default:
                    bytes = ByteHelper.Empty;
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
                return Empty;
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
                    return ByteHelper.ToInt16(value, startIndex + 1);
                case VarType.UINT16:
                    length = 3;
                    return ByteHelper.ToUInt16(value, startIndex + 1);
                case VarType.INT32:
                    length = 5;
                    return ByteHelper.ToInt32(value, startIndex + 1);
                case VarType.UINT32:
                    length = 5;
                    return ByteHelper.ToUInt32(value, startIndex + 1);
                case VarType.INT64:
                    length = 9;
                    return ByteHelper.ToInt64(value, startIndex + 1);
                case VarType.UINT64:
                    length = 9;
                    return ByteHelper.ToUInt64(value, startIndex + 1);
                case VarType.FLOAT:
                    length = 5;
                    return ByteHelper.ToSingle(value, startIndex + 1);
                case VarType.DOUBLE:
                    length = 9;
                    return ByteHelper.ToDouble(value, startIndex + 1);
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
                case VarType.NULL:
                    length = ((bytesSpan[startIndex] & 0xF0) == 0xF0) ? 1 : 0;
                    return Empty;
                default:
                    length = 0;
                    return Empty;
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
    }
}
