using BenchmarkDotNet.Running;
using System;

namespace Sample
{
    /// <summary>
    /// DEMO的使用可以通过在Progam创建该对象，以及调用不同函数来实现不同demo的展现
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // sqlserver数据库测试
            // new Test_DBSqlServer();
            // mysql数据库测试
            // new Test_DBMySql();
            // 日志测试
            // new Test_Log();
            // redis测试
            // new Test_Redis();
            // 数据存储测试
            // new Test_EasyStorage();
            // 时间流测试
            new Test_Time();
            // 随机数测试
            // new Test_RandomCode();
            // 数据流测试
            // new Test_SweetStream().Test1();
            // 数据流性能测试
            // BenchmarkRunner.Run<Test_SweetStream>();
            // 超级套接字测试
            // new Test_HyperSocket();
            // 简单聊天房测试
            // new Test_SimpleChatRoom();
            // 不停服热更新测试
            // new Test_Hotfix();
            // http服务测试
            // new Test_Http();
            // 工具类测试
            // new Test_Utils();
            // 可变变量测试
            // new Test_Variant();
            // Json转对象测试
            // new Test_Json2Object();

            Console.ReadLine();
        }

    }
}
