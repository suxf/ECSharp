#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using ECSharp.Utils;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace ECSharp.Crypto
{
    /// <summary>
    /// AES加密
    /// </summary>
    public class AES
    {
        /// <summary>
        /// AES对象
        /// </summary>
        private readonly ConcurrentBag<Aes> bag = new ConcurrentBag<Aes>();

        /// <summary>
        /// AES加密密钥
        /// </summary>
        private byte[] Key = ByteConverter.Empty;

        /// <summary>
        /// 新建一个Aes加密
        /// </summary>
        /// <param name="single">只创建一个单线程的加密器</param>
        public AES(bool single = true)
        {
            CreateAes(single);
        }

        /// <summary>
        /// 新建一个Aes加密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="single">只创建一个单线程的加密器</param>
        public AES(byte[] key, bool single = true)
        {
            Key = key;
            CreateAes(single);
        }

        /// <summary>
        /// 新建一个Aes加密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="single">只创建一个单线程的加密器</param>
        public AES(string key, bool single = true)
        {
            Key = Encoding.UTF8.GetBytes(key);
            CreateAes(single);
        }

        private void CreateAes(bool single)
        {
            for (int i = 0, len = single ? 1 : SystemInfo.ProcessorCount; i < len; i++)
            {
                Aes aes = Aes.Create();
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;
                aes.BlockSize = 128;

                if (Key.Length > 0)
                    aes.Key = Key;

                bag.Add(aes);
            }
        }

        private Aes Pop()
        {
            if (!bag.TryTake(out var aes))
            {
                CreateAes(false);
                return Pop();
            }
            return aes;
        }

        private void Push(Aes aes)
        {
            bag.Add(aes);
        }

        /// <summary>
        /// 设置密钥
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(string key)
        {
            Key = Encoding.UTF8.GetBytes(key);
            foreach (Aes aes in bag) aes.Key = Key;
        }

        /// <summary>
        /// 设置密钥
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(byte[] key)
        {
            Key = key;
            foreach (Aes aes in bag) aes.Key = Key;
        }

        /// <summary>
        /// 获取密钥
        /// <para>如果没有则自动创建一个密钥返回</para>
        /// <para>使用byte[]数组密钥来获取此函数可能会报错</para>
        /// </summary>
        /// <returns></returns>
        public string GetKey()
        {
            if (Key.Length <= 0)
            {
                string key = Randomizer.Generate(16, Randomizer.RandomCodeType.HighLetterAndNumber).ToLower();
                Key = Encoding.UTF8.GetBytes(key);

                foreach (Aes aes in bag) aes.Key = Key;
            }
            return Encoding.UTF8.GetString(Key);
        }

        /// <summary>
        /// 获取密钥
        /// <para>如果没有则自动创建一个密钥返回</para>
        /// </summary>
        /// <returns></returns>
        public byte[] GetKeyBytes()
        {
            if (Key.Length <= 0)
            {
                Key = Randomizer.GenerateBytes(16);

                foreach (Aes aes in bag) aes.Key = Key;
            }
            return Key;
        }

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="encryptArray">明文（待加密）</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] encryptArray)
        {
            Aes aes = Pop();
            lock (aes)
            {
                using (ICryptoTransform cTransform = aes.CreateEncryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(encryptArray, 0, encryptArray.Length);
                    Push(aes);
                    return resultArray;
                }
            }
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="decryptArray">密文（待解密）</param>
        /// <returns></returns>
        public byte[]? Decrypt(byte[] decryptArray)
        {
            Aes aes = Pop();
            lock (aes)
            {
                using (ICryptoTransform cTransform = aes.CreateDecryptor())
                {
                    byte[] resultArray;
                    try
                    {
                        resultArray = cTransform.TransformFinalBlock(decryptArray, 0, decryptArray.Length);
                    }
                    catch
                    {
                        aes.Clear();
                        return null;
                    }
                    finally
                    {
                        Push(aes);
                    }
                    return resultArray;
                }
            }
        }
    }
}
