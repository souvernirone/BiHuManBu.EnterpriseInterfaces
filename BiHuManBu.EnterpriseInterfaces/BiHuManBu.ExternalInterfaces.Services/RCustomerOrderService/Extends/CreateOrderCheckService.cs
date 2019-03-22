
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using IAgentConfigRepository = BiHuManBu.ExternalInterfaces.Models.IAgentConfigRepository;
using IUserInfoRepository = BiHuManBu.ExternalInterfaces.Models.IUserInfoRepository;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using ServiceStack.Text;
using System.Text;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends
{
    public class CreateOrderCheckService : ICreateOrderCheckService
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ISubmitInfoRepository _submitInfoRepository;
        private readonly IAgentConfigRepository _agentConfigRepository;
        private readonly ISaveQuoteRepository _saveQuoteRepository;
        private readonly IQuoteResultRepository _quoteResultRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IPushMessageService _pushMessageService;
        private readonly ISetIssuingPeopleService _setIssuingPeopleService;
        private readonly IQuoteReqCarInfoRepository _quoteReqCarInfoRepository;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private readonly ILog logError = LogManager.GetLogger("ERROR");

        public CreateOrderCheckService(IUserInfoRepository userInfoRepository, IOrderRepository orderRepository, ISubmitInfoRepository submitInfoRepository, IAgentConfigRepository agentConfigRepository, ISaveQuoteRepository saveQuoteRepository, IQuoteResultRepository quoteResultRepository, IAgentRepository agentRepository, IPushMessageService pushMessageService, ISetIssuingPeopleService setIssuingPeopleService, IQuoteReqCarInfoRepository quoteReqCarInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
            _orderRepository = orderRepository;
            _submitInfoRepository = submitInfoRepository;
            _agentConfigRepository = agentConfigRepository;
            _saveQuoteRepository = saveQuoteRepository;
            _quoteResultRepository = quoteResultRepository;
            _agentRepository = agentRepository;
            _pushMessageService = pushMessageService;
            _setIssuingPeopleService = setIssuingPeopleService;
            _quoteReqCarInfoRepository = quoteReqCarInfoRepository;
        }

        public CheckOrderView CreateOrderCheck(CreateOrderDetailRequest request)
        {
            try
            {
                var agentDetail = _agentRepository.GetAgent(request.AgentId);

                //获取随机分配的业务员信息
                var agent = _setIssuingPeopleService.SetIssuingPeople(agentDetail.TopAgentId);
                if (agent == null)
                {
                    return new CheckOrderView()
                    {
                        OrderId = 0,
                        OrderNum = "",
                        BusinessStatus = 0,
                        StatusMessage = "未设置出单员，请由系统管理员设置出单员后再提交订单"
                    };
                }


                if (request.FromMethod != 1 && request.FromMethod != 2 && request.FromMethod != 4 && request.FromMethod != 8)
                {//来源平台限制
                    return new CheckOrderView()
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
                    return new CheckOrderView()
                    {
                        OrderId = 0,
                        OrderNum = "",
                        BusinessStatus = 0,
                        StatusMessage = "创建失败，非法状态参数"
                    };
                }

                if (request.Source <= 0)
                {//订单状态限制
                    return new CheckOrderView()
                    {
                        OrderId = 0,
                        OrderNum = "",
                        BusinessStatus = 0,
                        StatusMessage = "创建失败，Source参数错误，必须是大于0"
                    };
                }
                var userinfo = _userInfoRepository.FindByBuid(request.Buid);

                if (userinfo == null)
                {//buid限制
                    return new CheckOrderView()
                    {
                        OrderId = 0,
                        OrderNum = "",
                        BusinessStatus = 0,
                        StatusMessage = "创建失败，无报价信息"
                    };
                }
                ////报价失败也可以创建订单
                //if (userinfo.QuoteStatus <= 0)
                //{
                //    return new CheckOrderView()
                //    {
                //        OrderId = 0,
                //        OrderNum = "",
                //        BusinessStatus = 0,
                //        StatusMessage = "创建失败，报价未成功"
                //    };
                //}
                if (request.OrderType == 0)
                {
                    userinfo.IsChangeRelation = userinfo.IsChangeRelation == 1 ? 1 : request.IsChangeRelated;
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
                        return new CheckOrderView()
                        {
                            OrderId = 0,
                            OrderNum = "",
                            BusinessStatus = 0,
                            StatusMessage = "创建失败，配送信息：联系人姓名、联系人电话，配送地址不能为空"
                        };
                    }
                }

                //查某buid下所有的订单
                var orderList = _orderRepository.FindOrderListByBuid(request.Buid);
                //查已存在出过单或正在进行的订单，就不让出单
                long ddOrderExit = 0;
                ///过期删掉

                //如果是暂存的单子，让其直接过，在下面执行编辑操作
                if (orderList.Any(i => i.order_lapse_time.HasValue && i.order_lapse_time > DateTime.Now && (new int?[] { 0, 3, 41, 42 }).Contains(i.order_type)))
                    ddOrderExit = orderList.Select(l => l.id).FirstOrDefault();
                if (ddOrderExit > 0)
                {
                    return new CheckOrderView()
                    {
                        OrderId = 0,
                        OrderNum = "",
                        BusinessStatus = 0,
                        StatusMessage = "创建失败，该车已存在订单"
                    };
                }

                #region 获取险种信息
                var sourceValue = 0;
                sourceValue = SourceGroupAlgorithm.GetOldSource(request.Source);
                var submitinfo = new bx_submit_info();
                //延长30秒返回submitinfo //因为以前经常拿到报价结果或者超时了，submitinfo没结果，跟中心沟通无果
                for (int i = 0; i < 30; i++)
                {
                    //循环获取bx_submit_info
                    submitinfo = _submitInfoRepository.GetSubmitInfoAsync(request.Buid, sourceValue).Result;
                    if (submitinfo != null && submitinfo.id > 0 && submitinfo.channel_id.HasValue && submitinfo.channel_id.Value > 0)
                    {
                        logInfo.Info(string.Format("创建订单轮循核保第{0}秒返回正确的结果",i));
                        break;
                    }
                    Task.Delay(TimeSpan.FromSeconds(1));
                }
                #endregion
                long channelid = submitinfo == null ? 0 : submitinfo.channel_id.HasValue ? submitinfo.channel_id.Value : 0;
                var agentConfig = _agentConfigRepository.GetAgentConfigById(channelid);
                if (agentConfig == null || agentConfig.is_used != 1)
                {
                    StringBuilder sbInfo = new StringBuilder();
                    sbInfo.Append(request.Buid).Append("订单创建失败渠道不可用：").Append(submitinfo == null ? "submitinfo为null" : channelid.ToString());
                    sbInfo.Append("；").Append(agentConfig == null ? "agentConfig为null" : agentConfig.is_used.ToString());
                    logInfo.Info(sbInfo.ToString());
                    return new CheckOrderView()
                    {
                        OrderId = 0,
                        OrderNum = "",
                        BusinessStatus = 0,
                        StatusMessage = "创建失败，渠道不可用"
                    };
                }

                #region 获取核保信息
                var savequote = new bx_savequote();
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
                var quoteresult = new bx_quoteresult();
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
                //if (savequote == null || quoteresult == null)
                //{
                //    return new CheckOrderView()
                //    {
                //        OrderId = 0,
                //        OrderNum = "",
                //        BusinessStatus = 0,
                //        StatusMessage = "创建失败，无险种信息"
                //    };
                //}
                //if (savequote.Id == 0 || quoteresult.Id == 0)
                //{
                //    return new CheckOrderView()
                //    {
                //        OrderId = 0,
                //        OrderNum = "",
                //        BusinessStatus = 0,
                //        StatusMessage = "创建失败，无险种信息"
                //    };
                //}
                #endregion

                //当报价的交强开始时间大于当前时间就按照交强开始时间加3天作为截止时间   //2018-01-29  之前这个需求有问题
                //var issueTime = quoteresult == null ? DateTime.Now : quoteresult.ForceStartDate ?? DateTime.Now;//出单时间

                var quotereq = _quoteReqCarInfoRepository.Find(request.Buid);
                DateTime? fdate;//交强险起保时间
                if (quoteresult != null)
                {
                    fdate = quoteresult.ForceStartDate ?? DateTime.MinValue;
                }
                else
                {
                    fdate = quotereq != null ? (quotereq.force_start_date ?? DateTime.MinValue) : DateTime.MinValue;
                }
                logInfo.Info("fdate初始化" + fdate.Value ?? "0");
                DateTime? bdate;//商业险起保时间
                if (quoteresult != null)
                {
                    bdate = quoteresult.BizStartDate ?? DateTime.MinValue;
                }
                else
                {
                    bdate = quotereq != null ? (quotereq.biz_start_date ?? DateTime.MinValue) : DateTime.MinValue;
                }
                logInfo.Info("bdate初始化" + bdate.Value ?? "0");
                //判断是否最小时间
                fdate = fdate == DateTime.MinValue ? null : fdate;
                bdate = bdate == DateTime.MinValue ? null : bdate;
                var orderLapsetime = DateTime.Now;//初始化订单失效时间
                logInfo.Info("orderLapsetime初始化" + orderLapsetime.ToString());
                if (fdate != null && bdate != null && fdate == bdate && fdate > orderLapsetime.AddDays(3))
                {
                    orderLapsetime = DateTime.Now.AddDays(3);
                    logInfo.Info("1:" + orderLapsetime);
                }
                else if (fdate != null && bdate != null && fdate == bdate)
                {
                    orderLapsetime = fdate.Value;
                    logInfo.Info("2:" + orderLapsetime);
                }
                else if (fdate != null && bdate != null && fdate < bdate && fdate > orderLapsetime.AddDays(3))
                {
                    orderLapsetime = DateTime.Now.AddDays(3);
                    logInfo.Info("3:" + orderLapsetime);
                }
                else if (fdate != null && bdate != null && fdate < bdate)
                {
                    orderLapsetime = fdate.Value;
                    logInfo.Info("4:" + orderLapsetime);
                }
                else if (fdate != null && bdate != null && fdate > bdate && bdate > orderLapsetime.AddDays(3))
                {
                    orderLapsetime = DateTime.Now.AddDays(3);
                    logInfo.Info("5:" + orderLapsetime);
                }
                else if (fdate != null && bdate != null && fdate > bdate)
                {
                    orderLapsetime = bdate.Value;
                    logInfo.Info("6:" + orderLapsetime);
                }
                else if (fdate == null && bdate != null && bdate > orderLapsetime.AddDays(3))
                {
                    orderLapsetime = DateTime.Now.AddDays(3);
                    logInfo.Info("7:" + orderLapsetime);
                }
                else if (fdate == null && bdate != null)
                {
                    orderLapsetime = bdate.Value;
                    logInfo.Info("8:" + orderLapsetime);
                }
                else if (fdate != null && bdate == null && fdate > orderLapsetime.AddDays(3))
                {
                    orderLapsetime = DateTime.Now.AddDays(3);
                    logInfo.Info("9:" + orderLapsetime);
                }
                else if (fdate != null && bdate == null)
                {
                    orderLapsetime = fdate.Value;
                    logInfo.Info("10:" + orderLapsetime);
                }
                else
                {
                    orderLapsetime = orderLapsetime.AddHours(-1);
                    logInfo.Info("11:" + orderLapsetime);
                }

                if (orderLapsetime < DateTime.Now)
                {
                    return new CheckOrderView()
                    {
                        OrderId = 0,
                        OrderNum = "",
                        BusinessStatus = 0,
                        StatusMessage = "创建失败，此订单距离上次操作时间过长，请重新报价"
                    };
                }

                dd_order order = null;

                return new CheckOrderView()
                {
                    Agent = agent,
                    SourceValue = sourceValue,
                    BusinessStatus = 1,
                    StatusMessage = "",
                    Userinfo = userinfo,
                    SubmitInfo = submitinfo,
                    AgentConfig = agentConfig,
                    Savequote = savequote,
                    Quoteresult = quoteresult,
                    OrderLapsetime = orderLapsetime,
                    Order = order,
                    TopAgent=agentDetail.TopAgentId
                };
            }
            catch (Exception ex)
            {
                logError.Info("创建订单异常，创建订单信息：" + request.ToJson() + "\n 异常信息:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return new CheckOrderView()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = -10003,
                    StatusMessage = "创建失败，检查订单信息发生异常，请重新提交一次"
                };
            }

        }

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
    }
}
