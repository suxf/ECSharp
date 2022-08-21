using ECSharp;
using ECSharp.Linq;
using ECSharp.Network.Sockets.HyperSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace Sample
{
    /// <summary>
    /// 简单聊天系统
    /// </summary>
    class Test_SimpleChatRoom
    {
        static HyperSocketServer server = null;
        static HyperSocket client = null;

        public Test_SimpleChatRoom()
        {
            Log.Info("[1]服务器 [2]客户端");
            Console.Write("输入:");
            var input = Console.ReadLine();
            if (input == "1")
            {
                Log.Info("启动服务器");
                StartServer();
            }
            else if (input == "2")
            {
                Log.Info("启动客户端");
                StartClient();
            }
            else Log.Info("啥都没");
        }

        public void StartServer()
        {
            server = new HyperSocketServer("127.0.0.1", 8888, 50, new ServerListener(), new HyperSocketConfig() { UseSSL = true });
            server.StartServer();
        }

        public void StartClient()
        {
            client = new HyperSocket("127.0.0.1", 8888, new ClientListener());
            client.Connect();
            while (true)
            {
                var id = Console.ReadLine();
                var str = Console.ReadLine();
                var jsonObj = new JObject();
                jsonObj.Add("id", int.Parse(id));
                jsonObj.Add("msg", str);
                client.SendUdp(jsonObj.AsBytes());
            }
        }

        class ClientListener : IHyperSocketClient
        {
            public void OnOpen(HyperSocket socket)
            {
                Log.Info($"连接服务器成功！聊天ID:{socket.SessionId}");
                Log.Info($"发送信息：聊天ID + 回车 + 消息，聊天ID等于0为群发消息");
                Log.Info($"例如：1 回车 HelloWorld");
            }

            public void OnTcpReceive(byte[] data, HyperSocket socket)
            {
                string str = Encoding.UTF8.GetString(data);
                var jsonObj = str.AsJObject();
                Log.Info($"来自:{jsonObj["id"]}, 内容:{jsonObj["msg"]}");
            }

            public void OnUdpReceive(byte[] data, HyperSocket socket)
            {
                Log.Info($"消息发送{Encoding.UTF8.GetString(data)}");
            }

            public void SocketError(HyperSocket socket, Exception ex)
            {
                Log.Info($"客户端错误:{ex.Message}");
            }
        }

        class ServerListener : IHyperSocketServer
        {
            ConcurrentDictionary<int, RemoteHyperSocket> sockets = new ConcurrentDictionary<int, RemoteHyperSocket>();

            public void OnClose(RemoteHyperSocket socket)
            {
                Log.Info($"客户端关闭:{socket.SessionId}");
                sockets.TryRemove(socket.SessionId, out _);
            }

            public void OnOpen(RemoteHyperSocket socket)
            {
                Log.Info($"聊天Id:{socket.SessionId}");
                sockets.TryAdd(socket.SessionId, socket);
            }

            public void OnTcpReceive(byte[] data, RemoteHyperSocket socket)
            {
            }

            public void OnUdpReceive(byte[] data, RemoteHyperSocket socket)
            {
                var jsondata = data.AsJObject();
                var jsonObj = new JObject();
                jsonObj.Add("id", socket.SessionId);
                jsonObj.Add("msg", jsondata["msg"]);
                var buffer = jsonObj.AsBytes();
                if (jsondata["id"].ToString() == "0")
                {
                    foreach (var item in sockets) if (item.Key != socket.SessionId) item.Value.SendTcp(buffer);
                    socket.SendUdp("(群)成功");
                }
                else
                {
                    if (sockets.TryGetValue(int.Parse(jsondata["id"].ToString()), out var value))
                    {
                        value.SendTcp(buffer);
                        socket.SendUdp("成功");
                    }
                    else
                    {
                        socket.SendUdp("失败");
                    }
                }
            }

            public void SocketError(RemoteHyperSocket socket, Exception ex)
            {
                Log.Info($"客户端错误:{ex.Message}");
                sockets.TryRemove(client.SessionId, out _);
            }
        }
    }
}
