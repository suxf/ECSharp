using System;

namespace ES.Time
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
