using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order.ThirdModel;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;
using log4net;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Achieves
{
    public class OrderPostThirdService : IOrderPostThirdService
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        private readonly IMapEntityService _mapEntityService;
        private readonly IOrderRepository _orderRepository;
        private readonly string _urlHost = ConfigurationManager.AppSettings["SendOrderUrl"];
        private readonly string _orderAgent = ConfigurationManager.AppSettings["SendOrderAgent"];


        public OrderPostThirdService(IMapEntityService mapEntityService, IOrderRepository orderRepository)
        {
            _mapEntityService = mapEntityService;
            _orderRepository = orderRepository;
        }
        public void SendPost(int agent, string secretKey, int urlType, dd_order_quoteresult orderQuoteresult, dd_order_savequote orderSavequote, dd_order order,
            dd_order_related_info orderRelatedInfo, List<dd_order_paymentresult> listoOrderPaymentresults, bx_userinfo userinfo)
        {
            if (string.IsNullOrEmpty(_urlHost))
            {
                return;
            }
            if (string.IsNullOrEmpty(_orderAgent))
            {
                return;
            }
            //urlType：1生成订单回调；2支付成功回调；3取消订单回调

            //只有特定的用户才请求回调数据接口。
            //测试阶段不加
            if (int.Parse(_orderAgent) != 0 && int.Parse(_orderAgent) != agent)
            {
                logInfo.Info(string.Format("订单回调请求第三方接口前agent不匹配，代理人id为：{0}", agent));
                return;
            }
            if (order == null)
            {
                logInfo.Info(string.Format("订单回调请求第三方接口前Order为空，代理人id为：{0}", agent));
                return;
            }
            string strUrl = string.Empty;
            switch (urlType)
            {
                case 1://生成订单
                    strUrl = string.Format("{0}/insautozi/api/order/bihu/proposal", _urlHost);
                    break;
                case 2://支付成功
                    strUrl = string.Format("{0}/insautozi/api/order/bihu/pay", _urlHost);
                    break;
                case 3://取消订单
                    strUrl = string.Format("{0}/insautozi/api/order/bihu/cancel", _urlHost);
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(strUrl))
            {
                logInfo.Info(string.Format("订单回调请求第三方接口url地址为空，来源不明，代理人id为：{0}", agent));
                return;
            }
            //获取银行信息
            PayWayBanksModel bank = null;
            if (order.pay_way_id > 0)
            {
                bank = _orderRepository.GetPayWayBank(order.pay_way_id);
            }
            //异步发起调用第三方
            System.Threading.Tasks.Task.Factory.StartNew(() =>
                DoPost(agent, secretKey, urlType, orderQuoteresult, orderSavequote, order, orderRelatedInfo, listoOrderPaymentresults, userinfo, bank, strUrl)
            );
        }

        private void DoPost(int agent, string secretKey, int urlType, dd_order_quoteresult orderQuoteresult, dd_order_savequote orderSavequote, dd_order order,
            dd_order_related_info orderRelatedInfo, List<dd_order_paymentresult> listoOrderPaymentresults, bx_userinfo userinfo, PayWayBanksModel bank, string strUrl)
        {
            try
            {

                #region 订单
                ThirdOrderDetail orderDetail = new ThirdOrderDetail();
                string statusname = string.Empty;
                orderDetail = new ThirdOrderDetail()
                {
                    OrderNum = order.order_num,
                    ReBackReason = "",
                    ReBackDate = "",
                    CancelReason = "",
                    CancelDate = "",
                    //起保时间
                    BizStartDate = orderQuoteresult != null ? (orderQuoteresult.biz_start_date.HasValue ? orderQuoteresult.biz_start_date.Value.ToString("yyyy-MM-dd") : "") : "",
                    ForceStartDate = orderQuoteresult != null ? (orderQuoteresult.force_start_date.HasValue ? orderQuoteresult.force_start_date.Value.ToString("yyyy-MM-dd") : "") : "",

                    CancelAgent = "",
                    DeliveryMethod = order.delivery_method,
                    DeliveryAddress = order.delivery_address ?? "",
                    DeliveryContacts = order.delivery_contacts ?? "",
                    DeliveryContactsMobile = order.delivery_contacts_mobile ?? "",
                    DeliveryAddressId = order.delivery_address_id,
                    IssuingPeopleId = order.issuing_people_id ?? 0,
                    IssuingPeopleName = order.issuing_people_name ?? "",
                    IssuingPeopleMobile = "",
                    OrderType = GetOrderStatus(order.order_type, order.order_lapse_time, out statusname),
                    LastBizEndDate = order.LastBizEndDate.HasValue ? order.LastBizEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                    LastForceEndDate = order.LastForceEndDate.HasValue ? order.LastForceEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",

                    PayWayId = order.pay_way_id,
                    PayWayBankName = bank == null ? "" : bank.BankName,
                    PayMentRemark = order.PayMentRemark,
                    PayMent = order.PayMent,

                    InputOrderLapseTime = order.order_lapse_time.HasValue ? order.order_lapse_time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",

                    Commission = "0.00",
                    Integral = "0",

                    AgentId = order.agent_id,
                    AgentName = order.agent_name ?? "",
                    AgentMobile = "",
                };
                orderDetail.OrderStatus = orderDetail.OrderType;

                orderDetail.UserName = orderDetail.AgentName;
                string isTest = ConfigurationManager.AppSettings["IsTest"];
                if (!string.IsNullOrEmpty(isTest) && isTest.Equals("1"))
                {
                    orderDetail.UserName = "1807251650100079";
                }
                orderDetail.AreaId = order.quote_city_id.ToString();
                orderDetail.Channel = "";
                orderDetail.SrcPartyId = order.source.ToString();
                orderDetail.UnderWritingTime = order.create_time.ToString("yyyy-MM-dd HH:mm:ss");
                #endregion

                #region 报价信息 //支付和取消订单不需要
                ThirdPrecisePrice precisePrice = new ThirdPrecisePrice();
                if (urlType != 2 && urlType != 3)
                {
                    PrecisePrice OldPrecisePrice = _mapEntityService.GetMapPrecisePrice(orderQuoteresult, orderSavequote, order, null);
                    string strprecisePrice = OldPrecisePrice.ToJson();
                    if (!string.IsNullOrEmpty(strprecisePrice))
                    {
                        precisePrice = strprecisePrice.FromJson<ThirdPrecisePrice>();
                    }
                }
                #endregion

                #region 关系人信息 //支付和取消订单不需要
                ThirdRelatedInfo relatedInfo = new ThirdRelatedInfo();
                RelatedInfo OldRelatedInfo = _mapEntityService.GetMapRelatedInfo(orderRelatedInfo);
                if (urlType != 2 && urlType != 3)
                {
                    relatedInfo = new ThirdRelatedInfo();
                    if (OldRelatedInfo != null)
                    {
                        relatedInfo = new ThirdRelatedInfo() { HolderPerson = OldRelatedInfo.HolderPerson, InsuredPerson = OldRelatedInfo.InsuredPerson, OwerPerson = OldRelatedInfo.OwerPerson };
                    }
                }
                #endregion

                #region 支付信息 //取消订单不需要
                ThirdPaymentResult paymentResult = new ThirdPaymentResult()
                {
                    BizNo = "",
                    BizTno = "",
                    CreateTime = "",
                    CredentialImg = "",
                    ForceNo = "",
                    ForceTno = "",
                    IsPaymentTime = "",
                    OrderNum = order.order_num ?? "",
                    PurchaseAmount = 0,
                    Remark = ""
                };
                if (urlType != 3)
                {
                    if (listoOrderPaymentresults != null && listoOrderPaymentresults.Any())
                    {
                        PaymentResult oldPaymentResult = _mapEntityService.GetMapPaymentResult(listoOrderPaymentresults);
                        paymentResult = new ThirdPaymentResult()
                        {
                            PurchaseAmount = oldPaymentResult.PurchaseAmount,
                            Remark = oldPaymentResult.Remark,
                            CredentialImg = oldPaymentResult.CredentialImg,
                            CreateTime = oldPaymentResult.CreateTime,
                            BizNo = oldPaymentResult.BizNo,
                            ForceNo = oldPaymentResult.ForceNo,
                            BizTno = order.biz_tno,//OldPrecisePrice != null ? OldPrecisePrice.BizTno : "",
                            ForceTno = order.force_tno,//OldPrecisePrice != null ? OldPrecisePrice.ForceTno : ""
                        };
                    }
                }
                #endregion

                #region 车辆信息 //支付和取消订单不需要
                ThirdOrderCarInfo orderCarInfo = new ThirdOrderCarInfo();
                if (urlType != 2 && urlType != 3)
                {
                    OrderCarInfo oldCarInfo = _mapEntityService.GetPayOrderCarInfo(null, order, userinfo, orderQuoteresult);
                    string strcarInfo = oldCarInfo.ToJson();
                    if (!string.IsNullOrEmpty(strcarInfo))
                    {
                        orderCarInfo = strcarInfo.FromJson<ThirdOrderCarInfo>();
                    }
                }
                #endregion

                #region 返回模型
                PostOrderDetailModel model = new PostOrderDetailModel();
                model.CarInfo = orderCarInfo;
                model.Order = orderDetail;
                model.PaymentResult = paymentResult;
                model.RelatedInfo = relatedInfo;
                model.PrecisePrice = precisePrice;
                #endregion

                string strMsg = HttpWebAsk.HttpClientPostAsync(model.ToJson(), strUrl);
                logInfo.Info(string.Format("订单回调请求第三方接口：{0} \n 请求值为：{1} \n 返回消息：{2}", agent, model.ToJson(), strMsg));
            }
            catch (Exception ex)
            {
                logError.Error("调用" + agent + "接口回传订单接口异常，Url为：" + urlType + "；\n 异常信息:" + ex.StackTrace + " \n " + ex.Message);
            }
        }

        /// <summary>
        /// 订单状态 0暂存、1已过期、2已取消（原来叫废弃）、3被踢回、4进行中、5已完成
        /// 暂存：业务员点击“保存，暂不提交”的订单，点击暂存状态的订单，进入【完善订单页】；
        /// 已完成：等同于客户付款状态=已支付 & 投保状态=已承保；
        /// 进行中：等同于点击“提交订单”后，客户付款状态=已支付 & 投保状态=未承保，客户付款状态=待支付 & 投保状态=已承保，客户付款状态=待支付 & 投保状态=待缴费这三种状态；
        /// 被踢回：出单员踢回的订单；
        /// 废弃：业务员自己取消的被踢回的订单；
        /// 已过期
        /// </summary>
        private int GetOrderStatus(int orderType, DateTime? issueTime, out string statusName)
        {
            statusName = string.Empty;
            //废弃状态不变
            if (orderType == 2 || orderType == 5)
            {
                statusName = orderType == 2 ? "已取消" : "已完成";
                return orderType;
            }
            //其他状态判断如果订单失效，则失效
            if (issueTime.HasValue && issueTime.Value < DateTime.Now)
            {
                statusName = "已过期";
                return 1;
            }
            else
            {
                switch (orderType)
                {
                    case 0:
                        statusName = "暂存";
                        break;
                    case 1:
                        statusName = "已过期";
                        break;
                    case 3:
                        statusName = "被踢回";
                        break;
                    case 4:
                        statusName = "进行中";
                        break;
                    case 41:
                        statusName = "待支付";
                        break;
                    case 42:
                        statusName = "待承保";
                        break;
                }
                return orderType;
            }
        }
    }
}
