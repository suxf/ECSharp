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
    }
}
