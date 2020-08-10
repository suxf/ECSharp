using ES.Common.Time;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ES.Common.Log
{
    /// <summary>
    /// 日志管理器
    /// 周期性写入文件
    /// 周期LOG_PERIOD、写入路径LOG_PATH和分文件大小限制LOG_UNIT_FILE_MAX_SIZE可以直接调用静态修改（程序启动时未第一次调用就应修改完成）
    /// </summary>
    internal class LogManager : TimeFlow
    {
        /// <summary>
        /// 单例静态对象
        /// </summary>
        private static LogManager instance = null;
        /// <summary>
        /// 获取单例
        /// </summary>
        internal static LogManager Instance { get { if (instance == null) instance = new LogManager(); return instance; } }

        /// <summary>
        /// 日志数据队列
        /// </summary>
        internal ConcurrentQueue<LogInfo> logInfos = null;
        /// <summary>
        /// 文件信息
        /// </summary>
        private FileInfo fileInfo = null;
        /// <summary>
        /// 日志ID
        /// </summary>
        private string logId = null;
        /// <summary>
        /// 日志索引，如果单个时间内日志太大则分开
        /// </summary>
        private int logIndex = 0;
        /// <summary>
        /// 进程名称
        /// </summary>
        private string proccessName = "";

        /// <summary>
        /// 构造函数
        /// </summary>
        private LogManager() : base(1)
        {
            proccessName = Process.GetCurrentProcess().ProcessName.ToLower();
            logId = new Random().Next(100, 999).ToString();
            logInfos = new ConcurrentQueue<LogInfo>();
            // 创建目录
            if (!Directory.Exists(LogConfig.LOG_PATH))
            {
                Directory.CreateDirectory(LogConfig.LOG_PATH);
            }
        }

        private int periodNow = 0;
        /// <summary>
        /// 系统调用
        /// </summary>
        /// <param name="dt"></param>
        protected override void Update(int dt)
        {
            periodNow += timeFlowPeriod;
            if (periodNow >= LogConfig.LOG_PERIOD)
            {
                periodNow = 0;

                // 如果没有日志则不处理
                if (logInfos.Count <= 0) return;
                // 创建当日目录
                if (!Directory.Exists(LogConfig.LOG_PATH + DateTime.Now.ToString("yyyy_MM_dd/")))
                {
                    Directory.CreateDirectory(LogConfig.LOG_PATH + DateTime.Now.ToString("yyyy_MM_dd/"));
                }
                string filename = LogConfig.LOG_PATH + string.Format(DateTime.Now.ToString("yyyy_MM_dd/{2}_HH_{0}_{1}{3}"), logIndex, logId, proccessName, LogConfig.LOG_FILE_SUFFIX);
                if (!File.Exists(filename)) fileInfo = null;
                // 检查文件
                if (fileInfo == null)
                    fileInfo = new FileInfo(filename);
                else
                    fileInfo.Refresh();
                if (fileInfo.Exists)
                {
                    if (fileInfo.Length > LogConfig.LOG_UNIT_FILE_MAX_SIZE)
                    {
                        fileInfo = new FileInfo(LogConfig.LOG_PATH + string.Format(DateTime.Now.ToString("yyyy_MM_dd/{2}_HH_{0}_{1}{3}"), ++logIndex, logId, proccessName, LogConfig.LOG_FILE_SUFFIX));
                        FileStream fs = fileInfo.Create();
                        fs.Close();
                        fileInfo.Refresh();
                    }
                }
                else
                {
                    FileStream fs = fileInfo.Create();
                    fs.Close();
                    fileInfo.Refresh();
                }

                // 写入日志
                while (logInfos.TryDequeue(out LogInfo log))
                {
                    if (log.data == null) continue;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0} {1} [", log.time, log.type);
                    if (log.spaceName != null) sb.AppendFormat(" {0} ", log.spaceName);
                    if (log.className != null) sb.AppendFormat(" {0} ", log.className);
                    if (log.methodName != null) sb.AppendFormat(" {0} ", log.methodName);
                    sb.AppendFormat("]:{0}", log.data);
                    if (LogConfig.LOG_CONSOLE_OUTPUT) Console.WriteLine(sb.ToString());
                    using (StreamWriter sw = fileInfo.AppendText()) sw.WriteLine(sb.ToString());
                }
            }
        }
    }
}
