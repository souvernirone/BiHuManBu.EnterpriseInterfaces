using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helper
{
   public class RSACryptionHelper
    {


       private static string xmlKeys = "<RSAKeyValue><Modulus>3HPOIY7URu3TqYHm7OYM3iiTDRmJiF6+EFk1h3xxt+/Cs2a9JKkWv5sqnSu9UpcQSjSR8k9Jeb7pvuhn0BrLL20l4ilRksGbsO4mOlyukxpR6sQDroiBAJduZhpBptlKmcjMUbi1Us+GZrm8SNe5EkjAkkDB/083TGhvQRt44K0=</Modulus><Exponent>AQAB</Exponent><P>6LED6T7rY3Ynic/mHbRieixbyyKvHV+muSxRLdYsbLb/XVb1GqGCqwaCDl/0Sy3ar8vqecwn2/7N3HamVxjoUw==</P><Q>8ojwRVpH7jk2+REgTYArur7UpfgCwyH16kY6pQ2OMmrtixGD8XncL2bH/5oAa7kVnq+bulAhWcSyri/t7Gny/w==</Q><DP>UwWthpAbfj5O9x3FVf3hUQP4sG6smkdhfhow0LDk4tkcHdqD0z+jFpBY4PYdfKFcsHKKM3DDG/w6yRlQWi0Z3w==</DP><DQ>6aGGKpiUL+QHk5eu0VlFRqgrOhGVv5kIRfwO5A4Ry3l7h/qAam3j7/mHcS5Nd3ecXvd1AN3NsqarJOZgv9szyw==</DQ><InverseQ>eak6teDrMYmjc9xY/s2uW4ZxBrl26PkzbD5wO99Icpb6x9rfatDGDIA7TqFnuu5MEH/Q2y2xQ8ZPcDGXP1ygJw==</InverseQ><D>YLctp+tRlgyg36zt8uC6pHyIhya8/+cQ23tH0Lj36wp1EN/x+zNzl3wrpeialrFPpD6MCRJf2dm4EtlDMjmPCd8K8s0cNkKUG1SUd8VpEtzRxMy7Cp1pniBcTdchjZG2ZqHLLDQNFspgygNQrjf3+JAj2rVxvgJMmxQiBjJ47ME=</D></RSAKeyValue>";
       private static string xmlPublicKeys = "<RSAKeyValue><Modulus>3HPOIY7URu3TqYHm7OYM3iiTDRmJiF6+EFk1h3xxt+/Cs2a9JKkWv5sqnSu9UpcQSjSR8k9Jeb7pvuhn0BrLL20l4ilRksGbsO4mOlyukxpR6sQDroiBAJduZhpBptlKmcjMUbi1Us+GZrm8SNe5EkjAkkDB/083TGhvQRt44K0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        #region RSA 加密解密 

        #region RSA 的密钥产生 
    
        /// <summary>
        /// RSA 的密钥产生 产生私钥 和公钥 
        /// </summary>
        /// <param name="xmlKeys"></param>
        /// <param name="xmlPublicKey"></param>
        public static void RSAKey(out string xmlKeys,out string xmlPublicKey) 
        {             
                System.Security.Cryptography.RSACryptoServiceProvider rsa=new RSACryptoServiceProvider(); 
                xmlKeys=rsa.ToXmlString(true); 
                xmlPublicKey = rsa.ToXmlString(false);             
        } 
        #endregion 

        #region RSA的加密函数 
        //############################################################################## 
        //RSA 方式加密 
        //说明KEY必须是XML的行式,返回的是字符串 
        //在有一点需要说明！！该加密方式有 长度 限制的！！ 
        //############################################################################## 

        /// <summary>
        /// RSA的加密函数  string
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <param name="m_strEncryptString"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string xmlPublicKey, string m_strEncryptString) 
        { 
            
            byte[] PlainTextBArray; 
            byte[] CypherTextBArray; 
            string Result; 
            RSACryptoServiceProvider rsa=new RSACryptoServiceProvider(); 
            rsa.FromXmlString(xmlPublicKey); 
            //PlainTextBArray = (new UnicodeEncoding()).GetBytes(m_strEncryptString); 
            PlainTextBArray = (Encoding.GetEncoding("gb2312")).GetBytes(m_strEncryptString);
            CypherTextBArray = rsa.Encrypt(PlainTextBArray, false); 
            Result=Convert.ToBase64String(CypherTextBArray); 
            return Result; 
            
        }


        /// <summary>
        /// RSA的加密函数  string
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <param name="m_strEncryptString"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string m_strEncryptString)
        {
            byte[] PlainTextBArray;
            byte[] CypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKeys);
            //PlainTextBArray = (new UnicodeEncoding()).GetBytes(m_strEncryptString);
            PlainTextBArray = (Encoding.GetEncoding("gb2312")).GetBytes(m_strEncryptString);
            CypherTextBArray = rsa.Encrypt(PlainTextBArray, false);
            Result = Convert.ToBase64String(CypherTextBArray);
            return Result;
        } 

        /// <summary>
        /// RSA的加密函数 byte[]
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <param name="EncryptString"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string xmlPublicKey, byte[] EncryptString) 
        { 
            
            byte[] CypherTextBArray; 
            string Result; 
            RSACryptoServiceProvider rsa=new RSACryptoServiceProvider(); 
            rsa.FromXmlString(xmlPublicKey); 
            CypherTextBArray = rsa.Encrypt(EncryptString, false); 
            Result=Convert.ToBase64String(CypherTextBArray); 
            return Result; 
            
        }

       /// <summary>
        /// RSA的加密函数 byte[]
       /// </summary>
       /// <param name="EncryptString"></param>
       /// <returns></returns>
        public static string RSAEncrypt(byte[] EncryptString)
        {

            byte[] CypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKeys);
            CypherTextBArray = rsa.Encrypt(EncryptString, false);
            Result = Convert.ToBase64String(CypherTextBArray);
            return Result;

        }
        #endregion 

        #region RSA的解密函数 
        /// <summary>
        /// RSA的解密函数  string
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="m_strDecryptString"></param>
        /// <returns></returns>
        public static string RSADecrypt(string xmlPrivateKey, string m_strDecryptString) 
        {            
            byte[] PlainTextBArray; 
            byte[] DypherTextBArray; 
            string Result; 
            System.Security.Cryptography.RSACryptoServiceProvider rsa=new RSACryptoServiceProvider(); 
            rsa.FromXmlString(xmlPrivateKey); 
            PlainTextBArray =Convert.FromBase64String(m_strDecryptString); 
            DypherTextBArray=rsa.Decrypt(PlainTextBArray, false); 
            //Result=(new UnicodeEncoding()).GetString(DypherTextBArray); 
            Result = (Encoding.GetEncoding("gb2312")).GetString(DypherTextBArray);
            return Result; 
            
        }


        /// <summary>
        /// RSA的解密函数  string
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="m_strDecryptString"></param>
        /// <returns></returns>
        public static string RSADecrypt(string m_strDecryptString)
        {
            byte[] PlainTextBArray;
            byte[] DypherTextBArray;
            string Result;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlKeys);
            PlainTextBArray = Convert.FromBase64String(m_strDecryptString);
            DypherTextBArray = rsa.Decrypt(PlainTextBArray, false);
            Result = (Encoding.GetEncoding("gb2312")).GetString(DypherTextBArray);
            return Result;

        } 

        /// <summary>
        /// RSA的解密函数  byte
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="DecryptString"></param>
        /// <returns></returns>
        public static string RSADecrypt(string xmlPrivateKey, byte[] DecryptString) 
        {            
            byte[] DypherTextBArray; 
            string Result; 
            System.Security.Cryptography.RSACryptoServiceProvider rsa=new RSACryptoServiceProvider(); 
            rsa.FromXmlString(xmlPrivateKey); 
            DypherTextBArray=rsa.Decrypt(DecryptString, false); 
            Result=(new UnicodeEncoding()).GetString(DypherTextBArray); 
            return Result; 
            
        }

        /// <summary>
        /// RSA的解密函数  byte
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="DecryptString"></param>
        /// <returns></returns>
        public static string RSADecrypt(byte[] DecryptString)
        {
            byte[] DypherTextBArray;
            string Result;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlKeys);
            DypherTextBArray = rsa.Decrypt(DecryptString, false);
            Result = (new UnicodeEncoding()).GetString(DypherTextBArray);
            return Result;

        }
        #endregion 

        #endregion 
    }
}
