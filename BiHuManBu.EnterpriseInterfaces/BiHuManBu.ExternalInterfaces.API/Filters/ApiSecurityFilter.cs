using BiHuManBu.ExternalInterfaces.API.Filters.Common.ApiSecurityUtils;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System.Configuration;
using System.Security.Cryptography;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace BiHuManBu.ExternalInterfaces.API.Filters
{
    public class ApiSecurityFilterAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //action具有[AllowAnonymous]特性不参与验证
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().OfType<AllowAnonymousAttribute>().Any(x => x is AllowAnonymousAttribute))
            {
                base.OnAuthorization(actionContext);
                return;
            }
            var request = actionContext.Request;
            string sign = string.Empty, timestamp = string.Empty, expireTime = string.Empty, platformtype = string.Empty, privateKey = string.Empty, expiry = string.Empty;
            if (!request.Headers.Contains("sign") || !request.Headers.Contains("platformtype") || !request.Headers.Contains("timestamp") || !request.Headers.Contains("expiry"))
            {
                HandleUnauthorizedRequest(actionContext);
                return;
            }
            sign = request.Headers.GetValues("sign").FirstOrDefault();
            timestamp = request.Headers.GetValues("timestamp").FirstOrDefault();
            platformtype = request.Headers.GetValues("platformtype").FirstOrDefault();
            expiry = request.Headers.GetValues("expiry").FirstOrDefault();
            privateKey = Encoding.UTF8.GetString(Convert.FromBase64String(ConfigurationManager.AppSettings[string.Format("PlatformPrivateKey_{0}",platformtype)]));

            //根据请求类型拼接参数
            string data = string.Empty;
            if (request.Method == HttpMethod.Post)
            {
                Stream sm = actionContext.Request.Content.ReadAsStreamAsync().Result;
                StreamReader streamReader = new StreamReader(sm);
                data = streamReader.ReadToEnd();
                HttpContext.Current.Request.InputStream.Position = 0;
            }
            else if (request.Method == HttpMethod.Get)
            {
                NameValueCollection form = HttpContext.Current.Request.QueryString;
                var sortedParams = new SortedDictionary<string, string>();
                //第一步：取出所有get参数
                for (int f = 0; f < form.Count; f++)
                {
                    string key = form.Keys[f];
                    sortedParams.Add(key, form[key]);
                }
                // 第二步：把所有参数名和参数值串在一起
                StringBuilder query = new StringBuilder();
                data = string.Join("&", sortedParams.Select(x => x.Key + "=" + x.Value.Trim()).ToArray());
            }
            else
            {
                HandleUnauthorizedRequest(actionContext);
                return;
            }

            bool result = SignValidate(expiry, privateKey, timestamp, data, sign);
            if (!result)
            {
                HandleUnauthorizedRequest(actionContext);
                return;
            }
            //判断timespan是否有效
            int expirySecond = expiry != "0" ? int.Parse(expiry) * 1000 : int.Parse(ConfigurationManager.AppSettings["UrlExpireTime"]) * 1000;
            double ts2 = Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds, 2), ts = ts2 - double.Parse(timestamp);
            bool falg = ts > expirySecond;
            if (falg)
            {
                HandleUnauthorizedRequest(actionContext);
                return;
            }


            base.IsAuthorized(actionContext);
        }
        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            var response = filterContext.Response = filterContext.Response ?? new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.Forbidden;
            var content = new
            {
                BusinessStatus = -10403,
                StatusMessage = "服务端拒绝访问"
            };
            response.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        }
        private bool SignValidate(string expiry, string privateKey, string timestamp, string data, string sign)
        {
            bool isValidate = false;
            var tempSign = RSAUtils.RSADecrypt(privateKey, sign);
            if (RSAUtils.EncryptSHA256(string.Format("expiry{0}timestamp{1}", expiry,timestamp) + data) == tempSign)
            {
                isValidate = true;
            }
            return isValidate;
        }
        private static string RSADecrypt(string privateKey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privateKey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(HttpUtility.UrlDecode(content)), false);
            return Encoding.UTF8.GetString(cipherbytes);

        }
  
        private static string encryptSHA256(string strSrc)
        {
            byte[] sourceByte = Encoding.UTF8.GetBytes(strSrc);
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] cryByte = sha256.ComputeHash(sourceByte);
            return byteToHexStr(cryByte);
        }

        //将数组转换成16进制字符串
        private static string byteToHexStr(byte[] bytes)
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