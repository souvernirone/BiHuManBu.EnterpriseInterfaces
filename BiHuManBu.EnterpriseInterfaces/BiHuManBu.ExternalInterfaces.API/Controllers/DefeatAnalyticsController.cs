using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using ServiceStack.Text;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 战败分析
    /// </summary>
    public class DefeatAnalyticsController : ApiController
    {
        private readonly ILog _logInfo;
        private readonly ILog _logError;
        private readonly Stopwatch _swWhere;
        private readonly IAgentService _agentService;
        private readonly IDefeatReasonHistoryService _defeatReasonHistoryService;
        private readonly IVerifyService _verifyService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="agentService"></param>
        /// <param name="verifyService"></param>
        /// <param name="defeatReasonHistoryService"></param>
        public DefeatAnalyticsController(IAgentService agentService, IDefeatReasonHistoryService defeatReasonHistoryService, IVerifyService verifyService)
        {
            try
            {
                _logInfo = LogManager.GetLogger("INFO");
                _logError = LogManager.GetLogger("ERROR");
                _agentService = agentService;
                _defeatReasonHistoryService = defeatReasonHistoryService;
                _verifyService = verifyService;
                _swWhere = new Stopwatch();
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 战败统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetDefeatAnalytics([FromUri] DefeatAnalyticsRequest request)
        {
            _logInfo.InfoFormat("获取战败统计请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson());
            _swWhere.Start();
            var viewModel = new DefeatAnalyticsViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                //安全校验
                var baseResponse = _verifyService.Verify(request.SecCode, Request.GetQueryNameValuePairs());
                if (baseResponse.ErrCode != 1)
                {
                    //校验失败，返回错误码
                    viewModel.BusinessStatus = baseResponse.ErrCode;
                    viewModel.StatusMessage = baseResponse.ErrMsg;
                    return viewModel.ResponseToJson();
                }
                viewModel = _agentService.GetDefeatAnalytics(request.RoleType==4?request.TopAgentId: request.AgentId, request.StartTime, request.EndTime);
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功！";
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            _swWhere.Stop();
            _logInfo.Info("获取战败统计请求运行时间:" + _swWhere.Elapsed);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 原因分析
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetReasonAnalytics([FromUri] ReasonAnalyticsRequest request)
        {
            _logInfo.InfoFormat("获取原因分析请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson());
            _swWhere.Start();
            var viewModel = new DefeatAnalyticsViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                //安全校验
                var baseResponse = _verifyService.Verify(request.SecCode, Request.GetQueryNameValuePairs());
                if (baseResponse.ErrCode != 1)
                {
                    //校验失败，返回错误码
                    viewModel.BusinessStatus = baseResponse.ErrCode;
                    viewModel.StatusMessage = baseResponse.ErrMsg;
                    return viewModel.ResponseToJson();
                }
                viewModel = _agentService.GetReasonAnalytics(request.RoleType==4?request.TopAgentId:request.AgentId, request.StartTime, request.EndTime);
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功！";
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            _swWhere.Stop();
            _logInfo.Info("获取原因分析请求运行时间:" + _swWhere.Elapsed);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 业务员战败数据统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetAgentAnalytics([FromUri] AgentAnalyticsRequest request)
        {
            _logInfo.InfoFormat("获取业务员战败数据统计请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson());
            _swWhere.Start();
            var viewModel = new DefeatAnalyticsViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                //安全校验
                var baseResponse = _verifyService.Verify(request.SecCode, Request.GetQueryNameValuePairs());
                if (baseResponse.ErrCode != 1)
                {
                    //校验失败，返回错误码
                    viewModel.BusinessStatus = baseResponse.ErrCode;
                    viewModel.StatusMessage = baseResponse.ErrMsg;
                    return viewModel.ResponseToJson();
                }
                if (request.RoleType == 4)
                    request.AgentId = request.TopAgentId;
                viewModel = _agentService.GetAgentAnalytics(request);
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功！";
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            _swWhere.Stop();
            _logInfo.Info("获取业务员战败数据统计请求运行时间:" + _swWhere.Elapsed);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 移动统计获取所有代理人战败总计列表
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="agentId">团队经理ID</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="searchText">代理人名称</param>
        /// <param name="categoryName">栏目名称</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDefeatAnalytics4Mobile(DateTime startTime, DateTime endTime, int agentId, int pageIndex = 1, int pageSize = 10, string searchText = null, string categoryName = null)
        {
            var result = _agentService.GetDefeatAnalytics4Mobile(startTime, endTime.AddDays(1), agentId, pageIndex, pageSize, searchText, categoryName);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 移动统计获取单个代理人战败数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="topAgentId">顶级代理人ID</param>
        /// <param name="isViewAllData">是否查看所有数据</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="categoryName">栏目名称</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDefeatAnalyticsDetails(DateTime startTime, DateTime endTime, int topAgentId, int isViewAllData = 0, int pageIndex = 1, int pageSize = 10, string categoryName = null)
        {
            var result = _defeatReasonHistoryService.GetDefeatAnalyticsDetailsByPage(startTime, endTime.AddDays(1), topAgentId, isViewAllData, pageIndex, pageSize, categoryName);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 原因分析
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetReasonAnalytics4Mobile([FromUri] ReasonAnalyticsRequest4Mobile request)
        {
            _logInfo.InfoFormat("获取原因分析请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson());
            _swWhere.Start();
            var viewModel = new DefeatAnalyticsViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                //安全校验
                var baseResponse = _verifyService.Verify(request.SecCode, Request.GetQueryNameValuePairs());
                if (baseResponse.ErrCode != 1)
                {
                    //校验失败，返回错误码
                    viewModel.BusinessStatus = baseResponse.ErrCode;
                    viewModel.StatusMessage = baseResponse.ErrMsg;
                    return viewModel.ResponseToJson();
                }
                viewModel = _agentService.GetReasonAnalytics4Mobile(request.AgentId, request.StartTime, request.EndTime.AddDays(1), request.categoryName, request.IsViewAllData);
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功！";
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            _swWhere.Stop();
            _logInfo.Info("获取原因分析请求运行时间:" + _swWhere.Elapsed);
            return viewModel.ResponseToJson();
        }
    }

}
