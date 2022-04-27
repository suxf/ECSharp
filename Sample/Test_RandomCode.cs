using ES;
using ES.Utils;
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
            Log.Info("HighLowLetterAndNumberAndSymbol:");
            Log.Info(RandomCode.Generate(32, RandomCode.RandomCodeType.HighLowLetterAndNumberAndSymbol));
            Log.Info("HighLowLetterAndNumber:");
            Log.Info(RandomCode.Generate(32, RandomCode.RandomCodeType.HighLowLetterAndNumber));
            Log.Info("HighLetterAndNumber:");
            Log.Info(RandomCode.Generate(32, RandomCode.RandomCodeType.HighLetterAndNumber));
            Log.Info("HighLetter:");
            Log.Info(RandomCode.Generate(32, RandomCode.RandomCodeType.HighLetter));
            Log.Info("Number:");
            Log.Info(RandomCode.Generate(32, RandomCode.RandomCodeType.Number));

            Log.Info("Guid[no line]:");
            Log.Info(RandomCode.GenerateGuid(false));
            Log.Info("Guid:");
            Log.Info(RandomCode.GenerateGuid(true));
        }
    }
}
