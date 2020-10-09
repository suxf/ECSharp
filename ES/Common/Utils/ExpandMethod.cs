using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace ES.Common.Utils
{
    /// <summary>
    /// 拓展方法类
    /// <para>此类用于拓展一些对象上的方法</para>
    /// <para>便于更快捷的开发</para>
    /// </summary>
    public static class ExtendClass
    {
        #region JSON工具助手 基于Newtonsoft.Json开源框架
        /// <summary>
        /// 通过json字节流新建Json对象
        /// <para>此字节流编码：UTF-8</para>
        /// </summary>
        public static JObject AsJObject(this byte[] json) { return JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(json)); }

        /// <summary>
        /// 通过json字符串新建Json对象
        /// </summary>
        public static JObject AsJObject(this string json) { return JsonConvert.DeserializeObject<JObject>(json); }

        /// <summary>
        /// 序列化json对象为字符串
        /// </summary>
        /// <returns>序列化的字符串</returns>
        public static string AsString(this JObject obj) { return JsonConvert.SerializeObject(obj); }

        /// <summary>
        /// 序列化json对象为字节流
        /// <para>此字节流编码：UTF-8</para>
        /// </summary>
        /// <returns>序列化的字节流</returns>
        public static byte[] AsBytes(this JObject obj) { return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)); }

        /// <summary>
        /// 通过json字节流新建Json数组对象
        /// <para>此字节流编码：UTF-8</para>
        /// </summary>
        public static JArray AsJArray(this byte[] json) { return JsonConvert.DeserializeObject<JArray>(Encoding.UTF8.GetString(json)); }

        /// <summary>
        /// 通过json字符串新建Json数组对象
        /// </summary>
        public static JArray AsJArray(this string json) { return JsonConvert.DeserializeObject<JArray>(json); }

        /// <summary>
        /// 序列化json数组对象为字符串
        /// </summary>
        /// <returns>序列化的字符串</returns>
        public static string AsString(this JArray obj) { return JsonConvert.SerializeObject(obj); }

        /// <summary>
        /// 序列化json数组对象为字节流
        /// <para>此字节流编码：UTF-8</para>
        /// </summary>
        /// <returns>序列化的字节流</returns>
        public static byte[] AsBytes(this JArray obj) { return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)); }
        #endregion

        #region base64编码\解码构造器
        /// <summary>
        /// 将正常字符串转化为base64编码字符串
        /// </summary>
        /// <param name="str">需要转化的正常字符串</param>
        /// <returns></returns>
        public static string ToBase64(this string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 将base64编码字符串转化为正常字符串
        /// </summary>
        /// <param name="str">需要转化的base64字符串</param>
        /// <returns></returns>
        public static string FromBase64(this string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
        #endregion

        #region DateTime拓展
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

        #endregion

        #region Concurrent系列拓展
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
        #endregion

        #region Byte数组比较
        /// <summary>
        /// 比较字节数组
        /// </summary>
        /// <param name="b1">字节数组1</param>
        /// <param name="b2">字节数组2</param>
        /// <returns>相同返回true 不同返回false</returns>
        public static bool Compare(this byte[] b1, byte[] b2)
        {
            if (b2 == null) return false;
            int result = 0;
            if (b1.Length != b2.Length)
                result = b1.Length - b2.Length;
            else
            {
                for (int i = 0; i < b1.Length; i++)
                {
                    if (b1[i] != b2[i])
                    {
                        result = b1[i] - b2[i];
                        break;
                    }
                }
            }
            return result == 0;
        }
        #endregion
    }
}
