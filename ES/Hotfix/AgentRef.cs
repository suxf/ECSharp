using System;
using System.Threading;

namespace ES.Hotfix
{
    /// <summary>
    /// 代理引用器
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
        internal dynamic _agent;
        /// <summary>
        /// 代理数据类型
        /// </summary>
        internal readonly Type type;
        /// <summary>
        /// 是否拷贝值
        /// </summary>
        private readonly bool isCopyValue = false;
        /// <summary>
        /// 代理数据对象
        /// </summary>
        private readonly AgentData agentData;

        /// <summary>
        /// 构建代理索引
        /// </summary>
        internal AgentRef(Type type, bool isCopyValue, AgentData agentData)
        {
            this.type = type;
            this.isCopyValue = isCopyValue;
            this.agentData = agentData;
        }

        /// <summary>
        /// 创建代理
        /// </summary>
        internal void CreateAgent()
        {
            if (!isCreated && HotfixMgr.Instance.agentTypeMap.TryGetValue(type, out var agentType))
            {
                isCreated = true;
                var newAgent = Activator.CreateInstance(agentType);
                (newAgent as BaseAgent)._self = agentData;
                // 处理值拷贝
                if (_agent != null && isCopyValue)
                {
                    var oldAgentType = _agent.GetType();
                    var fields = agentType.GetFields();
                    for (int i = 0, len = fields.Length; i < len; i++)
                    {
                        var newField = fields[i];
                        var oldField = oldAgentType.GetField(newField.Name);
                        if(newField.GetType() == oldField.GetType()) 
                            newField.SetValue(newAgent, oldField.GetValue(_agent));
                    }
                    var properties = agentType.GetProperties();
                    for (int i = 0, len = properties.Length; i < len; i++)
                    {
                        var newProperty = properties[i];
                        var oldProperty = oldAgentType.GetProperty(newProperty.Name);
                        if (newProperty.GetType() == oldProperty.GetType())
                            newProperty.SetValue(newAgent, oldProperty.GetValue(_agent));
                    }
                }
                // 替换代理
                Interlocked.Exchange(ref _agent, newAgent);
            }
        }
    }
}
