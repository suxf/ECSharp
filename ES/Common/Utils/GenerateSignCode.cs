using System;
using System.Text;

namespace ES.Common.Utils
{
    /// <summary>
    /// 生成符号代码
    /// 用于生成指定长度的符号代码
    /// </summary>
    public static class GenerateSignCode
    {
        /// <summary>
        /// 符号库
        /// </summary>
        private readonly static char[] characteristic = {
            '0','1','2','3','4','5','6','7','8','9',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };

        /// <summary>
        /// 生成大小写字母和数字组合的字符串
        /// </summary>
        /// <param name="len">生成长度</param>
        /// <returns>生成的字符串</returns>
        public static string Generate(int len)
        {
            StringBuilder newRandom = new StringBuilder(characteristic.Length);
            Random rd = new Random();
            for (int i = 0; i < len; i++)
            {
                newRandom.Append(characteristic[rd.Next(characteristic.Length)]);
            }
            return newRandom.ToString();
        }
    }
}
