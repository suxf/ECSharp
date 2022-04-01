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
            TimeClock.Create(delegate(DateTime time) {
                Log.Info($"Time Now Alarm Clock 1:{time}"); 
            }, 2022, 1, 1, 0, 0, 0).Start(true);

            TimeClock.Create(delegate (DateTime time)
            {
                Log.Info($"Time Now Alarm Clock 2:{time}");
            }, "00:00:00").Start(true);

            // 假设我们有特殊的需求需要关闭此对象的更新可以调用
            // 如果可能尽可能在不再使用时调用此函数
            // Close();
            // 同样需要关闭一切进程中的更新可以使用此类的静态函数
            // TimeFlow.CloseAll();
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
            TimeCaller caller = TimeCaller.Create(delegate { Log.Info("Hello TimeCaller"); },
                2000, 10000, TimeCaller.Infinite).Start();
            // 创建一个带守护的执行器
            TimeCaller.Create(delegate { Log.Info("Hello TimeCaller 2"); }, 2000, 1600, TimeCaller.Infinite).Start(true);

            // 接下来就是贯穿在以上两个类的一个重要类
            // TimeFix 时间修正类 当然需要特殊处理时间循环的时候可以单独使用这个类
            // 只需要告诉这个对象 想要的循环时间周期即可完成必要的时间修补
            // 注意此类没有沉睡Sleep函数 所以等待需要自己处理
            int periodNow = 1000;
            TimeFix timeFix = new TimeFix(1000);

            /* 这里我没有写Thread 那么就认为这个Thread开始 */
            // 如果在一个循环中 这个函数应该就在循环的最开始处
            timeFix.Begin();
            // 沉睡相应时间
            Thread.Sleep(periodNow);
            // 打印日志
            Log.Info("Hello TimeFix");
            // 如果在一个循环中 这个函数应该在循环的最后结束
            periodNow = timeFix.End();
            /* 至此第一次Thread循环结束 第二次开始 */
            timeFix.Begin();
            Thread.Sleep(periodNow);
            Log.Info("Hello TimeFix2");
            periodNow = timeFix.End();
            /* 以此循环往复 而periodNow的值会根据每次的耗时不同进行不断的修正 */
            

            // 临时变量 测试时间流自动停止
            // StartTempTime();
            // 需要gc回收一下
            // 此处gc不会影响当前大括号的其他定时器，因为这个函数域还没结束
            GC.Collect();
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
            // Thread.Sleep(500);
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
