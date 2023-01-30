#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;

namespace ECSharp.Time
{
    /// <summary>
    /// 系统时间接口
    /// </summary>
    public interface ISysTime
    {
        /// <summary>
        /// 获取时间
        /// </summary>
        DateTime Now { get; }
    }
}
