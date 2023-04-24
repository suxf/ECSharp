#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace ECSharp.Network.Http
{
    /// <summary>
    /// HTTP请求定义
    /// </summary>
    public class HttpRequest : HttpHeader
    {
        /// <summary>
        /// Get参数字典
        /// </summary>
        public Dictionary<string, string> GetParams { get; private set; }
        /// <summary>
        /// Get值
        /// </summary>
        public string? GetValue { get; private set; }
        /// <summary>
        /// Post参数字典
        /// </summary>
        public Dictionary<string, string> PostParams { get; private set; }
        /// <summary>
        /// Post值
        /// </summary>
        public string? PostValue { get; private set; }
        /// <summary>
        /// HTTP请求方式
        /// </summary>
        public HttpMethodType Method { get; private set; }
        /// <summary>
        /// 安全连接
        /// </summary>
        public bool IsSSL { get; private set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string RawUrl { get; private set; } = "";
        /// <summary>
        /// HTTP协议版本
        /// </summary>
        public string ProtocolVersion { get; private set; } = "";
        /// <summary>
        /// 连接客户端
        /// </summary>
        private readonly TcpClient tcpClient;

        /// <summary>
        /// 用户标记
        /// </summary>
        public object? Tag;

        /// <summary>
        /// 构建http请求对象
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tcpClient"></param>
        internal HttpRequest(Stream stream, TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            string row = GetRequestData(stream, tcpClient.ReceiveBufferSize);

            //Request URL & Method & Version
            var first = Regex.Split(row, @"(\s+)")
                .Where(e => e.Trim() != "")
                .ToArray();

            if (first.Length > 0)
            {
                switch (first[0])
                {
                    case "GET": Method = HttpMethodType.GET; break;
                    case "POST": Method = HttpMethodType.POST; break;
                    case "PUT": Method = HttpMethodType.PUT; break;
                    case "DELETE": Method = HttpMethodType.DELETE; break;
                    case "HEAD": Method = HttpMethodType.HEAD; break;
                    case "CONNECT": Method = HttpMethodType.CONNECT; break;
                    case "OPTIONS": Method = HttpMethodType.OPTIONS; break;
                    case "TRACE": Method = HttpMethodType.TRACE; break;
                    default: Method = HttpMethodType.UNKNOWN; break;
                }
            }

            if (first.Length > 1)
                RawUrl = Uri.UnescapeDataString(first[1]);

            if (first.Length > 2)
                ProtocolVersion = first[2];

            // 判定是不是安全连接
            IsSSL = ProtocolVersion.ToLower().IndexOf("https") != -1;

            // 获取get数据
            if (RawUrl.Contains('?'))
            {
                GetValue = RawUrl.Split('?')[1];
                GetParams = GetRequestParameters(GetValue);
            }
            else GetParams = new Dictionary<string, string>();

            // 获取消息体数据
            if (!string.IsNullOrEmpty(Body))
            {
                PostValue = Body;
                PostParams = GetRequestParameters(PostValue);
            }
            else PostParams = new Dictionary<string, string>();
        }

        /// <summary>
        /// 获取请求对象流
        /// </summary>
        /// <returns></returns>
        public TcpClient GetTcpClient()
        {
            return tcpClient;
        }

        /// <summary>
        /// 通过枚举类型获取HTTP头
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public string? GetHeader(RequestHeaders header)
        {
            var fieldName = RequestHeadersHelper.headerMap[header];
            return GetHeader(fieldName ?? "");
        }

        /// <summary>
        /// 通过枚举设置HTTP头
        /// </summary>
        /// <param name="header"></param>
        /// <param name="value"></param>
        public void SetHeader(RequestHeaders header, string value)
        {
            var fieldName = RequestHeadersHelper.headerMap[header];
            SetHeader(fieldName, value);
        }

        /// <summary>
        /// 获取请求数据
        /// </summary>
        /// <returns></returns>
        private string GetRequestData(Stream stream, int size)
        {
            List<string> rows = new List<string>();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[size];
                int bytesRead = 0;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                    // 如果长度获取没有超过缓存大小则直接返回
                    if (bytesRead < size) break;
                }

                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms, Encoding.UTF8, true, -1, true))
                {
                    string? line;
                    while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                    {
                        rows.Add(line);
                    }
                }

                //Request Headers
                headers = GetRequestHeaders(rows);
                if (int.TryParse(GetHeader(RequestHeaders.ContentLength), out int contentLength))
                {
                    ms.Position = ms.Length - contentLength;
                    BodyBytes = new byte[contentLength];
                    ms.Read(BodyBytes, 0, contentLength);

                    if (GetHeader(RequestHeaders.ContentType) != ContentType.Binary)
                    {
                        Body = Encoding.UTF8.GetString(BodyBytes);
                    }
                }

            }
            return rows[0] ?? "";
        }

        /// <summary>
        /// 获取请求头
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetRequestHeaders(IEnumerable<string> rows)
        {
            if (!rows.Any())
                return new Dictionary<string, string>();

            var target = rows.Select((v, i) => new { Value = v, Index = i }).FirstOrDefault(e => e.Value.Trim() == "");
            var length = target == null ? rows.Count() : target.Index;

            if (length <= 1)
                return new Dictionary<string, string>();

            var range = Enumerable.Range(1, length - 1);
            return range.Select(e => rows.ElementAt(e)).ToDictionary(e => e.Substring(0, e.IndexOf(':')).Trim(), e => e.Substring(e.IndexOf(':') + 1).Trim());
        }

        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetRequestParameters(string row)
        {
            if (string.IsNullOrEmpty(row))
                return new Dictionary<string, string>();

            var kvs = Regex.Split(row, "&");

            if (kvs == null || kvs.Length <= 0)
                return new Dictionary<string, string>();

            return kvs.ToDictionary(e => Regex.Split(e, "=")[0], e => { var p = Regex.Split(e, "="); return p.Length > 1 ? p[1] : ""; });
        }
    }
}
