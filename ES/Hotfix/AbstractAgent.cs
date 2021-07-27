namespace ES.Hotfix
{
    /// <summary>
    /// 热更新抽象代理
    /// <para>普通代理类型请使用Agent类继承</para>
    /// <para>代理对象为抽象类时使用此类进行继承</para>
    /// <para>本类不可以直接使用self对象，需要使用_self函数获取</para>
    /// <para>使用抽象类还需要继承IAgent接口才可以使用完整功能</para>
    /// </summary>
    public abstract class AbstractAgent
    {
        /// <summary>
        /// 动态数据对象
        /// </summary>
        internal dynamic __self;

        /// <summary>
        /// 获取代理数据动态类型
        /// <para>一般配合IAgent接口使用</para>
        /// </summary>
#pragma warning disable IDE1006 // 命名样式
        protected dynamic _self => __self;
#pragma warning restore IDE1006 // 命名样式
    }
}
