using System.Text;

namespace ES.Linq
{
    /// <summary>
    /// Byte数组比较
    /// 拓展方法类
    /// <para>此类用于拓展一些对象上的方法</para>
    /// <para>便于更快捷的开发</para>
    /// </summary>
    public static class ByteLinq
    {
        /// <summary>
        /// 比较字节数组
        /// </summary>
        /// <param name="b1">字节数组1</param>
        /// <param name="b2">字节数组2</param>
        /// <returns>相同返回true 不同返回false</returns>
        public static bool Compare(this byte[] b1, byte[] b2)
        {
            if (b2 == null) return false;
            int result = 0;
            if (b1.Length != b2.Length)
                result = b1.Length - b2.Length;
            else
            {
                for (int i = 0; i < b1.Length; i++)
                {
                    if (b1[i] != b2[i])
                    {
                        result = b1[i] - b2[i];
                        break;
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
