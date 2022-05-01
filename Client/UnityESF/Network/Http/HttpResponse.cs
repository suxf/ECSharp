using System;
using System.IO;
using System.Text;

namespace ES.Network.Http
{
    /// <summary>
    /// HTTP应答
    /// </summary>
    public class HttpResponse : HttpHeader
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int StatusCode { get; set; } = 200;
        /// <summary>
        /// 内容
        /// </summary>
        public byte[]? Content { get; private set; }
        /// <summary>
        /// 数据流句柄
        /// </summary>
        private readonly Stream handler;
        /// <summary>
        /// 是否有数据
        /// </summary>
        private bool hasContent = false;

        /// <summary>
        /// 构造一个应答对象
        /// </summary>
        /// <param name="stream"></param>
        internal HttpResponse(Stream stream)
        {
            handler = stream;
        }

        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码，默认UTF8</param>
        /// <returns></returns>
        public HttpResponse Write(byte[] content, Encoding? encoding = null)
        {
            hasContent = true;
            Content = content;
            Encoding = encoding ?? Encoding.UTF8;
            ContentLength = content.Length.ToString();
            return this;
        }

        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码，默认UTF8</param>
        /// <returns></returns>
        public HttpResponse Write(string content, Encoding? encoding = null)
        {
            //初始化内容
            encoding = encoding ?? Encoding.UTF8;
            return Write(encoding.GetBytes(content), encoding);
        }

        /// <summary>
        /// 获取应答数据流
        /// </summary>
        /// <returns></returns>
        public Stream GetResponseStream()
        {
            return handler;
        }

        /// <summary>
        /// 通过枚举获取应答头
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public string? GetHeader(ResponseHeaders header)
        {
            var fieldName = ResponseHeadersHelper.headerMap[header];
            return GetHeader(fieldName);
        }

        /// <summary>
        /// 通过枚举设置应答头
        /// </summary>
        /// <param name="header"></param>
        /// <param name="value"></param>
        public void SetHeader(ResponseHeaders header, string value)
        {
            var fieldName = ResponseHeadersHelper.headerMap[header];
            SetHeader(fieldName, value);
        }

        /// <summary>
        /// 构建响应头部
        /// </summary>
        /// <returns></returns>
        private string BuildHeader()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"HTTP/1.1 {StatusCode}\r\n");

            foreach (var item in headers) builder.AppendLine($"{item.Key}:{item.Value}");

            return builder.ToString();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        internal void Send()
        {
            if (!handler.CanWrite || !hasContent || Encoding == null || Content == null) return;
            //发送响应头
            var header = BuildHeader();
#if !UNITY_2020_1_OR_NEWER && !NET462 && !NETSTANDARD2_0
            ReadOnlySpan<byte> headerBytes = Encoding.GetBytes(header);
            handler.Write(headerBytes);
            //发送空行
            ReadOnlySpan<byte> lineBytes = Encoding.GetBytes(Environment.NewLine);
            handler.Write(lineBytes);
            //发送内容
            handler.Write(Content);
#else
            byte[] headerBytes = Encoding.GetBytes(header);
            handler.Write(headerBytes, 0, headerBytes.Length);
            //发送空行
            byte[] lineBytes = Encoding.GetBytes(Environment.NewLine);
            handler.Write(lineBytes, 0, lineBytes.Length);
            //发送内容
            handler.Write(Content, 0, Content.Length);
#endif
        }
    }
}
