using System.Diagnostics;

namespace ES.Common.Time
{
    /// <summary>
    /// 时间修补助手
    /// 用于线程循环中执行时间不对等产生问题
    /// </summary>
    public class TimeFix
    {
        /// <summary>
        /// 监视器
        /// </summary>
        private Stopwatch sw = new Stopwatch();
        /// <summary>
        /// 循环执行理想周期
        /// </summary>
        private int period = 0;
        /// <summary>
        /// 当前执行周期
        /// </summary>
        private int current_period = 0;
        /// <summary>
        /// 需要修复的时间差
        /// </summary>
        private int fix_time = 0;

        /// <summary>
        /// 创建一个时间修补助手
        /// </summary>
        /// <param name="period">循环周期，精度：ms</param>
        public TimeFix(int period)
        {
            this.period = period;
            current_period = period;
        }

        /// <summary>
        /// 获取理想设置周期
        /// </summary>
        /// <returns>理想周期，精度：ms</returns>
        public int GetPeriod()
        {
            return period;
        }

        /// <summary>
        /// 需要执行消耗开始的地方
        /// </summary>
        public void Begin()
        {
            // 计时开始
            sw.Start();
        }

        /// <summary>
        /// 需要执行消耗结束的地方
        /// 此结果为下一次周期执行等待的时间，如需计算补差值，可以拿理想周期减去实际周期即可
        /// </summary>
        /// <returns>下一次执行的周期时间</returns>
        public int End()
        {
            // 计时结束
            sw.Stop();

            // 本次耗时
            int use_time = (int)sw.Elapsed.TotalMilliseconds;
            // 本次耗时 与 预想周期 差值
            int delta_time = use_time - current_period;
            // 剩余所有补时差
            int left_fix_time = fix_time + delta_time;

            // 偏差计算开始
            if (left_fix_time >= 0 || delta_time > 0)
            {
                // 本次周期提前或者刚好 或 补时差无或仍有剩余
                if (left_fix_time >= period)
                {
                    // 补时差 有剩余且大于或等于理想周期
                    fix_time = left_fix_time - period;
                    current_period = 0;
                }
                else
                {
                    // 补时差 有剩余但是不超过理想周期
                    fix_time = 0;
                    current_period = period - left_fix_time;
                }
            }
            else
            {
                // 补时差 无剩余且无法弥补现有时差
                fix_time = 0;
                current_period = period - left_fix_time;
            }

            // 计时重置
            sw.Reset();
            return current_period;
        }

    }
}
