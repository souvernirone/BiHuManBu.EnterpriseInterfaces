using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Implements;
using log4net;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 代理人提现业务  李金友 2018-02-03 
    /// </summary>
    public class AgentWithdrawalController : ApiController
    {
        private readonly IAgentWithdrawalService _agentWithdrawalService;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");

        public AgentWithdrawalController(IAgentWithdrawalService agentWithdrawalService)
        {
            _agentWithdrawalService = agentWithdrawalService;
        }

        /// <summary>
        /// 新增提现记录  李金友 2018-02-02 /WX
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        private HttpResponseMessage AddWithdrawalList([FromUri]ListWithdrawalRequest request)
        {
            logInfo.Info(string.Format("新增提现记录：{0}", Request.RequestUri));
            return _agentWithdrawalService.AddWithdrawalList(request).ResponseToJson();
        }

        /// <summary>
        /// 获取提现金额  李金友 2018-02-02 /WX
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        private HttpResponseMessage GetListCommissions([FromUri]BaseRequest2 request)
        {
            logInfo.Info(string.Format("获取提现金额：{0}", Request.RequestUri));
            return _agentWithdrawalService.GetListCommissions(request).ResponseToJson();
        }

        /// <summary>
        /// 获取提现列表  李金友 2018-02-02 /WX
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetPageListWithdrawal([FromUri] PageListWithdrawalRequest request)
        {
            logInfo.Info(string.Format("提现列表：{0}", Request.RequestUri));
            return _agentWithdrawalService.GetPageListWithdrawal(request).ResponseToJson();
        }
        /// <summary>
        /// 设置提现状态（审核状态：默认-1，通过1，未通过0，已支付2）  李金友 2018-02-04 /WX
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage UpdateWithdrawalAuditStatus([FromUri]WithdrawalAuditRequset request)
        {
            logInfo.Info(string.Format("设置提现状态：{0}", Request.RequestUri));
            return _agentWithdrawalService.UpdateWithdrawalAuditStatus(request).ResponseToJson();
        }
        /// <summary>
        /// 获取佣金提现详情  李金友 2018-02-04 /WX
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetMoneyWithdrawalDetial([FromUri]WithdrawalAuditRequset request)
        {
            logInfo.Info(string.Format("获取佣金提现详情：{0}", Request.RequestUri));
            return _agentWithdrawalService.GetMoneyWithdrawalDetial(request).ResponseToJson();
        }

        /// <summary>
        /// 获取团队收益提现详情  李金友 2018-02-04 /WX
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetTeamMoneyWithdrawalDetial([FromUri]WithdrawalAuditRequset request)
        {
            logInfo.Info(string.Format("获取团队收益提现详情：{0}", Request.RequestUri));
            return _agentWithdrawalService.GetTeamMoneyWithdrawalDetial(request).ResponseToJson();
        }
    }
}
