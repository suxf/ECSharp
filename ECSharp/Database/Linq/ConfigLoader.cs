#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace ECSharp.Database.Linq
{
    /// <summary>
    /// 基础配置加载器
    /// </summary>
    public class ConfigLoader<T> where T : BaseConfigItem, new()
    {
        private readonly string sql = "";
        private readonly IDbHelper? dBHelper = null;

        private readonly string jsonFileName = "";

        private Dictionary<string, T> configs;
        /// <summary>
        /// 配置集合
        /// </summary>
        public Dictionary<string, T> Configs { get { return configs; } }

        /// <summary>
        /// 加载器构造函数
        /// <para>此操作是利用sql查询到结果然后进行绑定</para>
        /// </summary>
        /// <param name="dBHelper">数据库连接对象</param>
        /// <param name="sql">需要查询的语句</param>
        public ConfigLoader(IDbHelper dBHelper, string sql)
        {
            configs = new Dictionary<string, T>();
            this.dBHelper = dBHelper;
            this.sql = sql;

            Reload();
        }

        /// <summary>
        /// 加载器构造函数
        /// <para>此操作是利用json数据然后进行绑定</para>
        /// </summary>
        /// <param name="jsonFileName">json文件路径与名称</param>
        public ConfigLoader(string jsonFileName)
        {
            configs = new Dictionary<string, T>();
            this.jsonFileName = jsonFileName;

            Reload();
        }

        /// <summary>
        /// 查找主键所对应的值
        /// </summary>
        /// <param name="value">对象值</param>
        /// <returns></returns>
        public T? Find(object value)
        {
            string cval = value.ToString() ?? "";

            if (configs.ContainsKey(cval))
                return configs[cval];
            else
                return null;
        }

        /// <summary>
        /// 查找主键所对应的值
        /// </summary>
        /// <param name="value">对象值</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool TryFind(object value, out T? config)
        {
            return configs.TryGetValue(value.ToString() ?? "", out config);
        }

        /// <summary>
        /// 重新读取配置
        /// </summary>
        public void Reload()
        {
            if (dBHelper != null && sql != "")
            {
                var result = dBHelper.CommandSQL(sql);
                if (result.EffectNum >= 0)
                {
                    Dictionary<string, T> tempConfigs = new Dictionary<string, T>();
                    if(result.Rows != null)
                    {
                        foreach (DataRow? item in result.Rows)
                        {
                            if (item == null) continue;

                            T temp = new T();
                            temp.SetESConfig(item);
                            temp.SetESPrimaryKey(item);

#if !NET462 && !NETSTANDARD2_0
                            tempConfigs.TryAdd(temp.PrimaryKey, temp);
#else
                        tempConfigs.Add(temp.PrimaryKey, temp);
#endif
                        }
                    }
                    System.Threading.Interlocked.Exchange(ref configs, tempConfigs);
                }
            }
            else if (jsonFileName != "")
            {
                if (!File.Exists(jsonFileName))
                    return;

                JArray? jData = null;
                using (StreamReader file = File.OpenText(jsonFileName))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        jData = (JArray)JToken.ReadFrom(reader);
                    }
                }

                if (jData == null)
                    return;

                Dictionary<string, T> tempConfigs = new Dictionary<string, T>();
                for (int i = 0, len = jData.Count; i < len; i++)
                {
                    var jItem = jData[i];
                    T temp = new T();
                    temp.SetESConfig(jItem);
                    temp.SetESPrimaryKey(jItem);

#if !NET462 && !NETSTANDARD2_0
                    tempConfigs.TryAdd(temp.PrimaryKey, temp);
#else
                    tempConfigs.Add(temp.PrimaryKey, temp);
#endif
                }

                System.Threading.Interlocked.Exchange(ref configs, tempConfigs);
            }
        }
    }
}
