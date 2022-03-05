using ES.Utils;
using System.Security.Cryptography;
using System.Text;

namespace ES.Network.HyperSocket
{
    /// <summary>
    /// 安全协议类
    /// </summary>
    internal class SSL
    {
        /// <summary>
        /// RSA对象
        /// </summary>
        private readonly RSACryptoServiceProvider? rsa;
        /// <summary>
        /// AES对象
        /// </summary>
        private readonly Aes? aes;

        /// <summary>
        /// RSA加密公钥
        /// </summary>
        internal readonly string? RSAPublicKey;
        /// <summary>
        /// RSA加密私钥
        /// </summary>
        private readonly string? RSAPrivateKey;

        /// <summary>
        /// AES加密密钥
        /// </summary>
        private string? AESKey = null;

        /// <summary>
        /// 安全协议模式
        /// </summary>
        internal enum SSLMode
        {
            Both,
            RSA,
            AES
        }

        internal SSL(SSLMode mode)
        {
            if (mode == SSLMode.RSA || mode == SSLMode.Both)
            {
                rsa = new RSACryptoServiceProvider();
                RSAPublicKey = rsa.ToXmlString(false);
                RSAPrivateKey = rsa.ToXmlString(true);
                rsa.FromXmlString(RSAPrivateKey);
            }

            if (mode == SSLMode.AES || mode == SSLMode.Both)
            {
                aes = Aes.Create();
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;
                aes.BlockSize = 128;
            }
        }

        internal SSL(SSLMode mode, string rsaPublickey)
        {
            if (mode == SSLMode.RSA || mode == SSLMode.Both)
            {
                rsa = new RSACryptoServiceProvider();
                RSAPublicKey = rsaPublickey;
                rsa.FromXmlString(RSAPublicKey);
            }

            if (mode == SSLMode.AES || mode == SSLMode.Both)
            {
                aes = Aes.Create();
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;
                aes.BlockSize = 128;
            }
        }

        internal string GetRSAPublicKey()
        {
            return RSAPublicKey ?? "";
        }

        internal void SetAESKey(string aesKey)
        {
            AESKey = aesKey;
            aes!.Key = Encoding.UTF8.GetBytes(AESKey);
        }

        internal string GetAESKey()
        {
            if (AESKey == null)
            {
                AESKey = RandomCode.Generate(16, RandomCode.RandomCodeType.HighLetterAndNumber).ToLower();
                aes!.Key = Encoding.UTF8.GetBytes(AESKey);
            }
            return AESKey;
        }

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="encryptArray">明文（待加密）</param>
        /// <returns></returns>
        internal byte[] AESEncrypt(byte[] encryptArray)
        {
            ICryptoTransform cTransform = aes!.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(encryptArray, 0, encryptArray.Length);
            return resultArray;
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="decryptArray">密文（待解密）</param>
        /// <returns></returns>
        internal byte[]? AESDecrypt(byte[] decryptArray)
        {
            try
            {
                ICryptoTransform cTransform = aes!.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(decryptArray, 0, decryptArray.Length);
                return resultArray;
            }
            catch { return null; }
        }

        /// <summary>
        /// RSA公钥文件加密纯文本。
        /// </summary>
        /// <param name="encryptArray">要加密的文本</param>
        /// <returns>表示加密数据的64位编码字符串.</returns>
        internal byte[] RSAEncrypt(byte[] encryptArray)
        {
            return rsa!.Encrypt(encryptArray, false);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="decryptArray">加密的密文</param>
        /// <returns></returns>
        internal byte[]? RSADecrypt(byte[] decryptArray)
        {
            try
            {
                return rsa!.Decrypt(decryptArray, false);
            }
            catch { return null; }
        }

        /// <summary>
        /// RSA公钥文件签名纯文本。
        /// </summary>
        /// <param name="strArray">要签名的文本</param>
        /// <returns></returns>
        internal byte[] RSASignData(byte[] strArray)
        {
            return rsa!.SignData(strArray, "SHA1");
        }

        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="strArray">要签名的密文</param>
        /// <param name="signArray">签名</param>
        /// <returns></returns>
        internal bool RSAVerifyData(byte[] strArray, byte[] signArray)
        {
            return rsa!.VerifyData(strArray, "SHA1", signArray);
        }
    }
}
