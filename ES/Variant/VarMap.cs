using System;

namespace ES.Variant
{
    /// <summary>
    /// 可变变量字典
    /// </summary>
    public class VarMap : Map<Var, Var>
    {
        /// <summary>
        /// 新建列表
        /// </summary>
        public static VarMap New { get { return new VarMap(); } }

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
                return Var.Empty;
            }
            set { base[key] = value; }
        }

        /// <summary>
        /// 合并可变变量字典
        /// </summary>
        /// <param name="varmap"></param>
        /// <returns></returns>
        public VarMap Merge(VarMap varmap)
        {
            Merge(varmap);
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
            return this;
        }

        /// <summary>
        /// 增加一个可变列表对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public VarMap Add(Var key, VarList list)
        {
            base.Add(key, Var.ListVal(list));
            return this;
        }

        /// <summary>
        /// 增加一个可变字典对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public VarMap Add(Var key, VarMap map)
        {
            base.Add(key, Var.MapVal(map));
            return this;
        }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[][][] bs = new byte[Count + 1][][];
            int size = 0;
            int i = 1;
            foreach (var pair in this)
            {
                bs[i] = new byte[2][];
                bs[i][0] = pair.Key.GetBytes();
                bs[i][1] = pair.Value.GetBytes();
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
                throw new Exception($"Var Map Max Count 255, Now Count Is {Count}!");
            bytes[index++] = (byte)Count;
            bytes[index] = (byte)VarType.VARMAP_END;
            return bytes;
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
            if (data[startIndex] != (byte)VarType.VARMAP_HEAD)
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
            VarMap map = New;
            while (size > 0)
            {
                Var key = Var.Parse(data, startIndex, out int keyLen);
                startIndex += keyLen;
                Var value = Var.Parse(data, startIndex, out int valueLen);
                startIndex += valueLen;
                map.Add(key, value);
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
        /// <param name="length"></param>
        /// <returns></returns>
        public static VarMap? Parse(byte[] data, out int length)
        {
            return Parse(data, 0, out length);
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static VarMap? Parse(byte[] data, int startIndex)
        {
            return Parse(data, startIndex, out _);
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static VarMap? Parse(byte[] data)
        {
            return Parse(data, 0, out _);
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="map"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data, int startIndex, out VarMap map, out int length)
        {
            VarMap? tempMap = Parse(data, startIndex, out length);
            if (tempMap == null)
            {
                map = New;
                return false;
            }
            else
            {
                map = tempMap;
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
        public static bool TryParse(byte[] data, out VarMap map, out int length)
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
        public static bool TryParse(byte[] data, int startIndex, out VarMap map)
        {
            return TryParse(data, startIndex, out map, out _);
        }

        /// <summary>
        /// 转字典
        /// </summary>
        /// <param name="data"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool TryParse(byte[] data, out VarMap map)
        {
            return TryParse(data, 0, out map, out _);
        }
    }
}
