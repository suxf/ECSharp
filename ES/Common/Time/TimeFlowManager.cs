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
        /// 更新队列
        /// </summary>
        internal Queue<WeakReference<TimeFlow>>[] weakReferences = null;
        /// <summary>
        /// 固定刷新周期
        /// 刷新固定时间：10ms
        /// </summary>
        internal const int timeFlowPeriod = 10;

        /// <summary>
        /// 时间流控制线程 核心线程
        /// </summary>
        private readonly Thread[] timeFlowThread;

        /// <summary>
        /// 监控线程
        /// </summary>
        private readonly Thread timeFlowThreadMoniter;

        /// <summary>
        /// 正在更新状态值
        /// </summary>
        private readonly bool[] isHandleUpdates;

        /// <summary>
        /// 私有构造
        /// </summary>
        private TimeFlowManager()
        {
            // 最大处理任务线程数量 标准公式为 核心数 * 2 + 2 因为检测线程占用 1 个，所以此处只加了 1 个
            int MAX_HANDLE_TASK_THREAD = Environment.ProcessorCount * 2 + 1;
            // 不足四线程则改为4线程
            if (MAX_HANDLE_TASK_THREAD < 4) MAX_HANDLE_TASK_THREAD = 4;
            // 先初始化10个队列
            weakReferences = new Queue<WeakReference<TimeFlow>>[MAX_HANDLE_TASK_THREAD];
            isHandleUpdates = new bool[MAX_HANDLE_TASK_THREAD];
            timeFlowThread = new Thread[MAX_HANDLE_TASK_THREAD];
            for (int i = 0, len = weakReferences.Length; i < len; i++)
            {
                isHandleUpdates[i] = false;
                weakReferences[i] = new Queue<WeakReference<TimeFlow>>();
            }

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
        internal void PushTimeFlow(TimeFlow tf, int tfIndex = -1)
        {
            // 查找适用的时间流存储器
            Queue<WeakReference<TimeFlow>> timeFlows = null;
            int len = weakReferences.Length;
            int minQueueTaskTfCount = int.MaxValue;
            int index = tfIndex;
            if (tfIndex == -1)
            {
                // 按单核算 最高为4 索引位最高为3 否则会出问题
                for (int i = 3; i < len; i++)
                {
                    lock (weakReferences[i])
                    {
                        if (weakReferences[i].Count < minQueueTaskTfCount)
                        {
                            minQueueTaskTfCount = weakReferences[i].Count;
                            index = i;
                        }
                    }
                }
            }
            timeFlows = weakReferences[index];
            // 如果线程没有启动则启动
            if (!isHandleUpdates[index])
            {
                isHandleUpdates[index] = true;
                lock (timeFlowThread)
                {
                    timeFlowThread[index] = new Thread(UpdateHandle);
                    timeFlowThread[index].IsBackground = true;
                    timeFlowThread[index].Start(index);
                }
            }

            if (timeFlows == null) timeFlows = weakReferences[len - 1];
            // 压入操作
            lock (timeFlows) timeFlows.Enqueue(new WeakReference<TimeFlow>(tf));
        }

        /// <summary>
        /// 更新句柄
        /// </summary>
        private void UpdateHandle(object o)
        {
            int index = (int)o;
            bool isHandleUpdate = isHandleUpdates[index];
            Queue<WeakReference<TimeFlow>> timeFlows = weakReferences[index];
            // 时间补偿助手
            TimeFix timeFixHelper = new TimeFix(timeFlowPeriod);
            // 闲置处理时间计数， 如果超出10s空处理则关闭线程
            int idlHandleTimeCount = 0;

            int currentPeriod = timeFlowPeriod;
            while (isHandleUpdate)
            {
                timeFixHelper.Begin();
                Thread.Sleep(currentPeriod);
                lock (timeFlows)
                {
                    for (int i = 0, len = timeFlows.Count; i < len; i++)
                    {
                        WeakReference<TimeFlow> reference = timeFlows.Dequeue();
                        if (reference.TryGetTarget(out TimeFlow tf))
                        {
                            if (!tf.isTimeFlowPause) tf.UpdateES(timeFlowPeriod - currentPeriod);
                            if (!tf.isTimeFlowStop) timeFlows.Enqueue(reference);
                            tf = null;
                        }
                    }
                    if (timeFlows.Count <= 0) { if (++idlHandleTimeCount >= 1000) break; }
                    else { if (idlHandleTimeCount > 0) idlHandleTimeCount = 0; }
                }
                currentPeriod = timeFixHelper.End();
            }
            // 线程结束时则重置为false
            isHandleUpdates[(int)o] = false;
        }

        /// <summary>
        /// 更新检查阻塞线程并重启
        /// 检测周期为线程睡眠 1 秒
        /// </summary>
        private void UpdateCheckTimeFlowThread()
        {
            while (true)
            {
                try
                {
                    // 睡眠
                    Thread.Sleep(1000);
                    lock (timeFlowThread)
                    {
                        for (int i = timeFlowThread.Length - 1; i >= 0; i--)
                        {
                            var thread = timeFlowThread[i];
                            if (thread != null)
                            {
                                // 阻塞挂起
                                if (thread.ThreadState == ThreadState.WaitSleepJoin) { thread.Interrupt(); }
                                // 已经停止的
                                else if (thread.ThreadState == ThreadState.Aborted || !thread.IsAlive) { timeFlowThread[i] = null; }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 销毁所有更新
        /// 本次程序运行结束前都无法使用。建议只有在即将关闭前调用
        /// </summary>
        internal void Destroy()
        {
            for (int i = 0, len = isHandleUpdates.Length; i < len; i++)
            {
                isHandleUpdates[i] = false;
            }
        }
    }
}
