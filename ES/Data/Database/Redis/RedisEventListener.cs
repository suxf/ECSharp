using StackExchange.Redis;

namespace ES.Data.Database.Redis
{
    /// <summary>
    /// redis 事件监听器
    /// </summary>
    public interface RedisEventListener
    {
        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MuxerConfigurationChanged(object sender, EndPointEventArgs e);
        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MuxerErrorMessage(object sender, RedisErrorEventArgs e);
        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e);
        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e);
        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e);
        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MuxerInternalError(object sender, InternalErrorEventArgs e);
    }
}
