using System;

namespace ES.Hotfix
{
    /// <summary>
    /// 阻止代理自动化生成
    /// <para>添加到对应 AgentData 继承类，就会将与之对应的代理转为手动创建</para>
    /// </summary>
    public class NotCreateAgentAttribute : Attribute
    {
    }
}
