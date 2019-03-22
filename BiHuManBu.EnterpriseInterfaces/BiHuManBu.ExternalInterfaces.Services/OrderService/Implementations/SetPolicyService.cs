using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Repository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.OrderService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;
using log4net;
using ServiceStack.Text;

namespace BiHuManBu.ExternalInterfaces.Services.OrderService.Implementations
{
    public class SetPolicyService : ISetPolicyService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapEntityService _mapEntityService;
        private readonly ISubmitInfoRepository _submitInfoRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IQuoteReqCarInfoService _quoteReqCarInfoService;
        private static readonly ILog LogErro = LogManager.GetLogger("ERROR");
        private readonly ISetSaveCommissionService _setSaveCommissionService;

        public SetPolicyService(IOrderRepository orderRepository, IMapEntityService mapEntityService, ISubmitInfoRepository submitInfoRepository, IUserInfoRepository userInfoRepository, IQuoteReqCarInfoService quoteReqCarInfoService, ISetSaveCommissionService setSaveCommissionService)
        {
            _orderRepository = orderRepository;
            _mapEntityService = mapEntityService;
            _submitInfoRepository = submitInfoRepository;
            _userInfoRepository = userInfoRepository;
            _quoteReqCarInfoService = quoteReqCarInfoService;
            _setSaveCommissionService = setSaveCommissionService;
        }

        public BaseViewModel SetPolicyAllInfo(FindByTnoQueueInfoResponse request, TPolicy tPolicy)
        {
            string statusMessage = "";

            //获取渠道
            var sourceValue = SourceGroupAlgorithm.GetOldSource(request.Source);

            //获取用户信息
            var userinfo = _userInfoRepository.FindByBuid(request.Buid);

            //获取订单信息
            var order = _orderRepository.FindOrder(tPolicy.OrderNum);

            //获取核保信息
            var submitInfo = _submitInfoRepository.GetSubmitInfoAsync(order.b_uid, order.source).Result;

            //更新订单信息
            statusMessage = statusMessage + (UpdateOrder(request, submitInfo, userinfo, order, tPolicy) == -1 ? "订单信息更新异常！," : "");

            //当订单为已支付（即：完成状态） 重新定义佣金积分
            if (request.order.insurance_company_pay_status == 1)
            {
                //设置佣金积分
                _setSaveCommissionService.Save(tPolicy.OrderNum, 1);
            }

            //获取订单险种信息
            var orderQuoteresult = _orderRepository.FindDdOrderQuoteresultAsync(order.id).Result;

            //更新险种信息
            statusMessage = statusMessage + (UpdateQuoteresult(request, orderQuoteresult) == -1 ? "险种信息更新异常！," : "");

            //获取保险费用信息
            var orderSaveQuote = _orderRepository.FindDdOrderSavequote(order.id);

            //更新保险费用信息
            statusMessage = statusMessage + (UpdateSaveQuote(request, orderSaveQuote) == -1 ? "保险费用信息更新异常！," : "");

            //获取关系人信息
            var orderRelatedInfo = _orderRepository.FindDdOrderRelatedinfo(order.id);

            //更新关系人信息
            statusMessage = statusMessage + (UpdateOrderRelatedInfo(request, orderRelatedInfo) == -1 ? "关系人信息更新异常！," : "");

            //获取全款支付信息
            var payResult = _orderRepository.GetOrderPayResult(order.order_num).FirstOrDefault(x => x.type == 1);

            //更新全款支付信息
            statusMessage = statusMessage + (UpdatePayResult(request, payResult, order) == -1 ? "支付信息更新异常！," : "");

            //返回结果
            return new BaseViewModel()
            {
                BusinessStatus = statusMessage == "" ? 1 : -10003,
                StatusMessage = statusMessage == "" ? "操作成功！" : statusMessage
            };
        }
       

        private int UpdateOrder(FindByTnoQueueInfoResponse request, bx_submit_info submitInfo, bx_userinfo userinfo, dd_order order, TPolicy tPolicy)
        {
            try
            {
                // *平台来源 1crm2微信4app8对外16抓单 对应的是创建订单的平台来源
                const int fromMethod = 16;
                var orderNum = _mapEntityService.GenerateOrderNum(fromMethod);

                //订单有效截止时间
                //当报价的交强开始时间大于当前时间就按照交强开始时间加3天作为截止时间
                var issueTime = request.quoteresult.ForceStartDate;//出单时间
                var orderLapsetime = issueTime > DateTime.Now ? issueTime.AddDays(3) : DateTime.Now.AddDays(3);

                #region 订单跟新值

                order.biz_tno = request.order.BiztNo;
                order.force_tno = request.order.ForcetNo;
                order.update_time = DateTime.Now;
                order.order_lapse_time = orderLapsetime;
                order.issue_time = submitInfo == null ? DateTime.MinValue : submitInfo.create_time ?? DateTime.Now;//核保录入时间
                order.insure_year = submitInfo == null
                    ? DateTime.MinValue.Year
                    : (submitInfo.create_time ?? DateTime.Now).Year;
                order.MoldName = request.order.MoldName;
                order.get_order_method = 1;//1电子保单2纸质保单 //投保地区是北京的是电子保单
                order.total_amount = request.quoteresult.ForceTotal + request.quoteresult.BizTotal + request.quoteresult.TaxTotal; //总计金额
                //应收金额
                var receivableAmount = (request.quoteresult.ForceTotal *
                    (1- Convert.ToDouble(order.force_rate.HasValue ? order.force_rate.Value/100 : 0))) +
                                       request.quoteresult.BizTotal *
                                      (1- Convert.ToDouble(order.biz_rate.HasValue ? order.biz_rate.Value/100 : 0)) + request.quoteresult.TaxTotal;
                order.receivable_amount = receivableAmount;  //应收金额
                order.purchase_amount = receivableAmount;  //实收金额
                //LastBizEndDate = string.IsNullOrWhiteSpace(request.LastBizEndDate) ? DateTime.MinValue : Convert.ToDateTime(request.LastBizEndDate),
                //LastForceEndDate = string.IsNullOrWhiteSpace(request.LastForceEndDate) ? DateTime.MinValue : Convert.ToDateTime(request.LastForceEndDate),
                order.pay_type = request.order.pay_type;
                order.verification_code_status = request.order.verification_code_status;
                //当支付方式为微信的时候  设置默认支付银行  2018-01-24  L
                if (request.order.pay_type == 0)
                {
                    var bank =  _orderRepository.FindBankWayId(Convert.ToInt32(userinfo.CityCode));
                    order.pay_way_id = bank != null ? bank.id : 0;
                }
                order.order_type = request.order.insurance_company_pay_status == 1 ? 5 : order.order_type;
                order.insurance_company_pay_status = request.order.insurance_company_pay_status;
                order.licenseno = request.order.LicenseNo;

                #endregion
                
                DataContextFactory.GetDataContext().dd_order.AddOrUpdate(order);
                int countOrder = DataContextFactory.GetDataContext().SaveChanges();
                return countOrder;
            }
            catch (Exception ex)
            {
                LogErro.Error("【抓单】跟订单信息异常，异常信息：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException); ;
                return -1;
            }
        }

        private static int UpdateQuoteresult(FindByTnoQueueInfoResponse request, dd_order_quoteresult orderQuoteresult)
        {
            try
            {
                if (orderQuoteresult != null && orderQuoteresult.Id > 0)
                {
                    orderQuoteresult.CheSun = request.quoteresult.CheSun;
                    orderQuoteresult.CheSunBE = request.savequote.CheSun;
                    orderQuoteresult.SanZhe = request.quoteresult.SanZhe;
                    orderQuoteresult.DaoQiang = request.quoteresult.DaoQiang;
                    orderQuoteresult.DaoQiangBE = request.savequote.DaoQiang;
                    orderQuoteresult.SiJi = request.quoteresult.SiJi;
                    orderQuoteresult.ChengKe = request.quoteresult.ChengKe;
                    orderQuoteresult.BoLi = request.quoteresult.BoLi;
                    orderQuoteresult.HuaHen = request.quoteresult.HuaHen;
                    orderQuoteresult.BuJiMianCheSun = request.quoteresult.BuJiMianCheSun;
                    orderQuoteresult.BuJiMianDaoQiang = request.quoteresult.BuJiMianDaoQiang;
                    orderQuoteresult.BuJiMianRenYuan = request.quoteresult.BuJiMianRenYuan;
                    orderQuoteresult.BuJiMianSiJi = request.quoteresult.BuJiMianSiji;
                    orderQuoteresult.BuJiMianChengKe = request.quoteresult.BuJiMianChengKe;
                    orderQuoteresult.BuJiMianHuaHen = request.quoteresult.BuJiMianHuaHen;
                    orderQuoteresult.BuJiMianZiRan = request.quoteresult.BuJiMianZiRan;
                    orderQuoteresult.BuJiMianSheShui = request.quoteresult.BuJiMianSheShui;
                    orderQuoteresult.BuJiMianFuJian = request.quoteresult.BuJiMianFuJian;
                    orderQuoteresult.TeYue = request.quoteresult.TeYue;
                    orderQuoteresult.SheShui = request.quoteresult.SheShui;
                    orderQuoteresult.CheDeng = request.quoteresult.CheDeng;
                    orderQuoteresult.ZiRan = request.quoteresult.ZiRan;
                    orderQuoteresult.ZiRanBE = request.savequote.ZiRan;

                    


                    orderQuoteresult.BizTotal = request.quoteresult.BizTotal;
                    orderQuoteresult.ForceTotal = request.quoteresult.ForceTotal;
                    orderQuoteresult.TaxTotal = request.quoteresult.TaxTotal;
                    orderQuoteresult.SavedAmount = request.quoteresult.SavedAmount;
                    orderQuoteresult.BizStartDate = request.quoteresult.BizStartDate;
                    orderQuoteresult.ForceStartDate = request.quoteresult.ForceStartDate;
                    orderQuoteresult.HcSheBeiSunshi = request.quoteresult.HcSheBeiSunshi;
                    orderQuoteresult.HcHuoWuZeRen = request.quoteresult.HcHuoWuZeRen;
                    orderQuoteresult.HcFeiYongBuChang = request.quoteresult.HcFeiYongBuChang;

                    orderQuoteresult.HcFeiYongBuChangDays = request.quoteresult.FeiYongBuChangDays;


                    //orderQuoteresult.she = request.quoteresult.HcSheBeiSunshiInfo.SheBeiSunShi;
                    //orderQuoteresult.san = request.quoteresult.SanFangTeYue;
                    //orderQuoteresult.jing = request.quoteresult.JingShenSunShi;

                    orderQuoteresult.HcJingShenSunShi = request.quoteresult.HcJingShenSunShi;
                    orderQuoteresult.HcSanFangTeYue = request.quoteresult.HcSanFangTeYue;
                    orderQuoteresult.HcXiuLiChang = request.quoteresult.HcXiuLiChang;

                    orderQuoteresult.HcXiuLiChangType = Convert.ToInt32(request.quoteresult.HcXiuLiChangType);

                    orderQuoteresult.BuJiMianSheBeiSunshi = request.quoteresult.BuJiMianHcSheBeiSunshi;
                    orderQuoteresult.BuJiMianSanZhe = request.quoteresult.BuJiMianSanZhe;
                    orderQuoteresult.BuJiMianJingShenSunShi = request.quoteresult.BuJiMianJingShenSunShi;
                    orderQuoteresult.PingAnScore = request.quoteresult.PingAnScore;

                    //orderQuoteresult.lo = request.quoteresult.ForceExpectedLossRate; //找龙哥对一下

                    orderQuoteresult.biz_expected_loss_rate = request.quoteresult.BizExpectedLossRate;
                    orderQuoteresult.RateFactor1 = Convert.ToDecimal(request.quoteresult.NonClaimRate);
                    orderQuoteresult.RateFactor2 = Convert.ToDecimal(request.quoteresult.MultiDiscountRate);
                    orderQuoteresult.RateFactor3 = Convert.ToDecimal(request.quoteresult.AvgMileRate);
                    orderQuoteresult.RateFactor4 = Convert.ToDecimal(request.quoteresult.RiskRate);
                    orderQuoteresult.TotalRate = Convert.ToDecimal(request.quoteresult.TotalRate);

                    orderQuoteresult.BizStartDate = request.order.BizStartTime;
                    orderQuoteresult.ForceStartDate = request.order.ForceStartTime;

                    #region 新增字段修改的
                    //userinfo.MoldName = request.order.MoldName;
                    orderQuoteresult.RegisterDate = request.order.RegisterDate.ToString("yyyy-MM-dd HH:mm:ss");
                    orderQuoteresult.CarVIN = request.order.CarVin;
                    orderQuoteresult.EngineNo = request.order.EngineNo;

                    orderQuoteresult.biz_pno = request.order.BiztNo;
                    orderQuoteresult.biz_end_time = request.order.BizEndTime;
                    
                    orderQuoteresult.force_end_time = request.order.BizEndTime;
                    orderQuoteresult.biz_pno = request.order.BizpNo;
                    orderQuoteresult.force_pno = request.order.ForcepNo;
                    orderQuoteresult.quote_status = 1;
                    orderQuoteresult.submit_status = 1;
                    orderQuoteresult.submit_result = "核保成功";
                    orderQuoteresult.quote_result = "报价成功";

                    orderQuoteresult.auto_model_code = request.order.AutoModelCode;
                    orderQuoteresult.car_used_type = request.order.car_used_type.HasValue
                        ? Convert.ToInt32(request.order.car_used_type)
                        : -1;
                    orderQuoteresult.seat_count = string.IsNullOrWhiteSpace(request.order.SeatCount) ? -1 : Convert.ToInt32(request.order.SeatCount);
                    orderQuoteresult.PriceT = Convert.ToDecimal(request.order.PurchasePrice);


                    #endregion
                   

                    //更新
                    DataContextFactory.GetDataContext().dd_order_quoteresult.AddOrUpdate(orderQuoteresult);
                    int quoteResult = DataContextFactory.GetDataContext().SaveChanges();
                    return quoteResult;
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogErro.Error("【抓单】更新险种信息异常，异常信息：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return -1;
            }
        }

        private static int UpdateSaveQuote(FindByTnoQueueInfoResponse request, dd_order_savequote orderSavequote)
        {
            try
            {
                if (orderSavequote != null && orderSavequote.Id > 0)
                {
                    orderSavequote.CheSun = request.savequote.CheSun;
                    orderSavequote.SanZhe = request.savequote.SanZhe;
                    orderSavequote.DaoQiang = request.savequote.DaoQiang;
                    orderSavequote.SiJi = request.savequote.SiJi;
                    orderSavequote.ChengKe = request.savequote.ChengKe;
                    orderSavequote.BoLi = request.savequote.BoLi;
                    orderSavequote.HuaHen = request.savequote.HuaHen;
                    orderSavequote.BuJiMianCheSun = request.savequote.BuJiMianCheSun;
                    orderSavequote.BuJiMianSanZhe = request.savequote.BuJiMianSanZhe;
                    orderSavequote.BuJiMianDaoQiang = request.savequote.BuJiMianDaoQiang;
                    orderSavequote.BuJiMianRenYuan = request.savequote.BuJiMianRenYuan;
                    orderSavequote.BuJiMianSiJi = request.savequote.BuJiMianSiji;
                    orderSavequote.BuJiMianChengKe = request.savequote.BuJiMianChengKe;
                    orderSavequote.BuJiMianHuaHen = request.savequote.BuJiMianHuaHen;
                    orderSavequote.BuJiMianZiRan = request.savequote.BuJiMianZiRan;
                    orderSavequote.BuJiMianSheShui = request.savequote.BuJiMianSheShui;
                    orderSavequote.BuJiMianFuJian = request.savequote.BuJiMianFuJian;
                    orderSavequote.TeYue = request.savequote.TeYue;
                    orderSavequote.SheShui = request.savequote.SheShui;
                    orderSavequote.CheDeng = request.savequote.CheDeng;
                    orderSavequote.ZiRan = request.savequote.ZiRan;

                    //orderSavequote.biz = request.savequote.BizTotal;  暂定20180112
                    orderSavequote.BuJiMianSheBeiSunshi = request.savequote.BuJiMianHcSheBeiSunshi;  //20180125新增

                    orderSavequote.IsRenewal = request.savequote.IsRenewal;
                    orderSavequote.BizStartDate = request.savequote.BizStartDate;
                    orderSavequote.JiaoQiang = request.savequote.JiaoQiang;
                    orderSavequote.HcSheBeiSunshi = request.savequote.HcSheBeiSunshi;
                    orderSavequote.HcHuoWuZeRen = request.savequote.HcHuoWuZeRen;
                    orderSavequote.HcFeiYongBuChang = request.savequote.HcFeiYongBuChang;
                    orderSavequote.FeiYongBuChangDays = request.savequote.FeiYongBuChangDays;
                    orderSavequote.HcJingShenSunShi = request.savequote.HcJingShenSunShi;
                    orderSavequote.HcSanFangTeYue = request.savequote.HcSanFangTeYue;
                    orderSavequote.HcXiuLiChang = request.savequote.HcXiuLiChang;
                    orderSavequote.HcXiuLiChangType = Convert.ToInt32(request.savequote.HcXiuLiChangType);
                    orderSavequote.SheBeiSunShiConfig = request.savequote.HcSheBeiSunshiInfo.ToJson();
                    orderSavequote.BuJiMianJingShenSunShi = request.savequote.BuJiMianJingShenSunShi;
                    orderSavequote.co_real_value = request.savequote.co_real_value;

                    //更新
                    DataContextFactory.GetDataContext().dd_order_savequote.AddOrUpdate(orderSavequote);
                    int countOrderSavequote = DataContextFactory.GetDataContext().SaveChanges();
                    return countOrderSavequote;
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogErro.Error("【抓单】更新保费信息异常，异常信息：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return -1;
            }
        }

        private static int UpdateOrderRelatedInfo(FindByTnoQueueInfoResponse request, dd_order_related_info orderRelatedInfo)
        {
            try
            {
                if (orderRelatedInfo != null && orderRelatedInfo.id > 0)
                {
                    #region 被保险人信息
                    orderRelatedInfo.insured_name = request.related.InsuredName;
                    //orderRelatedInfo.insured_mobile = request.related.InsuredMobile;
                    orderRelatedInfo.insured_id_card = request.related.InsuredIdCard;
                    orderRelatedInfo.insured_address = request.related.InsuredAddress;
                    orderRelatedInfo.insured_id_type = request.related.InsuredIdType;
                    orderRelatedInfo.insured_certi_start_date = string.IsNullOrWhiteSpace(request.related.InsuredCertiStartdate) ? DateTime.MinValue : Convert.ToDateTime(request.related.InsuredCertiStartdate);
                    orderRelatedInfo.insured_certi_end_date = string.IsNullOrWhiteSpace(request.related.InsuredCertiEnddate) ? DateTime.MinValue : Convert.ToDateTime(request.related.InsuredCertiEnddate);
                    orderRelatedInfo.insured_email = request.related.InsuredEmail;
                    orderRelatedInfo.insured_sex = request.related.InsuredSex;
                    orderRelatedInfo.insured_nation = request.related.InsuredNation;
                    orderRelatedInfo.insured_birthday = string.IsNullOrWhiteSpace(request.related.InsuredBirthday) ? DateTime.MinValue : Convert.ToDateTime(request.related.InsuredBirthday);
                    orderRelatedInfo.insured_authority = request.related.InsuredIssuer;//签发机关
                    #endregion
                    #region  投保人信息
                    orderRelatedInfo.holder_name = request.related.HolderName;
                    orderRelatedInfo.holder_id_card = request.related.HolderIdCard;
                    //orderRelatedInfo.holder_mobile = request.related.HolderMobile;
                    orderRelatedInfo.holder_address = request.related.HolderAddress;
                    orderRelatedInfo.holder_id_type = request.related.HolderIdType;
                    orderRelatedInfo.holder_certi_start_date = string.IsNullOrWhiteSpace(request.related.HolderCertiStartdate) ? DateTime.MinValue : Convert.ToDateTime(request.related.HolderCertiStartdate);
                    orderRelatedInfo.holder_certi_end_date = string.IsNullOrWhiteSpace(request.related.HolderCertiEnddate) ? DateTime.MinValue : Convert.ToDateTime(request.related.HolderCertiEnddate);

                    orderRelatedInfo.holder_email = request.related.HolderEmail;
                    orderRelatedInfo.holder_sex = request.related.HolderSex;
                    orderRelatedInfo.holder_nation = request.related.HolderNation;
                    orderRelatedInfo.holder_birthday = string.IsNullOrWhiteSpace(request.related.HolderBirthday) ? DateTime.MinValue : Convert.ToDateTime(request.related.HolderBirthday);
                    orderRelatedInfo.holder_authority = request.related.HolderIssuer;
                    #endregion
                    #region 车主信息
                    orderRelatedInfo.ower_name = request.related.LicenseOwner;
                    //orderRelatedInfo.ower_mobile = request.related.Mobile;
                    orderRelatedInfo.ower_id_card = request.related.IdCard;
                    orderRelatedInfo.ower_id_type = request.related.OwnerIdCardType;
                    orderRelatedInfo.ower_certi_start_date = string.IsNullOrWhiteSpace(request.related.OwnerCertiStartdate) ? DateTime.MinValue : Convert.ToDateTime(request.related.OwnerCertiStartdate);
                    orderRelatedInfo.ower_certi_end_date = string.IsNullOrWhiteSpace(request.related.OwnerCertiEnddate) ? DateTime.MinValue : Convert.ToDateTime(request.related.OwnerCertiEnddate);
                    orderRelatedInfo.ower_address = request.related.OwnerCertiAddress;
                    orderRelatedInfo.ower_email = request.related.email;
                    orderRelatedInfo.ower_sex = request.related.OwnerCertiSex;
                    orderRelatedInfo.ower_nation = request.related.OwnerCertiNation;
                    orderRelatedInfo.ower_birthday = string.IsNullOrWhiteSpace(request.related.OwnerCertiBirthday) ? DateTime.MinValue : Convert.ToDateTime(request.related.OwnerCertiBirthday);
                    orderRelatedInfo.ower_authority = request.related.OwnerCertiIssuer;
                    #endregion

                    //更新
                    DataContextFactory.GetDataContext().dd_order_related_info.AddOrUpdate(orderRelatedInfo);
                    int countOrderRelatedInfo = DataContextFactory.GetDataContext().SaveChanges();
                    return countOrderRelatedInfo;
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogErro.Error("【抓单】更新关系人信息异常，异常信息：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return -1;
            }
        }

        private static int UpdatePayResult(FindByTnoQueueInfoResponse request, dd_order_paymentresult paymentresult, dd_order order)
        {
            try
            {
                if ((!string.IsNullOrWhiteSpace(request.order.BizpNo) || !string.IsNullOrWhiteSpace(request.order.ForcepNo)) && paymentresult != null)
                {
                    paymentresult.biz_no = request.order.BizpNo;
                    paymentresult.force_no = request.order.ForcepNo;
                    paymentresult.payment_bizt_time = request.quoteresult.PaymentBizTime;
                    paymentresult.payment_force_time = request.quoteresult.PaymentForceTime;
                    paymentresult.payment_time = request.quoteresult.PaymentBizTime > request.quoteresult.PaymentForceTime
                        ? request.quoteresult.PaymentBizTime
                        : request.quoteresult.PaymentForceTime;
                    paymentresult.pay_source = 4;//抓单
                    paymentresult.find_pay_result = 1;

                    DataContextFactory.GetDataContext().dd_order_paymentresult.AddOrUpdate(paymentresult);
                    int countOrderPaymentresult = DataContextFactory.GetDataContext().SaveChanges();
                    return countOrderPaymentresult;
                }
                else
                {
                    var newPaymentresult = new dd_order_paymentresult();
                    newPaymentresult.money = request.quoteresult.ForceTotal + request.quoteresult.BizTotal;
                    newPaymentresult.licenseNo = request.order.LicenseNo;
                    newPaymentresult.pay_source = 4;//抓单
                    newPaymentresult.find_pay_result = 0;
                    newPaymentresult.order_num = order.order_num;
                    newPaymentresult.order_id = Convert.ToInt32(order.id);
                    newPaymentresult.create_time = DateTime.Now;

                    DataContextFactory.GetDataContext().dd_order_paymentresult.Add(newPaymentresult);
                    int countOrderPaymentresult = DataContextFactory.GetDataContext().SaveChanges();
                    return countOrderPaymentresult;
                }
            }
            catch (Exception ex)
            {
                LogErro.Error("【抓单】更新支付信息异常，异常信息：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return -1;
            }
        }
    }
}
