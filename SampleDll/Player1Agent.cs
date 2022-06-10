using ES;
using ES.Hotfix;
using ES.Time;
using ES.Utils;
using Sample;

namespace SampleDll
{
    public class Player1Agent : Agent<Player1>, ITimeUpdate
    {
        public int copyCount = 0;
        public int period = 0;
        private readonly int seed = Randomizer.Random.Next(9999);

        protected override void Initialize()
        {
            // 两种相同作用
            Log.Info("IsFirstCreateAgent:" + self.IsFirstCreateAgent);
            Log.Info("isFirstCreate:" + IsFirstCreate);
            // 先处理代理数据构造函数，在处理代理构造
            Log.Info(self.test);
            TimeFlow.Create(this).Start();
        }

        public void Update(int deltaTime)
        {
            period += deltaTime;
            if (period >= 1000)
            {
                period -= 1000;
                Log.Info($"player1 count:{self.count++},copyCount:{copyCount++},seed:{seed}");
            }
        }

        public void UpdateEnd()
        {
        }
    }
}
