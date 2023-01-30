using ECSharp;
using ECSharp.Hotfix;
using Sample;
using System;
using System.Diagnostics;

namespace SampleDll
{
    /// <summary>
    /// 热更测试DLL入口
    /// </summary>
    public class Main
    {
        static readonly Player player = AgentDataPivot.AddOrGetObject<Player>("player");
        static Player1 player1;

        static StructValue<int> test_1 = AgentDataPivot.AddOrGetStruct("test_1", 0);

        static B b;
        static C c;
        public static void Main_Test(string[] args)
        {
            Log.Info($"Input args:{args[0]}, test_1:{test_1.Value++}");

            player1 = AgentDataPivot.AddOrGetObject<Player1>("player1");
            // b = new B();
            // c = new C();
            // Test2(b, c);
        }

        public static void Main_Test1(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            // 可以利用拓展特性来实现不每次都书写泛型实现代理
            // player.GetAgent<PlayerAgent>().Test();
            // player.GetAgent().Test();

            watch.Reset();
            watch.Start();
            player.GetAgent().Test();
            watch.Stop();
            Log.Info($"内部第一次热更层耗时3:{watch.Elapsed.TotalMilliseconds}ms\n");
            watch.Reset();
            watch.Start();
            player.GetAgent().Test();
            watch.Stop();
            Log.Info($"内部第二次热更层耗时3:{watch.Elapsed.TotalMilliseconds}ms\n\n");
        }

        public static void Test2(A obj1, A obj2)
        {
            Log.Info($"=======>>Test2 1");
            var ss = obj1.GetAbstractAgent<A_Agent>();
            ss.WriteHelloA();
            obj2.GetAbstractAgent<A_Agent>().WriteHelloA();
            obj1.GetAbstractAgent<A_Agent>().Hello();
            obj2.GetAbstractAgent<A_Agent>().Hello();
            obj1.GetAgent<B_Agent>().Hello();
            obj2.GetAgent<C_Agent>().Hello();
            Log.Info($"=======>>Test2 2");
        }
    }

    /// <summary>
    /// 如果觉得每次调用都需要使用GetAgent的泛型来处理
    /// 那么可以针对需要大量调用的代理，在热更层写一个静态拓展来实现不用再写代理泛型的重复工作
    /// </summary>
    public static class AgentRegister 
    {
        /// <summary>
        /// PlayerAgent代理
        /// 这样只需要在这里写一次，以后就可以直接借助GetAgent()函数直接使用了
        /// </summary>
        public static PlayerAgent GetAgent(this Player self) => self.GetAgent<PlayerAgent>();
    }
}
