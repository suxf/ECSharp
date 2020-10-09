using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Text;

namespace ES.Data.Buffer
{
    /// <summary>
    /// 简单数据通信流
    /// <para>只需要简单继承此类 即可达到类似protobuff的简单效果</para>
    /// <para>注意此类适合交互即时性要求不高的操作，如果延时性要求很高谨慎使用</para>
    /// </summary>
    public abstract class EasyBuffer<T> : EasyBuffer where T : EasyBuffer<T>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        protected EasyBuffer() : base(typeof(T).GetFields()) { }

        /// <summary>
        /// 获取对象字节流 UTF-8编码
        /// </summary>
        /// <param name="isCompress">是否要对对象进行压缩 默认true</param>
        /// <returns></returns>
        public byte[] ToBytes(bool isCompress = true)
        {
            return Encoding.UTF8.GetBytes(ToString(isCompress));
        }

        /// <summary>
        /// 解析对象 UTF-8 编码
        /// </summary>
        /// <param name="buffer">解析字节流</param>
        /// <param name="isCompress">是否压缩流 默认true</param>
        /// <returns></returns>
        public static T Parse(byte[] buffer, bool isCompress = true)
        {
            return Parse(Encoding.UTF8.GetString(buffer), isCompress);
        }

        /// <summary>
        /// 获取对象字符串
        /// </summary>
        /// <param name="isCompress">是否要对对象进行压缩 默认true</param>
        /// <returns></returns>
        public string ToString(bool isCompress = true)
        {
            if (isCompress)
            {
                // JObject jRaw = JObject.FromObject(this);
                JObject jRaw = GetJObject();
                // 压缩json格式
                JArray jCompress = CompressESBuffer(jRaw);
                var optimizedStr = new StringBuilder(JsonConvert.SerializeObject(jCompress));
                // 压缩json字符串
                return CompressJson(optimizedStr);
            }
            else return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// 解析对象
        /// </summary>
        /// <param name="buffer">解析字符串</param>
        /// <param name="isCompress">是否压缩流 默认true</param>
        /// <returns></returns>
        public static T Parse(string buffer, bool isCompress = true)
        {
            if (isCompress)
            {
                var optimizedStr = new StringBuilder(buffer);
                var jCompress = JsonConvert.DeserializeObject<JArray>(DeCompressJson(optimizedStr));
                JObject jDeCompress = (Activator.CreateInstance(typeof(T)) as EasyBuffer).DeCompressESBuffer(jCompress);
                return jDeCompress.ToObject<T>();
            }
            else return JsonConvert.DeserializeObject<T>(buffer);
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="optimizedStr"></param>
        private static string CompressJson(StringBuilder optimizedStr)
        {
            optimizedStr.Replace("\"},{\"", "\u0001");
            optimizedStr.Replace("\"],[\"", "\u0002");
            optimizedStr.Replace("\":{\"", "\u0003");
            optimizedStr.Replace("\"],\"", "\u0004");
            optimizedStr.Replace("},{\"", "\u0005");
            optimizedStr.Replace("\":\"", "\u0006");
            optimizedStr.Replace("\",\"", "\u0007");
            optimizedStr.Replace("\":[", "\u0008");
            optimizedStr.Replace("\"],", "\u0009");
            optimizedStr.Replace("],\"", "\u0010");
            optimizedStr.Replace("},\"", "\u0011");
            optimizedStr.Replace("[{\"", "\u0012");
            optimizedStr.Replace("\"}]", "\u0013");
            optimizedStr.Replace("],[", "\u0014");
            optimizedStr.Replace("{\"", "\u0015");
            optimizedStr.Replace("\":", "\u0016");
            optimizedStr.Replace(",\"", "\u0017");
            optimizedStr.Replace("[\"", "\u0018");
            optimizedStr.Replace("\"]", "\u0019");
            optimizedStr.Replace("{\"", "\u001A");
            optimizedStr.Replace("\"}", "\u001B");
            optimizedStr.Replace("\",", "\u0080");
            optimizedStr.Replace("]]", "\u0081");
            optimizedStr.Replace("{{", "\u0082");
            optimizedStr.Replace("}}", "\u0083");
            optimizedStr.Replace("[{", "\u0084");
            optimizedStr.Replace("}]", "\u0085");
            optimizedStr.Replace("]}", "\u0086");
            optimizedStr.Replace("[[", "\u0087");
            optimizedStr.Replace(":[", "\u0088");
            optimizedStr.Replace("],", "\u0089");
            optimizedStr.Replace("},", "\u008A");
            optimizedStr.Replace("null", "\u008B");
            return optimizedStr.ToString();
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="optimizedStr"></param>
        private static string DeCompressJson(StringBuilder optimizedStr)
        {
            optimizedStr.Replace("\u008B", "null");
            optimizedStr.Replace("\u008A", "},");
            optimizedStr.Replace("\u0089", "],");
            optimizedStr.Replace("\u0088", ":[");
            optimizedStr.Replace("\u0087", "[[");
            optimizedStr.Replace("\u0086", "]}");
            optimizedStr.Replace("\u0085", "}]");
            optimizedStr.Replace("\u0084", "[{");
            optimizedStr.Replace("\u0083", "}}");
            optimizedStr.Replace("\u0082", "{{");
            optimizedStr.Replace("\u0081", "]]");
            optimizedStr.Replace("\u0080", "\",");
            optimizedStr.Replace("\u001B", "\"}");
            optimizedStr.Replace("\u001A", "{\"");
            optimizedStr.Replace("\u0019", "\"]");
            optimizedStr.Replace("\u0018", "[\"");
            optimizedStr.Replace("\u0017", ",\"");
            optimizedStr.Replace("\u0016", "\":");
            optimizedStr.Replace("\u0015", "{\"");
            optimizedStr.Replace("\u0014", "],[");
            optimizedStr.Replace("\u0013", "\"}]");
            optimizedStr.Replace("\u0012", "[{\"");
            optimizedStr.Replace("\u0011", "},\"");
            optimizedStr.Replace("\u0010", "],\"");
            optimizedStr.Replace("\u0009", "\"],");
            optimizedStr.Replace("\u0008", "\":[");
            optimizedStr.Replace("\u0007", "\",\"");
            optimizedStr.Replace("\u0006", "\":\"");
            optimizedStr.Replace("\u0005", "},{\"");
            optimizedStr.Replace("\u0004", "\"],\"");
            optimizedStr.Replace("\u0003", "\":{\"");
            optimizedStr.Replace("\u0002", "\"],[\"");
            optimizedStr.Replace("\u0001", "\"},{\"");
            return optimizedStr.ToString();
        }
    }

    /// <summary>
    /// 简单数据通信流 基础类
    /// <para>实际使用中请使用 EasyBuffer`1 (T) 带模板类</para>
    /// </summary>
    public abstract class EasyBuffer
    {
        /// <summary>
        /// 反射字段
        /// </summary>
        private readonly FieldInfo[] _esfields;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_esfields"></param>
        internal EasyBuffer(FieldInfo[] _esfields)
        {
            this._esfields = _esfields;
        }

        internal JObject GetJObject()
        {
            JObject jRaw = new JObject();
            for (int i = 0, len = _esfields.Length; i < len; i++)
            {
                FieldInfo field = _esfields[i];
                if (field.FieldType.BaseType.FullName.Contains("EasyBuffer"))
                {
                    // JArray jItemArray = (field.GetValue(this) as EasyBuffer).CompressESBuffer(jItem.Value as JObject);
                    // jCompress.Add(jItemArray);
                }
                else
                {
                    if (field.FieldType == typeof(int)) jRaw.Add(field.Name, (int)field.GetValue(this));
                    else if (field.FieldType == typeof(bool)) jRaw.Add(field.Name, (bool)field.GetValue(this));
                    else if (field.FieldType == typeof(string)) jRaw.Add(field.Name, (string)field.GetValue(this));
                    else if (field.FieldType == typeof(string)) jRaw.Add(field.Name, (string)field.GetValue(this));
                    else if (field.FieldType == typeof(string)) jRaw.Add(field.Name, (string)field.GetValue(this));
                    else if (field.FieldType == typeof(string)) jRaw.Add(field.Name, (string)field.GetValue(this));
                    else if (field.FieldType == typeof(string)) jRaw.Add(field.Name, (string)field.GetValue(this));
                }
            }

            return jRaw;
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="jRaw"></param>
        internal JArray CompressESBuffer(JObject jRaw)
        {
            JArray jCompress = new JArray();
            foreach (var jItem in jRaw)
            {
                bool isField = false;
                for (int i = 0, len = _esfields.Length; i < len; i++)
                {
                    var field = _esfields[i];
                    if (jItem.Key == field.Name)
                    {
                        if (field.FieldType.BaseType.FullName.Contains("EasyBuffer"))
                        {
                            isField = true;

                            JArray jItemArray = (field.GetValue(this) as EasyBuffer).CompressESBuffer(jItem.Value as JObject);
                            jCompress.Add(jItemArray);
                        }
                        break;
                    }
                }
                if (!isField) jCompress.Add(jItem.Value);
            }
            return jCompress;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jCompress"></param>
        internal JObject DeCompressESBuffer(JArray jCompress)
        {
            JObject jDeCompress = new JObject();
            for (int i = 0, len = jCompress.Count; i < len; i++)
            {
                bool isField = false;

                var jItem = jCompress[i];
                var field = _esfields[i];
                if (field.FieldType.BaseType.FullName.Contains("EasyBuffer"))
                {
                    isField = true;

                    JObject jItemObj = (Activator.CreateInstance(field.FieldType) as EasyBuffer).DeCompressESBuffer(jItem as JArray);
                    jDeCompress.Add(field.Name, jItemObj);
                }
                if (!isField) jDeCompress.Add(field.Name, jItem);
            }
            return jDeCompress;
        }
    }

}
