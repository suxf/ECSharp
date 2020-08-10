using ES.Common.Utils;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ES.Network.Sockets
{
    /// <summary>
    /// 套接字信息体
    /// </summary>
    public class SocketMsg
    {
        /// <summary>
        /// 主指令
        /// </summary>
        public byte main;
        /// <summary>
        /// 副指令
        /// </summary>
        public byte second;
        /// <summary>
        /// 网络数据
        /// </summary>
        public byte[] data;
        /// <summary>
        /// 发送者
        /// </summary>
        public ClientSocket sender;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="main"></param>
        /// <param name="second"></param>
        /// <param name="data"></param>
        /// <param name="sender"></param>
        public SocketMsg(byte main, byte second, byte[] data, ClientSocket sender)
        {
            this.main = main;
            this.second = second;
            this.data = data;
            this.sender = sender;
        }

        /// <summary>
        /// 网络数据转为json对象
        /// 默认编码UTF-8 如错误解析则抛出异常
        /// </summary>
        /// <returns>json对象</returns>
        public JObject AsJObject()
        {
            return Encoding.UTF8.GetString(data).AsJObject();
        }

        /// <summary>
        /// 网络数据转为json数组
        /// 默认编码UTF-8 如错误解析则抛出异常
        /// </summary>
        /// <returns>json数组</returns>
        public JArray AsArray()
        {
            return Encoding.UTF8.GetString(data).AsJArray();
        }
    }
}
