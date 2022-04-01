#if !UNITY_2020_1_OR_NEWER
namespace ES.Hotfix
{
    /// <summary>
    /// 代理接口
    /// <para>当使用抽象代理AbstractAgent时候，需要添加此接口来约束代理数据</para>
    /// </summary>
    /// <typeparam name="T">当前代理类的代理数据类型</typeparam>
    public interface IAgent<T> where T : AgentData
    {
        /// <summary>
        /// self对象
        /// <para>在抽象代理中想要获取代理数据对象需要使用此函数</para>
        /// <para>一般情况下函数实现: public [new] T self => _self as T;</para>
        /// <para>由于抽象继承所以子类需要用new隐藏父类self</para>
        /// </summary>
#pragma warning disable IDE1006 // 命名样式
        T self { get; }
#pragma warning restore IDE1006 // 命名样式
    }
}
#endif