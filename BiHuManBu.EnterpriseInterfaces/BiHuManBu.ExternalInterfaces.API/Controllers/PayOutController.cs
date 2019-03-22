using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.Enums;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using log4net;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using System.Threading.Tasks;
using System.Text;
using BiHuManBu.ExternalInterfaces.Services.OrderService.Interfaces;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 身份证采集+支付  对外接口
    /// </summary>
    public class PayOutController : ApiController
    {
        private ICustomerOrderService _customerOrderService;
        private IPayService _payService;
        private IVerifyService _verifyService;
        private readonly IGetPolicyService _getPolicyService;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private readonly ILog logErro = LogManager.GetLogger("ERROR");


        public PayOutController(ICustomerOrderService customerOrderService, IVerifyService verifyService, IPayService payService,IGetPolicyService getPolicyService)
        {
            _customerOrderService = customerOrderService;
            _verifyService = verifyService;
            _payService = payService;
            _getPolicyService = getPolicyService;
        }

        #region 支付对外接口  武院院/pc 2017/09/13
        /// <summary>
        /// 获取采集设备(对外) 武院院/对外接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("【获取采集设备(对外)】", true), CollectLimit((int)EnumCollectLimitInterfaceType.Machine)]
        [ActionName("Machine")]
        public HttpResponseMessage GetMachines([FromUri]CollectIdCardRequest request)
        {
                var baseRequest = new BaseRequest();
                baseRequest.SecCode = request.SecCode;
                baseRequest.Agent = request.Agent;
                bool isTrue;
                var viewModel = DataVerify(baseRequest,Request.Method.Method,out isTrue);
                if (!isTrue)
                    return viewModel.ResponseToJson();

                request.IsOut = true;
                var response = _payService.Machines(request);
                var data = new List<Machine>();
                foreach (var item in response)
                {
                    // item.MachineCode = "";//机器码
                    item.SaleChannel = null;
                    if (item.InsuranceType != null)
                        item.Source = SourceGroupAlgorithm.GetNewSource((int)item.InsuranceType);
                    data.Add(item);
                }
                return new
                {
                    BusinessStatus = 1,
                    StatusMessage = "获取成功！",
                    Data = data
                }.ResponseToJson();
        }

        /// <summary>
        /// 获取采集信息（对外） 武院院/对外接口 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("【获取采集信息(对外)】", true), CollectLimit((int)EnumCollectLimitInterfaceType.CollectInfo)]
        [ActionName("CollectInfo")]
        public HttpResponseMessage CollectInfo([FromBody]CollectIdCardRequest request)
        {
                var baseRequest = new BaseRequest();
                baseRequest.SecCode = request.SecCode;
                baseRequest.Agent = request.Agent;
                bool isTrue;
                var viewModel = DataVerify(baseRequest, Request.Method.Method, out isTrue, CommonHelper.JsonSerializeObject(request));
                if (!isTrue)
                    return viewModel.ResponseToJson();

                request.IsOut = true;
                var response = _payService.CollectIdCard(request);
                return response.ResponseToJson();          
        }

        /// <summary>
        /// 获取支付地址信息（对外） 武院院/对外接口   
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("【获取支付地址信息(对外)】", true), CollectLimit((int)EnumCollectLimitInterfaceType.PayAddress)]
        [ActionName("PayAddress")]
        public HttpResponseMessage PayAddress([FromUri]OrderPayRequest request)
        {
            var obj = PayAddressCore(request);
            return obj.ResponseToJson();
        }

        private object PayAddressCore(OrderPayRequest request)
        {
            if (string.IsNullOrEmpty(request.ForcetNo) && string.IsNullOrEmpty(request.BiztNo))
            {
                return new
                {
                    BusinessStatus = -33001,
                    StatusMessage = "商业险投保单号和交强险投保单不能同时为空！"
                };
            }
            var baseRequest = new BaseRequest();
            baseRequest.SecCode = request.SecCode;
            baseRequest.Agent = request.Agent;
            bool isTrue;
            var viewModel = DataVerify(baseRequest, Request.Method.Method, out isTrue);
            if (!isTrue)
                return viewModel;

            #region //安心对外
            if (request.Source == 4096)
            {
                var aXPayRequest = new AXPayRequest
                {
                    OrderNo = request.OrderNo,
                    Agent = request.Agent,
                    BuId = request.BuId,
                    PayAmt = request.PayAmt,
                    LicenseNo = request.LicenseNo,
                    CarVin = request.CarVin,
                    Biztno = request.BiztNo,
                    Forcetno = request.ForcetNo,
                    PayMent = request.PayMent
                };
                var message = "获取成功";
                var payUrl = _payService.GetAxPayPath(aXPayRequest, out message);
                if (request.IsGetPayWay == 0)
                {
                    return new
                    {
                        BusinessStatus = string.IsNullOrEmpty(payUrl) ? -10019 : 1,
                        StatusMessage = message,
                        Data = new { PayUrl = payUrl }
                    };
                }
                else
                {
                    return new
                    {
                        BusinessStatus = string.IsNullOrEmpty(payUrl) ? -10019 : 1,
                        StatusMessage = message,
                        Data = new { PayUrl = payUrl, PayWay = 0 }
                    };
                }
            }
            #endregion

            request.IsOut = true;
            var response = _payService.PayQR(request);
            if (response.BusinessStatus == 33011)
            {
                response.BusinessStatus = -33011;
                response.StatusMessage = "服务器无法连接!";
            }
            if (response.BusinessStatus == 1)
            {
                response.BusinessStatus = null;
                response.StatusMessage = null;
                if (response.FailureTime != null)
                    response.FailureTimeStamp = CommonHelper.GetSecondsTimeStamp((DateTime)response.FailureTime);
                return new
                {
                    BusinessStatus = 1,
                    StatusMessage = "获取成功！",
                    Data = response
                };
            }
            return response;
        }

        /// <summary>
        /// 发送电子投保确认单 wyy/对外接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("【发送电子投保确认单(对外)】", true), CollectLimit((int)EnumCollectLimitInterfaceType.SnedSms)]
        [ActionName("SendSms")]
        public HttpResponseMessage SendSms([FromUri]OrderPayRequest request)
        {
            if (string.IsNullOrEmpty(request.ForcetNo) && string.IsNullOrEmpty(request.BiztNo))
            {
                return new
                {
                    BusinessStatus = -33001,
                    StatusMessage = "商业险投保单号和交强险投保单不能同时为空！"
                }.ResponseToJson();
            }
            var baseRequest = new BaseRequest();
            baseRequest.SecCode = request.SecCode;
            baseRequest.Agent = request.Agent;
            bool isTrue;
            var viewModel = DataVerify(baseRequest, Request.Method.Method, out isTrue);
            if (!isTrue)
                return viewModel.ResponseToJson();

            request.IsOut =true;
            return _payService.SendSms(request).ResponseToJson();
        }

        /// <summary>
        /// 获取电子投保确认单签单状态(对外) 易孟飞
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, HttpPost, Log("【获取电子投保确认单签单状态(对外)】", true),
            CollectLimit((int)EnumCollectLimitInterfaceType.SendSmsState)]
        public HttpResponseMessage SendSmsState([FromUri]OrderPayRequest request, [FromBody]OrderPayRequest request1)
        {
            request = request ?? request1;
            request.PayMent = 2;
            dynamic obj = PayAddressCore(request);//获取投保单签单状态

            if (obj.BusinessStatus == (int)PayOutErrCode.成功)
            {
                return new
                {
                    BusinessStatus = 1,
                    StatusMessage = "成功，客户已经签单"
                }.ResponseToJson();
            }
            else if (obj.BusinessStatus == (int)PayOutErrCode.广东_未签电子投保单不允许缴费)
            {
                return new
                {
                    BusinessStatus = 33035,
                    StatusMessage = "失败，客户未签电子投保单"
                }.ResponseToJson();
            }
            else
            {
                return (obj as object).ResponseToJson();
            }
        }

        /// <summary>
        /// 作废订单支付方式（对外） 武院院/对外接口   
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("【作废订单支付方式(对外)】", true), CollectLimit((int)EnumCollectLimitInterfaceType.VoidPay)]
        [ActionName("VoidPay")]
        public HttpResponseMessage VoidPayQR([FromUri]OrderPayRequest request)
        {
            var baseRequest = new BaseRequest();
            baseRequest.SecCode = request.SecCode;
            baseRequest.Agent = request.Agent;
            bool isTrue;
            var viewModel = DataVerify(baseRequest, Request.Method.Method, out isTrue);
            if (!isTrue)
                return viewModel.ResponseToJson();
            request.IsOut = true;
            var response = _payService.VoidPayQR(request);
            if (response.BusinessStatus == 1)
            {            
                return new
                {
                    response.BusinessStatus,
                    response.StatusMessage
                }.ResponseToJson();
            }
            return  new
            {
                BusinessStatus=-10019,
                StatusMessage="作废原支付方式失败!"
            }.ResponseToJson();
        }

        /// <summary>
        /// 获取支付成功信息(对外) 武院院/对外接口 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("【获取支付成功信息(对外)】",true), CollectLimit((int)EnumCollectLimitInterfaceType.PayInfo)]
        [ActionName("PayInfo")]
        public async Task<HttpResponseMessage> PayInfo([FromUri]OrderPayRequest request)
        {
                var baseRequest = new BaseRequest();
                baseRequest.SecCode = request.SecCode;
                baseRequest.Agent = request.Agent;
                bool isTrue;
                var viewModel = DataVerify(baseRequest, Request.Method.Method, out isTrue);
                if (!isTrue)
                    return viewModel.ResponseToJson();
                request.IsOut = true;
                //安心对外
                if (request.Source == 4096)
                {
                    var aXPayResponse = new AXPayResponse
                    {
                        ChannelId = (int)request.ChannelId,
                        CAppValidateNo = request.CAppValidateNo,
                        orderNo = request.OrderNo,
                        Biztno=request.BiztNo,
                        ForcetNo=request.ForcetNo
                    };
                    var result = await _payService.GetAXPayInfo(aXPayResponse, true);
                    var code = result.Code;
                    var message = result.ErrorMsg;
                    result.Code = null;
                    result.ErrorMsg = null;
                    return new
                    {
                        BusinessStatus = code == 1 ? 1 : -10001,
                        StatusMessage = code == 1 ? "支付成功！" : message,
                        Data = result
                    }.ResponseToJson();
                }
                var response = await _payService.Pay(request);
                var businessStatus = response.BusinessStatus;
                var statusMessage = response.StatusMessage;
                response.BusinessStatus = null;
                response.StatusMessage = null;
                return new
                {
                    BusinessStatus = businessStatus == 1 ? 1 : -10001,
                    StatusMessage = businessStatus == 1 ? "支付成功！" : statusMessage,
                    Data = businessStatus == 1 ? response : null
                }.ResponseToJson();
        }

        /// <summary>
        /// 补发电子保单 武院院/对外接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("【补发电子保单（对外】"), CollectLimit((int)EnumCollectLimitInterfaceType.ElectronicPolicy)]
        [ActionName("ElectronicPolicy")]
        public HttpResponseMessage ReissueElectronicPolicy([FromUri]OrderPayRequest request)
        {
            var baseRequest = new BaseRequest();
            baseRequest.SecCode = request.SecCode;
            baseRequest.Agent = request.Agent;
            bool isTrue;
            var viewModel = DataVerify(baseRequest, Request.Method.Method, out isTrue);
            if (!isTrue)
                return viewModel.ResponseToJson();
            request.IsOut = true;
            return _payService.ReissueElectronicPolicy(request).ResponseToJson();
        }

        /// <summary>
        /// 获取支付合作银行(对外) 武院院/对外接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("【获取支付合作银行(对外)】", true), CollectLimit((int)EnumCollectLimitInterfaceType.PayBank)]
        [ActionName("PayBank")]
        public HttpResponseMessage GetPayWayBanks([FromUri]PayWayRequest request)
        {
            var baseRequest = new BaseRequest();
            baseRequest.SecCode = request.SecCode;
            baseRequest.Agent = request.Agent;
            bool isTrue;
            var viewModel = DataVerify(baseRequest, Request.Method.Method, out isTrue);
            if (!isTrue)
                return viewModel.ResponseToJson();

            request.IsOut = true;
            var model = new PayWayBanksModel();
            model.BankId = request.BankId;
            model.BankName = request.BankName;
            model.CityId = request.CityId;
            model.PayWay = request.PayWay;                        
            var response = _payService.GetPayWayBanks(model);
            //转换
            var  pys = new List<PayWayBanksModel>();
            foreach (var item in pys) {
                var mode = new PayWayBanksModel();
                mode.BankId = mode.BankId;
                mode.BankName = mode.BankName;
                mode.CityId = mode.CityId;
                mode.PayWay = mode.PayWay;
                pys.Add(mode);
            }
            return new
            {
                BusinessStatus = 1,
                StatusMessage = "获取成功！",
                Data = response
            }.ResponseToJson();
        }
        #endregion

        #region  电子保单下载
        /// <summary>
        /// 申请下载电子保单(对外) lhy/对外接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("【申请下载电子保单(对外)】", true), CollectLimit((int)EnumCollectLimitInterfaceType.ApplyElecPolicy)]
        [ActionName("ApplyElecPolicy")]
        public HttpResponseMessage ApplyElecPolicy([FromUri]EPolicycs request)
        {
            var baseRequest = new BaseRequest();
            baseRequest.SecCode = request.SecCode;
            baseRequest.Agent = request.Agent;
            bool isTrue;
            var viewModel = DataVerify(baseRequest, Request.Method.Method, out isTrue);
            if (!isTrue)
                return viewModel.ResponseToJson();

            request.IsOut = 1;
            var response = _payService.ApplyElecPolicy(request);
            return response.ResponseToJson();
        }
        /// <summary>
        /// 获取申请下载电子保单状态(对外) lhy/对外接口 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("【获取申请下载电子保单状态(对外)】", true), CollectLimit((int)EnumCollectLimitInterfaceType.GetElecPolicyApply)]
        [ActionName("GetElecPolicyApply")]
        public HttpResponseMessage GetElecPolicyApply([FromUri]EPolicycs request)
        {
            var baseRequest = new BaseRequest();
            baseRequest.SecCode = request.SecCode;
            baseRequest.Agent = request.Agent;
            bool isTrue;
            var viewModel = DataVerify(baseRequest, Request.Method.Method, out isTrue);
            if (!isTrue)
                return viewModel.ResponseToJson();

            request.IsOut = 1;
            var response = _payService.GetElecPolicyApply(request);
            return response.ResponseToJson();
        }

        /// <summary>
        /// 下载电子保单(对外) lhy/对外接口  
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("【下载电子保单(对外)】", true), CollectLimit((int)EnumCollectLimitInterfaceType.DownloadElecPolicy)]
        [ActionName("DownloadElecPolicy")]
        public HttpResponseMessage DownloadElecPolicy([FromUri]EPolicycs request)
        {
            var baseRequest = new BaseRequest();
            baseRequest.SecCode = request.SecCode;
            baseRequest.Agent = request.Agent;
            bool isTrue;
            var viewModel = DataVerify(baseRequest, Request.Method.Method, out isTrue);
            if (!isTrue)
                return viewModel.ResponseToJson();

            request.IsOut = 1;
            var response = _payService.DownloadElecPolicy(request);
            if (response.BusinessStatus == 1 && !string.IsNullOrWhiteSpace(response.Data.ToString())&& response.Data.ToString().IndexOf("http")<0) {
                response.Data = "http://p1cswweui.bkt.clouddn.com" + response.Data;
            }
            return response.ResponseToJson();
        }

        #endregion

        #region 参数校验的公共方法
        /// <summary>
        /// 参数校验的公共方法
        /// </summary>
        /// <param name="request"></param>
        /// <param name="httpMethod">http请求方法</param>
        /// <param name="isTrue"></param>
        /// <param name="postParm">post或者put请求时,为请求实体的json,其他方式请求不必传该参数</param>
        /// <returns></returns>
        private BaseViewModel DataVerify(BaseRequest request,string httpMethod,out bool isTrue,string postParm="")
        {
            var viewModel = new BaseResponse();
            if (!ModelState.IsValid)
            {
                viewModel.ErrCode = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                if (!msg.Trim().Equals("ChildAgent参数错误;")) {
                    viewModel.ErrMsg = "输入参数错误，" + msg;
                    isTrue = false;
                    return new BaseViewModel
                    {
                        BusinessStatus = viewModel.ErrCode,
                        StatusMessage = viewModel.ErrMsg
                    }; 
                }
            }

            if (httpMethod.ToLower().Equals("post") || httpMethod.ToLower().Equals("put"))
                viewModel = _verifyService.ValidateReqestPost(postParm, request.SecCode);
            else
                viewModel = _verifyService.ValidateReqestGet(Request.GetQueryNameValuePairs(), request.SecCode);

            //校验返回值
            if (viewModel!=null && viewModel.ErrCode != 1)
            {
                //校验失败，返回错误码
                isTrue = false;
                return new BaseViewModel {
                    BusinessStatus= viewModel.ErrCode,
                    StatusMessage= viewModel.ErrMsg
                };
            }
            isTrue = true;
            return new BaseViewModel
            {
                BusinessStatus =1,
                StatusMessage = viewModel==null?"成功！":viewModel.ErrMsg
            };
        }
        #endregion

        #region  安心回调      
        /// <summary>
        /// 获取安心支付回调 武院院/pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("【安心支付后台回调接口】")]
        [ActionName("GetAXPayBack")]
        public HttpResponseMessage GetAXPayBack([FromBody]AXPayResponse request)
        {
            //反馈安心已接收到数据
            if (_payService.GetAXPayBack(request))
            {
                var responseMessage = new HttpResponseMessage
                {
                    Content = new StringContent("success", Encoding.GetEncoding("UTF-8"), "text/plain")
                };
                return responseMessage;
            }
            else
            {
                var responseMessage = new HttpResponseMessage
                {
                    Content = new StringContent("error", Encoding.GetEncoding("UTF-8"), "text/plain")
                };
                return responseMessage;
            }
        }

        #endregion

        #region 抓单
        /// <summary>
        /// 获取采集设备 武院院/pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("【投保单信息抓取】", true), CollectLimit((int)EnumCollectLimitInterfaceType.TPolicy)]        
        [ActionName("TPolicy")]
        public HttpResponseMessage GetPolicyAllInfo([FromUri]TPolicy request)
        {
            var baseRequest = new BaseRequest();
            baseRequest.SecCode = request.SecCode;
            baseRequest.Agent = request.Agent;
            bool isTrue;
            var viewModel = DataVerify(baseRequest, Request.Method.Method, out isTrue);
            if (!isTrue)
                return viewModel.ResponseToJson();
            //跟CustomerOrderController的GetPolicyAllInfo一样，如果后期改逻辑，两头都处理一下
            //建议顶级抓单，userinfo的isdistributed此处默认赋值为0，如果有调整需要参照续保接口初始化userinfo方法
            return _getPolicyService.GetPolicyAllInfoOut(request).ResponseToJson();
        }
        #endregion

    }
}
