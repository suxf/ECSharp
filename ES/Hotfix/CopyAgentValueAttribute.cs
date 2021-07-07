using System;

namespace ES.Hotfix
{
    /// <summary>
    /// 拷贝代理值
    /// <para>添加到对应 AgentData 继承类，在自动代理模式下可以开启代理类值通过反射拷贝</para>
    /// <para>此处使用反射，在对象庞大的类，还是建议不要使用这个特性</para>
    /// </summary>
    public class CopyAgentValueAttribute : Attribute
    {
    }
}
