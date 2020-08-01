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
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns></returns>
        public static int GetValidLength(byte[] bytes)
        {
            int i = 0;
            if (null == bytes || 0 == bytes.Length) return i;
            for (; i < bytes.Length; i++)
            {
                if (i + 4 < bytes.Length && bytes[i] + bytes[i + 1] + bytes[i + 2] + bytes[i + 3] + bytes[i + 4] == 0x00) break;
            }
            return i;
        }

        /// <summary>
        /// 获取byte的实际数据
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
