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
            player.GetAgent<PlayerAgent>().Test();
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
}
