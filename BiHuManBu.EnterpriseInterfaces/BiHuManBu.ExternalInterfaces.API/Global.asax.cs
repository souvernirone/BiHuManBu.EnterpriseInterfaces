using BiHuManBu.ExternalInterfaces.Services;
using log4net;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace BiHuManBu.ExternalInterfaces.API
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly ILog logError = LogManager.GetLogger("ERROR");
        protected void Application_Start()
        {
            Bootstrapper.Run(GlobalConfiguration.Configuration);
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

#if DEBUG
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
#endif

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception objExp = HttpContext.Current.Server.GetLastError();
            logError.Error("应用程序发生错误_______" + ("\r\n客户机IP:" + Request.UserHostAddress + "\r\n错误地址:" + Request.Url + "\r\n异常信息:" + Server.GetLastError().Message));
        }
        
    }
}