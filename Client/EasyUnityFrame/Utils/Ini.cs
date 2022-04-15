using System;
using System.IO;

namespace ES.Utils
{
    /// <summary>
    /// ini文件读取
    /// </summary>
    public static class Ini
    {
        /// <summary>
        /// ini文件解释器
        /// </summary>
        public class IniParser
        {
            internal readonly Map<string, Map<string, string>> map = new Map<string, Map<string, string>>();

            internal IniParser() { }

            /// <summary>
            /// 值
            /// </summary>
            /// <param name="key">键名</param>
            /// <returns></returns>
            public string? GetValue(string key)
            {
                return GetSectionValue("", key);
            }

            /// <summary>
            /// 值
            /// </summary>
            /// <param name="key">键名</param>
            /// <param name="defaultValue">默认值</param>
            /// <returns></returns>
            public string GetValue(string key, string defaultValue)
            {
                return GetValue(key) ?? defaultValue;
            }

            /// <summary>
            /// 值
            /// </summary>
            /// <param name="section">节选</param>
            /// <param name="key">键名</param>
            /// <returns></returns>
            public string? GetSectionValue(string section, string key)
            {
                if (map.TryGetValue(section, out var value) && value.TryGetValue(key, out var v))
                {
                    return v;
                }
                return null;
            }

            /// <summary>
            /// 值
            /// </summary>
            /// <param name="section">节选</param>
            /// <param name="key">键名</param>
            /// <param name="defaultValue">默认值</param>
            /// <returns></returns>
            public string GetSectionValue(string section, string key, string defaultValue)
            {
                if (map.TryGetValue(section, out var value) && value.TryGetValue(key, out var v))
                {
                    return v;
                }
                return defaultValue;
            }
        }

        private readonly static Map<string, IniParser> map = new Map<string, IniParser>();
        private static readonly IniParser empty = new IniParser();
        /// <summary>
        /// 当前加载解释器
        /// <para>默认每次最后加载的解释器</para>
        /// </summary>
        public static IniParser Current { get; private set; } = empty;
        /// <summary>
        /// 获取指定解释器
        /// </summary>
        /// <param name="fileName">文件名(及后缀)</param>
        /// <returns></returns>
        public static IniParser Parsers(string fileName)
        {
            if (map.TryGetValue(fileName, out var parse))
                return parse;
            return empty;
        }
        /// <summary>
        /// 替换当前解释器
        /// </summary>
        /// <param name="fileName">文件名(及后缀)</param>
        /// <returns></returns>
        public static bool ReplaceCurrentParsser(string fileName)
        {
            if (map.TryGetValue(fileName, out var parse))
            {
                Current = parse;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 根据路径加载解释器
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="isAbsolutePath">是否为绝对路径,默认为当前运行目录路径</param>
        /// <returns></returns>
        public static bool LoadParser(string filePath, bool isAbsolutePath = false)
        {
            if (!isAbsolutePath)
            {
                filePath = Environment.CurrentDirectory + @"\" + filePath;
            }
            if (!File.Exists(filePath))
                return false;

            string fileName = Path.GetFileName(filePath);
            IniParser parser = new IniParser();
            string section = "";
            StreamReader sr = new StreamReader(filePath);
            try
            {
                while (!sr.EndOfStream)
                {
                    string? str = sr.ReadLine();
                    if (string.IsNullOrEmpty(str))
                        continue;
                    str = str.Trim();
                    if (str.StartsWith(";"))
                        continue;
                    if (str.StartsWith("[") && str.EndsWith("]"))
                    {
                        section = str.Remove(str.Length - 1, 1).Remove(0, 1);
                        continue;
                    }
                    if (!str.Contains("="))
                        continue;
                    ReadOnlySpan<string> pair = str.Split('=');
                    if (pair.Length != 2)
                        continue;

                    if (!parser.map.TryGetValue(section, out var item))
                    {
                        item = new Map<string, string>();
                        parser.map.Add(section, item);
                    }
                    item.Add(pair[0].Trim(), pair[1].Trim());
                }
            }
            finally
            {
                sr.Close();
            }
#if !UNITY_2020_1_OR_NEWER
            if (!map.TryAdd(fileName, parser))
                map[fileName] = parser;
#else 
			if (!map.ContainsKey(fileName))
                map[fileName] = parser;
            else 
                map.Add(fileName, parser);
#endif
            Current = parser;

            return true;
        }

        /// <summary>
        /// Current的值
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string? GetValue(string key)
        {
            return Current.GetSectionValue("", key);
        }

        /// <summary>
        /// Current的值
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetValue(string key, string defaultValue)
        {
            return Current.GetValue(key) ?? defaultValue;
        }

        /// <summary>
        /// Current的值
        /// </summary>
        /// <param name="section">节选</param>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string? GetSectionValue(string section, string key)
        {
            if (Current.map.TryGetValue(section, out var value) && value.TryGetValue(key, out var v))
            {
                return v;
            }
            return null;
        }

        /// <summary>
        /// Current的值
        /// </summary>
        /// <param name="section">节选</param>
        /// <param name="key">键名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetSectionValue(string section, string key, string defaultValue)
        {
            if (Current.map.TryGetValue(section, out var value) && value.TryGetValue(key, out var v))
            {
                return v;
            }
            return defaultValue;
        }
    }
}
