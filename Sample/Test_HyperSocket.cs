using ES.Time;
using ES.Network.HyperSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ES;

namespace Sample
{
    /// <summary>
    /// 超级套接字演示
    /// </summary>
    class Test_HyperSocket
    {
        static BaseHyperSocket[] sockets = new BaseHyperSocket[10000];
        static TimeCaller[] timeCaller2 = new TimeCaller[10000];
        static int num = 0; 
        static HashSet<int> ssss = new HashSet<int>();

        static Random rd = new Random();

        public Test_HyperSocket()
        {
            Log.Info("[1]服务器 [2]并发启动客户端 [3]顺序启动客户端");
            var input = Log.ReadLine("输入:");
            if (input == "1")
            {
                Log.Info("启动服务器");
                StartServer(0);
            }
            else if (input == "2")
            {
                Log.Info("并发启动客户端");
                for (int i = 1; i <= 300; i++) StartClient(i);
                TimeCaller.Create(static delegate ()
                {
                    Log.Info($"Connect Num:{num}");
                }, 1000, 3000, TimeCaller.Infinite).Start(true);
                Log.ReadLine();
                for (int i = 1; i <= 300; i++)
                {
                    if (sockets[i] == null) continue;
                    if(((HyperSocket)sockets[i]).Tag >= 1)
                    {
                        ((HyperSocket)sockets[i]).Close();
                        Log.Info($"Close Client:{i}");
                    }
                }
            }
            else if (input == "3")
            {
                Log.Info("顺序启动客户端");
                for (int i = 1; i <= 300; i++)
                {
                    Log.Debug($"Start Client:", i.ToString());
                    sockets[i] = new HyperSocket("127.0.0.1", 8888, new ClientListener()).Connect();
                    ((HyperSocket)sockets[i]).Tag = i;
                    Thread.Sleep(100);
                }
                TimeCaller.Create(static delegate ()
                {
                    Log.Info($"Connect Num:{num}");
                }, 1000, 3000, TimeCaller.Infinite).Start(true);
                Log.ReadLine();
                for (int i = 1; i <= 300; i++)
                {
                    if (sockets[i] == null) continue;
                    if (((HyperSocket)sockets[i]).Tag >= 1)
                    {
                        ((HyperSocket)sockets[i]).Close();
                        Log.Info($"Close Client:{i}");
                    }
                }
            }
            else Log.Info("啥都没");
        }

        public void StartServer(int i)
        {
            sockets[i] = new HyperSocketServer("127.0.0.1", 8888, 500, new ServerListener(), new HyperSocketConfig() { UseSSL = true }).StartServer();
            TimeCaller.Create(delegate () {
                Log.Info($"【RealTime】 ServerId:{i} Connect Num:{ssss.Count}, Num2:{((HyperSocketServer)sockets[0]).ConnectedCount}");
            }, 1000, 3000, TimeCaller.Infinite).Start(true);
        }

        public void StartClient(int i)
        {
            //threads[i] = new Thread(delegate ()
            // {
            //Thread.Sleep(rd.Next(500, 5000));
            TimeCaller.Create(delegate ()
            {
                Log.Debug($"Start Client:{i}");
                sockets[i] = new HyperSocket("127.0.0.1", 8888, new ClientListener()).Connect();
                ((HyperSocket)sockets[i]).Tag = i;
            }, rd.Next(1000, 5000), 0).Start(true);
            //});
            //threads[i].Start();
        }

        class ServerListener : IHyperSocketServer
        {
            public void OnClose(RemoteHyperSocket socket)
            {
                lock (ssss)
                {
                    if (ssss.Remove(socket.SessionId))
                        Log.Warn($"【OnClose】Connect Num:{ssss.Count}, Num2:{((HyperSocketServer)sockets[0]).ConnectedCount}");
                }
                // Log.Info($"Socket Session Close:{socket.SessionId}");
            }

            public void SocketError(RemoteHyperSocket socket, Exception ex)
            {
                if (socket != null)
                {
                    lock (ssss)
                    {
                        if (ssss.Remove(socket.SessionId))
                            Log.Warn($"【OnClose】Connect Num:{ssss.Count}, Num2:{((HyperSocketServer)sockets[0]).ConnectedCount}");
                    }
                }
                Log.Exception(ex);
            }

            public void OnOpen(RemoteHyperSocket socket)
            {
                lock (ssss)
                {
                    ssss.Add(socket.SessionId);
                    Log.Info($"【OnOpen】Connect Num:{ssss.Count}, Num2:{((HyperSocketServer)sockets[0]).ConnectedCount}");
                }
                // Log.Info($"Connect OK:{socket.SessionId}");
            }

            public void OnTcpReceive(byte[] data, RemoteHyperSocket socket)
            {
                //Log.Info(Encoding.UTF8.GetString(data));
                socket.SendTcp(Encoding.UTF8.GetString(data));
                // socket.SendTcp("1111");
                // string str = Encoding.UTF8.GetString(data);
                // var num = int.Parse(str);
                // if (num == 1)
                // {
                //     socket.SendTcp((num + 1).ToString());
                // }
                // else
                // {
                //     if ((int)socket.Tag + 2 != num)
                //     {
                //         Log.Info($"SessionId TCP:{socket.SessionId}, Num:{num}");
                //     }
                //     else { socket.Tag = num; Log.Info($"SessionId TCP:{socket.SessionId}, OK"); }
                //     socket.SendTcp((num + 1).ToString());
                // }
                // Log.Info($"TCP.Cnt[{socket.SessionId}-{socket.GetRemoteIp()}]:" + Encoding.UTF8.GetString(data));
                // socket.SendTcp("Hello World Svr 3:" + Interlocked.Increment(ref num));
                // socket.SendUdp("Hello World Svr 4:" + Interlocked.Increment(ref num));
            }

            public void OnUdpReceive(byte[] data, RemoteHyperSocket socket)
            {
                // string str = Encoding.UTF8.GetString(data);
                // var num = int.Parse(str);
                // if (num == 1)
                // {
                //     socket.SendTcp((num + 1).ToString());
                // }
                // else
                // {
                //     if((int)socket.Tag + 2 != num) Log.Info($"SessionId UDP:{socket.SessionId}, Num:{num}");
                //     else { socket.Tag = num; Log.Info($"SessionId UDP:{socket.SessionId}, OK"); }
                //     socket.SendTcp((num + 1).ToString());
                // }
                //Log.Info(Encoding.UTF8.GetString(data));
                socket.SendTcp(Encoding.UTF8.GetString(data));
                //socket.SendTcp("2222");
                // Log.Info($"UDP.Cnt[{socket.SessionId}-{socket.GetRemoteIp()}]:" + Encoding.UTF8.GetString(data));
                // socket.SendUdp("Hello World Svr 5:" + Interlocked.Increment(ref num));
            }
        }

        class ClientListener : IHyperSocketClient
        {
            bool fff = false;

            public void SocketError(HyperSocket socket, Exception ex)
            {
                if (fff) Interlocked.Decrement(ref num);
                Log.Info($"【SocketError】 Connect Num:{num}");
                Log.Exception(ex);
                if (socket.Tag.IsNumber())
                {
                    timeCaller2[socket.Tag]?.Cancel();
                    sockets[socket.Tag] = null;
                }
                // Log.Warn($"【SocketError】 ReStart Client:{num}");
                // TimeCaller.Create(delegate {
                //     sockets[socket.Tag] = new HyperSocket("127.0.0.1", 8888, new ClientListener()).Connect();
                //     // socket.Connect(); 
                // }, rd.Next(1000, 3000)).Start(true);
            }

            public void OnOpen(HyperSocket socket)
            {
                // timeCaller1[i] = new TimeCaller(rd.Next(1000, 5000), rd.Next(500, 1000), true, -1, () =>
                // {
                // socket.SendTcp(1.ToString());
                // });
                fff = true;
                Interlocked.Increment(ref num);
                Log.Info($"【OnOpen】 Connect Num:{num}");
                // Log.Info($"Connect OK:{socket.SessionId}");
                timeCaller2[socket.Tag] = TimeCaller.Create(delegate ()
                {
                    // socket.SendUdp(count.ToString());
                    socket.SendUdp(ES.Utils.RandomCode.Generate(rd.Next(0, 2048)));
                }, 1000, 100, TimeCaller.Infinite).Start();
            }

            // int lastNum = 1;
            public void OnTcpReceive(byte[] data, HyperSocket socket)
            {
                // Log.Info("Svr:" + Encoding.UTF8.GetString(data));
                if (data == null) { Log.Info("************************************"); return; }
                string str = Encoding.UTF8.GetString(data);
                // var deltaNum = int.Parse(str) - lastNum - 1;
                // lastNum = int.Parse(str);
                // var num = int.Parse(str);
                // if(num % 10000 == 0) Console.Clear();
                // Log.Info($"SocketID:{socket.SessionId}, TCP Num:{str}, Delta:{deltaNum}");
                // socket.SendUdp((num + 1).ToString());
            }

            public void OnUdpReceive(byte[] data, HyperSocket socket)
            {
                // string str = Encoding.UTF8.GetString(data);
                // var num = int.Parse(str);
                // Log.Info($"UDP Num:{str}");
                // socket.SendUdp((num + 1).ToString());
                // Log.Info("Svr:" + Encoding.UTF8.GetString(data));
            }
        }
    }
}
