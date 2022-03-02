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
        /// 新建列表
        /// </summary>
        public static VarList New { get { return new VarList(); } }

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
        /// <returns></returns>
        public static VarList Parse(byte[] data, int startIndex, out int length)
        {
            if (data[startIndex] != (byte)VarType.VARLIST_HEAD)
            {
                length = 0;
                return New;
            }
            int size = Var.Parse(data, startIndex + 1, out int sizeLen);
            if (data[startIndex + size + sizeLen + 2] != (byte)VarType.VARLIST_END)
            {
                length = 0;
                return New;
            }
            startIndex += 1;
            length = size + 3 + sizeLen;
            startIndex += sizeLen;
            VarList list = New;
            while (size > 0)
            {
                list.Add(Var.Parse(data, startIndex, out int varLen));
                startIndex += varLen;
                size -= varLen;
            }
            if (data[startIndex] != (byte)list.Count)
            {
                length = 0;
                return New;
            }
            return list;
        }

        /// <summary>
        /// 转列表
        /// </summary>
        /// <returns></returns>
        public static VarList? Parse(byte[] data, out int length)
        {
            return Parse(data, 0, out length);
        }
    }
}
