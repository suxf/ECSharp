﻿using ES.Common.Time;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ES.Common.Log
{
    /// <summary>
    /// 日志管理器
    /// <para>周期性写入文件</para>
    /// <para>周期LOG_PERIOD、写入路径LOG_PATH和分文件大小限制LOG_UNIT_FILE_MAX_SIZE可以直接调用静态修改（程序启动时未第一次调用就应修改完成）</para>
    /// </summary>
    internal class LogManager : ITimeUpdate
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
        private readonly string logId = null;
        /// <summary>
        /// 日志索引，如果单个时间内日志太大则分开
        /// </summary>
        private int logIndex = 0;
        /// <summary>
        /// 进程名称
        /// </summary>
        private readonly string proccessName = "";

        private readonly BaseTimeFlow timeFlow;

        /// <summary>
        /// 构造函数
        /// </summary>
        private LogManager()
        {
            proccessName = Process.GetCurrentProcess().ProcessName.ToLower();
            logId = new Random().Next(100, 999).ToString();
            logInfos = new ConcurrentQueue<LogInfo>();
            // 创建目录
            if (!Directory.Exists(Log.LOG_PATH))
            {
                Directory.CreateDirectory(Log.LOG_PATH);
            }

            timeFlow = BaseTimeFlow.CreateTimeFlow(this, 1);
            timeFlow.StartTimeFlowES();
        }

        private int periodNow = 0;
        /// <summary>
        /// 系统调用
        /// </summary>
        /// <param name="dt"></param>
        public void Update(int dt)
        {
            periodNow += TimeFlow.period;
            if (periodNow >= Log.LOG_PERIOD)
            {
                periodNow = 0;

                // 如果没有日志则不处理
                if (logInfos.Count <= 0) return;
                // 创建当日目录
                if (!Directory.Exists(Log.LOG_PATH + DateTime.Now.ToString("yyyy_MM_dd/")))
                {
                    Directory.CreateDirectory(Log.LOG_PATH + DateTime.Now.ToString("yyyy_MM_dd/"));
                }
                string filename = Log.LOG_PATH + string.Format(DateTime.Now.ToString("yyyy_MM_dd/{2}_HH_{0}_{1}{3}"), logIndex, logId, proccessName, Log.LOG_FILE_SUFFIX);
                if (!File.Exists(filename)) fileInfo = null;
                // 检查文件
                if (fileInfo == null)
                    fileInfo = new FileInfo(filename);
                else
                    fileInfo.Refresh();
                if (fileInfo.Exists)
                {
                    if (fileInfo.Length > Log.LOG_UNIT_FILE_MAX_SIZE)
                    {
                        fileInfo = new FileInfo(Log.LOG_PATH + string.Format(DateTime.Now.ToString("yyyy_MM_dd/{2}_HH_{0}_{1}{3}"), ++logIndex, logId, proccessName, Log.LOG_FILE_SUFFIX));
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
                    sb.AppendFormat("{0} {1}", log.time, log.type);
                    if (!string.IsNullOrEmpty(log.spaceName)) sb.AppendFormat(" {0} ", log.spaceName);
                    if (!string.IsNullOrEmpty(log.className)) sb.AppendFormat(" {0} ", log.className);
                    if (!string.IsNullOrEmpty(log.methodName)) sb.AppendFormat(" {0} ", log.methodName);
                    sb.AppendFormat(":{0}", log.data);
                    if (Log.LOG_CONSOLE_OUTPUT) Console.WriteLine(sb.ToString());
                    using (StreamWriter sw = fileInfo.AppendText()) sw.WriteLine(sb.ToString());
                }
            }
        }

        /// <summary>
        /// 停止更新
        /// </summary>
        public void UpdateEnd()
        {
        }
    }
}
