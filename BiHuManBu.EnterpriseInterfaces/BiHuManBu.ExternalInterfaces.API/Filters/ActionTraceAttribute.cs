using System;
using System.Web.Http.Filters;
using log4net;

namespace BiHuManBu.ExternalInterfaces.API.Filters
{
    /// <summary>
    /// 监控接口执行时间
    /// </summary>
    public class ActionTraceAttribute : ActionFilterAttribute
    {
        static readonly object LockObj = new object();
        private readonly ILog _log = LogManager.GetLogger("TRACE");
        private DateTime _startTime;

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            _startTime = DateTime.Now;
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            int outSeconds;
            int.TryParse((System.Configuration.ConfigurationManager.AppSettings["ActionTrace"] ?? "60"), out outSeconds);

            var timeSpan = (DateTime.Now - _startTime).Seconds;
            if (timeSpan >= outSeconds)
            {
                lock (LockObj)
                {
                    //IO操作  以后缓存，报表
                    var info = string.Format("Action请求串：{0}，请求时间{1}秒", actionExecutedContext.ActionContext.Request.RequestUri,
                        timeSpan);
                    _log.Info(info);
                    
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}