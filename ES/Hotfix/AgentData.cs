using System.Threading;
using System.Threading.Tasks;

namespace ES.Hotfix
{
    /// <summary>
    /// 代理数据
    /// <para>只有通过继承代理数据的类才能使用热更代理层函数</para>
    /// <para>此类事代理数据类，代理类的变量或属性需要从此声明来使用</para>
    /// <para>需要在热更层使用的变量或者属性需要使用 public 访问修饰符</para>
    /// <para>当存在对应关系的代理 Agent 继承对象时，会在构建代理数据 AgentData 时自动构建 Agent 继承对象</para>
    /// <para>当然如果不要自动构建可以在 AgentData 继承类添加 [NotCreateAgent] 特性, 即可手动通过 GetAgent 函数创建</para>
    /// <para>如果确实需要在热更层新建字段或属性，并且希望可以重载之后值也可以保留，那么可以使用 [CopyAgentValue] 特性</para>
    /// </summary>
    public abstract class AgentData
    {
        /// <summary>
        /// 代理引用
        /// </summary>
        private readonly AgentRef _ref;

        /// <summary>
        /// 构建代理数据
        /// </summary>
        public AgentData()
        {
            var type = GetType();
            if (type.IsDefined(typeof(NotCreateAgentAttribute), false))
                _ref = new AgentRef(null, false, null);
            else
                _ref = new AgentRef(type, type.IsDefined(typeof(KeepAgentValueAttribute), false), this);
            HotfixMgr.Instance.AddAgentRef(_ref);
            // 如果不为空 就创建
            if (_ref.type != null) Task.Run(_ref.CreateAgent);
        }

        /// <summary>
        /// 构建代理数据
        /// </summary>
        /// <param name="isAutoCreate">是否自动创建代理</param>
        public AgentData(bool isAutoCreate)
        {
            if (isAutoCreate)
            {
                var type = GetType();
                _ref = new AgentRef(type, type.IsDefined(typeof(KeepAgentValueAttribute), false), this);
            }
            else _ref = new AgentRef(null, false, null);
            HotfixMgr.Instance.AddAgentRef(_ref);
            // 如果不为空 就创建
            if (_ref.type != null) Task.Run(_ref.CreateAgent);
        }

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <typeparam name="T">当前对象的代理类</typeparam>
        public T GetAgent<T>() where T : BaseAgent, new() 
        {
            if (!_ref.isCreated)
            {
                _ref.isCreated = true;
                Interlocked.Exchange(ref _ref._agent, new T() { _self = this });
            }
            return _ref._agent;
        }
    }
}
