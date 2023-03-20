#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;
using System.IO.Compression;
using System.IO;
using System.Text;

namespace ECSharp.Utils
{
    /// <summary>
    /// 基于GZip的基本压缩器
    /// </summary>
    public static class Compression
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CompressBase64(string data)
        {
            return Convert.ToBase64String(Compress(Encoding.UTF8.GetBytes(data)));
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DecompressBase64(string data)
        {
            return Encoding.UTF8.GetString(Decompress(Convert.FromBase64String(data)));
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CompressString(string data)
        {
            return Compress(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DecompressString(byte[] data)
        {
            return Encoding.UTF8.GetString(Decompress(data));
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="inputBytes"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] inputBytes)
        {
            using MemoryStream outputStream = new MemoryStream();
            using (GZipStream gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                gzipStream.Write(inputBytes, 0, inputBytes.Length);
            }

            return outputStream.ToArray();
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="compressedData"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] compressedData)
        {
            using MemoryStream inputStream = new MemoryStream(compressedData);
            using MemoryStream outputStream = new MemoryStream();
            using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(outputStream);
            }

            return outputStream.ToArray();
        }

    }
}
