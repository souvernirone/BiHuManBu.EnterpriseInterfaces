using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using ServiceStack.Text;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using AppInterfaces = BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using log4net;
using System;
using BiHuManBu.ExternalInterfaces.Services.Mapper.AppMapper;
using System.Net;
using Newtonsoft.Json;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Services.Implements;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 单独APP封装接口，订单、支付相关的除外
    /// </summary>
    //废弃，此处的校验为老版的校验//[AuthorizeFilter]
    public class BihuAppController : ApiController
    {
        private readonly IRenewalInfoService _renewalInfoService;
        private readonly AppInterfaces.IAppAchieveService _appAchieveService;
        private readonly ILog _logAppInfo;
        private readonly ILog _logInfo;
        private readonly ILog _logError;
        private readonly IAppoinmentService _appoinmentService;
        private readonly IVerifyService _verifyService;
        private readonly IAgentUKeyService _agentUKeyService;
        private readonly IAppVerifyService _appVerifyService;
        private readonly IStatisticsService _statisticsService;
        readonly ICustomerCategories _customerCategoriesService;
        public BihuAppController(AppInterfaces.IAppAchieveService appAchieveService, IAppoinmentService appoinmentService, IVerifyService verifyService
            , IAgentUKeyService agentUKeyService, IRenewalInfoService renewalInfoService, IAppVerifyService appVerifyService, IStatisticsService statisticsService, ICustomerCategories customerCategoriesService)
        {
            _customerCategoriesService = customerCategoriesService;
            _appAchieveService = appAchieveService;
            _appoinmentService = appoinmentService;
            _renewalInfoService = renewalInfoService;
            _verifyService = verifyService;
            _logAppInfo = LogManager.GetLogger("APP");
            _logInfo = LogManager.GetLogger("INFO");
            _logError = LogManager.GetLogger("ERROR");
            _agentUKeyService = agentUKeyService;
            _appVerifyService = appVerifyService;
            _statisticsService = statisticsService;
        }
        #region 续保、报价、核保 相关
        /// <summary>
        /// 请求续保 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetReInfo([FromUri]GetReInfoRequest request)
        {
            _logAppInfo.Info(string.Format("请求续保接口请求串：{0}", Request.RequestUri));
            var viewModel = new GetReInfoNewViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = await _appAchieveService.GetReInfo(request, Request.GetQueryNameValuePairs());
            //_logAppInfo.Info(string.Format("请求续保接口返回值：{0}", viewModel.ToJson()));
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 请求报价/核保 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> PostPrecisePrice([FromUri]PostPrecisePriceRequest request)
        {
            _logAppInfo.Info(string.Format("请求报价/核保接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = await _appAchieveService.PostPrecisePrice(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 重新请求核保 光鹏洁 2018-01-04 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> PostSubmitInfo([FromUri]PostSubmitInfoRequest request)
        {
            _logAppInfo.Info(string.Format("重新请求核保接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = await _appAchieveService.PostSubmitInfo(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取报价信息 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetSpecialPrecisePrice([FromUri]GetPrecisePriceRequest request)
        {
            _logAppInfo.Info(string.Format("获取报价信息接口请求串：{0}", Request.RequestUri));
            var viewModel = new GetPrecisePriceNewViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            //此处调用未带special的方法，是因为该service返回值已经增加了buid
            viewModel = await _appAchieveService.GetPrecisePrice(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取核保信息 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetSubmitInfo([FromUri]GetSubmitInfoRequest request)
        {
            _logAppInfo.Info(string.Format("获取核保信息接口请求串：{0}", Request.RequestUri));
            var viewModel = new SubmitInfoNewViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = await _appAchieveService.GetSubmitInfo(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取车辆出险信息 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetCreditInfo([FromUri]GetEscapedInfoRequest request)
        {
            _logAppInfo.Info(string.Format("获取车辆出险信息接口请求串：{0}", Request.RequestUri));
            var viewModel = new GetCreaditInfoViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = await _appAchieveService.GetCreditInfo(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取车辆出险信息 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetCreditDetailInfo([FromUri]GetEscapedInfoRequest request)
        {
            _logAppInfo.Info(string.Format("获取车辆出险信息接口请求串：{0}", Request.RequestUri));
            var viewModel = new GetCreaditDetailInfoViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = await _appAchieveService.GetCreditDetailInfo(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取车型信息 光鹏洁 2017-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// 即将废弃，但是需要兼容老版本的APP2.10以前
        [HttpGet]
        public async Task<HttpResponseMessage> GetVehicleInfo([FromUri]GetCarVehicleRequest request)
        {
            _logAppInfo.Info(string.Format("获取车型信息接口请求串：{0}", Request.RequestUri));
            var viewModel = new CarVehicleInfoNewViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = await _appAchieveService.GetVehicleInfo(request, Request.GetQueryNameValuePairs());
            //_logAppInfo.Info(string.Format("获取车辆出险信息接口返回值：{0}", viewModel.ToJson()));
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取车型信息新 光鹏洁 2017-12-21 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetNewVehicleInfo([FromUri]GetVehicleRequest request)
        {
            _logAppInfo.Info(string.Format("获取车型信息接口请求串：{0}", Request.RequestUri));
            var viewModel = new NewCarVehicleInfoViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = await _appAchieveService.GetNewVehicleInfo(request, Request.GetQueryNameValuePairs());
            //_logAppInfo.Info(string.Format("获取车辆出险信息接口返回值：{0}", viewModel.ToJson()));
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 车型报价校验 光鹏洁 2017-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> CheckVehicle([FromUri]GetNewCarSecondVehicleRequest request)
        {
            _logAppInfo.Info(string.Format("车型报价校验接口请求串：{0}", Request.RequestUri));
            var viewModel = new CheckCarVehicleInfoViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = await _appAchieveService.CheckVehicle(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 检查是否可以重复报价 光鹏洁 2017-08-11 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage IsHaveLicenseno([FromBody]AppRequest.AppIsHaveLicensenoRequest request)
        {
            _logAppInfo.Info("判断是否在一个顶级下面有其他人算过请求的车牌:" + Request.RequestUri + "\n参数为：" + request.ToJson());
            var viewModel = new IsHaveLicensenoViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.IsHaveLicenseno(request, Request.GetQueryNameValuePairs());
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }
        #endregion
        #region 报价、续保的列表、详情
        /// <summary>
        /// 获取报价单、续保列表 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetMyList([FromUri]GetMyListRequest request)
        {
            _logAppInfo.Info(string.Format("请求报价续保列表接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.MyAppListViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.GetMyList(request, Request.GetQueryNameValuePairs());
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "查询失败";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 报价单详情 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetPrecisePriceDetail([FromUri]GetMyBjdDetailRequest request)
        {
            _logAppInfo.Info(string.Format("获取报价详情接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.MyBaoJiaViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.GetPrecisePriceDetail(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 续保详情 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetReInfoDetail([FromUri]GetReInfoDetailRequest request)
        {
            _logAppInfo.Info(string.Format("获取续保详情接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.GetReInfoNewViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.GetReInfoDetail(request, Request.GetQueryNameValuePairs(), Request.RequestUri);
            return viewModel.ResponseToJson();
        }
        #region 关系人
        /// <summary>
        /// 获取临时关系人信息 潘自强 2017-?-? /APP
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetTempRelation([FromUri]AppRequest.GetTempRelationRequest request)
        {
            _logAppInfo.Info(string.Format("获取临时关系人信息，接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.TempRelationViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();

            }
            try
            {
                viewModel = _appAchieveService.GetTempRelation(request, Request.GetQueryNameValuePairs());
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 保存临时关系信息 潘自强 2017-?-? /APP
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveTempRelation([FromBody]AppRequest.SaveTempRelationRequest request)
        {
            _logAppInfo.Info(string.Format("保存临时关系信息，接口请求串：{0}；请求参数：{1}", Request.RequestUri, JsonConvert.SerializeObject(request)));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.SaveTempRelation(request, Request.GetQueryNameValuePairs());
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }
        #endregion
        /// <summary>
        /// 获取客户信息 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetConsumerInfo([FromUri]GetConsumerInfoRequest request)
        {
            _logAppInfo.Info(string.Format("请求报价续保列表接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.GetConsumerInfoViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = await _appAchieveService.GetConsumerInfo(request, Request.GetQueryNameValuePairs());
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "查询失败";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取跟进列表 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCrmStepsList([FromUri]GetCrmStepsListRequest request)
        {
            _logAppInfo.Info(string.Format("获取跟进列表请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.GetCrmStepsListViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.GetCrmStepsList(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        #region 回访记录
        /// <summary>
        /// 添加回访记录 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddReVisited([FromBody]AppRequest.AddReVisitedRequest request)
        {
            _logAppInfo.Info(string.Format("添加回访记录接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.AddReVisited(request, Request.GetQueryNameValuePairs());
            //_logAppInfo.Info(string.Format("添加回访记录列表接口返回值：{0}", viewModel.ToJson()));
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 回访记录列表 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage ReVisitedList([FromUri]AppRequest.ReVisitedListRequest request)
        {
            _logAppInfo.Info(string.Format("获取回访记录列表接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.ReVisitedListViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.ReVisitedList(request, Request.GetQueryNameValuePairs());
            //_logAppInfo.Info(string.Format("获取回访记录列表接口返回值：{0}", viewModel.ToJson()));
            return viewModel.ResponseToJson();
        }
        #endregion
        /// <summary>
        /// 分配 陈亮 2017-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostDistribute([FromBody]PostDistributeRequest request)
        {
            _logAppInfo.Info(string.Format("获取可分配的代理人，接口请求串：{0}；请求参数：{1}", Request.RequestUri, JsonConvert.SerializeObject(request)));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = await _appAchieveService.PostDistributeAsync(request, Request.GetQueryNameValuePairs());
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 删除客户数据接口 陈亮 2017-9-14 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public HttpResponseMessage DeleteCustomer([FromBody]DeleteCustomerRequest request)
        {
            var viewModel = _appAchieveService.DeleteCustomer(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        #endregion
        #region 分享报价单
        /// <summary>
        /// 生成报价单 光鹏洁 2017-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SharePrecisePrice([FromUri]CreateOrUpdateBjdInfoRequest request)
        {
            _logAppInfo.Info(string.Format("分享报价单接口请求串：{0}", Request.RequestUri));
            var viewModel = new SharePrecisePriceViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.SharePrecisePrice(request, Request.GetQueryNameValuePairs());
            //_logAppInfo.Info(string.Format("分享报价单接口返回值：{0}", viewModel.ToJson()));
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 查看被分享的报价单 光鹏洁 2017-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetShare([FromUri]GetBjdItemRequest request)
        {
            _logAppInfo.Info(string.Format("获取分享接口请求串：{0}", Request.RequestUri));
            var viewModel = new BaojiaItemViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.GetShare(request, Request.GetQueryNameValuePairs());
            //_logAppInfo.Info(string.Format("获取分享接口返回值：{0}", viewModel.ToJson()));
            return viewModel.ResponseToJson();
        }
        #endregion
        #region 系统基础信息
        /// <summary>
        /// 获取顶级代理的渠道资源 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAgentSource([FromUri]AppBaseRequest request)
        {
            _logAppInfo.Info(string.Format("获取代理渠道列表接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.AppAgentSourceViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.GetAgentSource(request, Request.GetQueryNameValuePairs(), Request.RequestUri);
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取代理人的城市报价有效天数 光鹏洁 2017-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAgentDredgeCityByApp([FromUri]AppBaseRequest request)
        {
            _logAppInfo.Info(string.Format(" 获取代理人的城市报价有效天数：{0}", Request.RequestUri));
            var viewModel = new AgentDredgeCityRequest();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.GetAgentDredgeCityByApp(request, Request.GetQueryNameValuePairs());
            //_logAppInfo.Info(string.Format("获取代理渠道列表接口返回值：{0}", viewModel.ToJson()));
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 比较版本号 光鹏洁 2017-?-? /APP
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage CompareVersion([FromUri]RequestCompareConfig request)
        {
            _logAppInfo.Info(string.Format("获取版本号对比接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.BxConfigViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.CompareVersion(request);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 修改APP升级是否验证版本号 李金友 2017-10-31 /PC
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage EditIsuploadByKey([FromUri]RequestKeyConfig request)
        {
            _logAppInfo.Info(string.Format("修改APP版本升级验证请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.BxConfigViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.EditIsuploadByKey(request);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 修改版本号 李金友 2017-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage EditVersion([FromUri]RequestEditConfig request)
        {
            _logAppInfo.Info(string.Format("获取修改版本号接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.EditBxConfigViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.EditVersion(request);
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取可分配的代理人 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSettedAgents([FromUri]GetSettedAgentsRequest request)
        {
            _logAppInfo.Info(string.Format("获取可分配的代理人，接口请求串：{0}；请求参数：{1}", Request.RequestUri, JsonConvert.SerializeObject(request)));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.GetSettedAgents(request, Request.GetQueryNameValuePairs());
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }
        #endregion
        #region 预约单相关接口
        #region Order接口转移
        /// <summary>
        /// 查询订单列表 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public HttpResponseMessage GetOrders([FromUri]AppRequest.GetOrdersRequest request)
        {
            _logAppInfo.Info("获取订单列表接口请求" + Request.RequestUri);
            var viewModel = new AppViewModels.OrdersViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                int totalCount = 0;
                //获取所有大于0的订单，正常已下单的
                var carOrders = _appAchieveService.GetOrders_TranApp(request, request.OrderType, Request.GetQueryNameValuePairs(), out totalCount).ConvertToViewModel();
                foreach (var item in carOrders)
                {
                    item.CreateTime_Str = item.create_time.HasValue ? item.create_time.Value.ToString("yyyy-MM-dd HH:mm") : "";
                    item.OrderId = item.id;
                }
                if (carOrders.Count == 0)
                {
                    viewModel.BusinessStatus = -1;
                    viewModel.StatusMessage = "无订单信息";
                }
                else
                {
                    viewModel.CarOrders = CommonHelper.TRNull(carOrders);
                    viewModel.TotalCount = totalCount;
                    viewModel.BusinessStatus = 1;
                }
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "查询订单列表失败";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 创建订单接口-微信创建订单第一步用 修改返回ViewModel 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> NewCreateOrder([FromBody]AppRequest.CreateOrderRequest request)
        {
            _logAppInfo.Info("创建订单请求" + request.ToJson());
            var viewModel = new AppViewModels.CreateOrderViewMode();
            //Dictionary<String, Object> map = new Dictionary<string, object>();
            //Type t = request.GetType();
            //PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //foreach (PropertyInfo p in pi)
            //{
            //    MethodInfo mi = p.GetGetMethod();
            //    if (mi != null && mi.IsPublic)
            //    {
            //        map.Add(p.Name, mi.Invoke(request, new Object[] { }));
            //    }
            //}
            //_logInfo.Info("返回的map：" + map.ToJson());
            //return viewModel.ResponseToJson();
            //if (!ModelState.IsValid)
            //{
            //    viewModel.BusinessStatus = -10000;
            //    string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            //    viewModel.StatusMessage = "输入参数错误，" + msg;
            //    return viewModel;
            //}
            //viewModel = await _appAchieveService.NewCreateOrder_Tran(request, Request.GetQueryNameValuePairs());
            //_logAppInfo.Info("创建订单返回值" + viewModel.ResponseToJson());
            //if (viewModel.OrderId > 0)
            //{
            //    viewModel.BusinessStatus = 1;
            //    viewModel.OrderId = viewModel.OrderId;
            //}
            //else
            //{
            //    viewModel.BusinessStatus = -10002;
            //    viewModel.StatusMessage = "创建订单失败";
            //}
            //return viewModel;
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = await _appAchieveService.NewCreateOrder_Tran(request, Request.GetQueryNameValuePairs(), Request.RequestUri);
            _logAppInfo.Info("创建订单返回值" + viewModel.ResponseToJson());
            if (viewModel.Status == HttpStatusCode.BadRequest || viewModel.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                return viewModel.ResponseToJson();
            }
            if (viewModel.Status == HttpStatusCode.ExpectationFailed)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "创建订单失败";
                return viewModel.ResponseToJson();
            }
            if (viewModel.OrderId > 0)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "创建订单成功";
            }
            else
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "创建订单失败";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 订单更新接口-微信创建订单第二步用 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UpdateOrder([FromBody]AppRequest.ModifyOrderRequest request)
        {
            request.IsUpdatePayStatus = false;//防止普通更改更改订单状态
            _logAppInfo.Info("更新订单请求" + request.ToJson());
            var viewModel = new AppViewModels.CreateOrderViewMode();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.UpdateOrder(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取已出保单 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetOutOrders([FromUri] AppRequest.GetOrdersRequest request)
        {
            _logAppInfo.Info("获取已出订单列表接口请求" + Request.RequestUri);
            var viewModel = new AppViewModels.OrdersViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                int totalCount = 0;
                //获取所有=-3的订单，正常已收单的
                var carOrder = _appAchieveService.GetOrders_Tran(request, -3, Request.GetQueryNameValuePairs(), out totalCount).ConvertToViewModel();
                if (carOrder.Count == 0)
                {
                    viewModel.BusinessStatus = -1;
                    viewModel.StatusMessage = "无订单信息";
                }
                else
                {
                    viewModel.CarOrders = carOrder;
                    viewModel.TotalCount = totalCount;
                    viewModel.BusinessStatus = 1;
                }
            }
            catch (Exception)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "查询订单列表失败";
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 查询订单详情 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public HttpResponseMessage GetOrder([FromUri]AppRequest.GetOrderRequest request)
        {
            _logAppInfo.Info("获取订单详情请求" + Request.RequestUri);
            var viewModel = new AppViewModels.OrderViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                var carOrder = _appAchieveService.FindCarOrderBy_Tran(request, Request.GetQueryNameValuePairs()).ConvertToViewModel();
                _logAppInfo.Info("获取订单详情返回值" + carOrder.ToJson());
                if (carOrder != null)
                {
                    viewModel.CarOrder = carOrder;
                    viewModel.BusinessStatus = 1;
                }
                else
                {
                    viewModel.BusinessStatus = -1;
                    viewModel.StatusMessage = "无此订单信息";
                }
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "查询订单列表失败";
            }
            return viewModel.ResponseToJson();
        }
        #endregion
        #region 地址 2018年9月13号后微信端小V盟共用此接口
        /// <summary>
        /// 新增收单地址,返回id 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="bxAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddAddress([FromBody]AppRequest.AddressRequest bxAddress)
        {
            _logAppInfo.Info(string.Format("新增收单地址接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.CreateAddressViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.AddAddress(bxAddress, Request.GetQueryNameValuePairs());
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "创建地址失败";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 按ID查询收单地址 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage FindAddress([FromUri]AppRequest.FindOrDeleteAddressRequest request)
        {
            _logAppInfo.Info(string.Format("查询地址集合接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.AddressViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.FindAddress(request, Request.GetQueryNameValuePairs());
            }
            catch (Exception)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "查询地址失败";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 删除收单地址 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage DeleteAddress([FromUri]AppRequest.FindOrDeleteAddressRequest request)
        {
            _logAppInfo.Info(string.Format("删除地址集合接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.DeleteAddress(request, Request.GetQueryNameValuePairs());
            }
            catch (Exception)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "删除失败";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 更新收单地址 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="bxAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UpdateAddress([FromBody]AppRequest.AddressRequest bxAddress)
        {
            _logAppInfo.Info(string.Format("更新地址集合接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.UpdateAddress(bxAddress, Request.GetQueryNameValuePairs());
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "更新地址失败";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 按openid和agentid获取收单地址列表 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAddress([FromUri]AppRequest.GetAddressRequest request)
        {
            _logAppInfo.Info(string.Format("获取地址集合接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.AddressesViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.FindByopenidAndAgentId(request, Request.GetQueryNameValuePairs());
                foreach (var item in viewModel.Addresses)
                {
                    item.CreateTime_Str = item.createtime.HasValue ? item.createtime.Value.ToString("yyyyy-MM-dd HH:mm") : "";
                }
                viewModel.BusinessStatus = 1;
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "查询地址失败";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 更新默认地址 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="findOrDeleteAddressRequest"></param>
        /// <returns></returns>
        public HttpResponseMessage SetDefalutAddress([FromBody]AppRequest.FindOrDeleteAddressRequest findOrDeleteAddressRequest)
        {
            _logAppInfo.Info(string.Format("获取地址集合接口请求串：{0}", Request.RequestUri));
            var baseViewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                baseViewModel.StatusMessage = "输入参数错误，" + msg;
                return baseViewModel.ResponseToJson();
            }
            try
            {
                baseViewModel = _appAchieveService.SetDefaultAddress(findOrDeleteAddressRequest, Request.GetQueryNameValuePairs());
                baseViewModel.BusinessStatus = 1;
            }
            catch
            {
                baseViewModel.BusinessStatus = -10002;
                baseViewModel.StatusMessage = "更新失败";
            }
            return baseViewModel.ResponseToJson();
        }
        #endregion
        /// <summary>
        /// 获取预约单，已出保单，报价单 数量 陈龙 2017-04-? /APP
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetCounts([FromUri]GetCountsRequest getCountsRequest)
        {
            _logAppInfo.Info(string.Format("获取预约单，保单，报价单数量接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.GetCountsViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.GetCounts(getCountsRequest, Request.GetQueryNameValuePairs());
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功";
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "查询失败";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取地区信息 陈龙 2017-04-? /APP
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetAreaInfoes([FromUri]GetAreaInfoesReqeust request)
        {
            _logAppInfo.Info(string.Format("获取省市县，接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.AreaInfoViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.GetAreaInfoes(request, Request.GetQueryNameValuePairs());
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功";
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "查询失败";
            }
            return viewModel.ResponseToJson();
        }
        #endregion
        #region 登陆相关
        /// <summary>
        /// 保存代理人和信鸽账户关系 陈龙 2017-04-? /APP
        /// </summary>
        [HttpPost]
        public HttpResponseMessage SaveAgent_XGAccount_RelationShip([FromBody]AddAgentRelationWithXgAccount request)
        {
            _logAppInfo.Info(string.Format("保存代理人和信鸽推送应用关系，接口请求串：{0}；请求参数：{1}", Request.RequestUri, JsonConvert.SerializeObject(request)));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.SaveAgent_XGAccount_RelationShip(request, Request.GetQueryNameValuePairs());
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 检查当前代理人是否已注册信鸽账号 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage CheckXgAccount([FromUri]CheckXgAccountRequest request)
        {
            _logAppInfo.Info(string.Format("检查当前代理人是否已注册信鸽账号，接口请求串：{0}；请求参数：{1}", Request.RequestUri, JsonConvert.SerializeObject(request)));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.CheckXgAccount(request, Request.GetQueryNameValuePairs());
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }
        #endregion
        #region 消息相关
        /// <summary>
        /// 更新消息状态 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public HttpResponseMessage UpdateMessageStatus([FromBody]UpdateMessageStatusRequest request)
        {
            _logAppInfo.Info(string.Format("更新消息状态，接口请求串：{0}；请求参数：{1}", Request.RequestUri, JsonConvert.SerializeObject(request)));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.UpdateMessageStatus(request, Request.GetQueryNameValuePairs());
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        ///删除消息 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DeleteMessage([FromBody]UpdateMessageStatusRequest request)
        {
            _logAppInfo.Info(string.Format("更新消息状态，接口请求串：{0}；请求参数：{1}", Request.RequestUri, JsonConvert.SerializeObject(request)));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.DeleteMessage(request, Request.GetQueryNameValuePairs());
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取消息历史 陈龙、光鹏洁 2017-?-? /APP
        /// </summary>
        [HttpGet]
        public HttpResponseMessage GetMessageHistory([FromUri]GetMessageHistoryRequest request)
        {
            _logAppInfo.Info(string.Format("获取消息历史，接口请求串：{0}；请求参数：{1}", Request.RequestUri, JsonConvert.SerializeObject(request)));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.GetMessageHistory(request, Request.GetQueryNameValuePairs());
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }
        #endregion
        #region 短信相关
        /// <summary>
        /// 获取可用短信条数 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetUsableSmsCount([FromUri]GetUsableSmsCountRequest request)
        {
            _logAppInfo.Info(string.Format("获取可分配的代理人，接口请求串：{0}；请求参数：{1}", Request.RequestUri, JsonConvert.SerializeObject(request)));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                viewModel = _appAchieveService.GetUsableSmsCount(request, Request.GetQueryNameValuePairs());
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// app短信发送 陈龙 2017-04-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage PostSendSms([FromBody]PostSendSmsRequest request)
        {
            _logAppInfo.Info(string.Format("发送短信接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            if (request.Buid < 1)
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "输入参数错误";
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.PostSendSms(request, Request.GetQueryNameValuePairs(), Request.RequestUri);
            return viewModel.ResponseToJson();
        }
        #endregion
        #region 代理人相关
        /// <summary>
        /// 修改代理账号信息 赵开永 2017-08-06 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditAgentInfo([FromBody]EditAgentRequest request)
        {
            _logAppInfo.Info(string.Format("修改代理人密码接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.AppBaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                if (!string.IsNullOrEmpty(request.AgentPwd) && (request.AgentPwd.Length < 8 || request.AgentPwd.Length > 20))
                {
                    viewModel.BusinessStatus = -10000;
                    viewModel.StatusMessage = "输入参数错误, 字段 AgentPwd 必须是一个字符串，其最小长度为 8，最大长度为 20";
                    return viewModel.ResponseToJson();
                }

                viewModel = _appAchieveService.EditAgentInfo(request, Request.GetQueryNameValuePairs());
            }
            catch
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "系统异常";
            }
            return viewModel.ResponseToJson();
        }
        #endregion
        #region 客户状态接口
        /// <summary>
        /// 获取客户状态信息 -潘自强 2017-12-07 /APP
        /// </summary>
        /// <param name="agentId">顶级代理人编号</param>
        /// <param name="t_Id">前端原编号</param>
        /// <param name="isDeleteData">是否查询出带删除的数据false：不查 true：查</param>
        /// <param name="isGetReView">是否获取回复数据</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCustomerStatus(int agentId = 0, int t_Id = -1, bool isDeleteData = false, bool isGetReView = true)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                var customerStatusInfo = _appAchieveService.GetCustomerStatus(agentId, t_Id, isDeleteData, isGetReView);
                _logAppInfo.Info(string.Format("获取客户状态信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), agentId, t_Id, isDeleteData));
                var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, customerStatusInfo = customerStatusInfo };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logAppInfo.Error(string.Format("获取客户状态信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }

        #endregion
        #region 已出保单列表
        /// <summary>
        /// 已出保单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetQuotationReceiptListData([FromBody]QuotationReceiptRequest request)
        {
            _logInfo.InfoFormat("获取已出保单请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson());
            try
            {
                if (!ModelState.IsValid)
                {
                    string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    return new
                    {
                        BusinessStatus = -10000,
                        StatusMessage = "输入参数错误，" + msg
                    }.ResponseToJson();
                }
                //安全校验
                var baseResponse = _verifyService.Verify(request.SecCode, Request.GetQueryNameValuePairs());
                if (baseResponse.ErrCode != 1)
                {//校验失败，返回错误码
                    return new
                    {
                        BusinessStatus = baseResponse.ErrCode,
                        StatusMessage = baseResponse.ErrMsg
                    }.ResponseToJson();
                }
                int totalCount;

                return _appoinmentService.GetQuotationReceiptListData(request);
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return new HttpResponseMessage();
        }
        #endregion

        #region 录入回访
        public HttpResponseMessage SaveConsumerReview([FromBody] AddConsumerReviewRequest request)
        {


            _logAppInfo.Info("录入回访" + Request.RequestUri + "\n参数为：" + request.ToJson());
            var viewModel = new BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.SaveConsumerReview(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();

        }
        #endregion
        #region 获取战败原因
        [HttpGet]
        public HttpResponseMessage GetDefeatReasonSetting([FromUri]AppBaseRequest request)
        {
            var viewModel = new BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            viewModel = _appAchieveService.GetDefeatReasonSetting(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();


        }
        #endregion

        #region 报价渠道相关

        /// <summary>
        /// 获取报价渠道列表 李金友 2018-05-23 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetListChannel([FromUri]AppAgentUKeyRequest request)
        {
            return _agentUKeyService.GetListChannel(request, Request.GetQueryNameValuePairs()).ResponseToJson();
        }

        /// <summary>
        /// 更改报价渠道是否可用状态 李金友 2018-05-23 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateChannelIsUsed([FromUri]AgentUKeyRequest request)
        {
            return _agentUKeyService.UpdateChannelIsUsed(request, Request.GetQueryNameValuePairs()).ResponseToJson();
        }

        /// <summary>
        /// 同步密码 李金友 2018-05-23 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditAgentUKey([FromBody]EditAgentUKeyRequest request)
        {
            return _agentUKeyService.EditAgentUKey(request, Request.GetQueryNameValuePairs()).ResponseToJson();
        }

        #endregion
        #region 保存客户信息
        /// <summary>
        /// 保存客户信息
        /// </summary>
        /// <param name="saveCustomerInfoViewModel">CarInfo</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> SaveCustomerInfo([FromBody]BiHuManBu.ExternalInterfaces.Models.ViewModels.SaveCustomerInfoViewModel saveCustomerInfoViewModel)
        {
            var customerInfo = new CustomerInfo()
            {   /// <summary>

                BuId = saveCustomerInfoViewModel.BuId,

                CustomerName = saveCustomerInfoViewModel.CustomerName,

                CustomerMobile = saveCustomerInfoViewModel.CustomerMobile,

                CustomerType = saveCustomerInfoViewModel.CustomerType,
                CityName = saveCustomerInfoViewModel.CityName,
                ClientMobileOther = saveCustomerInfoViewModel.ClientMobileOther,
                Remark = saveCustomerInfoViewModel.Remark,
                ClientAddress = saveCustomerInfoViewModel.ClientAddress,
                IntentionRemark = saveCustomerInfoViewModel.IntentionRemark,
                TagId = saveCustomerInfoViewModel.TagId
            };
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "保存失败" };
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            }
            else
            {
                try
                {
                    var isSuccess = await _renewalInfoService.SaveCustomerInfoAsync(customerInfo);
                    if (isSuccess)
                    {
                        baseViewModel.BusinessStatus = 1;
                        baseViewModel.StatusMessage = "保存成功";
                    }
                    _logInfo.Info(string.Format(baseViewModel.StatusMessage + "请求url为：{0}，请求数据为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(saveCustomerInfoViewModel)));
                }
                catch (Exception ex)
                {
                    baseViewModel.BusinessStatus = -10003;
                    baseViewModel.StatusMessage = "服务器异常";
                    _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));

                }

            }
            return baseViewModel.ResponseToJson();
        }
        #endregion
        #region app标签相关
        /// <summary>
        /// 获取该代理人所有标签  刘松年 2018-06-15 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAgentTagOfAll([FromUri]AppBaseRequest request)
        {
            var viewModel = new BaseViewModel();
            viewModel = _appAchieveService.GetAgentTag(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取代理人指定车牌标签  刘松年 2018-06-19 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAgentTagForCustomer([FromUri]AppBaseRequest request)
        {
            var viewModel = new BaseViewModel();
            viewModel = _appAchieveService.GetAgentTagForCustomer(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 为指定代理人增加标签  刘松年 2018-06-19 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage AddTagForCustomer([FromUri]AddTagRequest request)
        {
            var viewModel = new BaseViewModel();
            viewModel = _appAchieveService.AddTagForCustomer(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 删除代理人标签  刘松年 2018-06-19 /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage DelAgentTag([FromUri]DelTagRequest request)
        {
            var viewModel = new BaseViewModel();
            viewModel = _appAchieveService.DelAgentTag(request, Request.GetQueryNameValuePairs());
            return viewModel.ResponseToJson();
        }
        #endregion

        #region 进店及客户统计

        /// <summary>
        /// 摄像头进店数据总览
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public Models.ViewModels.BaseViewModel GetEntryOverView([FromBody]EntryOverViewRequest request)
        {
            var viewModel = new Models.ViewModels.BaseViewModel() { BusinessStatus = 200, StatusMessage = "获取成功" };

            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, request.ToValuePairs());
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion

            try
            {
                if (request.TimeType > 0)
                {
                    switch (request.TimeType)
                    {
                        case 1:
                            request.StartTime = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                            request.EndTime = request.StartTime;
                            break;
                        case 2:
                            request.StartTime = Convert.ToDateTime(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"));
                            request.EndTime = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                            break;
                        case 3:
                            request.StartTime = Convert.ToDateTime(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"));
                            request.EndTime = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                            break;
                    }
                }
                else if (!request.StartTime.HasValue || !request.EndTime.HasValue)
                {
                    viewModel.BusinessStatus = -10003;
                    viewModel.StatusMessage = "参数有误";
                    return viewModel;
                }
                viewModel.Data = _statisticsService.GetEntryOverView(request.RoleType == 4 ? request.Agent : request.ChildAgent, request.StartTime.Value, request.EndTime.Value, 1);
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "获取失败";
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel;
        }

        /// <summary>
        /// 客户分析
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public Models.ViewModels.BaseViewModel GetCustomerAnalysis([FromBody]CustomerAnalysisRequest request)
        {
            var viewModel = new Models.ViewModels.BaseViewModel() { BusinessStatus = 200, StatusMessage = "获取成功" };

            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, request.ToValuePairs());
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion

            try
            {
                viewModel.Data = _statisticsService.CustomerAnalysisOverView(request.RoleType == 4 ? request.Agent : request.ChildAgent, request.Agent, request.DueDate);
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "获取失败";
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel;
        }

        #endregion

        #region 顶级查询详情
        /// <summary>
        /// app不允许重复投保时，顶级也可以进入详情并重新报价
        /// add by qdk 2018-12-11
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetQuotationDetailModel([FromBody]QuotationDetailRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                return new
                {
                    BusinessStatus = -10000,
                    StatusMessage = "输入参数错误，" + msg
                }.ResponseToJson();
            }
            return _appAchieveService.GetQuotationDetailModel(request).ResponseToJson();
        }
        #endregion
        /// <summary>
        /// 获取客户类别信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCustomerCategories(int agentId = 0)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                var customerCategoriesInfo = _customerCategoriesService.GetCustomerCategories(agentId);
                _logInfo.Info(string.Format("获取客户类别信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), agentId));
                var IsStart = false;
                if (customerCategoriesInfo.Any())
                {
                    var ls = customerCategoriesInfo.Where(x => x.IsStart == 1).ToList();
                    //如果相等
                    if (customerCategoriesInfo.Count == ls.Count)
                    {
                        IsStart = true;
                    }
                }
                else
                {
                    IsStart = true;
                }
                var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, customerCategoriesInfo = customerCategoriesInfo, IsStart = IsStart };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("获取客户类别信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }
    }
}
