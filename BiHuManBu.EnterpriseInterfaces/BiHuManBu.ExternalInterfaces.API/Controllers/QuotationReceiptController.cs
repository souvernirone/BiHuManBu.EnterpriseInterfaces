using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using ServiceStack.Text;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 已出保单
    /// </summary>
    public class QuotationReceiptController : ApiController
    {
        private readonly ILog _logInfo;
        private readonly ILog _logError;
        private readonly IAppoinmentService _appoinmentService;
        private readonly IVerifyService _verifyService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public QuotationReceiptController(IAppoinmentService appoinmentService, IVerifyService verifyService)
        {
            try
            {
                _logInfo = LogManager.GetLogger("INFO");
                _logError = LogManager.GetLogger("ERROR");
                _appoinmentService = appoinmentService;
                _verifyService = verifyService;
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

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
                return _appoinmentService.GetQuotationReceiptListData(request);
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return new HttpResponseMessage();
        }
        /// <summary>
        /// 出单成功进行刷新续保(刷新历史数据）
        /// </summary>
        /// <returns></returns>
        [HttpGet, Log("出单成功进行刷新续保")]
        public HttpResponseMessage UpdateQuotationReceiptOldList()
        {
           return _appoinmentService.UpdateQuotationReceiptOldList().ResponseToJson();           
        }
    }
}
