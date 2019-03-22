using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using AppInterfaces = BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Mapper.AppMapper;
using log4net;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class BjdController : ApiController
    {
        private AppInterfaces.IBjdService _bjdService;
        private ILog logError;
        private ILog logInfo;
        private ILog _logAppInfo;
        private readonly IBatchRenewalService _batchRenewalService;


        public BjdController(AppInterfaces.IBjdService bjdService, IBatchRenewalService batchRenewalService)
        {
            _bjdService = bjdService;
            logError = LogManager.GetLogger("ERROR");
            logInfo = LogManager.GetLogger("INFO");
            _logAppInfo = LogManager.GetLogger("APP");
            _batchRenewalService = batchRenewalService;
        }
        /// <summary>
        /// 查看我的续保详情 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetReInfoDetail([FromUri]AppRequest.GetReInfoDetailRequest request)
        {
            _logAppInfo.Info(string.Format("获取续保详情接口请求串：{0}", Request.RequestUri));
            var model = new AppViewModels.AppReInfoViewModel();
            if (!ModelState.IsValid)
            {
                model.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                model.StatusMessage = "输入参数错误，" + msg;
                return model.ResponseToJson();
            }
            var response = _bjdService.GetReInfoDetail(request, Request.GetQueryNameValuePairs());
            //_logAppInfo.Info(string.Format("获取续保详情接口返回结果：{0}", response.ToJson()));
            if (response.Status == HttpStatusCode.BadRequest || response.Status == HttpStatusCode.Forbidden)
            {
                model.BusinessStatus = -10001;
                model.StatusMessage = "参数校验错误，请检查您的校验码";
                return model.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.ExpectationFailed)
            {
                model.BusinessStatus = -10003;
                model.StatusMessage = "服务发生异常";
                return model.ResponseToJson();
            }

            if (response.Status == HttpStatusCode.OK)
            {
                model.CustKey = response.UserInfo.OpenId;
                model.BusinessStatus = response.BusinessStatus == 8 ? 1 : response.BusinessStatus;
                model.StatusMessage = response.BusinessMessage;
                model.UserInfo = response.UserInfo.ConvertToViewModel(response.SaveQuote, response.CarInfo,
                    response.LastInfo, response.AutoMoldCode,1);
                model.SaveQuote = response.SaveQuote.ConvetToViewModel();
                if (model.SaveQuote != null)
                {
                    model.SaveQuote.Source = SourceGroupAlgorithm.GetNewSource((int)model.SaveQuote.Source);
                }
                //判断修改到期时间
                var renewalItem = _batchRenewalService.GetItemByBuId(long.Parse(model.UserInfo.Buid));
                if (renewalItem != null && (renewalItem.LastYearSource != -1 || renewalItem.ForceEndDate.HasValue || renewalItem.BizEndDate.HasValue))
                {
                    if (model.BusinessStatus == 1)
                    {

                        if (!string.IsNullOrEmpty(model.UserInfo.BusinessExpireDate) || renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd") != "1900-01-01")
                        {

                            if (renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd") != "1900-01-01" && (DateTime.Parse(model.UserInfo.BusinessExpireDate) < renewalItem.BizEndDate || string.IsNullOrEmpty(model.UserInfo.BusinessExpireDate)))
                            {
                                model.UserInfo.ModleName = renewalItem.MoldName;
                                model.SaveQuote.Source = renewalItem.LastYearSource == 0 ? 2 : renewalItem.LastYearSource > 1 && renewalItem.LastYearSource != 9999 ? Convert.ToInt32(Math.Pow(2, renewalItem.LastYearSource)) : renewalItem.LastYearSource;
                                model.UserInfo.BusinessExpireDate = renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                                model.UserInfo.ForceExpireDate = renewalItem.ForceEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(renewalItem.MoldName))
                        {
                            model.UserInfo.ModleName = renewalItem.MoldName;
                        }
                        model.SaveQuote.Source = renewalItem.LastYearSource == 0 ? 2 : renewalItem.LastYearSource > 1 && renewalItem.LastYearSource != 9999 ? Convert.ToInt32(Math.Pow(2, renewalItem.LastYearSource)) : renewalItem.LastYearSource;
                        model.UserInfo.BusinessExpireDate = renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd") == "1900-01-01" ? "" : renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        model.UserInfo.ForceExpireDate = renewalItem.ForceEndDate.Value.ToString("yyyy-MM-dd") == "1900-01-01" ? "" : renewalItem.ForceEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                //返回到期时间
                if (model.UserInfo != null) {
                    int dayminus = 0;
                    if (!string.IsNullOrEmpty(model.UserInfo.ForceExpireDate))
                    {
                        dayminus = TimeHelper.GetDayMinus(DateTime.Parse(model.UserInfo.ForceExpireDate), DateTime.Now);
                    }
                    else if (!string.IsNullOrEmpty(model.UserInfo.BusinessExpireDate))
                    {
                        dayminus = TimeHelper.GetDayMinus(DateTime.Parse(model.UserInfo.BusinessExpireDate), DateTime.Now);
                    }
                    model.UserInfo.ExpireDateNum = dayminus.ToString();
                }
                model.WorkOrder = response.WorkOrder.ConverToViewModel();
                if (model.WorkOrder != null)
                {
                    if (model.WorkOrder.IntentionCompany.HasValue)
                        model.WorkOrder.IntentionCompany = SourceGroupAlgorithm.GetNewSource((int)model.WorkOrder.IntentionCompany.Value);
                }
                model.DetailList = response.DetailList.ConverToViewModel();
                //model.DetailList里的意向公司有问题
                model.IsDistrib = response.IsDistrib;
                model.Buid = response.Buid;
                model.Agent = response.Agent;
                model.AgentName = response.AgentName;
                model.SaAgent = response.SaAgent;
                model.SaAgentName = response.SaAgentName;
                model.CreateTime = response.CreateTime;

                //当获取续保信息失败（包括状态是3），将去年source制为-1，以免与 平安的0值混淆
                if (response.BusinessStatus == 1)
                {
                    return model.ResponseToJson();
                }
                if (model.SaveQuote != null)
                {
                    model.SaveQuote.Source = -1;
                }
            }
            else
            {
                model.BusinessStatus = -10002;
                model.StatusMessage = "没有续保信息";
            }
            return model.ResponseToJson();
        }
    }
}
