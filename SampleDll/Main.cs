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
    }
}
