using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ES.Variant
{
    /// <summary>
    /// 可变变量列表
    /// </summary>
    public class VarList : List<Var>
    {
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
                return Var.Empty;
            }
            set { base[index] = value; }
        }

        /// <summary>
        /// 合并可变变量列表
        /// </summary>
        /// <param name="varlist"></param>
        /// <returns></returns>
        public VarList Merge(VarList varlist)
        {
            AddRange(varlist);
            return this;
        }

        /// <summary>
        /// 增加一个可变列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public VarList Add(VarList list)
        {
            base.Add(Var.ListVal(list));
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
            return this;
        }

        /// <summary>
        /// 增加多个可变变量
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public VarList Add(params Var[] value)
        {
            AddRange(value);
            return this;
        }

        /// <summary>
        /// 增加一个字典
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public VarList Add(VarMap map)
        {
            base.Add(Var.MapVal(map));
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
            varlist.Add(value);
            return varlist;
        }

        /// <summary>
        /// 增加可变变量
        /// </summary>
        /// <param name="varlist"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarList operator +(VarList varlist, Var value)
        {
            varlist.Add(value);
            return varlist;
        }

        /// <summary>
        /// 转json对象
        /// </summary>
        /// <returns></returns>
        public JArray ToJson()
        {
            JArray json = new JArray();
            foreach (var value in this)
            {
                switch (value.Type)
                {
                    case VarType.INT32: json.Add((int)value); break;
                    case VarType.UINT32: json.Add((uint)value); break;
                    case VarType.INT64: json.Add((long)value); break;
                    case VarType.UINT64: json.Add((ulong)value); break;
                    case VarType.FLOAT: json.Add((float)value); break;
                    case VarType.BOOL: json.Add((bool)value); break;
                    case VarType.STRING: json.Add((string)value); break;
                    case VarType.VARLIST: json.Add(value.List.ToJson()); break;
                    case VarType.VARMAP: json.Add(value.Map.ToJson()); break;
                }
            }
            return json;
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            return JsonConvert.SerializeObject(ToJson());
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static VarList Parse(JArray json)
        {
            VarList list = new VarList();
            foreach (var value in json)
            {
                if (value == null) continue;
                switch (value.Type)
                {
                    case JTokenType.Integer: list.Add((int)value); break;
                    case JTokenType.Float: list.Add((float)value); break;
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
        public static bool TryParse(string json, out VarList list)
        {
            VarList? value = Parse(json);
            if (value == null)
            {
                list = new VarList();
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
            byte[][] bs = new byte[Count + 1][];
            int size = 0;
            for (int i = 0, len = Count; i < len; i++)
            {
                var b = this[i].GetBytes();
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
                throw new Exception($"Var List Max Count 255, Now Count Is {Count}!");
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
        public static VarList? Parse(byte[] data, int startIndex, out int length)
        {
            if (data[startIndex] != (byte)VarType.VARLIST_HEAD)
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
        /// <param name="length"></param>
        /// <returns></returns>
        public static VarList? Parse(byte[] data, out int length)
        {
            return Parse(data, 0, out length);
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static VarList? Parse(byte[] data, int startIndex)
        {
            return Parse(data, startIndex, out _);
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static VarList? Parse(byte[] data)
        {
            return Parse(data, 0, out _);
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data, int startIndex, out VarList list, out int length)
        {
            VarList? tempList = Parse(data, startIndex, out length);
            if (tempList == null)
            {
                list = new VarList();
                return false;
            }
            else
            {
                list = tempList;
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
        public static bool TryParse(byte[] data, out VarList list, out int length)
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
        public static bool TryParse(byte[] data, int startIndex, out VarList list)
        {
            return TryParse(data, startIndex, out list, out _);
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data, out VarList list)
        {
            return TryParse(data, 0, out list, out _);
        }
    }
}
