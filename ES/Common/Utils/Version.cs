namespace ES.Common.Utils
{
    /// <summary>
    /// 框架版本信息
    /// </summary>
    public static class Version
    {
        /// <summary>
        /// 获取版本字符串
        /// </summary>
        /// <returns></returns>
        public new static string ToString()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
