#if !UNITY_2020_1_OR_NEWER
using Newtonsoft.Json.Linq;
using System.Data;

namespace ES.Database.Linq
{
    /// <summary>
    /// DataTable类型的配置基类
    /// <para>继承这个类，并且自定义数据表中各个字段，然后通过SetConfig进行绑定</para>
    /// </summary>
    public abstract class DataTableConfig : BaseConfigItem
    {
        /// <summary>
        /// 设置主键
        /// <para>此函数直接返回主键对象进行绑定</para>
        /// </summary>
        /// <param name="row">单个配置数据记录</param>
        /// <returns>返回主键对象</returns>
        protected abstract override void SetConfig(DataRow row);
        /// <summary>
        /// 设置配置
        /// <para>对需要的配置进行绑定对象</para>
        /// </summary>
        /// <param name="row">单个配置数据记录</param>
        protected abstract override object SetPrimaryKey(DataRow row);

        /// <summary>
        /// 当前抽象不使用
        /// </summary>
        /// <param name="token"></param>
        protected override void SetConfig(JToken token) { }
        /// <summary>
        /// 当前抽象不使用
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected override object SetPrimaryKey(JToken token) => new object();
    }
}
#endif