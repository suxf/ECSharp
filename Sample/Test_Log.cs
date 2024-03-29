﻿using ECSharp;

namespace Sample
{
    /// <summary>
    /// 日志类
    /// </summary>
    class Test_Log
    {
        /// <summary>
        /// 日志生成类
        /// 本类可以自动在预定的目录预定的周期写入相关日志信息
        /// 具体的功能点没有太多说明 详细可以使用后参考
        /// </summary>
        public Test_Log()
        {
            // LogConfig.CONSOLE_OUTPUT_LOG_TYPE = LogType.WARN;
            // LogConfig.LOG_FILE_STACK_TRACE_OUTPUT = true;
            // LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT = true;
            // LogConfig.LOG_CONSOLE_ASYNC_OUTPUT = false;
            // 此处只举例一个配置例子
            // 在首次调用Log前如果需要自定义一些配置输出
            // 可以使用Log进行更改
            LogConfig.LOG_PERIOD = 1000;
            // Log.LOG_CONSOLE_OUTPUT = false;
            // 以下四个函数均为普通分级日志输出函数
            LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT = true;
            Log.Debug("debug is this");
            Log.Info("Info is this");
            Log.Warn("warn is this");
            Log.Error("error is this");
            LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT = false;
            // 此函数可以写在try catch中 用于打印异常问题
            Log.Exception(new System.Exception(), "exception is this");

            // 测试对象传入
            Log.Debug(new object());
            Log.Debug(null, "|", new object());

            Test();
        }

        private void Test()
        {
            LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT = true;
            Log.Debug("debug is this 2");
            Log.Info("Info is this 2");
            Log.Warn("warn is this 2");
            Log.Error("error is this 2");
            Log.Exception(new System.Exception(), "exception is this 2");
            Log.Exception(new System.Exception("object test"), "exception is this 2", new object());
            LogConfig.LOG_CONSOLE_STACK_TRACE_OUTPUT = false;
        }
    }
}
