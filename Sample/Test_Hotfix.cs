using ES.Hotfix;
using System;
using System.Diagnostics;

namespace Sample
{
    class Test_Hotfix
    {
        // 实际创建都需要先完成热更模块读取完成后执行
        Player player = new Player();
        Player1 player1;
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
                if(player1 == null) player1 = new Player1();
                Console.ReadLine();
                Console.Clear();
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

    /// <summary>
    /// 手动创建对应的代理
    /// 如果每次热更重载后不主动创建 则代理不会运作
    /// 也可以通过带参数构造函数来设定手动
    /// </summary>
    [NotCreateAgent]
    public class Player : AgentData
    {
        public int count;

        /// <summary>
        /// 通过base(false)设置手动创建
        /// 这样就不用通过 NotCreateAgent 特性来判断 二者选其一即可
        /// </summary>
        public Player() : base(false) { }
       
        // 用于测试 实际上一般数据层不写逻辑
        public void Test()
        {
            for (int i = 0; i < 1000000; i++) count++;
            Console.WriteLine("直接调用计数:" + count);
        }
    }

    /// <summary>
    /// 自动创建代理
    /// 并且添加 KeepAgentValue 特性实现代理内变量保存
    /// 如果去除 KeepAgentValue 特性则变量不会在重载后保存
    /// </summary>
    [KeepAgentValue]
    public class Player1 : AgentData
    {
        public int count;

        public string test;

        public Player1()
        {
            test = "Hello World";
        }
    }
}
