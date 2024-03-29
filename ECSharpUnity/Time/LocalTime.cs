﻿using System;
using System.Diagnostics;

namespace ECSharp
{
    /// <summary>
    /// 本地时间
    /// <para>具备程序级独立运行的时间,不受系统时间修改而影响</para>
    /// </summary>
    public static class LocalTime
    {
        /// <summary>
        /// 秒表器
        /// </summary>
        private readonly static Stopwatch sw = Stopwatch.StartNew();

        /// <summary>
        /// 时间节点
        /// </summary>
        private static long time = DateTime.Now.Ticks;

        /// <summary>
        /// 计时器运行时长
        /// </summary>
        public static long ElapsedMilliseconds => sw.ElapsedMilliseconds;

        /// <summary>
        /// 当前UTC时间
        /// </summary>
        public static DateTime UtcNow => Now.ToUniversalTime();

        /// <summary>
        /// 当前时间
        /// </summary>
        public static DateTime Now => new DateTime(time + sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond);

        /// <summary>
        /// 当前时间戳 秒级
        /// </summary>
        public static long TimeStamp => (time + sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond - 621355968000000000) / TimeSpan.TicksPerSecond;

        /// <summary>
        /// 本地时间同步当前本地时间一次
        /// </summary>
        public static void Sync()
        {
            Sync(DateTime.Now);
        }

        /// <summary>
        /// 本地时间同步一次秒级时间戳
        /// </summary>
        /// <param name="timeStamp">时间戳，单位：秒</param>
        public static void Sync(long timeStamp)
        {
            time = timeStamp * TimeSpan.TicksPerSecond + 621355968000000000 - sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 本地时间同步一次时间对象
        /// </summary>
        /// <param name="dt"></param>
        public static void Sync(DateTime dt)
        {
            time = dt.Ticks - sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 本地时间永久增加毫秒
        /// </summary>
        /// <param name="milliseconds">毫秒</param>
        public static void AddMilliseconds(int milliseconds)
        {
            time += milliseconds * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 本地时间永久增加秒
        /// </summary>
        /// <param name="seconds">秒</param>
        public static void AddSeconds(int seconds)
        {
            time += seconds * TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// 本地时间永久增加分钟
        /// </summary>
        /// <param name="minutes">分钟</param>
        public static void AddMinutes(int minutes)
        {
            time += minutes * TimeSpan.FromMinutes(1).Ticks;
        }

        /// <summary>
        /// 本地时间永久增加小时
        /// </summary>
        /// <param name="hours">小时</param>
        public static void AddHours(int hours)
        {
            time += hours * TimeSpan.FromHours(1).Ticks;
        }

        /// <summary>
        /// 本地时间永久增加天数
        /// </summary>
        /// <param name="days">天数</param>
        public static void AddDays(int days)
        {
            time += days * TimeSpan.FromDays(1).Ticks;
        }

        /// <summary>
        /// 本地时间永久增加月数
        /// </summary>
        /// <param name="months">月数</param>
        public static void AddMonths(int months)
        {
            time = Now.AddMonths(months).Ticks - sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 本地时间永久增加年数
        /// </summary>
        /// <param name="years">年数</param>
        public static void AddYears(int years)
        {
            time = Now.AddYears(years).Ticks - sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 日期时间转换为秒级时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ConvertToTimeStamp(DateTime dt)
        {
            return (dt.Ticks - 621355968000000000) / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// 秒级时间戳转换为日期时间
        /// </summary>
        /// <param name="ticks">秒时间戳</param>
        /// <return></return>
        public static DateTime ConvertFromTimeStamp(long ticks)
        {
            return new DateTime(ticks * TimeSpan.TicksPerSecond + 621355968000000000);
        }
    }
}
