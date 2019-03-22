using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

namespace BiHuManBu.ExternalInterfaces.API.Filters
{
    /// <summary>
    /// 这个已经不用了，使用ErrorHandler代替
    /// </summary>
    public class GlobalExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var requestUri = actionExecutedContext.Request.RequestUri.ToString();
            var requestArguments = string.Empty;

            StringBuilder msgSb = new StringBuilder();
            msgSb.Append("请求地址：" + requestUri + "\r\n");

            if (actionExecutedContext.Request.Method == HttpMethod.Post)
            {
                requestArguments = JsonHelper.Serialize(actionExecutedContext.ActionContext.ActionArguments);

                msgSb.Append("请求参数：" + requestArguments + "\r\n");
            }

            Exception ex = actionExecutedContext.Exception;

            msgSb.Append("异常信息：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

            LogHelper.Error(msgSb.ToString());

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(System.Net.HttpStatusCode.OK
                , new { BusinessStatus = -10003, StatusMessage = "服务发生异常" }
                , "application/json");
        }
    }

    public class ErrorHandler : ExceptionHandler
    {
        public async  override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {

            var requestUri = context.Request.RequestUri.ToString();
            StringBuilder msgSb = new StringBuilder();
            msgSb.Append("请求地址：" + requestUri + Environment.NewLine);

            Exception ex = context.Exception;

            if (context.Request.Method == HttpMethod.Post)
            {
                var stream = await context.Request.Content.ReadAsStreamAsync();
                stream.Seek(0, SeekOrigin.Begin);
                var streamReader = new StreamReader(stream);
                var requestParam = streamReader.ReadToEnd();

                msgSb.Append("请求参数：" + Environment.NewLine + requestParam + Environment.NewLine);
            }

            msgSb.Append("异常信息：" + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine);

            LogHelper.Error(msgSb.ToString());

            var result = "{\"BusinessStatus\":\"-10003\",\"StatusMessage\":\"服务发生异常\"}";
            context.Result = new TestExceptionResult(result);
        }
    }

    public class TestExceptionResult : IHttpActionResult
    {
        private string _content;

        public TestExceptionResult(string content)
        {
            _content = content;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json"),
                StatusCode = System.Net.HttpStatusCode.OK
            };
            return Task.FromResult(response);
        }
    }

}