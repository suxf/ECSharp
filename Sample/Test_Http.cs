using ES.Network.Http;
using ES.Network.Http.Linq;
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
            // 给访问器增加函数
            visitor.Add("Hello", handle.Hello);
            // 建立http服务，填写前缀地址并且赋予访问器
            // 注：全监听 0.0.0.0 在这里用 + 号代替
            HttpService service = new HttpService("http://127.0.0.1:8080", visitor);
            // 启动服务
            service.StartServer();
            // 然后就可以通过浏览器或其他请求工具来访问了
            // 访问地址： http://127.0.0.1:8080/Hello?text=World

            Console.ReadLine();
        }

        class HttpHandle1 : IHttpVisitorException
        {
            public void Hello(HttpConnection conn)
            {
                if (!conn.getValue.TryGetValue("text", out var text)) text = "text没有内容";
                conn.writer.Write("Hello World:" + text);
            }

            public void CatchOnRequestException(HttpConnection conn, Exception ex)
            {
                // http异常处理
            }
        }

        /* 使用方法二 */
        public void Http2()
        {
            // 先继承http异常接口，这里把测试的访问函数也写在一个类中，实际不需要
            HttpHandle2 handle = new HttpHandle2();
            // 建立http服务，填写前缀地址并且赋予访问器
            // 注：全监听 0.0.0.0 在这里用 + 号代替
            HttpService service = new HttpService(handle);
            // 这里需要添加所有完整监听链接
            service.AddFullPrefix("http://127.0.0.1:8080/Hello");
            // 启动服务
            service.StartServer();
            // 然后就可以通过浏览器或其他请求工具来访问了
            // 访问地址： http://127.0.0.1:8080/Hello?text=World

            Console.ReadLine();
        }

        class HttpHandle2 : HttpInvoke
        {
            public void HttpException(Exception exception, HttpConnection conn)
            {
                // http异常处理
            }

            public void OnRequest(HttpConnection conn)
            {
                // 这里是全部消息回调接口
                // 所以如果需要高度自定义可以使用此方法
            }
        }
    }
}
