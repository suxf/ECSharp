#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ECSharp.Network.Http.Linq
{
    /// <summary>
    /// 简单HTTP请求助手
    /// <para>对http请求进行简单的访问</para>
    /// </summary>
    public static class HttpRequestHelper
    {
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postDataStr">请求数据</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static async Task<string?> Post(string url, string postDataStr, int retryNum = 0)
        {
            return await Post(url, Encoding.UTF8.GetBytes(postDataStr), ContentType.KeyValue, retryNum);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postDataStr">请求数据</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static async Task<string?> Post(string url, string postDataStr, string contentType, int retryNum = 0)
        {
            return await Post(url, Encoding.UTF8.GetBytes(postDataStr), contentType, retryNum);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">请求数据</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static async Task<string?> Post(string url, byte[] postData, int retryNum = 0)
        {
            return await Post(url, postData, ContentType.Binary, retryNum);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">请求数据</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static async Task<string?> Post(string url, byte[] postData, string contentType, int retryNum = 0)
        {
            var content = new ByteArrayContent(postData);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Headers.ContentType.CharSet = "utf-8";
            content.Headers.ContentLength = postData.Length;
            var result = await HttpPost(url, content, retryNum);
            if (result == null) return null;
            return await result.ReadAsStringAsync();
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postDataStr">请求数据</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static async Task<byte[]?> PostBytes(string url, string postDataStr, int retryNum = 0)
        {
            return await PostBytes(url, postDataStr, ContentType.KeyValue, retryNum);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postDataStr">请求数据</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static async Task<byte[]?> PostBytes(string url, string postDataStr, string contentType, int retryNum = 0)
        {
            return await PostBytes(url, Encoding.UTF8.GetBytes(postDataStr), contentType, retryNum);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">请求数据</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static async Task<byte[]?> PostBytes(string url, byte[] postData, int retryNum = 0)
        {
            return await PostBytes(url, postData, ContentType.Binary, retryNum);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">请求数据</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static async Task<byte[]?> PostBytes(string url, byte[] postData, string contentType, int retryNum = 0)
        {
            var content = new ByteArrayContent(postData);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Headers.ContentType.CharSet = "utf-8";
            content.Headers.ContentLength = postData.Length;
            var result = await HttpPost(url, content, retryNum);
            if (result == null) return null;
            return await result.ReadAsByteArrayAsync();
        }

        private static async Task<HttpContent?> HttpPost(string url, HttpContent content, int depthNum)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    if (depthNum <= 0) return null;
                    else return await HttpPost(url, content, depthNum - 1);
                }

                return response.Content;
            }
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static async Task<string?> Get(string url, int retryNum = 0)
        {
            var result = await HttpGet(url, retryNum);
            if (result == null) return null;
            return await result.ReadAsStringAsync();
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static async Task<byte[]?> GetBytes(string url, int retryNum = 0)
        {
            var result = await HttpGet(url, retryNum);
            if (result == null) return null;
            return await result.ReadAsByteArrayAsync();
        }

        private static async Task<HttpContent?> HttpGet(string url, int depthNum)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    if (depthNum <= 0) return null;
                    else return await HttpGet(url, depthNum - 1);
                }

                return response.Content;
            }
        }
    }
}
