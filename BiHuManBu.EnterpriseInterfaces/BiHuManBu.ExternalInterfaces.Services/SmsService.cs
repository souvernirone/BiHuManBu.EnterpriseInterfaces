using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Sms;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Sms;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response.Sms;
using log4net;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Net;
using System.Transactions;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class SmsService : CommonBehaviorService, ISmsService
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ISmsContentRepository _smsContentRepository;
        private ISmsOrderRepository _smsOrderRepository;
        private IAgentRepository _agentRepository;

        private static readonly string _smsCenter = System.Configuration.ConfigurationManager.AppSettings["SmsCenter"];
        private static readonly string _smsCenterSendSmsMethod = System.Configuration.ConfigurationManager.AppSettings["SmsCenterSendSmsMethod"];
        private static readonly string _smsSqlInterfaceUrl = System.Configuration.ConfigurationManager.AppSettings["SqlInterfaceUrl"];

        private static readonly string _smsAccountConfig = "{0}-bihu";

        public SmsService(IAgentRepository agentRepository, ICacheHelper cacheHelper,
            ISmsContentRepository smsContentRepository, ISmsOrderRepository smsOrderRepository)
            : base(agentRepository, cacheHelper)
        {
            _smsContentRepository = smsContentRepository;
            _smsOrderRepository = smsOrderRepository;
            _agentRepository = agentRepository;
        }

        public SmsResultViewModel_sms SendSms(SmsRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new SmsResultViewModel_sms();

            //参数校验
            //获取顶级经纪人信息
            bx_agent agentModel = _agentRepository.GetAgent(request.Agent);
            //获取当前经纪人信息
            bx_agent curagentModel = _agentRepository.GetAgent(request.CurAgent);
            if (agentModel == null || curagentModel == null)
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }
            //if (!ValidateReqest(pairs, agentModel.SecretKey, request.SecCode))
            //{
            //    response.Status = HttpStatusCode.Forbidden;
            //    return response;
            //}
            if ((int)request.BusinessType == 3 && string.IsNullOrWhiteSpace(request.LicenseNo))
            {
                response.Status = HttpStatusCode.UnsupportedMediaType;
                return response;
            }

            bool isByTopAgent = IsPayByTopAgent(request.CurAgent);//true为走顶级代理人账户,否则走当前账户
            int sendAgent = 0;//以哪个代理人id创建
            int messagePayType = 0;

            //当前代理人是顶级代理人可以充值
            if (isByTopAgent)
            {//顶级
                sendAgent = request.Agent;
                messagePayType = agentModel.MessagePayType.HasValue ? agentModel.MessagePayType.Value : 0;
            }
            else
            {//当前代理
                sendAgent = request.CurAgent;
                messagePayType = curagentModel.MessagePayType.HasValue ? curagentModel.MessagePayType.Value : 0;
            }

            //获取代理人的短信信息
            var smsAcount = _smsContentRepository.Find(sendAgent);
            if (smsAcount == null)
            {
                response.MessagePayType = messagePayType;
                response.Status = HttpStatusCode.NoContent;
                return response;
            }
            string posturl = string.Format("{0}/{1}", _smsCenter, _smsCenterSendSmsMethod);
            string postData = string.Format("account={0}&password={1}&mobile={2}&smscontent={3}&businessType={4}",
                smsAcount.sms_account, smsAcount.sms_password, request.Mobile, request.SmsContent, (int)request.BusinessType);
            string result;
            try
            {
                int ret = HttpWebAsk.Post(posturl, postData, out result);
                //result = "{\"ResponseType\":\"SubmitSms\",\"TradeTime\":\"2016-05-19 19:35:03\",\"ResultCode\":0,\"Message\":\"提交成功！\"}";
                if (!string.IsNullOrEmpty(result))
                {
                    var smsResult = result.FromJson<SmsResultModel>();
                    if (smsResult.ResultCode == 0)
                    {
                        //调取后台接口，将短信发送内容保存到bx_sms_account_content
                        string getData = string.Format("agent_id={0}&sent_mobile={1}&content={2}&agent_name={3}&sent_type={4}&&businessType={5}&license_no={6}",
                            sendAgent, request.Mobile, request.SmsContent, curagentModel.AgentName, request.SentType, (int)request.BusinessType, request.LicenseNo);
                        string getUrl = _smsSqlInterfaceUrl + getData;
                        logInfo.Info(string.Format("短信内容入库请求串：{0}", getUrl));
                        //string getResult = HttpWebAsk.HttpGet(getUrl);
                        string insertsms = string.Empty;
                        int insertret = HttpWebAsk.Post(_smsSqlInterfaceUrl, getData, out insertsms);
                        logInfo.Info(string.Format("短信内容入库返回结果：{0}", insertsms));

                        response.BusinessStatus = 1;
                        response.MessagePayType = messagePayType;
                        response.StatusMessage = "短信发送成功";
                        response.SmsResultModel = smsResult;
                    }
                    else
                    {
                        response.BusinessStatus = 0;
                        response.MessagePayType = messagePayType;
                        response.StatusMessage = smsResult.Message;
                    }
                }
                else
                {
                    response.BusinessStatus = 0;
                    response.MessagePayType = messagePayType;
                    response.StatusMessage = "短信发送失败";
                }
            }
            catch (Exception ex)
            {
                response = new SmsResultViewModel_sms();
                response.MessagePayType = messagePayType;
                response.Status = HttpStatusCode.ExpectationFailed;
                logError.Info("短信请求发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return response;
        }

        public GetSmsAccountResponse GetSmsAccount(SmsAccountRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new GetSmsAccountResponse();
            //参数校验
            bx_agent agentModel = _agentRepository.GetAgent(request.Agent);
            bx_agent curAgentModel = _agentRepository.GetAgent(request.CurAgent);
            if (agentModel == null || curAgentModel == null)
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }
            if (!ValidateReqest(pairs, agentModel.SecretKey, request.SecCode))
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }

            int messagePayType = curAgentModel.MessagePayType.HasValue ? curAgentModel.MessagePayType.Value : 0;
            //如果走顶级代理，则查顶级短信；否则，查当前短信信息
            var smsAcount = messagePayType == 0 ? _smsContentRepository.Find(request.Agent) : _smsContentRepository.Find(request.CurAgent);
            if (smsAcount != null)
            {
                response.Status = HttpStatusCode.OK;
                response.SmsAccount = smsAcount;
            }
            else
            {
                response.ErrCode = -1;
                response.ErrMsg = "没有此记录";
            }
            return response;
        }

        public GetSmsAccountResponse CreateAccount(CreateAccountRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new GetSmsAccountResponse();
            //参数校验
            bx_agent agentModel = _agentRepository.GetAgent(request.Agent);
            if (agentModel == null)
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }
            //if (!ValidateReqest(pairs, agentModel.SecretKey, request.SecCode))
            //{
            //    response.Status = HttpStatusCode.Forbidden;
            //    return response;
            //}
            //创建发短信的账号
            int smsAcountId = 0;
            smsAcountId = CreateAccount(request.CurAgent, request.Method, request.Agent);
            //如果返回来的Id是负数，则说明账号已存在，该账号的Id就是smsAcountId*(-1)
            if (smsAcountId < 0)
            {
                response.ErrCode = -2;
                response.ErrMsg = "用户名已存在，无法创建";
                return response;
            }
            //返回来的Id是正数，说明创建账号成功。
            if (smsAcountId > 0)
            {
                response.Status = HttpStatusCode.OK;
                response.SmsAccount = _smsContentRepository.Find(request.CurAgent);
            }
            else
            {
                response.ErrCode = -1;
                response.ErrMsg = "创建失败,微信端无法创建短信账号";
            }
            return response;
        }

        /// <summary>
        /// 创建短信充值订单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public GetSmsOrderResponse SmsCreateOrder(SmsCreateOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new GetSmsOrderResponse();
            //参数校验
            bx_agent agentModel = _agentRepository.GetAgent(request.Agent);
            if (agentModel == null)
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }
            //if (!ValidateReqest(pairs, agentModel.SecretKey, request.SecCode))
            //{
            //    response.Status = HttpStatusCode.Forbidden;
            //    return response;
            //}

            //创建订单对象
            bx_sms_order item = new bx_sms_order();

            Random rand = new Random();
            item.OrderNum = DateTime.Now.ToString("yyyyMMddhhmmss") + rand.Next(0, 9).ToString() + rand.Next(0, 9).ToString();
            //item.AgentId = request.CurAgent;
            item.RechargeAmount = request.Amount;
            item.SmsQuantity = request.Quantity;
            item.CreateTime = DateTime.Now;
            item.PayType = request.PayType;
            item.OrderStatus = 1;//下单成功
            //实现创建订单方法
            int orderId = CreateOrder(request.CurAgent, request.Method, request.Agent, item);
            //bx_sms_order smsOrder = _smsOrderRepository.Add(item);
            if (orderId > 0)
            {
                response.Status = HttpStatusCode.OK;
                item.Id = orderId;
                response.SmsOrder = item;
                logInfo.Info("创建短信充值订单成功,订单信息:" + request.ToJson());
            }
            else
            {
                response.Status = HttpStatusCode.ExpectationFailed;
                logError.Info("创建订单失败，订单信息：" + request.ToJson());
            }
            return response;
        }

        /// <summary>
        /// 更新短信充值订单状态
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public GetSmsOrderStatusResponse SmsOrderStatus(SmsOrderStatusRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new GetSmsOrderStatusResponse();
            //参数校验
            //bx_agent agentModel = GetAgent(request.Agent);
            //if (agentModel == null)
            //{
            //    response.Status = HttpStatusCode.Forbidden;
            //    return response;
            //}
            //if (!ValidateReqest(pairs, agentModel.SecretKey, request.SecCode))
            //{
            //    response.Status = HttpStatusCode.Forbidden;
            //    return response;
            //}

            var orderItem = _smsOrderRepository.Find(request.OrderNum);
            if (orderItem != null)
            {
                //bx_sms_order表中OrderStatus已经更新为2，所以直接返回成功，不在执行下面逻辑
                //主要避免同一订单多次调用本接口。
                if (orderItem.OrderStatus == 2)
                {
                    response.Status = HttpStatusCode.OK;
                    response.Count = 1;
                    response.ErrMsg = "短信充值成功";
                    logInfo.Info("短信充值成功,订单信息:" + request.ToJson());
                    return response;
                }
                int orderStatusBak = orderItem.OrderStatus;//备份订单当前状态，如果短信更新失败，则还原状态
                if (request.OrderStatus == 2)
                {
                    if (orderItem.OrderStatus > 1)
                    {
                        response.Status = HttpStatusCode.ExpectationFailed;
                        response.Count = 0;
                        response.ErrMsg = "状态修改失败，订单已支付";
                        logError.Info("状态修改失败，订单已支付，订单信息：" + request.ToJson());
                        AddSMSOrderError(request, orderItem);
                        return response;
                    }
                    if (string.IsNullOrEmpty(request.TradeNo))
                    {
                        response.Status = HttpStatusCode.ExpectationFailed;
                        response.Count = 0;
                        response.ErrMsg = "状态修改失败";
                        logError.Info("状态修改失败，订单信息：" + request.ToJson());
                        AddSMSOrderError(request, orderItem);
                        return response;
                    }
                    orderItem.PayDateTime = DateTime.Now;
                    orderItem.ThirdPartOrderNum = request.TradeNo;
                }

                #region 更新订单状态,然后进行充值(改成事务)
                int countRecharge = 0;//存储更新bx_sms_account表受影响的行数
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        //更新短信订状态和支付时间
                        orderItem.OrderStatus = request.OrderStatus;
                        int orderUpdateCount = _smsOrderRepository.Update(orderItem);
                        //订单状态更新成功            如果是支付成功，则开始充值
                        if (orderUpdateCount > 0 && request.OrderStatus == 2)
                        {
                            countRecharge = SmsRecharge(orderItem.Id, orderItem.AgentId, orderItem.SmsQuantity);
                            scope.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        logError.Error(string.Format("短信充值:修改短信充值订单状态，并为代理人充值接口，发生异常: 订单号:{0},支付宝单号:{1} ", orderItem.OrderNum, orderItem.ThirdPartOrderNum)
                            + "\n" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    }
                    finally
                    {
                        scope.Dispose();
                    }
                }
                //orderUpdateCount>0 && countRecharge > 0
                if (countRecharge > 0)
                {
                    response.Status = HttpStatusCode.OK;
                    response.Count = countRecharge;
                    response.ErrMsg = "短信充值成功";
                    logInfo.Info("短信充值成功,订单信息:" + request.ToJson());
                    return response;
                }
               
                response.Status = HttpStatusCode.ExpectationFailed;
                response.Count = 0;
                response.ErrMsg = "短信充值失败";
                logInfo.Info("短信充值失败，订单信息：" + request.ToJson());
                AddSMSOrderError(request, orderItem);
                #endregion

                #region 旧逻辑代码
                //int orderUpdateCount = _smsOrderRepository.Update(orderItem);
                ////订单状态更新成功
                //if (orderUpdateCount > 0)
                //{
                //    //如果是支付成功，则开始充值
                //    if (request.OrderStatus == 2)
                //    {
                //        int countRecharge = SmsRecharge(orderItem.Id, orderItem.AgentId, orderItem.SmsQuantity);
                //        if (countRecharge > 0)
                //        {
                //            response.Status = HttpStatusCode.OK;
                //            response.Count = countRecharge;
                //            response.ErrMsg = "短信充值成功";
                //            logInfo.Info("短信充值成功,订单信息:" + request.ToJson());
                //        }
                //        else
                //        {
                //            response.Status = HttpStatusCode.ExpectationFailed;
                //            response.Count = 0;

                //            //充值失败，还原订单状态
                //            orderItem.OrderStatus = orderStatusBak;
                //            _smsOrderRepository.Update(orderItem);
                //            response.ErrMsg = "短信充值失败";
                //            logError.Info("短信充值失败，订单信息：" + request.ToJson());
                //        }
                //    }
                //    else
                //    {
                //        response.Status = HttpStatusCode.OK;
                //        response.ErrMsg = "状态修改成功";
                //        logInfo.Info("状态修改成功,订单信息:" + request.ToJson());
                //    }
                //}
                ////失败
                //else
                //{
                //    response.Status = HttpStatusCode.ExpectationFailed;
                //    response.Count = 0;
                //    response.ErrMsg = "状态修改失败";
                //    logError.Info("状态修改失败，订单信息：" + request.ToJson());
                //} 
                #endregion
            }

            return response;
        }

        /// <summary>
        /// 短信充值发生异常，添加到错误表中
        /// </summary>
        private void AddSMSOrderError(SmsOrderStatusRequest request, bx_sms_order orderItem)
        {
            bx_sms_order_error model = new bx_sms_order_error() { 
            sms_order_id=orderItem.Id,
            trade_no=request.TradeNo,
            request_order_status=request.OrderStatus,
            createtime =DateTime.Now,
            isdealwith=0
            };
            _smsOrderRepository.AddSMSOrderError(model);
        }

        public GetSmsOrderDetailResponse GetSmsOrderDetail(GetSmsOrderDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new GetSmsOrderDetailResponse();
            //参数校验
            bx_agent agentModel = _agentRepository.GetAgent(request.Agent);
            if (agentModel == null)
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }
            if (!ValidateReqest(pairs, agentModel.SecretKey, request.SecCode))
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }
            var bxSmsOrder = _smsOrderRepository.Find(request.SmsOrderID);
            if (bxSmsOrder != null)
            {
                response.Status = HttpStatusCode.OK;
                response.BxSmsOrder = bxSmsOrder;
            }
            else
            {
                response.ErrCode = -1;
                response.ErrMsg = "没有此记录";
            }
            return response;
        }

        /// <summary>
        /// 更新短信条数
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="agentId"></param>
        public int SmsRecharge(int orderId, int agentId, int quantity)
        {
            int result = 0;
            bx_sms_account item = _smsContentRepository.Find(agentId);
            item.total_times = item.total_times + quantity;
            item.avail_times = item.avail_times + quantity;
            result = _smsContentRepository.Update(item);
            return result;
        }

        /// <summary>
        /// 根据代理人Id来创建短信账号
        /// </summary>
        /// <param name="curAgent"></param>
        /// <param name="method">来源方式 1微信申请；2PC申请</param>
        /// <param name="topAgent"></param>
        /// <returns>创建的账号Id</returns>
        public int CreateAccount(int curAgent, int method, int topAgent)
        {
            //bool isCreat = false;//true为创建账号
            //bool isByTopAgent = IsPayByTopAgent(curAgent);//true为走顶级代理人账户
            int creatAgent = 0;
            creatAgent = curAgent;
            //if (method == 1)
            //{
            //    //微信端
            //    //如果走顶级代理人账户:走顶级，不允许创建账号；不走顶级，当前！=顶级 时才可创建账号
            //    if (!isByTopAgent)
            //    {
            //        //顶级代理人无法创建短信账户
            //        if (curAgent != topAgent)
            //        {
            //            isCreat = true;
            //        }
            //        creatAgent = curAgent;
            //    }
            //}
            //else if (method == 2)
            //{
            //    //Pc端可以无限制创建短信账户
            //    isCreat = true;
            //    if (!isByTopAgent)
            //    {
            //        //如果不走顶级代理账户：创建账户用当前Agent
            //        creatAgent = curAgent;
            //    }
            //    else
            //    {
            //        creatAgent = topAgent;
            //    }
            //}
            //if (isCreat&&(creatAgent != 0))
            //{
            bx_sms_account getSmsAccount = _smsContentRepository.Find(creatAgent);
            if (getSmsAccount != null)
            {
                //如果存在账号，则返回负的当前账号id
                return getSmsAccount.id * (-1);
            }
            //否则，新创建账号，返回当前Id
            bx_sms_account bxSmsAccount = new bx_sms_account();
            bxSmsAccount.agent_id = creatAgent;
            bxSmsAccount.avail_times = 0;
            bxSmsAccount.total_times = 0;
            bxSmsAccount.create_time = DateTime.Now;
            bxSmsAccount.sms_account = string.Format(_smsAccountConfig, creatAgent);
            bxSmsAccount.sms_password = string.Format(_smsAccountConfig, creatAgent).GetMd5();
            bxSmsAccount.status = 1;//1可以使用
            int smsAcount = _smsContentRepository.Add(bxSmsAccount);
            return smsAcount;
            //}
            //return 0;//无权限创建账号
        }

        /// <summary>
        /// 短信充值下单
        /// </summary>
        /// <param name="curAgent"></param>
        /// <param name="method"></param>
        /// <param name="topAgent"></param>
        /// <param name="bxSmsOrder"></param>
        /// <returns></returns>
        public int CreateOrder(int curAgent, int method, int topAgent, bx_sms_order bxSmsOrder)
        {
            //bool isCreat = false;//true为创建订单
            //bool isByTopAgent = IsPayByTopAgent(curAgent);//true为走顶级代理人账户
            int creatAgent = 0;//以哪个代理人id创建
            creatAgent = curAgent;
            //当前代理人是顶级代理人可以充值
            //if (curAgent == topAgent)
            //{
            //    isCreat = true;
            //    creatAgent = topAgent;
            //}
            //else
            //{
            //    //当前代理人不是顶级代理人
            //    //如果不走顶级代理人账户：就给当前账户充值
            //    if (!isByTopAgent)
            //    {
            //        isCreat = true;
            //        creatAgent = curAgent;
            //    }
            //}
            //if (isCreat&&((creatAgent != 0)))
            //{
            int smsAccount = CreateAccount(curAgent, method, topAgent);
            if (smsAccount == 0)
            {
                return 0;
            }
            else
            {
                //创建订单
                bxSmsOrder.AgentId = creatAgent;
                bx_sms_order smsOrder = _smsOrderRepository.Add(bxSmsOrder);
                return smsOrder.Id;
            }
            //}
            //return 0;
        }

        /// <summary>
        /// 短信是否有顶级代理人支付
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool IsPayByTopAgent(int agentId)
        {
            bx_agent bxAgent = _agentRepository.GetAgent(agentId);
            if (bxAgent != null)
            {
                if (bxAgent.MessagePayType.HasValue)
                {
                    return bxAgent.MessagePayType.Value == 0;//0从顶级代理人扣费(短信)，1代理人本身扣费（短信）
                }
            }
            return false;
        }
    }
}