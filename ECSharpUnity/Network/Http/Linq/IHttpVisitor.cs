﻿#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;

namespace ECSharp.Network.Http.Linq
{
    /// <summary>
    /// 捕捉接受异常接口
    /// </summary>
    public interface IHttpVisitor
    {
        /// <summary>
        /// 异常捕捉回调
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ex"></param>
        void HttpVisitorException(HttpRequest? request, Exception ex);
    }
}
