using ES.Network.Http.Linq;
using System;
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
        private readonly TcpListener listener;
        /// <summary>
        /// HTTP访问回调委托
        /// </summary>
        private IHttp httpInvoke;

        /// <summary>
        /// 安全证书
        /// </summary>
        private readonly X509Certificate2? certificate;

        /// <summary>
        /// 构造函数
        /// <para>创建一个HTTP服务</para>
        /// </summary>
        /// <param name="ipAddress">监听ip地址</param>
        /// <param name="port">监听端口</param>
        /// <param name="visitor">访问器</param>
        /// <param name="certificate">安全证书</param>
        public HttpService(string ipAddress, int port, HttpVisitor visitor, X509Certificate2? certificate = null)
        {
            if (!HttpListener.IsSupported)
            {
                throw new Exception("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
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
        public HttpService(string ipAddress, int port, IHttp invoke, X509Certificate2? certificate = null)
        {
            if (!HttpListener.IsSupported)
            {
                throw new Exception("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
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
        /// <param name="backlog">并发数量 默认:10</param>
        public void StartServer(int backlog = 10)
        {
            try
            {
                listener.Start(backlog);
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
                TcpListener? tcpListener = ar.AsyncState as TcpListener;

                if (tcpListener == null)
                    return;

                // 结束异步
                TcpClient tcpClient = tcpListener.EndAcceptTcpClient(ar);
                // 以取得所有需要的参数 继续监听下一个请求
                listener.BeginAcceptTcpClient(new AsyncCallback(GetContextCallBack), listener);
                try
                {
                    // 获取流
                    NetworkStream networkStream = tcpClient.GetStream();

                    if (networkStream == null)
                        return;

                    SslStream? sslStream = null;
                    try
                    {
                        // 处理加密
                        if (certificate != null)
                        {
                            sslStream = new SslStream(networkStream);
                            sslStream.AuthenticateAsServer(certificate, false, SslProtocols.Tls, true);
                            // sslStream.ReadTimeout = 10000;
                            // sslStream.WriteTimeout = 10000;
                        }
                        // 处理回调
                        if (httpInvoke != null)
                        {
                            HttpRequest request = new HttpRequest(networkStream, sslStream, tcpClient);
                            try
                            {
                                HttpResponse response = new HttpResponse(networkStream);
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
                    catch (Exception ex)
                    {
                        httpInvoke.HttpException(null, ex);
                    }
                    finally
                    {
                        networkStream.Close();
                        sslStream?.Close();
                        tcpClient.Close();
                    }
                }
                catch (Exception ex)
                {
                    httpInvoke.HttpException(null, ex);
                }
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
                listener.Stop();// 立刻停止访问请求
            }
            catch (Exception ex)
            {
                httpInvoke.HttpException(null, ex);
            }
        }
    }
}
