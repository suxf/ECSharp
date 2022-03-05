using ES.Time;
using ES.Hotfix;
using Sample;
using System;
using System.Diagnostics;

namespace SampleDll
{
    public class PlayerAgent : Agent<Player>, ITimeUpdate
    {
        public TimeFlow tf;

        protected override void Initialize()
        {
            // tf = TimeFlow.Create(this);
            // tf.Start();
        }

        public void Test()
        {
            // Console.WriteLine(self.name);
            Stopwatch watch = new Stopwatch();
            /* 性能测试 */
            // 第一次直接调用
            watch.Start();
            for (int i = 0; i < 10000000; i++) Add();// { self.count++; }
            watch.Stop();
            Console.WriteLine($"热更层循环耗时:{watch.Elapsed.TotalMilliseconds}ms");
            // for (int i = 0; i < 1000000; i++) self.count++;
            Console.WriteLine("热更层计数:" + self.count);
        }

        private void Add()
        {
            self.count++;
        }

        int count = 0;
        public void Update(int deltaTime)
        {
            if (count % 1000 == 0) Console.WriteLine($"player count:{self.count++},copyCount:{count}");
            count += deltaTime;
        }

        public void UpdateEnd()
        {
        }
    }
}
