using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using ServiceStack.Text;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class QuoteHistoryController : ApiController
    {
        //
        // GET: /QuoteHistory/
        public IQuoteHistoryService _iQuoteHistoryService;
        private readonly IVerifyService _verifyService;
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public QuoteHistoryController(IQuoteHistoryService iQuoteHistoryService, IVerifyService verifyService)
        {
            _iQuoteHistoryService = iQuoteHistoryService;
            _verifyService = verifyService;
        }

        /// <summary>
        /// 获取报价历史数量
        /// </summary>
        /// <param name="licenseNo"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetQuoteHistoryCount([FromUri]GetQuoteHistoryRequest request)
        {

            logInfo.Info("获取报价历史数量请求串为：" + Request.RequestUri + "请求参数:" + request.ResponseToJson());
            BaseViewModel model = new BaseViewModel();
            if (!ModelState.IsValid)
            {
                model.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                model.StatusMessage = "输入参数错误，" + msg;
                return model.ResponseToJson();
            }
            try
            {
                int quoteLimitCount = 0;
                long buid = 0;
                string createtime = _iQuoteHistoryService.GetQuoteHistoryCount(request, Request.GetQueryNameValuePairs(), out buid, out quoteLimitCount);
                int status = buid > 0 ? 1 : 0;
                var modelObj = new
                {
                    BusinessStatus = status,
                    StatusMessage = status > 0 ? "存在报价" : "无报价历史",
                    QuoteTime = createtime,
                    BuId = buid,
                    QuoteCount = quoteLimitCount
                };
                return modelObj.ResponseToJson();
            }
            catch (Exception ex)
            {
                model.BusinessStatus = -10003;
                model.StatusMessage = "服务发生异常";
                logError.Error("请求串为：" + Request.RequestUri + "\n发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return model.ResponseToJson();
            }
        }
        /// <summary>
        /// 暂时没有用
        /// </summary>
        /// <param name="Buid"></param>
        /// <param name="SecCode"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetQuoteHistoryByBuid(long Buid = 0, string SecCode = "")
        {
            GetQuoteHistoryByAgentViewModel viewModel = new GetQuoteHistoryByAgentViewModel();
            if (Buid < 1)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验失败";
                return viewModel.ResponseToJson();
            }
            var baseResponse = _verifyService.Verify(SecCode, Request.GetQueryNameValuePairs());
            if (baseResponse.ErrCode != 1)
            {//校验失败，返回错误码
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel.ResponseToJson();
            }
            try
            {
                List<QuoteHistoryViewModel> list = _iQuoteHistoryService.GetQuoteHistoryByBuid(Buid);
                return new
                {
                    Date = list,
                    BusinessStatus = baseResponse.ErrCode,
                    StatusMessage = baseResponse.ErrMsg
                }.ResponseToJson();
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("请求串为：" + Request.RequestUri + "\n发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return viewModel.ResponseToJson();
            }
        }
        /// <summary>
        /// 获取车主寻价历史
        /// 刘松年  2018-7-16  /微信
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetQuoteHistoryByAgent([FromUri]GetQuoteHistoryByAgent request)
        {
            GetQuoteHistoryByAgentViewModel viewModel = new GetQuoteHistoryByAgentViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var baseResponse = _verifyService.Verify(request.SecCode, Request.GetQueryNameValuePairs());
            if (baseResponse.ErrCode != 1)
            {//校验失败，返回错误码
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel.ResponseToJson();
            }
            try
            {
                return _iQuoteHistoryService.GetQuoteHistoryByAgent(request).ResponseToJson();
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("请求串为：" + Request.RequestUri + "\n发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return viewModel.ResponseToJson();
            }
        }
    }
}
