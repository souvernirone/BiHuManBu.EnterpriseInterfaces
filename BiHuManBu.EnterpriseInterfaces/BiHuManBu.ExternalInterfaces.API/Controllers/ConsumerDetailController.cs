using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Enum;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using log4net;
using ServiceStack.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System.Diagnostics;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 客户详情页，报价/回访 内容,短信发送,去核保,获取未处理的预约单数量,CRM时间线,对应的接口
    /// </summary>
    public class ConsumerDetailController : ApiController
    {
        private readonly IConsumerDetailService _consumerDetailService;
        private readonly IEnterpriseAgentService _enterpriseAgentService;
        private readonly IAgentService _agentService;
        private readonly ISmsUtilService _smsUtilService;
        private readonly IUserinfoRenewalInfoService _userinfoRenewalInfoService;
        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        private readonly ILog _logError = LogManager.GetLogger("ERROR");
        readonly string txtPath = "/Content/BadWords/黑词160901.txt";
        readonly ISmsBulkSendManageService _smsBulkSendManageService;

        public ConsumerDetailController(IConsumerDetailService consumerDetailService, IEnterpriseAgentService enterpriseAgentService, IAgentService agentService, ISmsUtilService smsUtilService, IUserinfoRenewalInfoService userinfoRenewalInfoService, ISmsBulkSendManageService _smsBulkSendManageService)
        {
            _consumerDetailService = consumerDetailService;
            _enterpriseAgentService = enterpriseAgentService;
            _agentService = agentService;
            _smsUtilService = smsUtilService;
            _userinfoRenewalInfoService = userinfoRenewalInfoService;
            this._smsBulkSendManageService = _smsBulkSendManageService;
        }

        /// <summary>
        /// CRM时间线步骤记录
        /// </summary>
        /// <param name="bxCrm">Type 字段 1回访，2短信报价，3预约单，4保单已打印</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddCrmSteps([FromBody]ConsumerDetailCrmTlViewModel bxCrm)
        {
            //记录请求的url,
            _logInfo.Info("CRM时间线步骤记录:" + Request.RequestUri + "参数" + bxCrm.ToJson());
            var baseViewModel = new BaseViewModel();
            if (bxCrm.AgentId == 0 || bxCrm.BUid == 0 || bxCrm.JsonContent == null || bxCrm.Type == 0)
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "参数错误";
                return baseViewModel.ResponseToJson();
            }
            string requesturl = HttpContext.Current.Request.Url.ToString();
            //记录请求URL
            _logInfo.Info(requesturl);
            var bxCrmSteps = new bx_crm_steps
            {
                agent_id = bxCrm.AgentId,
                b_uid = bxCrm.BUid,
                create_time = DateTime.Now,
                json_content = bxCrm.JsonContent,
                type = bxCrm.Type
            };
            try
            {
                _consumerDetailService.AddCrmSteps(bxCrmSteps);
            }
            catch (Exception ex)
            {
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                baseViewModel.BusinessStatus = -10001;
                baseViewModel.StatusMessage = "服务器发生异常";
            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// CRM时间线步骤获取
        /// </summary>
        /// <param name="buid">报价单id</param>
        /// <param name="agentId">登录代理人id</param>
        /// <returns>Type 字段 1回访，2短信报价，3预约单，4保单，5 bx_userinfo批量删除，6摄像头进店，7分配，8回收</returns>
        [HttpGet]
        public HttpResponseMessage GetCrmStepsList(long buid, int agentId)
        {
            //记录请求数据
            _logInfo.Info("CRM时间线步骤获取:" + Request.RequestUri + buid + agentId);
            var consumerDetailCrmTlList = new ConsumerDetailCrmListResponse();
            consumerDetailCrmTlList.BxCrmStepses = new List<ConsumerDetailCrmListResponsedeatil>();
            var consumerDetailCrmListResponsedeatilList = new List<ConsumerDetailCrmListResponsedeatil>();
            try
            {
                List<bx_crm_steps> bxCrmStepses = _consumerDetailService.GetCrmStepsList(buid).OrderByDescending(c => c.create_time).ToList();
                var dateList = bxCrmStepses.Select(c => c.create_time.ToShortDateString()).Distinct();
                foreach (var date in dateList)
                {
                    var consumerDetailCrmListResponsedeatil = new ConsumerDetailCrmListResponsedeatil();
                    var cdclrsList = new List<ConsumerDetailCrmListResponseStep>();
                    List<bx_crm_steps> bxCrmSteps = bxCrmStepses.Where(c => c.create_time.ToShortDateString() == date).ToList();
                    foreach (var bxCrmStepse in bxCrmSteps)
                    {
                        if (bxCrmStepse.type == 7)
                        {
                            DistributeBackViewModel vModel =
                                ("[" + bxCrmStepse.json_content + "]").ToListT<DistributeBackViewModel>().FirstOrDefault();
                            if (vModel != null && vModel.OriId == 0)
                            {
                                vModel.OriName = "未分配";
                            }

                            bxCrmStepse.json_content = vModel.ToJson();
                        }
                        string tDate = bxCrmStepse.create_time.ToString("yyyy-MM-dd HH:mm:ss");
                        var cdclrs = new ConsumerDetailCrmListResponseStep
                        {
                            agent_id = bxCrmStepse.agent_id,
                            b_uid = bxCrmStepse.b_uid,
                            create_time = tDate,
                            stime = tDate.Substring(11),
                            json_content = bxCrmStepse.json_content,
                            type = bxCrmStepse.type == 9 ? 2 : bxCrmStepse.type == 10 ? 7 : bxCrmStepse.type
                        };
                        cdclrsList.Add(cdclrs);
                    }
                    consumerDetailCrmListResponsedeatil.JsonStepses = cdclrsList;
                    consumerDetailCrmListResponsedeatil.CreateDate = date;
                    consumerDetailCrmListResponsedeatilList.Add(consumerDetailCrmListResponsedeatil);
                }
                consumerDetailCrmTlList.BxCrmStepses = consumerDetailCrmListResponsedeatilList;
                consumerDetailCrmTlList.BusinessStatus = 1;
                consumerDetailCrmTlList.StatusMessage = "获取成功";
            }
            catch (Exception ex)
            {
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                consumerDetailCrmTlList.BusinessStatus = -10003;
                consumerDetailCrmTlList.StatusMessage = "统出现错误";
            }
            return consumerDetailCrmTlList.ResponseToJson();
        }

        /// <summary>
        /// 将代报价内容记录到跟进记录表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("将代报价内容记录到跟进记录表")]
        public async Task<HttpResponseMessage> AddBehelfQuoteToCrmSteps([FromBody]ConsumerDetailBehalfCrmViewModel request)
        {

            _logInfo.Info("CRM时间线代报价内容:" + Request.RequestUri + "参数" + request.ToJson());
            var baseViewModel = new BaseViewModel();
            //buid必须大于0
            if (request.AgentId == 0 || request.BUid == 0 || request.QuoteGroup < 1 || string.IsNullOrEmpty(request.ReQuoteName))
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "参数错误";
                return baseViewModel.ResponseToJson();
            }
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                string tempJson = await _consumerDetailService.GetBehelfQuoteAsync(request.BUid, request.ReQuoteAgent, request.ReQuoteName, request.QuoteGroup);

                var bxCrmSteps = new bx_crm_steps
                {
                    agent_id = request.AgentId,
                    b_uid = request.BUid,
                    create_time = DateTime.Now,
                    json_content = tempJson,
                    type = 14    //代报价
                };

                _consumerDetailService.AddCrmSteps(bxCrmSteps);
                baseViewModel.BusinessStatus = 1;
                baseViewModel.StatusMessage = "添加代报价跟进记录成功";
                sw.Stop();
                _logInfo.Info("检测时间：" + sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logError.Info("AddBehelfQuoteToCrmSteps方法发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                baseViewModel.BusinessStatus = -10001;
                baseViewModel.StatusMessage = "服务器发生异常";
            }
            return baseViewModel.ResponseToJson();
        }

        /// <summary>
        /// 车险报价器的短信发送接口(单个短信)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ModelVerify]
        public async Task<HttpResponseMessage> SentSms([FromBody]ConsumerDetailSmsViewModel request)
        {
            //记录请求的url
            _logInfo.Info("短信发送-请求:" + Request.RequestUri + "参数" + request.ToJson());
            var baseViewModel = new BaseViewModel();
            try
            {
                if (string.IsNullOrEmpty(request.content) || request.content.Length > 500)
                {
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "发送失败，短信字数过长").ResponseToJson();
                }
                var badWordsArray = await _smsUtilService.GetBadWordsCache();
                if (badWordsArray == default(string[]))
                {
                    badWordsArray = await _smsUtilService.InitBadWords(System.Web.HttpContext.Current.Server.MapPath(txtPath));
                }
                var badWordsList = _smsUtilService.BadWordsFilter(badWordsArray, request.content);
                if (badWordsList.Any())
                {
                    var result = new { BusinessStatus = -10014, StatusMessage = "短信内容不合法", badWordsList = badWordsList };
                    //baseViewModel.BusinessStatus = -10014;
                    //baseViewModel.StatusMessage = string.Format("短信内容不合法，不合法数据：{0}", string.Join(",", badWordsList));
                    return result.ResponseToJson();
                }
                int agentSelfId = request.agentId;
                bx_agent bxAgent = _agentService.GetAgent(agentSelfId);
                if (bxAgent.ParentAgent != 0)
                {
                    if (bxAgent.MessagePayType == 0)
                    {
                        agentSelfId = int.Parse(_consumerDetailService.GetTopAgent(agentSelfId));
                    }
                    else if (bxAgent.MessagePayType == 2)
                    {
                        agentSelfId = _agentService.GetAgent(bxAgent.ParentAgent).Id;//MessagePayType==2 三级代理 从父级扣费
                    }
                }
                bx_sms_account smsAccountInfo = _consumerDetailService.GetBxSmsAccount(agentSelfId);
                if (smsAccountInfo == null)
                {
                    var bxSmsAccount = new bx_sms_account
                    {
                        agent_id = agentSelfId,
                        sms_account = agentSelfId + "-bihu",
                        sms_password = agentSelfId.ToString(CultureInfo.InvariantCulture).ToMd5(),
                        total_times = 0,
                        avail_times = 0,
                        status = 1,
                        create_time = DateTime.Now
                    };
                    _consumerDetailService.InsetBxSmsAccount(bxSmsAccount);
                    baseViewModel.StatusMessage = "短信余额不足，请前往微信代理人中心充值后重试！";
                    baseViewModel.BusinessStatus = -10005;
                    return baseViewModel.ResponseToJson();
                }
                if (smsAccountInfo.status == 0)
                {
                    baseViewModel.StatusMessage = "短信账号不可用！";
                    baseViewModel.BusinessStatus = -10011;
                    return baseViewModel.ResponseToJson();
                }
                if (smsAccountInfo.avail_times < _smsBulkSendManageService.SDmessageCount(request.content,0))
                {
                    baseViewModel.StatusMessage = "短信余额不足，请充值后重试！";
                    baseViewModel.BusinessStatus = -10012;
                    return baseViewModel.ResponseToJson();
                }
                string smsAccount = "";
                string smsPassword = "";
                smsAccount = smsAccountInfo.sms_account;
                smsPassword = smsAccountInfo.sms_password;
                AddOrUpdateSmsAccountContentRequest smsContent = new AddOrUpdateSmsAccountContentRequest
                {
                    agent_id = bxAgent.Id,
                    agent_name = bxAgent.AgentName,
                    content = request.content,
                    sent_mobile = request.mobile,
                    sent_type = 0,
                    license_no = request.licenseNo,
                };
                //source值转换
                long newSource = request.IsNewSource == 0 ? SourceGroupAlgorithm.GetNewSource(request.source) : request.source;
                var crmTimeLineForSmsViewModel = new CrmTimeLineForSmsViewModel
                {
                    agent_id = bxAgent.Id,
                    agent_name = bxAgent.AgentName,
                    content = request.content,
                    sent_mobile = request.mobile,
                    sent_type = 0,
                    license_no = request.licenseNo,
                    Source = newSource,
                    sourceName = GetSourceName(newSource),
                    BizRate = request.bizRate,
                    ForceRate = request.forceRate,
                    ActivityId = request.ActivityId
                };
                SmsResultModel ssModel = _consumerDetailService.SendSmsForBaoJia(request.mobile, request.content, EnumSmsBusinessType.SentQuote, smsAccount, smsPassword, request.TopAgentId, request.SmsSign);
                _logInfo.Info("短信发送-内容," + request.content + ",手机号" + "，报价单id" + request.buid + "扣费账号" + smsAccount);
                if (ssModel.ResultCode == 0)
                {
                    //添加一条短信记录到bxSmsAccountContent
                    _enterpriseAgentService.AddOrUpdateSmsAccountContent(smsContent);
                    _logInfo.Info("短信发送-bxSmsAccountContent:" + smsContent.ResponseToJson());
                    //_insuranceService.AddBuidActivity(buid, fanxian, fanquan, youhuihuodong, Account.Name);
                    baseViewModel.StatusMessage = "发送短信已成功";
                    baseViewModel.BusinessStatus = 1;
                    var smsCount = _smsBulkSendManageService.UpdateSmsAccountUseCount(agentSelfId, _smsBulkSendManageService.SDmessageCount(request.content, 0), 2);
                    var bxCrmSteps = new bx_crm_steps
                    {
                        agent_id = request.agentId,
                        b_uid = request.buid,
                        create_time = DateTime.Now,
                        json_content = crmTimeLineForSmsViewModel.ToJson(),
                        type = EnumCrmTimeLineType.DuanXingBaoJia.GetHashCode()
                    };

                    bxAgent.phone_is_wechat = request.IsWeChat;

                    //添加到CrmTimeLine表中.
                    _consumerDetailService.AddCrmSteps(bxCrmSteps);
                    _logInfo.Info("短信发送-CrmTimeLine:" + bxCrmSteps.ResponseToJson());
                    //把手机号 更新到bx_userinfo_renewal_info 的客户手机
                    bool updateClient = _userinfoRenewalInfoService.UpdateClientMobileByBuid(request.buid, request.mobile);
                    if (updateClient)
                    {
                        return baseViewModel.ResponseToJson();
                    }
                    baseViewModel.StatusMessage = "更新手机号时出错，请稍后重试！";
                    baseViewModel.BusinessStatus = -10003;
                    return baseViewModel.ResponseToJson();
                }
                if (ssModel.ResultCode == -114)
                {
                    baseViewModel.StatusMessage = "短信余额不足，请充值后重试！";
                    baseViewModel.BusinessStatus = -10008;
                    return baseViewModel.ResponseToJson();
                }
                baseViewModel.StatusMessage = "账户状态异常，请联系客服人员！";
                baseViewModel.BusinessStatus = -10006;
                return baseViewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                _logError.Error("发生异常-短信发送:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "系统出现错误";
                return baseViewModel.ResponseToJson();
            }
        }

        /// <summary>
        /// 保存短信报价单信息 李金友 2017-10-13 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ModelVerify]
        public async Task<HttpResponseMessage> SaveBjdDetail([FromBody]ConsumerDetailSmsViewModel request)
        {
            //记录请求的url
            _logInfo.Info("保存分享报价单信息-请求:" + Request.RequestUri + "参数" + request.ToJson());
            var baseViewModel = new BaseViewModel();
            try
            {
                if (request.buid == 0)
                {
                    baseViewModel.BusinessStatus = 0;
                    baseViewModel.StatusMessage = "参数信息不正确buid为0";
                    return baseViewModel.ResponseToJson();
                }

                int agentSelfId = request.agentId;
                bx_agent bxAgent = _agentService.GetAgent(agentSelfId);
                if (bxAgent.ParentAgent != 0)
                {
                    if (bxAgent.MessagePayType == 0)
                    {
                        agentSelfId = int.Parse(_consumerDetailService.GetTopAgent(agentSelfId));
                    }
                    else if (bxAgent.MessagePayType == 2)
                    {
                        agentSelfId = _agentService.GetAgent(bxAgent.ParentAgent).Id;//MessagePayType==2 三级代理 从父级扣费
                    }
                }

                AddOrUpdateSmsAccountContentRequest smsContent = new AddOrUpdateSmsAccountContentRequest
                {
                    agent_id = bxAgent.Id,
                    agent_name = bxAgent.AgentName,
                    content = request.content,
                    sent_mobile = "",
                    sent_type = 0,
                    license_no = request.licenseNo,
                };
                int newSource = 0;
                if (request.IsNewSource == 0)
                {
                    switch (request.source)
                    {
                        case 0:
                            newSource = 2;
                            break;
                        case 1:
                            newSource = 1;
                            break;
                        case 2:
                            newSource = 4;
                            break;
                        case 3:
                            newSource = 8;
                            break;
                    }
                }
                else
                {
                    newSource = request.source;
                }
                var crmTimeLineForSmsViewModel = new CrmTimeLineForSmsViewModel
                {
                    agent_id = bxAgent.Id,
                    agent_name = bxAgent.AgentName,
                    content = request.content,
                    sent_type = 0,
                    license_no = request.licenseNo,
                    sent_mobile = "",
                    Source = newSource,
                    BizRate = request.bizRate,
                    ForceRate = request.forceRate,
                    ActivityId = request.ActivityId
                };

                var bxCrmSteps = new bx_crm_steps
                {
                    agent_id = request.agentId,
                    b_uid = request.buid,
                    create_time = DateTime.Now,
                    json_content = crmTimeLineForSmsViewModel.ToJson(),
                    type = 9
                };

                bxAgent.phone_is_wechat = request.IsWeChat;

                List<bx_crm_steps> listSteps = _consumerDetailService.GetCrmStepsList(request.buid);
                var item = listSteps.Where(n => n.type == 9).OrderByDescending(n => n.create_time).FirstOrDefault();

                /*张克亮 2018-08-17修改内容如下
                 * bx_crm_steps表中数据 type 没有9（保存短信信息）时，做数据非空处理，不执行3小时内的数据修改操作
                 */
                if (item!=null)
                {
                    CrmTimeLineForSmsViewModel CrmTimeLineForSmsModel = CommonHelper.ToListT<CrmTimeLineForSmsViewModel>("[" + item.json_content + "]").FirstOrDefault();
                    if (CrmTimeLineForSmsModel.content == request.content && (DateTime.Now - item.create_time) < new TimeSpan(0, 0, 3))
                    {
                        item.json_content = crmTimeLineForSmsViewModel.ToJson();
                        item.create_time = DateTime.Now;
                        int changeNum1 = _consumerDetailService.UpdateCrmSteps(item);
                        if (changeNum1 > 0)
                        {
                            baseViewModel.StatusMessage = "信息保存成功";
                            baseViewModel.BusinessStatus = 1;
                            return baseViewModel.ResponseToJson();
                        }
                        baseViewModel.StatusMessage = "信息已存在清勿多次保存";
                        baseViewModel.BusinessStatus = 0;
                        return baseViewModel.ResponseToJson();
                    }
                }

                //添加到CrmTimeLine表中.
                int changeNum = _consumerDetailService.AddCrmSteps(bxCrmSteps);
                if (changeNum > 0)
                {
                    baseViewModel.StatusMessage = "信息保存成功";
                    baseViewModel.BusinessStatus = 1;
                }
                return baseViewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                _logError.Error("分享报价-保存报价详情:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "系统出现错误";
                return baseViewModel.ResponseToJson();
            }
        }
        /// <summary>
        /// 获取最新报价信息
        /// </summary>
        /// <param name="buid">buid</param>
        /// <param name="agentId">登录人Id</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetNewQuoteInfo(long buid, int agentId)
        {
            //记录请求URL
            _logInfo.Info(Request.RequestUri);
            var newQuoteInfoView = new NewQuoteInfoViewModel();
            try
            {
                newQuoteInfoView = _consumerDetailService.GetNewQuoteInfo(buid, agentId);
                if (newQuoteInfoView != null)
                {
                    newQuoteInfoView.BusinessStatus = 1;
                    newQuoteInfoView.StatusMessage = "查询成功";
                    return newQuoteInfoView.ResponseToJson();
                }
                newQuoteInfoView.BusinessStatus = -10002;
                newQuoteInfoView.StatusMessage = "查询失败";
                return newQuoteInfoView.ResponseToJson();
            }
            catch (Exception ex)
            {
                _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                newQuoteInfoView.BusinessStatus = -10003;
                newQuoteInfoView.StatusMessage = "系统出现错误";
                return newQuoteInfoView.ResponseToJson(); ;
            }
        }
        /// <summary>
        /// 保存最新报价信息
        /// </summary>
        /// <param name="requestNew"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveNewQuoteInfo(RequestNewQuoteInfoViewModel requestNew)
        {
            //记录请求URL
            _logInfo.Info("保存最新报价信息:" + Request.RequestUri + "参数:" + requestNew.ToJson());
            var baseView = new BaseViewModel();
            try
            {
                _consumerDetailService.SaveNewQuoteInfo(requestNew);
                baseView.BusinessStatus = 1;
                baseView.StatusMessage = "保存成功";
                return baseView.ResponseToJson();
            }
            catch (Exception ex)
            {
                _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                baseView.BusinessStatus = -10003;
                baseView.StatusMessage = "系统出现错误";
                return baseView.ResponseToJson();
            }
        }
        /// <summary>
        /// 去核保
        /// </summary>
        /// <param name="buid">buid</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public HttpResponseMessage DoSubmit(long buid, int source)
        {
            //记录请求URL
            _logInfo.Info(Request.RequestUri);
            var baseview = new BaseViewModel();
            try
            {
                baseview = _consumerDetailService.DoSubmit(buid, source);
            }
            catch (Exception ex)
            {
                _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                baseview.BusinessStatus = -10003;
                baseview.StatusMessage = "系统出现错误";
                return baseview.ResponseToJson(); ;
            }
            return baseview.ResponseToJson();
        }
        /// <summary>
        /// 获取未处理的预约单数量
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public HttpResponseMessage GetAppoinmentInfoNum(int agentid)
        {
            //记录请求URL
            _logInfo.Info(Request.RequestUri + "," + agentid);
            var baseview = new AppoinmentInfoNumResultViewModel();
            try
            {
                baseview.Num = _consumerDetailService.GetAppoinmentInfoNum(agentid);
                baseview.BusinessStatus = 1;
                baseview.StatusMessage = "获取成功";
                return baseview.ResponseToJson();
            }
            catch (Exception ex)
            {
                _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                baseview.BusinessStatus = -10003;
                baseview.StatusMessage = "系统出现错误";
                return baseview.ResponseToJson(); ;
            }
            //return baseview.ResponseToJson();
        }

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
    }
}