using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;

namespace BiHuManBu.ExternalInterfaces.API.Filters
{
    /// <summary>
    /// 记录操作日志
    /// </summary>
    public class OperationLogAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 请求的名称
        /// </summary>
        public string ActionDescription { get; set; }

        public OperationLogAttribute(string actionDescription)
        {
            ActionDescription = actionDescription;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            AccidentSettingRepository _accidentSettingRepository = new AccidentSettingRepository();
            tx_operation_log log = new tx_operation_log();
            log.action_name = ActionDescription;
            log.action_url= actionContext.Request.RequestUri.LocalPath;
            log.operation_time = DateTime.Now;
            log.operator_id = 0;
            log.top_agent_id = 0;
            var param = actionContext.ActionArguments;
            var method = actionContext.Request.Method.Method;
            if (method == "GET")
            {
                foreach (var kv in param)
                {
                    if (kv.Key.ToUpper() == "AGENTID")
                    {
                        log.operator_id = Convert.ToInt32(kv.Value);
                    }
                    else if (kv.Key.ToUpper() == "TOPAGENTID")
                    {
                        log.top_agent_id = Convert.ToInt32(kv.Value);
                    }
                }
                if (log.operator_id == 0 || log.top_agent_id == 0)
                {
                    actionContext.Response = new { BusinessStatus = -10000, StatusMessage = "参数不正确" }.ResponseToJson();
                }
                else
                {
                    log.request_parameter = JsonConvert.SerializeObject(param);
                    _accidentSettingRepository.AddOperationLog(log);
                    base.OnActionExecuting(actionContext);
                }
            }
            else if (method == "POST")
            {
                HttpContextBase context = (HttpContextBase)actionContext.Request.Properties["MS_HttpContext"];
                byte[] bt = new byte[context.Request.InputStream.Length];
                context.Request.InputStream.Read(bt, 0, bt.Length);
                var req = System.Text.Encoding.UTF8.GetString(bt);
                req = HttpContext.Current.Server.UrlDecode(req);
                var jsonStr = JsonConvert.DeserializeObject<dynamic>(req.ToUpper());
                var agentId = Convert.ToInt32(jsonStr.AGENTID);
                var topAgentId = Convert.ToInt32(jsonStr.TOPAGENTID);
                if (agentId == 0 || topAgentId == 0)
                {
                    actionContext.Response = new { BusinessStatus = -10000, StatusMessage = "参数不正确" }.ResponseToJson();
                }
                else
                {
                    log.request_parameter = req;
                    log.operator_id = agentId;
                    log.top_agent_id = topAgentId;
                    _accidentSettingRepository.AddOperationLog(log);
                    base.OnActionExecuting(actionContext);
                }
            }
        }
    }
}