using ES.Utils;
using System.Collections.Concurrent;
using System.Threading;

namespace ES.Time
{
    /// <summary>
    /// 时间流 管理器
    /// </summary>
    internal class TimeFlowManager
    {
        /// <summary>
        /// 静态单例
        /// </summary>
        private static TimeFlowManager? instance = null;
        /// <summary>
        /// 获取单例对象
        /// </summary>
        internal static TimeFlowManager Instance { get { if (instance == null) instance = new TimeFlowManager(); return instance; } }

        /// <summary>
        /// 时间流控制线程
        /// </summary>
        private readonly ConcurrentBag<TimeFlowThread> timeFlowThreads;
        /// <summary>
        /// 核心时间流控制线程
        /// </summary>
        private readonly TimeFlowThread[] kernelTimeFlowThreads;

        /*
        /// <summary>
        /// 监控线程
        /// </summary>
        private readonly Thread timeFlowThreadMoniter;
        */

        /// <summary>
        /// 私有构造
        /// </summary>
        private TimeFlowManager()
        {
            // 最大处理任务线程数量 标准公式为 核心数 * 2 + 2 因为检测线程占用 1 个，所以此处只加了 1 个
            int MAX_HANDLE_TASK_THREAD = SystemInfo.ProcessorCount * 2 + 1;
            // 不足四线程则改为4线程
            if (MAX_HANDLE_TASK_THREAD < 4) MAX_HANDLE_TASK_THREAD = 4;
            timeFlowThreads = new ConcurrentBag<TimeFlowThread>();
            kernelTimeFlowThreads = new TimeFlowThread[3];
            for (int i = 0; i < MAX_HANDLE_TASK_THREAD; i++)
            {
                if (i < 3) kernelTimeFlowThreads[i] = new TimeFlowThread(i);
                else timeFlowThreads.Add(new TimeFlowThread(i));
            }

            /*
            // 检测线程启动
            timeFlowThreadMoniter = new Thread(UpdateCheckTimeFlowThread);
            timeFlowThreadMoniter.IsBackground = true;
            timeFlowThreadMoniter.Start();
            */
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
            TimeFlowThread? timeFlowThread = null;

            if (tfIndex == -1 || tfIndex >= 3)
            {
                // 按单核算 最高为4 索引位最高为3 否则会出问题
                foreach (var thread in timeFlowThreads)
                {
                    var count = thread.GetTaskCount();
                    if (count < minQueueTaskTfCount)
                    {
                        minQueueTaskTfCount = count;
                        timeFlowThread = thread;
                    }
                }
            }
            else
            {
                timeFlowThread = kernelTimeFlowThreads[tfIndex];
            }

            if (timeFlowThread == null && !timeFlowThreads.TryPeek(out timeFlowThread))
            {
                return;
            }
            // 压入操作
            timeFlowThread.Push(tf);
        }

        // internal int CreateExtraTimeFlow()
        // {
        //     var len = timeFlowThreads.Count;
        //     timeFlowThreads.Add(new TimeFlowThread(len));
        //     return len;
        // }

        /// <summary>
        /// 通过对象关闭时间流
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <returns></returns>
        internal bool CloseByObj(ITimeUpdate timeUpdate)
        {
            foreach (var thread in kernelTimeFlowThreads)
            {
                if (thread.CloseByObj(timeUpdate)) return true;
                else continue;
            }
            foreach (var thread in timeFlowThreads)
            {
                if (thread.CloseByObj(timeUpdate)) return true;
                else continue;
            }
            return false;
        }

        /*
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
                    foreach (var thread in kernelTimeFlowThreads)
                        thread.CheckThreadSafe();
                    foreach (var thread in timeFlowThreads)
                        thread.CheckThreadSafe();
                }
                catch { }
            }
        }
        */

        /*
        /// <summary>
        /// 销毁所有更新
        /// <para>本次程序运行结束前都无法使用。建议只有在即将关闭前调用</para>
        /// </summary>
        internal void Destroy()
        {
            foreach (var thread in kernelTimeFlowThreads)
                thread.Close();
            foreach (var thread in timeFlowThreads)
                thread.Close();
        }
        */
    }
}
