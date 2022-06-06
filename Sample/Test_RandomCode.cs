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
            Log.Info(ES.Utils.Randomizer.Generate(32, ES.Utils.Randomizer.RandomCodeType.HighLowLetterAndNumberAndSymbol));
            Log.Info("HighLowLetterAndNumber:");
            Log.Info(ES.Utils.Randomizer.Generate(32, ES.Utils.Randomizer.RandomCodeType.HighLowLetterAndNumber));
            Log.Info("HighLetterAndNumber:");
            Log.Info(ES.Utils.Randomizer.Generate(32, ES.Utils.Randomizer.RandomCodeType.HighLetterAndNumber));
            Log.Info("HighLetter:");
            Log.Info(ES.Utils.Randomizer.Generate(32, ES.Utils.Randomizer.RandomCodeType.HighLetter));
            Log.Info("Number:");
            Log.Info(ES.Utils.Randomizer.Generate(32, ES.Utils.Randomizer.RandomCodeType.Number));

            Log.Info("Guid[no line]:");
            Log.Info(ES.Utils.Randomizer.GenerateGuid(false));
            Log.Info("Guid:");
            Log.Info(ES.Utils.Randomizer.GenerateGuid(true));
        }
    }
}
