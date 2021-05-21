using System.Data;

namespace ES.Data.Database.SQLServer
{
    /// <summary>
    /// SQL语句执行对象结果
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// 单表执行影响行数
        /// <para>此值大于等于0的情况才代表执行成功，大于0的情况表示对数据有记录或影响</para>
        /// </summary>
        public int effectNum { internal set; get; }
        /// <summary>
        /// 执行数据合集
        /// </summary>
        public DataRowCollection collection { internal set; get; }
        /// <summary>
        /// 数据总合集
        /// </summary>
        public DataSet dataSet { internal set; get; }
    }
}
