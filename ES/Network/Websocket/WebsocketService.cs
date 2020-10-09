using Fleck;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;

namespace ES.Network.Websocket
{
    /// <summary>
    /// esf websocket服务器模块
    /// <para>模块是对第三方框架Fleck二次封装完成</para>
    /// <para>Fleck地址:https://github.com/statianzo/Fleck</para>
    /// </summary>
    public sealed class WebsocketService
    {
        /// <summary>
        /// Fleck websocket服务对象
        /// </summary>
        private WebSocketServer websocketSvr = null;

        private readonly ConcurrentDictionary<Guid, RemoteConnection> remoteConnections = new ConcurrentDictionary<Guid, RemoteConnection>();

        /// <summary>
        /// 创建服务
        /// </summary>
        /// <param name="url">监听地址</param>
        /// <param name="invoke">监听委托</param>

        public WebsocketService(string url, IWebsocketInvoke invoke)
        {
            websocketSvr = new WebSocketServer(url);

            InitInvoke(invoke);
        }

        /// <summary>
        /// 创建ssl服务
        /// </summary>
        /// <param name="url">监听地址</param>
        /// <param name="certificateFile">ssl证书路径</param>
        /// <param name="invoke">监听委托</param>
        public WebsocketService(string url, string certificateFile, IWebsocketInvoke invoke)
        {
            websocketSvr = new WebSocketServer(url);
            websocketSvr.Certificate = new X509Certificate2(certificateFile);

            InitInvoke(invoke);
        }

        /// <summary>
        /// 初始化委托
        /// </summary>
        /// <param name="invoke"></param>
        private void InitInvoke(IWebsocketInvoke invoke)
        {
            websocketSvr.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    if (!remoteConnections.TryGetValue(socket.ConnectionInfo.Id, out var conn))
                    {
                        conn = new RemoteConnection();
                        conn.socket = socket;
                        remoteConnections.TryAdd(socket.ConnectionInfo.Id, conn);
                    }
                    invoke.OnOpen(conn);
                };
                socket.OnClose = () =>
                {
                    if (remoteConnections.TryRemove(socket.ConnectionInfo.Id, out var conn))
                    {
                        invoke.OnClose(conn);
                        conn.tag = null;
                        conn.message = null;
                        conn.socket = null;
                    }
                };
                socket.OnMessage = message =>
                {
                    if (remoteConnections.TryGetValue(socket.ConnectionInfo.Id, out var conn))
                    {
                        conn.message = message;
                        invoke.OnMessage(conn);
                    }
                };
                socket.OnBinary = buffer =>
                {
                    if (remoteConnections.TryGetValue(socket.ConnectionInfo.Id, out var conn))
                    {
                        conn.buffer = buffer;
                        invoke.OnBinary(conn);
                    }
                };
                socket.OnError = exception =>
                {
                    if (remoteConnections.TryGetValue(socket.ConnectionInfo.Id, out var conn))
                    {
                        invoke.OnError(conn, exception);
                    }
                };
            });
        }

        /// <summary>
        /// 关闭websocket
        /// </summary>
        public void CloseWebSocketSvrService()
        {
            if (websocketSvr != null)
                websocketSvr.Dispose();
            websocketSvr = null;
        }

    }
}
