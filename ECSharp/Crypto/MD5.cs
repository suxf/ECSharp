#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;
using System.Text;

namespace ECSharp.Crypto
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
        /// <param name="needLen">需要的长度,最大不超过32个</param>
        /// <returns></returns>
        public static string Encrypt(string str, int needLen = -1)
        {
            return Encrypt(Encoding.UTF8.GetBytes(str), needLen);
        }

        /// <summary>
        /// 加密（32位小写）
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <param name="needLen">需要的长度,最大不超过32个</param>
        /// <returns></returns>
        public static string Encrypt(byte[] data, int needLen = -1)
        {
            byte[] md5data = EncryptBytes(data);
                
            StringBuilder sb = new StringBuilder();
            for (int i = 0, len = 0 <= needLen && needLen < md5data.Length ? needLen : md5data.Length; i < len; i++)
            {
                sb.Append(md5data[i].ToString("x2").PadLeft(2, '0'));
            }
            return sb.ToString().ToLower();
        }

        /// <summary>
        /// 加密（16位字节）
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <param name="needLen">需要的长度,最大不超过16个</param>
        /// <returns></returns>
        public static byte[] EncryptBytes(byte[] data, int needLen = -1)
        {
            Span<byte> md5data;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
            }
            return md5data.Slice(0, 0 <= needLen && needLen < md5data.Length ? needLen : md5data.Length).ToArray();
        }
    }
}
