using System.Text;

namespace ES.Linq
{
    /// <summary>
    /// 字符串转其他类型拓展
    /// 拓展方法类
    /// <para>此类用于拓展一些对象上的方法</para>
    /// <para>便于更快捷的开发</para>
    /// </summary>
    public static class StringLinq
    {
        /// <summary>
        /// 转为UTF-8编码的字节数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static byte[] AsBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 转字节
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte AsByte(this string str)
        {
            return byte.Parse(str);
        }
        /// <summary>
        /// 转32位整型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int AsInt32(this string str)
        {
            return int.Parse(str);
        }
        /// <summary>
        /// 转64位整型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long AsInt64(this string str)
        {
            return long.Parse(str);
        }
        /// <summary>
        /// 转单精度浮点型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float AsFloat(this string str)
        {
            return float.Parse(str);
        }
        /// <summary>
        /// 转双精度浮点型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double AsDouble(this string str)
        {
            return double.Parse(str);
        }
        /// <summary>
        /// 转布尔型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool AsBool(this string str)
        {
            return bool.Parse(str);
        }
    }
}
