using ES.Linq;
using ES.Utils;
using Newtonsoft.Json.Linq;
using System;

namespace Sample
{
    /// <summary>
    /// json测试
    /// 本框架引用MIT协议的Newtonsoft.Json来创建和解析Json数据
    /// 本案例旨在进一步简化一些创建过程中操作
    /// </summary>
    class Test_Json
    {
        /// <summary>
        /// 注意此处指描述了JObject的相关操作
        /// JArray没有写入事例，因为它们之间的相关调用都一样
        /// 只需要实现区分json的类型到底是对象还是数组采用不同方式处理即可
        /// </summary>
        public Test_Json()
        {
            // 先简单创建一个json数据
            JObject jobj = new JObject() { { "id", 100 }, { "content", "hello world" } };
            // 通过AsString可以快速转换对象为string类型
            var sobj = jobj.AsString();
            // 通过AsBytes可以快速转换对象为byte[]类型
            // 注意本框架所有案例的byte转码如无特殊说明都为 utf-8 编码
            var bobj = jobj.AsBytes();
            // 通过json字符串转为Json对象
            var jobj2 = sobj.AsJObject();
            // 通过json字节数组转为Json对象
            var jobj3 = bobj.AsJObject();

            Log.Info($"json:{sobj}");
        }
    }
}
