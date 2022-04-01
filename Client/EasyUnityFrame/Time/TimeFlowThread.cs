#if UNITY_2017_1_OR_NEWER
using ES.Linq;
#endif
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ES.Time
{
    internal class TimeFlowThread
    {
        private Thread? thread;
        private List<BaseTimeFlow> timeFlows = new List<BaseTimeFlow>();
        private ConcurrentBag<BaseTimeFlow> waitTimeFlows = new ConcurrentBag<BaseTimeFlow>();
        internal int Index { private set; get; } = -1;
        /// <summary>
        /// 正在更新状态值
        /// </summary>
        internal bool IsRunning { private set; get; } = false;
        /// <summary>
        /// 是否停止推送任务
        /// </summary>
        internal bool IsPausePushTask { private set; get; } = false;

        /// <summary>
        /// 高精度模式
        /// </summary>
        internal static bool isHighPrecisionMode = false;

        /// <summary>
        /// 线程阻塞超时计数
        /// </summary>
        private int threadBlockTimeOutCount = 0;

        internal TimeFlowThread(int index)
        {
            Index = index;
        }

        internal void Start()
        {
            try
            {
                if (!IsRunning)
                {
                    IsRunning = true;
                    IsPausePushTask = false;
                    thread = new Thread(UpdateHandle);
                    thread.IsBackground = true;
                    thread.Start();
                }
            }
            catch
            {
                Start();
            }
        }

        internal int GetTaskCount()
        {
            if (IsPausePushTask) return int.MaxValue;
            else return timeFlows.Count;
        }

        internal void Push(BaseTimeFlow timeFlow)
        {
            waitTimeFlows.Add(timeFlow);
        }

        internal void CheckThreadSafe()
        {
            if (IsRunning && thread != null)
            {
                // 阻塞挂起
                if (thread.ThreadState == ThreadState.WaitSleepJoin) { thread.Interrupt(); }
                // 已经停止的
                else if (thread.ThreadState == ThreadState.Aborted || !thread.IsAlive) { Close(); }

                // 检测阻塞超时 10s
                Interlocked.Increment(ref threadBlockTimeOutCount);
                if (threadBlockTimeOutCount >= 10)
                {
                    Interlocked.Exchange(ref threadBlockTimeOutCount, 0);
                    BaseTimeFlow[] temp;
                    temp = timeFlows.ToArray();
                    // 超时终止当前线程并切换线程
                    Close();
                    // 创建新的时间线
                    var index = TimeFlowManager.Instance.CreateExtraTimeFlow();
                    for (int i = 0, len = temp.Length; i < len; i++)
                    {
                        if (temp[i].IsTimeUpdateActive()) TimeFlowManager.Instance.PushTimeFlow(temp[i], index);
                    }
                }
            }
        }

        /// <summary>
        /// 更新句柄
        /// 这个地方要优化，在原基础线程优化方案上改成自动增长的模式，检测线程里工作线数量与处理时长的比例是否对称和目标延迟是否对等，否则增加新的线程并且移动到新线程中
        /// 以及线程超时优化
        /// </summary>
        private void UpdateHandle()
        {
            // 时间补偿助手
            // TimeFix timeFixHelper = new TimeFix(TimeFlowManager.timeFlowPeriod);
            // 闲置处理时间计数
            int idlHandleTimeCount = 0;
            // 计算句柄超出间隔次数 
            int mathHandleTimeCount = 0;
            // 重置计算句柄超出间隔次数计数
            int mathHandleTimeResetCount = 0;
            // 暂停推送任务计数 超出1分钟尝试重新接管线程
            int pausePushTaskCount = 0;
            // 总体目标运行间隔时间
            int totalAllPeriod;
            // 转移其他线程组
            BaseTimeFlow[]? moveOtherThreadFlow = null;
            // 耗时监视器
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            // int currentPeriod = TimeFlowManager.timeFlowPeriod;
            while (IsRunning)
            {
                // timeFixHelper.Begin();
                int totalTime = 0;
                int len = timeFlows.Count;
                totalAllPeriod = 0;
                for (int i = len - 1; i >= 0; i--)
                {
                    BaseTimeFlow tf = timeFlows[i];
                    if (tf.IsTimeUpdateActive())
                    {
                        if (tf.isTimeFlowStop)
                        {
                            timeFlows.RemoveAt(i);
                            tf.UpdateEndES();
                        }
                        else if (!tf.isTimeFlowPause)
                        {
                            totalAllPeriod += tf.fixedTime;
                            tf.UpdateES(watch.Elapsed.TotalSeconds);
                            totalTime += tf.lastUseTime;
                        }
                    }
                }
                // 重置超时检测
                Interlocked.Exchange(ref threadBlockTimeOutCount, 0);

                // 无任务进行则关闭
                if (len <= 0)
                {
                    // 超出闲置时间跳出循环
                    if (++idlHandleTimeCount >= 1000000) break;
                }
                else
                {
                    if (idlHandleTimeCount > 0) idlHandleTimeCount = 0;
                    // index大于等于3为0 1 2核心线程不需要处理分离任务
                    if (Index >= 3)
                    {
                        // 超出运行算率3次 分割算率建立新时间线
                        if (len > 1 && totalTime * len > totalAllPeriod)
                        {
                            mathHandleTimeResetCount = 0;
                            if (++mathHandleTimeCount >= 100)
                            {
                                mathHandleTimeCount = 0;
                                IsPausePushTask = true;

                                // 移除一半内容进入新的时间线
                                int moveOtherThreadFlowIndex = 0;
                                moveOtherThreadFlow = new BaseTimeFlow[len - len / 2];
                                for (int i = len - 1, end = len / 2; i >= end; i--)
                                {
                                    BaseTimeFlow tf = timeFlows[i];
                                    if (tf.IsTimeUpdateActive())
                                    {
                                        moveOtherThreadFlow[moveOtherThreadFlowIndex++] = tf;
                                        timeFlows.RemoveAt(i);
                                    }
                                }
                            }
                        }
                        // 重置一次分割检测任务
                        if (++mathHandleTimeResetCount >= 100) { mathHandleTimeResetCount = 0; mathHandleTimeCount = 0; }

                        // 暂停接管任务缓和处理
                        if (IsPausePushTask)
                        {
                            if (++pausePushTaskCount >= 6000000)
                            {
                                pausePushTaskCount = 0;
                                IsPausePushTask = false;
                            }
                        }
                    }
                }
                // 在锁外迁移
                if (moveOtherThreadFlow != null)
                {
                    // 创建新的时间线
                    var index = TimeFlowManager.Instance.CreateExtraTimeFlow();
                    for (int i = 0, len1 = moveOtherThreadFlow.Length; i < len1; i++)
                    {
                        TimeFlowManager.Instance.PushTimeFlow(moveOtherThreadFlow[i], index);
                    }
                    moveOtherThreadFlow = null;
                }

                // 加入新的时间流
                timeFlows.AddRange(waitTimeFlows);
#if !UNITY_2020_1_OR_NEWER
                waitTimeFlows.Clear();
#else
				waitTimeFlows.ClearAll();
#endif

                // 精度调整
                if (isHighPrecisionMode) Thread.Yield();
                else Thread.Sleep(1);
                // currentPeriod = timeFixHelper.End();
            }
            // 线程结束时则重置为false
            timeFlows.Clear();
        }

        /// <summary>
        /// 通过对象关闭时间流
        /// </summary>
        /// <param name="timeUpdate"></param>
        /// <returns></returns>
        internal bool CloseByObj(ITimeUpdate timeUpdate)
        {
            if (timeFlows == null) return false;
            for (int i = 0, len = timeFlows.Count; i < len; i++)
            {
                if (timeFlows[i].CloseByObj(timeUpdate)) return true;
                else continue;
            }
            return false;
        }

        internal void Close()
        {
            IsRunning = false;
            IsPausePushTask = true;
            thread = null;
        }
    }
}
