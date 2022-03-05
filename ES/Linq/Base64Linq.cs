using System;
using System.Text;

namespace ES.Linq
{
    /// <summary>
    /// base64编码\解码构造器
    /// 拓展方法类
    /// <para>此类用于拓展一些对象上的方法</para>
    /// <para>便于更快捷的开发</para>
    /// </summary>
    public static class Base64Linq
    {
        /// <summary>
        /// 将正常字符串转化为base64编码字符串
        /// </summary>
        /// <param name="str">需要转化的正常字符串</param>
        /// <returns></returns>
        public static string ToBase64(this string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 将base64编码字符串转化为正常字符串
        /// </summary>
        /// <param name="str">需要转化的base64字符串</param>
        /// <returns></returns>
        public static string FromBase64(this string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
    }
}
