using ECSharp.Network.Websocket;
using System;

namespace Sample
{
    class Test_Websocket
    {
        public Test_Websocket()
        {
            WebsocketService service = new WebsocketService("ws://127.0.0.1:8081", new WebsocketHandle());

            Console.ReadLine();
        }

        class WebsocketHandle : IWebsocket
        {
            public void OnBinary(RemoteConnection conn)
            {
                // 数据使用byte流传输调用
            }

            public void OnClose(RemoteConnection conn)
            {
                // 关闭时调用
            }

            public void OnError(RemoteConnection conn, Exception exception)
            {
                // 出现异常调用
            }

            public void OnMessage(RemoteConnection conn)
            {
                // 数据使用文本时调用
            }

            public void OnOpen(RemoteConnection conn)
            {
                // 新连接产生调用
            }
        }
    }
}
