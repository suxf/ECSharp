using ECSharp;
using ECSharp.Network.Http;
using ECSharp.Network.Http.Linq;
using System;

namespace Sample
{
    class Test_Http
    {

        public Test_Http()
        {
            Http1();
        }

        /* 使用方法一 */
        public void Http1()
        {
            // 先继承http异常接口，这里把测试的访问函数也写在一个类中，实际不需要
            HttpHandle1 handle = new HttpHandle1();
            // 建立http访问器，并载入异常接口类
            HttpVisitor visitor = new HttpVisitor(handle);
            // 建立http服务，填写地址并且赋予访问器
            // X509Certificate2 certificate = new X509Certificate2("https.pfx", "8888");
            // HttpService service = new HttpService("127.0.0.1", 8080, visitor, certificate);
            HttpService service = new HttpService("127.0.0.1", 8080, visitor);
            // 给访问器增加函数
            visitor.Add("", handle.Index);
            visitor.Add("Hello", handle.Hello);
            visitor.Add(handle.Hello2);
            // 启动服务
            service.StartServer();
            // 然后就可以通过浏览器或其他请求工具来访问了
            // 访问地址： http://127.0.0.1:8080/Hello?text=World

            Console.ReadLine();
        }

        class HttpHandle1 : IHttpVisitor
        {
            public void Index(HttpRequest request, HttpResponse response)
            {
                Log.Debug("Index Visit");
                // 首页根访问
                if (!request.GetParams.TryGetValue("text", out var text1)) text1 = "get text no content";
                if (!request.PostParams.TryGetValue("text", out var text2)) text2 = "post text no content";
                response.Write($"Index \nGet:{text1}\nPost:{text2}");
            }
            
            public void Hello(HttpRequest request, HttpResponse response)
            {
                Log.Debug("Hello Visit");
                response.Write("Hello World:" + request.PostValue);
            }

            public void Hello2(HttpRequest request, HttpResponse response)
            {
                Log.Debug("Hello Visit2");
                response.Write("Hello World2:" + request.PostValue);
            }

            public void HttpVisitorException(HttpRequest request, Exception ex)
            {
                // http异常处理
            }
        }

        /* 使用方法二 */
        public void Http2()
        {
            // 先继承http异常接口，这里把测试的访问函数也写在一个类中，实际不需要
            HttpHandle2 handle = new HttpHandle2();
            // 建立http服务，填写地址
            HttpService service = new HttpService("127.0.0.1", 8080, handle);
            // 启动服务
            service.StartServer();
            // 然后就可以通过浏览器或其他请求工具来访问了
            // 访问地址： http://127.0.0.1:8080/Hello?text=World

            Console.ReadLine();
        }

        class HttpHandle2 : IHttp
        {
            public void HttpException(HttpRequest request, Exception exception)
            {
                // http异常处理
            }

            public void OnRequest(HttpRequest request, HttpResponse response)
            {
                // 这里是全部消息回调接口
                // 所以如果需要高度自定义可以使用此方法
            }
        }
    }
}
