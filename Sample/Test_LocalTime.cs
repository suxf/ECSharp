using BenchmarkDotNet.Attributes;
using ECSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    /// <summary>
    /// 测试本地时间
    /// </summary>
    public class Test_LocalTime
    {
        public Test_LocalTime()
        {
            Log.Info("DateTime:     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
            Log.Info("LocalTime:    " + LocalTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
            Log.Info("UtcDateTime:  " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
            Log.Info("UtcLocalTime: " + LocalTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:ffff"));

            Log.Info("TimeStamp:    " + LocalTime.TimeStamp);

            LocalTime.AddYears(1);
            Log.Info("AddYears 1 LocalTime: " + LocalTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
            LocalTime.AddHours(5);
            Log.Info("AddHours 5 LocalTime: " + LocalTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
            LocalTime.Sync();
            Log.Info("Sync LocalTime:       " + LocalTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));

            // BenchmarkDotNet.Running.BenchmarkRunner.Run<Test_LocalTime>();
        }

        [Benchmark]
        public void Test1()
        {
            DateTime dt = DateTime.Now;
        }

        [Benchmark]
        public void Test2()
        {
            DateTime dt = LocalTime.Now;
        }
    }
}
