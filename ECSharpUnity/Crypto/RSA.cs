using System.Security.Cryptography;

namespace ECSharp.Crypto
{
    /// <summary>
    /// RSA加密
    /// </summary>
    public class RSA
    {
        /// <summary>
        /// RSA对象
        /// </summary>
        private readonly RSACryptoServiceProvider rsa;

        /// <summary>
        /// RSA加密公钥
        /// </summary>
        public string PublicKey { get; private set; } = "";
        /// <summary>
        /// RSA加密私钥
        /// </summary>
        public string PrivateKey { get; private set; } = "";

        /// <summary>
        /// 新建一个Rsa加密
        /// </summary>
        public RSA()
        {
            rsa = new RSACryptoServiceProvider();
            PublicKey = rsa.ToXmlString(false);
            PrivateKey = rsa.ToXmlString(true);
            rsa.FromXmlString(PrivateKey);
        }

        /// <summary>
        /// 新建一个Rsa加密
        /// </summary>
        public RSA(string publickey)
        {
            rsa = new RSACryptoServiceProvider();
            PublicKey = publickey;
            rsa.FromXmlString(PublicKey);
        }

        /// <summary>
        /// RSA公钥文件加密纯文本。
        /// </summary>
        /// <param name="encryptArray">要加密的文本</param>
        /// <returns>表示加密数据的64位编码字符串.</returns>
        public byte[] Encrypt(byte[] encryptArray)
        {
            return rsa.Encrypt(encryptArray, false);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="decryptArray">加密的密文</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] decryptArray)
        {
            return rsa.Decrypt(decryptArray, false);
        }

        /// <summary>
        /// RSA公钥文件签名纯文本。
        /// </summary>
        /// <param name="strArray">要签名的文本</param>
        /// <returns></returns>
        public byte[] SignData(byte[] strArray)
        {
            return rsa.SignData(strArray, "SHA1");
        }

        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="strArray">要签名的密文</param>
        /// <param name="signArray">签名</param>
        /// <returns></returns>
        public bool VerifyData(byte[] strArray, byte[] signArray)
        {
            return rsa.VerifyData(strArray, "SHA1", signArray);
        }
    }
}
