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
        private Thread[] timeFlowThread = null;
        /// <summary>
        /// 正在更新状态值
        /// </summary>
        private bool[] isHandleUpdates;

        /// <summary>
        /// 私有构造
        /// </summary>
        private TimeFlowManager()
        {
            // 最大处理任务线程数量
            int MAX_HANDLE_TASK_THREAD = Environment.ProcessorCount * 2 + 2;
            // 先初始化10个队列
            weakReferences = new Queue<WeakReference<TimeFlow>>[MAX_HANDLE_TASK_THREAD];
            isHandleUpdates = new bool[MAX_HANDLE_TASK_THREAD];
            timeFlowThread = new Thread[MAX_HANDLE_TASK_THREAD];
            for (int i = 0, len = weakReferences.Length; i < len; i++)
            {
                isHandleUpdates[i] = false;
                weakReferences[i] = new Queue<WeakReference<TimeFlow>>();
            }
        }

        /// <summary>
        /// 压入一个时间流继承对象
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="tfIndex">数组前两个线程是给框架使用，0负责数据部分 1负责文件部分</param>
        internal void PushTimeFlow(TimeFlow tf, int tfIndex = -1)
        {
            // 查找适用的时间流存储器
            Queue<WeakReference<TimeFlow>> timeFlows = null;
            int len = weakReferences.Length;
            int minQueueTaskTfCount = int.MaxValue;
            int index = tfIndex;
            if (tfIndex == -1)
            {
                for (int i = 2; i < len; i++)
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
                timeFlowThread[index] = new Thread(UpdateHandle);
                timeFlowThread[index].IsBackground = true;
                timeFlowThread[index].Start(index);
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
                            if (!tf.isTimeFlowPause) tf.Update(timeFlowPeriod - currentPeriod);
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
