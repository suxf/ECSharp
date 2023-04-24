#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;
using System.Collections.Concurrent;

namespace ECSharp.Network.Http.Linq
{
    /// <summary>
    /// 回调委托
    /// </summary>
    public delegate void Request(HttpRequest request, HttpResponse response);
    /// <summary>
    /// 回调委托
    /// </summary>
    public delegate bool RequestPassOn(HttpRequest request, HttpResponse response);

    /// <summary>
    /// http访问器
    /// </summary>
    public class HttpVisitor : IHttp
    {
        /// <summary>
        /// 回调委托列表
        /// </summary>
        internal ConcurrentDictionary<string, Request> commandList;
        /// <summary>
        /// 全局Http监听者
        /// </summary>
        private RequestPassOn? allHttpListener = null;
        /// <summary>
        /// 异常回调函数地址
        /// </summary>
        private readonly IHttpVisitor listener;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpVisitor(IHttpVisitor listener)
        {
            commandList = new ConcurrentDictionary<string, Request>();
            this.listener = listener;
        }

        /// <summary>
        /// 添加访问函数
        /// 相同访问后缀可以被覆盖 可重复注册相同后缀访问已更新内容
        /// </summary>
        /// <param name="suffix">标记后缀,空字符串表示根访问</param>
        /// <param name="callback">访问函数</param>
        public void Add(string suffix, Request callback)
        {
            if (!commandList.TryAdd(suffix, callback)) commandList[suffix] = callback;
        }

        /// <summary>
        /// 添加访问函数
        /// 相同访问后缀可以被覆盖 可重复注册相同后缀访问已更新内容
        /// </summary>
        /// <param name="callback">访问函数并且根据函数名设置后缀</param>
        public void Add(Request callback)
        {
            if (!commandList.TryAdd(callback.Method.Name, callback)) commandList[callback.Method.Name] = callback;
        }

        /// <summary>
        /// 设置全局监听者
        /// <para>虽然访问器可以监听各个想要监听的地址</para>
        /// <para>但是无法有一个共同回调来处理一些特殊的需求</para>
        /// <para>这个监听是返回所有可以接收到的请求,以此来实现添加Add无法实现的全部监听</para>
        /// <para>并且如果返回值为false那么就不会传递给访问器下面的链接,以统一规则拦截非法请求</para>
        /// </summary>
        /// <param name="callback"></param>
        public void SetAllListener(RequestPassOn callback)
        {
            allHttpListener = callback;
        }

        void IHttp.OnRequest(HttpRequest request, HttpResponse response)
        {
            Request? or = null;
#if !UNITY_2020_1_OR_NEWER && !NET462 && !NETSTANDARD2_0
            var url = request.RawUrl.Split("?")[0];
#else
			var url = request.RawUrl.Split('?')[0];
#endif
            if (url.Length >= 1)
                url = url[0] == '/' ? url.Substring(1) : url;

            if (url.Length >= 1)
                url = url[url.Length - 1] == '/' ? url.Substring(0, url.Length - 1) : url;

            if (commandList.TryGetValue(url, out Request? value))
            {
                or = value;
            }

            if (or != null)
            {
                try
                {
                    bool isPassOn = true;
                    if (allHttpListener != null)
                    {
                        isPassOn = allHttpListener.Invoke(request, response);
                    }
                    if(isPassOn) or.Invoke(request, response);
                }
                catch (Exception ex)
                {
                    listener.HttpVisitorException(request, ex);
                    response.StatusCode = 404;
                }
            }
            else response.StatusCode = 404;
        }

        /// <summary>
        /// 异常捕捉回调
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="request"></param>
        public void HttpException(HttpRequest? request, Exception exception)
        {
            listener.HttpVisitorException(request, exception);
        }
    }
}
