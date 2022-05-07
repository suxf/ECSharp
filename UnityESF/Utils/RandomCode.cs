using System;
using System.Text;

namespace ES.Utils
{
    /// <summary>
    /// 随机码
    /// <para>用于生成指定长度的符号代码</para>
    /// </summary>
    public static class RandomCode
    {
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
        private readonly static char[] charHighLowLetterAndNumberAndSymbol = {
            '0','1','2','3','4','5','6','7','8','9',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            '!','@','#','$','%','^','&','*'
        };

        /// <summary>
        /// 符号库 大写小写字母数字
        /// </summary>
        private readonly static char[] charHighLowLetterAndNumber = {
            '0','1','2','3','4','5','6','7','8','9',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };

        /// <summary>
        /// 符号库 大写字母数字
        /// </summary>
        private readonly static char[] charHighLetterAndNumber = {
            '0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };

        /// <summary>
        /// 符号库 大写字母
        /// </summary>
        private readonly static char[] charHighLetter = {
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };

        /// <summary>
        /// 符号库 数字
        /// </summary>
        private readonly static char[] charNumber = {
            '0','1','2','3','4','5','6','7','8','9'
        };

        /// <summary>
        /// 随机器
        /// </summary>
        private readonly static Random random = new Random();

        /// <summary>
        /// 生成大小写字母和数字组合的字符串
        /// <para>默认随机为大小写和数字</para>
        /// </summary>
        /// <param name="len">生成长度</param>
        /// <param name="type">随机代码类型</param>
        /// <param name="seed">随机种子</param>
        /// <returns>生成的字符串</returns>
        public static string Generate(int len, RandomCodeType type = RandomCodeType.HighLowLetterAndNumber, int? seed = null)
        {
            StringBuilder? newRandom = null;
            var rd = random;
            if (seed != null) rd = new Random((int)seed);
            switch (type)
            {
                case RandomCodeType.HighLowLetterAndNumberAndSymbol:
                    newRandom = new StringBuilder(charHighLowLetterAndNumberAndSymbol.Length);
                    for (int i = 0, arrlen = charHighLowLetterAndNumberAndSymbol.Length; i < len; i++) newRandom.Append(charHighLowLetterAndNumberAndSymbol[rd.Next(arrlen)]);
                    break;
                case RandomCodeType.HighLowLetterAndNumber:
                    newRandom = new StringBuilder(charHighLowLetterAndNumber.Length);
                    for (int i = 0, arrlen = charHighLowLetterAndNumber.Length; i < len; i++) newRandom.Append(charHighLowLetterAndNumber[rd.Next(arrlen)]);
                    break;
                case RandomCodeType.HighLetterAndNumber:
                    newRandom = new StringBuilder(charHighLetterAndNumber.Length);
                    for (int i = 0, arrlen = charHighLetterAndNumber.Length; i < len; i++) newRandom.Append(charHighLetterAndNumber[rd.Next(arrlen)]);
                    break;
                case RandomCodeType.HighLetter:
                    newRandom = new StringBuilder(charHighLetter.Length);
                    for (int i = 0, arrlen = charHighLetter.Length; i < len; i++) newRandom.Append(charHighLetter[rd.Next(arrlen)]);
                    break;
                case RandomCodeType.Number:
                    newRandom = new StringBuilder(charNumber.Length);
                    for (int i = 0, arrlen = charNumber.Length; i < len; i++) newRandom.Append(charNumber[rd.Next(arrlen)]);
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
            if (hasLine) return Guid.NewGuid().ToString();
            else return Guid.NewGuid().ToString("N");
        }
    }
}
