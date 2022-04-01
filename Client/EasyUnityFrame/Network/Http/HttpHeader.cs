using System.Collections.Generic;
using System.Text;

namespace ES.Network.Http
{
    /// <summary>
    /// HTTP消息头
    /// </summary>
    public class HttpHeader
    {
        /// <summary>
        /// 消息体
        /// </summary>
        public string? Body { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding? Encoding { get; set; }
        /// <summary>
        /// 内容长度
        /// </summary>
        public string? ContentLength { get; internal set; }
        /// <summary>
        /// 头信息字典
        /// </summary>
        protected Dictionary<string, string> headers = new Dictionary<string, string>();

        /// <summary>
        /// 通过字符串获取HTTP头
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string? GetHeader(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName)) return null;
            var hasKey = headers.ContainsKey(fieldName);
            if (!hasKey) return null;
            return headers[fieldName];
        }

        /// <summary>
        /// 通过字符串设置HTTP头
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public void SetHeader(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(fieldName)) return;
            var hasKey = headers.ContainsKey(fieldName);
            if (!hasKey) headers.Add(fieldName, value);
            headers[fieldName] = value;
        }
    }
}
