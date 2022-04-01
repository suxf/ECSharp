#if !UNITY_2020_1_OR_NEWER
using StackExchange.Redis;

namespace ES.Database.Redis
{
    /// <summary>
    /// redis 事件监听器
    /// </summary>
    public interface IRedisEvent
    {
        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnConfigurationChanged(object sender, EndPointEventArgs e);
        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnErrorMessage(object sender, RedisErrorEventArgs e);
        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnConnectionRestored(object sender, ConnectionFailedEventArgs e);
        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnConnectionFailed(object sender, ConnectionFailedEventArgs e);
        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHashSlotMoved(object sender, HashSlotMovedEventArgs e);
        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnInternalError(object sender, InternalErrorEventArgs e);
    }
}
#endif