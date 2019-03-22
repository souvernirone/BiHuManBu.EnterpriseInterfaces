using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BiHuManBu.ExternalInterfaces.API.Filters
{
    /// <summary>
    /// 模型验证过滤器
    /// </summary>
    public class ModelVerifyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if(actionContext.ActionArguments.Where(o=>o.Value!=null).Any())
            {
                if (!actionContext.ModelState.IsValid)
                {
                    var listErrMsg = new List<string>();
                    foreach (var value in actionContext.ModelState.Values)
                    {
                        if (value.Errors.AsEnumerable().Any())
                        {
                            var errMsg = value.Errors.FirstOrDefault().ErrorMessage;
                            if (string.IsNullOrEmpty(errMsg))
                            {
                                errMsg = value.Errors.FirstOrDefault().Exception.Message;
                            }
                            listErrMsg.Add(errMsg);
                        }
                    }
                    HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);

                    var data = new
                    {
                        BusinessStatus = -10000,
                        StatusMessage = string.Join(" ", listErrMsg)
                    };

                    response.Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
                    actionContext.Response = response;
                }
            }
            else
            {
                // 这里是一个参数也没传的情况
                var data = new
                {
                    BusinessStatus = -10000,
                    StatusMessage = "参数不能为空"
                };
                HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.Accepted)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json")
                };
                actionContext.Response = response;
            }
            
        }
    }
}