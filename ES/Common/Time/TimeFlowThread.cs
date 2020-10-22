using System;
using System.Collections.Generic;
using System.Threading;

namespace ES.Common.Time
{
    internal class TimeFlowThread
    {
        private Thread thread;
        private List<WeakReference<BaseTimeFlow>> timeFlows;
        private readonly object m_lock = new object();
        internal int index { private set; get; } = -1;
        /// <summary>
        /// 正在更新状态值
        /// </summary>
        internal bool IsRunning { private set; get; } = false;
        /// <summary>
        /// 是否停止推送任务
        /// </summary>
        internal bool IsPausePushTask { private set; get; } = false;

        internal TimeFlowThread(int index)
        {
            this.index = index;
        }

        internal void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                IsPausePushTask = false;
                timeFlows = new List<WeakReference<BaseTimeFlow>>();
                thread = new Thread(UpdateHandle);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        internal int GetTaskCount()
        {
            lock (m_lock)
            {
                if (timeFlows == null) return 0;
                else
                {
                    if (IsPausePushTask) return int.MaxValue;
                    else return timeFlows.Count;
                }
            }
        }

        internal void Push(BaseTimeFlow timeFlow)
        {
            lock (m_lock) timeFlows.Add(new WeakReference<BaseTimeFlow>(timeFlow));
        }

        internal void CheckThreadSafe()
        {
            if (thread != null)
            {
                // 阻塞挂起
                if (thread.ThreadState == ThreadState.WaitSleepJoin) { thread.Interrupt(); }
                // 已经停止的
                else if (thread.ThreadState == ThreadState.Aborted || !thread.IsAlive) { Close(); }
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
            TimeFix timeFixHelper = new TimeFix(TimeFlowManager.timeFlowPeriod);
            // 闲置处理时间计数， 1000次为10s 如果超出10s空处理则关闭线程
            int idlHandleTimeCount = 0;
            // 计算句柄超出间隔次数 1s内连续超出10次则分割任务 
            int mathHandleTimeCount = 0;
            // 重置计算句柄超出间隔次数计数 100次为1s 
            int mathHandleTimeResetCount = 0;
            // 暂停推送任务计数 6000次为60s 超出1分钟尝试重新接管线程
            int pausePushTaskCount = 0;
            // 转移其他线程组
            BaseTimeFlow[] moveOtherThreadFlow = null;

            int currentPeriod = TimeFlowManager.timeFlowPeriod;
            while (IsRunning)
            {
                timeFixHelper.Begin();
                lock (m_lock)
                {
                    int totalTime = 0;
                    var len = timeFlows.Count;
                    for (int i = len - 1; i >= 0; i--)
                    {
                        WeakReference<BaseTimeFlow> reference = timeFlows[i];
                        if (reference.TryGetTarget(out BaseTimeFlow tf))
                        {
                            if (tf.isTimeFlowStop)
                            {
                                timeFlows.RemoveAt(i);
                                tf.OnUpdateEndES();
                            }
                            else if (!tf.isTimeFlowPause)
                            {
                                tf.UpdateES(TimeFlowManager.timeFlowPeriod - currentPeriod);
                                totalTime += tf.lastUseTime;
                            }
                        }
                    }

                    // 无任务进行则关闭
                    if (len <= 0)
                    {
                        // 超出闲置时间跳出循环
                        if (++idlHandleTimeCount >= 1000) break;
                    }
                    else
                    {
                        if (idlHandleTimeCount > 0) idlHandleTimeCount = 0;
                        // index大于等于3为0 1 2核心线程不需要处理分离任务
                        if (index >= 3)
                        {
                            // 超出运行算率3次 分割算率建立新时间线
                            if (len > 1 && totalTime > TimeFlowManager.timeFlowPeriod)
                            {
                                if (++mathHandleTimeCount >= 10)
                                {
                                    mathHandleTimeCount = 0;
                                    IsPausePushTask = true;

                                    // 移除一半内容进入新的时间线
                                    int moveOtherThreadFlowIndex = 0;
                                    moveOtherThreadFlow = new BaseTimeFlow[len - len / 2];
                                    for (int i = len - 1, end = len / 2; i >= end; i--)
                                    {
                                        WeakReference<BaseTimeFlow> reference = timeFlows[i];
                                        if (reference.TryGetTarget(out BaseTimeFlow tf))
                                        {
                                            moveOtherThreadFlow[moveOtherThreadFlowIndex++] = tf;
                                            timeFlows.RemoveAt(i);
                                        }
                                    }
                                }

                                // 超过10s 重置一次分割检测任务
                                if (++mathHandleTimeResetCount >= 100) { mathHandleTimeResetCount = 0; mathHandleTimeCount = 0; }
                            }

                            // 暂停接管任务缓和处理
                            if (IsPausePushTask)
                            {
                                if (++pausePushTaskCount >= 6000)
                                {
                                    pausePushTaskCount = 0;
                                    IsPausePushTask = false;
                                }
                            }
                        }
                    }
                }
                // 在锁外迁移
                if (moveOtherThreadFlow != null)
                {
                    // 创建新的时间线
                    var index = TimeFlowManager.Instance.CreateExtraTimeFlow();
                    for (int i = 0, len = moveOtherThreadFlow.Length; i < len; i++)
                    {
                        TimeFlowManager.Instance.PushTimeFlow(moveOtherThreadFlow[i], index);
                    }
                    moveOtherThreadFlow = null;
                }

                Thread.Sleep(currentPeriod);
                currentPeriod = timeFixHelper.End();
            }
            // 线程结束时则重置为false
            Close();
        }

        internal void Close()
        {
            IsRunning = false;
        }
    }
}
