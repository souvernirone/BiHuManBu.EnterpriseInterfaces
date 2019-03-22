using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using log4net;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;

namespace BiHuManBu.ExternalInterfaces.API.Filters
{
    public class LogAttribute : ActionFilterAttribute
    {
        private readonly ILog _logOutSource = log4net.LogManager.GetLogger("OUTSOURCE");
        /// <summary>
        /// 日志标识
        /// </summary>
        public string LogFlag { get; set; }

        /// <summary>
        /// 是否是全局记录日志
        /// </summary>
        public bool IsGlobal { get; set; }

        /// <summary>
        /// 是否记录全部返回值
        /// </summary>
        public bool IsWriteAllResult { get; set; }
        /// <summary>
        /// 是否为对外接口
        /// </summary>
        public bool IsOutSource { get; set; }

        public LogAttribute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isGlobal">是否是全局记录日志</param>
        /// <param name="isWriteAllResult">是否记录全部返回值</param>
        public LogAttribute(bool isGlobal, bool isWriteAllResult)
        {
            IsWriteAllResult = isWriteAllResult;
            IsGlobal = isGlobal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFlag">日志标识</param>
        /// <param name="isOutSource">是否对外</param>
        public LogAttribute(string logFlag,bool isOutSource=false)
        {
            LogFlag = logFlag;
            IsOutSource = isOutSource;
        }

        public LogAttribute(bool isWriteAllResult)
        {
            IsWriteAllResult = isWriteAllResult;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            StringBuilder msgSb = new StringBuilder();
            if (IsGlobal && SkipWriteLog(actionExecutedContext))
                return;
            // 请求标识 请求地址  请求参数
            var requestUri = actionExecutedContext.Request.RequestUri.ToString();
            var requestArguments = string.Empty;

            if (!string.IsNullOrEmpty(LogFlag))
            {
                msgSb.Append("日志标识：" + LogFlag + "\r\n");
            }
            msgSb.Append("请求地址：" + requestUri + "\r\n");

            if (actionExecutedContext.Request.Method == HttpMethod.Post)
            {
                if (actionExecutedContext.ActionContext.ActionArguments.Count > 0)
                {
                    requestArguments = JsonHelper.Serialize(actionExecutedContext.ActionContext.ActionArguments);
                    msgSb.Append("请求参数：" + requestArguments + "\r\n");
                }
            }

            var response= actionExecutedContext.Response;
            if(response!=null)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                if (IsWriteAllResult)
                {
                    msgSb.Append("请求结果：" + result + "\r\n");
                }
                else
                {
                    try
                    {
                        var briefResult = JsonHelper.DeSerialize<BaseViewModel>(result);
                        var strBriefResult = JsonHelper.Serialize(briefResult);
                        msgSb.Append("请求结果（部分）：" + strBriefResult + "\r\n");
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                msgSb.Append("请求结果：无返回值");
            }

            // 移除最后的回车
            var msg = msgSb.Remove(msgSb.Length - 2, 2).ToString();
            if (IsOutSource)
                _logOutSource.Info(msg);
            else
                LogHelper.Info(msg);
        }


        public static bool SkipWriteLog(HttpActionExecutedContext actionExecutedContext)
        {
            if (!actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<LogAttribute>().Any())
            {
                return actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<LogAttribute>().Any();
            }
            return true;
        }
    }
}