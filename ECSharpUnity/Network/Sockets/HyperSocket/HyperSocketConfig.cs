namespace ECSharp.Network.Sockets.HyperSocket
{
    /// <summary>
    /// 超级套接字配置
    /// <para>创建套接字所需要的一些配置</para>
    /// <para>配置中括号C代表客户端配置 S代表服务端配置 C+S 代表双端皆使用的配置</para>
    /// </summary>
    public class HyperSocketConfig
    {
        /// <summary>
        /// [C+S] TCP协议接受数据大小
        /// </summary>
        public uint TcpReceiveSize = 1380;

        /// <summary>
        /// [C+S] UDP协议接受数据大小
        /// <para>mtu建议设置小于默认值1380以下 否则可能导致丢包</para>
        /// </summary>
        public uint UdpReceiveSize = 1380;

        /// <summary>
        /// [C+S] 心跳超时时间 单位 毫秒ms
        /// <para>超过心跳服务器/客户端自动断开释放</para>
        /// <para>可以通过调整心跳检测周期来更改误差</para>
        /// </summary>
        public int HeartTimeOut = 10000;
        /// <summary>
        /// [S] 服务端心跳检测超时周期 单位 毫秒ms
        /// <para>系统会根据此时间来接受心跳检测</para>
        /// </summary>
        public int HeartCheckPeriod = 4000;

        /// <summary>
        /// [C] 客户端心跳发送周期 单位 毫秒ms
        /// <para>系统会根据此时间来循环发送心跳用来检测</para>
        /// </summary>
        public int HeartSendPeriod = 10000;

        /// <summary>
        /// [S] 使用安全传输协议
        /// <para>设置为true则打开安全协议</para>
        /// <para>只需要服务端配置为使用状态，客户端会自动以安全传输连接</para>
        /// <para></para>
        /// </summary>
        public bool UseSSL = false;
        /// <summary>
        /// [S] 使用安全传输模式
        /// <para>传输可以决定在安全协议下 哪种通信使用加密传输 默认TCP/UDP都是用加密传输</para>
        /// <para>0 TCP/UDP全使用加密传输 1 TCP使用加密传输 2 UDP使用加密传输</para>
        /// <para></para>
        /// </summary>
        public int SSLMode = 0;

        /// <summary>
        /// [C+S] KCP模式
        /// </summary>
        public KcpMode kcpMode = KcpMode.Fast;
        /// <summary>
        /// [C+S] KCP窗口大小[目前输出输入窗口采用一个值]
        /// </summary>
        public int KcpWinSize = 64;
    }
}
