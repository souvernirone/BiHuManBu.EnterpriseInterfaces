using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class QuoteRecordController : ApiController
    {
        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        private readonly ILog _logError = LogManager.GetLogger("ERROR");
        private readonly IQuoteRecordService _quoteRecordService;
        private readonly IVerifyService _verifyService;
        public QuoteRecordController(IQuoteRecordService quoteRecordService, IVerifyService verifyService)
        {
            _verifyService = verifyService;
            _quoteRecordService = quoteRecordService;
        }
        /// <summary>
        /// 添加车主寻价记录
        /// 刘松年  2018-7-13  /微信
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> AddQuoteRecord([FromUri]AddQuoteRecordRequest request)
        {
            _logInfo.Info("添加车主寻价记录请求串：" + Request.RequestUri);
            BaseViewModel viewModel = new BaseViewModel();

            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            //安全校验
            //var baseResponse = _verifyService.Verify(request.SecCode, Request.GetQueryNameValuePairs());
            //if (baseResponse.ErrCode != 1)
            //{//校验失败，返回错误码
            //    viewModel.BusinessStatus = baseResponse.ErrCode;
            //    viewModel.StatusMessage = baseResponse.ErrMsg;
            //    return viewModel.ResponseToJson();
            //}
            try
            {
                var result = await _quoteRecordService.AddQuoteRecord(request);
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                _logError.Error("请求串为：" + Request.RequestUri + "\n发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return viewModel.ResponseToJson();
            }
        }
    }
}