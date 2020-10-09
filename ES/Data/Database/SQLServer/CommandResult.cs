using System.Data;

namespace ES.Data.Database.SQLServer
{
    /// <summary>
    /// SQL语句执行对象结果
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// 影响行数
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

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool isCompleted { internal set; get; }
    }
}
