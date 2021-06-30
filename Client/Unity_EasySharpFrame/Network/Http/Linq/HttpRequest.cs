using System;
using System.IO;
using System.Net;
using System.Text;

namespace ES.Network.Http.Linq
{
    /// <summary>
    /// 简单HTTP请求
    /// <para>对http请求进行简单的访问</para>
    /// </summary>
    public static class HttpRequest
    {
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postDataStr">请求数据</param>
        /// <returns>返回数据</returns>
        public static string Post(string url, string postDataStr)
        {
            return HttpPost(url, postDataStr, 0);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns>返回数据</returns>
        public static string Get(string url)
        {
            return HttpGet(url, 0);
        }

        /// <summary>
        /// Post请求
        /// <para>可以在请求失败后重新尝试</para>
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postDataStr">请求数据</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static string Post(string url, string postDataStr, int retryNum)
        {
            return HttpPost(url, postDataStr, retryNum);
        }

        /// <summary>
        /// Get请求
        /// <para>可以在请求失败后重新尝试</para>
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="retryNum">重试次数</param>
        /// <returns>返回数据</returns>
        public static string Get(string url, int retryNum)
        {
            return HttpGet(url, retryNum);
        }

        /// <summary>
        /// POST方法
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postDataStr">post数据</param>
        /// <param name="depthNum">重试深度</param>
        /// <returns></returns>
        private static string HttpPost(string url, string postDataStr, int depthNum)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ServerCertificateValidationCallback = delegate { return true; };
                Encoding encoding = Encoding.UTF8;
                byte[] postData = encoding.GetBytes(postDataStr);
                request.ContentLength = postData.Length;
                Stream myRequestStream = request.GetRequestStream();
                myRequestStream.Write(postData, 0, postData.Length);
                myRequestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch 
            {
                if (depthNum <= 0) throw;
                else return HttpPost(url, postDataStr, depthNum - 1);
            }
        }


        /// <summary>
        /// GET方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="depthNum">重试深度</param>
        /// <returns></returns>
        private static string HttpGet(string url, int depthNum)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.ServerCertificateValidationCallback = delegate { return true; };
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch
            {
                if (depthNum <= 0) throw;
                else return HttpGet(url, depthNum - 1);
            }
        }
    }
}
