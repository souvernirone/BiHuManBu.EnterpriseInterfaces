using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;
using ServiceStack.Text;
using IUserInfoRepository = BiHuManBu.ExternalInterfaces.Models.IUserInfoRepository;
using BiHuManBu.ExternalInterfaces.Models.IRepository;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends
{
    public class OrderCorrelateService : IOrderCorrelateService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IUserClaimRepository _userClaimRepository;
        private readonly IConsumerDetailService _consumerDetailService;
        private readonly ISubmitInfoRepository _submitInfoRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IPushMessageService _pushMessageService;
        private readonly IQuoteReqCarInfoRepository _quoteReqCarInfo;
        private readonly ISetSaveCommissionService _setSaveCommissionService;

       

        public OrderCorrelateService(IOrderRepository orderRepository, IUserInfoRepository userInfoRepository, IUserClaimRepository userClaimRepository, IConsumerDetailService consumerDetailService, ISubmitInfoRepository submitInfoRepository, IAgentRepository agentRepository, IPushMessageService pushMessageService, IQuoteReqCarInfoRepository quoteReqCarInfo, ISetSaveCommissionService setSaveCommissionService)
        {
            _orderRepository = orderRepository;
            _userInfoRepository = userInfoRepository;
            _userClaimRepository = userClaimRepository;
            _consumerDetailService = consumerDetailService;
            _submitInfoRepository = submitInfoRepository;
            _agentRepository = agentRepository;
            _pushMessageService = pushMessageService;
            _quoteReqCarInfo = quoteReqCarInfo;
            _setSaveCommissionService = setSaveCommissionService;
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

        public void CreateOrderSuccessCorrelate(CreateOrderDetailRequest request, dd_order_related_info orderRelatedInfo, bx_userinfo userinfo, bx_quoteresult quoteresult, long orderId, dd_order order, bx_submit_info submitInfo, int sourceValue)
        {
            //2018-01-26 L 太平洋不推送消息
            if (order.source != 1)
            {
                #region 核保推送消息问题
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
                //核保状态，0=核保失败，1=核保成功,2=未核保,3=核保中,4=非意向公司未核保,5=报价失败未核保,6=核保关闭未核保
                if (submitInfo == null || submitInfo.submit_status == 2)
                {
                    var agent = _agentRepository.GetAgent(order.agent_id);
                    if (agent != null)
                    {
                        var messageRequest = new PushMessageRequest()
                        {
                            MsgType = 10,
                            Agent = agent.TopAgentId,
                            ChildAgent = order.agent_id,
                            OrderNum = order.order_num,
                            LincenseNo = order.licenseno,
                            Buid = order.b_uid,
                            TitleStr = "您收到一张核保失败的订单（" + order.licenseno + "），请尽快处理！"
                        };
                        var pushMessage = _pushMessageService.PushMessage(messageRequest);
                    }
                }
                else
                {
                    if (submitInfo.submit_status == 0)
                    {
                        //给业务员推送消息
                        var agent = _agentRepository.GetAgent(order.agent_id);
                        if (agent != null)
                        {
                            var messageRequest = new PushMessageRequest()
                            {
                                MsgType = 10,
                                Agent = agent.TopAgentId,
                                ChildAgent = order.agent_id,
                                OrderNum = order.order_num,
                                LincenseNo = order.licenseno,
                                Buid = order.b_uid,
                                TitleStr = "您提交的订单（" + order.licenseno + "）核保失败，请尽快处理！"
                            };
                            _pushMessageService.PushMessage(messageRequest);
                        }


                        //给出单员推送消息
                        var id = order.issuing_people_id.HasValue ? order.issuing_people_id.Value : 0;
                        if (id > 0)
                        {
                            var issuAgent = _agentRepository.GetAgent(Convert.ToInt32(id));
                            if (issuAgent != null)
                            {
                                var messageRequest1 = new PushMessageRequest()
                                {
                                    MsgType = 10,
                                    Agent = issuAgent.TopAgentId,
                                    ChildAgent = id,
                                    OrderNum = order.order_num,
                                    LincenseNo = order.licenseno,
                                    Buid = order.b_uid,
                                    TitleStr = "您收到一张未核保订单（" + order.licenseno + "），请尽快处理！"
                                };
                                _pushMessageService.PushMessage(messageRequest1);
                            }
                        }
                    }
                    //正常情况  创建订单  提醒出单员
                    if (submitInfo.submit_status == 1)
                    {
                        //给业务员推送消息
                        var agent = _agentRepository.GetAgent(order.agent_id);
                        if (agent != null)
                        {
                            var messageRequest = new PushMessageRequest()
                            {
                                MsgType = 10,
                                Agent = agent.TopAgentId,
                                ChildAgent = order.agent_id,
                                OrderNum = order.order_num,
                                LincenseNo = order.licenseno,
                                Buid = order.b_uid,
                                TitleStr = "您提交的订单（" + order.licenseno + "）已核保成功，请尽快支付！"
                            };
                            _pushMessageService.PushMessage(messageRequest);
                        }

                        //给出单员推送消息
                        var id = order.issuing_people_id.HasValue ? order.issuing_people_id.Value : 0;
                        if (id > 0)
                        {
                            var issuAgent = _agentRepository.GetAgent(Convert.ToInt32(id));
                            if (issuAgent != null)
                            {
                                var messageRequest1 = new PushMessageRequest()
                                {
                                    MsgType = 10,
                                    Agent = issuAgent.TopAgentId,
                                    ChildAgent = id,
                                    OrderNum = order.order_num,
                                    LincenseNo = order.licenseno,
                                    Buid = order.b_uid,
                                    TitleStr = "您收到一张核保成功订单（" + order.licenseno + "），请尽快处理！"
                                };
                                _pushMessageService.PushMessage(messageRequest1);
                            }
                        }

                    }
                    if (submitInfo.submit_status == 3)
                    {
                        var id = order.issuing_people_id.HasValue ? order.issuing_people_id.Value : 0;
                        if (id > 0)
                        {
                            var issuAgent = _agentRepository.GetAgent(Convert.ToInt32(id));
                            if (issuAgent != null)
                            {
                                var messageRequest1 = new PushMessageRequest()
                                {
                                    MsgType = 10,
                                    Agent = issuAgent.TopAgentId,
                                    ChildAgent = id,
                                    OrderNum = order.order_num,
                                    LincenseNo = order.licenseno,
                                    Buid = order.b_uid,
                                    TitleStr = "您收到一张核保中订单（" + order.licenseno + "），请尽快处理！"
                                };
                                _pushMessageService.PushMessage(messageRequest1);
                            }
                        }
                    }
                }
                #endregion

                #region 报价失败推送消息
                if (userinfo.QuoteStatus <= 0)
                {
                    var id = order.issuing_people_id.HasValue ? order.issuing_people_id.Value : 0;
                    if (id > 0)
                    {
                        var issuAgent = _agentRepository.GetAgent(Convert.ToInt32(id));
                        if (issuAgent != null)
                        {
                            var messageRequest1 = new PushMessageRequest()
                            {
                                MsgType = 10,
                                Agent = issuAgent.TopAgentId,
                                ChildAgent = id,
                                OrderNum = order.order_num,
                                LincenseNo = order.licenseno,
                                Buid = order.b_uid,
                                TitleStr = "您收到一张核保失败订单（" + order.licenseno + "），请尽快处理！"
                            };
                            _pushMessageService.PushMessage(messageRequest1);
                        }
                    }
                }
                #endregion
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
            
            var orderQuoteresult = _orderRepository.FindDdOrderQuoteresultAsync(orderId).Result;

           


            List<bx_claim_detail> claimDetails = _userClaimRepository.FindList(request.Buid);

            //商业
            if (orderQuoteresult != null)
            {
                var carinfo = _quoteReqCarInfo.FindAsync(order.b_uid).Result;

                //submitinfo
                orderQuoteresult.submit_status = submitInfo.submit_status;
                orderQuoteresult.quote_status = submitInfo.quote_status;
                orderQuoteresult.submit_result = submitInfo.submit_result;
                orderQuoteresult.quote_result = submitInfo.quote_result;
                orderQuoteresult.biz_end_time = submitInfo.biz_end_time;
                orderQuoteresult.force_end_time = submitInfo.force_end_time;
                orderQuoteresult.biz_pno = submitInfo.biz_pno;
                orderQuoteresult.force_pno = submitInfo.force_pno;
                //bx_quotereq_carinfo
                orderQuoteresult.auto_model_code = carinfo.auto_model_code;
                orderQuoteresult.biz_start_date = carinfo.biz_start_date;
                orderQuoteresult.force_start_date = carinfo.force_start_date;
                orderQuoteresult.car_used_type = carinfo.car_used_type;
                orderQuoteresult.PriceT = carinfo.PriceT;
                orderQuoteresult.seat_count = carinfo.seat_count;
                //bx_userinfo
                orderQuoteresult.CarVIN = userinfo.CarVIN;
                orderQuoteresult.RegisterDate = userinfo.RegisterDate;
                orderQuoteresult.EngineNo = userinfo.EngineNo;

                orderQuoteresult.biz_claim_count = claimDetails.Count(n => n.pay_type == 0);
                //交强
                orderQuoteresult.force_claim_count = claimDetails.Count(n => n.pay_type == 1);

                orderQuoteresult.expected_loss_rate = quoteresult == null ? null : quoteresult.expected_loss_rate;
                orderQuoteresult.biz_expected_loss_rate = quoteresult == null ? null : quoteresult.biz_expected_loss_rate;

                _orderRepository.Update(orderQuoteresult);
            }

            //设置佣金积分
            _setSaveCommissionService.Save(order.order_num, 2);
        }
    }
}
