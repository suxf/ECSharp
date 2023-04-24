#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ECSharp.Variant
{
    /// <summary>
    /// 可变变量列表
    /// </summary>
    public class VarList : List<Var>
    {
        /// <summary>
        /// 新建一个可变变量列表
        /// </summary>
        public static VarList New => new VarList();

        /// <summary>
        /// 变化监听
        /// </summary>
        private Action<int, Var>? changeListener;

        /// <summary>
        /// 构建可变变量列表
        /// </summary>
        public VarList() { }

        /// <summary>
        /// 构建可变变量列表
        /// </summary>
        /// <param name="value"></param>
        public VarList(Var value) => Add(value);

        /// <summary>
        /// 构建可变变量列表
        /// </summary>
        /// <param name="values"></param>
        public VarList(params Var[] values) => AddRange(values);

        /// <summary>
        /// 构建可变变量列表
        /// </summary>
        /// <param name="value"></param>
        public VarList(IEnumerable<Var> value) => AddRange(value);

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// 根据索引安全获取值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new Var this[int index]
        {
            get
            {
                if (0 <= index && index < Count)
                    return base[index];
                return Var.Null;
            }
            set 
            { 
                base[index] = value;
                changeListener?.Invoke(index, value);
            }
        }

        /// <summary>
        /// 根据索引安全获取值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Var this[Enum index]
        {
            get
            {
                return this[Convert.ToInt32(index)];
            }
            set 
            {
                this[Convert.ToInt32(index)] = value;
            }
        }

        /// <summary>
        /// 设置一个值变化监听，当列表被修改就会触发监听
        /// </summary>
        /// <param name="changeListener"></param>
        public void SetChangeListener(Action<int, Var> changeListener)
        {
            this.changeListener = changeListener;
        }

        /// <summary>
        /// 合并可变变量列表
        /// </summary>
        /// <param name="varlist"></param>
        /// <returns></returns>
        public VarList Merge(VarList varlist)
        {
            return AddRange(varlist);
        }

        /// <summary>
        /// 插入一个值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public new VarList Insert(int index, Var value)
        {
            base.Insert(index, value);
            changeListener?.Invoke(index, value);
            return this;
        }

        /// <summary>
        /// 插入一组值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public new VarList InsertRange(int index, IEnumerable<Var> value)
        {
            base.InsertRange(index, value);
            changeListener?.Invoke(index, Var.Null);
            return this;
        }

        /// <summary>
        /// 增加一个可变变量
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public new VarList Add(Var value)
        {
            base.Add(value);
            changeListener?.Invoke(Count - 1, value);
            return this;
        }

        /// <summary>
        /// 增加多个可变变量
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public VarList MultiAdd(params Var[] value)
        {
            return AddRange(value);
        }

        /// <summary>
        /// 增加多个可变变量
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public new VarList AddRange(IEnumerable<Var> value)
        {
            base.AddRange(value);
            changeListener?.Invoke(Count - 1, Var.Null);
            return this;
        }

        /// <summary>
        /// 追加可变列表
        /// </summary>
        /// <param name="varlist"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarList operator +(VarList varlist, VarList value)
        {
            return varlist.Add(value);
        }

        /// <summary>
        /// 追加可变字典
        /// </summary>
        /// <param name="varlist"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarList operator +(VarList varlist, VarMap value)
        {
            return varlist.Add(value);
        }

        /// <summary>
        /// 增加可变变量
        /// </summary>
        /// <param name="varlist"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarList operator +(VarList varlist, Var value)
        {
            return varlist.Add(value);
        }

        /// <summary>
        /// 转json对象
        /// </summary>
        /// <returns></returns>
        public JArray ToJArray()
        {
            JArray json = new JArray();
            for (int i = 0, len = Count; i < len; i++)
            {
                var value = this[i];
                switch (value.Type)
                {
                    case VarType.INT32: json.Add((int)value); break;
                    case VarType.INT64: json.Add((long)value); break;
                    case VarType.FLOAT: json.Add((float)value); break;
                    case VarType.BOOL: json.Add((bool)value); break;
                    case VarType.STRING: json.Add((string)value); break;
                    case VarType.VARLIST: json.Add(value.ToJArray()); break;
                    case VarType.VARMAP: json.Add(value.ToJObject()); break;
                }
            }
            return json;
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(ToJArray());
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static VarList Parse(JArray json)
        {
            VarList list = new VarList();
            for (int i = 0, len = json.Count; i < len; i++)
            {
                var value = json[i];
                if (value == null) continue;
                switch (value.Type)
                {
                    case JTokenType.Integer: list.Add((long)value); break;
                    case JTokenType.Float: list.Add((double)value); break;
                    case JTokenType.Boolean: list.Add((bool)value); break;
                    case JTokenType.Array: list.Add(Parse((JArray)value)); break;
                    case JTokenType.Object: list.Add(VarMap.Parse((JObject)value)); break;
                    case JTokenType.None: break;
                    default: list.Add(value.ToString()); break;
                }
            }
            return list;
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static VarList? Parse(string json)
        {
            JArray? jsonArr = JsonConvert.DeserializeObject<JArray>(json);
            if (jsonArr == null) return null;
            return Parse(jsonArr);
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="json"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool TryParse(string json,
#if NETCOREAPP3_1_OR_GREATER
            [MaybeNullWhen(false)]
#endif
        out VarList list)
        {
            VarList? value = Parse(json);
            if (value == null)
            {
                list = null;
                return false;
            }
            else
            {
                list = value;
                return true;
            }
        }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return new Var(this).GetBytes();
        }

        internal byte[] GetBytesInternal()
        {
            byte[][] bs = new byte[Count + 1][];
            int size = 0;
            for (int i = 0, len = Count; i < len; i++)
            {
                var b = this[i].GetBytes();

                if (b.Length == 0)
                    continue;

                size += b.Length;
                bs[i + 1] = b;
            }
            bs[0] = ((Var)size).GetBytes();
            size += bs[0].Length;
            byte[] bytes = new byte[size + 3];
            bytes[0] = (byte)VarType.VARLIST_HEAD;
            int index = 1;

            for (int j = 0, len2 = bs.Length; j < len2; j++)
            {
                var bLen = bs[j].Length;
                Buffer.BlockCopy(bs[j], 0, bytes, index, bLen);
                index += bLen;
            }

            if (Count > byte.MaxValue)
                throw VarException.CreateLengthError(Count);
            bytes[index++] = (byte)Count;
            bytes[index] = (byte)VarType.VARLIST_END;
            return bytes;
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static VarList? ParseInternal(byte[] data, int startIndex, out int length)
        {
            if (data.Length == 0 || data[startIndex] != (byte)VarType.VARLIST_HEAD)
            {
                length = 0;
                return null;
            }

            int size = Var.Parse(data, startIndex + 1, out int sizeLen);
            if (data[startIndex + size + sizeLen + 2] != (byte)VarType.VARLIST_END)
            {
                length = 0;
                return null;
            }

            startIndex += 1;
            length = size + 3 + sizeLen;
            startIndex += sizeLen;
            VarList list = new VarList();
            while (size > 0)
            {
                list.Add(Var.Parse(data, startIndex, out int varLen));

                if (varLen == 0)
                    break;

                startIndex += varLen;
                size -= varLen;
            }

            if (data[startIndex] != (byte)list.Count)
            {
                length = 0;
                return null;
            }
            return list;
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static VarList? Parse(byte[] data, int startIndex, out int length)
        {
            Var result = Var.Parse(data, startIndex, out length);
            if (result.IsList) return result.List;
            return null;
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static VarList? Parse(byte[] data, out int length)
        {
            Var result = Var.Parse(data, 0, out length);
            if (result.IsList) return result.List;
            return null;
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static VarList? Parse(byte[] data, int startIndex)
        {
            Var result = Var.Parse(data, startIndex, out _);
            if (result.IsList) return result.List;
            return null;
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static VarList? Parse(byte[] data)
        {
            Var result = Var.Parse(data, 0, out _);
            if (result.IsList) return result.List;
            return null;
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data, int startIndex,
#if NETCOREAPP3_1_OR_GREATER
            [MaybeNullWhen(false)]
#endif
        out VarList list, out int length)
        {
            Var result = Var.Parse(data, startIndex, out length);
            if (!result.IsList)
            {
                list = null;
                return false;
            }
            else
            {
                list = result;
                return true;
            }
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data, out VarList? list, out int length)
        {
            return TryParse(data, 0, out list, out length);
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data, int startIndex,
#if NETCOREAPP3_1_OR_GREATER
            [MaybeNullWhen(false)]
#endif
        out VarList list)
        {
            return TryParse(data, startIndex, out list, out _);
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data,
#if NETCOREAPP3_1_OR_GREATER
            [MaybeNullWhen(false)]
#endif
        out VarList list)
        {
            return TryParse(data, 0, out list, out _);
        }
    }
}
