#if !UNITY_2020_1_OR_NEWER
using ECSharp.Variant;
using Fleck;
using System;

namespace ECSharp.Network.Websocket
{
    /// <summary>
    /// 远程连接对象
    /// </summary>
    public class RemoteConnection
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        public IWebSocketConnection Socket { get; private set; }
        /// <summary>
        /// 用户自定义标识 绑定对象
        /// </summary>
        public Var Tag = Var.Empty;
        /// <summary>
        /// 字符消息
        /// </summary>
        public string Message { get; internal set; } = "";
        /// <summary>
        /// 字节消息
        /// </summary>
        public byte[] Buffer { get; internal set; } = Utils.ByteConverter.Empty;

        /// <summary>
        /// 连接对象是否有效
        /// </summary>
        public bool IsAvailable { get { if (Socket != null) return Socket.IsAvailable; else return false; } }

        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="socket"></param>
        public RemoteConnection(IWebSocketConnection socket)
        {
            Socket = socket;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">字符消息</param>
        public bool Send(string message)
        {
            if (Socket != null && Socket.IsAvailable) { Socket.Send(message); return true; }
            else return false;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">字节消息</param>
        public bool Send(byte[] message)
        {
            if (Socket != null && Socket.IsAvailable) { Socket.Send(message); return true; }
            else return false;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">字节消息</param>
        public bool Send(Var message)
        {
            return Send(message.GetBytes());
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">字节消息</param>
        public bool Send(VarList message)
        {
            return Send(message.GetBytes());
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">字节消息</param>
        public bool Send(VarMap message)
        {
            return Send(message.GetBytes());
        }

        /// <summary>
        /// 获取唯一标识ID 同一个socket连接id不变
        /// </summary>
        /// <returns></returns>
        public Guid GetSocketGuid()
        {
            if (Socket != null && Socket.IsAvailable) { return Socket.ConnectionInfo.Id; }
            else return default;
        }

        /// <summary>
        /// 获取连接信息
        /// </summary>
        /// <returns></returns>
        public IWebSocketConnectionInfo? GetConnectionInfo()
        {
            if (IsAvailable) return Socket.ConnectionInfo;
            else return null;
        }

        /// <summary>
        /// 关闭当前连接
        /// </summary>
        public void Close()
        {
            if (IsAvailable) Socket.Close();
            // Socket = null;
        }
    }
}
#endif