﻿using ES.Time;
using ES.Hotfix;
using Sample;
using System;

namespace SampleDll
{
    public class Player1Agent : Agent<Player1>, ITimeUpdate
    {
        public int copyCount = 0;
        private readonly int seed = new Random().Next(9999);

        protected override void Initialize()
        {
            // 两种相同作用
            Console.WriteLine("IsFirstCreateAgent:" + self.IsFirstCreateAgent);
            Console.WriteLine("isFirstCreate:" + IsFirstCreate);
            // 先处理代理数据构造函数，在处理代理构造
            Console.WriteLine(self.test);
            TimeFlow.Create(this).Start();
        }

        public void Update(int deltaTime)
        {
            if (copyCount % 1000 == 0) Console.WriteLine($"player1 count:{self.count++},copyCount:{copyCount},seed:{seed}");
            copyCount += deltaTime;
        }

        public void UpdateEnd()
        {
        }
    }
}
