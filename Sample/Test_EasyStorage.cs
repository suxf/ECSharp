using ES.Common.Log;
using ES.Data.Storage;

namespace Sample
{
    /// <summary>
    /// 简单存储demo展示
    /// </summary>
    class Test_EasyStorage
    {
        public Test_EasyStorage()
        {
            // 设置一个值
            EasyStorage.Set("test", 1);
            EasyStorage.Set("test2", "Hello world");
            // 取出值
            Log.Info("Test 1:" + EasyStorage.Get("test"));
            // 写入一个文件数据
            EasyStorage.WriteData("hello world", "test.txt");
            // 读取文件数据
            Log.Info("Test 2:" + EasyStorage.ReadData("test.txt"));
        }
    }
}
