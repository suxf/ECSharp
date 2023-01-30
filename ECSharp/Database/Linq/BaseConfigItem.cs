#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using Newtonsoft.Json.Linq;
using System.Data;

namespace ECSharp.Database.Linq
{
    /// <summary>
    /// 基础配置加载器单个配置
    /// <para>DataTable类型继承DataTableConfig</para>
    /// <para>Json类型继承JsonConfig</para>
    /// </summary>
    public abstract class BaseConfigItem
    {
        /// <summary>
        /// 主键 内部变量
        /// <para>对某个字段值进行绑定</para>
        /// </summary>
        internal string PrimaryKey { private set; get; } = "";

        /// <summary>
        /// 设置主键
        /// </summary>
        internal void SetESPrimaryKey(DataRow row)
        {
            PrimaryKey = SetPrimaryKey(row).ToString() ?? "";
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        internal void SetESConfig(DataRow row)
        {
            SetConfig(row);
        }

        /// <summary>
        /// 设置主键
        /// </summary>
        internal void SetESPrimaryKey(JToken token)
        {
            PrimaryKey = SetPrimaryKey(token).ToString() ?? "";
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        internal void SetESConfig(JToken token)
        {
            SetConfig(token);
        }

        /// <summary>
        /// 设置主键
        /// <para>此函数直接返回主键对象进行绑定</para>
        /// </summary>
        /// <param name="row">单个配置数据记录</param>
        /// <returns>返回主键对象</returns>
        protected abstract object SetPrimaryKey(DataRow row);

        /// <summary>
        /// 设置配置
        /// <para>对需要的配置进行绑定对象</para>
        /// </summary>
        /// <param name="row">单个配置数据记录</param>
        protected abstract void SetConfig(DataRow row);

        /// <summary>
        /// 设置主键
        /// <para>此函数直接返回主键对象进行绑定</para>
        /// </summary>
        /// <param name="token">单个配置数据记录</param>
        /// <returns>返回主键对象</returns>
        protected abstract object SetPrimaryKey(JToken token);

        /// <summary>
        /// 设置配置
        /// <para>对需要的配置进行绑定对象</para>
        /// </summary>
        /// <param name="token">单个配置数据记录</param>
        protected abstract void SetConfig(JToken token);
    }
}
