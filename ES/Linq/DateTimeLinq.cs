using System;

namespace ES.Linq
{
    /// <summary>
    /// 拓展方法类
    /// <para>DateTime拓展</para>
    /// </summary>
    public static partial class EsLinq
    {
        /// <summary>
        /// 转换为毫秒单位时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToMilliSecondTicks(this DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// 转换为秒单位时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToSecondTicks(this DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        /// <summary>
        /// 对比[年]范围的数据
        /// <para>[等于 0 两个日期相等]</para>
        /// <para>[大于 0 当前日期大于对比日期]</para>
        /// <para>[小于 0 当前日期小于对比日期]</para>
        /// <para>对比值单位秒</para>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="compareDatetime"></param>
        /// <returns>对比值单位秒</returns>
        public static long CompareYear(this DateTime dateTime, DateTime compareDatetime)
        {
            return (DateTime.Parse(dateTime.ToString("yyyy-01-01 00:00:00")).Ticks - DateTime.Parse(compareDatetime.ToString("yyyy-01-01 00:00:00")).Ticks) / 10000000;
        }

        /// <summary>
        /// 对比[年月]范围的数据
        /// <para>[等于 0 两个日期相等]</para>
        /// <para>[大于 0 当前日期大于对比日期]</para>
        /// <para>[小于 0 当前日期小于对比日期]</para>
        /// <para>对比值单位秒</para>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="compareDatetime"></param>
        /// <returns>对比值单位秒</returns>
        public static long CompareMonth(this DateTime dateTime, DateTime compareDatetime)
        {
            return (DateTime.Parse(dateTime.ToString("yyyy-MM-01 00:00:00")).Ticks - DateTime.Parse(compareDatetime.ToString("yyyy-MM-01 00:00:00")).Ticks) / 10000000;
        }

        /// <summary>
        /// 对比[年月日]范围的数据
        /// <para>[等于 0 两个日期相等]</para>
        /// <para>[大于 0 当前日期大于对比日期]</para>
        /// <para>[小于 0 当前日期小于对比日期]</para>
        /// <para>对比值单位秒</para>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="compareDatetime"></param>
        /// <returns>对比值单位秒</returns>
        public static long CompareDay(this DateTime dateTime, DateTime compareDatetime)
        {
            return (DateTime.Parse(dateTime.ToString("yyyy-MM-dd 00:00:00")).Ticks - DateTime.Parse(compareDatetime.ToString("yyyy-MM-dd 00:00:00")).Ticks) / 10000000;
        }

        /// <summary>
        /// 对比[年月日时]范围的数据
        /// <para>[等于 0 两个日期相等]</para>
        /// <para>[大于 0 当前日期大于对比日期]</para>
        /// <para>[小于 0 当前日期小于对比日期]</para>
        /// <para>对比值单位秒</para>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="compareDatetime"></param>
        /// <returns>对比值单位秒</returns>
        public static long CompareHour(this DateTime dateTime, DateTime compareDatetime)
        {
            return (DateTime.Parse(dateTime.ToString("yyyy-MM-dd HH:00:00")).Ticks - DateTime.Parse(compareDatetime.ToString("yyyy-MM-dd HH:00:00")).Ticks) / 10000000;
        }

        /// <summary>
        /// 对比[年月日时分]范围的数据
        /// <para>[等于 0 两个日期相等]</para>
        /// <para>[大于 0 当前日期大于对比日期]</para>
        /// <para>[小于 0 当前日期小于对比日期]</para>
        /// <para>对比值单位秒</para>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="compareDatetime"></param>
        /// <returns>对比值单位秒</returns>
        public static long CompareMinute(this DateTime dateTime, DateTime compareDatetime)
        {
            return (DateTime.Parse(dateTime.ToString("yyyy-MM-dd HH:mm:00")).Ticks - DateTime.Parse(compareDatetime.ToString("yyyy-MM-dd HH:mm:00")).Ticks) / 10000000;
        }

        /// <summary>
        /// 对比[年月日时分秒]范围的数据
        /// <para>[等于 0 两个日期相等]</para>
        /// <para>[大于 0 当前日期大于对比日期]</para>
        /// <para>[小于 0 当前日期小于对比日期]</para>
        /// <para>对比值单位秒</para>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="compareDatetime"></param>
        /// <returns>对比值单位秒</returns>
        public static long CompareSecond(this DateTime dateTime, DateTime compareDatetime)
        {
            return (DateTime.Parse(dateTime.ToString("yyyy-MM-dd HH:mm:ss")).Ticks - DateTime.Parse(compareDatetime.ToString("yyyy-MM-dd HH:mm:ss")).Ticks) / 10000000;
        }
    }
}
