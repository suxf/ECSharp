#if !NET462 && !NETSTANDARD2_0
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECSharp.Hotfix
{
    /// <summary>
    /// 热更新代理引用器
    /// </summary>
    internal class AgentRef
    {
        /// <summary>
        /// 代理索引是否被创建
        /// </summary>
        internal bool isCreated = false;
        /// <summary>
        /// 代理
        /// <para>通过代理可以执行关于类的函数</para>
        /// </summary>
        internal AbstractAgent? _agent;
        /// <summary>
        /// 代理数据类型
        /// </summary>
        private readonly Type? type;
        /// <summary>
        /// 自动创建
        /// </summary>
        internal bool isAutoCreate;
        /// <summary>
        /// 是否为第一次创建代理
        /// </summary>
        internal bool isFirstCreateAgent = true;
        /// <summary>
        /// 是否拷贝值
        /// </summary>
        private readonly bool isCopyValue = false;
        /// <summary>
        /// 代理数据对象
        /// </summary>
        private readonly AgentData? agentData;
        /// <summary>
        /// 读写锁
        /// </summary>
        private readonly object m_lock = new object();
        /// <summary>
        /// 创建代理次数
        /// </summary>
        private int createAgentCount = 0;

        /// <summary>
        /// 构建代理索引
        /// </summary>
        internal AgentRef(Type? type, bool isCopyValue, AgentData? agentData)
        {
            this.type = type;
            isAutoCreate = type != null;
            this.isCopyValue = isCopyValue;
            this.agentData = agentData;
        }

        /// <summary>
        /// 异步创建代理
        /// </summary>
        internal void CreateAsyncAgent()
        {
            if (isAutoCreate)
                Task.Run(CreateAgent);
        }

        /// <summary>
        /// 创建代理
        /// </summary>
        internal void CreateAgent<T>(AgentData data) where T : AbstractAgent, new()
        {
            if (!isCreated)
            {
                lock (m_lock)
                {
                    if (isCreated) return;

                    isCreated = true;

                    // 修改第一次创建状态标记
                    if (++createAgentCount >= 2)
                        isFirstCreateAgent = false;

                    Interlocked.Exchange(ref _agent, new T() { __self = data });
                    _agent.InitializeES(isFirstCreateAgent);
                }
            }
        }

        /// <summary>
        /// 创建代理
        /// </summary>
        internal void CreateAgent()
        {
            if (!isCreated && type != null && HotfixMgr.agentTypeMap.TryGetValue(type, out var agentType))
            {
                lock (m_lock)
                {
                    if (isCreated) return;

                    isCreated = true;

                    // 修改第一次创建状态标记
                    if (++createAgentCount >= 2)
                        isFirstCreateAgent = false;

                    // 此处有问题带有一个参数的构造函数无法正确使用GetAgent 暂时舍弃 后期有其他方案再修复
                    // object newAgent = null;
                    // var constructors = agentType.GetConstructors();
                    // for (int i = 0, len = constructors.Length; i < len; i++)
                    // {
                    //     var constructor = constructors[i];
                    //     var parameters = constructor.GetParameters();
                    //     if (parameters.Length == 1 && parameters[0].ParameterType == type)
                    //         newAgent = Activator.CreateInstance(agentType, agentData);
                    //     else
                    //         newAgent = Activator.CreateInstance(agentType);
                    // }
                    var newAgent = Activator.CreateInstance(agentType) as AbstractAgent;
                    if (newAgent != null)
                    {
                        newAgent.__self = agentData;
                        newAgent.InitializeES(isFirstCreateAgent);
                    }

                    // 处理值拷贝
                    if (_agent != null && isCopyValue)
                    {
                        var oldAgentType = _agent.GetType();
                        var fields = agentType.GetFields();
                        for (int i = 0, len = fields.Length; i < len; i++)
                        {
                            var newField = fields[i];
                            var oldField = oldAgentType.GetField(newField.Name);

                            if (newField.GetType() == oldField?.GetType() && !newField.IsInitOnly)
                                newField.SetValue(newAgent, oldField.GetValue(_agent));
                        }

                        var properties = agentType.GetProperties();
                        for (int i = 0, len = properties.Length; i < len; i++)
                        {
                            var newProperty = properties[i];
                            var oldProperty = oldAgentType.GetProperty(newProperty.Name);

                            if (newProperty.GetType() == oldProperty?.GetType() && newProperty.CanWrite)
                                newProperty.SetValue(newAgent, oldProperty.GetValue(_agent));
                        }
                    }

                    // 替换代理
                    Interlocked.Exchange(ref _agent, newAgent);
                }
            }
        }
    }
}
#endif