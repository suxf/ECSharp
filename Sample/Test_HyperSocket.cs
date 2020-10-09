﻿using ES.Common.Time;
using ES.Network.HyperSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Sample
{
    /// <summary>
    /// 超级套接字演示
    /// </summary>
    class Test_HyperSocket
    {
        static HyperSocket[] sockets = new HyperSocket[10000];
        static TimeCaller[] timeCaller1 = new TimeCaller[10000];
        static TimeCaller[] timeCaller2 = new TimeCaller[10000];
        static Thread[] threads = new Thread[10000];
        static int index = 0;
        static int num = 0;
        static List<int> ssss = new List<int>();

        static Random rd = new Random();

        public Test_HyperSocket()
        {
            Console.WriteLine("[1]服务器 [2]客户端");
            Console.Write("输入:");
            var input = Console.ReadLine();
            if (input == "1")
            {
                Console.WriteLine("启动服务器");
                StartServer(0);
            }
            else if (input == "2")
            {
                Console.WriteLine("启动客户端");
                for (int i = 1; i <= 200; i++) StartClient(i);
            }
            else Console.WriteLine("啥都没");
        }

        public void StartServer(int i)
        {
            sockets[i] = HyperSocket.CreateServer("127.0.0.1", 8888, 600, new ServerListener());
        }

        public void StartClient(int i)
        {
            threads[i] = new Thread(delegate ()
            {
                Thread.Sleep(rd.Next(100, 10000));
                sockets[i] = HyperSocket.CreateClient("127.0.0.1", 8888, new ClientListener());
            });
            threads[i].Start();
        }

        class ClientListener : IHyperSocketClientListener
        {
            bool fff = false;
       
            public void OnError(HyperSocket socket, Exception ex)
            {
                if (fff) --num;
                Console.WriteLine($"Connect Num:{num}");
                Console.WriteLine(ex.Message);
            }

            public void OnOpen(HyperSocket socket)
            {
                // timeCaller1[i] = new TimeCaller(rd.Next(1000, 5000), rd.Next(500, 1000), true, -1, () =>
                // {
                // socket.SendTcp(1.ToString());
                // });
                fff = true;
                Console.WriteLine($"Connect Num:{++num}");
                // Console.WriteLine($"Connect OK:{socket.SessionId}");
                timeCaller2[index++] = new TimeCaller(1000, 50, true, -1, (long count) =>
                {
                    socket.SendUdp(count.ToString());
                });
            }

            public void OnTcpReceive(byte[] data, HyperSocket socket)
            {
                // Console.WriteLine("Svr:" + Encoding.UTF8.GetString(data));
                string str = Encoding.UTF8.GetString(data);
                var num = int.Parse(str);
                // if(num % 10000 == 0) Console.Clear();
                // Console.WriteLine($"SocketID:{socket.SessionId}, TCP Num:{str}");
                // socket.SendUdp((num + 1).ToString());
            }

            public void OnUdpReceive(byte[] data, HyperSocket socket)
            {
                // string str = Encoding.UTF8.GetString(data);
                // var num = int.Parse(str);
                // Console.WriteLine($"UDP Num:{str}");
                // socket.SendUdp((num + 1).ToString());
                // Console.WriteLine("Svr:" + Encoding.UTF8.GetString(data));
            }
        }

        class ServerListener : IHyperSocketServerListener
        {
            

            public void OnClose(RemoteHyperSocket socket)
            {
                if(ssss.Remove(socket.SessionId)) Console.WriteLine($"【OnClose】Connect Num:{ssss.Count}");
                // Console.WriteLine($"Socket Session Close:{socket.SessionId}");
            }

            public void OnError(Exception ex)
            {
                Console.WriteLine($"【OnError】Connect Message:{ex.Message}");
            }

            public void OnOpen(RemoteHyperSocket socket)
            {
                ssss.Add(socket.SessionId);
                Console.WriteLine($"【OnOpen】Connect Num:{ssss.Count}");
                // Console.WriteLine($"Connect OK:{socket.SessionId}");
                socket.Tag = 1;
            }

            public void OnTcpReceive(byte[] data, RemoteHyperSocket socket)
            {
                socket.SendTcp(Encoding.UTF8.GetString(data));
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
                //         Console.WriteLine($"SessionId TCP:{socket.SessionId}, Num:{num}");
                //     }
                //     else { socket.Tag = num; Console.WriteLine($"SessionId TCP:{socket.SessionId}, OK"); }
                //     socket.SendTcp((num + 1).ToString());
                // }
                // Console.WriteLine($"TCP.Cnt[{socket.SessionId}-{socket.GetRemoteIp()}]:" + Encoding.UTF8.GetString(data));
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
                //     if((int)socket.Tag + 2 != num) Console.WriteLine($"SessionId UDP:{socket.SessionId}, Num:{num}");
                //     else { socket.Tag = num; Console.WriteLine($"SessionId UDP:{socket.SessionId}, OK"); }
                //     socket.SendTcp((num + 1).ToString());
                // }

                socket.SendTcp(Encoding.UTF8.GetString(data));
                // Console.WriteLine($"UDP.Cnt[{socket.SessionId}-{socket.GetRemoteIp()}]:" + Encoding.UTF8.GetString(data));
                // socket.SendUdp("Hello World Svr 5:" + Interlocked.Increment(ref num));
            }
        }
    }
}
