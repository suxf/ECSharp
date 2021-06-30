using ES.Hotfix;
using System;
using System.Diagnostics;

namespace Sample
{
    class Test_Hotfix
    {
        readonly Player player = new Player();
        public Test_Hotfix()
        {
            TestHotfix();
        }

        /// <summary>
        /// 测试只需要放入构造函数
        /// 热更测试
        /// </summary>
        public void TestHotfix()
        {
            while (true)
            {
                HotfixMgr.Instance.Load("SampleDll", "SampleDll.Main");
                HotfixMgr.Instance.Agent.Test();
                Console.ReadLine();
                Console.Clear();
                GC.Collect();
            }
        }

        /// <summary>
        /// 测试只需要放入构造函数
        /// 耗时测试
        /// </summary>
        public void ConsumeTime()
        {
            HotfixMgr.Instance.Load("SampleDll", "SampleDll.Main");
            Stopwatch watch = new Stopwatch();
            /* 性能测试 */
            // 第一次直接调用
            Console.WriteLine("直接调用开始~");
            watch.Reset();
            watch.Start();
            player.Test();
            watch.Stop();
            Console.WriteLine("直接调用耗时1:" + watch.ElapsedMilliseconds);
            // 第一次实测热更调用
            Console.WriteLine("\n\n热更调用开始~");
            watch.Reset();
            watch.Start();
            HotfixMgr.Instance.Agent.Test();
            watch.Stop();
            Console.WriteLine("热更层耗时1:" + watch.ElapsedMilliseconds);
            // 第二次直接调用
            Console.WriteLine("\n\n直接调用开始~");
            watch.Reset();
            watch.Start();
            player.Test();
            watch.Stop();
            Console.WriteLine("直接调用耗时2:" + watch.ElapsedMilliseconds);
            // 第二次实测热更调用
            Console.WriteLine("\n\n热更调用开始~");
            watch.Reset();
            watch.Start();
            HotfixMgr.Instance.Agent.Test();
            watch.Stop();
            Console.WriteLine("热更层耗时2:" + watch.ElapsedMilliseconds);
        }
    }

    public class Player : AgentData
    {
        public int count;
       
        // 用于测试 实际上一般数据层不写逻辑
        public void Test()
        {
            for (int i = 0; i < 1000000; i++) count++;
            Console.WriteLine("直接调用计数:" + count);
        }
    }
}
