#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;
using System.Security.Cryptography;
using System.Text;

namespace ECSharp.Utils
{
    /// <summary>
    /// 随机器
    /// <para>用于生成指定长度的符号代码或者获取一个共享的随机器</para>
    /// <para>全局共享一个随机种子的随机器</para>
    /// </summary>
    public static class Randomizer
    {
        /// <summary>
        /// 随机器
        /// </summary>
        private static Random _rand = new Random();

        /// <summary>
        /// 随机器
        /// </summary>
        /// <returns></returns>
        public static Random Random => _rand;

        /// <summary>
        /// 重置随机器
        /// </summary>
        /// <param name="seed">随机种子</param>
        public static void Reset(int seed)
        {
            _rand = new Random(seed);
        }


        /// <summary>
        /// 随机字母类型
        /// </summary>
        public enum RandomCodeType
        {
            /// <summary>
            /// 大小写字母和数字和符号
            /// </summary>
            HighLowLetterAndNumberAndSymbol,
            /// <summary>
            /// 大小写字母和数字
            /// </summary>
            HighLowLetterAndNumber,
            /// <summary>
            /// 大写字母和数字
            /// </summary>
            HighLetterAndNumber,
            /// <summary>
            /// 大写字母
            /// </summary>
            HighLetter,
            /// <summary>
            /// 数字
            /// </summary>
            Number,
        }

        /// <summary>
        /// 符号库 大写小写字母数字特殊符号
        /// </summary>
        private readonly static char[] symbols = {
            '0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            '!','@','#','$','%','^','&','*'
        };

        /// <summary>
        /// 生成大小写字母和数字组合的字符串
        /// <para>默认随机为大小写和数字</para>
        /// </summary>
        /// <param name="len">生成长度</param>
        /// <param name="type">随机代码类型</param>
        /// <returns>生成的字符串</returns>
        public static string Generate(int len, RandomCodeType type = RandomCodeType.HighLowLetterAndNumber)
        {
            StringBuilder? newRandom = null;

            switch (type)
            {
                case RandomCodeType.HighLowLetterAndNumberAndSymbol:
                    newRandom = new StringBuilder(70);
                    for (int i = 0, arrlen = 70; i < len; i++)
                    {
                        newRandom.Append(symbols[_rand.Next(arrlen)]);
                    }
                    break;
                case RandomCodeType.HighLowLetterAndNumber:
                    newRandom = new StringBuilder(62);
                    for (int i = 0, arrlen = 62; i < len; i++)
                    {
                        newRandom.Append(symbols[_rand.Next(arrlen)]);
                    }
                    break;
                case RandomCodeType.HighLetterAndNumber:
                    newRandom = new StringBuilder(36);
                    for (int i = 0, arrlen = 36; i < len; i++)
                    {
                        newRandom.Append(symbols[_rand.Next(arrlen)]);
                    }
                    break;
                case RandomCodeType.HighLetter:
                    newRandom = new StringBuilder(26);
                    for (int i = 0, arrlen = 26; i < len; i++)
                    {
                        newRandom.Append(symbols[_rand.Next(arrlen) + 10]);
                    }
                    break;
                case RandomCodeType.Number:
                    newRandom = new StringBuilder(10);
                    for (int i = 0, arrlen = 10; i < len; i++)
                    {
                        newRandom.Append(symbols[_rand.Next(arrlen)]);
                    }
                    break;
            }
            return newRandom?.ToString() ?? "";
        }

        /// <summary>
        /// 生成唯一Guid
        /// <para>默认无横线 格式为32个字符</para>
        /// </summary>
        /// <param name="hasLine">是否需要分段横线 默认无横线</param>
        /// <returns></returns>
        public static string GenerateGuid(bool hasLine = false)
        {
            if (hasLine)
                return Guid.NewGuid().ToString();
            else
                return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 生成随机字节数组
        /// </summary>
        /// <param name="len">需要的长度</param>
        /// <param name="ultra">极限求值,具备更高的随机性</param>
        /// <returns></returns>
        public static byte[] GenerateBytes(int len, bool ultra = false)
        {
            byte[] bytes = new byte[len];

            if (ultra)
            {
                RandomNumberGenerator.Create().GetBytes(bytes);
            }
            else
            {
                _rand.NextBytes(bytes);
            }
            
            return bytes;
        }
    }
}
