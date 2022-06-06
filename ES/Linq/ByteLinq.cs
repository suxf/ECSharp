using System;
using System.Text;

namespace ES.Linq
{
    /// <summary>
    /// 拓展方法类
    /// <para>Byte数组比较</para>
    /// </summary>
    public static partial class EsLinq
    {
        /// <summary>
        /// 比较字节数组
        /// </summary>
        /// <param name="b1">字节数组1</param>
        /// <param name="b2">字节数组2</param>
        /// <returns>相同返回true 不同返回false</returns>
        public static bool Compare(this byte[] b1, byte[] b2)
        {
            ReadOnlySpan<byte> sb1 = b1;
            ReadOnlySpan<byte> sb2 = b2;
            int result = 0;

            if (sb1.Length != sb2.Length)
                result = sb1.Length - sb2.Length;
            else
            {
                int len = sb1.Length;
                if (len > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        if (sb1[i] != sb1[i])
                        {
                            result = sb1[i] - sb1[i];
                            break;
                        }
                    }
                }
            }

            return result == 0;
        }

        /// <summary>
        /// 转为UTF-8编码的字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public static string AsString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
