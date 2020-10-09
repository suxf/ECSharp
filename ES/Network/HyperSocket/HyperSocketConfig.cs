namespace ES.Network.HyperSocket
{
    /// <summary>
    /// 超级套接字配置
    /// <para>创建套接字所需要的一些配置</para>
    /// </summary>
    public class HyperSocketConfig
    {
        /// <summary>
        /// TCP协议接受数据大小
        /// </summary>
        public uint TcpReceiveSize = 1380;

        /// <summary>
        /// UDP协议接受数据大小
        /// mtu建议设置小于默认值1380以下 否则可能导致丢包
        /// </summary>
        public uint UdpReceiveSize = 1380;

        /// <summary>
        /// 心跳超时时间 单位 毫秒ms
        /// <para>超过心跳服务器/客户端自动断开释放</para>
        /// <para>可以通过调整心跳检测周期来更改误差</para>
        /// </summary>
        public int HeartTimeOut = 10000;
        /// <summary>
        /// 服务端心跳检测超时周期 单位 毫秒ms
        /// <para>系统会根据此时间来接受心跳检测</para>
        /// </summary>
        public int HeartCheckPeriod = 4000;

        /// <summary>
        /// 客户端心跳发送周期 单位 毫秒ms
        /// <para>系统会根据此时间来循环发送心跳用来检测</para>
        /// </summary>
        public int HeartSendPeriod = 10000;

        /// <summary>
        /// KCP模式
        /// </summary>
        public KcpMode kcpMode = KcpMode.Fast;
        /// <summary>
        /// KCP窗口大小[目前输出输入窗口采用一个值]
        /// </summary>
        public int KcpWinSize = 64;
    }
}
