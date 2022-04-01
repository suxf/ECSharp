using ES.Utils;

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
            LocalStorage.Set("test", 1);
            LocalStorage.Set("test2", "Hello world");
            // 取出值
            Log.Info("Test 1:" + LocalStorage.Get("test"));
            // 写入一个文件数据
            LocalStorage.WriteData("hello world", "test.txt");
            // 读取文件数据
            Log.Info("Test 2:" + LocalStorage.ReadData("test.txt"));
        }
    }
}
