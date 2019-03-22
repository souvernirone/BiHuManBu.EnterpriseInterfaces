using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 事故车推修设置项
    /// </summary>
    public class AccidentSettingController : ApiController
    {
        private readonly IAccidentSettingService _accidentSettingService;
        private readonly IVerifyService _verifyService;
        public AccidentSettingController(IAccidentSettingService accidentSettingService, IVerifyService verifyService)
        {
            this._accidentSettingService = accidentSettingService;
            this._verifyService = verifyService;
        }

        /// <summary>
        /// 添加/修改超时提醒设置
        /// </summary>
        /// <param name="overNoticeSettingRequest"></param>
        /// <returns></returns>
        [HttpPost, OperationLog("添加/修改超时提醒设置")]
        public HttpResponseMessage AddOrUpdateOverNoticeSetting([FromBody]OverNoticeSettingRequest overNoticeSettingRequest)
        {
            return _accidentSettingService.AddOrUpdateOverNoticeSetting(overNoticeSettingRequest).ResponseToJson();
        }

        /// <summary>
        /// 获取超时提醒设置
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetOverNoticeSetting(int agentId, int roleType)
        {
            var overNoticeSetting = _accidentSettingService.GetOverNoticeSetting(agentId, roleType);
            if (overNoticeSetting == null)
            {
                return "-1".ResponseToJson();
            }
            return overNoticeSetting.ResponseToJson();
        }

        /// <summary>
        /// 获取接收线索分配规则设置
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetClueDistributeRuleSetting(int agentId)
        {
            var clueDistributeRuleSetting = _accidentSettingService.GetClueDistributeRuleSetting(agentId);
            if (clueDistributeRuleSetting == null)
            {
                return "-1".ResponseToJson();
            }
            return clueDistributeRuleSetting.ResponseToJson();
        }

        /// <summary>
        /// 添加/修改接收线索分配规则设置
        /// </summary>
        /// <param name="distributeRuleSettingRequest"></param>
        /// <returns></returns>
        [HttpPost, OperationLog("添加/修改接收线索分配规则设置")]
        public HttpResponseMessage AddOrUpdateClueDistributeRuleSetting([FromBody]DistributeRuleSettingRequest distributeRuleSettingRequest)
        {
            return _accidentSettingService.AddOrUpdateClueDistributeRuleSetting(distributeRuleSettingRequest.TopAgentId, distributeRuleSettingRequest.DistributeType).ResponseToJson();
        }

        /// <summary>
        /// 获取接车人员分配规则设置
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetReceivesDistributeRuleSetting(int agentId)
        {
            var receivesDistributeRuleSetting = _accidentSettingService.GetReceivesDistributeRuleSetting(agentId);
            if (receivesDistributeRuleSetting == null)
            {
                return "-1".ResponseToJson();
            }
            return receivesDistributeRuleSetting.ResponseToJson();
        }

        /// <summary>
        /// 添加/修改接车人员分配规则设置
        /// </summary>
        /// <param name="distributeRuleSettingRequest"></param>
        /// <returns></returns>
        [HttpPost, OperationLog("添加/修改接车人员分配规则设置")]
        public HttpResponseMessage AddOrUpdateReceivesDistributeRuleSetting([FromBody]DistributeRuleSettingRequest distributeRuleSettingRequest)
        {
            return _accidentSettingService.AddOrUpdateReceivesDistributeRuleSetting(distributeRuleSettingRequest.TopAgentId, distributeRuleSettingRequest.DistributeType).ResponseToJson();
        }

        /// <summary>
        /// 分页获取代理人下所有短信模板
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSmsTemplate(int agentId, int pageIndex = 1, int pageSize = 10)
        {
            return _accidentSettingService.GetSmsTemplate(agentId, pageIndex, pageSize).ResponseToJson();
        }

        /// <summary>
        /// 获取单个短信模板
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSingleSmsTemplate(int agentId, int templateId)
        {
            var template = _accidentSettingService.GetSingleSmsTemplate(agentId, templateId);
            if (template == null)
            {
                return "-1".ResponseToJson();
            }
            return template.ResponseToJson();
        }

        /// <summary>
        /// 添加/修改短信模板
        /// </summary>
        /// <param name="smsTemplateRequest"></param>
        /// <returns></returns>
        [HttpPost, OperationLog("添加/修改短信模板")]
        public HttpResponseMessage AddOrUpdateSmsTemplate([FromBody]SmsTemplateRequest smsTemplateRequest)
        {
            return _accidentSettingService.AddOrUpdateSmsTemplate(smsTemplateRequest).ResponseToJson();
        }

        /// <summary>
        /// 删除短信模板
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DeleteSmsTemplate([FromBody]int templateId)
        {
            return _accidentSettingService.DeleteSmsTemplate(templateId).ResponseToJson();
        }

        /// <summary>
        /// 获取事故车手机号设置
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetPhoneSetting(int agentId)
        {
            return _accidentSettingService.GetPhoneSetting(agentId).ResponseToJson();
        }

        /// <summary>
        /// 根据串号获取绑定的手机号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetPhoneSettingByIMEI([FromUri]GetPhoneSettingRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new { Data = new List<tx_mobile_agent_relationship>(), Code = -1, Message = statusMessage }.ResponseToJson();
            }
            //安全校验
            var str = Request.RequestUri.ToString().Split('?')[0] + "?IMEI=" + request.IMEI;
            if (str.GetMd5() != request.SecCode)
            {
                return new { Data = new List<tx_mobile_agent_relationship>(), Code = -1, Message = "参数校验失败" }.ResponseToJson();
            }
            var result = _accidentSettingService.GetPhoneSettingByIMEI(request.IMEI);
            return new { Data = result, Code = 1, Message = "获取成功" }.ResponseToJson();
        }

        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage BindPhoneWithIMEI(AccidentBindPhoneRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new { Data = false, Code = -1, Message = statusMessage }.ResponseToJson();
            }
            //安全校验
            var str = Request.RequestUri.ToString() + "?Code=" + request.Code + "&IMEI=" + request.IMEI + "&Mobile=" + request.Mobile;
            if (str.GetMd5() != request.SecCode)
            {
                return new { Data = false, Code = -1, Message = "参数校验失败" }.ResponseToJson();
            }
            var keyValue = System.Web.HttpRuntime.Cache.Get(request.Mobile);
            if (keyValue != null && !string.IsNullOrEmpty(keyValue.ToString()) && Convert.ToInt32(keyValue) == request.Code)
            {
                var result = _accidentSettingService.BindPhoneWithIMEI(request.IMEI, request.Mobile);
                if (result == 1)
                {
                    return new { Data = true, Code = 1, Message = "绑定成功" }.ResponseToJson();
                }
                else if (result == 0)
                {
                    return new { Data = false, Code = -1, Message = "未找到该手机号" }.ResponseToJson();
                }
                else
                {
                    return new { Data = false, Code = -1, Message = "该手机号已绑定设备" }.ResponseToJson();
                }
            }
            else
            {
                if (keyValue != null && string.IsNullOrEmpty(keyValue.ToString()))
                {
                    return new { Data = false, Code = -1, Message = "验证码过期" }.ResponseToJson();
                }
                else
                {
                    return new { Data = false, Code = -1, Message = "验证码错误" }.ResponseToJson();
                }
            }
        }

        /// <summary>
        /// APP解绑手机
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage APPUnbindPhone(UnbindPhoneRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new { Data = false, Code = -1, Message = statusMessage }.ResponseToJson();
            }
            //安全校验
            var str = Request.RequestUri.ToString() + "?PhoneId=" + request.PhoneId;
            if (str.GetMd5() != request.SecCode)
            {
                return new { Data = false, Code = -1, Message = "参数校验失败" }.ResponseToJson();
            }
            var result = _accidentSettingService.UnbindPhone(request.PhoneId, false);
            if (result == -1)
            {
                return new { Data = false, Code = -1, Message = "手机号不存在" }.ResponseToJson();
            }
            return new { Data = true, Code = 1, Message = "解绑成功" }.ResponseToJson();
        }

        /// <summary>
        /// 解绑手机号
        /// </summary>
        /// <param name="phoneId"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UnbindPhone([FromBody]int phoneId)
        {
            return _accidentSettingService.UnbindPhone(phoneId, true).ResponseToJson();
        }

        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="phoneSettingRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddPhone(PhoneSettingRequest phoneSettingRequest)
        {
            return _accidentSettingService.AddPhone(phoneSettingRequest.AgentId, phoneSettingRequest.PhoneNumber).ResponseToJson();
        }

        /// <summary>
        /// 是否可以绑定(手机号已存在或绑定数量达到上限不可绑定)
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage IsCanBind(int agentId,string phoneNumber)
        {
            BaseViewModel viewModel = new BaseViewModel();
            int isCanBind = _accidentSettingService.IsCanBind(agentId, phoneNumber);
            if (isCanBind == -1)
            {
                viewModel.Data = -1;
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "绑定数量已达上限";
                return viewModel.ResponseToJson();
            }
            if (isCanBind == 0)
            {
                viewModel.Data = -1;
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "该手机号已存在";
                return viewModel.ResponseToJson();
            }
            viewModel.Data = 1;
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "可以绑定";
            return viewModel.ResponseToJson();
        }
    }
}
