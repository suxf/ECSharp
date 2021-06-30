using System.Threading;

namespace ES.Hotfix
{
    /// <summary>
    /// 代理数据
    /// <para>只有通过继承代理数据的类才能使用热更代理层函数</para>
    /// <para>此类事代理数据类，代理类的变量或属性需要从此声明来使用</para>
    /// <para>需要在热更层使用的变量或者属性需要使用 public 访问修饰符</para>
    /// <para>请不要在数据层继承对象使用时间流接口调用热更层函数</para>
    /// </summary>
    public abstract class AgentData
    {
        /// <summary>
        /// 代理引用
        /// </summary>
        private readonly AgentRef _ref = new AgentRef();

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <typeparam name="T">当前对象的代理类</typeparam>
        public T GetAgent<T>() where T : BaseAgent, new() 
        {
            if (!_ref.isCreated)
            {
                _ref.isCreated = true;
                HotfixMgr.Instance.AddAgentRef(_ref);
                Interlocked.Exchange(ref _ref._agent, new T() { _self = this });
            }
            return _ref._agent;
        }
    }
}
