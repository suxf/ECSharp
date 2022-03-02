using ES.Common.Utils;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace ES.Network.Sockets.Server
{
    /// <summary>
    /// 远程套接字信息体
    /// </summary>
    public class RemoteSocketMsg
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
        public RemoteConnection? sender;
        /// <summary>
        /// 发送远程终端
        /// </summary>
        public EndPoint remoteEndPoint;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="data"></param>
        /// <param name="sender"></param>
        internal RemoteSocketMsg(ushort sessionId, byte[] data, RemoteConnection sender)
        {
            this.sessionId = sessionId;
            this.data = data;
            this.sender = sender;
            remoteEndPoint = sender.Socket!.endPoint;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="data"></param>
        /// <param name="remoteEndPoint"></param>
        internal RemoteSocketMsg(ushort sessionId, byte[] data, EndPoint remoteEndPoint)
        {
            this.sessionId = sessionId;
            this.data = data;
            this.remoteEndPoint = remoteEndPoint;
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
        public JArray? AsJArray()
        {
            return Encoding.UTF8.GetString(data).AsJArray();
        }
    }
}
