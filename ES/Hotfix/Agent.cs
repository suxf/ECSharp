namespace ES.Hotfix
{
    /// <summary>
    /// 热更代理
    /// <para>只有继承这个代理类才能在热更层使用其泛型的变量值</para>
    /// <para>继承此类使用数据层变量请用 self 代替 this </para>
    /// <para>代理类中函数如果存在委托情况，请注意如仅存于非热更层的委托可能存在问题</para>
    /// <para>此类事代理类，请不要使用代理类来直接声明变量或属性</para>
    /// <para>当然如果真的有需要，可以使用 AgentDataPivot 类来存储变量实现热更层声明变量</para>
    /// <para>另外虽然可以在热更层通过代理来直接传值达到既可以使用函数又可以读取值，但还是建议使用代理数据类来传递</para>
    /// <para>热更层可以继承时间流接口，每次热更重载后时间流重置</para>
    /// </summary>
    /// <typeparam name="T">代理数据类型</typeparam>
    public abstract class Agent<T> : BaseAgent where T : AgentData
    {
        /// <summary>
        /// 代理类对象
        /// <para>通过此对象可以获取代理数据的对象，相当于this的用法</para>
        /// </summary>
        public T self { get { return _self; } }
    }
}
