using System.Data;

namespace ES.Data.Database.SQLServer.Linq
{
    /// <summary>
    /// SQLServer基础配置加载器
    /// </summary>
    public class ConfigLoader<T> where T : ConfigLoaderItem, new()
    {
        private readonly string sql = null;
        private readonly SQLServerDBHelper dBHelper = null;
        /// <summary>
        /// 配置集合
        /// </summary>
        public T[] Configs { get; private set; }

        /// <summary>
        /// 加载器构造函数
        /// 此操作是利用sql查询到结果然后进行绑定
        /// </summary>
        /// <param name="dBHelper">数据库连接对象</param>
        /// <param name="sql">需要查询的语句</param>
        public ConfigLoader(SQLServerDBHelper dBHelper, string sql)
        {
            this.dBHelper = dBHelper;
            this.sql = sql;
            Reload();
        }

        /// <summary>
        /// 查找主键所对应的值
        /// </summary>
        /// <param name="value">对象值</param>
        /// <returns></returns>
        public T Find(object value)
        {
            string cval = value.ToString();
            for (int i = 0, len = Configs.Length; i < len; i++)
            {
                if (cval == Configs[i].___PrimaryKey) return Configs[i];
            }
            return default;
        }

        /// <summary>
        /// 重新读取配置
        /// </summary>
        public void Reload()
        {
            var result = dBHelper.CommandSQL(sql);
            if(result.effectNum >= 0)
            {
                Configs = new T[result.collection.Count];
                int i = 0;
                foreach (DataRow item in result.collection)
                {
                    var temp = Configs[i++] = new T();
                    temp.SetESPrimaryKey(item);
                    temp.SetESConfig(item);
                }
            }
        }
    }
}
