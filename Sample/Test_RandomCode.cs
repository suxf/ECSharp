using ES.Common.Utils;
using System;

namespace Sample
{
    /// <summary>
    /// 随机码 事例
    /// </summary>
    class Test_RandomCode
    {
        public Test_RandomCode()
        {
            Console.WriteLine("HighLowLetterAndNumberAndSymbol:");
            Console.WriteLine(RandomCode.Generate(32, RandomCode.RandomCodeType.HighLowLetterAndNumberAndSymbol));
            Console.WriteLine("HighLowLetterAndNumber:");
            Console.WriteLine(RandomCode.Generate(32, RandomCode.RandomCodeType.HighLowLetterAndNumber));
            Console.WriteLine("HighLetterAndNumber:");
            Console.WriteLine(RandomCode.Generate(32, RandomCode.RandomCodeType.HighLetterAndNumber));
            Console.WriteLine("HighLetter:");
            Console.WriteLine(RandomCode.Generate(32, RandomCode.RandomCodeType.HighLetter));
            Console.WriteLine("Number:");
            Console.WriteLine(RandomCode.Generate(32, RandomCode.RandomCodeType.Number));

            Console.WriteLine("Guid[no line]:");
            Console.WriteLine(RandomCode.GenerateGuid(false));
            Console.WriteLine("Guid:");
            Console.WriteLine(RandomCode.GenerateGuid(true));
        }
    }
}
