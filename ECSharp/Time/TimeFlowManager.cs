#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using ECSharp.Utils;
using System.Reflection;

namespace ECSharp.Time
{
    /// <summary>
    /// 时间流 管理器
    /// </summary>
    internal static class TimeFlowManager
    {
        /// <summary>
        /// 时间流控制线程
        /// </summary>
        private readonly static TimeFlowThread[] threads = new TimeFlowThread[SystemInfo.ProcessorCount + 1];

        /// <summary>
        /// 压入一个时间流继承对象
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="isSync">同步标记</param>
        internal static void PushTimeFlow(BaseTimeFlow tf, bool isSync)
        {
#if UNITY_2020_1_OR_NEWER
            isSync = true;
#endif

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

            for (int i = 1; i <= SystemInfo.ProcessorCount; i++)
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
            for (int i = 0; i <= SystemInfo.ProcessorCount; i++)
            {
                if (threads[i] != null && threads[i].CloseByObj(timeUpdate))
                    return true;
                else
                    continue;
            }

            return false;
        }

#if UNITY_2020_1_OR_NEWER
        internal static System.Collections.IEnumerator OnUnityUpdate()
        {
            yield return threads[0]?.OnUnityUpdate(threads[0]);
        }
#endif

        internal static void DoWithAssembly(int action, Assembly? assembly)
        {
            if (assembly == null) return;

            for (int i = 0; i <= SystemInfo.ProcessorCount; i++)
            {
                if (threads[i] == null)
                    continue;

                threads[i].DoWithAssembly(action, assembly);
            }
        }
    }
}
