using System.Collections.Concurrent;

namespace ECSharp.Linq
{
    /// <summary>
    /// 拓展方法类
    /// <para>Concurrent系列拓展</para>
    /// </summary>
    public static class ConcurrentLinq
    {
        /// <summary>
        /// 清空 拓展方法提供
        /// </summary>
        /// <param name="concurrentQueue"></param>
        public static void ClearAll<T>(this ConcurrentQueue<T> concurrentQueue)
        {
            while (concurrentQueue.TryDequeue(out _)) ;
        }

        /// <summary>
        /// 清空 拓展方法提供
        /// </summary>
        /// <param name="concurrentStack"></param>
        public static void ClearAll<T>(this ConcurrentStack<T> concurrentStack)
        {
            while (concurrentStack.TryPop(out _)) ;
        }

        /// <summary>
        /// 清空 拓展方法提供
        /// </summary>
        /// <param name="concurrentBag"></param>
        public static void ClearAll<T>(this ConcurrentBag<T> concurrentBag)
        {
            while (concurrentBag.TryTake(out _)) ;
        }
    }
}
