using ECSharp;
using ECSharp.Network.Http;
using ECSharp.Network.Http.Linq;
using System;

namespace Sample
{
    class Test_HttpHelper
    {
        public Test_HttpHelper()
        {
            Test();
        }

        async void Test()
        {
            string result1 = await HttpRequestHelper.Post("http://www.baidu.com", "");
            Log.Debug("Post:" + result1);
            string result2 = await HttpRequestHelper.Get("http://www.baidu.com");
            Log.Info("Get:" + result2);
        }
    }
}
