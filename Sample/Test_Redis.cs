using ES.Common.Log;
using ES.Data.Database.Redis;
using StackExchange.Redis;

namespace Sample
{
    /// <summary>
    /// redis 测试
    /// </summary>
    class Test_Redis : RedisEventListener
    {
        public Test_Redis()
        {
            // 先开启控制台日志 以便观察
            LogConfig.LOG_CONSOLE_OUTPUT = true;

            RedisHelper helper = new RedisHelper("127.0.0.1:6379", 2);
            // 增加事件监听用于检测连接状态
            helper.AddEventListener(this);
            // 设置一个值
            helper.Set("test", 111);
            // 取出值
            Log.Info("Test:" + helper.Get<int>("test"));

            // 关于redis还有很多函数方法提供这里不再赘述，有兴趣可以看看源码
        }

        public void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            ES.Common.Log.Log.Info("MuxerConfigurationChanged" + e.EndPoint.ToString());
        }

        public void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            ES.Common.Log.Log.Info("MuxerConnectionFailed" + e.EndPoint.ToString());
        }

        public void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            ES.Common.Log.Log.Info("MuxerConnectionRestored" + e.EndPoint.ToString());
        }

        public void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            ES.Common.Log.Log.Info("MuxerErrorMessage" + e.EndPoint.ToString());
        }

        public void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            ES.Common.Log.Log.Info("MuxerHashSlotMoved" + e.NewEndPoint.ToString());
        }

        public void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            ES.Common.Log.Log.Info("MuxerInternalError" + e.EndPoint.ToString());
        }
    }
}
