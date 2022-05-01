using ES;
using ES.Time;
using System;
using System.Threading;

namespace Sample
{
    /// <summary>
    /// 时间帮助类测试
    /// </summary>
    class Test_Time : ITimeUpdate/* 继承此类可以周期性执行Update函数 */
    {
        double period1 = 0;
        private readonly TimeFlow timeFlow;

        public Test_Time()
        {
            // 时间闹钟
            TimeClock.Create(delegate (DateTime time)
            {
                Log.Info($"Time Now Alarm Clock 1:{time}");
            }, 2022, 1, 1, 0, 0, 0).Start(true);

            TimeClock.Create(delegate (DateTime time)
            {
                Log.Info($"Time Now Alarm Clock 2:{time}");
            }, "00:00:00").Start(true);

            // 测试同步和异步
            TimeCaller.Create(static delegate () { Log.Info("1"); }, 0, 5000, -1).Start(true);
            TimeCaller.Create(static delegate (){ Log.Info("2"); }, 0, 5000, -1).Start(true);
            TimeCaller.CreateSync(static delegate () { Log.Info("aa"); }, 0, 5000, -1).Start(true);
            TimeCaller.CreateSync(static delegate () { Log.Info("bb"); }, 0, 5000, -1).Start(true);
            // 假设我们有特殊的需求需要关闭此对象的更新可以调用
            // 如果可能尽可能在不再使用时调用此函数
            // Close();
            // 如果只是暂停的话 那么可以调用
            // Pause();
            // 再次恢复时间更新
            // Start();
            // 最后如果你需要查看此时update的状态话可以通过以下两个变量
            // isPause
            // isStop
            timeFlow = TimeFlow.Create(this, 1000);
            timeFlow.Start();
            // TimeFlow.SetHighPrecisionMode(true);
            // 时间执行器
            // 了解了TimeFlow有时候觉得太繁琐
            // 项目中根本不需要使用到那么就可以使用此类
            // 这个类根据类似Task设计思想开发
            // 但是他们还是有些区别，注意不管是这类函数操作中不要在包含一些特别耗时的操作
            // 比如说 Thread.Sleep 这种
            TimeCaller caller = TimeCaller.Create(static delegate (){ Log.Info("Hello TimeCaller"); },
                2000, 10000, TimeCaller.Infinite).Start();
            // 创建一个带守护的执行器
            TimeCaller.Create(static delegate () { Log.Info("Hello TimeCaller 2"); }, 2000, 1600, TimeCaller.Infinite).Start(true);
        }

        /// <summary>
        /// 可以从 timeFlowPeriod 直接获取周期时间
        /// dt为消耗时间的差值 因为程序不可能每次都精准10毫秒执行
        /// 所以update会不断调整时间执行时间 dt就是这个时间的差值
        /// 一般情况下不需要管理，因为在总时间循环中 几乎可以忽略 因为我们有自动修正
        /// </summary>
        /// <param name="dt"></param>
        public void Update(int dt)
        {
            /* 如果需要统计时间在处理就需要处理 */
            period1 += dt;
            /* 在此处可以处理预期过了时间的一些判定或者内容 */
            // 这里我们每1秒执行一次
            if (period1 >= 1000)
            {
                period1 = 0;
                Log.Info($"Hello TimeFlow:[{dt}]{DateTime.Now:yyyy-MM-dd HH:mm:ss:fffffff}");
            }
        }

        /// <summary>
        /// 停止更新
        /// </summary>
        public void UpdateEnd()
        {
            Log.Info("TimeFlow End");
        }

        private class Time2 : ITimeUpdate
        {
            public Time2()
            {
                TimeFlow.Create(this).Start();
            }

            int period1 = 0;
            public void Update(int deltaTime)
            {
                /* 如果需要统计时间在处理就需要处理 */
                period1 += deltaTime;

                /* 在此处可以处理预期过了时间的一些判定或者内容 */
                // 这里我们每5秒执行一次
                if (period1 >= 5000)
                {
                    period1 = 0;
                    Log.Info("Hello TimeFlow2");
                }
            }

            public void UpdateEnd()
            {

            }
        }
    }
}
