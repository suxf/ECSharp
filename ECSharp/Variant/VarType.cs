namespace ECSharp.Variant
{
    /// <summary>
    /// 可变变量类型
    /// </summary>
    public enum VarType : byte
    {
        /// <summary>
        /// 空值
        /// </summary>
        NULL = 0x00,
        /* 数值存储位 */
        /// <summary>
        /// 字节型
        /// </summary>
        BYTE = 0x01,
        /// <summary>
        /// 有符号字节型
        /// </summary>
        SBYTE = 0x02,
        /// <summary>
        /// 短整型
        /// </summary>
        INT16 = 0x03,
        /// <summary>
        /// 无符号短整型
        /// </summary>
        UINT16 = 0x04,
        /// <summary>
        /// 整型
        /// </summary>
        INT32 = 0x05,
        /// <summary>
        /// 无符号整型
        /// </summary>
        UINT32 = 0x06,
        /// <summary>
        /// 长整型
        /// </summary>
        INT64 = 0x07,
        /// <summary>
        /// 无符号长整型
        /// </summary>
        UINT64 = 0x08,
        /// <summary>
        /// 单精度浮点型
        /// </summary>
        FLOAT = 0x09,
        /// <summary>
        /// 双精度浮点型
        /// </summary>
        DOUBLE = 0x0A,
        /// <summary>
        /// 布尔型
        /// </summary>
        BOOL = 0x0B,
        /// <summary>
        /// 字符串型
        /// </summary>
        STRING = 0x0C,
        /// <summary>
        /// 列表
        /// </summary>
        VARLIST = 0x0D,
        /// <summary>
        /// 字典
        /// </summary>
        VARMAP = 0x0E,
        /// <summary>
        /// 结构体
        /// </summary>
        STRUCT = 0x0F,

        /// <summary>
        /// 值字典 头
        /// </summary>
        VARMAP_HEAD = 0xFB,
        /// <summary>
        /// 值字典 结尾
        /// </summary>
        VARMAP_END = 0xFC,
        /// <summary>
        /// 值列表 头
        /// </summary>
        VARLIST_HEAD = 0xFD,
        /// <summary>
        /// 值列表 结尾
        /// </summary>
        VARLIST_END = 0xFE,
        /// <summary>
        /// 对象
        /// </summary>
        OBJECT = 0xFF,
    }
}
