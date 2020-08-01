using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ES.Data.Storage
{
    /// <summary>
    /// 简单信息存储,也可以自己读/写入新文件
    /// 用于文本生成一些由用户或机器产生的持久数据存储
    /// 默认存储文件为根目录下:default.json
    /// </summary>
    public static class EasyStorage
    {
        /// <summary>
        /// json缓存
        /// </summary>
        private static JObject jsonCache = null;

        /// <summary>
        /// 获取所有信息内容
        /// </summary>
        /// <returns></returns>
        public static JObject GetAll()
        {
            if(jsonCache != null) return jsonCache;
            var data = ReadData("default.json");
            if (string.IsNullOrWhiteSpace(data)) data = "{}";
            jsonCache = JsonConvert.DeserializeObject<JObject>(data);
            return jsonCache;
        }

        /// <summary>
        /// 获取对应key值数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static JToken Get(string key)
        {
            if (GetAll().ContainsKey(key)) return GetAll()[key];
            else return default;
        }

        /// <summary>
        /// 设置某个key值数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(string key, JToken value)
        {
            if (GetAll().ContainsKey(key)) GetAll()[key] = value;
            else GetAll().Add(key, value);
            // 写入数据
            WriteData(JsonConvert.SerializeObject(GetAll()), "default.json");
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data">数据内容</param>
        /// <param name="fileName">文件名和后缀类型，这里不需要带路径</param>
        /// <param name="path">路径[路径最后需要包含斜杠]，默认当前程序根目录</param>
        public static void WriteData(string data, string fileName, string path = ".\\")
        {
            // 创建目录
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            FileStream fs = new FileStream(path + fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            // 开始写入
            sw.Write(data);
            // 清空缓冲区
            sw.Flush();
            // 关闭流
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="fileName">文件名和后缀类型，这里不需要带路径</param>
        /// <param name="path">路径[路径最后需要包含斜杠]，默认当前程序根目录</param>
        public static string ReadData(string fileName, string path = ".\\")
        {
            // 查看是否为空
            if (!File.Exists(path + fileName)) return null;
            // 开始读取
            FileStream fs = new FileStream(path + fileName, FileMode.Open);
            StreamReader sw = new StreamReader(fs);
            // 开始读取
            string data = sw.ReadToEnd();
            // 关闭流
            sw.Close();
            fs.Close();
            return data;
        }
    }
}
