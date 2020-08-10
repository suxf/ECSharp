using Fleck;
using System;

namespace ES.Network.Websocket
{
    /// <summary>
    /// 远程连接对象
    /// </summary>
    public class RemoteConnection
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        public IWebSocketConnection socket;
        /// <summary>
        /// 用户自定义标识 绑定对象
        /// </summary>
        public object tag;
        /// <summary>
        /// 字符消息
        /// </summary>
        public string message;
        /// <summary>
        /// 字节消息
        /// </summary>
        public byte[] buffer;

        /// <summary>
        /// 连接对象是否有效
        /// </summary>
        public bool IsAvailable { get { if (socket != null) return socket.IsAvailable; else return false; } }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">字符消息</param>
        public bool Send(string message)
        {
            if (socket != null && socket.IsAvailable) { socket.Send(message); return true; }
            else return false;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">字节消息</param>
        public bool Send(byte[] message)
        {
            if (socket != null && socket.IsAvailable) { socket.Send(message); return true; }
            else return false;
        }

        /// <summary>
        /// 获取唯一标识ID 同一个socket连接id不变
        /// </summary>
        /// <returns></returns>
        public Guid GetSocketGuid()
        {
            if (socket != null && socket.IsAvailable) { return socket.ConnectionInfo.Id; }
            else return default;
        }
    }
}
