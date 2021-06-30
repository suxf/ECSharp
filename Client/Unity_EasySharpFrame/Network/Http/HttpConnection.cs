using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ES.Network.Http
{
    /// <summary>
    /// 超文本访问连接对象
    /// <para>对象包含了此次访问的所有参数</para>
    /// </summary>
    public class HttpConnection
    {
        /// <summary>
        /// 访问详细参数
        /// </summary>
        public HttpListenerRequest request;
        /// <summary>
        /// 访问方法
        /// </summary>
        public HttpMethodType methodType;
        /// <summary>
        /// 访问后缀。
        /// <para>链接除协议、主机和参数外的部分</para>
        /// </summary>
        public string suffix;
        /// <summary>
        /// GET访问参数
        /// <para>post模式下也可以获得get的参数</para>
        /// </summary>
        public Dictionary<string, string> getValue;
        /// <summary>
        /// POST访问参数
        /// </summary>
        public string postValue;
        /// <summary>
        /// POST访问参数 字节形式
        /// </summary>
        public byte[] postBuffer;
        /// <summary>
        /// 返回对象
        /// <para>可以通过此对象写入返回内容</para>
        /// </summary>
        public HttpListenerResponse response;
        /// <summary>
        /// 写入对象
        /// <para>可以通过此对象写入返回内容</para>
        /// </summary>
        public StreamWriter writer;
    }
}
