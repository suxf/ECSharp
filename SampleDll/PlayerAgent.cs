﻿using ES.Common.Time;
using ES.Hotfix;
using Sample;
using System;
using System.Diagnostics;

namespace SampleDll
{
    public class PlayerAgent : Agent<Player>, ITimeUpdate
    {
        public TimeFlow tf;

        public PlayerAgent()
        {
            tf = TimeFlow.Create(this);
            tf.Start();
        }

        public void Test()
        {
            // Console.WriteLine(self.name);
            Stopwatch watch = new Stopwatch();
            /* 性能测试 */
            // 第一次直接调用
            watch.Start();
            for (int i = 0; i < 1000000; i++) self.count++;
            watch.Stop();
            Console.WriteLine("热更层循环耗时:" + watch.ElapsedMilliseconds);
            // for (int i = 0; i < 1000000; i++) self.count++;
            Console.WriteLine("热更层计数:" + self.count);
        }

        int count = 0;
        public void Update(int deltaTime)
        {
            if (count % 1000 == 0) Console.WriteLine("1:" + count);
            count += TimeFlow.period;
        }

        public void UpdateEnd()
        {
        }
    }
}
