using BenchmarkDotNet.Running;
using ES;
using System;
using System.Diagnostics;

namespace Sample
{
    /// <summary>
    /// DEMO的使用可以通过在Progam创建该对象，以及调用不同函数来实现不同demo的展现
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Log.Info("测试开始...");
            Log.Info("[1] \tsqlserver数据库测试");
            Log.Info("[2] \tmysql数据库测试");
            Log.Info("[3] \t日志测试");
            Log.Info("[4] \tredis测试");
            Log.Info("[5] \t数据存储测试");
            Log.Info("[6] \t时间流测试");
            Log.Info("[7] \t随机数测试");
            Log.Info("[8] \t超级套接字测试");
            Log.Info("[9] \t简单聊天房测试");
            Log.Info("[10]\t不停服热更新测试");
            Log.Info("[11]\thttp服务测试");
            Log.Info("[12]\t工具类测试");
            Log.Info("[13]\t可变变量测试");
            string optionId = Log.ReadLine("选择要测试的功能序号:");
            switch (optionId)
            {
                // sqlserver数据库测试
                case "1": new Test_DBSqlServer(); break;
                // mysql数据库测试
                case "2": new Test_DBMySql(); break;
                // 日志测试
                case "3": new Test_Log(); break;
                // redis测试
                case "4": new Test_Redis(); break;
                // 数据存储测试
                case "5": new Test_EasyStorage(); break;
                // 时间流测试
                case "6": new Test_Time(); break;
                // 随机数测试
                case "7": new Test_RandomCode(); break;
                // 超级套接字测试
                case "8": new Test_HyperSocket(); break;
                // 简单聊天房测试
                case "9": new Test_SimpleChatRoom(); break;
                // 不停服热更新测试
                case "10": new Test_Hotfix(); break;
                // http服务测试
                case "11": new Test_Http(); break;
                // 工具类测试
                case "12": new Test_Utils(); break;
                // 可变变量测试
                case "13": new Test_Variant(); break;
            }
            System.Threading.Thread.Sleep(-1);
        }

    }
}
