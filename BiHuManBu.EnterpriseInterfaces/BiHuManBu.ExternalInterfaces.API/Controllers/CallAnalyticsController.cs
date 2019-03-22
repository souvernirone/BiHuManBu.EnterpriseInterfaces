using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class CallAnalyticsController : ApiController
    {
        readonly ICallAnalyticsService _callAnalyticsService;
        readonly ILog _logInfo = LogManager.GetLogger("INFO");
        readonly ILog _logError = LogManager.GetLogger("ERROR");
        public CallAnalyticsController(ICallAnalyticsService _callAnalyticsService)
        {
            this._callAnalyticsService = _callAnalyticsService;
        }

        /// <summary>
        /// 保存通话记录
        /// </summary>
        /// <param name="saveRecordViewModel">保存通话记录模型</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveRecord([FromBody]SaveRecordViewModel saveRecordViewModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "保存成功" };
            try
            {
                var result = _callAnalyticsService.SaveRecord(saveRecordViewModel);
                baseViewModel.BusinessStatus = result.IsSuccess ? 1 : -1;
                baseViewModel.StatusMessage = result.Message;
                _logInfo.Info(string.Format("保存通话记录url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(saveRecordViewModel)));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 更新通话记录文件上传状态和失败日志
        /// </summary>
        /// <param name="updateRecordViewModel">更新通话记录文件状态和日志模型</param>
        /// <returns></returns>

        [HttpPost]

        public HttpResponseMessage UpdateRecord([FromBody] UpdateRecordViewModel updateRecordViewModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "更改成功" };
            try
            {
                var result = _callAnalyticsService.UpdateRecord(updateRecordViewModel);
                baseViewModel.BusinessStatus = result.IsSuccess ? 1 : -1;
                baseViewModel.StatusMessage = result.Message;
         
                _logInfo.Info(string.Format("更新通话记录文件上传状态和失败日志url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(updateRecordViewModel)));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取通话统计结果
        /// </summary>
        /// <param name="searchRecordAnalyticsDataWhereViewModel">获取通话统计结果的查询条件</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetRecordAnalyticsData([FromUri]SearchRecordAnalyticsDataWhereViewModel searchRecordAnalyticsDataWhereViewModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                OverviewOfDataViewModel overviewOfDataViewModel = new OverviewOfDataViewModel();
                List<TimePassAnalysisViewModel> timePassAnalysisList = new List<TimePassAnalysisViewModel>();
                List<SalesmanStatisticsViewModel> salesmanStatisticsViewModelList = new List<SalesmanStatisticsViewModel>();
                TimePassAnalysisViewModelFinally timePassAnalysisViewModelFinally = new TimePassAnalysisViewModelFinally();
                int totalCount = 0;
                List<string> callDataInDates = new List<string>();
                if (!searchRecordAnalyticsDataWhereViewModel.IsOnlyAnalyticsBusinesser) {
               
                //获得数据概览
                 overviewOfDataViewModel = _callAnalyticsService.GetOverviewOfData(searchRecordAnalyticsDataWhereViewModel.AgentId, searchRecordAnalyticsDataWhereViewModel.TopAgentId, searchRecordAnalyticsDataWhereViewModel.EffectiveCallDuration,searchRecordAnalyticsDataWhereViewModel.AnalyticsStartTime, searchRecordAnalyticsDataWhereViewModel.AnalyticsEndTime.AddDays(1));
                //获得通时分析
                timePassAnalysisList = _callAnalyticsService.GetTimePassAnalysis(searchRecordAnalyticsDataWhereViewModel.AgentId, searchRecordAnalyticsDataWhereViewModel.AnalyticsStartTime, searchRecordAnalyticsDataWhereViewModel.AnalyticsEndTime.AddDays(1));
                    var zero_one = timePassAnalysisList.FirstOrDefault(x => x.CallTimeType == 0);
                    var one_two= timePassAnalysisList.FirstOrDefault(x => x.CallTimeType == 1);
                    var tow_five = timePassAnalysisList.FirstOrDefault(x => x.CallTimeType == 2);
                    var thanFive= timePassAnalysisList.FirstOrDefault(x => x.CallTimeType == 3);
                    timePassAnalysisViewModelFinally.Zero_One_CallTimes = zero_one == null ? 0 : zero_one.CallDurationTimes;
                    timePassAnalysisViewModelFinally.One_Two_CallTimes = one_two == null ? 0 : one_two.CallDurationTimes;
                    timePassAnalysisViewModelFinally.Two_Five_CallTimes = tow_five == null ? 0 : tow_five.CallDurationTimes;
                    timePassAnalysisViewModelFinally.TanFive_CallTimes = thanFive == null ? 0 : thanFive.CallDurationTimes;
                   
                }
                if (searchRecordAnalyticsDataWhereViewModel.IsAnalyticsBusinesser) {
                    //获得业务员统计
                    salesmanStatisticsViewModelList = _callAnalyticsService.GetSalesmanStatistics(searchRecordAnalyticsDataWhereViewModel.AgentId, searchRecordAnalyticsDataWhereViewModel.TopAgentId, searchRecordAnalyticsDataWhereViewModel.EffectiveCallDuration, searchRecordAnalyticsDataWhereViewModel.AnalyticsStartTime, searchRecordAnalyticsDataWhereViewModel.AnalyticsEndTime.AddDays(1), searchRecordAnalyticsDataWhereViewModel.PageIndex, searchRecordAnalyticsDataWhereViewModel.PageSize, false, out totalCount, out callDataInDates);
                }

                var result = new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, OverviewOfDataViewModel = overviewOfDataViewModel, TimePassAnalysisViewModelFinally = timePassAnalysisViewModelFinally, SalesmanStatisticsViewModel = new { ToTalCount = totalCount, SalesmanStatisticsViewModelList = salesmanStatisticsViewModelList } };

                _logInfo.Info(string.Format("获取通话统计结果url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(searchRecordAnalyticsDataWhereViewModel)));
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
               
            }
        
        }

        /// <summary>
        /// 导出通话统计数据
        /// </summary>
        /// <param name="searchRecordAnalyticsDataWhereViewModel"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage ExportCallData([FromUri]SearchRecordAnalyticsDataWhereViewModel searchRecordAnalyticsDataWhereViewModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                int totalCount = 0;
                List<string> callDataInDates = new List<string>();
                var salesmanStatisticsViewModelList = _callAnalyticsService.GetSalesmanStatistics(searchRecordAnalyticsDataWhereViewModel.AgentId, searchRecordAnalyticsDataWhereViewModel.TopAgentId, searchRecordAnalyticsDataWhereViewModel.EffectiveCallDuration, searchRecordAnalyticsDataWhereViewModel.AnalyticsStartTime, searchRecordAnalyticsDataWhereViewModel.AnalyticsEndTime.AddDays(1), searchRecordAnalyticsDataWhereViewModel.PageIndex, searchRecordAnalyticsDataWhereViewModel.PageSize, true, out totalCount, out callDataInDates);
                var result = new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, SalesmanStatisticsViewModel = new { ToTalCount = totalCount, SalesmanStatisticsViewModelList = salesmanStatisticsViewModelList }, CallDataInDates = callDataInDates };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }

        /// <summary>
        /// 获取通话记录
        /// </summary>
        /// <param name="searchtRecordListWhereViewModel">获取通话记录查询条件</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetRecordList([FromUri] SearchtRecordListWhereViewModel searchtRecordListWhereViewModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            int totalCount = 0;
            try
            {
                var recordList = _callAnalyticsService.GetRecordList(searchtRecordListWhereViewModel, out totalCount);
            
                _logInfo.Info(string.Format("获取通话记录url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(searchtRecordListWhereViewModel)));
                return new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, RecordList = recordList ,TotalCount= totalCount }.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
           
        }
    }
}
