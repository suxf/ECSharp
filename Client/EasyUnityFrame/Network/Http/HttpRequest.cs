using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ES.Network.Http
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
        /// 定义缓冲区
        /// </summary>
        private readonly byte[] bytes;
        /// <summary>
        /// 数据流句柄
        /// </summary>
        private readonly Stream handler;

        /// <summary>
        /// 构建http请求对象
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="size"></param>
        internal HttpRequest(Stream stream, int size)
        {
            handler = stream;
            bytes = new byte[size];
            var data = GetRequestData(handler);
            var rows = Regex.Split(data, Environment.NewLine);

            //Request URL & Method & Version
            var first = Regex.Split(rows[0], @"(\s+)")
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
            if (first.Length > 1) RawUrl = Uri.UnescapeDataString(first[1]);
            if (first.Length > 2) ProtocolVersion = first[2];

            // 判定是不是安全连接
            IsSSL = ProtocolVersion.ToLower().IndexOf("https") != -1;
            //Request Headers
            headers = GetRequestHeaders(rows);
            //Request Body
            Body = GetRequestBody(rows);
            var contentLength = GetHeader(RequestHeaders.ContentLength);
            if (int.TryParse(contentLength, out var length) && Body.Length != length)
            {
                do
                {
                    length = stream.Read(bytes, 0, bytes.Length);
                    Body += Encoding.UTF8.GetString(bytes, 0, length);
                } while (Body.Length != length);
            }

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
        public Stream GetRequestStream()
        {
            return handler;
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
        /// <param name="stream"></param>
        /// <returns></returns>
        private string GetRequestData(Stream stream)
        {
            int length;
            var data = "";
            do
            {
                length = stream.Read(bytes, 0, bytes.Length);
                data += Encoding.UTF8.GetString(bytes, 0, length);
            } while (length > 0 && !data.Contains("\r\n\r\n"));

            return data;
        }

        /// <summary>
        /// 获取请求体
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private string GetRequestBody(IEnumerable<string> rows)
        {
            var target = rows.Select((v, i) => new { Value = v, Index = i }).FirstOrDefault(e => e.Value.Trim() == "");
            if (target == null) return "";
            var range = Enumerable.Range(target.Index + 1, rows.Count() - target.Index - 1);
            return string.Join(Environment.NewLine, range.Select(e => rows.ElementAt(e)).ToArray());
        }

        /// <summary>
        /// 获取请求头
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetRequestHeaders(IEnumerable<string> rows)
        {
            if (!rows.Any()) return new Dictionary<string, string>();
            var target = rows.Select((v, i) => new { Value = v, Index = i }).FirstOrDefault(e => e.Value.Trim() == "");
            var length = target == null ? rows.Count() - 1 : target.Index;
            if (length <= 1) return new Dictionary<string, string>();
            var range = Enumerable.Range(1, length - 1);
            return range.Select(e => rows.ElementAt(e)).ToDictionary(e => e.Split(':')[0], e => e.Split(':')[1].Trim());
        }

        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetRequestParameters(string row)
        {
            if (string.IsNullOrEmpty(row)) return new Dictionary<string, string>();
            var kvs = Regex.Split(row, "&");
            if (kvs == null || kvs.Count() <= 0) return new Dictionary<string, string>();
            return kvs.ToDictionary(e => Regex.Split(e, "=")[0], e => { var p = Regex.Split(e, "="); return p.Length > 1 ? p[1] : ""; });
        }
    }
}
