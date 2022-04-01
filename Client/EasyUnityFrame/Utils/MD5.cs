using System;
using System.Text;

namespace ES.Utils
{
    /// <summary>
    /// md5工具
    /// </summary>
    public static class MD5
    {
        /// <summary>
        /// 加密（32位小写）
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string Encrypt(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);//将字符编码为一个字节序列 
            return Encrypt(data);
        }

        /// <summary>
        /// 加密（32位小写）
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <returns></returns>
        public static string Encrypt(byte[] data)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            ReadOnlySpan<byte> md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
            md5.Clear();
            StringBuilder sb = new StringBuilder();
            for (int i = 0, len = md5data.Length; i < len; i++)
            {
                sb.Append(md5data[i].ToString("x2").PadLeft(2, '0'));
            }
            return sb.ToString().ToLower();
        }
    }
}
