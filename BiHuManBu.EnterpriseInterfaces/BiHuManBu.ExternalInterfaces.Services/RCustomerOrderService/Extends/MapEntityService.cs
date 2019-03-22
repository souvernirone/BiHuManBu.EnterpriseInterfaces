using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using ServiceStack.Text;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends
{
    public class MapEntityService : IMapEntityService
    {
        #region create
        public dd_order CreateMapOrder(CreateOrderDetailRequest request, dd_order order, bx_submit_info submitInfo, bx_userinfo userinfo, DateTime orderLapsetime, int sourceValue, bx_agent agent)
        {
            string orderNum = GenerateOrderNum(request.FromMethod);
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
                order.issuing_people_id = agent.Id;
                order.issuing_people_name = agent.AgentName;
                order.delivery_type = request.DeliveryType;
                order.remark = request.Remark;
                order.order_type = request.OrderType;
                order.order_lapse_time = orderLapsetime;
                order.issue_time = submitInfo == null ? DateTime.MinValue : submitInfo.create_time ?? DateTime.Now;//核保录入时间
                order.licenseno = request.Licenseno;
                order.source = sourceValue;
                order.quote_city_id = int.Parse(userinfo.CityCode);
                order.quote_config_id = submitInfo == null ? 0 : submitInfo.channel_id.HasValue ? (int)submitInfo.channel_id.Value : 0;
                order.force_tno = submitInfo == null ? "" : submitInfo.force_tno;
                order.biz_tno = submitInfo == null ? "" : submitInfo.biz_tno;
                order.insure_year = submitInfo == null ? DateTime.MinValue.Year : (submitInfo.create_time ?? DateTime.Now).Year;//投保年度为核保时间
                order.preferential = request.Preferential;
                order.get_order_method = request.GetOrderMethod;//1电子保单2纸质保单 //投保地区是北京的是电子保单
                order.total_amount = request.TotalAmount;
                order.purchase_amount = request.PurchaseAmount;
                order.receivable_amount = request.ReceivableAmount;
                order.force_rate = request.ForceRate;
                order.biz_rate = request.BizRate;
                order.MoldName = userinfo.MoldName;
                order.LastBizEndDate = string.IsNullOrWhiteSpace(request.LastBizEndDate) ? DateTime.MinValue : Convert.ToDateTime(request.LastBizEndDate);
                order.LastForceEndDate = string.IsNullOrWhiteSpace(request.LastForceEndDate) ? DateTime.MinValue : Convert.ToDateTime(request.LastForceEndDate);
                order.payee = request.Payee;
                order.pay_way_id = request.PayWayId;
                order.PayMentRemark = "";
                order.ax_order_num = request.AXOrderNum;
                order.CarOwnerId = request.CarOwnerId;
            }
            else
            {//没有订单，新增
                order = new dd_order()
                {
                    ax_order_num = request.AXOrderNum,
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
                    issuing_people_id = agent.Id,
                    issuing_people_name = agent.AgentName,
                    delivery_type = request.DeliveryType,
                    remark = request.Remark,
                    order_type = request.OrderType,
                    order_lapse_time = orderLapsetime,
                    issue_time = submitInfo == null ? DateTime.MinValue : submitInfo.create_time ?? DateTime.Now,//核保录入时间
                    licenseno = request.Licenseno,
                    source = sourceValue,
                    quote_city_id = int.Parse(userinfo.CityCode),
                    quote_config_id = submitInfo == null ? 0 : submitInfo.channel_id.HasValue ? (int)submitInfo.channel_id.Value : 0,
                    force_tno = submitInfo == null ? "" : submitInfo.force_tno,
                    biz_tno = submitInfo == null ? "" : submitInfo.biz_tno,
                    insure_year = submitInfo == null ? DateTime.MinValue.Year : (submitInfo.create_time ?? DateTime.Now).Year,
                    preferential = request.Preferential,
                    get_order_method = request.GetOrderMethod,//1电子保单2纸质保单 //投保地区是北京的是电子保单
                    total_amount = request.TotalAmount,
                    purchase_amount = request.PurchaseAmount,
                    receivable_amount = request.ReceivableAmount,
                    force_rate = request.ForceRate,
                    biz_rate = request.BizRate,
                    MoldName = userinfo.MoldName,
                    LastBizEndDate = string.IsNullOrWhiteSpace(request.LastBizEndDate) ? DateTime.MinValue : Convert.ToDateTime(request.LastBizEndDate),
                    LastForceEndDate = string.IsNullOrWhiteSpace(request.LastForceEndDate) ? DateTime.MinValue : Convert.ToDateTime(request.LastForceEndDate),
                    payee = request.Payee,
                    pay_way_id = request.PayWayId,
                    PayMentRemark = "",
                    CarOwnerId = request.CarOwnerId
                };
            }

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

            return order;
        }

        public dd_order_related_info CreateMapOrderRelatedInfo(CreateOrderDetailRequest request, bx_userinfo userinfo)
        {
            var orderRelatedInfo = new dd_order_related_info()
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
            return orderRelatedInfo;
        }


        public OrderDetail GetMapOrder(GetOrderDetailRequest request, dd_order order, bx_userinfo userinfo, PayWayBanksModel bank, bx_agent_config agentConfig, dd_order_quoteresult quoteresult, dd_order_savequote savequote, List<dd_order_steps> listSteps, List<dd_order_commission> listCommission, List<bx_agent> listAgents, List<dd_order_paymentresult> listoOrderPaymentresults)
        {
            string statusname = string.Empty;
            var agent = listAgents.FirstOrDefault(x => x.Id == order.agent_id);
            var issuingPeople = listAgents.FirstOrDefault(x => x.Id == order.issuing_people_id);
            var insured =
                listoOrderPaymentresults.Where(x => x.type == 1).OrderByDescending(x => x.id).FirstOrDefault();
            var netfee =
                listoOrderPaymentresults.Where(x => x.type == 2).OrderByDescending(x => x.id).FirstOrDefault();

            var commission = listCommission != null && listCommission.Count > 0 ? listCommission.Sum(x => x.money) : 0;
            var integral = listCommission != null && listCommission.Count > 0 ? listCommission.Sum(x => x.credit) : 0;
            var orderDetail = new OrderDetail()
            {
                Id = order.id,
                OrderNum = order.order_num,
                OrderGuid = order.order_guid,
                DeliveryMethod = order.delivery_method,
                DeliveryAddress = order.delivery_address ?? "",
                DeliveryContacts = order.delivery_contacts ?? "",
                DeliveryContactsMobile = order.delivery_contacts_mobile ?? "",
                //2018-09-15 加入配送地址Id
                DeliveryAddressId = order.delivery_address_id,
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
                QuoteConfigName = agentConfig == null ? "" : agentConfig.config_name ?? "",
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
                PayWayBankName = bank == null ? "" : bank.BankName,
                AgentMobile = agent == null ? "" : agent.Mobile ?? "",
                IssuingPeopleMobile = issuingPeople == null ? "" : issuingPeople.Mobile ?? "",
                InsuredDate = insured == null ? "" : insured.create_time.HasValue ? insured.create_time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                NetfeePayType = netfee == null ? 999 : netfee.pay_type.HasValue ? netfee.pay_type.Value : 999,
                QuoteGroup = userinfo.IsSingleSubmit.HasValue ? userinfo.IsSingleSubmit.Value : -1,
                SubmitGroup = userinfo.Source.HasValue ? userinfo.Source.Value : -1
                ,
                PayMentRemark = order.PayMentRemark,
                PayMent = order.PayMent,
                InputOrderLapseTime = order.order_lapse_time.HasValue ? order.order_lapse_time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                Commission = String.Format("{0:F}", commission),
                Integral = Convert.ToInt32(integral).ToString()
            };
            if (savequote != null && quoteresult != null)
            {
                orderDetail.BizStartDate = "";
                if (quoteresult.BizStartDate.HasValue && quoteresult.BizStartDate.Value.Date != DateTime.MinValue)
                {
                    orderDetail.BizStartDate = quoteresult.BizStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                orderDetail.ForceStartDate = "";
                if (quoteresult.ForceStartDate.HasValue && quoteresult.ForceStartDate.Value.Date != DateTime.MinValue)
                {
                    orderDetail.ForceStartDate = quoteresult.ForceStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }

            dd_order_steps step = listSteps.FirstOrDefault(l => l.type == 4);
            dd_order_steps step1 = listSteps.FirstOrDefault(l => l.type == 5);
            //订单流转记录表的4是踢回
            if (order.order_type == 3 && step != null)
            {
                CustomerOrderService.StepsModel jsonmodel = !string.IsNullOrEmpty(step.json_content) ? step.json_content.FromJson<CustomerOrderService.StepsModel>() : new CustomerOrderService.StepsModel();
                orderDetail.ReBackReason = jsonmodel.Remark ?? "";
                orderDetail.ReBackDate = step.create_time == null ? "" : step.create_time.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (order.order_type == 2 && step1 != null)
            {
                orderDetail.CancelReason = step1 != null ? string.IsNullOrWhiteSpace(step1.json_content) ? "" : CommonHelper.ToListT<UpdateOrderDetile>("[" + step1.json_content + "]").FirstOrDefault().Remark == null ? "" : CommonHelper.ToListT<UpdateOrderDetile>("[" + step1.json_content + "]").FirstOrDefault().Remark : "";
                orderDetail.CancelAgent = step1 != null ? string.IsNullOrWhiteSpace(step1.json_content) ? "" : CommonHelper.ToListT<UpdateOrderDetile>("[" + step1.json_content + "]").FirstOrDefault().AgentName == null ? "" : CommonHelper.ToListT<UpdateOrderDetile>("[" + step1.json_content + "]").FirstOrDefault().AgentName : "";
                orderDetail.CancelDate = step1.create_time == null ? "" : step1.create_time.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                orderDetail.ReBackReason = "";
            }
            orderDetail.CancelAgent = orderDetail.CancelAgent == null ? "" : orderDetail.CancelAgent;
            orderDetail.CancelReason = orderDetail.CancelReason == null ? "" : orderDetail.CancelReason;
            orderDetail.CancelDate = orderDetail.CancelDate == null ? "" : orderDetail.CancelDate;
            orderDetail.ReBackReason = orderDetail.ReBackReason == null ? "" : orderDetail.ReBackReason;
            orderDetail.ReBackDate = orderDetail.ReBackDate == null ? "" : orderDetail.ReBackDate;
            //记录哪个业务员取消的


            return orderDetail;
        }

        public PrecisePrice GetMapPrecisePrice(dd_order_quoteresult quoteresult, dd_order_savequote savequote, dd_order order, bx_submit_info submitinfo)
        {
            #region dd_order_quoteresult、dd_order_savequote模型转换
            if (savequote != null && quoteresult != null)
            {
                bool isquoteresult = true;
                var precisePrice = new PrecisePrice()
                {
                    AXOrderNum = string.IsNullOrWhiteSpace(order.ax_order_num) ? "" : order.ax_order_num,
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
                    QuoteStatus = quoteresult.quote_status ?? 0,
                    QuoteResult = quoteresult.quote_result ?? "",
                    SubmitStatus = quoteresult.submit_status ?? 0,
                    SubmitResult = quoteresult.submit_result ?? "",
                    JiaoQiang = quoteresult.ForceTotal.Value > 0 ? 1 : savequote.JiaoQiang.HasValue ? savequote.JiaoQiang.Value : 1,
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
                    },
                    PingAnScore = (quoteresult.PingAnScore ?? 0).ToString()
                };
                return precisePrice;
            }
            else
            {
                var pri = new PrecisePrice()
                {
                    AXOrderNum = "",
                    BizClaimCount = 0,
                    ForceClaimCount = 0,
                    ExpectedLossRate = 0,
                    BizExpectedLossRate = 0,
                    BizTotal = 0,
                    ForceTotal = 0,
                    TaxTotal = 0,
                    Source = SourceGroupAlgorithm.GetNewSource(order.source),
                    QuoteStatus = 0,
                    QuoteResult = "",
                    SubmitStatus = 0,
                    SubmitResult = "",
                    JiaoQiang = 1,
                    CheSun = new XianZhongUnit(),
                    SanZhe = new XianZhongUnit(),
                    DaoQiang = new XianZhongUnit(),
                    SiJi = new XianZhongUnit(),
                    ChengKe = new XianZhongUnit(),
                    BoLi = new XianZhongUnit(),
                    HuaHen = new XianZhongUnit(),
                    BuJiMianCheSun = new XianZhongUnit(),
                    BuJiMianSanZhe = new XianZhongUnit(),
                    BuJiMianDaoQiang = new XianZhongUnit(),
                    BuJiMianRenYuan = new XianZhongUnit(),
                    BuJiMianFuJia = new XianZhongUnit(),
                    BuJiMianChengKe = new XianZhongUnit(),
                    BuJiMianSiJi = new XianZhongUnit(),
                    BuJiMianHuaHen = new XianZhongUnit(),
                    BuJiMianSheShui = new XianZhongUnit(),
                    BuJiMianZiRan = new XianZhongUnit(),
                    BuJiMianJingShenSunShi = new XianZhongUnit(),
                    HcSheBeiSunshi = new XianZhongUnit(),
                    HcHuoWuZeRen = new XianZhongUnit(),
                    HcFeiYongBuChang = new XianZhongUnit(),
                    HcJingShenSunShi = new XianZhongUnit(),
                    HcSanFangTeYue = new XianZhongUnit(),
                    HcXiuLiChang = new XianZhongUnit(),
                    HcXiuLiChangType = "",
                    SheShui = new XianZhongUnit(),
                    CheDeng = new XianZhongUnit(),
                    ZiRan = new XianZhongUnit(),
                    RateFactor1 = 0,
                    RateFactor2 = 0,
                    RateFactor3 = 0,
                    RateFactor4 = 0,
                    TotalRate = "0",
                    Fybc = new XianZhongUnit(),
                    FybcDays = new XianZhongUnit(),
                    SheBeiSunShi = new XianZhongUnit(),
                    BjmSheBeiSunShi = new XianZhongUnit(),
                    SanZheJieJiaRi = new XianZhongUnit(),
                    PingAnScore = ""
                };
                return pri;
            }

            #endregion
        }

        public RelatedInfo GetMapRelatedInfo(dd_order_related_info related_info)
        {
            RelatedInfo relatedInfo = new RelatedInfo();
            #region dd_order_related_info模型转换
            if (related_info != null)
            {
                //关系人模型
                #region holder
                relatedInfo.HolderPerson = new RelatedPerson()
                {
                    Name = related_info.holder_name ?? "",
                    IdType = related_info.holder_id_type.HasValue ? related_info.holder_id_type.Value : 0,
                    IdCard = related_info.holder_id_card ?? "",
                    Sex = related_info.holder_sex.HasValue ? related_info.holder_sex.Value : 0,
                    Nation = related_info.holder_nation??"",
                    Birthday =
                        related_info.holder_birthday.HasValue
                            ? (related_info.holder_birthday.Value.ToString("yyyy-MM-dd"))
                            : "",
                    Address = related_info.holder_address ?? "",
                    CertiStartDate =
                        related_info.holder_certi_start_date.HasValue
                            ? related_info.holder_certi_start_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    CertiEndDate =
                        related_info.holder_certi_end_date.HasValue
                            ? related_info.holder_certi_end_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    Authority = related_info.holder_authority ?? "",
                    Mobile = related_info.holder_mobile ?? "",
                    Email = related_info.holder_email ?? ""
                };
                #endregion
                #region insured
                relatedInfo.InsuredPerson = new RelatedPerson()
                {
                    Name = related_info.insured_name ?? "",
                    IdType = related_info.insured_id_type.HasValue ? related_info.insured_id_type.Value : 0,
                    IdCard = related_info.insured_id_card ?? "",
                    Sex = related_info.insured_sex.HasValue ? related_info.insured_sex.Value : 0,
                    Nation = related_info.insured_nation ?? "",
                    Birthday =
                        related_info.insured_birthday.HasValue
                            ? related_info.insured_birthday.Value.ToString("yyyy-MM-dd")
                            : "",
                    Address = related_info.insured_address ?? "",
                    CertiStartDate =
                        related_info.insured_certi_start_date.HasValue
                            ? related_info.insured_certi_start_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    CertiEndDate =
                        related_info.insured_certi_end_date.HasValue
                            ? related_info.insured_certi_end_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    Authority = related_info.insured_authority ?? "",
                    Mobile = related_info.insured_mobile ?? "",
                    Email = related_info.insured_email ?? ""
                };
                #endregion
                #region ower
                relatedInfo.OwerPerson = new RelatedPerson()
                {
                    Name = related_info.ower_name ?? "",
                    IdType = related_info.ower_id_type.HasValue ? related_info.ower_id_type.Value : 0,
                    IdCard = related_info.ower_id_card ?? "",
                    Sex = related_info.ower_sex.HasValue ? related_info.ower_sex.Value : 0,
                    Nation = related_info.ower_nation ?? "",
                    Birthday =
                        related_info.ower_birthday.HasValue
                            ? related_info.ower_birthday.Value.ToString("yyyy-MM-dd")
                            : "",
                    Address = related_info.ower_address ?? "",
                    CertiStartDate =
                        related_info.ower_certi_start_date.HasValue
                            ? related_info.ower_certi_start_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    CertiEndDate =
                        related_info.ower_certi_end_date.HasValue
                            ? related_info.ower_certi_end_date.Value.ToString("yyyy-MM-dd")
                            : "",
                    Authority = related_info.ower_authority ?? "",
                    Mobile = related_info.ower_mobile ?? "",
                    Email = related_info.ower_email ?? ""
                };
                #endregion
            }
            else
            {
                relatedInfo.HolderPerson = new RelatedPerson()
                {
                    Name = "",
                    IdType = 0,
                    IdCard = "",
                    Sex = 0,
                    Nation = "",
                    Birthday = "",
                    Address = "",
                    CertiStartDate = "",
                    CertiEndDate = "",
                    Authority = "",
                    Mobile = "",
                    Email = ""
                };
                relatedInfo.InsuredPerson = new RelatedPerson()
                {
                    Name = "",
                    IdType = 0,
                    IdCard = "",
                    Sex = 0,
                    Nation = "",
                    Birthday = "",
                    Address = "",
                    CertiStartDate = "",
                    CertiEndDate = "",
                    Authority = "",
                    Mobile = "",
                    Email = ""
                };
                relatedInfo.OwerPerson = new RelatedPerson()
                {
                    Name = "",
                    IdType = 0,
                    IdCard = "",
                    Sex = 0,
                    Nation = "",
                    Birthday = "",
                    Address = "",
                    CertiStartDate = "",
                    CertiEndDate = "",
                    Authority = "",
                    Mobile = "",
                    Email = ""
                };
            }
            #endregion

            return relatedInfo;
        }

        public PaymentResult GetMapPaymentResult(List<dd_order_paymentresult> paymentresult)
        {
            #region dd_order_paymentresult模型转换

            //净费支付信息
            var jinfei = paymentresult.FirstOrDefault(l => l.type == 2);
            var paymentResult = new PaymentResult()
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
            paymentResult.BizNo = quankuan != null ? quankuan.biz_no ?? string.Empty : string.Empty;
            paymentResult.IsPaymentTime = quankuan != null
                ? quankuan.payment_time.HasValue ? quankuan.payment_time.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""
                : "";
            paymentResult.ForceNo = quankuan != null ? quankuan.force_no ?? string.Empty : string.Empty;
            return paymentResult;

            #endregion
        }

        public OrderCarInfo GetPayOrderCarInfo(bx_quotereq_carinfo carinfo, dd_order order, bx_userinfo userinfo, dd_order_quoteresult quoteresult)
        {
            #region bx_quotereq_carinfo模型转换
            var carInfo = new OrderCarInfo()
            {
                AutoMoldCode = quoteresult != null ? (quoteresult.auto_model_code ?? "") : "", //精友编码
                BizStartDate = quoteresult != null ?
                    (quoteresult.biz_start_date.HasValue ? quoteresult.biz_start_date.Value.ToString("yyyy-MM-dd HH:mm") : "") : "", //商业险起保时间
                ForceStartDate = quoteresult != null ?
                    (quoteresult.force_start_date.HasValue ? quoteresult.force_start_date.Value.ToString("yyyy-MM-dd HH:mm") : "") : "", //交强险起保时间
                LicenseNo = order.licenseno,
                CarUsedType = quoteresult != null ? (quoteresult.car_used_type ?? 0) : 0, //车辆类型
                CarVin = quoteresult == null ? "" : string.IsNullOrWhiteSpace(quoteresult.CarVIN) ? "" : quoteresult.CarVIN, //车辆识别代码
                EngineNo = quoteresult == null ? "" : quoteresult.EngineNo ?? "", //发动机号
                PurchasePrice = quoteresult != null ? (quoteresult.PriceT ?? 0) : 0, //新车购置价
                RegisterDate = quoteresult != null ? quoteresult.RegisterDate ?? "" : "", //注册日期
                MoldName = userinfo == null ? "" : userinfo.MoldName ?? "",
                SeatCount = quoteresult != null ? (quoteresult.seat_count ?? 0) : 0,
                IsLoans = carinfo != null ? (carinfo.is_loans ?? -1) : -1,
                TransferDate = carinfo != null ? (carinfo.transfer_date.HasValue ? carinfo.transfer_date.Value.ToString("yyyy-MM-dd HH:mm:ss") : "") : ""
            };
            return carInfo;

            #endregion
        }

        #endregion


        /// <summary>
        /// 生成订单编码
        /// </summary>
        /// <param name="fountain">平台编号 1crm2微信4app</param>
        /// <returns></returns>
        public string GenerateOrderNum(int fountain)
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

        private int GetDetailPageType(int childagent, int agentid, int issuingagentid)
        {
            return childagent == issuingagentid ? 0 : 1;
        }

        public ChangeStr GetChangeStr(CreateOrderDetailRequest request)
        {
            try
            {
                var totalRateChangeStr = request.OldTotalRate != request.TotalRate && request.OldTotalRate > 0
                ? "折扣系数由" + request.OldTotalRate + "变化为" + request.TotalRate : "";
                //使用性质 1：家庭自用车（默认），2：党政机关、事业团体，3：非营业企业客车，4：不区分营业非营业（仅人保），5：出租租赁（仅人保），6：营业货车（仅人保），7：非营业货车（仅人保）

                var list = new List<string>()
                {
                    "家庭自用车",
                    "党政机关、事业团体",
                    "非营业企业客车",
                    "不区分营业非营业",
                    "出租租赁",
                    "营业货车",
                    "非营业货车"
                };

                //当报价失败的时候当前车辆使用性质CarUsedType无值   默认为0 就导致索引出错
                var carUsedTypeChangeStr = "";
                if (request.CarUsedType > 0 && request.CarUsedType > 0)
                {
                    request.CarUsedType = request.CarUsedType > 7 ? 7 : request.CarUsedType;
                    request.OldCarUsedType = request.OldCarUsedType > 7 ? 7 : request.OldCarUsedType;
                    carUsedTypeChangeStr = request.OldCarUsedType != request.CarUsedType && request.OldCarUsedType > 0
                    ? "车辆使用性质由" + list[request.OldCarUsedType - 1] + "变化为" +
                      list[request.CarUsedType - 1]
                    : "";
                }

                return new ChangeStr()
                {
                    TotalRateChangeStr = totalRateChangeStr,
                    CarUsedTypeChangeStr = carUsedTypeChangeStr,
                    OldPurchaseAmount = request.OldPurchaseAmount,
                    PurchaseAmount = request.PurchaseAmount
                };
            }
            catch (Exception ex)
            {
                return new ChangeStr()
                {
                    TotalRateChangeStr = "",
                    CarUsedTypeChangeStr = "",
                    OldPurchaseAmount = request.OldPurchaseAmount,
                    PurchaseAmount = request.PurchaseAmount
                };
            }

        }
    }
}
