using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.ZCTeam;
using BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Extends;
using BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;
using log4net;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 增城人保团队接口
    /// </summary>
    public class ZCTeamController : ApiController
    {
        private readonly IGetTeamTaskSettingService _getTeamTaskSettingService;
        private readonly ISaveTeamTaskSettingService _saveTeamTaskSettingService;
        private readonly IGetTeamIncomingSettingService _getTeamIncomingSettingService;
        private readonly ISaveTeamIncomingSettingService _saveTeamIncomingSettingService;
        private readonly IGetOrderAndChildAgentService _getOrderAndChildAgentService;
        private readonly IGetAgentSonPremiumService _getAgentSonPremiumService;
        private readonly IZCTeamService _zcTeamService;
        private readonly IUpdateCompleteTaskService _updateCompleteTaskService;
        private readonly ITeamIncomeService _teamIncomeService;
        private readonly ITeamListService _getTeamListService;
        private readonly ITeamListService _teamListService;
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");

        public ZCTeamController(IGetTeamTaskSettingService getTeamTaskSettingService, ISaveTeamTaskSettingService saveTeamTaskSettingService, IGetTeamIncomingSettingService getTeamIncomingSettingService, ISaveTeamIncomingSettingService saveTeamIncomingSettingService, IGetOrderAndChildAgentService getOrderAndChildAgentService, IGetAgentSonPremiumService getAgentSonPremiumService, IZCTeamService zcTeamService, IUpdateCompleteTaskService updateCompleteTaskService, ITeamIncomeService teamIncomeService, ITeamListService teamListService)
        {
            _getTeamTaskSettingService = getTeamTaskSettingService;
            _saveTeamTaskSettingService = saveTeamTaskSettingService;
            _getTeamIncomingSettingService = getTeamIncomingSettingService;
            _saveTeamIncomingSettingService = saveTeamIncomingSettingService;
            _getOrderAndChildAgentService = getOrderAndChildAgentService;
            _getAgentSonPremiumService = getAgentSonPremiumService;
            _zcTeamService = zcTeamService;
            _updateCompleteTaskService = updateCompleteTaskService;
            _teamIncomeService = teamIncomeService;
            _teamListService = teamListService;
        }

        /// <summary>
        /// 保存团队收益 /2018-02-05 /增城PC后台 /光鹏洁
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public HttpResponseMessage SaveTeamIncomingSetting([FromBody]SaveTeamIncomingSettingRequest request)
        {
            var model = _saveTeamIncomingSettingService.SaveTeamIncomingSetting(request.TeamIncomingSetting);
            return model.ResponseToJson();
        }
        /// <summary>
        /// 获取团队收益 /2018-02-05 /增城PC后台 /光鹏洁
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetTeamIncomingSetting([FromUri]GetTeamIncomingSettingRequest request)
        {
            var model = _getTeamIncomingSettingService.GetTeamIncomingSetting();
            return model.ResponseToJson();
        }
        /// <summary>
        /// 保存团队任务设置 /2018-02-04 /增城PC后台 /光鹏洁
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage SaveTeamTaskSetting([FromUri]SaveTeamTaskSettingRequest request)
        {
            var model = _saveTeamTaskSettingService.SaveTeamTaskSetting(request.AgentCount, request.OrderCount);
            return model.ResponseToJson();
        }
        /// <summary>
        /// 获取团队任务设置 /2018-02-04 /增城PC后台 /光鹏洁
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetTeamTaskSetting([FromUri]GetTeamTaskSettingRequest request)
        {
            var model = _getTeamTaskSettingService.GetTeamTaskSetting();
            return model.ResponseToJson();
        }
        /// <summary>
        /// 获取剩余分享用户数量和剩余出单数量 sjy 2018-2-4
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetOrderAndChildAgent(int agentId)
        {
            return _getOrderAndChildAgentService.GetOrderAndChildAgent(agentId).ResponseToJson();
        }
        /// <summary>
        /// 获取下级代理人单人净保费列表 sjy 2018-2-5
        /// modify 齐大康  2018-04-20
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetAgentSonPremium(int agentId,string createTime="")
        {
            logInfo.Info("获取下级代理人单人净保费列表：" + Request.RequestUri + "请求参数:agentId" + agentId + ",createTime=" + createTime);
            return _getAgentSonPremiumService.GetAgentSonPremium(agentId, createTime).ResponseToJson();
        }

        /// <summary>
        /// 获取当前代理人下级和下下级的总人数和总收益 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        //[CustomizedRequestAuthorize]
        public HttpResponseMessage GetSonAndGrandsonIncome(int agentId)
        {
            BaseViewModel viewModel = new BaseViewModel();
            viewModel = _zcTeamService.GetSonAndGrandsonIncome(agentId);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 获取团队级别、总净保费、预计收益 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        //[CustomizedRequestAuthorize]
        public HttpResponseMessage GetTeamLevelMoneyExpectedIncome(int agentId)
        {
            BaseViewModel viewModel = new BaseViewModel();
            viewModel = _zcTeamService.GetTeamLevelMoneyExpectedIncome(agentId);
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 完成团队任务创建团队 sjy 2018-2-6
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public HttpResponseMessage UpdateCompleteTask(int agentId)
        {
            return _updateCompleteTaskService.UpdateCompleteTask(agentId).ResponseToJson();
        }

        /// <summary>
        /// 制定时间结算团队收益 李金友 2018-2-8
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("制定时间结算团队收益")]
        public HttpResponseMessage SetTeamIncomeByDay([FromUri]GetTeamIncomeByDayRequest request)
        {
            return _teamIncomeService.SetTeamIncomeByDay(request).ResponseToJson();
        }

        /// <summary>
        /// 查询团队列表数据  qidakang  2018-04-09
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        //[CustomizedRequestAuthorize]
        public HttpResponseMessage GetTeamList([FromUri]GetTeamManagerRequest request)
        {
            logInfo.Info("查询团队列表数据请求串为：" + Request.RequestUri + "请求参数:" + request.ResponseToJson());
            TeamListViewModel viewModel = new TeamListViewModel();
            try
            {
                viewModel = _teamListService.GetTeamList(request);
                return viewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("请求串为：" + Request.RequestUri + "\n发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return viewModel.ResponseToJson();
            }
        }
        /// <summary>
        /// 获取团队列表-二级团员保费明细 qidakang  2018-04-09
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        //[CustomizedRequestAuthorize]
        public HttpResponseMessage GetTeamChildLevelList([FromUri]GetTeamChildLevelListRequest request)
        {
            logInfo.Info("获取二级团员保费明细请求串为：" + Request.RequestUri + "请求参数:" + request.ResponseToJson());
            TeamChildLevelListViewModel viewModel = new TeamChildLevelListViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _teamListService.GetTeamChildLevelList(request);
                return viewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("请求串为：" + Request.RequestUri + "\n发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return viewModel.ResponseToJson();
            }
        }

        /// <summary>
        /// 1.代理人邀请的人员数据
        /// 2.近期邀请的10个人 qidakang 2018-04-11
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
       // [CustomizedRequestAuthorize]
        public HttpResponseMessage GetNextLevelAgentList([FromUri] GetNextLevelAgentListRequest request)
        {
            logInfo.Info("获取代理人邀请（最新10个）的人员请求串为：" + Request.RequestUri + "请求参数:" + request.ResponseToJson());
            NextLevelAgentListViewModel viewModel = new NextLevelAgentListViewModel();
            try
            {
                viewModel = _teamListService.GetNextLevelAgentList(request);
                return viewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("请求串为：" + Request.RequestUri + "\n发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return viewModel.ResponseToJson();
            }
        }
    }
}