using ES.Common.Log;
using ES.Data.Database.Redis;
using StackExchange.Redis;

namespace Sample
{
    /// <summary>
    /// redis 测试
    /// </summary>
    class Test_Redis : IRedisEvent
    {
        public Test_Redis()
        {
            RedisHelper helper = new RedisHelper("127.0.0.1:6379", 2);
            // 增加事件监听用于检测连接状态
            helper.AddEventListener(this);
            // 设置一个值
            helper.Set("test", 111);
            // 取出值
            Log.Info("Test:" + helper.Get<int>("test"));

            // 关于redis还有很多函数方法提供这里不再赘述，有兴趣可以看看源码
        }

        public void OnConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Log.Info("OnConfigurationChanged" + e.EndPoint.ToString());
        }

        public void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Log.Info("OnConnectionFailed" + e.EndPoint.ToString());
        }

        public void OnConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Log.Info("OnConnectionRestored" + e.EndPoint.ToString());
        }

        public void OnErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Log.Info("OnErrorMessage" + e.EndPoint.ToString());
        }

        public void OnHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Log.Info("OnHashSlotMoved" + e.NewEndPoint.ToString());
        }

        public void OnInternalError(object sender, InternalErrorEventArgs e)
        {
            Log.Info("OnInternalError" + e.EndPoint.ToString());
        }
    }
}
