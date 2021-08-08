namespace ES.Hotfix
{
    /// <summary>
    /// 热更新代理数据
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
        /// 是否为第一次创建代理
        /// <para>用于代理在第一次同时和代理数据创建时不需要处理的内容作为提示</para>
        /// <para>第一次调用代理构造函数此值为true，之后每次调用都为false</para>
        /// </summary>
        public bool IsFirstCreateAgent => _ref.isFirstCreateAgent;

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
            HotfixMgr.AddAgentRef(_ref);
            _ref.CreateAsyncAgent();
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
            HotfixMgr.AddAgentRef(_ref);
            _ref.CreateAsyncAgent();
        }

        /// <summary>
        /// 手动创建自动代理
        /// <para>使用条件需要满足是自动创建代理的代理数据类才能成功创建</para>
        /// </summary>
        private void CreateAgent()
        {
            if (_ref.isAutoCreate) _ref.CreateAgent();
        }

        /// <summary>
        /// 获取动态类型代理
        /// <para>此函数主要用于支持数据层能够动态调用热更层函数</para>
        /// <para>热更层可以用GetAgent泛型来直接获取代理目标对象</para>
        /// </summary>
        public dynamic GetDynamicAgent()
        {
            CreateAgent();
            return _ref._agent;
        }

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <typeparam name="T">当前对象的代理类</typeparam>
        public T GetAgent<T>() where T : AbstractAgent, new()
        {
            _ref.CreateAgent<T>(this);
            return _ref._agent as T;
        }

        /// <summary>
        /// 获取抽象代理
        /// </summary>
        /// <typeparam name="T">当前对象的抽象代理类</typeparam>
        public T GetAbstractAgent<T>() where T : AbstractAgent
        {
            CreateAgent();
            return _ref._agent as T;
        }
    }
}
