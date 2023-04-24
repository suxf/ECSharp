#if !NET462 && !NETSTANDARD2_0
namespace ECSharp.Hotfix
{
    /// <summary>
    /// 热更新抽象代理
    /// <para>普通代理类型请使用Agent类继承</para>
    /// <para>代理对象为抽象类时使用此类进行继承</para>
    /// <para>不要在代理类使用构造函数，请使用Initialize来初始化一些内容</para>
    /// <para>本类不可以直接使用self对象，需要使用_self函数获取</para>
    /// <para>使用抽象类还需要继承IAgent接口才可以使用完整功能</para>
    /// <para>其它功能参考Agent类</para>
    /// </summary>
    public abstract class AbstractAgent
    {
        /// <summary>
        /// 是否是第一次代理创建标记
        /// </summary>
        protected bool IsFirstCreate { get; private set; }

        /// <summary>
        /// 动态数据对象
        /// </summary>
        internal AgentData? _self;

        /// <summary>
        /// 动态数据对象
        /// </summary>
        public virtual AgentData self => _self!;

        /// <summary>
        /// 初始化函数
        /// <para>代理创建后会主动调用此函数</para>
        /// <para>不要对代理使用构造函数</para>
        /// <para>可以通过 IsFirstCreate 来检测代理是否第一次创建</para>
        /// <para>需要注意的是此函数不会主动执行父类的 Initialize 函数</para>
        /// <para>初始化父类函数需要在每个子类 Initialize 函数 首行加入 base.Initialize(); </para>
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// 初始化函数 内部调用
        /// </summary>
        /// <param name="isFirstCreate"></param>
        internal void InitializeES(bool isFirstCreate)
        {
            IsFirstCreate = isFirstCreate;
            Initialize();
        }
    }
}
#endif