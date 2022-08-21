using ECSharp;
using ECSharp.Utils;
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
            Log.Info(Randomizer.Generate(32, Randomizer.RandomCodeType.HighLowLetterAndNumberAndSymbol));
            Log.Info("HighLowLetterAndNumber:");
            Log.Info(Randomizer.Generate(32, Randomizer.RandomCodeType.HighLowLetterAndNumber));
            Log.Info("HighLetterAndNumber:");
            Log.Info(Randomizer.Generate(32, Randomizer.RandomCodeType.HighLetterAndNumber));
            Log.Info("HighLetter:");
            Log.Info(Randomizer.Generate(32, Randomizer.RandomCodeType.HighLetter));
            Log.Info("Number:");
            Log.Info(Randomizer.Generate(32, Randomizer.RandomCodeType.Number));

            Log.Info("Guid[no line]:");
            Log.Info(Randomizer.GenerateGuid(false));
            Log.Info("Guid:");
            Log.Info(Randomizer.GenerateGuid(true));
        }
    }
}
