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
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class TempInsuredInfoController : ApiController
    {
        readonly ILog logError;
        readonly ILog logInfo;
        readonly ITempInsuredService _tempInsuredService;
        public TempInsuredInfoController(ITempInsuredService tempInsuredService)
        {
            this._tempInsuredService = tempInsuredService;
            this.logError = LogManager.GetLogger("ERROR");
            this.logInfo = LogManager.GetLogger("INFO");
        }
        /// <summary>
        /// 获取临时被保险信息
        /// </summary>
        /// <param name="agentId">bx_agent.id</param>
        /// <param name="buId">bx_userinfo.Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetTempInsuredInfoAsync(long buId = -1, int agentId = -1)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                var tempInsuredInfoAsync = _tempInsuredService.GetTempInsuredInfoAsync(agentId, buId);
                logInfo.Info(string.Format("获取被保险人信息请求url为：{0}；请求参数为：{1}，{2}", Request.RequestUri.ToString(), agentId, buId));

                var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, tempInsuredInfo = await tempInsuredInfoAsync };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }

        }
        /// <summary>
        /// 保存临时被保险人信息
        /// </summary>
        /// <param name="tempInsuredViewModel">被保险信息</param>
        /// <param name="isEdit">是否为编辑，默认为false</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> SaveTempInsuredInfoAsync(TempInsuredViewModel tempInsuredViewModel, bool isEdit = false)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "保存失败" };
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                return baseViewModel.ResponseToJson();
            }
            try
            {
                var isSuccess = _tempInsuredService.SaveTempInsuredInfoAsync(tempInsuredViewModel, isEdit);
                logInfo.Info(string.Format("保存被保险人信息请求url为：{0}；请求参数为：{1}，{2}", Request.RequestUri.ToString(), JsonHelper.Serialize(tempInsuredViewModel), isEdit));
                if (await isSuccess)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "保存成功";
                }
                return baseViewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常 ";
                logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }

        /// <summary>
        /// 报价用来存放用户临时邮箱电话
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> SaveUserExpandAsync([FromUri]UserExpandRequest request)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "保存失败" };
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                return baseViewModel.ResponseToJson();
            }
            try
            {
                var isSuccess = await _tempInsuredService.SaveUserExpandAsync(request);
                logInfo.Info(string.Format("用户临时信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(request)));
                if (isSuccess)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "保存成功";
                }
                return baseViewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常 ";
                logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }
    }
}