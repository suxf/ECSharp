using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace ES.Utils
{
    /// <summary>
    /// Aes加密
    /// </summary>
    public class AesCrypto
    {
        /// <summary>
        /// AES对象
        /// </summary>
        private readonly ConcurrentBag<Aes> bag = new ConcurrentBag<Aes>();

        /// <summary>
        /// AES加密密钥
        /// </summary>
        private string Key = "";

        /// <summary>
        /// 新建一个Aes加密
        /// </summary>
        public AesCrypto()
        {
            CreateAes();
        }

        private void CreateAes()
        {
            for (int i = 0; i < SystemInfo.ProcessorCount; i++)
            {
                Aes aes = Aes.Create();
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;
                aes.BlockSize = 128;
                if (Key != "") aes.Key = Encoding.UTF8.GetBytes(Key);
                bag.Add(aes);
            }
        }

        private Aes Pop()
        {
            if (!bag.TryTake(out var aes))
            {
                CreateAes();
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
            Key = key;
            byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
            foreach (Aes aes in bag) aes.Key = keyBytes;
        }

        /// <summary>
        /// 获取密钥
        /// <para>如果没有则自动创建一个密钥返回</para>
        /// </summary>
        /// <returns></returns>
        public string GetKey()
        {
            if (Key == "")
            {
                Key = RandomCode.Generate(16, RandomCode.RandomCodeType.HighLetterAndNumber).ToLower();
                byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
                foreach (Aes aes in bag) aes.Key = keyBytes;
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
        public byte[] Decrypt(byte[] decryptArray)
        {
            Aes aes = Pop();
            lock (aes)
            {
                using (ICryptoTransform cTransform = aes.CreateDecryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(decryptArray, 0, decryptArray.Length);
                    Push(aes);
                    return resultArray;
                }
            }
        }
    }
}
