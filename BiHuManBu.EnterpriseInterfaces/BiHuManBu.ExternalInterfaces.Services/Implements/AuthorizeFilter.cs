using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    /// <summary>
    /// 自定义验证Api调用者是调用权限
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]

    public class AuthorizeFilter : AuthorizeAttribute
    {
        private const string BasicAuthResponseHeader = "WWW-Authenticate";
        private const string BasicAuthResponseHeaderValue = "Basic";

        /// <summary>
        /// Api密钥
        /// </summary>
        private readonly string _apiSecretKey = System.Configuration.ConfigurationManager.AppSettings["MobileSecKey"];

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            ILog logInfo = LogManager.GetLogger("APP");
            try
            {
                AuthenticationHeaderValue authValue = actionContext.Request.Headers.Authorization;
                logInfo.Info("验证请求Head：" + actionContext.Request.Headers);
                if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter) && authValue.Scheme == BasicAuthResponseHeaderValue)
                {
                    Credentials parsedCredentials = ParseAuthorizationHeader(authValue.Parameter);
                    if (parsedCredentials == null)
                    {
                        logInfo.Info("授权被拒绝");
                        throw new Exception("已拒绝为此请求授权！");
                    }
                    //获取时间戳
                    IEnumerable<string> stamps = null;
                    if (!actionContext.Request.Headers.TryGetValues("Timestamp", out stamps))
                    {
                        logInfo.Info("授权被拒绝");
                        throw new Exception("已拒绝为此请求授权！");
                    }
                    string stamp = stamps.FirstOrDefault();
                    //时间戳校验时效：2分钟
                    if (ConvertToTimeStmap(DateTime.Now) - long.Parse(stamp) > 120)
                    {
                        logInfo.Info("授权被拒绝");
                        throw new Exception("已拒绝为此请求授权！");
                    }
                    //userid+密钥+时间戳的md5加密校验
                    string mySecret = (parsedCredentials.UserId + _apiSecretKey + stamp).GetMd5().ToUpper();
                    if (!parsedCredentials.AppSecret.ToUpper().Equals(mySecret))
                    {
                        logInfo.Info("授权被拒绝");
                        throw new Exception("已拒绝为此请求授权！");
                    }
                }
                else
                {
                    logInfo.Info("授权被拒绝");
                    throw new Exception("已拒绝为此请求授权！");
                }

            }
            catch (Exception)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                actionContext.Response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);
                base.OnAuthorization(actionContext);
                return;
            }
        }


        private Credentials ParseAuthorizationHeader(string authHeader)
        {
            string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authHeader)).Split(new[] { ':' });

            if (credentials.Length < 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
                return null;

            return new Credentials() { UserId = credentials[0], AppSecret = credentials[1] };
        }


        /// <summary>
        /// 将时间转换为时间戳
        /// </summary>
        /// <param name="dt">时间类型对象比如(DateTime.Now)</param>
        /// <returns></returns>
        public long ConvertToTimeStmap(DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
        /// <summary>
        /// 将时间戳转化为对应的时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public DateTime ConvertToDateTime(long time)
        {
            DateTime timeStamp = new DateTime(1970, 1, 1);  //得到1970年的时间戳
            long t = (time + 8 * 60 * 60) * 10000000 + timeStamp.Ticks;
            DateTime dt = new DateTime(t);
            return dt;
        }
    }

    internal class Credentials
    {
        //机器码
        public string UserId { get; set; }
        public string AppSecret { get; set; }
    }
}
