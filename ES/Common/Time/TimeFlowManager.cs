using System;
using System.Collections.Generic;
using System.Threading;

namespace ES.Common.Time
{
    /// <summary>
    /// 时间流 管理器
    /// </summary>
    internal class TimeFlowManager
    {
        /// <summary>
        /// 静态单例
        /// </summary>
        private static TimeFlowManager instance = null;
        /// <summary>
        /// 获取单例对象
        /// </summary>
        internal static TimeFlowManager Instance { get { if (instance == null) instance = new TimeFlowManager(); return instance; } }
       
        /// <summary>
        /// 时间流控制线程 核心线程
        /// </summary>
        private readonly List<TimeFlowThread> timeFlowThreads;
        /// <summary>
        /// 锁
        /// </summary>
        private readonly object m_lock = new object();

        /// <summary>
        /// 监控线程
        /// </summary>
        private readonly Thread timeFlowThreadMoniter;

        /// <summary>
        /// 私有构造
        /// </summary>
        private TimeFlowManager()
        {
            // 最大处理任务线程数量 标准公式为 核心数 * 2 + 2 因为检测线程占用 1 个，所以此处只加了 1 个
            int MAX_HANDLE_TASK_THREAD = Environment.ProcessorCount * 2 + 1;
            // 不足四线程则改为4线程
            if (MAX_HANDLE_TASK_THREAD < 4) MAX_HANDLE_TASK_THREAD = 4;
            timeFlowThreads = new List<TimeFlowThread>();
            for (int i = 0; i < MAX_HANDLE_TASK_THREAD; i++) timeFlowThreads.Add(new TimeFlowThread(i));

            // 检测线程启动
            timeFlowThreadMoniter = new Thread(UpdateCheckTimeFlowThread);
            timeFlowThreadMoniter.IsBackground = true;
            timeFlowThreadMoniter.Start();
        }

        /// <summary>
        /// 压入一个时间流继承对象
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="tfIndex">数组前两个线程是给框架使用，0负责数据部分 1负责文件部分 2 单线程同步update</param>
        internal void PushTimeFlow(BaseTimeFlow tf, int tfIndex = -1)
        {
            // 查找适用的时间流存储器
            int minQueueTaskTfCount = int.MaxValue;
            int index = tfIndex;

            lock (m_lock)
            {
                if (tfIndex == -1)
                {
                    // 按单核算 最高为4 索引位最高为3 否则会出问题
                    for (int i = 3, len = timeFlowThreads.Count; i < len; i++)
                    {
                        var count = timeFlowThreads[i].GetTaskCount();
                        if (count < minQueueTaskTfCount)
                        {
                            minQueueTaskTfCount = count;
                            index = i;
                        }
                    }
                }
                // 如果线程没有启动则启动
                var timeFlows = timeFlowThreads[index];
                if (!timeFlows.IsRunning) timeFlows.Start();
                if (timeFlows == null) timeFlows = timeFlowThreads[timeFlowThreads.Count - 1];
                // 压入操作
                timeFlows.Push(tf);
            }
        }

        internal int CreateExtraTimeFlow()
        {
            lock (m_lock)
            {
                var len = timeFlowThreads.Count;
                timeFlowThreads.Add(new TimeFlowThread(len));
                return len;
            }
        }

        /// <summary>
        /// 更新检查阻塞线程并重启
        /// <para>检测周期为线程睡眠 1 秒</para>
        /// </summary>
        private void UpdateCheckTimeFlowThread()
        {
            while (true)
            {
                try
                {
                    // 睡眠
                    Thread.Sleep(1000);
                    lock (m_lock)
                    {
                        for (int i = timeFlowThreads.Count - 1; i >= 0; i--)
                        {
                            var thread = timeFlowThreads[i];
                            if (thread != null)
                            {
                                thread.CheckThreadSafe();
                            }
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 销毁所有更新
        /// <para>本次程序运行结束前都无法使用。建议只有在即将关闭前调用</para>
        /// </summary>
        internal void Destroy()
        {
            lock (m_lock)
            {
                for (int i = timeFlowThreads.Count - 1; i >= 0; i--)
                {
                    var thread = timeFlowThreads[i];
                    if (thread != null)
                    {
                        thread.Close();
                    }
                }
            }
        }
    }
}
