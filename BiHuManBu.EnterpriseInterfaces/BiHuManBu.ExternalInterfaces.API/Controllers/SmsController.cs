using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Sms;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Mapper;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Sms;
using log4net;
using ServiceStack.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class DB {

    }
    /// <summary>
    /// 
    /// </summary>
    public class SmsController : ApiController
    {
        private ILog logInfo;
        private ILog logError;

        public ISmsService _smsService;

        public SmsController(ISmsService smsService)
        {
            DB db = new DB();
            logInfo = LogManager.GetLogger("INFO");
            logError = LogManager.GetLogger("ERROR");
            _smsService = smsService;
        }

        /// <summary>
        /// 发送短信此接口不新调用。请使用ConsumerDetail->SentSms
        /// </summary>
        /// <param name="smsRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SendSms([FromBody]SmsRequest smsRequest)
        {
           
            logInfo.Info(string.Format("短信接口请求串：{0}", smsRequest.ToJson()));
            var viewModel = new SmsResultViewModel_sms();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }

            var response = _smsService.SendSms(smsRequest, Request.GetQueryNameValuePairs());
            logInfo.Info(string.Format("短信接口返回值：{0}", response.ToJson()));
            if (response.Status == HttpStatusCode.BadRequest || response.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
            }
            else if (response.Status == HttpStatusCode.ExpectationFailed)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
            }
            else if (response.Status == HttpStatusCode.UnsupportedMediaType)
            {
                viewModel.BusinessStatus = -10004;
                viewModel.StatusMessage = "报价短信不允许车牌为空";
            }
            else if (response.Status == HttpStatusCode.NoContent)
            {
                viewModel.BusinessStatus = 0;
                viewModel.MessagePayType = response.MessagePayType;
                viewModel.StatusMessage = "账号短信余额不足";
            }
            else
            {
                viewModel = response;
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 获取代理人短信对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSmsAccount([FromUri]SmsAccountRequest request)
        {
            logInfo.Info(string.Format("获取经纪人短信信息接口请求串：{0}", Request.RequestUri));
            var viewModel = new SmsAccountViewModel();

            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var response = _smsService.GetSmsAccount(request, Request.GetQueryNameValuePairs());
            logInfo.Info("获取经纪人短信信息接口返回值" + response.ToJson());
            if (response.Status == HttpStatusCode.BadRequest || response.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.ExpectationFailed)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                return viewModel.ResponseToJson();
            }
            if (response.ErrCode == -1)
            {
                viewModel.BusinessStatus = -1;
                viewModel.StatusMessage = "无经纪人短信信息";
            }
            else
            {
                viewModel.SmsAccount = response.SmsAccount.ConverToViewModel();
                viewModel.BusinessStatus = 1;
            }

            return viewModel.ResponseToJson();
        }

        #region 短信充值SmsRecharge

        /// <summary>
        /// 创建短信账号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage CreateAccount([FromUri]CreateAccountRequest request)
        {
            logInfo.Info(string.Format("创建账号接口请求串：{0}", request.ToJson()));
            var viewModel = new SmsAccountViewModel();

            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var response = _smsService.CreateAccount(request, Request.GetQueryNameValuePairs());
            logInfo.Info("创建账号接口返回值" + response.ToJson());
            if (response.ErrCode == -2)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = response.ErrMsg;//账号已存在
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.BadRequest || response.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.ExpectationFailed)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = response.ErrMsg;
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.OK)
            {
                viewModel.BusinessStatus = 1;
                viewModel.SmsAccount = response.SmsAccount.ConverToViewModel();
            }
            else
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = response.ErrMsg;
            }

            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 短信充值:创建短信充值订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SmsCreateOrder([FromBody]SmsCreateOrderRequest request)
        {
            logInfo.Info(string.Format("获取创建短信充值订单接口请求串：{0}", request.ToJson()));
            var viewModel = new SmsOrderViewModel();

            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var response = _smsService.SmsCreateOrder(request, Request.GetQueryNameValuePairs());
            logInfo.Info("获取创建短信充值订单接口返回值" + response.ToJson());
            if (response.Status == HttpStatusCode.BadRequest || response.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.ExpectationFailed)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "创建订单失败";
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.OK)
            {
                viewModel.BusinessStatus = 1;
                viewModel.SmsOrder = response.SmsOrder.ConverToViewModel();
            }
            else
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "创建订单失败";
            }

            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 短信充值:修改短信充值订单状态，并为代理人充值
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SmsOrderStatus([FromUri]SmsOrderStatusRequest request)
        {
            
            logInfo.Info(string.Format("获取短信充值状态修改接口请求串：{0}", Request.RequestUri));
            var viewModel = new BaseViewModel();

            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var response = _smsService.SmsOrderStatus(request, Request.GetQueryNameValuePairs());
            logInfo.Info("获取短信充值状态修改接口返回值" + response.ToJson());
            if (response.Status == HttpStatusCode.BadRequest || response.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.ExpectationFailed)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = response.ErrMsg;//充值订单修改状态失败
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.OK)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = response.ErrMsg; //充值订单修改状态失败
            }
            else
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = response.ErrMsg;//充值订单修改状态失败
            }

            return viewModel.ResponseToJson();
        }

        [HttpGet]
        public HttpResponseMessage GetSmsOrderDetail([FromUri]GetSmsOrderDetailRequest request)
        {
            logInfo.Info(string.Format("获取短信充值订单详情接口请求串：{0}", Request.RequestUri));
            var viewModel = new SmsOrderViewModel();

            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var response = _smsService.GetSmsOrderDetail(request, Request.GetQueryNameValuePairs());
            logInfo.Info("获取短信充值订单详情接口返回值" + response.ToJson());
            if (response.Status == HttpStatusCode.ExpectationFailed)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                return viewModel.ResponseToJson();
            }
            if (response.ErrCode == -1)
            {
                viewModel.BusinessStatus = -1;
                viewModel.StatusMessage = "无此订单信息";
            }
            else
            {
                viewModel.SmsOrder = response.BxSmsOrder.ConverToViewModel();
                viewModel.BusinessStatus = 1;
            }

            return viewModel.ResponseToJson();
        }

        #endregion
    }

}
