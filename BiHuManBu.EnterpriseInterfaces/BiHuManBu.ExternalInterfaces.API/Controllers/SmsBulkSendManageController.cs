using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class SmsBulkSendManageController : ApiController
    {
        readonly ISmsBulkSendManageService _smsBulkSendManageService;
        readonly ILog _logInfo = LogManager.GetLogger("INFO");
        readonly ILog _logError = LogManager.GetLogger("ERROR");
        readonly ISmsUtilService _smsUtilService;
        readonly string txtPath = "/Content/BadWords/黑词160901.txt";

        public SmsBulkSendManageController(
            ISmsBulkSendManageService _smsBulkSendManageService
            , ISmsUtilService _smsUtilService
            )
        {
            this._smsBulkSendManageService = _smsBulkSendManageService;
            this._smsUtilService = _smsUtilService;
        }
        /// <summary>
        /// 添加批量短信记录
        /// </summary>
        /// <param name="request">添加批量短信记录请求模型</param>
        /// <returns></returns>

        [HttpPost]
        public async Task<HttpResponseMessage> AddSmsBulkSendRecord([FromBody]  AddSmsBulkSendRecordRequest request)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "保存成功" };
            var strUrl = string.Format("AgentId={0}&AgentName={1}&SmsContent={2}&SendTime={3}&TopAgentId={4}&CustKey={5}", request.AgentId, request.AgentName, request.SmsContent, request.SendTime.ToString("yyyy-MM-dd HH:mm:ss"), request.TopAgentId, request.CustKey);
            if (!string.Equals( strUrl.GetUrl().ToMd5(2), request.SecCode,StringComparison.CurrentCultureIgnoreCase) ){
                baseViewModel.BusinessStatus = -10004;
                baseViewModel.StatusMessage = "校验失败";
                return baseViewModel.ResponseToJson();
            }
            //ToDo:添加事物
  
            try
            {
                var checkResult = await CheckSmsCountAndContent(request.SmsContent, request.MobileList.Distinct().Count(), request.AgentId, request.TopAgentId,1);
                if (checkResult.BusinessStatus == 0)
                {
                    baseViewModel = checkResult;
                    return baseViewModel.ResponseToJson();
                }
                int id = 0;
                var smsResultModel = _smsBulkSendManageService.AddSmsBulkSendRecord(request, out id);
                if (id <= 0)
                {
                    baseViewModel.BusinessStatus = 0;
                    baseViewModel.StatusMessage = "保存失败";
                }
                else
                {
                    if (smsResultModel.ResultCode != -1)
                    {
                        if (smsResultModel.ResultCode != 0)
                        {
                            baseViewModel.BusinessStatus = smsResultModel.ResultCode != -114 ? -10008 : -10006;
                            baseViewModel.StatusMessage = smsResultModel.ResultCode != -114 ? "账户状态异常，请联系客服人员！" : "短信余额不足，请充值后重试！";
                        }
                        else
                        {
                            baseViewModel.StatusMessage = "发送成功";
                        }
                    }
                }



                _logInfo.Info(string.Format("保存批量短信记录url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(request)));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("保存批量短信记录发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 撤销发送
        /// </summary>
        /// <param name="id">批次号</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage CancelSend(int id)
        {
            //ToDo:增加事物
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "撤销成功" };
            try
            {
                var result = _smsBulkSendManageService.CancelSend(id);
                if (!result)
                {
                    baseViewModel.BusinessStatus = 0;
                    baseViewModel.StatusMessage = "撤销失败";
                }

                _logInfo.Info(string.Format("撤销批量短信记录url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), id));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("撤销批量短信记录发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 删除发送
        /// </summary>
        /// <param name="id">批次号</param>
        /// <returns></returns>
        [HttpGet]

        public HttpResponseMessage DeleteSmsBulkSendRecord(int id)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "删除成功" };
            try
            {
                var result = _smsBulkSendManageService.DeleteSmsBulkSendRecord(id);
                if (!result)
                {
                    baseViewModel.BusinessStatus = 0;
                    baseViewModel.StatusMessage = "删除失败";
                }

                _logInfo.Info(string.Format("删除批量短信记录url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), id));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("删除批量短信记录发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取批量发送记录
        /// </summary>
        /// <param name="request">获取批量发送记录模型</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSmsBulkSendRecord([FromUri]GetSmsBulkSendRecordRequest request)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            int totalCount = 0;
            try
            {
                var result = _smsBulkSendManageService.GetSmsBulkSendRecord(request, out totalCount);
                _logInfo.Info(string.Format("查询批量短信记录url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(request)));
                return new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, SmsBulkSendRecord = result, TotalCount = totalCount }.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("查询批量短信记录发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }

        }
        /// <summary>
        /// 获取目标用户
        /// </summary>
        /// <param name="request">获取目标用户模型</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetTargetUsersMobile([FromUri] GetTargetUsersRequest request)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            int totalCount = 0;
            try
            {
                var result = _smsBulkSendManageService.GetTargetUsersMobile(request, out totalCount);
                _logInfo.Info(string.Format("查询目标用户手机号url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(request)));
                return new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, TargetUsersMobile = result, TotalCount = totalCount }.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("查询目标用户手机号发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }
        /// <summary>
        /// 更新批量发送记录
        /// </summary>
        /// <param name="request">批次号</param>
        /// <returns></returns>
        [HttpPost]
     
        public async Task<HttpResponseMessage> UpdateSmsBulkSendRecord([FromBody]UpdateSmsBulkSendRecordRequest request)
        {
            //ToDo:增加事物
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "更改成功" };


            var strUrl = string.Format("Id={0}&SmsContent={1}&SendTime={2}&AgentId={3}&TopAgentId={4}&AgentName={5}&CustKey={6}", request.Id, request.SmsContent, request.SendTime.ToString("yyyy-MM-dd HH:mm:ss"), request.AgentId, request.TopAgentId,request.AgentName, request.CustKey);
            if (!string.Equals( strUrl.GetUrl().ToMd5(2),  request.SecCode,StringComparison.CurrentCultureIgnoreCase))
            {
                baseViewModel.BusinessStatus = -10004;
                baseViewModel.StatusMessage = "校验失败";
                return baseViewModel.ResponseToJson();
            }
            try
            {
                var oldBulkSend = _smsBulkSendManageService.GetBulkSendRecordById(request.Id);
                var checkResult = await CheckSmsCountAndContent(request.SmsContent, request.MobileList.Distinct().Count(), request.AgentId, request.TopAgentId,oldBulkSend.Status,oldBulkSend.CustomerCount,oldBulkSend.Content);
                if (checkResult.BusinessStatus == 0)
                {
                    baseViewModel = checkResult;
                    return baseViewModel.ResponseToJson();
                }
                var result = _smsBulkSendManageService.UpdateSmsBulkSendRecord(request, oldBulkSend);
                if (result.ResultCode != -1 && result.ResultCode != 1)
                {
                    if (result.ResultCode != 0)
                    {
                        baseViewModel.BusinessStatus = result.ResultCode != -114 ? -10008 : -10006;
                        baseViewModel.StatusMessage = result.ResultCode != -114 ? "账户状态异常，请联系客服人员！" : "短信余额不足，请充值后重试！";
                    }
                    else
                    {
                        baseViewModel.StatusMessage = "发送成功";
                    }
                }
                else if(result.ResultCode == -1)
                {
                    baseViewModel.BusinessStatus = 0;
                    baseViewModel.StatusMessage = "更新失败";
                }
                _logInfo.Info(string.Format("更改批量短信发送时间url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(request)));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("更改批量短信发送时间发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取发送电话号和发送内容
        /// </summary>
        /// <param name="id">批次编号</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSendMobilesAndContentById(int id)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "获取成功" };
            try
            {
                var result = _smsBulkSendManageService.GetSendMobilesAndContentById(id);


                _logInfo.Info(string.Format("获取发送电话号和发送内容url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), id));
                return new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, SendMobilesAndContent = result }.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("获取发送电话号和发送内容发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }

        }
        /// <summary>
        /// 服务发送接口
        /// </summary>
        /// <param name="request">服务请求参数模型</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage BulkSend(List<SendRequest> request)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "批量发送" };
            try
            {
                _smsBulkSendManageService.BulkSend(request);


                _logInfo.Info(string.Format("服务批量发送短信url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(request)));

            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("获取发送电话号和发送内容发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));

            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 获得可用数量
        /// </summary>
        /// <param name="agentId">当前代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAvailCount(int agentId, int topAgentId)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            int availCount = 0;
            try
            {
                availCount = _smsBulkSendManageService.GetAvailCount(agentId, topAgentId);


                _logInfo.Info(string.Format("查询短信可用数量url为：{0}；请求参数为：agentId:{1}，topAgentId:{2}", Request.RequestUri.ToString(), agentId, topAgentId));
                return new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, AvailCount = availCount }.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("查询短信可用数量发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }

        }

        private async Task<BaseViewModel> CheckSmsCountAndContent(string smsContent, int count, int agentId, int topAgentId,int status,int oldCount=-1,string oldSmsContent="")
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "成功" };
            if (string.IsNullOrWhiteSpace(smsContent))
            {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = "短信内容为空";
                return baseViewModel;
            }
            var smsCount = ApplicationSettingsFactory.GetApplicationSettings().BatchSmsCount;
            if (ApplicationSettingsFactory.GetApplicationSettings().BatchSmsSign.Length+ ApplicationSettingsFactory.GetApplicationSettings().BatchSmsTail.Length+smsContent.Length > smsCount) {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = string.Format("短信内容超过{0}字", smsCount);
                return baseViewModel;
            }
            var availCount = _smsBulkSendManageService.GetAvailCount(agentId, topAgentId);
            var newCount = status == 1 || status == 2 ? count * _smsBulkSendManageService.SDmessageCount(smsContent) : count * _smsBulkSendManageService.SDmessageCount(smsContent) - oldCount * _smsBulkSendManageService.SDmessageCount(oldSmsContent);
            if (newCount > availCount)
            {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = "发送失败！短信剩余量不足！";
                return baseViewModel;
            }

            var badWordsArray = await _smsUtilService.GetBadWordsCache();
            if (badWordsArray == default(string[]))
            {
                badWordsArray = await _smsUtilService.InitBadWords(System.Web.HttpContext.Current.Server.MapPath(txtPath));
            }
            var badWordsList = _smsUtilService.BadWordsFilter(badWordsArray, smsContent);
            if (badWordsList.Any())
            {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = string.Format("您得编辑结果存在非法内容：{0}；暂不能发送，请及时修正。", string.Join(",", badWordsList));
                return baseViewModel;
            }
            else
            {
                return baseViewModel;
            }
        }
    }
}
