using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;

namespace Sample
{
    public class Test_MemoryRW
    {
        public Test_MemoryRW()
        {
            BenchmarkRunner.Run<Test1>();
        }

        public class Test1
        {
            Random random = new Random();

            [Benchmark]
            public void Test3_A()
            {
                for (int i = 0; i < 10000; i++)
                {
                    byte[] data = new byte[1000];
                    random.NextBytes(data);
                    int ss = ES.Utils.ByteHelper.GetValidLength(data);
                }
            }

            [Benchmark]
            public void Test3_B()
            {
                for (int k = 0; k < 10000; k++)
                {
                    byte[] data = new byte[1000];
                    random.NextBytes(data);
                    int ss = GetValidLength(data);
                }
            }

            private static int GetValidLength(byte[] bytes)
            {
                int i = 0;
                if (null == bytes || 0 == bytes.Length) return i;
                for (; i < bytes.Length; i++)
                {
                    int index = i;
                    if (i + 8 < bytes.Length)
                    {
                        int r = bytes[index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index];
                        if (r == 0x00) break;
                    }
                }
                return i;
            }

            //[Benchmark]
            public void Test2_A()
            {
                byte[] result = new byte[500];
                for (int i = 0; i < 1000; i++)
                {
                    byte[] data = new byte[1000];
                    random.NextBytes(data);
                    // Buffer.BlockCopy(data, 300, result, 0, result.Length);
                    result[250] = 100;
                }
            }

            //[Benchmark]
            public void Test2_B()
            {
                Span<byte> span2 = stackalloc byte[500];
                for (int i = 0; i < 1000; i++)
                {
                    byte[] data = new byte[1000];
                    random.NextBytes(data);
                    Span<byte> span = data.AsSpan();
                    // span.Slice(300, span2.Length).CopyTo(span2);
                    span2[250] = 100;
                    span2.ToArray();
                }
            }

            //[Benchmark]
            public void Test1_A()
            {
                for(int i = 0; i < 1000; i++)
                {
                    byte[] data = new byte[1000];
                    random.NextBytes(data);
                    int add = 0;
                    for (int k = 0; k < data.Length; k++)
                    {
                        add += data[k];
                    }
                    // Console.WriteLine("T1:" + add);
                }
            }

            //[Benchmark]
            public void Test1_B()
            {
                for (int i = 0; i < 1000; i++)
                {
                    byte[] data = new byte[1000];
                    random.NextBytes(data);
                    Span<byte> span = data.AsSpan();
                    int add = 0;
                    for (int k = 0; k < span.Length; k++)
                    {
                        add += span[k];
                    }
                    // Console.WriteLine("T1:" + add);
                }
            }
        }
    }
}
