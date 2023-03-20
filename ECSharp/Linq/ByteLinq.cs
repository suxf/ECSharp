#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;
using System.Text;

namespace ECSharp.Linq
{
    /// <summary>
    /// 拓展方法类
    /// <para>Byte数组比较</para>
    /// </summary>
    public static class ByteLinq
    {
        /// <summary>
        /// 比较字节数组
        /// </summary>
        /// <param name="sb1">字节数组1</param>
        /// <param name="sb2">字节数组2</param>
        /// <returns>相同返回true 不同返回false</returns>
        public static bool Compare(this ReadOnlySpan<byte> sb1, ReadOnlySpan<byte> sb2)
        {
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
        /// 比较字节数组
        /// </summary>
        /// <param name="b1">字节数组1</param>
        /// <param name="b2">字节数组2</param>
        /// <returns>相同返回true 不同返回false</returns>
        public static bool Compare(this byte[] b1, byte[] b2)
        {
            return Compare(b1.AsSpan(), b2.AsSpan());
        }

        /// <summary>
        /// 比较字节数组
        /// </summary>
        /// <param name="b1">字节数组1</param>
        /// <param name="b2">字节数组2</param>
        /// <returns>相同返回true 不同返回false</returns>
        public static bool Compare(this ReadOnlyMemory<byte> b1, ReadOnlyMemory<byte> b2)
        {
            return Compare(b1.Span, b2.Span);
        }

        /// <summary>
        /// 比较字节数组
        /// </summary>
        /// <param name="b1">字节数组1</param>
        /// <param name="b2">字节数组2</param>
        /// <returns>相同返回true 不同返回false</returns>
        public static bool Compare(this Span<byte> b1, Span<byte> b2)
        {
            return Compare((ReadOnlySpan<byte>)b1, (ReadOnlySpan<byte>)b2);
        }

        /// <summary>
        /// 比较字节数组
        /// </summary>
        /// <param name="b1">字节数组1</param>
        /// <param name="b2">字节数组2</param>
        /// <returns>相同返回true 不同返回false</returns>
        public static bool Compare(this Memory<byte> b1, Memory<byte> b2)
        {
            return Compare(b1.Span, b2.Span);
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

        /// <summary>
        /// 转为UTF-8编码的字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public static string AsString(this ReadOnlySpan<byte> bytes)
        {
            return AsString(bytes.ToArray());
        }

        /// <summary>
        /// 转为UTF-8编码的字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public static string AsString(this ReadOnlyMemory<byte> bytes)
        {
            return AsString(bytes.ToArray());
        }
    }
}
