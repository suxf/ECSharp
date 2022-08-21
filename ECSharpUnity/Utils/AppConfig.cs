using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace ECSharp.Utils
{
    /// <summary>
    /// 应用配置文件读取
    /// <para>默认读取项目同级目录下 [应用名].config 或 [应用名].dll.config 的XML文件(dll同样调用启动程序配置)</para>
    /// <para>配置文件 可以直接通过VS新建项 配置文件类型.config 生成</para>
    /// <para>最简单的方式通过VS添加新建项选择 应用程序配置文件 后直接确定 即可生成名为 App.config 的文件 就可以了</para>
    /// <para>仅支持一级层级和二级层级</para>
    /// <para>具体格式：&lt;?xml version="1.0" encoding="utf-8" ?&gt;&lt;configuration&gt;&lt;Hello&gt;Hello World&lt;/Hello&gt;&lt;RootTest&gt;&lt;Hello&gt;Hello World2&lt;/Hello&gt;&lt;/RootTest&gt;&lt;/configuration&gt;</para>
    /// </summary>
    public static class AppConfig
    {
        /// <summary>
        /// 读取对象
        /// </summary>
        private static XDocument? doc = null;
        private static XElement? root = null;

        /// <summary>
        /// 从文件中重新读取最新的配置
        /// </summary>
        public static bool Reload()
        {
            if (File.Exists($"./{Process.GetCurrentProcess().ProcessName}.config"))
            {
                doc = XDocument.Load($"./{Process.GetCurrentProcess().ProcessName}.config");
                root = doc.Root;
                return true;
            }
            else if (File.Exists($"./{Process.GetCurrentProcess().ProcessName}.dll.config"))
            {
                doc = XDocument.Load($"./{Process.GetCurrentProcess().ProcessName}.dll.config");
                root = doc.Root;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 读取参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string? Read(string name)
        {
            if (doc == null && !Reload())
                return null;

            return root?.Element(name)?.Value;
        }

        /// <summary>
        /// 读取参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
#if !UNITY_2020_1_OR_NEWER
        public static T? Read<T>(string name)
#else
		public static T Read<T>(string name)
#endif
        {
            if (doc == null && !Reload())
                return default;

            return (T)Convert.ChangeType(root?.Element(name)?.Value, typeof(T))!;
        }

        /// <summary>
        /// 读取参数 可以往下读一级
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string? Read(string group, string name)
        {
            if (doc == null && !Reload())
                return null;

            return root?.Element(group)?.Element(name)?.Value;
        }

        /// <summary>
        /// 读取参数 可以往下读一级
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <returns></returns>
#if !UNITY_2020_1_OR_NEWER
        public static T? Read<T>(string group, string name)
#else
		public static T Read<T>(string group, string name)
#endif
        {
            if (doc == null && !Reload())
                return default;

            return (T)Convert.ChangeType(root?.Element(group)?.Element(name)?.Value, typeof(T))!;
        }
    }
}
