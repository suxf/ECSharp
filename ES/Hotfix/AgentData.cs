namespace ES.Hotfix
{
    /// <summary>
    /// 热更新代理数据
    /// <para>创建新的代理数据需要执行Initialize函数完成初始化</para>
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
        /// 创建代理数据
        /// </summary>
        public AgentData()
        {
            var type = GetType();
            if (type.IsDefined(typeof(NotCreateAgentAttribute), false))
                _ref = new AgentRef(null, false, null);
            else
                _ref = new AgentRef(type, type.IsDefined(typeof(KeepAgentValueAttribute), false), this);
            HotfixMgr.Instance.AddAgentRef(_ref);
            if (_ref.type != null) _ref.CreateAsyncAgent();
        }

        /// <summary>
        /// 创建代理数据
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
            if (_ref.type != null) _ref.CreateAsyncAgent();
        }

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <typeparam name="T">当前对象的代理类</typeparam>
        public T GetAgent<T>() where T : AbstractAgent, new()
        {
            _ref.CreateAgent<T>(this);
            return _ref._agent;
        }

        /// <summary>
        /// 获取抽象代理
        /// <para>获取抽象代理要注意在代理数据创建的一开始，请先调用一次GetAgent获取最初的代理来确保代理类正确创建</para>
        /// <para>否则直接在构建新代理数据的时候可能抽象代理的继承代理子类还未创建，如果是后续线程调用则没有问题</para>
        /// </summary>
        /// <typeparam name="T">当前对象的抽象代理类</typeparam>
        public T GetAbstractAgent<T>() where T : AbstractAgent
        {
            return _ref._agent;
        }
    }
}
