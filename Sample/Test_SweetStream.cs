using ES.Utils;
using ES.Network.Sockets;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Sample
{
    class Test_SweetStream
    {

        public Test_SweetStream()
        {

            
            SweetStream sw = new SweetStream();
            while (true)
            {
                MemoryStream memoryStream = new MemoryStream();

                for (int i = 1; i <= 1000; i++)
                {
                    byte[] b = sw.Encode(Encoding.UTF8.GetBytes(RandomCode.Generate(new Random().Next(1, 5000))));
                    // byte[] b = sw.Encode(Encoding.UTF8.GetBytes("a"));
                    int size = 128 - memoryStream.ToArray().Length;
                    if (size > 0)
                    {
                        int sss = size - b.Length;
                        if (sss >= 0)
                        {
                            memoryStream.Write(b);
                        }
                        else
                        {
                            memoryStream.Write(b, 0, size);
                            sw.Decode(memoryStream.ToArray());
                            memoryStream = new MemoryStream();
                            memoryStream.Write(b, size, -sss);
                        }
                    }
                    else
                    {
                        sw.Decode(memoryStream.ToArray());
                        memoryStream = new MemoryStream();
                        memoryStream.Write(b);
                    }
                }

                if (memoryStream.ToArray().Length > 0) sw.Decode(memoryStream.ToArray());

                int count = 0;
                byte[] r = sw.TakeStreamBuffer();
                do
                {
                    count++;
                    Console.WriteLine(Encoding.UTF8.GetString(r));
                    r = sw.TakeStreamBuffer();
                } while (r != null);
                Console.WriteLine("all:" + count);

                Thread.Sleep(100);
                Console.ReadKey();
            }
        }

        public void Test2()
        {
            // emoryStream 与 纯byte组 赋值与取出 效率对比
            int all_loop_len = 100;
            int create_new_size = 100000;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            {
                for(int k = 0; k < all_loop_len; k++)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        for (int i = 0, len = create_new_size; i < len; i++)
                            ms.WriteByte(0xFF);
                        byte[] b = ms.ToArray();
                    }
                }
            }
            stopwatch.Stop();
            double use_time1 = stopwatch.Elapsed.TotalMilliseconds;
            stopwatch.Reset();
            stopwatch.Start();
            {
                for (int k = 0; k < all_loop_len; k++)
                {
                    byte[] buffer = new byte[create_new_size];
                    for (int i = 0, len = buffer.Length; i < len; i++)
                        buffer[i] = 0xFF;
                    byte[] b = buffer;
                }
            }
            stopwatch.Stop();
            double use_time2 = stopwatch.Elapsed.TotalMilliseconds;
            Console.WriteLine("MemoryStream:" + use_time1);
            Console.WriteLine("        byte:" + use_time2);
            Console.ReadLine();
        }

    }
}
