using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.JDPay.signature
{
   public class MD5Util
    {
        public static string Md5(string data)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(Encoding.Unicode.GetBytes(data));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.AppendFormat("{0:X2}", b);
            return sb.ToString();
        }
        public static string Md5UpperCase(string text, string salt)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(Encoding.Unicode.GetBytes(text+salt));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.AppendFormat("{0:X2}", b);
            return sb.ToString();
        }
        public static string  Md5LowerCase(string text,string salt)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(text + salt));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }
        public static bool Verify(string text, string salt, string md5)
        {
            var md5Text = Md5UpperCase(text, salt);
            return md5Text == md5 ? true : false;

       
        }
    }
}
