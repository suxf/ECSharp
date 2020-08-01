using ES.Common.Log;

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
            // 此处只举例一个配置例子
            // 在首次调用Log前如果需要自定义一些配置输出
            // 可以使用LogConfig进行更改 之后更改也将无效
            LogConfig.LOG_PERIOD = 1000;
            LogConfig.LOG_CONSOLE_OUTPUT = false;
            // 以下四个函数均为普通分级日志输出函数
            Log.Debug("debug is this");
            Log.Info("Info is this");
            Log.Warn("warn is this");
            Log.Error("error is this");
            // 此函数可以写在try catch中 用于打印异常问题
            Log.Exception(new System.Exception(), "exception is this");
        }
    }
}
