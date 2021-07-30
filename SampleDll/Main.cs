using ES.Hotfix;
using Sample;

namespace SampleDll
{
    /// <summary>
    /// 热更测试DLL入口
    /// </summary>
    public class Main
    {
        readonly Player player = AgentDataPivot.AddOrGetObject<Player>("player");
        public void Test()
        {
            // 可以利用拓展特性来实现不每次都书写泛型实现代理
            // player.GetAgent<PlayerAgent>().Test();
            player.GetAgent().Test();
        }

        public void Test2(A obj1, A obj2)
        {
            obj1.GetAbstractAgent<A_Agent>().WriteHelloA();
            obj2.GetAbstractAgent<A_Agent>().WriteHelloA();
            obj1.GetAbstractAgent<A_Agent>().Hello();
            obj2.GetAbstractAgent<A_Agent>().Hello();
            obj1.GetAgent<B_Agent>().Hello();
            obj2.GetAgent<C_Agent>().Hello();
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
