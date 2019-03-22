using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BiHuManBu.ExternalInterfaces.API.Filters.Common.ApiSecurityUtils
{
    public class RSAUtils
    {
        /// <summary>
        /// 根据公钥加密
        /// </summary>
        /// <param name="publickey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string publickey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
            return Convert.ToBase64String(cipherbytes);

        }
        /// <summary>
        /// 根据私钥解密
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        internal static string RSADecrypt(string privateKey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privateKey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(HttpUtility.UrlDecode(content)), false);
            return Encoding.UTF8.GetString(cipherbytes);

        }

        internal static string EncryptSHA256(string strSrc)
        {
            byte[] sourceByte = Encoding.UTF8.GetBytes(strSrc);
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] cryByte = sha256.ComputeHash(sourceByte);
            return ByteToHexStr(cryByte);
        }

        //将数组转换成16进制字符串
        private static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr.ToLower();
        }
    }
}