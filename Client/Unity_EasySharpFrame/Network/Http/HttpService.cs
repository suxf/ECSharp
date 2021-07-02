using ES.Network.Http.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ES.Network.Http
{
    /// <summary>
    /// HTTP访问服务
    /// </summary>
    public class HttpService
    {
        /// <summary>
        /// Http监听器
        /// </summary>
        private HttpListener listener = null;
        /// <summary>
        /// HTTP访问回调委托
        /// </summary>
        private IHttp httpInvoke = null;

        /// <summary>
        /// 默认前缀
        /// </summary>
        private string defaultPrefix = "";

        // & 风格符
        private readonly static char[] separator1 = new char[] { '&' };

        // = 风格符
        private readonly static char[] separator2 = new char[] { '=' };

        /// <summary>
        /// 构造函数
        /// <para>创建一个HTTP服务</para>
        /// </summary>
        /// <param name="prefix">网址地址（端口号(包括端口)之前的部分:protocol+hostname+port）</param>
        /// <param name="visitor">访问器(注意：访问器需要全部初始化加载完成后才能创建http服务)</param>
        public HttpService(string prefix, HttpVisitor visitor)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            // 绑定访问回调委托
            httpInvoke = visitor;

            // Create a listener.
            listener = new HttpListener();

            // 初始化访问器列表
            foreach (var item in visitor.commandList)
                AddPrefix(item.Key, prefix);

            // Log.Info("HttpService Listening...");
        }

        /// <summary>
        /// 构造函数
        /// <para>创建一个HTTP服务</para>
        /// </summary>
        /// <param name="invoke">回调接口[不适用访问器添加]</param>
        public HttpService(IHttp invoke)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            // 绑定访问回调委托
            httpInvoke = invoke;

            // Create a listener.
            listener = new HttpListener();
            // Log.Info("HttpService Listening...");
        }

        /// <summary>
        /// 开启服务器
        /// </summary>
        public void StartServer()
        {
            try
            {
                listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                listener.TimeoutManager.IdleConnection = new TimeSpan(0, 0, 5);// 允许
                listener.Start();

                listener.BeginGetContext(new AsyncCallback(GetContextCallBack), listener);
            }
            catch (Exception ex)
            {
                httpInvoke.HttpException(ex, null);
            }
        }

        /// <summary>
        /// 设置HTTP委托
        /// </summary>
        /// <param name="invoke">委托接口</param>
        public void SetHttpInvoke(IHttp invoke)
        {
            httpInvoke = invoke;
        }

        /// <summary>
        /// 获取HTTP监听对象
        /// </summary>
        /// <returns>监听对象</returns>
        public HttpListener GetHttpListener()
        {
            return listener;
        }

        /// <summary>
        /// 添加监听链接
        /// <para>第二个参数不填默认使用之前填过的第二个参数的值。</para>
        /// </summary>
        /// <param name="suffix">网址后缀</param>
        /// <param name="prefix">网址前缀，固定不变的部分，第一次使用必填</param>
        public void AddPrefix(string suffix, string prefix = null)
        {
            if (prefix != null) defaultPrefix = prefix;
            if (defaultPrefix[prefix.Length - 1] != '/') defaultPrefix += '/';
            AddFullPrefix(defaultPrefix + suffix);
        }

        /// <summary>
        /// 添加完整的监听链接
        /// for example "http://example.com:8080/index/".
        /// </summary>
        /// <param name="prefix">完整链接</param>
        public void AddFullPrefix(string prefix)
        {
            if (listener != null && prefix != null)
            {
                if (prefix[prefix.Length - 1] != '/') prefix += '/';
                listener.Prefixes.Add(prefix);
            }
        }

        /// <summary>
        /// 异步回调
        /// </summary>
        private void GetContextCallBack(IAsyncResult ar)
        {
            HttpListener httpListener = ar.AsyncState as HttpListener;

            try
            {
                HttpListenerContext httpListenerContext = httpListener.EndGetContext(ar);
                HttpListenerRequest request = httpListenerContext.Request;

                string requestUrl = request.RawUrl;
                // 去掉首尾 / 
                if (requestUrl[0] == '/') requestUrl = requestUrl.Substring(1);
                if (requestUrl.Length >= 1 && requestUrl[requestUrl.Length - 1] == '/') requestUrl = requestUrl.Substring(0, requestUrl.Length - 1);
                int index = requestUrl.IndexOf("?");
                HttpConnection conn = new HttpConnection();
                // 读取get值
                var kvPair = conn.getValue = new Dictionary<string, string>();
                if (index >= 0)
                {
                    string getdata = null;
                    getdata = requestUrl.Substring(index + 1, requestUrl.Length - index - 1);
                    requestUrl = requestUrl.Substring(0, index);
                    string[] parameters = getdata.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0, len = parameters.Length; i < len; i++)
                    {
                        string[] kv = parameters[i].Split(separator2, StringSplitOptions.RemoveEmptyEntries);
                        int kvLen = kv.Length;
                        if (kvLen == 2) kvPair.Add(kv[0], kv[1]);
                        else if (kvLen == 1) kvPair.Add(kv[0], "");
                    }
                }
                // 读取post值
                if (request.InputStream != null)
                {
                    conn.postBuffer = new byte[request.ContentLength64];
                    request.InputStream.Read(conn.postBuffer, 0, conn.postBuffer.Length);
                    conn.postValue = Encoding.UTF8.GetString(conn.postBuffer);
                }
                // 处理回调
                if (httpInvoke != null)
                {
                    using (StreamWriter writer = new StreamWriter(httpListenerContext.Response.OutputStream))
                    {
                        if (request.HttpMethod.ToUpper() == "GET") conn.methodType = HttpMethodType.GET;
                        else if (request.HttpMethod.ToUpper() == "POST") conn.methodType = HttpMethodType.POST;
                        else conn.methodType = HttpMethodType.UNKNOWN;

                        conn.request = request;
                        conn.suffix = requestUrl;
                        conn.response = httpListenerContext.Response;
                        conn.writer = writer;
                        try
                        {
                            httpInvoke.OnRequest(conn);
                        }
                        catch (Exception ex)
                        {
                            // Log.Exception(ex, $"[Request Url]:{request.RawUrl}", "HttpService", "GetContextCallBack", "Http");
                            httpInvoke.HttpException(ex, conn);
                        }
                        // int status = (int)httpInvoke.OnRequest(conn);
                        // httpListenerContext.Response.StatusCode = status;
                        // if (status == 400 || status == 404) httpListenerContext.Response.Abort();
                    }
                }

                // 异步
                if (httpListener != null && httpListener.IsListening)
                    httpListener.BeginGetContext(new AsyncCallback(GetContextCallBack), httpListener);
            }
            catch (Exception ex)
            {
                // Log.Exception(ex, "HttpService", "GetContextCallBack", "Http");
                httpInvoke.HttpException(ex, null);
            }
        }

        /// <summary>
        /// 关闭HTTP服务器
        /// </summary>
        public void CloseHttpServer()
        {
            try
            {
                if (listener != null)
                {
                    listener.Abort();// 立刻停止访问请求
                    listener = null;
                }
                httpInvoke = null;
            }
            catch (Exception ex)
            {
                // Log.Exception(ex, "HttpService", "GetContextCallBack", "Http");
                httpInvoke.HttpException(ex, null);
            }
        }
    }
}
