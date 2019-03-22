using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{

    /// <summary>
    /// 统计
    /// </summary>
    public class BusinessStatisticsController : ApiController
    {
        private readonly ILog _logInfo;
        private readonly ILog _logError;
        private readonly Stopwatch _swWhere;
        private readonly IAgentService _agentService;
        private readonly IVerifyService _verifyService;
        readonly IStatisticsService _statisticsService;
        readonly ICustomerCategories _customerCategories;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="agentService"></param>
        /// <param name="verifyService"></param>
        /// <param name="_statisticsService"></param>
        /// <param name="_customerCategories"></param>
        public BusinessStatisticsController(IAgentService agentService, IVerifyService verifyService, IStatisticsService _statisticsService, ICustomerCategories _customerCategories)
        {
            try
            {
                _logInfo = LogManager.GetLogger("INFO");
                _logError = LogManager.GetLogger("ERROR");
                _agentService = agentService;
                _verifyService = verifyService;
                this._statisticsService = _statisticsService;
                _swWhere = new Stopwatch();
                this._customerCategories = _customerCategories;
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        #region 业务统计、战败统计、战败分析
        /// <summary>
        /// 业务统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetBusinessStatistics([FromUri] BusinessStatisticsRequest request)
        {
            _logInfo.InfoFormat("获取业务统计请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson());
            _swWhere.Start();
            var viewModel = new BusinessStatisticsViewModel();
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
                viewModel = _agentService.GetBusinessStatistics(request.RoleType == 4 ? request.TopAgentId : request.AgentId, request.StartTime, request.EndTime);
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功！";
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            _swWhere.Stop();
            _logInfo.Info("获取业务统计请求运行时间:" + _swWhere.Elapsed);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 趋势图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetTrendMap([FromUri]BusinessStatisticsRequest request)
        {
            _logInfo.InfoFormat("获取趋势图统计请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson());
            _swWhere.Start();
            var viewModel = new TrendMapViewModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    viewModel.BusinessStatus = -10000;
                    string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    viewModel.StatusMessage = "输入参数错误，" + msg;
                    return viewModel.ResponseToJson();
                }
                //安全校验
                var baseResponse = _verifyService.Verify(request.SecCode, Request.GetQueryNameValuePairs());
                if (baseResponse.ErrCode != 1)
                {//校验失败，返回错误码
                    viewModel.BusinessStatus = baseResponse.ErrCode;
                    viewModel.StatusMessage = baseResponse.ErrMsg;
                    return viewModel.ResponseToJson();
                }

                viewModel = _agentService.GetTrendMap(request.RoleType == 4 ? request.TopAgentId : request.AgentId, request.StartTime, request.EndTime);

                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功！";
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            _swWhere.Stop();
            _logInfo.Info("获取趋势图统计请求运行时间:" + _swWhere.Elapsed);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 业务员统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAgentData([FromUri] AgentDataRequest request)
        {
            _swWhere.Start();
            _logInfo.InfoFormat("获取业务员统计请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson());
            var viewModel = new AgentStatisticsViewModel();

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
                {//校验失败，返回错误码
                    viewModel.BusinessStatus = baseResponse.ErrCode;
                    viewModel.StatusMessage = baseResponse.ErrMsg;
                    return viewModel.ResponseToJson();
                }
                viewModel = _agentService.GetAgentDataByPage(request.RoleType == 4 ? request.TopAgentId : request.AgentId, request.StartTime, request.EndTime, request.IsDesc, request.OrderBy, request.CurPage, request.PageSize, request.SearchTxt, request.IsByLevel);
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功！";
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            _swWhere.Stop();
            _logInfo.Info("获取业务员数据请求运行时间:" + _swWhere.Elapsed);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 单个业务员的业务统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSingleAgentData([FromUri] SingleAgentDataRequest request)
        {
            _logInfo.InfoFormat("获取单个业务员的业务统计请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson());
            var viewModel = new AgentStatisticsViewModel();

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
                viewModel = _agentService.GetSingleAgentData(request.AgentId, request.StartTime, request.EndTime);
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }
        #endregion

        #region 续保统计、工作统计

        /// <summary>
        ///初始数据到数据库
        /// </summary>
        /// <param name="dataInTimeStart">数据统计开始时间</param>
        /// <param name="dataInTimeEnd">数据统计结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage InitDataIntoDB(DateTime? dataInTimeStart, DateTime? dataInTimeEnd)
        {
            if (!dataInTimeStart.HasValue)
            {
                dataInTimeStart = DateTime.Today.AddDays(-1);
            }
            if (!dataInTimeEnd.HasValue)
            {
                dataInTimeEnd = DateTime.Today;

            }
            var isSuccess = _statisticsService.InitDataIntoDB(dataInTimeStart.Value, dataInTimeEnd.Value);
            return new { BusinessStatus = 1, StatusMessage = "数据统计成功", IsSuccess = isSuccess }.ResponseToJson();

        }
        /// <summary>
        /// 续保统计->跟进分布
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="agentId">当前查询代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="roleType">角色</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetFollowUpAllocationResult(DateTime bizEndDate, int agentId, int topAgentId, int roleType = 0)
        {

            var agentIdAndNameTuple = GetSearchAgentIdAndName(false, topAgentId, roleType == 4 ? topAgentId : agentId);

            var categoryNames = GetSearchCategories(topAgentId);
            categoryNames.Add("未分类");
            var followUpAllocationResult = _statisticsService.GetFollowUpAllocationResult(bizEndDate, agentIdAndNameTuple.Item3);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", FollowUpAllocationResult = followUpAllocationResult, CategoryNames = categoryNames }.ResponseToJson();
        }
        /// <summary>
        /// 续保统计->跟进分布->业务员明细
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="agentId">当前登录代理人编号</param>
        /// <param name="roleType">角色</param>
        /// <param name="topAgentId"> 顶级代理人编号</param>
        /// <param name="searchAgentId">当前搜索的代理人编号</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetFollowUpAllocationResultAboutAgent(DateTime bizEndDate, int topAgentId, int agentId, int roleType = 0, int searchAgentId = -1, int pageIndex = 1, int pageSize = 10, string customerCategory = null)
        {

            var agentIdAndNameTuple = GetSearchAgentIdAndName(true, topAgentId, roleType == 4 ? topAgentId : agentId, pageIndex, pageSize);
            var followUpAllocationResultAboutAgent = _statisticsService.GetFollowUpAllocationResultAboutAgent(bizEndDate, customerCategory, searchAgentId == -1 ? agentIdAndNameTuple.Item3 : new List<int> { searchAgentId });
            return new { BusinessStatus = 1, StatusMessage = "获取成功", TotalAgentIdAndName = agentIdAndNameTuple.Item1, FollowUpAllocationResultAboutAgent = followUpAllocationResultAboutAgent, PagingAgentIdAndName = searchAgentId == -1 ? agentIdAndNameTuple.Item2 : agentIdAndNameTuple.Item1.Where(x => x.AgentId == searchAgentId).ToList(), TotalCount = agentIdAndNameTuple.Item4 }.ResponseToJson();
        }
        /// <summary>
        /// 续保统计->出单分布
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="statusIdStr">回访状态编号字符串</param>
        /// <param name="agentId">当前查询代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="roleType">角色</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetOutOrderAllocationResult(DateTime bizEndDate, string statusIdStr, int agentId, int topAgentId, int roleType = 0)
        {
            var agentIdAndNameTuple = GetSearchAgentIdAndName(false, topAgentId, roleType == 4 ? topAgentId : agentId);
            var categoryNames = GetSearchCategories(topAgentId);
            categoryNames.Add("未分类");
            var outOrderAllocationResult = _statisticsService.GetOutOrderAllocationResult(bizEndDate, statusIdStr, agentIdAndNameTuple.Item3);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", OutOrderAllocationResult = outOrderAllocationResult, CategoryNames = categoryNames }.ResponseToJson();

        }
        /// <summary>
        /// 续保统计->出单分布->业务员明细
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="agentId">当前登录代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="statusIdStr">回访状态编号字符串</param>
        /// <param name="roleType">角色</param>
        /// <param name="searchAgentId">当前查询的代理人编号</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetOutOrderAllocationResultAboutAgent(DateTime bizEndDate, int topAgentId, int agentId, string statusIdStr, int roleType = 0, int searchAgentId = -1, int pageIndex = 1, int pageSize = 10, string customerCategory = null)
        {
            var agentIdAndNameTuple = GetSearchAgentIdAndName(true, topAgentId, roleType == 4 ? topAgentId : agentId, pageIndex, pageSize);
            var outOrderAllocationResultAboutAgent = _statisticsService.GetOutOrderAllocationResultAboutAgent(bizEndDate, customerCategory, searchAgentId == -1 ? agentIdAndNameTuple.Item3 : new List<int> { searchAgentId }, statusIdStr);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", OutOrderAllocationResultAboutAgent = outOrderAllocationResultAboutAgent, TotalAgentIdAndName = agentIdAndNameTuple.Item1, PagingAgentIdAndName = searchAgentId == -1 ? agentIdAndNameTuple.Item2 : agentIdAndNameTuple.Item1.Where(x => x.AgentId == searchAgentId).ToList(), TotalCount = agentIdAndNameTuple.Item4 }.ResponseToJson();

        }
        /// <summary>
        /// 续保统计->战败分布
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="statusIdStr">回访状态编号字符串</param>
        /// <param name="agentId">当前查询代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="roleType">角色</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDefeatAllocationResult(DateTime bizEndDate, string statusIdStr, int agentId, int topAgentId, int roleType = 0)
        {
            var agentIdAndNameTuple = GetSearchAgentIdAndName(false, topAgentId, roleType == 4 ? topAgentId : agentId);
            var categoryNames = GetSearchCategories(topAgentId);
            categoryNames.Add("未分类");
            var defeatAllocationResult = _statisticsService.GetDefeatAllocationResult(bizEndDate, statusIdStr, agentIdAndNameTuple.Item3);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", DefeatAllocationResult = defeatAllocationResult, CategoryNames = categoryNames }.ResponseToJson();

        }
        /// <summary>
        /// 续保统计->战败分布->业务员明细
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="agentId">当前登录代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="searchAgentId">当前搜索代理人编号</param>
        /// <param name="statusIdStr">回访状态编号字符串</param>
        /// <param name="roleType">角色</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDefeatAllocationResultAboutAgent(DateTime bizEndDate, int topAgentId, int agentId, string statusIdStr, int roleType = 0, int searchAgentId = -1, int pageIndex = 1, int pageSize = 10, string customerCategory = null)
        {

            var agentIdAndNameTuple = GetSearchAgentIdAndName(true, topAgentId, roleType == 4 ? topAgentId : agentId, pageIndex, pageSize);
            var defeatAllocationResultAboutAgent = _statisticsService.GetDefeatAllocationResultAboutAgent(bizEndDate, customerCategory, searchAgentId == -1 ? agentIdAndNameTuple.Item3 : new List<int> { searchAgentId }, statusIdStr);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", DefeatAllocationResultAboutAgent = defeatAllocationResultAboutAgent, TotalAgentIdAndName = agentIdAndNameTuple.Item1, PagingAgentIdAndName = searchAgentId == -1 ? agentIdAndNameTuple.Item2 : agentIdAndNameTuple.Item1.Where(x => x.AgentId == searchAgentId).ToList(), TotalCount = agentIdAndNameTuple.Item4 }.ResponseToJson();

        }
        /// <summary>
        /// 工作统计->出单统计
        /// </summary>
        /// <param name="reviewTime">回访时间</param>
        /// <param name="agentId">当前查询代理人编号</param>
        /// <param name="statusIdStr">回访状态编号字符串</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="roleType">角色</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetOutOrderStatisticsResult(DateTime reviewTime, int agentId, string statusIdStr, int topAgentId, int roleType = 0)
        {
            var agentIdAndNameTuple = GetSearchAgentIdAndName(false, topAgentId, roleType == 4 ? topAgentId : agentId);
            var categoryNames = GetSearchCategories(topAgentId);
            categoryNames.Add("未分类");
            List<OutOrderStatisticsVM> outOrderStatisticsResult = new List<OutOrderStatisticsVM>();
            outOrderStatisticsResult = _statisticsService.GetOutOrderStatisticsResult(reviewTime, agentIdAndNameTuple.Item3, statusIdStr);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", OutOrderStatisticsResult = outOrderStatisticsResult, CategoryNames = categoryNames }.ResponseToJson();
        }
        /// <summary>
        /// 工作统计->出单统计->业务员明细
        /// </summary>
        /// <param name="reviewTime">回访时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="agentId">当前d登录代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="searchAgentId">当前搜索代理人编号</param>
        /// <param name="statusIdStr">回访状态编号字符串</param>
        /// <param name="roleType">角色</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetOutOrderStatisticsResultAboutAgent(DateTime reviewTime, int topAgentId, int agentId, string statusIdStr, int roleType = 0, int searchAgentId = -1, int pageIndex = 1, int pageSize = 10, string customerCategory = null)
        {

            var agentIdAndNameTuple = GetSearchAgentIdAndName(true, topAgentId, roleType == 4 ? topAgentId : agentId, pageIndex, pageSize);
            List<OutOrderStatisticsResultAboutAgentVM> outOrderStatisticsResultAboutAgent = null;

            outOrderStatisticsResultAboutAgent = _statisticsService.GetOutOrderStatisticsResultAboutAgent(reviewTime, customerCategory, searchAgentId == -1 ? agentIdAndNameTuple.Item3 : new List<int> { searchAgentId }, statusIdStr);

            return new { BusinessStatus = 1, StatusMessage = "获取成功", OutOrderStatisticsResultAboutAgent = outOrderStatisticsResultAboutAgent, TotalAgentIdAndName = agentIdAndNameTuple.Item1, PagingAgentIdAndName = searchAgentId == -1 ? agentIdAndNameTuple.Item2 : agentIdAndNameTuple.Item1.Where(x => x.AgentId == searchAgentId).ToList(), TotalCount = agentIdAndNameTuple.Item4 }.ResponseToJson();
        }
        /// <summary>
        /// 工作统计->跟进统计
        /// </summary>
        /// <param name="reviewStartTime">回访开始时间</param>
        /// <param name="reviewEndTime">回访结束时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="agentId">当前查询代理人编号</param>
        /// <param name="roleType">角色</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetFollowUpStatisticsResult(DateTime reviewStartTime, DateTime reviewEndTime, int topAgentId, int agentId, int roleType = 0, string customerCategory = null)
        {
            var agentIdAndNameTuple = GetSearchAgentIdAndName(false, topAgentId, roleType == 4 ? topAgentId : agentId);
            var followUpStatisticsResult = _statisticsService.GetFollowUpStatisticsResult(reviewStartTime, reviewEndTime.AddDays(1), customerCategory, agentIdAndNameTuple.Item3);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", FollowUpStatisticsResult = followUpStatisticsResult }.ResponseToJson();

        }
        /// <summary>
        /// 工作统计->跟进统计->业务员明细
        /// </summary>
        /// <param name="reviewStartTime">回访开始时间</param>
        /// <param name="reviewEndTime">回访结束时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="agentId">登录的代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="searchAgentId">当前查询的代理人编号</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <param name="roleType">角色</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetFollowUpStatisticsResultAboutAgent(DateTime reviewStartTime, DateTime reviewEndTime, int topAgentId, int agentId, int roleType = 0, int searchAgentId = -1, int pageIndex = 1, int pageSize = 10, string customerCategory = null)
        {
            var agentIdAndNameTuple = GetSearchAgentIdAndName(true, topAgentId, roleType == 4 ? topAgentId : agentId, pageIndex, pageSize);
            var followUpStatisticsResultAboutAgent = _statisticsService.GetFollowUpStatisticsResultAboutAgent(reviewStartTime, reviewEndTime.AddDays(1), customerCategory, searchAgentId == -1 ? agentIdAndNameTuple.Item3 : new List<int> { searchAgentId });
            return new { BusinessStatus = 1, StatusMessage = "获取成功", FollowUpStatisticsResultAboutAgent = followUpStatisticsResultAboutAgent, TotalAgentIdAndName = agentIdAndNameTuple.Item1, PagingAgentIdAndName = searchAgentId == -1 ? agentIdAndNameTuple.Item2 : agentIdAndNameTuple.Item1.Where(x => x.AgentId == searchAgentId).ToList(), TotalCount = agentIdAndNameTuple.Item4 }.ResponseToJson();

        }
        /// <summary>
        /// 续保统计->跟进分布->单元格数据明细
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="agentId">登录的代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="searchAgentId">当前查询的代理人编号</param>
        /// <param name="categoryName">类别名称</param>
        /// <param name="statusIdStr">回访状态id字符串</param>
        /// <param name="isSingleStatusSearch">是否为单个回访状态查询</param>
        /// <param name="isSingleCategorySearch">是否为单个客户类别查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <param name="roleType">角色</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetFollowUpAllocationResultDetail(DateTime bizEndDate, int topAgentId, int agentId, bool isSingleStatusSearch, bool isSingleCategorySearch, int pageIndex, int pageSize, int roleType = 0, string categoryName = null, int searchAgentId = -1, string statusIdStr = null)
        {
            var followUpAllocationResultDetail = _statisticsService.GetFollowUpAllocationResultDetail(bizEndDate, roleType == 4 ? topAgentId : agentId, searchAgentId, categoryName, statusIdStr, isSingleStatusSearch, isSingleCategorySearch, pageIndex, pageSize);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", FollowUpAllocationResultDetail = followUpAllocationResultDetail }.ResponseToJson();
        }
        /// <summary>
        /// 续保统计->出单或者战败分布->单元格数据明细
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="agentId">登录的代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="searchAgentId">当前查询的代理人编号</param>
        /// <param name="categoryName">类别名称</param>
        /// <param name="statusIdStr">回访状态id字符串</param>
        /// <param name="month">查询月份</param>
        /// <param name="isSingleCategorySearch">是否为单个客户类别查询</param>
        /// <param name="isSingleMonthSearch">是否为单个月查询</param>
        /// <param name="isAboutMonth">是否有关于月</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <param name="roleType">角色</param>
        /// <returns></returns>

        [HttpGet]
        public HttpResponseMessage GetOutOrderOrDefeatAllocationResultDetail(DateTime bizEndDate, int topAgentId, int agentId, bool isSingleCategorySearch, bool isSingleMonthSearch, bool isAboutMonth, int pageIndex, int pageSize, int roleType = 0, string categoryName = null, int searchAgentId = -1, string statusIdStr = null, string month = null)
        {
            var outOrderOrDefeatAllocationResultDetail = _statisticsService.GetOutOrderOrDefeatAllocationResultDetail(bizEndDate, roleType == 4 ? topAgentId : agentId, searchAgentId, categoryName, statusIdStr, month, isSingleCategorySearch, isSingleMonthSearch, isAboutMonth, pageIndex, pageSize);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", OutOrderOrDefeatAllocationResultDetail = outOrderOrDefeatAllocationResultDetail }.ResponseToJson();
        }
        /// <summary>
        /// 工作统计->出单统计->单元格数据明细
        /// </summary>
        /// <param name="reviewTime">回访时间</param>
        /// <param name="agentId">登录的代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="searchAgentId">当前查询的代理人编号</param>
        /// <param name="categoryName">类别名称</param>
        /// <param name="statusIdStr">回访状态id字符串</param>
        /// <param name="month">查询月份</param>
        /// <param name="isSingleCategorySearch">是否为单个客户类别查询</param>
        /// <param name="isSingleMonthSearch">是否为单个月查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <param name="roleType">角色</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetOutOrderStatisticsResultDetail(DateTime reviewTime, int topAgentId, int agentId, string statusIdStr, bool isSingleCategorySearch, bool isSingleMonthSearch, int pageIndex, int pageSize, int roleType = 0, string categoryName = null, int searchAgentId = -1, string month = null)
        {
            var outOrderStatisticsResultDetail = _statisticsService.GetOutOrderStatisticsResultDetail(reviewTime, roleType == 4 ? topAgentId : agentId, searchAgentId, categoryName, statusIdStr, month, isSingleCategorySearch, isSingleMonthSearch, pageIndex, pageSize);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", OutOrderStatisticsResultDetail = outOrderStatisticsResultDetail }.ResponseToJson();
        }


        /// <summary>
        /// 工作统计->跟进统计->有效电话数->单元格数据明细
        /// </summary>
        /// <param name="reviewStartTime">回访开始时间</param>
        /// <param name="reviewEndTime">回访结束时间</param>
        /// <param name="agentId">登录的代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="searchAgentId">当前查询的代理人编号</param>
        /// <param name="month">查询月份</param>
        /// <param name="isSingleMonthSearch">是否为单个月查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <param name="roleType">角色</param>
        /// <param name="categoryName">类别名称</param>
        /// <param name="isSingleCategorySearch">是否为单个类别查询</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAnswerCallTimesResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int topAgentId, int agentId, bool isSingleMonthSearch, bool isSingleCategorySearch, int pageIndex, int pageSize, int roleType = 0, int searchAgentId = -1, string month = null, string categoryName = null)
        {
            var answerCallTimesResultDetail = _statisticsService.GetAnswerCallTimesResultDetail(reviewStartTime, reviewEndTime.AddDays(1), roleType == 4 ? topAgentId : agentId, searchAgentId, month, isSingleMonthSearch, isSingleCategorySearch, categoryName, pageIndex, pageSize);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", AnswerCallTimesResultDetail = answerCallTimesResultDetail }.ResponseToJson();
        }
        /// <summary>
        /// 工作统计->跟进统计-预约数->单元格数据明细
        /// </summary>
        /// <param name="reviewStartTime">回访开始时间</param>
        /// <param name="reviewEndTime">回访结束时间</param>
        /// <param name="agentId">登录的代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="searchAgentId">当前查询的代理人编号</param>
        /// <param name="month">查询月份</param>
        /// <param name="isSingleMonthSearch">是否为单个月查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <param name="roleType">角色</param>
        /// <param name="categoryName">类别名称</param>
        /// <param name="isSingleCategorySearch">是否为单个类别查询</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAppointmentResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int topAgentId, int agentId, bool isSingleMonthSearch, bool isSingleCategorySearch, int pageIndex, int pageSize, int roleType = 0, int searchAgentId = -1, string categoryName = null, string month = null)
        {
            var appointmentResultDetail = _statisticsService.GetAppointmentResultDetail(reviewStartTime, reviewEndTime.AddDays(1), roleType == 4 ? topAgentId : agentId, searchAgentId, month, isSingleMonthSearch, isSingleCategorySearch, categoryName, pageIndex, pageSize);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", AppointmentResultDetail = appointmentResultDetail }.ResponseToJson();
        }
        /// <summary>
        /// 工作统计->跟进统计->战败数或者出单数->单元格明细
        /// </summary>
        /// <param name="reviewStartTime">回访开始时间</param>
        /// <param name="reviewEndTime">回访结束时间</param>
        /// <param name="agentId">登录的代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="searchAgentId">当前查询的代理人编号</param>
        /// <param name="statusIdStr">回访状态id字符串</param>
        /// <param name="month">查询月份</param>
        /// <param name="isSingleMonthSearch">是否为单个月查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <param name="roleType">角色</param>
        /// <param name="categoryName">类别名称</param>
        /// <param name="isSingleCategorySearch">是否为单个类别查询</param>
        /// <returns></returns>

        [HttpGet]
        public HttpResponseMessage GetOutOrderOrDefeatStatisticsResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int topAgentId, int agentId, string statusIdStr, bool isSingleMonthSearch, bool isSingleCategorySearch, int pageIndex, int pageSize, int roleType = 0, int searchAgentId = -1, string month = null, string categoryName = null)
        {
            var outOrderOrDefeatStatisticsResultDetail = _statisticsService.GetOutOrderOrDefeatStatisticsResultDetail(reviewStartTime, reviewEndTime.AddDays(1), roleType == 4 ? topAgentId : agentId, searchAgentId, statusIdStr, month, isSingleMonthSearch, isSingleCategorySearch, categoryName, pageIndex, pageSize);
            return new { BusinessStatus = 1, StatusMessage = "获取成功", OutOrderOrDefeatStatisticsResultDetail = outOrderOrDefeatStatisticsResultDetail }.ResponseToJson();
        }
        #endregion

        #region 深圳人保表单

        #region 进场分析
        /// <summary>
        /// 进场分析_进场时段分析
        /// </summary>
        /// <param name="entryDate">进场日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage AnalysisEntryPeriod(DateTime entryDate, string topAgentIds)
        {

            var result = _statisticsService.AnalysisEntryPeriod(entryDate, topAgentIds);
            return result.ResponseToJson();

        }
        /// <summary>
        /// 进场分析_进场占比分析
        /// </summary>
        /// <param name="entryDate">进场日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage AnalysisEntryProportion(DateTime entryDate, string topAgentIds)
        {
            var result = _statisticsService.AnalysisEntryProportion(entryDate, topAgentIds);
            return result.ResponseToJson();
        }
        /// <summary>
        /// 进场分析_进场跟进分析
        /// </summary>
        /// <param name="entryDate">进场跟进分析</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>

        [HttpGet]
        public HttpResponseMessage AnalysisEntryFollowUp(DateTime entryDate, string topAgentIds)
        {
            var result = _statisticsService.AnalysisEntryFollowUp(entryDate, topAgentIds);
            return result.ResponseToJson();

        }
        #endregion

        #region 报价分析
        /// <summary>
        /// 报价分析_报价次数分析
        /// </summary>
        /// <param name="quoteDate">报价日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage AnalysisQuoteTimes(DateTime quoteDate, string topAgentIds)
        {
            var result = _statisticsService.AnalysisQuoteTimes(quoteDate, topAgentIds);

            return JsonHelper.Serialize(result).StringToHttpResponseMessage();
        }
        /// <summary>
        ///报价分析_ 报价行为分析
        /// </summary>
        /// <param name="quoteDate">报价日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage AnalysisQuoteAction(DateTime quoteDate, string topAgentIds)
        {
            var result = _statisticsService.AnalysisQuoteAction(quoteDate, topAgentIds);
            return result.ResponseToJson();
        }
        #endregion

        #region 投保分析
        /// <summary>
        ///投保分析_ 投保分布分析
        /// </summary>
        /// <param name="insureDate">投保日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage AnalysisInsureDistribution(DateTime insureDate, string topAgentIds)
        {

            var result = _statisticsService.AnalysisInsureDistribution(insureDate, topAgentIds);
            return JsonHelper.Serialize(result).StringToHttpResponseMessage();

        }
        /// <summary>
        /// 投保分析->投保结构分析->商业险均单分析
        /// </summary>
        /// <param name="insureDate">投保日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage AnalysisInsureBizAvg(DateTime insureDate, string topAgentIds)
        {
            var result = _statisticsService.AnalysisInsureBizAvg(insureDate, topAgentIds);
            return result.ResponseToJson();
        }
        /// <summary>
        /// 投保分析->投保结构分析->险别分析
        /// </summary>
        /// <param name="insureDate">投保日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage AnalysisInsureRisk(DateTime insureDate, string topAgentIds)
        {

            var result = _statisticsService.AnalysisInsureRisk(insureDate, topAgentIds);
            return result.ResponseToJson();
        }


        /// <summary>
        /// 投保分析->投保结构分析->提前投保分析
        /// </summary>
        /// <param name="insureDate">投保日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage AnalysisInsureAdvance(DateTime insureDate, string topAgentIds)
        {

            var result = _statisticsService.AnalysisInsureAdvance(insureDate, topAgentIds);
            return result.ResponseToJson();
        }
        #endregion

        #region 流量分析
        /// <summary>
        /// 流向分析_去年人保本年转化分析
        /// </summary>
        /// <param name="insureDate">投保日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage AnalysisFlowDirectionFromRenBao(DateTime insureDate, string topAgentIds)
        {

            var result = _statisticsService.AnalysisFlowDirectionFromRenBao(insureDate, topAgentIds);
            return result.ResponseToJson();
        }
        /// <summary>
        /// 流向分析_本年转入人保上年承保归属
        /// </summary>
        /// <param name="insureDate">投保日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage AnalysisFlowDirectionToRenBao(DateTime insureDate, string topAgentIds)
        {

            var result = _statisticsService.AnalysisFlowDirectionToRenBao(insureDate, topAgentIds);
            return result.ResponseToJson();
        }
        /// <summary>
        /// 流量监控
        /// </summary>
        /// <param name="analysisDate">统计日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage MonitorFlow(DateTime analysisDate, string topAgentIds)
        {

            var result = _statisticsService.MonitorFlow(analysisDate, topAgentIds);
            return result.ResponseToJson();
        }
        #endregion

        #region 深分移动统计
        /// <summary>
        /// 深分统计H5
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAgentData4SfH5([FromUri] SfH5Request request)
        {
            var viewModel = new SfH5ViewModel();

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
                viewModel = _agentService.GetAgentData4SfH5ByPage(request.AgentId, request.StartTime, request.EndTime, request.IsDesc, request.OrderBy, request.PageIndex, request.PageSize, request.SerachText);
                viewModel.BusinessStatus = 1;
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }

        [HttpGet]
        public HttpResponseMessage GetAverageData([FromUri] SingleAgentDataRequest request)
        {
            var viewModel = new SfH5AverageViewModel();

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
                viewModel = _agentService.GetAverageData(request.AgentId, request.StartTime, request.EndTime);
                viewModel.BusinessStatus = 1;
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 深分移动统计车商报表总览
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="agentId">支公司编号</param>
        /// <param name="categoryName">客户类别</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCarDealerReport(DateTime startTime, DateTime endTime, int agentId, string categoryName = null)
        {
            var result = _statisticsService.GetCarDealerReport(startTime, endTime.AddDays(1), agentId, categoryName);
            return result.ResponseToJson();
        }
        /// <summary>
        /// 深分移动统计车商报表列表
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="agentId">支公司编号</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="searchText">搜索文本</param>
        /// <param name="categoryName">客户类别</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCarDealerReportDetails(DateTime startTime, DateTime endTime, int agentId, int pageIndex = 1, int pageSize = 10, string searchText = null, string categoryName = null)
        {
            var result = _statisticsService.GetCarDealerReportDetailsByPage(startTime, endTime.AddDays(1), agentId, pageIndex, pageSize, searchText, categoryName);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 深分移动统计进店详情
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="categoryName">客户类别</param>
        /// <param name="isViewAllData">是否查看全部数据</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetEntryDetails(DateTime startTime, DateTime endTime, int topAgentId, int isViewAllData = 0, int pageIndex = 1, int pageSize = 10, string categoryName = null)
        {
            var result = _statisticsService.GetEntryDetailsByPage(startTime, endTime.AddDays(1), topAgentId, isViewAllData, pageIndex, pageSize, categoryName);
            return result.ResponseToJson();
        }

        [HttpGet]
        public HttpResponseMessage GetDailyWork(DateTime startTime, DateTime endTime, int agentId, int pageIndex = 1, int pageSize = 10, string searchText = null, string categoryName = null)
        {
            var result = _statisticsService.GetDailyWorkByPage(startTime, endTime.AddDays(1), agentId, pageIndex, pageSize, searchText, categoryName);
            return result.ResponseToJson();
        }
        #endregion

        #region 集团统计->数据监测

        /// <summary>
        /// 集团统计->进店数据
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="agentName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetEntryDataByPage(int groupId, DateTime startTime, DateTime endTime, string agentName = "", int pageIndex = 1, int pageSize = 10)
        {
            var viewModel = new BaseViewModel() { BusinessStatus = 200, StatusMessage = "获取成功" };
            try
            {
                viewModel.Data = _statisticsService.GetEntryDataByPage(groupId, startTime, endTime, pageIndex, pageSize, agentName);
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "获取失败";
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 客户分析(集团)
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="dueDate"></param>
        /// <param name="agentName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCustomerAnalysisByGroup(int groupId, DateTime dueDate, string agentName = "", int pageIndex = 1, int pageSize = 10)
        {
            var viewModel = new BaseViewModel() { BusinessStatus = 200, StatusMessage = "获取成功" };
            try
            {
                viewModel.Data = _statisticsService.CustomerAnalysisByGroup(groupId, dueDate, agentName, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "获取失败";
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }

        #endregion

        #endregion

        /// <summary>
        /// 保费统计
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="agentId">代理人Id</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage PremiumStatistics(DateTime startTime, DateTime endTime, int agentId)
        {
            return _statisticsService.PremiumStatistics(startTime, endTime.AddDays(1), agentId).ResponseToJson();
        }

        /// <summary>
        /// 日报
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="agentId">代理人Id</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage DailyPaper(DateTime startTime, DateTime endTime, int agentId)
        {
            return _statisticsService.GetDailyPaper(startTime, endTime.AddDays(1), agentId).ResponseToJson();
        }

        /// <summary>
        /// 摄像头进店数据总览
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="roleType"></param>
        /// <param name="topAgentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="viewType">返回数据类型(1:总览,2:明细-用于导出)</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetEntryOverView(int agentId, int roleType, int topAgentId, DateTime startTime, DateTime endTime, int viewType = 1)
        {
            var viewModel = new BaseViewModel() { BusinessStatus = 200, StatusMessage = "获取成功" };
            try
            {
                viewModel.Data = _statisticsService.GetEntryOverView(roleType == 4 ? topAgentId : agentId, startTime, endTime, viewType);
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "获取失败";
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 客户分析
        /// </summary>
        /// <param name="agentId">代理人ID</param>
        /// <param name="topAgentId">顶级代理人ID</param>
        /// <param name="roleType">角色</param>
        /// <param name="dueDate">保险到期时间</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCustomerAnalysis(int agentId, int topAgentId, int roleType, DateTime dueDate)
        {
            var viewModel = new BaseViewModel() { BusinessStatus = 200, StatusMessage = "获取成功" };
            try
            {
                var result = _statisticsService.CustomerAnalysisOverView(roleType == 4 ? topAgentId : agentId, topAgentId, dueDate);
                viewModel.Data = new { DataList = result.Item1, DataUpdateTime = result.Item2 };
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "获取失败";
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 客户分析(分页)
        /// </summary>
        /// <param name="agentId">代理人ID</param>
        /// <param name="topAgentId">顶级代理人ID</param>
        /// <param name="roleType">角色</param>
        /// <param name="dueDate">保险到期时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCustomerAnalysisByPage(int agentId, int topAgentId, int roleType, DateTime dueDate, int pageIndex = 1, int pageSize = 10)
        {
            var viewModel = new BaseViewModel() { BusinessStatus = 200, StatusMessage = "获取成功" };
            try
            {
                viewModel.Data = _statisticsService.CustomerAnalysisByPage(roleType == 4 ? topAgentId : agentId, topAgentId, dueDate, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "获取失败";
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 战败分析
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <param name="roleType"></param>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDefeatAnalysis(int agentId, int topAgentId, int roleType, DateTime dueDate)
        {
            var viewModel = new BaseViewModel() { BusinessStatus = 200, StatusMessage = "获取成功" };
            try
            {
                viewModel.Data = _statisticsService.DefeatAnalysis(roleType == 4 ? topAgentId : agentId, topAgentId, dueDate);
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "获取失败";
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }

        private Tuple<List<AgentIdAndAgentName>, List<AgentIdAndAgentName>, List<int>, int> GetSearchAgentIdAndName(bool isPaging, int topAgentId, int agentId, int pageIndex = 1, int pageSize = 10)
        {
            var sonAgentsViewModel = _agentService.GetSonAgents(topAgentId, agentId);

            var data = new Tuple<List<AgentIdAndAgentName>, List<AgentIdAndAgentName>, List<int>, int>(sonAgentsViewModel.ListAgent, isPaging ? sonAgentsViewModel.ListAgent.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList() : null, isPaging ? sonAgentsViewModel.AgentIds.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList() : sonAgentsViewModel.AgentIds, sonAgentsViewModel.ListAgent.Count);
            return data;
        }
        private List<string> GetSearchCategories(int agentId)
        {
            return _customerCategories.GetCustomerCategories(agentId).Select(x => x.CategoryInfo).ToList();
        }
    }
}
