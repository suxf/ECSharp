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
    /// 可变变量字典
    /// </summary>
    public class VarMap : Dictionary<Var, Var>
    {
        /// <summary>
        /// 新建一个可变变量字典
        /// </summary>
        public static VarMap New => new VarMap();

        /// <summary>
        /// 变化监听
        /// </summary>
        private Action<Var, Var>? changeListener;

        /// <summary>
        /// 构建可变变量字典
        /// </summary>
        public VarMap() { }

        /// <summary>
        /// 构建可变变量字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public VarMap(Var key, Var value) => Add(key, value);

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// 根据键名安全获取键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new Var this[Var key]
        {
            get
            {
                if (TryGetValue(key, out Var value))
                    return value;

                return Var.Null;
            }
            set 
            {
                base[key] = value;
                changeListener?.Invoke(key, value);
            }
        }

        /// <summary>
        /// 设置一个值变化监听，当列表被修改就会触发监听
        /// </summary>
        /// <param name="changeListener"></param>
        public void SetChangeListener(Action<Var, Var> changeListener)
        {
            this.changeListener = changeListener;
        }

        /// <summary>
        /// 合并可变变量字典
        /// <para>合并字典中已存在的则覆盖最新值</para>
        /// </summary>
        /// <param name="varmap"></param>
        /// <returns></returns>
        public VarMap Merge(VarMap varmap)
        {
            foreach (var key in varmap.Keys)
            {
                if (ContainsKey(key))
                    this[key] = varmap[key];
                else
                    Add(key, varmap[key]);
            }
            return this;
        }

        /// <summary>
        /// 增加一个可变变量对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public new VarMap Add(Var key, Var value)
        {
            base.Add(key, value);
            changeListener?.Invoke(key, value);
            return this;
        }

        /// <summary>
        /// 添加或者更新
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public VarMap AddOrUpdate(Var key, Var value)
        {
            if (ContainsKey(key))
                this[key] = value;
            else
                Add(key, value);
            return this;
        }

#if NETCOREAPP3_1_OR_GREATER
        /// <summary>
        /// 尝试添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public new bool TryAdd(Var key, Var value)
        {
            if (base.TryAdd(key, value))
            {
                changeListener?.Invoke(key, value);
                return true;
            }
            return false;
        }
#endif

        /// <summary>
        /// 转Json对象
        /// </summary>
        /// <returns></returns>
        public JObject ToJObject()
        {
            JObject json = new JObject();
            foreach (var item in this)
            {
                Var key = item.Key;
                if (key.Type == VarType.NULL || key.Type == VarType.STRUCT || key.Type == VarType.OBJECT
                    || key.Type == VarType.VARLIST || key.Type == VarType.VARMAP)
                    continue;

                Var value = item.Value;
                switch (value.Type)
                {
                    case VarType.INT32: json.Add(key.ToString(), (int)value); break;
                    case VarType.INT64: json.Add(key.ToString(), (long)value); break;
                    case VarType.FLOAT: json.Add(key.ToString(), (float)value); break;
                    case VarType.BOOL: json.Add(key.ToString(), (bool)value); break;
                    case VarType.STRING: json.Add(key.ToString(), (string)value); break;
                    case VarType.VARLIST: json.Add(key.ToString(), value.ToJArray()); break;
                    case VarType.VARMAP: json.Add(key.ToString(), value.ToJObject()); break;
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
            return JsonConvert.SerializeObject(ToJObject());
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static VarMap Parse(JObject json)
        {
            VarMap map = new VarMap();
            foreach (var kv in json)
            {
                var key = kv.Key;
                var value = kv.Value;

                if (key == null || value == null)
                    continue;

                switch (value.Type)
                {
                    case JTokenType.Integer: map.Add(key, (long)value); break;
                    case JTokenType.Float: map.Add(key, (double)value); break;
                    case JTokenType.Boolean: map.Add(key, (bool)value); break;
                    case JTokenType.Array: map.Add(key, VarList.Parse((JArray)value)); break;
                    case JTokenType.Object: map.Add(key, Parse((JObject)value)); break;
                    case JTokenType.None: break;
                    default: map.Add(key, value.ToString()); break;
                }
            }
            return map;
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static VarMap? Parse(string json)
        {
            JObject? jsonObj = JsonConvert.DeserializeObject<JObject>(json);

            if (jsonObj == null)
                return null;

            return Parse(jsonObj);
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="json"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool TryParse(string json,
#if NETCOREAPP3_1_OR_GREATER
            [MaybeNullWhen(false)]
#endif
        out VarMap map)
        {
            VarMap? value = Parse(json);
            if (value == null)
            {
                map = null;
                return false;
            }
            else
            {
                map = value;
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
            byte[][][] bs = new byte[Count + 1][][];
            int size = 0;
            int i = 1;
            foreach (var pair in this)
            {
                bs[i] = new byte[2][];
                bs[i][0] = pair.Key.GetBytes();
                bs[i][1] = pair.Value.GetBytes();

                if (bs[i][0].Length == 0 || bs[i][1].Length == 0)
                    continue;

                size += bs[i][0].Length + bs[i][1].Length;
                i += 1;
            }
            bs[0] = new byte[1][];
            bs[0][0] = ((Var)size).GetBytes();
            size += bs[0][0].Length;
            byte[] bytes = new byte[size + 3];
            bytes[0] = (byte)VarType.VARMAP_HEAD;
            int index = 1;
            for (int j = 0, len2 = bs.Length; j < len2; j++)
            {
                var subBytes = bs[j];
                for (int k = 0, len3 = subBytes.Length; k < len3; k++)
                {
                    var bLen = subBytes[k].Length;
                    Buffer.BlockCopy(subBytes[k], 0, bytes, index, bLen);
                    index += bLen;
                }
            }

            if (Count > byte.MaxValue)
                throw VarException.CreateLengthError(Count);

            bytes[index++] = (byte)Count;
            bytes[index] = (byte)VarType.VARMAP_END;
            return bytes;
        }

        internal static VarMap? ParseInternal(byte[] data, int startIndex, out int length)
        {
            if (data.Length == 0 || data[startIndex] != (byte)VarType.VARMAP_HEAD)
            {
                length = 0;
                return null;
            }

            int size = Var.Parse(data, startIndex + 1, out int sizeLen);
            if (data[startIndex + size + sizeLen + 2] != (byte)VarType.VARMAP_END)
            {
                length = 0;
                return null;
            }

            startIndex += 1;
            length = size + 3 + sizeLen;
            startIndex += sizeLen;
            VarMap map = new VarMap();
            while (size > 0)
            {
                Var key = Var.Parse(data, startIndex, out int keyLen);
                startIndex += keyLen;
                Var value = Var.Parse(data, startIndex, out int valueLen);
                startIndex += valueLen;
                map.Add(key, value);

                if (keyLen + valueLen == 0)
                    break;

                size = size - keyLen - valueLen;
            }

            if (data[startIndex] != (byte)map.Count)
            {
                length = 0;
                return null;
            }
            return map;
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static VarMap? Parse(byte[] data, int startIndex, out int length)
        {
            Var result = Var.Parse(data, startIndex, out length);
            if(result.IsMap) return result;
            return null;
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static VarMap? Parse(byte[] data, out int length)
        {
            Var result = Var.Parse(data, 0, out length);
            if(result.IsMap) return result;
            return null;
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static VarMap? Parse(byte[] data, int startIndex)
        {
            Var result = Var.Parse(data, startIndex, out _);
            if(result.IsMap) return result;
            return null;
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static VarMap? Parse(byte[] data)
        {
            Var result = Var.Parse(data, 0, out _);
            if(result.IsMap) return result;

            return null;
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="map"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data, int startIndex,
#if NETCOREAPP3_1_OR_GREATER
            [MaybeNullWhen(false)]
#endif
        out VarMap map, out int length)
        {
            Var result = Var.Parse(data, startIndex, out length);
            if (!result.IsMap)
            {
                map = null;
                return false;
            }
            else
            {
                map = result;
                return true;
            }
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <param name="map"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data,
#if NETCOREAPP3_1_OR_GREATER
            [MaybeNullWhen(false)]
#endif 
        out VarMap map, out int length)
        {
            return TryParse(data, 0, out map, out length);
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data, int startIndex,
#if NETCOREAPP3_1_OR_GREATER
            [MaybeNullWhen(false)]
#endif
        out VarMap map)
        {
            return TryParse(data, startIndex, out map, out _);
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data,
#if NETCOREAPP3_1_OR_GREATER
            [MaybeNullWhen(false)]
#endif
        out VarMap map)
        {
            return TryParse(data, 0, out map, out _);
        }
    }
}
