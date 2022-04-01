using ES.Hotfix;
using System;
using System.Diagnostics;

namespace Sample
{
    class Test_Hotfix
    {
        public Test_Hotfix()
        {
            while (true)
            {
                // 普通测试
                TestHotfix();
                // 耗时测试
                // ConsumeTime();

                Log.Info($"Is First Load:{HotfixMgr.IsFirstLoad}");

                // 回车重载测试
                Console.ReadLine();
                Console.Clear();
            }
        }

        /// <summary>
        /// 测试只需要放入构造函数
        /// 热更测试
        /// </summary>
        public void TestHotfix()
        {
            Log.Info($"=======>>TestHotfix 1");
            HotfixMgr.Load("SampleDll", "SampleDll.Main", new string[] { "Hello World" }, "Main_Test");
            Log.Info($"=======>>TestHotfix 2");
        }

        /// <summary>
        /// 测试只需要放入构造函数
        /// 耗时测试
        /// </summary>
        public void ConsumeTime()
        {
            HotfixMgr.Load("SampleDll", "SampleDll.Main", null, "Main_Test1");
            Player player = new Player();
            Stopwatch watch = new Stopwatch();
            /* 性能测试 */
            // 第一次直接调用
            Log.Info("第一次直接调用开始~");
            watch.Reset();
            watch.Start();
            player.Test();
            watch.Stop();
            Log.Info($"第一次直接调用耗时1:{watch.Elapsed.TotalMilliseconds}ms");
            // 第一次实测热更调用
            Log.Info("\n\n热更调用开始~");
            watch.Reset();
            watch.Start();
            player.GetDynamicAgent().Test();
            watch.Stop();
            Log.Info($"第一次热更层耗时1:{watch.Elapsed.TotalMilliseconds}ms");
            // 第二次直接调用
            Log.Info("\n\n第二次直接调用开始~");
            watch.Reset();
            watch.Start();
            player.Test();
            watch.Stop();
            Log.Info($"第二次直接调用耗时2:{watch.Elapsed.TotalMilliseconds}ms");
            // 第二次实测热更调用
            Log.Info("\n\n热更调用开始~");
            watch.Reset();
            watch.Start();
            player.GetDynamicAgent().Test();
            watch.Stop();
            Log.Info($"第二次热更层耗时2:{watch.Elapsed.TotalMilliseconds}ms");
        }
    }

    /// <summary>
    /// 手动创建对应的代理
    /// 如果每次热更重载后不主动创建 则代理不会运作
    /// 也可以通过带参数构造函数来设定手动
    /// </summary>
    // [NotCreateAgent]
    public class Player : AgentData
    {
        public int count;
       
        // 用于测试 实际上一般数据层不写逻辑
        public void Test()
        {
            for (int i = 0; i < 10000000; i++) count++;
            Log.Info("直接调用计数:" + count);
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

    /// <summary>
    /// 抽象类使用热更新测试基类A
    /// </summary>
    public abstract class A : AgentData
    {
        public int test1 = 0;
        public string test2 = "hello";
    }

    /// <summary>
    /// 抽象类使用热更新测试继承类B
    /// </summary>
    public class B : A
    {
        public string test3 = "hello";
    }

    /// <summary>
    /// 抽象类使用热更新测试继承类C
    /// </summary>
    public class C : A
    {
        public string test4 = "hello";
    }
}
