using ES.Linq;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ES.Network.Sockets.Client
{
    /// <summary>
    /// 套接字信息体
    /// </summary>
    public class SocketMsg
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public ushort sessionId;
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
        /// <param name="sessionId"></param>
        /// <param name="data"></param>
        /// <param name="sender"></param>
        public SocketMsg(ushort sessionId, byte[] data, ClientSocket sender)
        {
            this.sessionId = sessionId;
            this.data = data;
            this.sender = sender;
        }

        /// <summary>
        /// 网络数据转为json对象
        /// <para>默认编码UTF-8 如错误解析则抛出异常</para>
        /// </summary>
        /// <returns>json对象</returns>
        public JObject? AsJObject()
        {
            return Encoding.UTF8.GetString(data).AsJObject();
        }

        /// <summary>
        /// 网络数据转为json数组
        /// <para>默认编码UTF-8 如错误解析则抛出异常</para>
        /// </summary>
        /// <returns>json数组</returns>
        public JArray? AsArray()
        {
            return Encoding.UTF8.GetString(data).AsJArray();
        }
    }
}
