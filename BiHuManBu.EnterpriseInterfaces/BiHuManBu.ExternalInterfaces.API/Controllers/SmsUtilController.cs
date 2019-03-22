
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class SmsUtilController : ApiController
    {
        readonly ISmsUtilService _smsUtilService;
        readonly ILog logError;
        readonly ILog logInfo;
        readonly string txtPath = "/Content/BadWords/黑词160901.txt";
       
        public SmsUtilController(ISmsUtilService smsUtilService)
        {
            this._smsUtilService = smsUtilService;
            this.logInfo = LogManager.GetLogger("INFO");
            this.logError = LogManager.GetLogger("ERROR");
        }
        /// <summary>
        /// 初始化黑词到缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> InitBadWords()
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "初始化成功" };
            try
            {
                var badWordsArray = await _smsUtilService.InitBadWords( System.Web.HttpContext.Current.Server.MapPath(txtPath));
            }
            catch (Exception ex) {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 移除缓存中的黑词
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> RemoveBadWordsCache()
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "清除缓存成功" };
            try
            {
                var isSuccess = await _smsUtilService.RemoveBadWordsCache();
            }
            catch (Exception ex) {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
 
        }
    }
}