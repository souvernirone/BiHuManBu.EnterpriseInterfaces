using BiHuManBu.ExternalInterfaces.API.Filters;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace BiHuManBu.ExternalInterfaces.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // 记录请求日志
            config.Filters.Add(new LogAttribute(true, false));
            // 全局异常过滤器
            config.Services.Replace(typeof(IExceptionHandler), new Filters.ErrorHandler());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
