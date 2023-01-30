using ECSharp.Utils;
using System.Diagnostics;

namespace ECSharp.Time
{
    /// <summary>
    /// 时间流 管理器
    /// </summary>
    internal static class TimeFlowManager
    {
        /// <summary>
        /// 最大处理任务线程数量 为逻辑处理器数量
        /// </summary>
        private readonly static int MAX_HANDLE_TASK_THREAD = SystemInfo.ProcessorCount;

        /// <summary>
        /// 时间流控制线程
        /// </summary>
        private readonly static TimeFlowThread[] threads = new TimeFlowThread[MAX_HANDLE_TASK_THREAD + 1];

        /// <summary>
        /// 秒表器
        /// </summary>
        private readonly static Stopwatch stopwatch = Stopwatch.StartNew();

        /// <summary>
        /// 程序运行总时长
        /// </summary>
        internal static long TotalRunTime => stopwatch.ElapsedMilliseconds;

        /// <summary>
        /// 压入一个时间流继承对象
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="isSync">同步标记</param>
        internal static void PushTimeFlow(BaseTimeFlow tf, bool isSync)
        {
            // 同步线程
            if (isSync)
            {
                // 压入操作
                if (threads[0] == null)
                {
                    threads[0] = new TimeFlowThread(true);
                }

                threads[0].Push(tf);
                return;
            }

            // 查找适用的时间流存储器
            int minQueueTaskTfCount = int.MaxValue;
            TimeFlowThread? thread = null;

            for (int i = 1; i <= MAX_HANDLE_TASK_THREAD; i++)
            {
                if (threads[i] == null)
                {
                    thread = threads[i] = new TimeFlowThread(false);
                    break;
                }

                var count = threads[i].GetTaskCount();

                if (count <= 100)
                {
                    thread = threads[i];
                    break;
                }

                if (count < minQueueTaskTfCount)
                {
                    minQueueTaskTfCount = count;
                    thread = threads[i];
                }
            }
            thread?.Push(tf);
        }

        /// <summary>
        /// 通过对象关闭时间流
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <returns></returns>
        internal static bool CloseByObj(ITimeUpdate timeUpdate)
        {
            for (int i = 0; i <= MAX_HANDLE_TASK_THREAD; i++)
            {
                if (threads[i] != null && threads[i].CloseByObj(timeUpdate))
                    return true;
                else
                    continue;
            }

            return false;
        }
    }
}
