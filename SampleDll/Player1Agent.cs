using ES.Common.Time;
using ES.Hotfix;
using Sample;
using System;

namespace SampleDll
{
    public class Player1Agent : Agent<Player1>, ITimeUpdate
    {
        public int copyCount = 0;

        public Player1Agent(Player1 self)
        {
            // 先处理代理数据构造函数，在处理代理构造
            Console.WriteLine(self.test);
            TimeFlow.Create(this).Start();
        }

        public void Update(int deltaTime)
        {
            if (copyCount % 1000 == 0) Console.WriteLine($"player1 count:{self.count++},copyCount:{copyCount}");
            copyCount += deltaTime;
        }

        public void UpdateEnd()
        {
        }
    }
}
