using System;

namespace ES.Data.Database.SQLServer.Linq
{
    /// <summary>
    /// 数据实体
    /// <para>此类为了解决手动频繁装/卸箱操作设计</para>
    /// <para>DataAgentRow可以直接得到object，但是每次都需要强转值才行</para>
    /// <para>使用此类可以一次成型</para>
    /// </summary>
    public class DataEntity<T> where T : IComparable
    {
        /// <summary>
        /// 对象名字
        /// </summary>
        public string Name { get; private set; }

        internal DataEntityRow parent = null;

        /// <summary>
        /// 通过名字和数据记录得到一个数据对象
        /// </summary>
        /// <param name="name">数据记录中数据对象的名字</param>
        /// <param name="p">数据记录对象 弱对象</param>
        internal DataEntity(string name, DataEntityRow p)
        {
            Name = name;
            parent = p;
        }

        /// <summary>
        /// 值
        /// </summary>
        public T Value
        {
            get
            {
                return (T)parent[Name];
            }

            set
            {
                parent[Name] = value;
            }
        }
    }
}
