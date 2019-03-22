using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using BiHuManBu.ExternalInterfaces.Services.Implements;
using Newtonsoft.Json;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Infrastructure.JDPay;
using BiHuManBu.ExternalInterfaces.Infrastructure.JDPay.signature;
using BiHuManBu.ExternalInterfaces.Infrastructure.JDPay.httpUtil;
using BiHuManBu.ExternalInterfaces.Infrastructure.JDPay.responseObj;
using log4net;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppInterfaces = BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using System.Text.RegularExpressions;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class JDPayController : ApiController
    {
        private readonly AppInterfaces.IOrderService _orderService;
        private readonly AppInterfaces.IAppAchieveService _appAchieveService;
        private SortedDictionary<String, String> orderInfoDic;
        private readonly ILog _logInfo;
        private readonly ILog _logError;
        private readonly string priKey;
        private readonly string desKey;
        private readonly string pubKey;
        private readonly string createJdPayOrderUrl;
        private readonly string jdPay_Version;
        private readonly string jdPay_Merchant;
        private readonly string jdPay_NotifyUrl;
        private readonly string jdPay_Md5Solt;
        public JDPayController(AppInterfaces.IOrderService orderService, AppInterfaces.IAppAchieveService appAchieveService)
        {
            _orderService = orderService;
            _appAchieveService = appAchieveService;
            orderInfoDic = new SortedDictionary<string, string>();
            _logInfo = LogManager.GetLogger("INFO");
            _logError = LogManager.GetLogger("ERROR");
            priKey = ApplicationSettingsFactory.GetApplicationSettings().JDPay_PriKey;
            desKey = ApplicationSettingsFactory.GetApplicationSettings().JDPay_DesKey;
            pubKey = ApplicationSettingsFactory.GetApplicationSettings().JDPay_PubKey;
            createJdPayOrderUrl = ApplicationSettingsFactory.GetApplicationSettings().JDPay_CreateOrderUrl;
            jdPay_Version = ApplicationSettingsFactory.GetApplicationSettings().JDPay_Version;
            jdPay_Merchant = ApplicationSettingsFactory.GetApplicationSettings().JDPay_Merchant;
            jdPay_Md5Solt = ApplicationSettingsFactory.GetApplicationSettings().JDPay_Md5Solt;
            jdPay_NotifyUrl = ApplicationSettingsFactory.GetApplicationSettings().JDPay_NotifyUrl;
        }
        /// <summary>
        /// JD下单
        /// </summary>
        /// <returns></returns>
        [AuthorizeFilter]
        [HttpPost]
        public HttpResponseMessage CreateJdOrder([FromBody]CreateJdOrderRequest createJdOrderRequest)
        {
            var viewModel = new BaseViewModel();
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "创建JD订单成功";
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var carOrder = _appAchieveService.FindOrderByOrderId(createJdOrderRequest, Request.GetQueryNameValuePairs());
            if (carOrder.BusinessStatus != 1)
            {
                viewModel.BusinessStatus = carOrder.BusinessStatus;
                viewModel.StatusMessage = carOrder.StatusMessage;
                return viewModel.ResponseToJson();
            }
            if (carOrder.order_num != createJdOrderRequest.TradeNum)
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "输入参数错误，订单号和流水号不匹配";
                return viewModel.ResponseToJson();
            }
            orderInfoDic.AddOrPeplace("version", jdPay_Version);
            //orderInfoDic.AddOrPeplace("sign", this.Value);
            orderInfoDic.AddOrPeplace("merchant", jdPay_Merchant.Trim());
            //orderInfoDic.AddOrPeplace("device", this.device.Value.Trim());
            orderInfoDic.AddOrPeplace("tradeNum", createJdOrderRequest.TradeNum);
            orderInfoDic.AddOrPeplace("tradeName", DateTime.Now.ToString("yyyyMMddHHmmss"));
            //orderInfoDic.AddOrPeplace("tradeDesc", this.tradeDesc.Value.Trim());
            orderInfoDic.AddOrPeplace("tradeTime", DateTime.Now.ToString("yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo));
            //orderInfoDic.AddOrPeplace("amount", (Convert.ToInt32(createJdOrderRequest.Amount) * 100).ToString().Trim());
            orderInfoDic.AddOrPeplace("amount", ((int)(createJdOrderRequest.Amount*100)).ToString().Trim());
            orderInfoDic.AddOrPeplace("orderType", 1.ToString());
            orderInfoDic.AddOrPeplace("currency", "CNY");
            orderInfoDic.AddOrPeplace("userId", createJdOrderRequest.ChildAgent.ToString());
            //orderInfoDic.AddOrPeplace("note", this.note.Value.Trim());
            orderInfoDic.AddOrPeplace("notifyUrl", jdPay_NotifyUrl);
            //orderInfoDic.AddOrPeplace("ip", GetIp().Trim());
            //orderInfoDic.AddOrPeplace("specCardNo", this.specCardNo.Value.Trim());
            //orderInfoDic.AddOrPeplace("specId", this.specId.Value.Trim());
            //orderInfoDic.AddOrPeplace("specName", this.specName.Value.Trim());
            //orderInfoDic.AddOrPeplace("userType", this.userType.Value.Trim());
            //orderInfoDic.AddOrPeplace("userId", this.userId.Value.Trim());
            //orderInfoDic.AddOrPeplace("expireTime", this.expireTime.Value.Trim());
            //orderInfoDic.AddOrPeplace("industryCategoryCode", this.industryCategoryCode.Value.Trim());
            string reqXmlStr = string.Empty;
            try
            {
                 reqXmlStr = XMLUtil.encryptReqXml(priKey, desKey, orderInfoDic);
            }
            catch {
            }
     
            string response = string.Empty;
            CreateOrderResponse createOrderResponse = null;
            try
            {

                response = HttpUtil.postRest(createJdPayOrderUrl, reqXmlStr);

                createOrderResponse = XMLUtil.decryptResXml<CreateOrderResponse>(pubKey, desKey, response);
                if (createOrderResponse.result.code != "000000")
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = createOrderResponse.result.desc;
                }
                else {
                    string signData = MD5Util.Md5LowerCase(string.Format("merchant={0}&orderId={1}&key={2}", createOrderResponse.merchant, createOrderResponse.orderId, jdPay_Md5Solt), "");
                    createOrderResponse.sign = signData;
                }
                _logInfo.Info(string.Format("JD下单接口：返回值：{0}", JsonConvert.SerializeObject(createOrderResponse)));
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "创建JD订单失败";
                _logError.Error(string.Format("JD下单发生异常：{0}\n{1}\n{2}", ex.Source, ex.StackTrace, ex.Message));
            }

            return new { viewModel.BusinessStatus, viewModel.StatusMessage, createOrderResponse = createOrderResponse }.ResponseToJson();
        }
        /// <summary>
        /// jd支付回调
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage NotifyAsync()
        {
            HttpContextBase context = null;
            string req = string.Empty;
            string error = string.Empty;

            try
            {
                context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                byte[] byts = new byte[context.Request.InputStream.Length];
                context.Request.InputStream.Read(byts, 0, byts.Length);
                req = Encoding.UTF8.GetString(byts);
                AsynNotifyResponse anyResponse = XMLUtil.decryptResXml<AsynNotifyResponse>(pubKey, desKey, FormatString(req));
                int status = Convert.ToInt32(anyResponse.status);
                if (status == 2 || status == 3)
                {
                    _orderService.UpdateOrder(new ModifyOrderRequest { IsUpdatePayStatus = true, OrderNum = anyResponse.tradeNum, PayStatus = status });
                    error = "success";
                }
                _logInfo.Info(string.Format("JD支付回调接口：回调返回值：{0}", JsonConvert.SerializeObject(anyResponse)));

            }
            catch (Exception ex)
            {
                error = "fail";
                _logError.Error(string.Format("JD支付回调发生异常：回调返回值：{0}；错误信息：{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message));
            }
            return error.StringToHttpResponseMessage();
        }
        /// <summary>
        /// 通知APP是否成功付款
        /// </summary>
        /// <returns></returns>
        [AuthorizeFilter]
        [HttpGet]
        public HttpResponseMessage NotifyApp([FromUri]CreateJdOrderRequest createJdOrderRequest)
        {
            
            var baseViewModel = new BaseViewModel { BusinessStatus = -10002, StatusMessage = "支付处理中" };
            int payStatus = -1;
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                baseViewModel.StatusMessage = "输入参数错误，" + msg;
                return baseViewModel.ResponseToJson();
            }
            var carOrder = _appAchieveService.FindOrderByOrderId(createJdOrderRequest, Request.GetQueryNameValuePairs());
            if (carOrder.BusinessStatus != 1)
            {
                baseViewModel.BusinessStatus = carOrder.BusinessStatus;
                baseViewModel.StatusMessage = carOrder.StatusMessage;
                return baseViewModel.ResponseToJson();
            }
            if (carOrder.pay_status.HasValue && carOrder.pay_status.Value == 2 || carOrder.pay_status == 3)
            {
                payStatus = carOrder.pay_status.Value;
                baseViewModel.BusinessStatus = 1;
                baseViewModel.StatusMessage = "支付完成";
            }
            _logInfo.Info(string.Format("获取订单支付状态：订单编号：{0}；订单流水：{1};支付状态：{2}", createJdOrderRequest.OrderId, createJdOrderRequest.TradeNum, payStatus));
            return new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, PayStatus = payStatus }.ResponseToJson();

        }
        /// <summary>
        /// 获得Ip
        /// </summary>
        /// <returns></returns>
        private string GetIp()
        {
            IPAddress[] localIPs;
            localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            StringCollection IpCollection = new StringCollection();
            foreach (IPAddress ip in localIPs)
            {
                //根据AddressFamily判断是否为ipv4,如果是InterNetWork则为ipv6
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    String ipaddress = ip.ToString();
                    if (ipaddress.StartsWith("192.168") || ipaddress.StartsWith("127.0"))
                    {
                        continue;
                    }
                    IpCollection.Add(ip.ToString());
                }

            }
            string[] IpArray = new string[IpCollection.Count];
            IpCollection.CopyTo(IpArray, 0);
            return IpArray[0];
        }
        private string FormatString(string data)
        {
            return Regex.Replace(data.Replace(" ", "").Replace("<?xmlversion=\"1.0\"encoding=\"UTF-8\"?>",
                "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>"), @">\s+<", "><");
        }

    }


}

