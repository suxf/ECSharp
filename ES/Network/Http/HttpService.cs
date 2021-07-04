using ES.Network.Http.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

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
        private TcpListener listener = null;
        /// <summary>
        /// HTTP访问回调委托
        /// </summary>
        private IHttp httpInvoke = null;

        /// <summary>
        /// 安全证书
        /// </summary>
        private readonly X509Certificate2 certificate = null;

        /// <summary>
        /// 构造函数
        /// <para>创建一个HTTP服务</para>
        /// </summary>
        /// <param name="ipAddress">监听ip地址</param>
        /// <param name="port">监听端口</param>
        /// <param name="visitor">访问器</param>
        /// <param name="certificate">安全证书</param>
        public HttpService(string ipAddress, int port, HttpVisitor visitor, X509Certificate2 certificate = null)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            // 赋值证书
            this.certificate = certificate;
            // 绑定访问回调委托
            httpInvoke = visitor;

            // Create a listener.
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);

            // Log.Info("HttpService Listening...");
        }

        /// <summary>
        /// 构造函数
        /// <para>创建一个HTTP服务</para>
        /// </summary>
        /// <param name="ipAddress">监听ip地址</param>
        /// <param name="port">监听端口</param>
        /// <param name="invoke">回调接口[不适用访问器添加]</param>
        /// <param name="certificate">安全证书</param>
        public HttpService(string ipAddress, int port, IHttp invoke, X509Certificate2 certificate = null)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            // 赋值证书
            this.certificate = certificate;
            // 绑定访问回调委托
            httpInvoke = invoke;

            // Create a listener.
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            // Log.Info("HttpService Listening...");
        }

        /// <summary>
        /// 开启服务器
        /// </summary>
        public void StartServer()
        {
            try
            {
                listener.Start();
                listener.BeginAcceptTcpClient(new AsyncCallback(GetContextCallBack), listener);
            }
            catch (Exception ex)
            {
                httpInvoke.HttpException(null, ex);
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
        public TcpListener GetTcpListener()
        {
            return listener;
        }

        /// <summary>
        /// 异步回调
        /// </summary>
        private void GetContextCallBack(IAsyncResult ar)
        {
            try
            {
                var tcpListener = ar.AsyncState as TcpListener;
                // 结束异步
                TcpClient tcpClient = tcpListener.EndAcceptTcpClient(ar);
                // 以取得所有需要的参数 继续监听下一个请求
                if (listener != null)
                    listener.BeginAcceptTcpClient(new AsyncCallback(GetContextCallBack), listener);
                // 获取流
                Stream stream = tcpClient.GetStream();
                try
                {
                    // 处理加密
                    if (certificate != null)
                    {
                        SslStream sslStream = new SslStream(stream);
                        sslStream.AuthenticateAsServer(certificate, false, SslProtocols.Tls, true);
                        sslStream.ReadTimeout = 10000;
                        sslStream.WriteTimeout = 10000;
                        stream = sslStream;
                    }
                    // 处理回调
                    if (httpInvoke != null)
                    {
                        HttpRequest request = new HttpRequest(stream, tcpClient.ReceiveBufferSize);
                        try
                        {
                            HttpResponse response = new HttpResponse(stream);
                            // 处理
                            httpInvoke.OnRequest(request, response);
                            // 发送
                            response.Send();
                        }
                        catch (Exception ex)
                        {
                            httpInvoke.HttpException(request, ex);
                        }
                    }
                }
                catch(Exception ex)
                {
                    httpInvoke.HttpException(null, ex);
                }
                stream.Close();
            }
            catch (Exception ex)
            {
                httpInvoke.HttpException(null, ex);
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
                    listener.Stop();// 立刻停止访问请求
                    listener = null;
                }
                httpInvoke = null;
            }
            catch (Exception ex)
            {
                httpInvoke.HttpException(null, ex);
            }
        }
    }
}
