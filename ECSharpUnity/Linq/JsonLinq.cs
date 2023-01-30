#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ECSharp.Linq
{
    /// <summary>
    /// 拓展方法类
    /// <para>JSON工具助手 基于Newtonsoft.Json开源框架</para>
    /// </summary>
    public static class JsonLinq
    {
        /// <summary>
        /// 通过json字节流新建Json对象
        /// <para>此字节流编码：UTF-8</para>
        /// </summary>
        public static JObject? AsJObject(this byte[] json) { return JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(json)); }

        /// <summary>
        /// 通过json字符串新建Json对象
        /// </summary>
        public static JObject? AsJObject(this string json) { return JsonConvert.DeserializeObject<JObject>(json); }

        /// <summary>
        /// 序列化json对象为字符串
        /// </summary>
        /// <returns>序列化的字符串</returns>
        public static string? AsString(this JObject obj) { return JsonConvert.SerializeObject(obj); }

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
        public static JArray? AsJArray(this byte[] json) { return JsonConvert.DeserializeObject<JArray>(Encoding.UTF8.GetString(json)); }

        /// <summary>
        /// 通过json字符串新建Json数组对象
        /// </summary>
        public static JArray? AsJArray(this string json) { return JsonConvert.DeserializeObject<JArray>(json); }

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
    }
}
