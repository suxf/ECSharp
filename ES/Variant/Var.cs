using ES.Common.Utils;
using System;
using System.Collections.Generic;

namespace ES.Variant
{
    /// <summary>
    /// 可变变量
    /// <para>变量类型受第一次调用 [设置 | 取得] 变量值时的类型限定</para>
    /// <para>支持的数据位数:</para>
    /// <para>整型；长整型；单精度浮点型；布尔型；字符型；对象型(引用)</para>
    /// </summary>
    public struct Var
    {
        /// <summary>
        /// 空值
        /// </summary>
        public static readonly Var Empty = new Var();
        /// <summary>
        /// 变量类型
        /// </summary>
        public VarType Type { private set; get; } = VarType.UNKNOWN;

        private int intValue = 0;
        private long longValue = 0L;
        private float floatValue = 0F;
        private string stringValue = "";
        private object? objectValue = null;

        private VarList? listValue = null;
        private VarMap? mapValue = null;

        /// <summary>
        /// 对象
        /// </summary>
        public object Object { get { CheckType(VarType.OBJECT); return objectValue!; } }
        /// <summary>
        /// 列表
        /// </summary>
        public VarList List { get { CheckType(VarType.VARLIST); return listValue!; } }
        /// <summary>
        /// 字典
        /// </summary>
        public VarMap Map { get { CheckType(VarType.VARMAP); return mapValue!; } }

        /// <summary>
        /// 字节型可变变量
        /// </summary>
        /// <param name="value">字节型变量</param>
        public static implicit operator Var(byte value)
            => new Var() { Type = VarType.INT32, intValue = value };
        /// <summary>
        /// 有符号字节型可变变量
        /// </summary>
        /// <param name="value">有符号字节型变量</param>
        public static implicit operator Var(sbyte value)
            => new Var() { Type = VarType.INT32, intValue = value };
        /// <summary>
        /// 短整型可变变量
        /// </summary>
        /// <param name="value">短整型变量</param>
        public static implicit operator Var(short value)
            => new Var() { Type = VarType.INT32, intValue = value };
        /// <summary>
        /// 无符号短整型可变变量
        /// </summary>
        /// <param name="value">无符号短整型变量</param>
        public static implicit operator Var(ushort value)
            => new Var() { Type = VarType.INT32, intValue = value };
        /// <summary>
        /// 整型可变变量
        /// </summary>
        /// <param name="value">整型变量</param>
        public static implicit operator Var(int value)
            => new Var() { Type = VarType.INT32, intValue = value };
        /// <summary>
        /// 无符号整型可变变量
        /// </summary>
        /// <param name="value">无符号整型变量</param>
        public static implicit operator Var(uint value)
            => new Var() { Type = VarType.UINT32, intValue = (int)value };
        /// <summary>
        /// 长整型可变变量
        /// </summary>
        /// <param name="value">长整型变量</param>
        public static implicit operator Var(long value)
            => new Var() { Type = VarType.INT64, longValue = value };
        /// <summary>
        /// 无符号长整型可变变量
        /// </summary>
        /// <param name="value">无符号长整型变量</param>
        public static implicit operator Var(ulong value)
            => new Var() { Type = VarType.UINT64, longValue = (long)value };
        /// <summary>
        /// 单精度浮点型可变变量
        /// </summary>
        /// <param name="value">单精度浮点型变量</param>
        public static implicit operator Var(float value)
            => new Var() { Type = VarType.FLOAT, floatValue = value };
        /// <summary>
        /// 双精度浮点型转单精度浮点型可变变量
        /// <para>不支持双精度浮点型，会丢失精度转为单精度浮点型</para>
        /// </summary>
        /// <param name="value">双精度浮点型变量</param>
        public static implicit operator Var(double value)
            => new Var() { Type = VarType.FLOAT, floatValue = (float)value };
        /// <summary>
        /// 布尔型可变变量
        /// </summary>
        /// <param name="value">布尔型变量</param>
        public static implicit operator Var(bool value)
            => new Var() { Type = VarType.BOOL, intValue = value ? 1 : 0 };
        /// <summary>
        /// 字符串型可变变量
        /// </summary>
        /// <param name="value">字符串型变量</param>
        public static implicit operator Var(string value)
            => new Var() { Type = VarType.STRING, stringValue = value };
        /// <summary>
        /// 通过字节数组转可变变量
        /// </summary>
        /// <param name="value">可变变量的字节数组</param>
        public static implicit operator Var(byte[] value)
        {
            return Parse(value, out _);
        }

        /// <summary>
        /// 可变变量类型
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator VarType(Var value)
        {
            return value.Type;
        }
        /// <summary>
        /// 整型可变变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator int(Var value)
        {
            value.CheckType(VarType.INT32);
            return value.intValue;
        }
        /// <summary>
        /// 无符号整型可变变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator uint(Var value)
        {
            value.CheckType(VarType.UINT32);
            return (uint)value.intValue;
        }
        /// <summary>
        /// 长整型可变变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator long(Var value)
        {
            switch (value.Type)
            {
                case VarType.INT32:
                case VarType.UINT32:
                    return value.intValue;
                case VarType.INT64:
                    return value.longValue;
                default:
                    value.CheckType(VarType.INT64);
                    return 0;
            }
        }
        /// <summary>
        /// 无符号长整型可变变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator ulong(Var value)
        {
            switch (value.Type)
            {
                case VarType.UINT32:
                    return (ulong)value.intValue;
                case VarType.UINT64:
                    return (ulong)value.longValue;
                default:
                    value.CheckType(VarType.UINT64);
                    return 0;
            }
        }
        /// <summary>
        /// 单精度浮点型可变变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator float(Var value)
        {
            switch (value.Type)
            {
                case VarType.INT32:
                case VarType.UINT32:
                    return value.intValue;
                case VarType.INT64:
                case VarType.UINT64:
                    return value.longValue;
                case VarType.FLOAT:
                    return value.floatValue;
                default:
                    value.CheckType(VarType.FLOAT);
                    return 0;
            }
        }
        /// <summary>
        /// 单精度浮点型强转双精度浮点型可变变量
        /// <para>满足同双精度浮点型计算要求，实际只有单精度浮点型的精度</para>
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator double(Var value)
        {
            return (float)value;
        }
        /// <summary>
        /// 布尔型可变变量
        /// </summary>
        /// <param name="value">可变变量</param>
        public static implicit operator bool(Var value)
        {
            value.CheckType(VarType.BOOL);
            return value.intValue == 1;
        }
        /// <summary>
        /// 字符串型可变变量
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
                case VarType.UINT32:
                    return ((uint)intValue).ToString();
                case VarType.INT64:
                    return longValue.ToString();
                case VarType.UINT64:
                    return ((ulong)longValue).ToString();
                case VarType.FLOAT:
                    return floatValue.ToString();
                case VarType.BOOL:
                    return (intValue == 1).ToString();
                case VarType.STRING:
                    return stringValue;
                case VarType.OBJECT:
                    return objectValue?.ToString() ?? "";
                default:
                    CheckType(VarType.STRING);
                    return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value1">可变变量</param>
        /// <param name="value2">可变变量</param>
        public static Var operator +(Var value1, Var value2)
        {
            switch (value1.Type)
            {
                case VarType.INT32:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.intValue + value2.intValue;
                        case VarType.UINT32: return value1.intValue + (uint)value2.intValue;
                        case VarType.INT64: return value1.intValue + value2.longValue;
                        case VarType.FLOAT: return value1.intValue + value2.floatValue;
                        case VarType.STRING: return value1.intValue + value2.ToString();
                    }
                    break;
                case VarType.UINT32:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return (uint)value1.intValue + value2.intValue;
                        case VarType.UINT32: return (uint)value1.intValue + (uint)value2.intValue;
                        case VarType.INT64: return (uint)value1.intValue + value2.longValue;
                        case VarType.UINT64: return (uint)value1.intValue + (ulong)value2.longValue;
                        case VarType.FLOAT: return (uint)value1.intValue + value2.floatValue;
                        case VarType.STRING: return (uint)value1.intValue + value2.ToString();
                    }
                    break;
                case VarType.INT64:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.longValue + value2.intValue;
                        case VarType.UINT32: return value1.longValue + (uint)value2.intValue;
                        case VarType.INT64: return value1.longValue + value2.longValue;
                        case VarType.FLOAT: return value1.longValue + value2.floatValue;
                        case VarType.STRING: return value1.longValue + value2.ToString();
                    }
                    break;
                case VarType.UINT64:
                    switch (value2.Type)
                    {
                        case VarType.UINT32: return (ulong)value1.longValue + (uint)value2.intValue;
                        case VarType.UINT64: return (ulong)value1.longValue + (ulong)value2.intValue;
                        case VarType.FLOAT: return (ulong)value1.longValue + value2.floatValue;
                        case VarType.STRING: return (ulong)value1.longValue + value2.ToString();
                    }
                    break;
                case VarType.FLOAT:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.floatValue + value2.intValue;
                        case VarType.UINT32: return value1.floatValue + (uint)value2.intValue;
                        case VarType.INT64: return value1.floatValue + value2.longValue;
                        case VarType.UINT64: return value1.floatValue + (ulong)value2.longValue;
                        case VarType.FLOAT: return value1.floatValue + value2.floatValue;
                        case VarType.STRING: return value1.floatValue + value2.ToString();
                    }
                    break;
                case VarType.STRING:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.ToString() + value2.intValue;
                        case VarType.UINT32: return value1.ToString() + (uint)value2.intValue;
                        case VarType.INT64: return value1.ToString() + value2.longValue;
                        case VarType.UINT64: return value1.ToString() + (ulong)value2.longValue;
                        case VarType.FLOAT: return value1.ToString() + value2.floatValue;
                        case VarType.STRING: return value1.ToString() + value2.ToString();
                    }
                    break;
            }
            value1.CheckType(value2.Type);
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value1">可变变量</param>
        /// <param name="value2">可变变量</param>
        public static Var operator -(Var value1, Var value2)
        {
            switch (value1.Type)
            {
                case VarType.INT32:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.intValue - value2.intValue;
                        case VarType.UINT32: return value1.intValue - (uint)value2.intValue;
                        case VarType.INT64: return value1.intValue - value2.longValue;
                        case VarType.FLOAT: return value1.intValue - value2.floatValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return (uint)value1.intValue - value2.intValue;
                        case VarType.UINT32: return (uint)value1.intValue - (uint)value2.intValue;
                        case VarType.INT64: return (uint)value1.intValue - value2.longValue;
                        case VarType.UINT64: return (uint)value1.intValue - (ulong)value2.longValue;
                        case VarType.FLOAT: return (uint)value1.intValue - value2.floatValue;
                    }
                    break;
                case VarType.INT64:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.longValue - value2.intValue;
                        case VarType.UINT32: return value1.longValue - (uint)value2.intValue;
                        case VarType.INT64: return value1.longValue - value2.longValue;
                        case VarType.FLOAT: return value1.longValue - value2.floatValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (value2.Type)
                    {
                        case VarType.UINT32: return (ulong)value1.longValue - (uint)value2.intValue;
                        case VarType.UINT64: return (ulong)value1.longValue - (ulong)value2.intValue;
                        case VarType.FLOAT: return (ulong)value1.longValue - value2.floatValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.floatValue - value2.intValue;
                        case VarType.UINT32: return value1.floatValue - (uint)value2.intValue;
                        case VarType.INT64: return value1.floatValue - value2.longValue;
                        case VarType.UINT64: return value1.floatValue - (ulong)value2.longValue;
                        case VarType.FLOAT: return value1.floatValue - value2.floatValue;
                    }
                    break;
            }
            value1.CheckType(value2.Type);
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value1">可变变量</param>
        /// <param name="value2">可变变量</param>
        public static Var operator *(Var value1, Var value2)
        {
            switch (value1.Type)
            {
                case VarType.INT32:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.intValue * value2.intValue;
                        case VarType.UINT32: return value1.intValue * (uint)value2.intValue;
                        case VarType.INT64: return value1.intValue * value2.longValue;
                        case VarType.FLOAT: return value1.intValue * value2.floatValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return (uint)value1.intValue * value2.intValue;
                        case VarType.UINT32: return (uint)value1.intValue * (uint)value2.intValue;
                        case VarType.INT64: return (uint)value1.intValue * value2.longValue;
                        case VarType.UINT64: return (uint)value1.intValue * (ulong)value2.longValue;
                        case VarType.FLOAT: return (uint)value1.intValue * value2.floatValue;
                    }
                    break;
                case VarType.INT64:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.longValue * value2.intValue;
                        case VarType.UINT32: return value1.longValue * (uint)value2.intValue;
                        case VarType.INT64: return value1.longValue * value2.longValue;
                        case VarType.FLOAT: return value1.longValue * value2.floatValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (value2.Type)
                    {
                        case VarType.UINT32: return (ulong)value1.longValue * (uint)value2.intValue;
                        case VarType.UINT64: return (ulong)value1.longValue * (ulong)value2.intValue;
                        case VarType.FLOAT: return (ulong)value1.longValue * value2.floatValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.floatValue * value2.intValue;
                        case VarType.UINT32: return value1.floatValue * (uint)value2.intValue;
                        case VarType.INT64: return value1.floatValue * value2.longValue;
                        case VarType.UINT64: return value1.floatValue * (ulong)value2.longValue;
                        case VarType.FLOAT: return value1.floatValue * value2.floatValue;
                    }
                    break;
            }
            value1.CheckType(value2.Type);
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value1">可变变量</param>
        /// <param name="value2">可变变量</param>
        public static Var operator /(Var value1, Var value2)
        {
            switch (value1.Type)
            {
                case VarType.INT32:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.intValue / value2.intValue;
                        case VarType.UINT32: return value1.intValue / (uint)value2.intValue;
                        case VarType.INT64: return value1.intValue / value2.longValue;
                        case VarType.FLOAT: return value1.intValue / value2.floatValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return (uint)value1.intValue / value2.intValue;
                        case VarType.UINT32: return (uint)value1.intValue / (uint)value2.intValue;
                        case VarType.INT64: return (uint)value1.intValue / value2.longValue;
                        case VarType.UINT64: return (uint)value1.intValue / (ulong)value2.longValue;
                        case VarType.FLOAT: return (uint)value1.intValue / value2.floatValue;
                    }
                    break;
                case VarType.INT64:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.longValue / value2.intValue;
                        case VarType.UINT32: return value1.longValue / (uint)value2.intValue;
                        case VarType.INT64: return value1.longValue / value2.longValue;
                        case VarType.FLOAT: return value1.longValue / value2.floatValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (value2.Type)
                    {
                        case VarType.UINT32: return (ulong)value1.longValue / (uint)value2.intValue;
                        case VarType.UINT64: return (ulong)value1.longValue / (ulong)value2.intValue;
                        case VarType.FLOAT: return (ulong)value1.longValue / value2.floatValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.floatValue / value2.intValue;
                        case VarType.UINT32: return value1.floatValue / (uint)value2.intValue;
                        case VarType.INT64: return value1.floatValue / value2.longValue;
                        case VarType.UINT64: return value1.floatValue / (ulong)value2.longValue;
                        case VarType.FLOAT: return value1.floatValue / value2.floatValue;
                    }
                    break;
            }
            value1.CheckType(value2.Type);
            return 0;
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
                case VarType.UINT32:
                    return value.intValue + 1;
                case VarType.INT64:
                case VarType.UINT64:
                    return value.longValue + 1;
                case VarType.FLOAT:
                    return value.floatValue + 1;
                default:
                    throw new Exception($"Var type [{value.Type}] is error!");
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
                case VarType.UINT32:
                    return value.intValue - 1;
                case VarType.INT64:
                case VarType.UINT64:
                    return value.longValue - 1;
                case VarType.FLOAT:
                    return value.floatValue - 1;
                default:
                    throw new Exception($"Var type [{value.Type}] is error!");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value1">可变变量1</param>
        /// <param name="value2">可变变量2</param>
        public static bool operator ==(Var value1, Var value2)
        {
            switch (value1.Type)
            {
                case VarType.UNKNOWN:
                    switch (value2.Type)
                    {
                        case VarType.UNKNOWN: return true;
                        default: return false;
                    }
                case VarType.INT32:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.intValue == value2.intValue;
                        case VarType.UINT32: return value1.intValue == (uint)value2.intValue;
                        case VarType.INT64: return value1.intValue == value2.longValue;
                        case VarType.FLOAT: return value1.intValue == value2.floatValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return (uint)value1.intValue == value2.intValue;
                        case VarType.UINT32: return (uint)value1.intValue == (uint)value2.intValue;
                        case VarType.INT64: return (uint)value1.intValue == value2.longValue;
                        case VarType.UINT64: return (uint)value1.intValue == (ulong)value2.longValue;
                        case VarType.FLOAT: return (uint)value1.intValue == value2.floatValue;
                    }
                    break;
                case VarType.INT64:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.longValue == value2.intValue;
                        case VarType.UINT32: return value1.longValue == (uint)value2.intValue;
                        case VarType.INT64: return value1.longValue == value2.longValue;
                        case VarType.FLOAT: return value1.longValue == value2.floatValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (value2.Type)
                    {
                        case VarType.UINT32: return (ulong)value1.longValue == (uint)value2.intValue;
                        case VarType.UINT64: return (ulong)value1.longValue == (ulong)value2.longValue;
                        case VarType.FLOAT: return (ulong)value1.longValue == value2.floatValue;
                    }
                    break;
                case VarType.FLOAT:
                    switch (value2.Type)
                    {
                        case VarType.INT32: return value1.floatValue == value2.intValue;
                        case VarType.UINT32: return value1.floatValue == (uint)value2.intValue;
                        case VarType.INT64: return value1.floatValue == value2.longValue;
                        case VarType.UINT64: return value1.floatValue == (ulong)value2.longValue;
                        case VarType.FLOAT: return value1.floatValue == value2.floatValue;
                    }
                    break;
                case VarType.BOOL:
                    switch (value2.Type)
                    {
                        case VarType.BOOL: return value1.intValue == value2.intValue;
                    }
                    break;
                case VarType.STRING:
                    switch (value2.Type)
                    {
                        case VarType.STRING: return value1.stringValue == value2.stringValue;
                    }
                    break;
                case VarType.OBJECT:
                    switch (value2.Type)
                    {
                        case VarType.OBJECT: return value1.objectValue == value2.objectValue;
                    }
                    break;
            }
            value1.CheckType(value2.Type);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value1">可变变量1</param>
        /// <param name="value2">可变变量2</param>
        public static bool operator !=(Var value1, Var value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            return obj is Var var &&
                   Type == var.Type &&
                   intValue == var.intValue &&
                   longValue == var.longValue &&
                   floatValue == var.floatValue &&
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
            return HashCode.Combine(Type, intValue, longValue, floatValue, stringValue, objectValue, listValue, mapValue);
        }

        /// <summary>
        /// 设置对象类型
        /// </summary>
        /// <param name="value"></param>
        public static Var ObjectVal(object value) { return new Var() { Type = VarType.OBJECT, objectValue = value }; }

        /// <summary>
        /// 设置列表类型
        /// </summary>
        /// <param name="list"></param>
        public static Var ListVal(VarList list) { return new Var() { Type = VarType.VARLIST, listValue = list }; }

        /// <summary>
        /// 设置列表类型
        /// </summary>
        /// <param name="map"></param>
        public static Var MapVal(VarMap map) { return new Var() { Type = VarType.VARMAP, mapValue = map }; }

        /// <summary>
        /// 检查类型
        /// </summary>
        /// <param name="toType"></param>
        /// <exception cref="Exception"></exception>
        private void CheckType(VarType toType)
        {
            if (Type != toType)
            {
                throw new Exception($"Var Type [{Type}] To [{toType}] Is Error");
            }
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
                    if (byte.MinValue <= intValue && intValue <= byte.MaxValue)
                    { bytes = new byte[2] { (byte)VarType.BYTE, (byte)intValue }; }
                    else if (sbyte.MinValue <= intValue && intValue <= sbyte.MaxValue)
                    { bytes = new byte[2] { (byte)VarType.SBYTE, (byte)intValue }; }
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
                    else if (int.MinValue <= intValue && intValue <= int.MaxValue)
                    {
                        bytes = new byte[5];
                        bytes[0] = (byte)VarType.INT32;
                        Buffer.BlockCopy(ByteHelper.GetBytes(intValue), 0, bytes, 1, 4);
                    }
                    else throw new Exception($"Var type [{Type}] is not to bytes!");
                    break;
                case VarType.UINT32:
                    bytes = new byte[5];
                    bytes[0] = (byte)VarType.UINT32;
                    Buffer.BlockCopy(ByteHelper.GetBytes((uint)intValue), 0, bytes, 1, 4);
                    break;
                case VarType.INT64:
                    bytes = new byte[9];
                    bytes[0] = (byte)VarType.INT64;
                    Buffer.BlockCopy(ByteHelper.GetBytes(longValue), 0, bytes, 1, 8);
                    break;
                case VarType.UINT64:
                    bytes = new byte[9];
                    bytes[0] = (byte)VarType.UINT64;
                    Buffer.BlockCopy(ByteHelper.GetBytes((ulong)longValue), 0, bytes, 1, 8);
                    break;
                case VarType.FLOAT:
                    bytes = new byte[5];
                    bytes[0] = (byte)VarType.FLOAT;
                    Buffer.BlockCopy(ByteHelper.GetBytes(floatValue), 0, bytes, 1, 4);
                    break;
                case VarType.BOOL:
                    bytes = new byte[1] { (byte)((byte)VarType.BOOL | (intValue == 1 ? 0x10 : 0x00)) };
                    break;
                case VarType.STRING:
                    byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(stringValue);
                    if (strBytes.Length <= byte.MaxValue)
                    {
                        bytes = new byte[2 + strBytes.Length];
                        bytes[0] = (byte)VarType.STRING | 0x10;
                        bytes[1] = (byte)strBytes.Length;
                        Buffer.BlockCopy(strBytes, 0, bytes, 2, strBytes.Length);
                    }
                    else if (strBytes.Length <= short.MaxValue)
                    {
                        bytes = new byte[3 + strBytes.Length];
                        bytes[0] = (byte)VarType.STRING | 0x20;
                        bytes[1] = (byte)(strBytes.Length >> 8);
                        bytes[2] = (byte)strBytes.Length;
                        Buffer.BlockCopy(strBytes, 0, bytes, 3, strBytes.Length);
                    }
                    else
                    {
                        bytes = new byte[5 + strBytes.Length];
                        bytes[0] = (byte)VarType.STRING | 0x30;
                        bytes[1] = (byte)(strBytes.Length >> 24);
                        bytes[2] = (byte)(strBytes.Length >> 16);
                        bytes[3] = (byte)(strBytes.Length >> 8);
                        bytes[4] = (byte)strBytes.Length;
                        Buffer.BlockCopy(strBytes, 0, bytes, 5, strBytes.Length);
                    }
                    break;
                case VarType.VARLIST:
                    byte[] listBytes = (listValue ?? VarList.New).GetBytes();
                    bytes = new byte[1 + listBytes.Length];
                    bytes[0] = (byte)VarType.VARLIST;
                    Buffer.BlockCopy(listBytes, 0, bytes, 1, listBytes.Length);
                    break;
                case VarType.VARMAP:
                    byte[] mapBytes = (mapValue ?? VarMap.New).GetBytes();
                    bytes = new byte[1 + mapBytes.Length];
                    bytes[0] = (byte)VarType.VARMAP;
                    Buffer.BlockCopy(mapBytes, 0, bytes, 1, mapBytes.Length);
                    break;
                case VarType.UNKNOWN:
                    bytes = new byte[1] { (byte)VarType.UNKNOWN | 0xF0 };
                    break;
                default:
                    throw new Exception($"Var type [{Type}] is not to bytes!");
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
            VarType Type = (VarType)(value[startIndex] & 0x0F);
            switch (Type)
            {
                case VarType.BYTE:
                    length = 2;
                    return value[startIndex + 1];
                case VarType.SBYTE:
                    length = 2;
                    return (sbyte)value[startIndex + 1];
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
                case VarType.BOOL:
                    length = 1;
                    return (value[startIndex] & 0xF0) == 0x10;
                case VarType.STRING:
                    int tag = value[startIndex] & 0xF0;
                    int strLen;
                    int index;
                    if (tag == 0x10) { strLen = value[startIndex + 1]; length = strLen + 2; index = 2; }
                    else if (tag == 0x20) { strLen = value[startIndex + 1] << 8 | value[startIndex + 2]; length = strLen + 3; index = 3; }
                    else { strLen = value[startIndex + 1] << 24 | value[startIndex + 2] << 16 | value[startIndex + 3] << 8 | value[startIndex + 4]; length = strLen + 5; index = 5; }
                    return System.Text.Encoding.UTF8.GetString(value, startIndex + index, strLen);
                case VarType.VARLIST:
                    VarList list = VarList.Parse(value, startIndex + 1, out int listLen) ?? VarList.New;
                    length = listLen + 1;
                    return ListVal(list);
                case VarType.VARMAP:
                    VarMap map = VarMap.Parse(value, startIndex + 1, out int mapLen) ?? VarMap.New;
                    length = mapLen + 1;
                    return MapVal(map);
                case VarType.UNKNOWN:
                    length = 1;
                    if ((value[startIndex] & 0xF0) == 0xF0)
                        return Empty;
                    throw new Exception($"Var UNKNOWN type is [{value[startIndex]}] error!");
                default:
                    throw new Exception($"Var type [{Type}] is error!");
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
