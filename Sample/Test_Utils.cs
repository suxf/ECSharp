using ES.Common.Utils;
using System;

namespace Sample
{
    /// <summary>
    /// 工具类测试
    /// </summary>
    class Test_Utils
    {
        /// <summary>
        /// 这个类相对较为简单
        /// 没有太多的逻辑 都是相对单一的功能静态类或者单例
        /// </summary>
        public Test_Utils()
        {
            // 读取配置 默认和读取程序名一样配置文件 比如此程序生成为Sample.exe那么读取对应的是Sample.exe.config文件
            // 一般来说使用vs2019开发 只需要在新建一个和程序集名称一模一样的.config配置文件即可
            // 注意此函数不支持读取其他文件 此demo已经创建了配置文件详见项目 Sample.config
            // 本类设计初只能读取两层 具体结构可以参照样例
            // 此处读取第一层配置数据
            string test = AppConfig.Read("test");
            int test1 = AppConfig.Read<int>("test1");
            bool test2 = AppConfig.Read<bool>("test2");
            Console.WriteLine($"test->{test}");
            Console.WriteLine($"test1->{test1}");
            Console.WriteLine($"test2->{test2}");
            // 此处读取第二层配置数据
            string tests2 = AppConfig.Read("testgroup", "test2");
            float tests3 = AppConfig.Read<float>("testgroup", "test3");
            Console.WriteLine($"test2->{tests2}");
            Console.WriteLine($"test3->{tests3}");

            // 获取有效字节
            // 此判定依据是在某索引位为0开始 往后4位皆为0 则认为后续数据无效实现
            // 所以这里的设定还是要看具体情况来 不一定适用所有情况
            // ByteHelper.GetValidLength 则是直接获取长度大小 而非返回数据
            byte[] bytes = ByteHelper.GetValidByte(new byte[] { 1, 2, 3, 4, 0, 0, 0 });
            Console.WriteLine($"bytes len:{bytes.Length}");

            // 随机生成指定位数的字符串
            // 字符串将有数字与大小写字母组成
            string code = RandomCode.Generate(32);
            Console.WriteLine($"code:{code}");

            // md5的封装
            string md5Str = MD5.Encrypt("helloworld");
            Console.WriteLine($"helloworld:{md5Str}");

            // 获取此框架的版本信息
            string versionStr = ES.Common.Utils.Version.ToString();
            Console.WriteLine($"es version:{versionStr}");
        }
    }
}
