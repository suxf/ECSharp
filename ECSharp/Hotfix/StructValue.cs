#if !NET462 && !NETSTANDARD2_0
namespace ECSharp.Hotfix
{
    /// <summary>
    /// 结构体值保存对象
    /// <para>用于保存和使用结构体类型的数据</para>
    /// </summary>
    public sealed class StructValue<T> : BaseStructValue where T : struct
    {
        /// <summary>
        /// 结构体值
        /// </summary>
        public T Value { get { return (T)_value!; } set { _value = value; } }
    }
}
#endif