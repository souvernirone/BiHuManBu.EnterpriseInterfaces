using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class DefeatReasonHistoryController : ApiController
    {
        readonly IDefeatReasonHistoryService _defeatReasonHistoryService;

        readonly ILog _logInfo = LogManager.GetLogger("INFO");
        readonly ILog _logError = LogManager.GetLogger("ERROR");
        public DefeatReasonHistoryController(IDefeatReasonHistoryService defeatReasonHistoryService, IAgentService agentService)
        {
            _defeatReasonHistoryService = defeatReasonHistoryService;
        }

        /// <summary>
        /// 获取战败历史列表
        /// </summary>
        /// <param name="seachDefeatReasonHistoryCondition">过滤条件</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDefeatReasonHistory([FromUri]SeachDefeatReasonHistoryCondition seachDefeatReasonHistoryCondition)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "获取失败" };
            var defeatReasonHistoryList = new List<DefeatReasonDataViewModel>();
            int totalCount = 0;

            try
            {

                defeatReasonHistoryList = _defeatReasonHistoryService.GetDefeatReasonHistory(seachDefeatReasonHistoryCondition, out totalCount);

                defeatReasonHistoryList.ForEach(x => x.CreateTimeStr = x.CreateTime.Value.ToString("yyyy-MM-dd"));

                baseViewModel.BusinessStatus = 1;
                baseViewModel.StatusMessage = "获取成功";
                _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(seachDefeatReasonHistoryCondition)));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "系统异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));

            }

            return new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, TotalCount = totalCount, DefeatReasonHistory = defeatReasonHistoryList }.ResponseToJson();
        }
        /// <summary>
        /// 获取战败历史列表数量    -刘松年  2018-7-9
        /// </summary>
        /// <param name="seachDefeatReasonHistoryCondition">过滤条件</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDefeatReasonHistoryCount([FromUri]SeachDefeatReasonHistoryCondition seachDefeatReasonHistoryCondition)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "获取失败" };
            var defeatReasonHistoryList = new List<DefeatReasonDataViewModel>();
            int totalCount = 0;

            try
            {

                totalCount = _defeatReasonHistoryService.GetDefeatReasonHistoryCount(seachDefeatReasonHistoryCondition);
                baseViewModel.BusinessStatus = 1;
                baseViewModel.StatusMessage = "获取成功";
                _logInfo.Info(string.Format("获取获取战败历史数量请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(seachDefeatReasonHistoryCondition)));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "系统异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));

            }

            return new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, TotalCount = totalCount }.ResponseToJson();
        }

        /// <summary>
        /// 添加数据到战败历史列表  
        /// </summary>
        /// <param name="singleDefeatReasonDataViewModel">单条战败数据模型</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddToDefeatReasonHistory([FromBody]SingleDefeatReasonDataViewModel singleDefeatReasonDataViewModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "添加失败" };
            bool isSuccess = false;
            try
            {

                isSuccess = _defeatReasonHistoryService.AddToDefeatReasonHistory(singleDefeatReasonDataViewModel);
                if (isSuccess)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "添加成功";
                    _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(singleDefeatReasonDataViewModel)));
                }
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "系统异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }
        [HttpGet,Log("战败数据刷新续保")]
        public HttpResponseMessage UpdateDefeatHistoryOldList()
        {
            return _defeatReasonHistoryService.UpdateDefeatHistoryOldList().ResponseToJson();
        }
    }
}
