﻿using System;
using System.Collections.Generic;

namespace ES.Time
{
    /// <summary>
    /// 定时时钟
    /// </summary>
    public class TimeClock : ITimeUpdate
    {
        private readonly static List<TimeClock> timeClocks = new List<TimeClock>();

        /// <summary>
        /// 回调执行的函数
        /// </summary>
        private readonly Action<DateTime> handle;

        /// <summary>
        /// 是否重复
        /// </summary>
        public readonly bool IsRepeat;

        private readonly ISysTime sysTime;

        private readonly BaseTimeFlow timeFlow;

        /// <summary>
        /// 单位周期 1s
        /// </summary>
        private int unitPeriod = 0;

        /// <summary>
        /// 今日是否触发
        /// </summary>
        public bool TriggerToday { private set; get; } = false;

        /// <summary>
        /// 上一个时间
        /// </summary>
        private int lastHour;

        /// <summary>
        /// 年
        /// </summary>
        public readonly int year;
        /// <summary>
        /// 月
        /// </summary>
        public readonly int month;
        /// <summary>
        /// 日
        /// </summary>
        public readonly int day;
        /// <summary>
        /// 时
        /// </summary>
        public readonly int hour;
        /// <summary>
        /// 分
        /// </summary>
        public readonly int minute;
        /// <summary>
        /// 秒
        /// </summary>
        public readonly int second;

        /// <summary>
        /// 是否已取消执行
        /// </summary>
        public bool IsCancel => timeFlow.isTimeFlowStop;

        /// <summary>
        /// 创建一个定时时钟
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <param name="hour">时</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        /// <param name="isRepeat">是否重复</param>
        /// <param name="sysTime">获取时间接口</param>
        /// <param name="tfIndex">时间流索引</param>
        private TimeClock(Action<DateTime> handle, int year, int month, int day, int hour, int minute, int second, bool isRepeat, ISysTime? sysTime, int tfIndex = -1)
        {
            IsRepeat = isRepeat;
            this.handle = handle;
            this.sysTime = sysTime ?? new SysTime();

            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;

            // 获取当前时
            lastHour = DateTime.Now.Hour;

            if (tfIndex == -1)
                timeFlow = BaseTimeFlow.CreateTimeFlow(this);
            else
                timeFlow = BaseTimeFlow.CreateTimeFlow(this, tfIndex);
        }

        /// <summary>
        /// 创建一个定时时钟
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="time">时间 格式：HH:mm::ss</param>
        /// <param name="isRepeat">是否重复</param>
        /// <param name="sysTime">系统时间获取接口</param>
        public static TimeClock Create(Action<DateTime> handle, string time, bool isRepeat = false, ISysTime? sysTime = null)
        {
            DateTime dtTime = Convert.ToDateTime(time);
            return new TimeClock(handle, -1, -1, -1, dtTime.Hour, dtTime.Minute, dtTime.Second, isRepeat, sysTime);
        }

        /// <summary>
        /// 创建一个定时时钟
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <param name="hour">时</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        /// <param name="sysTime">系统时间获取接口</param>
        public static TimeClock Create(Action<DateTime> handle, int year, int month, int day, int hour, int minute, int second, ISysTime? sysTime = null)
        {
            return new TimeClock(handle, year, month, day, hour, minute, second, false, sysTime);
        }

        /// <summary>
        /// 创建一个定时时钟
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <param name="hour">时</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        /// <param name="isRepeat">是否重复</param>
        /// <param name="sysTime">系统时间获取接口</param>
        public static TimeClock Create(Action<DateTime> handle, int month, int day, int hour, int minute, int second, bool isRepeat = false, ISysTime? sysTime = null)
        {
            return new TimeClock(handle, -1, month, day, hour, minute, second, isRepeat, sysTime);
        }

        /// <summary>
        /// 创建一个定时时钟
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="day">日</param>
        /// <param name="hour">时</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        /// <param name="isRepeat">是否重复</param>
        /// <param name="sysTime">系统时间获取接口</param>
        public static TimeClock Create(Action<DateTime> handle, int day, int hour, int minute, int second, bool isRepeat = false, ISysTime? sysTime = null)
        {
            return new TimeClock(handle, -1, -1, day, hour, minute, second, isRepeat, sysTime);
        }

        /// <summary>
        /// 创建一个定时时钟
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="hour">时</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        /// <param name="isRepeat">是否重复</param>
        /// <param name="sysTime">系统时间获取接口</param>
        public static TimeClock Create(Action<DateTime> handle, int hour, int minute, int second, bool isRepeat = false, ISysTime? sysTime = null)
        {
            return new TimeClock(handle, -1, -1, -1, hour, minute, second, isRepeat, sysTime);
        }

        /// <summary>
        /// 创建一个同步定时时钟
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <param name="hour">时</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        /// <param name="sysTime">系统时间获取接口</param>
        public static TimeClock CreateSync(Action<DateTime> handle, int year, int month, int day, int hour, int minute, int second, ISysTime? sysTime = null)
        {
            return new TimeClock(handle, year, month, day, hour, minute, second, false, sysTime, 2);
        }

        /// <summary>
        /// 创建一个同步定时时钟
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <param name="hour">时</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        /// <param name="isRepeat">是否重复</param>
        /// <param name="sysTime">系统时间获取接口</param>
        public static TimeClock CreateSync(Action<DateTime> handle, int month, int day, int hour, int minute, int second, bool isRepeat = false, ISysTime? sysTime = null)
        {
            return new TimeClock(handle, -1, month, day, hour, minute, second, isRepeat, sysTime, 2);
        }

        /// <summary>
        /// 创建一个同步定时时钟
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="day">日</param>
        /// <param name="hour">时</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        /// <param name="isRepeat">是否重复</param>
        /// <param name="sysTime">系统时间获取接口</param>
        public static TimeClock CreateSync(Action<DateTime> handle, int day, int hour, int minute, int second, bool isRepeat = false, ISysTime? sysTime = null)
        {
            return new TimeClock(handle, -1, -1, day, hour, minute, second, isRepeat, sysTime, 2);
        }

        /// <summary>
        /// 创建一个同步定时时钟
        /// </summary>
        /// <param name="handle">需要被执行的函数</param>
        /// <param name="hour">时</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        /// <param name="isRepeat">是否重复</param>
        /// <param name="sysTime">系统时间获取接口</param>
        public static TimeClock CreateSync(Action<DateTime> handle, int hour, int minute, int second, bool isRepeat = false, ISysTime? sysTime = null)
        {
            return new TimeClock(handle, -1, -1, -1, hour, minute, second, isRepeat, sysTime, 2);
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="isDaemon">是否守护执行,守护执行后不再需要保存执行器对象</param>
        /// <returns></returns>
        public TimeClock Start(bool isDaemon = false)
        {
            if (isDaemon) lock (timeClocks) timeClocks.Add(this);

            timeFlow.StartTimeFlowES();
            return this;
        }

        /// <summary>
        /// 取消时间执行器任务
        /// </summary>
        public void Cancel()
        {
            timeFlow.CloseTimeFlowES();
        }

        /// <summary>
        /// 取消所有守护的时间执行器任务
        /// </summary>
        public static void CancelAllDaemonTimeClock()
        {
            lock (timeClocks)
            {
                foreach (var t in timeClocks)
                {
                    t.Cancel();
                }
                timeClocks.Clear();
            }
        }

        /// <summary>
        /// 系统调用
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(int deltaTime)
        {
            unitPeriod += deltaTime;
            // 每一秒执行一次
            if (unitPeriod < 1000)
                return;
            unitPeriod = 0;
            DateTime now = sysTime.Now;

            if (now.Hour != lastHour)
            {
                // 如果时间大小发生变化说明新一天到来
                if (now.Hour < lastHour) TriggerToday = false;
                lastHour = now.Hour;
            }

            // 今天已触发
            if (TriggerToday)
                return;

            if (year != -1 && now.Year != year)
                return;
            if (month != -1 && now.Month != month)
                return;
            if (day != -1 && now.Day != day)
                return;
            if (now.Hour != hour)
                return;
            if (now.Minute != minute)
                return;
            if (now.Second < second)
                return;
            // 今天已触发
            TriggerToday = true;
            // 执行函数
            handle?.Invoke(now);
            if (!IsRepeat)
            {
                timeFlow.CloseTimeFlowES();
            }
        }

        /// <summary>
        /// 停止更新
        /// </summary>
        public void UpdateEnd()
        {
            lock (timeClocks) timeClocks.Remove(this);
        }

        private class SysTime : ISysTime
        {
            public DateTime Now => DateTime.Now;
        }
    }
}