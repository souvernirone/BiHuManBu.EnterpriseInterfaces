using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class DefeatReasonSettingController : ApiController
    {
        readonly IDefeatReasonSettingService _defeatReasonSettingService;
        readonly IAgentService _agentService;
         readonly ILog _logInfo = LogManager.GetLogger("INFO");
         readonly ILog _logError = LogManager.GetLogger("ERROR");
        readonly int maxDefeatReasonConetentLength = Convert.ToInt32(ConfigurationManager.AppSettings["maxDefeatReasonConetentLength"]);
        readonly  int maxDefeatReasonCount= Convert.ToInt32(ConfigurationManager.AppSettings["maxDefeatReasonCount"]);
        public DefeatReasonSettingController(IDefeatReasonSettingService defeatReasonSettingService, IAgentService agentService)
        {
            _defeatReasonSettingService = defeatReasonSettingService;
            _agentService = agentService;
        }
        /// <summary>
        /// 获取战败原因列表
        /// </summary>
        /// <param name="agentId">代理人编号</param>
        /// <returns></returns>
        public HttpResponseMessage GetDefeatReasonSetting(int agentId)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            dynamic defeatReasonSettingList = null;
            try
            {
                //var topAgentId = int.Parse(_agentService.GetTopAgentId(agentId));
                defeatReasonSettingList= _defeatReasonSettingService.GetDefeatReasonSetting(agentId);
                _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), agentId));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "查询失败";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }

            return new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, DefeatReasonSettingList = defeatReasonSettingList }.ResponseToJson();
         
        }
        /// <summary>
        /// 编辑战败原因
        /// </summary>
        /// <param name="defeatReasonSettingViewModel">编辑战败原因模型</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditDefeatReason(DefeatReasonSettingViewModel defeatReasonSettingViewModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "更新失败" };
        
          
            if (CheckDefeatReasonLengthIsTooLong(defeatReasonSettingViewModel.DefeatReason))
            {
                baseViewModel.StatusMessage = "单个战败原因字数不能超过10";
            }

        
            string message = string.Empty;
            try {
                var defeatReasonSettingList = _defeatReasonSettingService.GetDefeatReasonSetting(defeatReasonSettingViewModel.AgentId);
                if (defeatReasonSettingList.Any(x => x.DefeatReason == defeatReasonSettingViewModel.DefeatReason)) {
                    baseViewModel.StatusMessage = "更新失败，战败原因重复";
                    return baseViewModel.ResponseToJson();
                }
                defeatReasonSettingViewModel.IsChange = true;
                message = _defeatReasonSettingService.EditDefeatReason(defeatReasonSettingViewModel);
                if (message == "更新成功")
                {
                    baseViewModel.BusinessStatus = 1;
                   
                }
                else {
                    baseViewModel.BusinessStatus = 0;
                
                }
                baseViewModel.StatusMessage = message;
                _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(defeatReasonSettingViewModel)));

            } catch (Exception ex) {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "系统异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));

            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 添加战败原因
        /// </summary>
        /// <param name="defeatReasonSettingViewModel">添加战败原因模型</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddDefeatReason(DefeatReasonSettingViewModel defeatReasonSettingViewModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "添加失败" };
            if (CheckDefeatReasonLengthIsTooLong(defeatReasonSettingViewModel.DefeatReason)) {
                baseViewModel.StatusMessage = "单个战败原因字数不能超过10";
            }
        
            var defeatReasonSettingList = _defeatReasonSettingService.GetDefeatReasonSetting(defeatReasonSettingViewModel.AgentId);

            if (defeatReasonSettingList.Count() >= maxDefeatReasonCount) {
                baseViewModel.StatusMessage = "添加失败，战败原因上限（"+ maxDefeatReasonCount + "）";//ToDo:基于配置
                return baseViewModel.ResponseToJson();
            }
            if (defeatReasonSettingList.Any(x => x.DefeatReason == defeatReasonSettingViewModel.DefeatReason)) {
                baseViewModel.StatusMessage = "添加失败，战败原因重复";
                return baseViewModel.ResponseToJson();
            }
            dynamic result = null;
       
            try
            {
                defeatReasonSettingViewModel.IsChange = true;
                result = _defeatReasonSettingService.AddDefeatReason(defeatReasonSettingViewModel);
                if (result.IsSuccess)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "添加成功";
                }
                _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(defeatReasonSettingViewModel)));

            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "系统异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));

            }
            return new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, Id = result.Id }.ResponseToJson();
        }

        /// <summary>
        /// 删除当前战败原因
        /// </summary>
        /// <param name="defeatReasonId">战败原因编号</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage DeleteDefeatReason(int defeatReasonId)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "删除失败" };
            var message = string.Empty;
            try
            {
                message = _defeatReasonSettingService.DeleteDefeatReason(defeatReasonId);
   
                if (message=="删除成功")
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "删除成功";
                }
                else {
                    baseViewModel.BusinessStatus = 0;
                    baseViewModel.StatusMessage =message;
                }
                _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), defeatReasonId));

            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "系统异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));

            }
            return baseViewModel.ResponseToJson();
         
        }

        private bool CheckDefeatReasonLengthIsTooLong(string defeatReason)
        {
            var isTooLong=false;
            if (defeatReason.Length > maxDefeatReasonConetentLength) {//ToDo:基于配置
                isTooLong = true;
            }
            return isTooLong;
        }

    }
}
