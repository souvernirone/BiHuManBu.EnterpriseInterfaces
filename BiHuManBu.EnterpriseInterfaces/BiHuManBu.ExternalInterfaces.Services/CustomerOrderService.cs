using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.AppIRepository;
using BiHuManBu.ExternalInterfaces.Models.Enums;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Services.BindBusiuser;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using log4net;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using IAgentConfigRepository = BiHuManBu.ExternalInterfaces.Models.IAgentConfigRepository;
using IAgentRepository = BiHuManBu.ExternalInterfaces.Models.IAgentRepository;
using IOrderRepository = BiHuManBu.ExternalInterfaces.Models.IOrderRepository;
using IUserInfoRepository = BiHuManBu.ExternalInterfaces.Models.IUserInfoRepository;
using BiHuManBu.ExternalInterfaces.Services.Implements;
using IAgentService = BiHuManBu.ExternalInterfaces.Services.Interfaces.IAgentService;
using Qiniu.Util;
using Qiniu.Storage;
using System.Security.Cryptography;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class CustomerOrderService : CommonBehaviorService, ICustomerOrderService
    {
        /// <summary>
        /// 采集系统的账号密码salt
        /// </summary>
        private const string UQ = "(*HY53U&3#+-34MF&`\t\"&gt;";


        private IUserClaimRepository _userClaimRepository;
        private IOrderRepository _orderRepository;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private const string logSeparator = "\r\n---------------------------------------------------------------------------------------- \r\n";
        private readonly ILog logErro = LogManager.GetLogger("ERROR");
        private readonly ILog logMsg = LogManager.GetLogger("MSG");
        private IAgentRepository agentRepository;
        private IUserInfoRepository _userInfoRepository;
        private ISaveQuoteRepository _saveQuoteRepository;
        private IQuoteResultRepository _quoteResultRepository;
        private ISubmitInfoRepository _submitInfoRepository;
        private IQuoteResultCarinfoRepository _carinfoRepository;
        private IQuoteReqCarInfoRepository _quoteReqCarInfo;
        private IAgentService _agentService;
        private ICustomerTopLevelService _customerTopLevelService;
        private ICarInfoRepository _iCarInfoRepository;
        private IAddressRepository _iAddressRepository;
        private IAgentConfigRepository _agentConfigRepository;
        private readonly IAgentDistributedRepository _agentDistributedRepository;
        private readonly IOrderService _orderService;
        private readonly IConsumerDetailService _consumerDetailService;
        private ICheckCanBindBusiuserListService _checkCanBindBusiuserListService;
        private readonly IPushMessageService _pushMessageService;
        private readonly IEpolicyRepository _epolicyRepository;
        private readonly ISetSaveCommissionService _setSaveCommissionService;
        private readonly IOrderCommissionRepository _orderCommissionRepository;
        private readonly IUpdateCompleteTaskService _updateCompleteTaskService;
        private readonly IAgentAnswernCityRepository _agentAnswernCityReposity;
        private readonly IGetCommissionIntegralService _getCommissionIntegralService;
        private readonly IOrderPostThirdService _orderPostThirdService;

        public CustomerOrderService(IAgentConfigRepository agentConfigRepository, IAddressRepository AddressRepository,
            ICarInfoRepository CarInfoRepository, ICustomerTopLevelService customerustomerTopLevelService,
            IAgentRepository agentRepository, ICacheHelper cacheHelper, IOrderRepository orderRepository,
            IUserInfoRepository userInfoRepository, ISaveQuoteRepository saveQuoteRepository,
            ISubmitInfoRepository submitInfoRepository, IQuoteResultCarinfoRepository carinfoRepository,
            IQuoteResultRepository quoteResultRepository, IQuoteReqCarInfoRepository quoteReqCarInfo,
            IAgentService agentService, Models.IRepository.IAgentDistributedRepository agentDistributedRepository,
           IOrderService orderService, IConsumerDetailService consumerDetailService,
ICheckCanBindBusiuserListService checkCanBindBusiuserListService, IUserClaimRepository userClaimRepository, IPushMessageService pushMessageService, IEpolicyRepository epolicyRepository,
ISetSaveCommissionService setSaveCommissionService, IOrderCommissionRepository orderCommissionRepository, IUpdateCompleteTaskService updateCompleteTaskService,
IAgentAnswernCityRepository agentAnswernCityReposity, IGetCommissionIntegralService getCommissionIntegralService, IOrderPostThirdService orderPostThirdService)//
            : base(agentRepository, cacheHelper)
        {
            _agentConfigRepository = agentConfigRepository;
            _iAddressRepository = AddressRepository;
            _iCarInfoRepository = CarInfoRepository;
            _userInfoRepository = userInfoRepository;
            _orderRepository = orderRepository;
            _saveQuoteRepository = saveQuoteRepository;
            _submitInfoRepository = submitInfoRepository;
            _quoteResultRepository = quoteResultRepository;
            _carinfoRepository = carinfoRepository;
            _quoteReqCarInfo = quoteReqCarInfo;
            this.agentRepository = agentRepository;
            _agentService = agentService;
            _customerTopLevelService = customerustomerTopLevelService;
            _agentDistributedRepository = agentDistributedRepository;
            _orderService = orderService;
            _consumerDetailService = consumerDetailService;
            _checkCanBindBusiuserListService = checkCanBindBusiuserListService;
            _userClaimRepository = userClaimRepository;
            _pushMessageService = pushMessageService;
            _epolicyRepository = epolicyRepository;
            _setSaveCommissionService = setSaveCommissionService;
            _orderCommissionRepository = orderCommissionRepository;
            _updateCompleteTaskService = updateCompleteTaskService;
            _agentAnswernCityReposity = agentAnswernCityReposity;
            _getCommissionIntegralService = getCommissionIntegralService;
            _orderPostThirdService = orderPostThirdService;
        }

        /// <summary>
        /// 验证是否下过单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IsValidOrderViewModel IsValidOrderByDrivingInfo(IsValidOrderByDrivingInfoRequest request)
        {
            //List<AgentNameViewModel> agent = _userInfoRepository.IsHaveLicensenoList(request.Agent, request.ChildAgent, request.LicenseNo, request.CarVin, request.TypeId, false);
            //List<long> buids1 = agent.Select(i => i.Buid).ToList();

            //2018-01-26  L  优化
            var listAgent = _agentService.GetSonsListFromRedisToString(request.Agent);
            var buids = _userInfoRepository.GetListUseridByAgentIds("'" + string.Join("','", listAgent) + "'", request.LicenseNo, request.CarVin, request.TypeId);

            dd_order order = _orderRepository.FindOrderNum(buids);
            string orderNum = string.Empty;
            int orderType = 0;
            string statusName = string.Empty;
            if (order != null)
            {
                orderNum = order.order_num;
                orderType = GetOrderStatus(order.order_type, order.order_lapse_time, out statusName);
            }
            return new IsValidOrderViewModel()
            {
                OrderNum = orderNum,
                OrderType = orderType,
                OrderStatus = statusName,
                BusinessStatus = 1,
                StatusMessage = "获取成功"
            };
        }
        /// <summary>
        /// 验证是否下过单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IsValidOrderViewModel IsValidOrder(IsValidOrderRequest request)
        {
            dd_order order = _orderRepository.FindOrderNum(request.Buid);
            string orderNum = string.Empty;
            int orderType = 0;
            string statusName = string.Empty;
            if (order != null)
            {
                orderNum = order.order_num;
                orderType = GetOrderStatus(order.order_type, order.order_lapse_time, out statusName);
            }
            return new IsValidOrderViewModel()
            {
                OrderNum = orderNum,
                OrderType = orderType,
                OrderStatus = statusName,
                BusinessStatus = 1,
                StatusMessage = "获取成功"
            };
        }
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetOrderDetailViewModel GetOrderDetail(GetOrderDetailRequest request)
        {
            //获取订单
            dd_order order = _orderRepository.FindOrder(request.OrderNum);
            if (order == null)
            {
                return new GetOrderDetailViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "订单获取失败"
                };
            }
            bx_userinfo userinfo = _userInfoRepository.FindByBuid(order.b_uid);
            if (userinfo == null)
            {
                return new GetOrderDetailViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "订单获取失败"
                };
            }

            PayWayBanksModel bank = null;
            if (order.pay_way_id > 0)
            {
                bank = _orderRepository.GetPayWayBank(order.pay_way_id);
            }

            //初始化模型
            GetOrderDetailViewModel viewModel = new GetOrderDetailViewModel();
            //初始化userinfo记录的2个字段
            viewModel.CurAgent = userinfo.Agent;
            viewModel.CurOpenId = userinfo.OpenId;
            #region dd_order模型转换
            string statusname = string.Empty;
            viewModel.Order = new OrderDetail()
            {
                Id = order.id,
                OrderNum = order.order_num,
                OrderGuid = order.order_guid,
                DeliveryMethod = order.delivery_method,
                DeliveryAddress = order.delivery_address ?? "",
                DeliveryContacts = order.delivery_contacts ?? "",
                DeliveryContactsMobile = order.delivery_contacts_mobile ?? "",
                PayType = order.pay_type,
                Buid = order.b_uid,
                UpdateTime = order.update_time.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateTime = order.create_time.ToString("yyyy-MM-dd HH:mm:ss"),
                AgentId = order.agent_id,
                AgentName = order.agent_name ?? "",
                IssuingPeopleId = order.issuing_people_id ?? 0,
                IssuingPeopleName = order.issuing_people_name ?? "",
                DeliveryType = order.delivery_type ?? -1,
                Remark = order.remark ?? "",
                OrderType = GetOrderStatus(order.order_type, order.order_lapse_time, out statusname),
                //订单失效时间为29天以后，创建的时候已赋值。
                OrderLapseTime = order.order_lapse_time.HasValue ? order.order_lapse_time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                //采集失效时间为当天的23点59分59秒//去掉采集失效的判断
                VerificationCodeCreateTime = DateTime.MaxValue.ToString("yyyy-MM-dd HH:mm:ss"),// order.verification_code_create_time.HasValue ? order.verification_code_create_time.Value.ToString("yyyy-MM-dd") + " 23:59:59" : "",
                //二维码失效时间是2个小时，10点以后不允许生成二维码，前端和院院需要触发的时候判断
                PayCodeCreateTime = order.pay_code_create_time.HasValue ? order.pay_code_create_time.Value.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss") : "",
                IssueTime = order.issue_time.ToString("yyyy-MM-dd HH:mm:ss"),
                PayCodeUrl = order.pay_code_url ?? "",
                Licenseno = order.licenseno,
                Source = SourceGroupAlgorithm.GetNewSource(order.source),//待转换
                ConsumerPayStatus = order.consumer_pay_status,
                InsuranceCompanyPayStatus = order.insurance_company_pay_status,
                //采集状态在下文有单独的处理//单独的处理已去掉，不做判断
                VerificationCodeStatus = order.verification_code_status,
                QuoteCityId = order.quote_city_id,
                QuoteConfigId = order.quote_config_id,
                QuoteConfigName = agentRepository.GetUkeyName(order.quote_config_id) ?? "",
                ForceTno = order.force_tno ?? "",
                BizTno = order.biz_tno ?? "",
                InsureYear = order.insure_year,
                Preferential = order.preferential ?? "",
                GetOrderMethod = order.get_order_method,
                TotalAmount = order.total_amount.HasValue ? order.total_amount.Value : 0,
                ReceivableAmount = order.receivable_amount.HasValue ? order.receivable_amount.Value : 0,
                PurchaseAmount = order.purchase_amount.HasValue ? order.purchase_amount.Value : 0,
                ForceRate = order.force_rate.HasValue ? order.force_rate.Value : 0,
                BizRate = order.biz_rate.HasValue ? order.biz_rate.Value : 0,
                DetailPageType = GetDetailPageType(request.ChildAgent, order.agent_id, order.issuing_people_id ?? 0),//order.issuing_people_id.HasValue ? (order.issuing_people_id == order.agent_id ? 0 : 1) : 1,
                IsChangeRelated = userinfo.IsChangeRelation == 1 ? 1 : 0,
                LastBizEndDate = order.LastBizEndDate.HasValue ? order.LastBizEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                LastForceEndDate = order.LastForceEndDate.HasValue ? order.LastForceEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                Payee = order.payee,
                PayWayId = order.pay_way_id,
                PayWayBankName = bank == null ? "" : bank.BankName
            };
            // 身份证采集状态
            //viewModel.Order.VerificationCodeStatus = GetIdCardStatus(viewModel.Order.VerificationCodeStatus, order.verification_code_create_time);

            #endregion
            dd_order_quoteresult quoteresult = _orderRepository.FindDdOrderQuoteresultAsync(order.id).Result;
            dd_order_savequote savequote = _orderRepository.FindDdOrderSavequoteAsync(order.id).Result;
            bx_submit_info submitinfo = _submitInfoRepository.GetSubmitInfoAsync(order.b_uid, order.source).Result;
            #region dd_order_quoteresult、dd_order_savequote模型转换
            if (savequote != null && quoteresult != null)
            {
                viewModel.Order.BizStartDate = "";
                if (quoteresult.BizStartDate.HasValue && quoteresult.BizStartDate.Value.Date != DateTime.MinValue)
                {
                    viewModel.Order.BizStartDate = quoteresult.BizStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                viewModel.Order.ForceStartDate = "";
                if (quoteresult.ForceStartDate.HasValue && quoteresult.ForceStartDate.Value.Date != DateTime.MinValue)
                {
                    viewModel.Order.ForceStartDate = quoteresult.ForceStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                bool isquoteresult = true;
                viewModel.PrecisePrice = new PrecisePrice()
                {
                    BizClaimCount = quoteresult.biz_claim_count,
                    ForceClaimCount = quoteresult.force_claim_count,
                    ExpectedLossRate = quoteresult.expected_loss_rate,
                    BizExpectedLossRate = quoteresult.biz_expected_loss_rate,
                    //BizRate = (double)(submitInfo.biz_rate.HasValue ? submitInfo.biz_rate.Value : 0),
                    //ForceRate = (double)(submitInfo.force_rate.HasValue ? submitInfo.force_rate.Value : 0),
                    BizTotal = quoteresult.BizTotal.HasValue ? quoteresult.BizTotal.Value : 0,
                    ForceTotal = quoteresult.ForceTotal ?? 0,
                    TaxTotal = quoteresult.TaxTotal ?? 0,
                    Source = SourceGroupAlgorithm.GetNewSource(order.source),
                    QuoteStatus = order.order_type == 0 ? (submitinfo != null ? (submitinfo.quote_status ?? 0) : 0) : 1,
                    QuoteResult = order.order_type == 0 ? (submitinfo != null ? (submitinfo.quote_result ?? "") : "") : "报价成功",
                    SubmitStatus = order.order_type == 0 ? (submitinfo != null ? (submitinfo.quote_status ?? 0) : 0) : 1,
                    SubmitResult = order.order_type == 0 ? (submitinfo != null ? (submitinfo.submit_result ?? "") : "") : "核保成功",
                    JiaoQiang = savequote.JiaoQiang.HasValue ? savequote.JiaoQiang.Value : 1,
                    CheSun = new XianZhongUnit
                    {
                        BaoE = isquoteresult
                            ? (quoteresult.CheSunBE.HasValue ? quoteresult.CheSunBE.Value : 0)
                            : (savequote.CheSun.HasValue ? savequote.CheSun.Value : 0),
                        BaoFei = quoteresult.CheSun.HasValue ? quoteresult.CheSun.Value : 0
                    },
                    SanZhe = new XianZhongUnit
                    {
                        BaoE = savequote.SanZhe.HasValue ? savequote.SanZhe.Value : 0,
                        BaoFei = quoteresult.SanZhe.HasValue ? quoteresult.SanZhe.Value : 0
                    },
                    DaoQiang = new XianZhongUnit
                    {
                        BaoE = isquoteresult
                            ? (quoteresult.DaoQiangBE.HasValue ? quoteresult.DaoQiangBE.Value : 0)
                            : (savequote.DaoQiang.HasValue ? savequote.DaoQiang.Value : 0),
                        BaoFei = quoteresult.DaoQiang.HasValue ? quoteresult.DaoQiang.Value : 0
                    },
                    SiJi = new XianZhongUnit
                    {
                        BaoE = savequote.SiJi.HasValue ? savequote.SiJi.Value : 0,
                        BaoFei = quoteresult.SiJi.HasValue ? quoteresult.SiJi.Value : 0
                    },
                    ChengKe = new XianZhongUnit
                    {
                        BaoE = savequote.ChengKe.HasValue ? savequote.ChengKe.Value : 0,
                        BaoFei = quoteresult.ChengKe.HasValue ? quoteresult.ChengKe.Value : 0
                    },
                    BoLi = new XianZhongUnit
                    {
                        BaoE = savequote.BoLi.HasValue ? savequote.BoLi.Value : 0,
                        BaoFei = quoteresult.BoLi.HasValue ? quoteresult.BoLi.Value : 0
                    },
                    HuaHen = new XianZhongUnit
                    {
                        BaoE = savequote.HuaHen.HasValue ? savequote.HuaHen.Value : 0,
                        BaoFei = quoteresult.HuaHen.HasValue ? quoteresult.HuaHen.Value : 0
                    },
                    BuJiMianCheSun = new XianZhongUnit
                    {
                        BaoE = savequote.BuJiMianCheSun.HasValue ? savequote.BuJiMianCheSun.Value : 0,
                        BaoFei = quoteresult.BuJiMianCheSun.HasValue ? quoteresult.BuJiMianCheSun.Value : 0
                    },
                    BuJiMianSanZhe = new XianZhongUnit
                    {
                        BaoE = savequote.BuJiMianSanZhe.HasValue ? savequote.BuJiMianSanZhe.Value : 0,
                        BaoFei = quoteresult.BuJiMianSanZhe.HasValue ? quoteresult.BuJiMianSanZhe.Value : 0
                    },
                    BuJiMianDaoQiang = new XianZhongUnit
                    {
                        BaoE = savequote.BuJiMianDaoQiang.HasValue ? savequote.BuJiMianDaoQiang.Value : 0,
                        BaoFei = quoteresult.BuJiMianDaoQiang.HasValue ? quoteresult.BuJiMianDaoQiang.Value : 0
                    },
                    BuJiMianRenYuan = new XianZhongUnit
                    {
                        BaoE = savequote.BuJiMianRenYuan.HasValue ? savequote.BuJiMianRenYuan.Value : 0,
                        BaoFei = quoteresult.BuJiMianRenYuan.HasValue ? quoteresult.BuJiMianRenYuan.Value : 0
                    },
                    BuJiMianFuJia = new XianZhongUnit
                    {
                        BaoE = savequote.BuJiMianFuJian.HasValue ? savequote.BuJiMianFuJian.Value : 0,
                        BaoFei = quoteresult.BuJiMianFuJian.HasValue ? quoteresult.BuJiMianFuJian.Value : 0
                    },
                    BuJiMianChengKe = new XianZhongUnit()
                    {
                        BaoE = savequote.BuJiMianChengKe.HasValue ? savequote.BuJiMianChengKe.Value : 0,
                        BaoFei = quoteresult.BuJiMianChengKe.HasValue ? quoteresult.BuJiMianChengKe.Value : 0
                    },
                    BuJiMianSiJi = new XianZhongUnit()
                    {
                        BaoE = savequote.BuJiMianSiJi.HasValue ? savequote.BuJiMianSiJi.Value : 0,
                        BaoFei = quoteresult.BuJiMianSiJi.HasValue ? quoteresult.BuJiMianSiJi.Value : 0
                    },
                    BuJiMianHuaHen = new XianZhongUnit()
                    {
                        BaoE = savequote.BuJiMianHuaHen.HasValue ? savequote.BuJiMianHuaHen.Value : 0,
                        BaoFei = quoteresult.BuJiMianHuaHen.HasValue ? quoteresult.BuJiMianHuaHen.Value : 0
                    },
                    BuJiMianSheShui = new XianZhongUnit()
                    {
                        BaoE = savequote.BuJiMianSheShui.HasValue ? savequote.BuJiMianSheShui.Value : 0,
                        BaoFei = quoteresult.BuJiMianSheShui.HasValue ? quoteresult.BuJiMianSheShui.Value : 0
                    },
                    BuJiMianZiRan = new XianZhongUnit()
                    {
                        BaoE = savequote.BuJiMianZiRan.HasValue ? savequote.BuJiMianZiRan.Value : 0,
                        BaoFei = quoteresult.BuJiMianZiRan.HasValue ? quoteresult.BuJiMianZiRan.Value : 0
                    },
                    BuJiMianJingShenSunShi = new XianZhongUnit()
                    {
                        BaoE = savequote.BuJiMianJingShenSunShi.HasValue ? savequote.BuJiMianJingShenSunShi.Value : 0,
                        BaoFei = quoteresult.BuJiMianJingShenSunShi.HasValue
                            ? quoteresult.BuJiMianJingShenSunShi.Value
                            : 0
                    },
                    HcSheBeiSunshi = new XianZhongUnit
                    {
                        BaoE = savequote.HcSheBeiSunshi.HasValue ? savequote.HcSheBeiSunshi.Value : 0,
                        BaoFei = quoteresult.HcSheBeiSunshi.HasValue ? quoteresult.HcSheBeiSunshi.Value : 0
                    },
                    HcHuoWuZeRen = new XianZhongUnit
                    {
                        BaoE = savequote.HcHuoWuZeRen.HasValue ? savequote.HcHuoWuZeRen.Value : 0,
                        BaoFei = quoteresult.HcHuoWuZeRen.HasValue ? quoteresult.HcHuoWuZeRen.Value : 0
                    },
                    HcFeiYongBuChang = new XianZhongUnit
                    {
                        BaoE = savequote.HcFeiYongBuChang.HasValue ? savequote.HcFeiYongBuChang.Value : 0,
                        BaoFei = quoteresult.HcFeiYongBuChang.HasValue ? quoteresult.HcFeiYongBuChang.Value : 0
                    },
                    HcJingShenSunShi = new XianZhongUnit
                    {
                        BaoE = savequote.HcJingShenSunShi.HasValue ? savequote.HcJingShenSunShi.Value : 0,
                        BaoFei = quoteresult.HcJingShenSunShi.HasValue ? quoteresult.HcJingShenSunShi.Value : 0
                    },
                    HcSanFangTeYue = new XianZhongUnit
                    {
                        BaoE = savequote.HcSanFangTeYue.HasValue ? savequote.HcSanFangTeYue.Value : 0,
                        BaoFei = quoteresult.HcSanFangTeYue.HasValue ? quoteresult.HcSanFangTeYue.Value : 0
                    },
                    HcXiuLiChang = new XianZhongUnit
                    {
                        BaoE = savequote.HcXiuLiChang.HasValue ? savequote.HcXiuLiChang.Value : 0,
                        BaoFei = quoteresult.HcXiuLiChang.HasValue ? quoteresult.HcXiuLiChang.Value : 0
                    },
                    HcXiuLiChangType = savequote.HcXiuLiChangType.HasValue
                        ? savequote.HcXiuLiChangType.Value.ToString()
                        : string.Empty,
                    SheShui = new XianZhongUnit
                    {
                        BaoE = savequote.SheShui.HasValue ? savequote.SheShui.Value : 0,
                        BaoFei = quoteresult.SheShui.HasValue ? quoteresult.SheShui.Value : 0
                    },
                    CheDeng = new XianZhongUnit
                    {
                        BaoE = savequote.CheDeng.HasValue ? savequote.CheDeng.Value : 0,
                        BaoFei = quoteresult.CheDeng.HasValue ? quoteresult.CheDeng.Value : 0
                    },
                    ZiRan = new XianZhongUnit
                    {
                        BaoE = isquoteresult
                            ? (quoteresult.ZiRanBE.HasValue ? quoteresult.ZiRanBE.Value : 0)
                            : (savequote.ZiRan.HasValue ? savequote.ZiRan.Value : 0),
                        BaoFei = quoteresult.ZiRan.HasValue ? quoteresult.ZiRan.Value : 0
                    },
                    RateFactor1 = quoteresult.RateFactor1.HasValue ? quoteresult.RateFactor1.Value : 0,
                    RateFactor2 = quoteresult.RateFactor2.HasValue ? quoteresult.RateFactor2.Value : 0,
                    RateFactor3 = quoteresult.RateFactor3.HasValue ? quoteresult.RateFactor3.Value : 0,
                    RateFactor4 = quoteresult.RateFactor4.HasValue ? quoteresult.RateFactor4.Value : 0,
                    TotalRate = (quoteresult.TotalRate ?? 0).ToString(CultureInfo.InvariantCulture),
                    //BizTno = string.IsNullOrEmpty(submitInfo.biz_tno) ? string.Empty : submitInfo.biz_tno,
                    //ForceTno = string.IsNullOrEmpty(submitInfo.force_tno) ? string.Empty : submitInfo.force_tno,
                    Fybc = new XianZhongUnit
                    {
                        BaoE = savequote.HcFeiYongBuChang.HasValue ? savequote.HcFeiYongBuChang.Value : 0,
                        BaoFei = quoteresult.HcFeiYongBuChang.HasValue ? quoteresult.HcFeiYongBuChang.Value : 0
                    },
                    FybcDays = new XianZhongUnit()
                    {
                        BaoE = savequote.FeiYongBuChangDays.HasValue ? savequote.FeiYongBuChangDays.Value : 0,
                        BaoFei = savequote.FeiYongBuChangDays.HasValue ? savequote.FeiYongBuChangDays.Value : 0
                    },
                    SheBeiSunShi = new XianZhongUnit
                    {
                        BaoE = savequote.HcSheBeiSunshi ?? 0,
                        BaoFei = quoteresult.HcSheBeiSunshi ?? 0
                    },
                    BjmSheBeiSunShi = new XianZhongUnit
                    {
                        BaoE = savequote.BuJiMianSheBeiSunshi ?? 0,
                        BaoFei = quoteresult.BuJiMianSheBeiSunshi ?? 0
                    },
                    SanZheJieJiaRi = new XianZhongUnit
                    {
                        BaoE = (savequote.SanZheJieJiaRi.HasValue ? savequote.SanZheJieJiaRi.Value : 0) > 0 ? 1 : 0,
                        BaoFei = quoteresult.SanZheJieJiaRi.HasValue ? quoteresult.SanZheJieJiaRi.Value : 0
                    }
                };
            }
            else
            {
                viewModel.PrecisePrice = new PrecisePrice();
            }
            #endregion
            dd_order_related_info related_info = _orderRepository.FindDdOrderRelatedinfoAsync(order.id).Result;
            #region dd_order_related_info模型转换
            if (related_info != null)
            {
                //关系人模型
                var relatedInfo = new RelatedInfo();
                #region holder
                relatedInfo.HolderPerson = new RelatedPerson()
                {
                    Name = related_info.holder_name,
                    IdType = related_info.holder_id_type.HasValue ? related_info.holder_id_type.Value : 0,
                    IdCard = related_info.holder_id_card,
                    Sex = related_info.holder_sex.HasValue ? related_info.holder_sex.Value : 0,
                    Nation = related_info.holder_nation,
                    Birthday =
                        related_info.holder_birthday.HasValue
                            ? (related_info.holder_birthday.Value.ToString("yyyy-MM-dd"))
                            : "",
                    Address = related_info.holder_address,
                    CertiStartDate =
                        related_info.holder_certi_start_date.HasValue
                            ? related_info.holder_certi_start_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    CertiEndDate =
                        related_info.holder_certi_end_date.HasValue
                            ? related_info.holder_certi_end_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    Authority = related_info.holder_authority,
                    Mobile = related_info.holder_mobile,
                    Email = related_info.holder_email
                };
                #endregion
                #region insured
                relatedInfo.InsuredPerson = new RelatedPerson()
                {
                    Name = related_info.insured_name,
                    IdType = related_info.insured_id_type.HasValue ? related_info.insured_id_type.Value : 0,
                    IdCard = related_info.insured_id_card,
                    Sex = related_info.insured_sex.HasValue ? related_info.insured_sex.Value : 0,
                    Nation = related_info.insured_nation,
                    Birthday =
                        related_info.insured_birthday.HasValue
                            ? related_info.insured_birthday.Value.ToString("yyyy-MM-dd")
                            : "",
                    Address = related_info.insured_address,
                    CertiStartDate =
                        related_info.insured_certi_start_date.HasValue
                            ? related_info.insured_certi_start_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    CertiEndDate =
                        related_info.insured_certi_end_date.HasValue
                            ? related_info.insured_certi_end_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    Authority = related_info.insured_authority,
                    Mobile = related_info.insured_mobile,
                    Email = related_info.insured_email
                };
                #endregion
                #region ower
                relatedInfo.OwerPerson = new RelatedPerson()
                {
                    Name = related_info.ower_name,
                    IdType = related_info.ower_id_type.HasValue ? related_info.ower_id_type.Value : 0,
                    IdCard = related_info.ower_id_card,
                    Sex = related_info.ower_sex.HasValue ? related_info.ower_sex.Value : 0,
                    Nation = related_info.ower_nation,
                    Birthday =
                        related_info.ower_birthday.HasValue
                            ? related_info.ower_birthday.Value.ToString("yyyy-MM-dd")
                            : "",
                    Address = related_info.ower_address,
                    CertiStartDate =
                        related_info.ower_certi_start_date.HasValue
                            ? related_info.ower_certi_start_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    CertiEndDate =
                        related_info.ower_certi_end_date.HasValue
                            ? related_info.ower_certi_end_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    Authority = related_info.ower_authority,
                    Mobile = related_info.ower_mobile,
                    Email = related_info.ower_email
                };
                #endregion
                viewModel.RelatedInfo = relatedInfo;
            }
            else
            {
                viewModel.RelatedInfo = new RelatedInfo();
            }
            #endregion
            List<dd_order_paymentresult> paymentresult = _orderRepository.GetOrderPayResultAsync(order.order_num).Result;
            #region dd_order_paymentresult模型转换
            //净费支付信息
            var jinfei = paymentresult.FirstOrDefault(l => l.type == 2);
            viewModel.PaymentResult = new PaymentResult()
            {
                PurchaseAmount = jinfei != null ? (jinfei.money.HasValue ? jinfei.money.Value : 0) : 0,
                Remark = jinfei != null ? (jinfei.remarks ?? "") : "",
                CredentialImg = jinfei != null ? (jinfei.credential_img ?? "") : "",
                CreateTime = jinfei != null ? (
                    jinfei.create_time.HasValue
                        ? jinfei.create_time.Value.ToString("yyyy-MM-dd HH:mm:ss")
                        : "") : ""
            };
            //全款支付信息
            var quankuan = paymentresult.FirstOrDefault(l => l.type == 1 && l.find_pay_result == 1);
            viewModel.PaymentResult.BizNo = quankuan != null ? quankuan.biz_no ?? string.Empty : string.Empty;
            viewModel.PaymentResult.ForceNo = quankuan != null ? quankuan.force_no ?? string.Empty : string.Empty;
            #endregion
            bx_quotereq_carinfo carinfo = _quoteReqCarInfo.FindAsync(order.b_uid).Result;
            #region bx_quotereq_carinfo模型转换
            viewModel.CarInfo = new OrderCarInfo()
            {
                AutoMoldCode = carinfo != null ? (carinfo.auto_model_code ?? "") : "", //精友编码
                BizStartDate = carinfo != null ?
                    (carinfo.biz_start_date.HasValue ? carinfo.biz_start_date.Value.ToString("yyyy-MM-dd HH:mm") : "") : "", //商业险起保时间
                ForceStartDate = carinfo != null ?
                    (carinfo.force_start_date.HasValue ? carinfo.force_start_date.Value.ToString("yyyy-MM-dd HH:mm") : "") : "", //交强险起保时间
                LicenseNo = order.licenseno,
                CarUsedType = carinfo != null ? (carinfo.car_used_type ?? 0) : 0, //车辆类型
                CarVin = userinfo.CarVIN ?? "", //车辆识别代码
                EngineNo = userinfo.EngineNo ?? "", //发动机号
                PurchasePrice = carinfo != null ? (carinfo.PriceT ?? 0) : 0, //新车购置价
                RegisterDate = userinfo.RegisterDate ?? "", //注册日期
                MoldName = userinfo.MoldName ?? "",
                SeatCount = carinfo != null ? (carinfo.seat_count ?? 0) : 0
            };
            #endregion

            var listSteps = _orderRepository.GetOrderStepsAsync(order.id).Result.OrderByDescending(o => o.create_time);
            dd_order_steps step = listSteps.FirstOrDefault(l => l.type == 4);
            dd_order_steps step1 = listSteps.FirstOrDefault(l => l.type == 5);
            //订单流转记录表的4是踢回
            if (order.order_type == 3 && step != null)
            {
                StepsModel jsonmodel = !string.IsNullOrEmpty(step.json_content) ? step.json_content.FromJson<StepsModel>() : new StepsModel();
                viewModel.Order.ReBackReason = jsonmodel.Remark == null ? "" : jsonmodel.Remark;
                viewModel.Order.ReBackDate = step.create_time == null ? "" : step.create_time.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (order.order_type == 2 && step1 != null)
            {
                StepsModel jsonmodel = !string.IsNullOrEmpty(step1.json_content) ? step1.json_content.FromJson<StepsModel>() : new StepsModel();
                viewModel.Order.ReBackReason = jsonmodel.Remark == null ? "" : jsonmodel.Remark;
                viewModel.Order.CancelDate = step1.create_time == null ? "" : step1.create_time.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                viewModel.Order.ReBackReason = "";
            }
            //记录哪个业务员取消的
            viewModel.Order.CancelReason = step1 != null ? string.IsNullOrWhiteSpace(step1.json_content) ? "" : CommonHelper.ToListT<UpdateOrderDetile>("[" + step1.json_content + "]").FirstOrDefault().Remark : "";
            viewModel.Order.CancelAgent = step1 != null ? string.IsNullOrWhiteSpace(step1.json_content) ? "" : CommonHelper.ToListT<UpdateOrderDetile>("[" + step1.json_content + "]").FirstOrDefault().AgentName : "";
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "获取成功";
            return viewModel;
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CreateOrderDetailViewModel CreateOrderDetail(CreateOrderDetailRequest request)
        {
            if (request.FromMethod != 1 && request.FromMethod != 2 && request.FromMethod != 4 && request.FromMethod != 8)
            {//来源平台限制
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，非法渠道，禁止创建"
                };
            }
            //20171024   L：  进行中改为待支付41、待承保42  在此进行中4的判断42不继承  因为41才是符合逻辑的新建订单的状态
            if (request.OrderType != 0 && request.OrderType != 41)
            {//订单状态限制
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，非法状态参数"
                };
            }
            bx_userinfo userinfo = _userInfoRepository.FindByBuid(request.Buid);

            if (userinfo == null)
            {//buid限制
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，无报价信息"
                };
            }
            //非暂存状态，配送信息必填
            if (request.OrderType != 0)
            {
                //20171025   接收保单方式 1电子 2纸质  为  2纸质   配送方式:1、快递保单；2、网点配送 的时候:   配送信息：联系人姓名、联系人电话，配送地址不能为空
                if (request.GetOrderMethod == 2 && (request.DeliveryMethod == 0 || request.DeliveryMethod == 1) &&
                    (string.IsNullOrWhiteSpace(request.DeliveryAddress) ||
                     string.IsNullOrWhiteSpace(request.DeliveryContacts) ||
                     string.IsNullOrWhiteSpace(request.DeliveryContactsMobile)))
                {
                    return new CreateOrderDetailViewModel()
                    {
                        OrderId = 0,
                        OrderNum = "",
                        BusinessStatus = 0,
                        StatusMessage = "创建失败，配送信息：联系人姓名、联系人电话，配送地址不能为空"
                    };
                }

                ////20171025 当新增为APP过来的时候   配送地址ID必须大于0
                //if (request.FromMethod == 4 && request.DeliveryAddressId <= 0)
                //{
                //    return new CreateOrderDetailViewModel()
                //    {
                //        OrderId = 0,
                //        OrderNum = "",
                //        BusinessStatus = 0,
                //        StatusMessage = "创建失败,订单信息有误，请重新填写"
                //    };
                //}
            }

            //查某buid下所有的订单
            List<dd_order> orderList = _orderRepository.FindOrderListByBuid(request.Buid);
            //查已存在出过单或正在进行的订单，就不让出单
            long ddOrderExit = 0;
            //如果是暂存的单子，让其直接过，在下面执行编辑操作
            if (orderList.Any(i => i.order_lapse_time.HasValue && i.order_lapse_time > DateTime.Now && (new int?[] { 3, 41, 42 }).Contains(i.order_type)))
                ddOrderExit = orderList.Where(i => i.order_lapse_time.HasValue && i.order_lapse_time > DateTime.Now && (new int?[] { 3, 41, 42 }).Contains(i.order_type)).Select(l => l.id).FirstOrDefault();
            if (ddOrderExit > 0)
            {
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，该车已存在订单"
                };
            }
            //source转换
            int sourceValue = 0;
            sourceValue = SourceGroupAlgorithm.GetOldSource(request.Source);
            bx_submit_info submitinfo = _submitInfoRepository.GetSubmitInfoAsync(request.Buid, sourceValue).Result;

            if (submitinfo != null && submitinfo.submit_status != 1)
            {
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，核保失败不予投保"
                };
            }

            long id = submitinfo.channel_id.HasValue ? submitinfo.channel_id.Value : 0;
            bx_agent_config agentConfig = _agentConfigRepository.GetAgentConfigById(id);
            if (agentConfig == null || agentConfig.is_used != 1)
            {
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，核保渠道不可用"
                };
            }

            #region 获取核保信息
            bx_savequote savequote = new bx_savequote();
            for (int i = 0; i < 20; i++)
            {
                //循环获取bx_savequote
                savequote = _saveQuoteRepository.GetSavequoteByBuid(request.Buid);
                if (savequote != null && savequote.Id > 0)
                {
                    break;
                }
                Task.Delay(TimeSpan.FromSeconds(1));
            }
            bx_quoteresult quoteresult = new bx_quoteresult();
            for (int i = 0; i < 20; i++)
            {
                //循环获取bx_quoteresult
                quoteresult = _quoteResultRepository.GetQuoteResultByBuid(request.Buid, sourceValue);
                if (quoteresult != null && quoteresult.Id > 0)
                {
                    break;
                }
                Task.Delay(TimeSpan.FromSeconds(1));
            }
            if (savequote == null || quoteresult == null)
            {
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，无险种信息"
                };
            }
            if (savequote.Id == 0 || quoteresult.Id == 0)
            {
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，无险种信息"
                };
            }

            #endregion
            #region 获取险种信息
            bx_submit_info submitInfo = new bx_submit_info();
            DateTime issueTime = submitInfo.create_time ?? DateTime.Now;//出单时间
            if (GetOrderLapseTime(issueTime, submitInfo.biz_start_time, submitInfo.force_start_time) < DateTime.Now)
            {
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，需要重新报价！核保时间距离上次核保时间久远，请重新报价。"
                };
            }
            for (int i = 0; i < 20; i++)
            {
                //循环获取submitinfo
                submitInfo = _submitInfoRepository.GetSubmitInfo(request.Buid, sourceValue);
                if (submitInfo != null && submitInfo.id > 0)
                {
                    break;
                }
                Task.Delay(TimeSpan.FromSeconds(1));
            }
            if (submitInfo == null)
            {
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，无核保信息"
                };
            }
            if (submitInfo.id == 0)
            {
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，无核保信息"
                };
            }
            if (submitinfo.id != 0 && submitinfo.submit_status == 1 && (string.IsNullOrEmpty(submitinfo.force_pno) && string.IsNullOrEmpty(submitinfo.biz_pno)))
            {
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，核保信息有误"
                };
            }
            #endregion
            //生成随机数
            string orderNum = GenerateOrderNum(request.FromMethod);
            //取报价结果的车辆信息
            bx_quoteresult_carinfo carInfo = _carinfoRepository.Find(request.Buid, sourceValue);
            #region dd_order对象
            //取出暂存的单子，直接进行编辑，否则就新增
            dd_order order = orderList.FirstOrDefault(l => l.order_type == 0);
            bx_carinfo carinfo = _iCarInfoRepository.Find(request.Licenseno);
            if (order != null && order.id > 0)
            {//已存在订单，进行修改
                orderNum = order.order_num;//此处赋值是要在接口结尾返回该值
                order.delivery_method = request.DeliveryMethod;
                order.delivery_address = request.DeliveryAddress;
                order.delivery_contacts = request.DeliveryContacts;
                order.delivery_contacts_mobile = request.DeliveryContactsMobile;
                order.pay_type = request.PayType;
                //order.b_uid = request.Buid;
                order.update_time = DateTime.Now;
                //order.create_time = DateTime.Now,
                order.agent_id = request.AgentId;
                order.agent_name = request.AgentName;
                order.issuing_people_id = request.IssuingPeopleId;
                order.issuing_people_name = request.IssuingPeopleName;
                order.delivery_type = request.DeliveryType;
                order.remark = request.Remark;
                order.order_type = request.OrderType;
                order.order_lapse_time = GetOrderLapseTime(issueTime, submitInfo.biz_start_time, submitInfo.force_start_time);
                order.issue_time = issueTime;//核保录入时间
                order.licenseno = request.Licenseno;
                order.source = sourceValue;
                order.quote_city_id = int.Parse(userinfo.CityCode);
                order.quote_config_id = submitInfo.channel_id.HasValue ? (int)submitInfo.channel_id.Value : 0;
                order.force_tno = submitInfo.force_tno;
                order.biz_tno = submitInfo.biz_tno;
                order.insure_year = issueTime.Year;//投保年度为核保时间
                order.preferential = request.Preferential;
                order.get_order_method = request.GetOrderMethod;//1电子保单2纸质保单 //投保地区是北京的是电子保单
                order.total_amount = Convert.ToDouble(request.TotalAmount);
                order.purchase_amount = Convert.ToDouble(request.PurchaseAmount);
                order.receivable_amount = Convert.ToDouble(request.ReceivableAmount);
                order.force_rate = request.ForceRate;
                order.biz_rate = request.BizRate;
                order.MoldName = userinfo.MoldName;
                order.LastBizEndDate = string.IsNullOrWhiteSpace(request.LastBizEndDate) ? DateTime.MinValue : Convert.ToDateTime(request.LastBizEndDate);
                order.LastForceEndDate = string.IsNullOrWhiteSpace(request.LastForceEndDate) ? DateTime.MinValue : Convert.ToDateTime(request.LastForceEndDate);
                order.payee = request.Payee;
                order.pay_way_id = request.PayWayId;
            }
            else
            {//没有订单，新增
                order = new dd_order()
                {
                    order_num = orderNum,
                    order_guid = Guid.NewGuid().ToString().Replace("-", ""),//生成唯一GUID
                    delivery_method = request.DeliveryMethod,
                    delivery_address = request.DeliveryAddress,
                    delivery_contacts = request.DeliveryContacts,
                    delivery_contacts_mobile = request.DeliveryContactsMobile,
                    pay_type = request.PayType,
                    b_uid = request.Buid,
                    update_time = DateTime.Now,
                    create_time = DateTime.Now,
                    agent_id = request.AgentId,
                    agent_name = request.AgentName,
                    issuing_people_id = request.IssuingPeopleId,
                    issuing_people_name = request.IssuingPeopleName,
                    delivery_type = request.DeliveryType,
                    remark = request.Remark,
                    order_type = request.OrderType,
                    order_lapse_time = GetOrderLapseTime(issueTime, submitInfo.biz_start_time, submitInfo.force_start_time),
                    issue_time = issueTime,//核保录入时间
                    licenseno = request.Licenseno,
                    source = sourceValue,
                    quote_city_id = int.Parse(userinfo.CityCode),
                    quote_config_id = submitInfo.channel_id.HasValue ? (int)submitInfo.channel_id.Value : 0,
                    force_tno = submitInfo.force_tno,
                    biz_tno = submitInfo.biz_tno,
                    insure_year = issueTime.Year,
                    preferential = request.Preferential,
                    get_order_method = request.GetOrderMethod,//1电子保单2纸质保单 //投保地区是北京的是电子保单
                    total_amount = Convert.ToDouble(request.TotalAmount),
                    purchase_amount = Convert.ToDouble(request.PurchaseAmount),
                    receivable_amount = Convert.ToDouble(request.ReceivableAmount),
                    force_rate = request.ForceRate,
                    biz_rate = request.BizRate,
                    MoldName = userinfo.MoldName,
                    LastBizEndDate = string.IsNullOrWhiteSpace(request.LastBizEndDate) ? DateTime.MinValue : Convert.ToDateTime(request.LastBizEndDate),
                    LastForceEndDate = string.IsNullOrWhiteSpace(request.LastForceEndDate) ? DateTime.MinValue : Convert.ToDateTime(request.LastForceEndDate),
                    payee = request.Payee,
                    pay_way_id = request.PayWayId
                };
            }

            if (order.order_lapse_time < DateTime.Now)
            {
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败，此订单距离上次操作时间过长，请重新报价核保。"
                };
            }

            #endregion
            #region dd_order_related_info对象
            dd_order_related_info orderRelatedInfo = new dd_order_related_info()
            {
                dd_order_id = 0,
                b_uid = userinfo.Id,
                ower_name = request.OwerName,
                ower_id_type = request.OwerIdType,
                ower_id_card = request.OwerIdCard,
                ower_sex = request.OwerSex,
                ower_nation = request.OwerNation,
                ower_birthday = !string.IsNullOrWhiteSpace(request.OwerBirthday) ? DateTime.Parse(request.OwerBirthday) : (DateTime?)null,
                ower_address = request.OwerAddress,
                ower_certi_start_date = !string.IsNullOrWhiteSpace(request.OwerCertiStartDate) ? DateTime.Parse(request.OwerCertiStartDate) : (DateTime?)null,
                ower_certi_end_date = !string.IsNullOrWhiteSpace(request.OwerCertiEndDate) ? DateTime.Parse(request.OwerCertiEndDate) : (DateTime?)null,
                ower_authority = request.OwerAuthority,
                ower_mobile = request.OwerMobile,
                ower_email = request.OwerEmail,
                holder_name = request.HolderName,
                holder_id_type = request.HolderIdType,
                holder_id_card = request.HolderIdCard,
                holder_sex = request.HolderSex,
                holder_nation = request.HolderNation,
                holder_birthday = !string.IsNullOrWhiteSpace(request.HolderBirthday) ? DateTime.Parse(request.HolderBirthday) : (DateTime?)null,
                holder_address = request.HolderAddress,
                holder_certi_start_date = !string.IsNullOrWhiteSpace(request.HolderCertiStartDate) ? DateTime.Parse(request.HolderCertiStartDate) : (DateTime?)null,
                holder_certi_end_date = !string.IsNullOrWhiteSpace(request.HolderCertiEndDate) ? DateTime.Parse(request.HolderCertiEndDate) : (DateTime?)null,
                holder_authority = request.HolderAuthority,
                holder_mobile = request.HolderMobile,
                holder_email = request.HolderEmail,
                insured_name = request.InsuredName,
                insured_id_type = request.InsuredIdType,
                insured_id_card = request.InsuredIdCard,
                insured_sex = request.InsuredSex,
                insured_nation = request.InsuredNation,
                insured_birthday = !string.IsNullOrWhiteSpace(request.InsuredBirthday) ? DateTime.Parse(request.InsuredBirthday) : (DateTime?)null,
                insured_address = request.InsuredAddress,
                insured_certi_start_date = !string.IsNullOrWhiteSpace(request.InsuredCertiStartDate) ? DateTime.Parse(request.InsuredCertiStartDate) : (DateTime?)null,
                insured_certi_end_date = !string.IsNullOrWhiteSpace(request.InsuredCertiEndDate) ? DateTime.Parse(request.InsuredCertiEndDate) : (DateTime?)null,
                insured_authority = request.InsuredAuthority,
                insured_mobile = request.InsuredMobile,
                insured_email = request.InsuredEmail,
                create_time = DateTime.Now,
                update_time = DateTime.Now
            };
            #endregion
            #region 配送信息处理   可能更改bx_address表数据
            ////当请求来源自PC端，地址和bx_address表中相同取表中的ID 否则 取新增bx_address数据的ID
            //if ((request.DeliveryMethod == 0 || request.DeliveryMethod == 1))
            //{
            //    //-1默认值0快递保单1网点派送2网点自提
            //    if (!string.IsNullOrWhiteSpace(request.DeliveryAddress))
            //    {
            //        bx_address address = 
            //            _iAddressRepository.FindByBuidAndAgentId(Convert.ToInt32(request.Buid), true)
            //                .OrderByDescending(n => n.id)
            //                .FirstOrDefault();
            //        if (request.FromMethod == 4)
            //        {
            //            order.delivery_address_id = request.DeliveryAddressId;
            //        }
            //        else if (address != null && address.address == request.DeliveryAddress)
            //        {
            //            order.delivery_address_id = address.id;
            //        }
            //        else
            //        {
            //            if (request.ProvinceId != 0)
            //            {
            //                address.provinceId = request.ProvinceId;
            //            }
            //            if (request.CityId != 0)
            //            {
            //                address.cityId = request.CityId;
            //            }
            //            if (request.AreaId != 0)
            //            {
            //                address.areaId = request.AreaId;
            //            }
            //            address.Name = request.DeliveryContacts;
            //            address.phone = request.DeliveryContactsMobile;
            //            address.updatetime = DateTime.Now;
            //            address.createtime = DateTime.Now;
            //            address.agentId = request.AgentId;
            //            address.userid = Convert.ToInt32(request.Buid);
            //            address.address = request.DeliveryAddress;
            //            address.Status = 1;
            //            address.isdefault = false;
            //            order.delivery_address_id = _iAddressRepository.Add(address);
            //        }
            //    }
            //}
            //else
            //{
            //    order.delivery_address = "0";
            //}
            #endregion
            if (request.OrderType == 0)
            {
                //暂存的信息，再次暂存，更新录入时间，以最后一次为准
                order.create_time = DateTime.Now;
            }
            else
            {
                //20171020  L：  订单状态被重新定义   此处更新新定义的状态
                //订单状态 0暂存、1已过期、2取消、3被踢回、41待支付（待支付待承保&待支付已承保）、42待承保（已支付待承保）、5已承保（已支付已承保）  
                order.order_type = 41;
            }

            //特殊处理是否修改关系人
            //暂存的时候，前端有可能会修改此值
            //如果之前已经有修改过，则不做修改，还是修改过的状态为1
            if (request.OrderType == 0)
            {
                userinfo.IsChangeRelation = userinfo.IsChangeRelation == 1 ? 1 : request.IsChangeRelated;
            }
            List<dd_order> list = new List<dd_order>();
            CommonHelper.TMRNull<dd_order>(order);
            //创建订单

            CreateOrderReturnModel model = new CreateOrderReturnModel();
            long orderId = _orderRepository.CreateOrderDetail(order, orderRelatedInfo, userinfo, savequote, submitInfo, quoteresult, carInfo, out model);
            if (orderId > 0)
            {
                ////20171020   L:   判断关系人变更
                ////当订单状态为暂存的时候OrderType==0
                if (request.OrderType == 0)
                {
                    OwerHolderInsured modelOHIRequest = new OwerHolderInsured();
                    #region holder关系人
                    modelOHIRequest.HolderName = orderRelatedInfo.holder_name;
                    modelOHIRequest.HolderIdType = orderRelatedInfo.holder_id_type;
                    modelOHIRequest.HolderIdCard = orderRelatedInfo.holder_id_card;
                    modelOHIRequest.HolderSex = orderRelatedInfo.holder_sex;
                    modelOHIRequest.HolderNation = orderRelatedInfo.holder_nation;
                    modelOHIRequest.HolderBirthday = orderRelatedInfo.holder_birthday.HasValue ? orderRelatedInfo.holder_birthday.Value.ToString("yyyy-MM-dd") : "";
                    modelOHIRequest.HolderAddress = orderRelatedInfo.holder_address;
                    modelOHIRequest.HolderCertiStartdate = orderRelatedInfo.holder_certi_start_date.HasValue ? orderRelatedInfo.holder_certi_start_date.Value.ToString("yyyy-MM-dd") : "";
                    modelOHIRequest.HolderCertiEnddate = orderRelatedInfo.holder_certi_end_date.HasValue ? orderRelatedInfo.holder_certi_end_date.Value.ToString("yyyy-MM-dd") : "";
                    modelOHIRequest.HolderIssuer = orderRelatedInfo.holder_authority;
                    modelOHIRequest.HolderMobile = orderRelatedInfo.holder_mobile;
                    modelOHIRequest.HolderEmail = orderRelatedInfo.holder_email;
                    #endregion
                    #region owner关系人
                    modelOHIRequest.LicenseOwner = orderRelatedInfo.ower_name;
                    modelOHIRequest.OwnerIdCardType = orderRelatedInfo.ower_id_type.HasValue ? orderRelatedInfo.ower_id_type.Value : 0;
                    modelOHIRequest.IdCard = orderRelatedInfo.ower_id_card;
                    modelOHIRequest.OwnerSex = orderRelatedInfo.ower_sex;
                    modelOHIRequest.OwnerNation = orderRelatedInfo.ower_nation;
                    modelOHIRequest.OwnerBirthday = orderRelatedInfo.ower_birthday.HasValue ? orderRelatedInfo.ower_birthday.Value.ToString("yyyy-MM-dd") : "";
                    modelOHIRequest.Address = orderRelatedInfo.ower_address;
                    modelOHIRequest.OwnerCertiStartdate = orderRelatedInfo.ower_certi_start_date.HasValue ? orderRelatedInfo.ower_certi_start_date.Value.ToString("yyyy-MM-dd") : "";
                    modelOHIRequest.OwnerCertiEnddate = orderRelatedInfo.ower_certi_end_date.HasValue ? orderRelatedInfo.ower_certi_end_date.Value.ToString("yyyy-MM-dd") : "";
                    modelOHIRequest.OwnerIssuer = orderRelatedInfo.ower_authority;
                    modelOHIRequest.Mobile = orderRelatedInfo.ower_mobile;
                    modelOHIRequest.Email = orderRelatedInfo.ower_email;
                    #endregion
                    #region insured关系人
                    modelOHIRequest.InsuredName = orderRelatedInfo.insured_name;
                    modelOHIRequest.InsuredIdType = orderRelatedInfo.insured_id_type;
                    modelOHIRequest.InsuredIdCard = orderRelatedInfo.insured_id_card;
                    modelOHIRequest.InsuredSex = orderRelatedInfo.insured_sex;
                    modelOHIRequest.InsuredNation = orderRelatedInfo.insured_nation;
                    modelOHIRequest.InsuredBirthday = orderRelatedInfo.insured_birthday.HasValue ? orderRelatedInfo.insured_birthday.Value.ToString("yyyy-MM-dd") : "";
                    modelOHIRequest.InsuredAddress = orderRelatedInfo.insured_address;
                    modelOHIRequest.InsuredCertiStartdate = orderRelatedInfo.insured_certi_start_date.HasValue ? orderRelatedInfo.insured_certi_start_date.Value.ToString("yyyy-MM-dd") : "";
                    modelOHIRequest.InsuredCertiEnddate = orderRelatedInfo.insured_certi_end_date.HasValue ? orderRelatedInfo.insured_certi_end_date.Value.ToString("yyyy-MM-dd") : "";
                    modelOHIRequest.InsuredIssuer = orderRelatedInfo.insured_authority;
                    modelOHIRequest.InsuredMobile = orderRelatedInfo.insured_mobile;
                    modelOHIRequest.InsuredEmail = orderRelatedInfo.insured_email;
                    #endregion

                    string modelOHIRequestStr = modelOHIRequest.ToJson();
                    string modelOHIInUserStr = CommonHelper.ToListT<OwerHolderInsured>("[" + userinfo.ToJson() + "]").FirstOrDefault().ToJson();

                    ////判断关系人是否变更（当比对不一样的时候）
                    if (modelOHIRequestStr != modelOHIInUserStr)
                    {
                        ////修改关系人是否变更状态为  已变更
                        userinfo.IsChangeRelation = 1;
                        userinfo.UpdateTime = DateTime.Now;
                        _userInfoRepository.Update(userinfo);
                    }
                }
                long ordersource = SourceGroupAlgorithm.GetNewSource(order.source);
                var crmTimeLineForSmsViewModel = new FollowStepsViewModel
                {
                    AgentId = order.agent_id,
                    BuId = order.b_uid,
                    ReviewStatus = 14,
                    ReviewTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ReviewContent = "申请投保",
                    DefeatReasonContent = "",
                    Source = ordersource,
                    sourceName = GetSourceName(ordersource),
                    SingleTime = "",
                    OrderNum = order.order_num
                };

                var bxCrmSteps = new bx_crm_steps
                {
                    agent_id = order.agent_id,
                    b_uid = order.b_uid,
                    create_time = DateTime.Now,
                    json_content = crmTimeLineForSmsViewModel.ToJson(),
                    type = 12
                };

                int changeNum = _consumerDetailService.AddCrmSteps(bxCrmSteps);

                dd_order_quoteresult orderQuoteresult = _orderRepository.FindDdOrderQuoteresultAsync(order.id).Result;

                List<bx_claim_detail> claimDetails = _userClaimRepository.FindList(request.Buid);
                //商业
                orderQuoteresult.biz_claim_count = claimDetails.Count(n => n.pay_type == 0);
                //交强
                orderQuoteresult.force_claim_count = claimDetails.Count(n => n.pay_type == 1);

                orderQuoteresult.expected_loss_rate = quoteresult == null ? null : quoteresult.expected_loss_rate;
                orderQuoteresult.biz_expected_loss_rate = quoteresult == null ? null : quoteresult.biz_expected_loss_rate;

                _orderRepository.Update(orderQuoteresult);





                return new CreateOrderDetailViewModel()
                {
                    OrderId = request.NeedOrderId == 1 ? orderId : 0,
                    OrderNum = orderNum,
                    BusinessStatus = 1,
                    StatusMessage = "创建成功"
                };
            }
            else
            {
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = 0,
                    StatusMessage = "创建失败"
                };
            }
        }

        #region 获取渠道名称

        public string GetSourceName(long newSource)
        {
            if (newSource == 2147483648)
            {
                return "恒邦车险";
            }
            if (newSource == 4294967296)
            {
                return "中铁车险";
            }
            if (newSource == 8589934592)
            {
                return "美亚车险";
            }
            if (newSource == 17179869184)
            {
                return "富邦车险";
            }
            if (newSource == 34359738368)
            {
                return "众诚车险";
            }
            return ToEnumDescription(newSource, typeof(EnumSourceNew));
        }

        public static String ToEnumDescription(long value, Type enumType)
        {
            NameValueCollection nvc = GetNvcFromEnumValue(enumType);
            return nvc[value.ToString()];
        }
        public static NameValueCollection GetNvcFromEnumValue(Type enumType)
        {
            var nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        var aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = "";
                    }
                    nvc.Add(strValue, strText);
                }
            }
            return nvc;
        }

        #endregion


        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UpdateOrderDetailViewModel UpdateOrderDetail(UpdateOrderDetailRequest request)
        {
            //获取订单
            dd_order order = new dd_order();
            order = _orderRepository.FindOrder(request.OrderNum);
            if (order == null)
            {
                return new UpdateOrderDetailViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败，未查到订单信息。"
                };
            }
            //订单状态 0暂存、1已过期、2废弃(取消)、3被踢回、4进行中、5已完成
            if (order.order_type == 1 || order.order_type == 2 || order.order_type == 5)
            {
                return new UpdateOrderDetailViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败，取消或者完成的订单不允许被修改。"
                };
            }
            //order.order_type = request.OrderType;
            int countUpdate = _orderRepository.UpdateOrder(order);
            if (countUpdate > 0)
            {
                return new UpdateOrderDetailViewModel()
                {
                    BusinessStatus = 1,
                    StatusMessage = "修改成功"
                };
            }
            else
            {
                return new UpdateOrderDetailViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败"
                };
            }
        }
        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UpdateOrderStatusViewModel UpdateOrderStatus(UpdateOrderStatusRequest request)
        {
            //获取订单
            dd_order order = new dd_order();
            order = _orderRepository.FindOrder(request.OrderNum);
            if (order == null)
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败，未查到订单信息。"
                };
            }
            //订单状态 0暂存、1已过期、2废弃(取消)、3被踢回、4进行中、5已完成
            if (order.order_type == 1 || order.order_type == 2 || order.order_type == 5)
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败，取消或者完成的订单不允许被修改。"
                };
            }
            order.order_type = request.OrderType;

            int countUpdate = 0;
            countUpdate = _orderRepository.UpdateOrder(order);

            if (countUpdate > 0)
            {
                countUpdate = 0;
                if (request.OrderType == 2)
                {
                    bx_agent thisAgent = _agentService.GetAgent(request.ChildAgent);
                    UpdateOrderDetile upJson = new UpdateOrderDetile()
                    {
                        AgentId = request.ChildAgent,
                        AgentName = thisAgent.AgentName,
                        Remark = request.Remark
                    };

                    dd_order_steps orderSteps = new dd_order_steps()
                    {
                        json_content = upJson.ToJson(),
                        create_time = DateTime.Now,
                        agent_id = request.ChildAgent,
                        type = 5,
                        order_id = order.id
                    };
                    countUpdate = _orderRepository.AddOrderSteps(orderSteps);

                    //发送post请求调用第三方
                    _orderPostThirdService.SendPost(request.Agent, "", 3, null, null, order, null, null, null);
                }
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 1,
                    StatusMessage = "修改成功"
                };

            }
            else
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败"
                };
            }
        }

        public UpdateOrderStatusViewModel ReBackOrder(ReBackOrderRequest request)
        {
            //获取订单
            dd_order order = new dd_order();
            order = _orderRepository.FindOrder(request.OrderNum);
            if (order == null)
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败，未查到订单信息。"
                };
            }
            //订单状态 0暂存、1已过期、2废弃(取消)、3被踢回、41待支付、42待承保、5已完成
            if (order.order_type != 41)
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败，取消或者完成的订单不允许被修改。"
                };
            }
            order.order_type = 3;
            int countUpdate = _orderRepository.UpdateOrder(order);
            if (countUpdate > 0)
            {
                //给业务员推送消息
                var agent = agentRepository.GetAgent(order.agent_id);
                if (agent != null)
                {
                    var messageRequest = new PushMessageRequest()
                    {
                        MsgType = 10,
                        Agent = agent.TopAgentId,
                        OrderNum = order.order_num,
                        ChildAgent = order.agent_id,
                        LincenseNo = order.licenseno,
                        Buid = order.b_uid,
                        TitleStr = "您提交的订单（" + order.licenseno + "）被踢回了，请及时查看！"
                    };
                    _pushMessageService.PushMessage(messageRequest);
                }


                //给出单员推送消息
                var id = order.issuing_people_id.HasValue ? order.issuing_people_id.Value : 0;
                if (id > 0)
                {
                    var issuAgent = agentRepository.GetAgent(Convert.ToInt32(id));
                    if (issuAgent != null)
                    {
                        var messageRequest1 = new PushMessageRequest()
                        {
                            MsgType = 10,
                            Agent = issuAgent.TopAgentId,
                            OrderNum = order.order_num,
                            ChildAgent = id,
                            LincenseNo = order.licenseno,
                            Buid = order.b_uid,
                            TitleStr = "您提交的订单（" + order.licenseno + "）被踢回了，请及时查看！"
                        };
                        _pushMessageService.PushMessage(messageRequest1);
                    }
                }

                int stepid = WriteOrderSteps(request.Remark, order.id, request.ChildAgent, order.agent_id, 4);
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 1,
                    StatusMessage = "修改成功"
                };
            }
            else
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败"
                };
            }
        }

        /// <summary>
        /// 订单设置合作银行 李金友 2017-11-06 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UpdateOrderStatusViewModel UpdateOrderPayWayId(PayWayOrderRequest request)
        {
            //获取订单
            dd_order order = new dd_order();
            order = _orderRepository.FindOrder(request.OrderNum);
            if (order == null)
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败，未查到订单信息。"
                };
            }
            //订单状态 0暂存、1已过期、2废弃(取消)、3被踢回、41待支付、42待承保、5已完成
            //if (order.order_type != 41)
            //{
            //    return new UpdateOrderStatusViewModel()
            //    {
            //        BusinessStatus = 0,
            //        StatusMessage = "修改失败，取消或者完成的订单不允许被修改。"
            //    };
            //}
            order.pay_way_id = request.PayWayId;
            int countUpdate = _orderRepository.UpdateOrder(order);
            if (countUpdate > 0)
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 1,
                    StatusMessage = "修改成功"
                };
            }
            else
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败"
                };
            }
        }

        public UpdateOrderStatusViewModel UpdateOrderPayWayIdOrType(PayWayOrTypeOrderRequest request)
        {
            try
            {
                //获取订单
                dd_order order = new dd_order();
                order = _orderRepository.FindOrder(request.OrderNum);
                if (order == null)
                {
                    return new UpdateOrderStatusViewModel()
                    {
                        BusinessStatus = 0,
                        StatusMessage = "修改失败，未查到订单信息。"
                    };
                }
                //订单状态 0暂存、1已过期、2废弃(取消)、3被踢回、41待支付、42待承保、5已完成
                //if (order.order_type != 41)
                //{
                //    return new UpdateOrderStatusViewModel()
                //    {
                //        BusinessStatus = 0,
                //        StatusMessage = "修改失败，取消或者完成的订单不允许被修改。"
                //    };
                //}
                order.pay_way_id = request.PayWayId == -1 ? order.pay_way_id : request.PayWayId;
                order.pay_type = request.PayType == -1 ? order.pay_type : request.PayType;
                order.PayMentRemark = string.IsNullOrWhiteSpace(request.PayMentRemark) ? "" : request.PayMentRemark;
                order.PayMent = request.PayMent;
                int countUpdate = _orderRepository.UpdateOrder(order);
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 1,
                    StatusMessage = "修改成功"
                };
            }
            catch (Exception ex)
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = -10003,
                    StatusMessage = "修改失败"
                };
            }
        }

        public UpdateOrderStatusViewModel UpdateOrderTnoAndAmount(TnoAndAmountOrderRequest request)
        {
            //获取订单
            dd_order order = _orderRepository.FindOrder(request.OrderNum);
            if (order == null)
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败，未查到订单信息。"
                };
            }

            order.biz_tno = string.IsNullOrWhiteSpace(request.BizTno) ? order.biz_tno : request.BizTno;
            order.force_tno = string.IsNullOrWhiteSpace(request.ForceTno) ? order.force_tno : request.ForceTno;
            order.total_amount = request.TotalAmount == -1 ? order.total_amount : request.TotalAmount;
            order.purchase_amount = request.PurchaseAmount == -1 ? order.purchase_amount : request.PurchaseAmount;
            order.receivable_amount = request.ReceivableAmount == -1 ? order.receivable_amount : request.ReceivableAmount;
            int countUpdate = _orderRepository.UpdateOrder(order);
            if (countUpdate > 0)
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 1,
                    StatusMessage = "修改成功"
                };
            }
            else
            {
                return new UpdateOrderStatusViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "修改失败"
                };
            }
        }


        #region wyy 2017-08-17 10:18 add
        #region  身份证采集
        private BaseViewModel CollectIdCardBase(CollectIdCardRequest request)
        {
            #region 正则 民族校验
            //安心的不需要校验
            if (request.Source != 4096)
            {
                if (string.IsNullOrWhiteSpace(request.HolderNation))
                    return SetBaseViewModel(-10001, "投保人民族不能为空!");
                if (string.IsNullOrWhiteSpace(request.InsuredNation))
                    return SetBaseViewModel(-10001, "被保人民族不能为空!");
                //民族列表
                var nationList = EnumHelper.GetEnumItemDesc(typeof(EnumNation));
                var isHolderNation = false;
                var isInsuredNation = false;
                foreach (var item in nationList)
                {
                    if (request.HolderNation.Equals(item.Value))
                        isHolderNation = true;
                    if (request.InsuredNation.Equals(item.Value))
                        isInsuredNation = true;
                }
                if (!isHolderNation)
                    return SetBaseViewModel(-10001, "投保人民族不正确!");
                if (!isInsuredNation)
                    return SetBaseViewModel(-10001, "被保人民族不正确!");
                //外部需要转换保险公司
                if (string.IsNullOrWhiteSpace(request.OrderNum))
                    request.Source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
                request.HolderCertiEnddate = Convert.ToDateTime(request.HolderCertiEnddate).ToString("yyyyMMdd");
                request.HolderCertiStartdate = Convert.ToDateTime(request.HolderCertiStartdate).ToString("yyyyMMdd");
                request.InsuredCertiEnddate = Convert.ToDateTime(request.InsuredCertiEnddate).ToString("yyyyMMdd");
                request.InsuredCertiStartdate = Convert.ToDateTime(request.InsuredCertiStartdate).ToString("yyyyMMdd");
                if (request.InsuredCertiEnddate.Equals("00010101"))
                    return SetBaseViewModel(-10001, "被保人证件有效期止期不正确!");
                if (request.HolderCertiEnddate.Equals("00010101"))
                    return SetBaseViewModel(-10001, "投保人证件有效期起期不正确!");
                if (request.HolderCertiStartdate.Equals("00010101"))
                    return SetBaseViewModel(-10001, "被保人证件有效期止期不正确!");
                if (request.InsuredCertiStartdate.Equals("00010101"))
                    return SetBaseViewModel(-10001, "被保人证件有效期起期不正确!");
                if (string.IsNullOrWhiteSpace(request.HolderBirthday))
                    return SetBaseViewModel(-10001, "投保人生日日期不正确!");
                if (string.IsNullOrWhiteSpace(request.InsuredBirthday))
                    return SetBaseViewModel(-10001, "被保人保人生日日期不正确!");
            }
            else
            {
                //安心的不传投保单号
                var submitInfo = _submitInfoRepository.GetSubmitInfo(request.buId ?? 0, 12);
                if (submitInfo != null)
                {
                    request.BiztNo = submitInfo.biz_tno;
                    request.ForcetNo = submitInfo.force_tno;
                }
                else
                {
                    return SetBaseViewModel(-10001, "不存在报价信息!");
                }
            }
            #endregion
            //对外的缓存key
            var verifyCodeKey = string.Format("{0}_{1}_{2}", request.Agent, request.CarVin, request.MachineId);
            //提交时判断验证码是否有效
            //去掉安心
            if (request.Source != 4096)
            {
                if (request.IsOut == null && !string.IsNullOrEmpty(request.Code) && !_orderRepository.CheckOrderVerificationDate(request.OrderNum))
                {
                    return SetBaseViewModel(-10001, "验证码已失效!");
                }
                else
                {
                    //对外接口查询验证码是否有效 key=agent+CarVin+MachineId        _cacheClientString.KeyExists(verifyCodeKey)          
                    if (request.IsOut != null && !string.IsNullOrEmpty(request.Code) && CacheProvider.Get<int>(verifyCodeKey) < 1)
                        return SetBaseViewModel(-10001, "验证码已失效!");
                }
                request.settingid = request.MachineId;
            }

            #region//向炎龙发送消息体
            if (!IsUsedChannel(request.ChannelId))
                return SetBaseViewModel(-10015, "渠道不可用!");
            //传递消息体的缓存key
            request.NotifyCacheKey = Guid.NewGuid().ToString();
            //缓存key         
            var sendMode = new
            {
                request.BiztNo,
                request.busiuserId,
                request.CarVin,
                request.ChannelId,
                request.Code,
                request.ForcetNo,
                request.HolderAddress,
                request.HolderBirthday,
                request.HolderCertiEnddate,
                request.HolderCertiStartdate,
                request.HolderEmail,
                request.HolderIdCard,
                request.HolderIdType,
                request.HolderIssuer,
                request.HolderMobile,
                request.HolderName,
                request.HolderNation,
                request.HolderSex,
                request.InsuredAddress,
                request.InsuredBirthday,
                request.InsuredCertiEnddate,
                request.InsuredCertiStartdate,
                request.InsuredEmail,
                request.InsuredIdCard,
                request.InsuredIssuer,
                request.InsuredMobile,
                request.InsuredName,
                request.InsuredNation,
                request.InsuredSex,
                request.LicenseNo,
                request.MachineCode,
                request.MobileNumber,
                request.NotifyCacheKey,
                request.SaleChannel,
                request.settingid,
                //安心的需要转换
                Source = request.Source == 4096 ? 12 : request.Source,
                B_Uid = request.buId
            };
            #endregion

            #region 判断是验证还是发送阅知单(验证)
            //安心的没有验证
            if (string.IsNullOrEmpty(request.Code) || (!string.IsNullOrEmpty(request.Code) && request.Source != 4096))
            {
                var notifyCacheKey = string.Empty;
                if (string.IsNullOrEmpty(request.Code))
                {
                    SendToMessageCenter(CommonHelper.JsonSerializeObject(sendMode), "bx_idCardCollection");
                    notifyCacheKey = string.Format("{0}-IdCard", request.NotifyCacheKey);
                }
                else
                {
                    SendToMessageCenter(CommonHelper.JsonSerializeObject(sendMode), "bx_cardVerify");
                    notifyCacheKey = string.Format("{0}-CardVerify", request.NotifyCacheKey);
                }
                // #endregion
                // #region //读取缓存 数据校验
                CollectIdCardResult centerMsg = null;
                centerMsg = CacheProvider.Get<CollectIdCardResult>(notifyCacheKey);
                var PayWaitDate = Convert.ToInt32(CommonHelper.GetAppSettings("PayWaitDate"));
                if (null == centerMsg)
                {
                    for (int i = 0; i < PayWaitDate; i++)
                    {
                        centerMsg = CacheProvider.Get<CollectIdCardResult>(notifyCacheKey);
                        if (centerMsg != null)
                            break;
                        Task.Delay(TimeSpan.FromMilliseconds(1000)).Wait(); //500毫秒
                    }
                }
                if (string.IsNullOrEmpty(request.Code))
                    logMsg.Info(string.Format("【订单支付-采集发送阅知单接口】中心返回信息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);
                else
                    logMsg.Info(string.Format("【订单支付-采集验证接口】中心返回信息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);
                if (centerMsg == null)
                    return SetBaseViewModel(-10019, "请求超时,请重试!");
                //已经采集过了，无需采集 2017/10/09 wyy add  -60029
                if (centerMsg.ErrCode == -33029)
                {
                    //不是对外时修改订单的采集状态 
                    if (!request.IsOut.HasValue || request.IsOut == false)
                    {
                        if (_orderRepository.UpdateVerificationCodeStatus(request.OrderNum, 1))
                            return SetBaseViewModel((int)centerMsg.ErrCode, "已经采集过了,无需采集!");
                        else
                            return SetBaseViewModel(-10014, "订单不存在,验证失败!");
                    }
                    return SetBaseViewModel((int)centerMsg.ErrCode, "无需采集!");
                }
                if (centerMsg.ErrCode != 0)
                    return SetBaseViewModel((int)centerMsg.ErrCode, string.IsNullOrEmpty(centerMsg.ErrMsg) ? " 请求失败！" : centerMsg.ErrMsg);
            }
            #endregion

            #region //结果处理
            //添加采集日志
            if (!InsertCollectionLog(request))
                return SetBaseViewModel(-10019, "添加采集日志失败!");
            if (string.IsNullOrEmpty(request.Code) || (!string.IsNullOrEmpty(request.Code) && request.Source != 4096))
            {
                if (string.IsNullOrEmpty(request.Code))
                {
                    if (request.IsOut == null && request.Source != 4096)
                    {
                        //更新订单表的验证码时间 dd_order.verification_code_lapse_time
                        if (_orderRepository.UpdateOrderVerificationDate(request.OrderNum))
                            return SetBaseViewModel(1, "验证码已发送!");
                        else
                            return SetBaseViewModel(-10014, "更改验证有误!");
                    }
                    else
                    {
                        //有效时间117秒
                        CacheProvider.Set(verifyCodeKey, 1, 117 * 60);
                        return SetBaseViewModel(1, "验证码已发送!");
                    }
                }
                else
                {
                    if (request.IsOut == null && request.Source != 4096)
                    {
                        if (_orderRepository.UpdateVerificationCodeStatus(request.OrderNum, 1))
                            return SetBaseViewModel(1, "验证成功!");
                        else
                            return SetBaseViewModel(-10014, "订单不存在,验证失败!");
                    }
                    else
                    {
                        return SetBaseViewModel(1, "验证成功!");
                    }
                }
            }
            else
            {
                return SetBaseViewModel(1, "操作成功!");
            }
            #endregion
        }

        /// <summary>
        /// 添加采集日志
        /// </summary>
        /// <returns></returns>
        private bool InsertCollectionLog(CollectIdCardRequest request)
        {
            var o = new dd_order_collection();
            o.biz_tno = request.BiztNo;
            o.force_tno = request.ForcetNo;
            o.order_num = string.IsNullOrWhiteSpace(request.OrderNum) ? request.CarVin : request.OrderNum;
            if (request.IsOut.HasValue && request.IsOut == true)
                o.is_out = 1;
            else
                o.is_out = 0;
            o.licenseno = request.LicenseNo;
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                o.status = -1;
                o.verify_code = string.IsNullOrWhiteSpace(request.Code) ? " " : request.Code;
                o.update_time = DateTime.Now;
                o.creat_time = o.update_time;
                if (_orderRepository.InsertOrdderCollection(o) > 0)
                    return true;
            }
            else
            {
                var m = _orderRepository.GetOrdderCollection(o.order_num, null);
                if (m != null)
                {
                    m.status = 1;
                    m.verify_code = request.Code;
                    m.update_time = DateTime.Now;
                    if (_orderRepository.ModifyOrdderCollection(m) > 0)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取最近的一条采集信息验证码
        /// </summary>
        /// <param name="num">订单号或者车架号</param>
        /// <param name="status">状态</param>
        /// <returns>获取采集的信息</returns>
        private dd_order_collection GetcollectionLog(string num, int? status)
        {
            return _orderRepository.GetOrdderCollection(num, status);
        }


        /// <summary>
        /// 设置返回值
        /// </summary>
        /// <param name="code">消息码</param>
        /// <param name="msg">消息内容</param>
        /// <param name="data">结果</param>
        /// <returns></returns>
        public BaseViewModel SetBaseViewModel(int code, string msg, object data = null)
        {
            var model = new BaseViewModel();
            model.BusinessStatus = code;
            model.StatusMessage = msg;
            model.Data = data;
            return model;
        }
        #endregion


        /// <summary>
        /// 渠道是否可用
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public bool IsUsedChannel(int? channelId)
        {
            if (!channelId.HasValue || channelId < 1)
                return false;
            var agentConfig = _agentConfigRepository.GetAgentConfigById((long)channelId);
            if (agentConfig == null || agentConfig.is_used != 1)
                return false;
            return true;
        }

        /// <summary>
        ///  向中心发送获取保单请求
        /// </summary>
        /// <param name="data">参数字符串</param>
        /// <param name="method">方法名</param>
        private string SendToMessageCenter(string data, string method)
        {
            string url = string.Format("{0}/SetBusinessMessage", CommonHelper.GetAppSettings("MessageCenter"), method);
            string postData = string.Format("data={0}&queueName={1}", data, method);
            string result;
            int ret = HttpWebAsk.Post(url, postData, out result);
            var md = string.Empty;
            switch (method)
            {
                case "reissue_policy": md = "补发电子保单"; break;
                case "bx_idCardCollection": md = "身份采集"; break;
                case "bx_cardVerify": md = "采集验证"; break;
                case "bx_insurance": md = "获取支付链接"; break;
                case "pay_outcome": md = "到账查询"; break;
                case "bx_rejectPolicy": md = "作废支付"; break;
                case "bx_epolicyDownload": md = "电子保单下载"; break;
            }
            logMsg.Info(string.Format("【订单支付-{4}】向中心发送 消息地址：{0}, \n  参数：{1}, \n 返回结果：{2}{3}", url, postData, result, logSeparator, md));
            return result;
        }


        #region  电子保单下载
        /// <summary>
        /// 是否已申请
        /// </summary>
        /// <param name="request"></param>
        /// <param name="msg"></param>
        /// <param name="isCheckStatus">是够校验下载状态</param>
        /// <returns>true=已申请,false=未申请</returns>
        public bx_epolicy_log IsApplyElecPolicy(EPolicycs request, bool isCheckStatus, out string msg)
        {
            msg = "ok!";
            if (string.IsNullOrWhiteSpace(request.LicenseNo))
            {
                msg = "缺少LicenseNo参数!";
                return null;
            }
            var objs = _epolicyRepository.FindEpolicyLogByAgentAnd(request.Agent, (int)request.Source, request.LicenseNo, request.ChannelId);
            if (!objs.Any())
                return null;
            var obj = objs.OrderByDescending(s => s.id).FirstOrDefault();
            //5分钟内申请过 status=2下载失败
            if (isCheckStatus && obj.status == 2)
                return null;
            //已申请过了
            return obj;
        }
        /// <summary>
        /// 获取下载的url(有效期1小时)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ElecPolicyUrl(string key)
        {
            Mac mac = new Mac(CommonHelper.GetAppSettings("QiNiu_AccessKey"), CommonHelper.GetAppSettings("QiNiu_SecretKey"));
            string privateUrl = DownloadManager.CreatePrivateUrl(mac, CommonHelper.GetAppSettings("QiNiu_domain"), key, 3600);
            return privateUrl;
        }

        #endregion

        #endregion

        public SearchOrderViewModel SearchOrder(SearchOrderRequest request)
        {
            SearchOrderDto listOrder = null;
            if (request.CarOwnerId == null)
            {
                // 要搜索的业务员集合
                var listAgentId = new List<int>();
                // 要搜索的出单员结合
                var listIssuingPerpleId = new List<int>();
                // 业务员和出单员是and还是or，1：and  2：or，这里是当时顶级代理人使用，现在保留，增加了扩展性
                var isAndOr = 1;

                // 17号：查询条件----业务员角色为3，4，5的时候查询业务员查询所有子级的业务员数据   18号：新增二级查看子级权限
                // 当业务员选中传值不为0的时候，查询当前业务员的所有订单数据（条件如上）
                if (request.AgentId > 0)
                    listAgentId.Add(request.AgentId);
                // listAgentId = _customerTopLevelService.GetListAgent(request.AgentId, request.Agent);
                else
                    listAgentId = _customerTopLevelService.GetListAgent(request.ChildAgent, request.Agent);

                //当出单员选中传值不为0的时候，当业务员为二级的时候查询所有子级的订单
                if (request.IssuingPeopleId < 0)
                    listIssuingPerpleId = _customerTopLevelService.GetListAgent(request.ChildAgent, request.Agent);
                else
                    listIssuingPerpleId.Add(request.IssuingPeopleId);
                //listIssuingPerpleId = _customerTopLevelService.GetListAgent(request.IssuingPeopleId, request.Agent);

                if (request.AgentId < 0 && request.IssuingPeopleId < 0)
                {
                    isAndOr = 2;
                }

                listOrder = _orderRepository.SearchOrder(request, listAgentId, listIssuingPerpleId, isAndOr);
            }
            else
            {
                listOrder = _orderRepository.SearchOrder(request, null, null, 0, request.CarOwnerId);
            }
            var result = SearchOrderViewModel.GetModel(BusinessStatusType.OK);

            if (!listOrder.ListOrder.Any())
            {
                result.TotalCount = 0;
                return result;
            }

            long[] ids = listOrder.ListOrder.Select(n => n.Id).ToArray();
            List<dd_order_savequote> listSavequote = _orderRepository.FinDdOrderSavequotes(ids);
            List<dd_order_steps> listStepses = _orderRepository.GetOrderSteps(ids);
            List<dd_order_commission> listCommissions = _orderCommissionRepository.GetList(x => ids.Contains(x.order_id));

            string statusName = string.Empty;
            foreach (var item in listOrder.ListOrder)
            {
                item.IsPaymentTime = item.PaymentTime.HasValue ? item.PaymentTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                item.PaymentTime = null;
                item.InputPayCodeEndTime = item.PayCodeCreateTime.HasValue
                    ? item.PayCodeCreateTime.Value.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss")
                    : "";
                item.OrderTypeExplain =
                    getExplain(
                        listStepses.Where(n => n.order_id == item.Id).OrderByDescending(n => n.id).FirstOrDefault());
                item.InputLastBizEndDate = item.LastBizEndDate.HasValue ? item.LastBizEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") == DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss") ? "" : item.LastBizEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                item.InputLastForceEndDate = item.LastForceEndDate.HasValue ? item.LastForceEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") == DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss") ? "" : item.LastForceEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                item.Insurances = GetInsurances(listSavequote.Where(n => n.dd_order_id == item.Id).FirstOrDefault());
                // 获取订单状态
                GetOrderStatus(item.OrderType, item.OrderLapseTime, out statusName);
                item.newSource = SourceGroupAlgorithm.GetNewSource(item.Source);
                item.OrderStatus = statusName;

                var commission = listCommissions != null && listCommissions.Where(x => x.order_id == item.Id).ToList().Count > 0 ? listCommissions.Where(x => x.order_id == item.Id).ToList().Sum(x => x.money) : 0;
                var integral = listCommissions != null && listCommissions.Where(x => x.order_id == item.Id).ToList().Count > 0 ? listCommissions.Where(x => x.order_id == item.Id).ToList().Sum(x => x.credit) : 0;

                #region 2018-10-08 张克亮，佣金为0时将根据保单交强商业费率与车辆性质去获取顶级经纪人下配置的佣金配置计算出佣金
                if (commission <= 0)
                {
                    //获取报价结果
                    var quoteresult = _quoteResultRepository.GetQuoteResultByBuid(item.BUID, item.Source);
                    //获取报价结果车辆信息
                    var quoteresultCarinfo = _carinfoRepository.Find(item.BUID, item.Source);
                    //获取核保信息
                    var submitinfo = _submitInfoRepository.GetSubmitInfo(item.BUID, item.Source);

                    //获取计算出商业交强佣金
                    CommissionIntegralViewModel getComm;
                    if (quoteresult != null && submitinfo != null)
                    {
                        var commissionIntegral = new CommissionIntegralRequest
                        {
                            //商业险保费总金额
                            BizTotal = quoteresult.BizTotal.HasValue ? quoteresult.BizTotal.Value : 0,
                            //交强险保费总金额
                            ForceTotal = quoteresult.ForceTotal.HasValue ? quoteresult.ForceTotal.Value : 0,
                            //车辆使用性质
                            CarUsedType =
                                quoteresultCarinfo != null && quoteresultCarinfo.car_used_type.HasValue
                                    ? quoteresultCarinfo.car_used_type.Value
                                    : -1,
                            //RbJSKJ = submitinfo.RbJSKJ
                            Agent = request.Agent,
                        };
                        getComm = _getCommissionIntegralService.GetCommissionIntegral(commissionIntegral);
                        //商业+交强的佣金是订单最后的总佣金
                        commission = getComm.BizCommission + getComm.ForceCommission;
                        //商业+交强的积分是订单最后的总积分
                        integral = getComm.BizIntegral + getComm.ForceIntegral;
                    }
                }
                #endregion

                item.Commission = String.Format("{0:F}", commission);
                item.Integral = Convert.ToInt32(integral);

                if (statusName == "已过期")
                {
                    item.OrderType = 1;
                }
                item.QuoteStatus = item.QuoteStatus ?? 0;
                item.SubmitStatus = item.SubmitStatus ?? 0;
                // 获取身份证采集状态
                item.VerificationCodeStatus = item.VerificationCodeStatus;//GetIdCardStatus(item.VerificationCodeStatus, item.VerificationCodeCreateTime);

                #region 2018-10-09 张克亮，加入订单配送信息
                var deliveryInfoResult = GetOrderDeliveryInfo(item.OrderNum);
                if (deliveryInfoResult.BusinessStatus == 1 && deliveryInfoResult.Data != null)
                {
                    item.DeliveryInfo = deliveryInfoResult.Data as OrderDeliveryInfoResponse;
                }
                #endregion
            }

            listOrder.ListOrder.TRNull<DDOrder>();
            result.ListOrder = listOrder.ListOrder;
            result.TotalCount = listOrder.TotalCount;
            return result;
        }

        public async Task<BaseViewModel> AddConsumerPayResultAsync(AddConsumerPayResultRequest request)
        {
            if (request.ListImgs.Any() && request.ListImgs.Count > 4)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "最多可以上传4张影像凭据");
            }

            var order = _orderRepository.FindOrder(request.OrderNum);
            if (order == null)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "订单Id不准确");
            }

            // 判断是否已经录入了净费支付结果
            if (await _orderRepository.IsExistPaymentResultAsync(order.id, 2))
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.NoCondition, "该订单已经录入过净费支付结果，请勿重复录入");
            }

            dd_order_paymentresult payResult = new dd_order_paymentresult
            {
                biz_no = order.biz_tno,
                create_time = request.PayDateTime,
                force_no = order.force_tno,
                licenseNo = order.licenseno,
                money = (double)request.Money,
                order_id = (int)order.id,
                credential_img = string.Join(",", request.ListImgs),
                type = 2,
                remarks = request.Remarks,
                pay_type = request.PayType,
                order_num = request.OrderNum
            };

            // 修改订单的consumer_pay_status=1，update_time
            order.update_time = DateTime.Now;
            order.consumer_pay_status = 1;
            //如果全款支付insurance_company_pay_status和净费支付consumer_pay_status状态都为1（已支付），则将订单状态改为5（已完成）
            order.order_type = order.insurance_company_pay_status == 1 ? 5 : 42;
            //42待承保（已支付待承保）。备注一个概念：只要全款缴费成功，则为已承保
            try
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    _orderRepository.InsertOrderPayResult(payResult);
                    _orderRepository.UpdateOrder(order);
                    trans.Complete();
                }
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            }
            catch (Exception ex)
            {
                logErro.Error("录入净费支付异常，异常信息：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException + logSeparator);
            }
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
        }

        #region 采集器绑定-新的
        //public async Task<BaseViewModel> CanBandBusiuserAsync(CanBandBusiuserRequest request)
        //{
        //    // 判断是否是顶级代理人
        //    var agent = agentRepository.GetAgent(request.Agent);
        //    if (agent == null || agent.IsDaiLi != 1)
        //    {
        //        return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "不是顶级代理人");
        //    }
        //    var md5Pwd = CommonHelper.GetMd5(UQ + request.Password);
        //    var busiuser = _orderRepository.GetBusiuserByUserNamePwd(request.UserName, md5Pwd);
        //    if (busiuser == null)
        //    {
        //        return BaseViewModel.GetBaseViewModel(BusinessStatusType.GetFailed, "账号或密码错误！");
        //    }
        //    // 根据bx_busiuser.Id获取设备列表，并且除去已经绑定的
        //    var busiuserList = await _orderRepository.GetBusiuserListAsync(busiuser.Id, agent.Id);

        //    return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, busiuserList);
        //}

        //public async Task<BaseViewModel> RelieveBandAsync(RelieveBandRequest request)
        //{
        //    var busiuser = await _orderRepository.FindAgentBusiuserAsync(request.AgentBusiuserId);
        //    if (busiuser == null)
        //        return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "采集账号不存在");
        //    if (busiuser.agent_id != request.Agent)
        //    {
        //        return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "采集账号不是本人的");
        //    }
        //    busiuser.is_used = 0;
        //    if (_orderRepository.UpdateAgentBusiuser(busiuser))
        //    {
        //        return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
        //    }
        //    return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
        //}

        //public GetBusiuserViewModel GetBusiuserList(GetBusiuserRequest request)
        //{
        //    var result = GetBusiuserViewModel.GetModel(BusinessStatusType.OK);
        //    result.ListBusiuser = _orderRepository.GetBusiuserByAgent(request.Agent);
        //    return result;
        //} 
        #endregion

        /// <summary>
        /// 生成订单编码
        /// </summary>
        /// <param name="fountain">平台编号 1crm2微信4app</param>
        /// <returns></returns>
        private string GenerateOrderNum(int fountain)
        {
            int randomnum = GetRandomNumber(99, 9999);
            var sb = new StringBuilder();
            sb.Append(fountain);
            sb.Append(DateTime.Now.ConvertToTimeStmap().ToString());
            sb.Append((10000 + randomnum).ToString());
            return sb.ToString();
        }
        /// <summary>
        /// 根据Guid生成随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static int GetRandomNumber(int min, int max)
        {
            int rtn = 0;
            Random r = new Random();
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            r = new Random(iSeed); //随机数
            rtn = r.Next(min, max + 1);
            return rtn;
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
        public int GetOrderStatus(int orderType, DateTime? issueTime, out string statusName)
        {
            statusName = string.Empty;
            //废弃状态不变
            if (orderType == 2 || orderType == 5)
            {
                statusName = orderType == 2 ? "已取消" : "已承保";
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
                        statusName = "待出单"; //待承保  20180108改动
                        break;
                }
                return orderType;
            }
        }

        public string getExplain(dd_order_steps model)
        {
            string returnStr = model == null
                ? ""
                : CommonHelper.ToListT<UpdateOrderDetile>("[" + model.json_content + "]").FirstOrDefault().Remark;
            return returnStr;
        }

        public string GetInsurances(dd_order_savequote model)
        {
            string Insurances = string.Empty;
            if (model.CheSun > 0)
            {
                Insurances = Insurances + "车损险，";
            }
            if (model.SanZhe > 0)
            {
                Insurances = Insurances + "三者险，";
            }
            if (model.DaoQiang > 0)
            {
                Insurances = Insurances + "盗抢险，";
            }
            if (model.SiJi > 0)
            {
                Insurances = Insurances + "司机座位险，";
            }
            if (model.ChengKe > 0)
            {
                Insurances = Insurances + "乘客座位险，";
            }
            if (model.BoLi > 0)
            {
                Insurances = Insurances + "玻璃险，";
            }
            if (model.HuaHen > 0)
            {
                Insurances = Insurances + "划痕险，";
            }
            if (model.BuJiMianCheSun > 0 || model.BuJiMianChengKe > 0 || model.BuJiMianDaoQiang > 0 || model.BuJiMianFuJian > 0 || model.BuJiMianHuaHen > 0 || model.BuJiMianJingShenSunShi > 0 || model.BuJiMianRenYuan > 0 || model.BuJiMianSanZhe > 0 || model.BuJiMianSheBeiSunshi > 0 || model.BuJiMianSheShui > 0 || model.BuJiMianSiJi > 0 || model.BuJiMianZiRan > 0)
            {
                Insurances = Insurances + "不计免赔险，";
            }
            return Insurances != "" ? Insurances.Substring(0, Insurances.Length - 1) : "";
        }

        /// <summary>
        /// 设置身份证采集状态
        /// </summary>
        /// <param name="verificationCodeStatus">数据库里面的采集状态</param>
        /// <param name="verificationCodeCreateTime">身份证验证码创建时间</param>
        /// <returns></returns>
        private int GetIdCardStatus(int verificationCodeStatus, DateTime? verificationCodeCreateTime)
        {
            if (verificationCodeStatus != 0 && verificationCodeCreateTime.HasValue)
            {
                // 如果采集状态不等于0，说明肯定有采集时间
                if (DateTime.Parse(verificationCodeCreateTime.Value.ToShortDateString()).AddDays(1).AddSeconds(-1) < DateTime.Now)
                {
                    // 采集有效期为当天
                    verificationCodeStatus = 2;
                }
            }
            return verificationCodeStatus;
        }

        /// <summary>
        /// 获取订单失效时间
        /// </summary>
        /// <param name="issueTime"></param>
        /// <param name="bizDate"></param>
        /// <param name="forceDate"></param>
        /// <returns></returns>
        private DateTime GetOrderLapseTime(DateTime issueTime, DateTime? bizDate, DateTime? forceDate)
        {
            //正常的核保的时间+29天
            issueTime = issueTime.AddDays(29);
            //要用于比较的时间
            DateTime dt = issueTime;
            //跟商业险时间比较，取最小值
            if (bizDate.HasValue)
            {
                if (bizDate.Value.Date != DateTime.MinValue.Date)
                {
                    dt = bizDate.Value > issueTime ? issueTime : bizDate.Value;
                }
            }
            if (forceDate.HasValue)
            {
                if (forceDate.Value.Date != DateTime.MinValue.Date)
                {
                    dt = forceDate.Value > dt ? dt : forceDate.Value;
                }
            }
            return dt;
        }

        /// <summary>
        /// 插入订单步骤记录
        /// </summary>
        /// <param name="remark"></param>
        /// <param name="orderId"></param>
        /// <param name="doAgentId"></param>
        /// <param name="toAgentId"></param>
        /// <param name="type">1创建订单2订单流转3支付4订单完成</param>
        private int WriteOrderSteps(string remark, long orderId, int doAgentId, int toAgentId, int type)
        {
            var model = new StepsModel
            {
                DoAgentId = doAgentId,
                ToAgentId = toAgentId,
                Remark = remark ?? ""
            };
            dd_order_steps orderSteps = new dd_order_steps()
            {
                json_content = model.ToJson(),
                create_time = DateTime.Now,
                agent_id = doAgentId,
                type = type,
                order_id = orderId
            };
            return _orderRepository.AddOrderSteps(orderSteps);
        }
        public class StepsModel
        {
            public int DoAgentId { get; set; }
            public int ToAgentId { get; set; }
            public string Remark { get; set; }
        }

        private int GetDetailPageType(int childagent, int agentid, int issuingagentid)
        {
            return childagent == issuingagentid ? 0 : 1;
        }

        public async Task<BaseViewModel> BandBusiuserAsync(BandBusiuserRequest request)
        {
            if (!request.ListBusiuserId.Any())
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            }

            var listBusiusersetting = await _orderRepository.GetBusiusersettingPartialAsync(request.ListBusiuserId);

            List<bx_agent_busiuser> listBusiuser = new List<bx_agent_busiuser>();
            foreach (var item in listBusiusersetting)
            {
                listBusiuser.Add(new bx_agent_busiuser
                {
                    agent_id = request.Agent,
                    busiuser_id = item.BusiuserId,
                    bx_busiusersetting_id = item.Id,
                    create_time = DateTime.Now,
                    is_used = 1,
                    source = item.Source
                });
            }
            if (await _orderRepository.BandAgentBusiuserAsync(listBusiuser))
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
        }

        public async Task<BaseViewModel> EditBusiuserAsync(EditBusiuserRequest request)
        {
            var agentBusiuser = await _orderRepository.FindAgentBusiuserAsync(request.AgentBusiuserId);
            if (agentBusiuser == null)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "采集器不存在");

            var busiuserSetting = await _orderRepository.GetBusiusersettingAsync(agentBusiuser.bx_busiusersetting_id);
            if (busiuserSetting == null)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.DataError, "当前绑定的采集器不可用");
            }
            var agentConfig = await _agentConfigRepository.FirstOrDefaultAsync(o => o.id == request.AgentConfigId && o.is_used == 1);
            if (agentConfig == null)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "渠道不可用");
            }

            if (!agentConfig.source.HasValue)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "渠道不可用");
            }
            // 判断采集器和渠道是否是同一个保险公司
            if (agentConfig.source.Value != ConvertHelper.GetOldSourceByInsuranceType(busiuserSetting.InsuranceType.Value))
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "渠道的保险公司和采集器的不同");

            agentBusiuser.bx_agent_config_id = request.AgentConfigId;

            if (_orderRepository.UpdateAgentBusiuser(agentBusiuser))
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
        }

        public async Task<BaseViewModel> GetAgentConfigAsync(GetAgentConfigRequest request)
        {
            // 获取bx_agent_busiuser信息
            var agentBusiuser = await _orderRepository.FindAgentBusiuserAsync(request.AgentBusiuserId);
            if (agentBusiuser == null)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "采集器不存在");

            var agentConfig = _orderRepository.GetCanUseAgentConfig(request.Agent, agentBusiuser.source, request.AgentBusiuserId);

            if (agentConfig == null)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.NoData);
            List<GetAgentConfigViewModel> list = new List<GetAgentConfigViewModel>();
            foreach (var item in agentConfig)
            {
                list.Add(new GetAgentConfigViewModel
                {
                    AgentConfigId = item.id,
                    ConfigName = item.config_name
                });
            }
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, list);
        }

        public async Task<BaseViewModel> CanBandBusiuserAsync(CanBandBusiuserRequest request)
        {
            var checkResult = await _checkCanBindBusiuserListService.CheckRequestAsync(request);
            if (checkResult.Item1.BusinessStatus != 1)
                return checkResult.Item1;

            // 根据bx_busiuser.Id获取设备列表，并且除去已经绑定的
            var busiuserList = await _orderRepository.GetBusiuserListAsync(-1, request.Agent);

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, busiuserList);
        }
        public async Task<BaseViewModel> RelieveBandAsync(RelieveBandRequest request)
        {
            var busiuser = await _orderRepository.FindAgentBusiuserAsync(request.AgentBusiuserId);
            if (busiuser == null)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "采集账号不存在");
            if (busiuser.agent_id != request.Agent)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "采集账号不是本人的");
            }
            busiuser.is_used = 0;
            if (_orderRepository.UpdateAgentBusiuser(busiuser))
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            }
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
        }

        public GetBusiuserViewModel GetBusiuserList(GetBusiuserRequest request)
        {
            var result = GetBusiuserViewModel.GetModel(BusinessStatusType.OK);
            result.ListBusiuser = _orderRepository.GetBusiuserByAgent(request.Agent);
            return result;
        }
        public OrderInformationViewModel GetOrderInformation(dd_order order)
        {
            try
            {
                string sql =
                    string.Format(@"SELECT * FROM dd_order_quoteresult WHERE dd_order_id = {0} ORDER BY update_time DESC LIMIT 0,1;
                                        SELECT * FROM dd_order_savequote WHERE dd_order_id = {0} ORDER BY update_time DESC LIMIT 0,1;
                                        SELECT * FROM bx_submit_info WHERE b_uid = {1} AND source = {2} ORDER BY update_time DESC LIMIT 0,1;  
                                        SELECT * FROM dd_order_related_info WHERE dd_order_id = {0} ORDER BY update_time DESC LIMIT 0,1;
                                        SELECT * FROM dd_order_paymentresult WHERE order_num = {3}; 
                                        SELECT * FROM bx_quotereq_carinfo WHERE b_uid = {1} ORDER BY update_time DESC LIMIT 0,1;  
                                        SELECT * FROM dd_order_steps WHERE order_id = {0} ORDER BY Id DESC;
                                        SELECT * FROM bx_agent_config WHERE id = {4} LIMIT 0,1;
                                        SELECT * FROM dd_order_commission WHERE order_id = {0};
                                        SELECT * FROM bx_agent WHERE id IN ({5}); ",
                        order.id, order.b_uid, order.source, order.order_num, order.quote_config_id,
                        order.agent_id + "," + order.issuing_people_id);
                return _orderRepository.GetOrderInformation(sql);
            }
            catch (Exception ex)
            {
                return new OrderInformationViewModel() { BusinessStatus = 0, StatusMessage = "查询信息异常：" + ex.Message };
            }

        }

        public bool PayMessage(string orderNum, int type)
        {
            var returnVal = false;
            var order = string.IsNullOrWhiteSpace(orderNum) ? null : _orderRepository.FindOrder(orderNum.Trim());
            //已承保
            if (type == 1)
            {
                if (order != null)
                {
                    //给业务员推送消息
                    var agent = agentRepository.GetAgent(order.agent_id);
                    if (agent != null)
                    {
                        var messageRequest = new PushMessageRequest()
                        {
                            MsgType = 10,
                            Agent = agent.TopAgentId,
                            OrderNum = order.order_num,
                            ChildAgent = order.agent_id,
                            LincenseNo = order.licenseno,
                            Buid = order.b_uid,
                            TitleStr = "您提交的订单（" + order.licenseno + "）已承保，请尽快处理！"
                        };
                        returnVal = _pushMessageService.PushMessage(messageRequest);
                    }


                    //给出单员推送消息
                    var id = order.issuing_people_id.HasValue ? order.issuing_people_id.Value : 0;
                    if (id > 0)
                    {
                        var issuAgent = agentRepository.GetAgent(Convert.ToInt32(id));
                        if (issuAgent != null)
                        {
                            var messageRequest1 = new PushMessageRequest()
                            {
                                MsgType = 10,
                                Agent = issuAgent.TopAgentId,
                                OrderNum = order.order_num,
                                ChildAgent = id,
                                LincenseNo = order.licenseno,
                                Buid = order.b_uid,
                                TitleStr = "您提交的订单（" + order.licenseno + "）已承保，请尽快处理！"
                            };
                            returnVal = _pushMessageService.PushMessage(messageRequest1);
                        }
                    }
                }
            }
            if (type == 2)
            {
                if (order != null)
                {
                    //给出单员推送消息
                    var id = order.issuing_people_id.HasValue ? order.issuing_people_id.Value : 0;
                    if (id > 0)
                    {
                        var issuAgent = agentRepository.GetAgent(Convert.ToInt32(id));
                        if (issuAgent != null)
                        {
                            var messageRequest1 = new PushMessageRequest()
                            {
                                MsgType = 10,
                                Agent = issuAgent.TopAgentId,
                                OrderNum = order.order_num,
                                ChildAgent = id,
                                LincenseNo = order.licenseno,
                                Buid = order.b_uid,
                                TitleStr = "您提交的订单（" + order.licenseno + "）已承保，请尽快处理！"
                            };
                            returnVal = _pushMessageService.PushMessage(messageRequest1);
                        }
                    }
                }
            }

            return returnVal;
        }

        public ListOrderAgentAmountViewModel GetTeamOrder(string agentIds, string startDate, string endDate)
        {
            try
            {
                string dateSql = "";
                if (!string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate))
                {
                    dateSql = string.Format("AND dd_order_paymentresult.payment_time BETWEEN '{0}' AND '{1}'", Convert.ToDateTime(startDate).ToShortDateString() + " 00:00:00",
                        Convert.ToDateTime(endDate).ToShortDateString() + " 00:00:00");
                }
                string sql =
                    string.Format(
                        @"SELECT dd_order.id AS OrderId,dd_order.licenseno AS LicenseNo,dd_order.order_num AS OrderNum,dd_order.purchase_amount AS PurchaseAmount,dd_order.agent_id AS AgentId,dd_order.issuing_people_id AS IssuingPeopleId FROM dd_order LEFT JOIN dd_order_paymentresult ON dd_order.id = dd_order_paymentresult.order_id AND dd_order_paymentresult.type = 1  WHERE dd_order.agent_id IN ({0}) AND dd_order.order_type = 5 AND dd_order_paymentresult.type = 1 {1}",
                        agentIds, dateSql);
                var listOrder = _orderRepository.GetTeamOrder(sql);

                return new ListOrderAgentAmountViewModel() { ListAgentAmount = listOrder, BusinessStatus = 1, StatusMessage = "查询信息成功" };

            }
            catch (Exception ex)
            {
                return new ListOrderAgentAmountViewModel() { BusinessStatus = 0, StatusMessage = "查询信息异常：" + ex.Message };
            }
        }
        public BaseViewModel RefreshOrderStatus(string orderNum)
        {
            try
            {
                var order = _orderRepository.FindOrder(orderNum);
                var submitInfo = _submitInfoRepository.GetSubmitInfo(order.b_uid, order.source);
                var ddOrderQuoteresult = _orderRepository.FindDdOrderQuoteresultAsync(order.id).Result;
                if (submitInfo.submit_status == 1 && submitInfo.quote_status == 1)
                {
                    ddOrderQuoteresult.quote_status = submitInfo.quote_status;
                    ddOrderQuoteresult.quote_result = submitInfo.quote_result;
                    ddOrderQuoteresult.submit_status = submitInfo.submit_status;
                    ddOrderQuoteresult.submit_result = submitInfo.submit_result;
                    _orderRepository.Update(ddOrderQuoteresult);
                    return BaseViewModel.GetBaseViewModel(1, "刷新订单状态成功", new { submitStatus = ddOrderQuoteresult.submit_status, submitResult = ddOrderQuoteresult.submit_result });
                }
                else return BaseViewModel.GetBaseViewModel(0, "因为不是报价核保状态不是成功，所以没刷新", new { submitStatus = ddOrderQuoteresult.submit_status, submitResult = ddOrderQuoteresult.submit_result });
            }
            catch (Exception ex)
            {
                return new BaseViewModel() { BusinessStatus = 0, StatusMessage = "查询信息异常：" + ex.Message };
            }
        }

        /// <summary>
        /// 修改订单配送信息 2018-09-14 张克亮
        /// </summary>
        /// <param name="request">修改订单请求模型</param>
        /// <returns></returns>
        public BaseViewModel UpdateOrderDeliveryInfo(OrderDeliveryInfoRequest request)
        {
            BaseViewModel baseViewModel = new BaseViewModel();

            try
            {
                //配送地址表更新结果
                int updateAddressResult = 0;
                //订单表更新结果
                int updateOrderResult = 0;
                //数据库事务
                using (TransactionScope scope = new TransactionScope())
                {
                    //配送地址Id为0则需要向配送地址表中增加新数据更新订单表中的配送地址ID
                    if (request.DeliveryAddressId == 0)
                    {
                        #region 向bx_address表中插入配送信息
                        var address = new bx_address();
                        if (!string.IsNullOrEmpty(request.DeliveryAddress) &&
                            //网点自取联系人可以为空，其它则不能为空
                            (request.DeliveryMethod == 2 ? true : !string.IsNullOrEmpty(request.DeliveryContacts)) &&
                            !string.IsNullOrEmpty(request.DeliveryContactsMobile))
                        {
                            address.address = request.DeliveryAddress;
                            address.provinceId = request.ProvinceId;
                            address.cityId = request.CityId;
                            address.areaId = request.AreaId;
                            address.province_name = request.ProvinceName;
                            address.city_name = request.CityName;
                            address.area_name = request.AreaName;
                            address.phone = request.DeliveryContactsMobile;
                            address.Name = request.DeliveryContacts;
                            address.agentId = request.AgentId;
                            address.createtime = DateTime.Now;
                            address.userid = 0;
                            address.Status = 1;
                            request.DeliveryAddressId = _iAddressRepository.Add(address);
                            //此处用来标记是否插入成功
                            updateAddressResult = request.DeliveryAddressId;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 更新bx_address配送信息
                        //更新配送地址表信息
                        var address = new bx_address();
                        address.id = request.DeliveryAddressId;
                        address.address = request.DeliveryAddress;
                        address.provinceId = request.ProvinceId;
                        address.cityId = request.CityId;
                        address.areaId = request.AreaId;
                        address.province_name = request.ProvinceName;
                        address.city_name = request.CityName;
                        address.area_name = request.AreaName;
                        address.phone = request.DeliveryContactsMobile;
                        address.Name = request.DeliveryContacts;
                        address.agentId = request.AgentId;
                        address.createtime = DateTime.Now;
                        address.userid = 0;
                        updateAddressResult = _iAddressRepository.Update(address);
                        #endregion
                    }

                    //更新订单表中的配送地址
                    if (updateAddressResult > 0)
                    {
                        updateOrderResult = _orderRepository.UpdateOrderDeliveryInfo(request);
                    }

                    //全都操作成功提交事务
                    if (updateAddressResult > 0 && updateOrderResult == 1)
                    {
                        scope.Complete();
                        baseViewModel.BusinessStatus = 1;
                        baseViewModel.StatusMessage = "更新订单配送地址信息成功";
                    }
                    else
                    {
                        //写入更新失败信息
                        baseViewModel.BusinessStatus = 0;
                        baseViewModel.StatusMessage = "更新订单配送地址信息失败";
                    }
                }
            }
            catch (Exception ex)
            {
                logErro.Error(string.Format("CustomerOrderService-UpdateOrderDeliveryInfo:入参【OrderDeliveryInfoRequest ={0}】 发生异常[{1}]", request.ToJson(), ex.Message), ex);
                baseViewModel.BusinessStatus = -1;
                baseViewModel.StatusMessage = "更新订单配送地址信息异常";
            }

            return baseViewModel;
        }

        /// <summary>
        /// 获取订单配送信息 2018-09-15 张克亮
        /// </summary>
        /// <param name="orderNum">订单号</param>
        /// <returns></returns>
        public BaseViewModel GetOrderDeliveryInfo(string orderNum)
        {
            BaseViewModel baseView = new BaseViewModel();
            try
            {
                //获取订单配送信息
                var addressInfo = _orderRepository.GetOrderDeliveryInfo(orderNum);
                //查询成功
                if (addressInfo != null)
                {
                    //封装配送地址信息及查询成功后的状态信息
                    baseView.BusinessStatus = 1;
                    baseView.StatusMessage = "获取订单配送信息成功";
                    baseView.Data = addressInfo;
                }
                else
                {
                    //查询失败返回结果置为失败状态结果
                    baseView.BusinessStatus = 1;
                    baseView.StatusMessage = "未获取到订单配送信息";
                }
            }
            catch (Exception ex)
            {
                logErro.Error(string.Format("CustomerOrderService-GetOrderDeliveryInfo:入参【orderNum ={0}】 发生异常[{1}]", orderNum.ToString(), ex.Message), ex);
                baseView.BusinessStatus = -1;
                baseView.StatusMessage = "获取订单配送信息异常";
            }
            return baseView;
        }

        public List<dd_order_quoteresult> GetOrderQuoteResultListByOrderId(List<long> orderIds)
        {
            return _orderRepository.GetOrderQuoteResultListByOrderId(orderIds);
        }
    }
}
