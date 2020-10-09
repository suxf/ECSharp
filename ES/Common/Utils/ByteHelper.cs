using System;

namespace ES.Common.Utils
{
    /// <summary>
    /// 字节助手
    /// </summary>
    public static class ByteHelper
    {
        /// <summary>
        /// 获取byte的实际长度
        /// <para>数组中有连续9个字节连续为0的情况</para>
        /// <para>原理 默认基础类型字节占用情况最大为8个</para>
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns></returns>
        public static int GetValidLength(byte[] bytes)
        {
            int i = 0;
            if (null == bytes || 0 == bytes.Length) return i;
            for (; i < bytes.Length; i++)
            {
                int index = i;
                if (i + 8 < bytes.Length)
                {
                    int r = bytes[index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index];
                    if (r == 0x00) break;
                }
            }
            return i;
        }

        /// <summary>
        /// 获取byte的实际数据
        /// <para>数组中有连续9个字节连续为0的情况</para>
        /// <para>原理 默认基础类型字节占用情况最大为8个</para>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>实际长度的byte[]</returns>
        public static byte[] GetValidByte(byte[] bytes)
        {
            int length = GetValidLength(bytes);
            if (0 == length) return null;
            byte[] bb = new byte[length];
            Buffer.BlockCopy(bytes, 0, bb, 0, length);
            return bb;
        }
    }
}
