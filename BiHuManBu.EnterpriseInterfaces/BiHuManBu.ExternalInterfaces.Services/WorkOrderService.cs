using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse;
using log4net;
using ServiceStack.Text;
using IAgentService = BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces.IAgentService;
using IMessageService = BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces.IMessageService;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class WorkOrderService : CommonBehaviorService, IWorkOrderService
    {
        #region
        private IUserinfoRenewalInfoRepository _userinfoRenewalInfoRepository;
        private IConsumerReviewRepository _consumerReviewRepository;
        private IUserInfoRepository _userInfoRepository;
        private IAgentDistributedRepository _agentDistributedRepository;
        private ITransferRecordRepository _transferRecordRepository;
        private ILastInfoRepository _lastInfoRepository;
        private ICarRenewalRepository _carRenewalRepository;
        private IMessageService _messageService;
        private ILog logError;
        private ILog logInfo;
        private IAgentRepository _agentRepository;
        private ICacheHelper _cacheHelper;
        private INoticexbService _noticexbService;
        private IAgentService _agentService;
        public WorkOrderService(IUserinfoRenewalInfoRepository userinfoRenewalInfoRepository, IUserInfoRepository userInfoRepository, IAgentRepository agentRepository, IAgentDistributedRepository agentDistributedRepository, ITransferRecordRepository transferRecordRepository, ILastInfoRepository lastInfoRepository, ICacheHelper cacheHelper, INoticexbService noticexbService, IConsumerReviewRepository consumerReviewRepository, IMessageService messageService, ICarRenewalRepository carRenewalRepository, IAgentService agentService)
            : base(agentRepository, cacheHelper)
        {
            _agentRepository = agentRepository;
            logError = LogManager.GetLogger("ERROR");
            logInfo = LogManager.GetLogger("Info");

            _cacheHelper = cacheHelper;

            _userinfoRenewalInfoRepository = userinfoRenewalInfoRepository;
            _userInfoRepository = userInfoRepository;
            _agentDistributedRepository = agentDistributedRepository;
            _transferRecordRepository = transferRecordRepository;
            _lastInfoRepository = lastInfoRepository;
            _consumerReviewRepository = consumerReviewRepository;
            _noticexbService = noticexbService;
            _messageService = messageService;
            _carRenewalRepository = carRenewalRepository;
            _agentService = agentService;
        }
        #endregion

        #region zanyong 
        public bool AppTokenValidateReqest(string bhToken, int childAgent)
        {
            var checkApp = string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["CheckApp"]) ? 0 : int.Parse(System.Configuration.ConfigurationManager.AppSettings["CheckApp"]);
            if (checkApp == 0)
            {
                return true;
            }
            try
            {
                //2018-05-16 L：  判断传入验证的值是否为NULL或者空  直接跳出
                if (string.IsNullOrWhiteSpace(bhToken))
                {
                    return false;
                }
                string tokenkey = Infrastructure.Helpers.AppHelpers.RsaCryptionHelper.RSADecrypt(bhToken);
                if (string.IsNullOrEmpty(tokenkey))
                {
                    return false;
                }
                if (!tokenkey.Contains("&"))
                {
                    return false;
                }
                string[] strItem = tokenkey.Split('&');
                string itemAgentId = strItem[0].Substring(strItem[0].IndexOf("=") + 1,
                    strItem[0].Length - strItem[0].IndexOf("=") - 1);
                if (!itemAgentId.Equals(childAgent.ToString()))
                {
                    return false;
                }
                string itemDatetime = strItem[1].Substring(strItem[1].IndexOf("=") + 1,
                    strItem[1].Length - strItem[1].IndexOf("=") - 1);
                if (!(DateTime.Parse(itemDatetime) > DateTime.Now) &&
                    DateTime.Parse(itemDatetime).AddDays(30) < DateTime.Now)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                logError.Info("接口BhToken校验异常：BhToken为：" + bhToken + "\n 当前代理人为：" + childAgent + "\n 异常信息:" + ex.StackTrace + " \n " + ex.Message);
                return false;
            }
            return true;
        }

        public IBxAgent GetAgentModelFactory(int agentid)
        {
            string cacheKey = string.Format("agent_key_cache_{0}", agentid);
            IBxAgent cacheValue = CacheProvider.Get<bx_agent>(cacheKey);
            if (cacheValue == null)
            {
                cacheValue = _agentRepository.GetAgent(agentid);
                if (cacheValue == null)
                {
                    cacheValue = new NullBxAgent();
                }
                CacheProvider.Set(cacheKey, cacheValue, 120);//5分钟
            }

            return cacheValue;
        }

        private static readonly string _mobileSecKey = System.Configuration.ConfigurationManager.AppSettings["MobileSecKey"];
        /// <summary>
        /// 此处校验复制ValidateReqest的方法，App用来校验，代码逻辑一样
        /// app端校验，微信端未做校验
        /// </summary>
        /// <param name="list"></param>
        /// <param name="configKey"></param>
        /// <param name="checkCode"></param>
        /// <returns></returns>
        public bool AppValidateReqest(IEnumerable<KeyValuePair<string, string>> list, string checkCode)
        {
            var checkApp = string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["CheckApp"]) ? 0 : int.Parse(System.Configuration.ConfigurationManager.AppSettings["CheckApp"]);
            if (checkApp == 0)
            {
                return true;
            }
            if (!list.Any()) return true;
            if (string.IsNullOrEmpty(_mobileSecKey)) return false;

            var inputParamsString = new StringBuilder();
            foreach (KeyValuePair<string, string> item in list)
            {
                //排除 安卓端无效参数

                if (item.Key.ToLower().Equals("callbackparamskey2") || item.Key.ToLower().Equals("callbackparamskey1") || item.Key.ToLower().Equals("sign") || item.Key.ToLower().Equals("nonce") || item.Key.ToLower().Equals("token") || item.Key.ToLower().Equals("timestamp"))
                {
                }
                else if (item.Key.ToLower() != "seccode")
                {
                    inputParamsString.Append(string.Format("{0}={1}&", item.Key, item.Value));
                }
            }

            var content = inputParamsString.ToString();
            string strsec = content.Substring(0, content.Length - 1);// + _mobileSecKey;
            var securityString = strsec.GetMd5().ToUpper();
            return securityString == checkCode.ToUpper();
        }

        #endregion

        /// <summary>
        /// 业务：sa用。第一次创建WorkOrder
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public AddOrUpdateWorkOrderResponse AddOrUpdateWorkOrder(AddOrUpdateWorkOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AddOrUpdateWorkOrderResponse();
            //bhToken校验
            if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
            {
                response.ErrCode = -300;
                response.ErrMsg = "登录信息已过期，请重新登录";
                return response;
            }
            //传参校验
            IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            //参数校验
            if (agentModel == null)// || childAgent == null)
            {
                response.Status = HttpStatusCode.BadRequest;
                return response;
            }
            if (!AppValidateReqest(pairs, request.SecCode))
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }
            if (request.IntentionView == 0)
            {
                response.ErrCode = -1;
                response.ErrMsg = "意向不能为空";
                return response;
            }
            if (request.OwnerAgent == 0)
            {
                response.ErrCode = -2;
                response.ErrMsg = "车辆当前受理人Id不能为空";
                return response;
            }
            try
            {
                bx_userinfo bxUserInfo;
                if (request.Buid.HasValue && request.Buid.Value != 0)
                {
                    //如果传buid过来，重新赋值request
                    bxUserInfo = _userInfoRepository.FindByBuid(request.Buid.Value);
                    if (bxUserInfo != null)
                    {
                        request.LicenseNo = bxUserInfo.LicenseNo;
                        request.OwnerAgent = int.Parse(bxUserInfo.Agent);
                    }
                }
                else
                {
                    //根据OpenId、车牌号、代理人Id找userinfo对象
                    bxUserInfo = _userInfoRepository.FindByAgentLicense(request.LicenseNo, request.OwnerAgent.ToString());
                }

                if (bxUserInfo == null)
                {
                    response.ErrCode = 0;
                    response.ErrMsg = "查不到此续保信息";
                    return response;
                }
                int workOrderId = 0;//标记添加修改意向成功与否
                DateTime dateTime = DateTime.Now;
                bx_userinfo_renewal_info workOrder = _userinfoRenewalInfoRepository.FindByBuid(bxUserInfo.Id);
                if (workOrder != null)
                {//如果库里有记录，执行更新
                    workOrder.sa_id = request.ChildAgent;
                    workOrder.sa_name = request.ChildName;
                    workOrder.client_name = request.CustomerName;
                    workOrder.client_mobile = request.Phone;
                    workOrder.client_intention = request.IntentionView;
                    workOrder.remark = request.Remark;
                    workOrderId = _userinfoRenewalInfoRepository.Update(workOrder);
                }
                else
                {//如果库里无记录，执行插入
                    workOrder = new bx_userinfo_renewal_info();
                    workOrder.sa_id = request.ChildAgent;
                    workOrder.sa_name = request.ChildName;
                    workOrder.client_name = request.CustomerName;
                    workOrder.client_mobile = request.Phone;
                    workOrder.b_uid = bxUserInfo.Id;
                    workOrder.client_intention = request.IntentionView;
                    workOrder.remark = request.Remark;
                    workOrder.create_time = dateTime;
                    workOrderId = _userinfoRenewalInfoRepository.Add(workOrder);
                }
                if (workOrderId > 0)
                {
                    response.ErrCode = 1;
                    response.WorkOrderId = workOrderId;
                }
                else
                {
                    response.ErrCode = -10002;
                    response.ErrMsg = "添加意向失败，未存在可更新的内容";
                }
            }
            catch (Exception ex)
            {
                response.ErrCode = -10003;
                logError.Info("意向接口异常，添加意向信息：" + request.ToJson() + "\n 异常信息:" + ex.StackTrace + " \n " + ex.Message);
            }
            return response;
        }

        /// <summary>
        /// 业务：续保顾问用。创建WorkOrderDetail对象和更新WorkOrder对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AddOrUpdateWorkOrderDetailResponse AddOrUpdateWorkOrderDetail(AddOrUpdateWorkOrderDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AddOrUpdateWorkOrderDetailResponse();
            //bhToken校验
            if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
            {
                response.ErrCode = -300;
                response.ErrMsg = "登录信息已过期，请重新登录";
                return response;
            }
            //传参校验
            IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            //参数校验
            if (!agentModel.AgentCanUse())
            {
                response.Status = HttpStatusCode.BadRequest;
                return response;
            }
            if (!AppValidateReqest(pairs, request.SecCode))
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }

            if (request.Status == 0)
            {
                response.ErrCode = -1;
                response.ErrMsg = "受理状态不能为空";
                return response;
            }
            try
            {
                DateTime dateTime = DateTime.Now;
                bx_userinfo_renewal_info workOrder;

                bx_userinfo bxUserInfo;
                if (request.Buid.HasValue && request.Buid.Value != 0)
                {
                    //如果传buid过来，重新赋值request
                    bxUserInfo = _userInfoRepository.FindByBuid(request.Buid.Value);
                    if (bxUserInfo != null)
                    {
                        request.LicenseNo = bxUserInfo.LicenseNo;
                        request.OwnerAgent = int.Parse(bxUserInfo.Agent);
                    }
                }
                else
                {
                    //根据OpenId、车牌号、代理人Id找userinfo对象
                    bxUserInfo = _userInfoRepository.FindByAgentLicense(request.LicenseNo, request.OwnerAgent.ToString());
                }

                if (bxUserInfo == null)
                {
                    response.ErrCode = 0;
                    response.ErrMsg = "查不到此续保信息";
                    return response;
                }
                int workOrderId = request.WorkOrderId;
                if (workOrderId == 0)
                {
                    workOrder = new bx_userinfo_renewal_info();
                    workOrder.sa_id = request.SaAgent;
                    workOrder.sa_name = request.SaAgentName;
                    workOrder.client_name = request.CustomerName;
                    workOrder.client_mobile = request.Phone;
                    workOrder.b_uid = (int)bxUserInfo.Id;
                    workOrder.client_intention = 3;//默认添加，其他情况
                    //workOrder.remark = request.Remark;
                    workOrder.create_time = dateTime;
                    workOrder.xubao_id = request.ChildAgent;
                    workOrder.xubao_name = request.ChildName;
                    workOrderId = _userinfoRenewalInfoRepository.Add(workOrder);
                }
                if (workOrderId > 0)
                {
                    var workOrderDetail = new bx_consumer_review
                    {
                        b_uid = (int)bxUserInfo.Id,
                        content = request.Remark,
                        operatorId = request.ChildAgent,
                        operatorName = request.ChildName
                    };
                    if (!string.IsNullOrWhiteSpace(request.NextReviewDate))
                    {
                        workOrderDetail.next_review_date = DateTime.Parse(request.NextReviewDate);
                    }
                    workOrderDetail.read_status = 0;
                    workOrderDetail.status = 0;//跟进类型,0回访,1投诉,2跟进..
                    workOrderDetail.result_status = request.Status;
                    workOrderDetail.intentioncompany = request.IntentionCompany;
                    workOrderDetail.create_time = dateTime;
                    int workOrderDetailId = _consumerReviewRepository.AddDetail(workOrderDetail);
                    if (workOrderDetailId > 0)
                    {
                        if (workOrderId != 0)
                        {
                            if (request.Status == 1)
                            {
                                //5，添加出单通知
                                var msgRequest = new AddMessageRequest
                                {
                                    Body = request.LicenseNo,
                                    MsgType = 7,
                                    ToAgentId = request.Agent,
                                    MsgLevel = 1,
                                    SendTime = DateTime.Now.ToString(),
                                    ChildAgent = request.ChildAgent,
                                    LicenseNo = request.LicenseNo,
                                    Buid = bxUserInfo.Id,
                                    Source = request.IntentionCompany
                                };
                                _messageService.AddMessage(msgRequest);
                            }
                            //修改意向里的主记录
                            workOrder = new bx_userinfo_renewal_info();
                            workOrder = _userinfoRenewalInfoRepository.FindById(workOrderId);
                            if (workOrder != null)
                            {
                                workOrder.intentioncompany = request.IntentionCompany;
                                workOrder.result_status = request.Status;
                                workOrder.xubao_id = request.ChildAgent;
                                workOrder.xubao_name = request.ChildName;
                                _userinfoRenewalInfoRepository.Update(workOrder);
                                //if (_userinfoRenewalInfoRepository.Update(workOrder) > 0)
                                //{
                                response.ErrCode = 1;
                                response.WorkOrderDetailId = workOrderDetailId;
                                //}
                                //else
                                //{
                                //    response.ErrCode = 0;
                                //}
                            }
                        }
                        else
                        {
                            response.ErrCode = 0;
                            //response.ErrCode = 1;
                            //response.WorkOrderDetailId = workOrderDetailId;
                        }

                    }
                    else
                    {
                        response.ErrCode = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrCode = 0;
                logError.Info("添加受理记录异常，受理记录信息：" + request.ToJson() + "\n 异常信息:" + ex.StackTrace + " \n " + ex.Message);
            }
            return response;
        }

        public WorkOrderDetailListResponse WorkOrderDetailList(WorkOrderDetailListRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new WorkOrderDetailListResponse();
            //bhToken校验
            //if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
            //{
            //    response.ErrCode = -300;
            //    response.ErrMsg = "登录信息已过期，请重新登录";
            //    return response;
            //}
            IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            //参数校验
            if (!agentModel.AgentCanUse())
            {
                response.Status = HttpStatusCode.BadRequest;
                return response;
            }
            if (!AppValidateReqest(pairs, request.SecCode))
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }

            try
            {
                //根据OpenId、车牌号、代理人Id找Buid
                bx_userinfo bxUserInfo = _userInfoRepository.FindByAgentLicense(request.LicenseNo, request.ChildAgent.ToString());
                if (bxUserInfo == null)
                {
                    response.ErrCode = 0;
                    response.ErrMsg = "查不到此续保信息";
                    return response;
                }
                var bxWorkOrder = _userinfoRenewalInfoRepository.FindByBuid(bxUserInfo.Id);
                if (bxWorkOrder != null)
                {
                    response.ErrCode = 1;
                    response.WorkOrder = bxWorkOrder;
                    response.WorkOrderDetailList = _consumerReviewRepository.FindDetails(bxUserInfo.Id);
                }
                else
                {
                    response.ErrCode = 0;
                    response.ErrMsg = "查无数据";
                }
            }
            catch (Exception ex)
            {
                response.ErrCode = -1;
                logError.Info("获取受理记录异常，更新订单信息：" + request.ToJson() + "\n 异常信息:" + ex.StackTrace + " \n " + ex.Message);
            }
            return response;
        }

        public async Task<GetReInfoNewViewModel> GetReInfo(GetReInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            //返回对象
            var viewModel = new GetReInfoNewViewModel();
            #region 校验
            //bhToken校验
            if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
            {
                viewModel.BusinessStatus = -300;
                viewModel.StatusMessage = "登录信息已过期，请重新登录";
                return viewModel;
            }
            //传参校验
            if (!AppValidateReqest(pairs, request.SecCode))
            {
                viewModel.BusinessStatus = -301;
                viewModel.StatusMessage = "参数校验失败";
                return viewModel;
            }
            //代理人信息校验
            if (request.Agent == request.ChildAgent)
            {
                IBxAgent childagentModel = GetAgentModelFactory(request.ChildAgent);
                //1，当前代理人是否可用；
                if (!childagentModel.AgentCanUse())
                {
                    viewModel.BusinessStatus = -302;
                    viewModel.StatusMessage = "账号信息有变动，请退出重新登录";
                    return viewModel;
                }
                //2，顶级代理人是否可用；
                if (!_agentService.IsTopAgentId(request.Agent))
                {
                    viewModel.BusinessStatus = -302;
                    viewModel.StatusMessage = "账号信息有变动，请退出重新登录";
                    return viewModel;
                }
            }
            else
            {
                var childagentModel = GetAgentModelFactory(request.ChildAgent);
                if (!childagentModel.AgentCanUse())
                {
                    viewModel.BusinessStatus = -302;
                    viewModel.StatusMessage = "账号信息有变动，请退出重新登录";
                    return viewModel;
                }
                var agentModel = GetAgentModelFactory(request.Agent);
                if (!agentModel.AgentCanUse())
                {
                    viewModel.BusinessStatus = -302;
                    viewModel.StatusMessage = "账号信息有变动，请退出重新登录";
                    return viewModel;
                }
                if (!_agentService.IsTopAgentId(request.Agent))
                {
                    viewModel.BusinessStatus = -302;
                    viewModel.StatusMessage = "账号信息有变动，请退出重新登录";
                    return viewModel;
                }
                //3，当前代理人是否在顶级代理人下
                if (!_agentRepository.GetTopAgentId(request.ChildAgent).Contains(request.Agent.ToString()))
                {
                    viewModel.BusinessStatus = -302;
                    viewModel.StatusMessage = "账号信息有变动，请退出重新登录";
                    return viewModel;
                }
            }
            #endregion

            //是否在未分配的池子里
            bool isInAssign = false;

            //1,校验重复数据
            //userinfo 顶级代理下的所有代理，出过的车，存在就提示
            //原来userinfo里的记录
            var bxUserinfo = IsExistLicense(request, out isInAssign);

            //拼接请求串
            var sb = new StringBuilder();
            sb.Append("?");
            bool isAndroid = true;
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                if (pair.Key.ToLower() == "renewaltype")
                {
                    isAndroid = false;
                    sb.Append(string.Format("{0}={1}&", pair.Key, pair.Value));
                }
                else if (pair.Key.ToLower() == "childagent")
                {
                    //********************
                    //判断是否是新入店车辆
                    sb.Append(bxUserinfo != null
                        ? string.Format("{0}={1}&", pair.Key, bxUserinfo.Agent)
                        : string.Format("{0}={1}&", pair.Key, pair.Value));
                }
                else if (pair.Key.ToLower() == "custkey")
                {
                    //********************
                    //判断是否是新入店车辆
                    sb.Append(bxUserinfo != null
                        ? string.Format("{0}={1}&", pair.Key, bxUserinfo.OpenId)
                        : string.Format("{0}={1}&", pair.Key, pair.Value));
                }
                else
                {
                    sb.Append(string.Format("{0}={1}&", pair.Key, pair.Value));
                }

            }
            if (isAndroid)
            {
                sb.Append(string.Format("RenewalType={0}&", 7));
            }
            sb.Remove(sb.Length - 1, 1);
            string geturl = string.Format("http://{0}/api/CarInsurance/GetReInfo{1}", uri.Host + ":" + uri.Port, sb.ToString());

            //2,分配
            //给续保车分配续保顾问
            //********************
            //判断是否是新入店车辆
            var advAgent = bxUserinfo != null ? _agentRepository.GetAgent(int.Parse(bxUserinfo.Agent)) : AssignAdvAgent(request.Agent);
            int userinfoAgent = 0;//查询原来的userinfo用
            //********************
            //判断是否是新入店车辆
            if (bxUserinfo != null)
            {
                userinfoAgent = int.Parse(bxUserinfo.Agent);
            }
            else
            {
                userinfoAgent = request.ChildAgent > 0 ? request.ChildAgent : request.Agent;
            }

            //3,续保
            string strGetMsg = string.Empty;
            using (var client = new HttpClient())
            {
                var clientResult = client.GetAsync(geturl).Result;

                if (clientResult.StatusCode.ToString().Equals("429"))
                {
                    viewModel.BusinessStatus = -429;
                    viewModel.StatusMessage = "请求客户端过多，请稍候再试";
                    return viewModel;
                }
                if (clientResult.IsSuccessStatusCode)
                {
                    strGetMsg = await clientResult.Content.ReadAsStringAsync();
                }
                if (string.IsNullOrWhiteSpace(strGetMsg))
                {
                    viewModel.BusinessStatus = -10002;
                    viewModel.StatusMessage = "获取到续保信息失败";
                    return viewModel;
                }
                viewModel = strGetMsg.FromJson<GetReInfoNewViewModel>();
                viewModel.Agent = advAgent.Id;
                //********************
                //判断是否是新入店车辆
                if (bxUserinfo != null)
                {
                    viewModel.AgentName = !isInAssign ? advAgent.AgentName : "未分配";
                }
                else
                {
                    viewModel.AgentName = "未分配";
                }
            }

            //插入意向

            //4,插入续保修改人日志
            //
            var newBxUserinfo = new bx_userinfo();

            //********************
            //判断是否是新入店车辆
            if (bxUserinfo == null)
            {
                newBxUserinfo = GetUserInfo(request.LicenseNo, userinfoAgent.ToString());
                if (newBxUserinfo == null)
                {
                    viewModel.BusinessStatus = -10002;
                    viewModel.StatusMessage = "获取到续保信息失败";
                    return viewModel;
                }
                //修改userinfo
                newBxUserinfo.OpenId = advAgent.OpenId;
                newBxUserinfo.Agent = advAgent.Id.ToString();
                newBxUserinfo.UpdateTime = DateTime.Now;
                if (_userInfoRepository.Update(newBxUserinfo) > 0)
                {
                    viewModel.CustKey = advAgent.OpenId;
                }

            }

            if (bxUserinfo != null)
            {
                //返回当前记录的buid
                viewModel.Buid = bxUserinfo.Id.ToString();
                //插入续保修改记录
                AddChangeLog(bxUserinfo.Id, userinfoAgent, request.ChildAgent, 1);//第一步
                if (!isInAssign)
                {//如果不在未分配的池子里
                    AddChangeLog(bxUserinfo.Id, request.ChildAgent, advAgent.Id, 2); //第二步
                }
                else
                {
                    AddChangeLog(bxUserinfo.Id, request.ChildAgent, advAgent.Id, 6); //第二步
                }
            }
            else
            {
                //返回当前记录的buid
                viewModel.Buid = newBxUserinfo.Id.ToString();
                //插入续保修改记录
                AddChangeLog(newBxUserinfo.Id, userinfoAgent, 0, 1);//第一步
                AddChangeLog(newBxUserinfo.Id, userinfoAgent, advAgent.Id, 6); //第二步
            }

            //********************
            //判断是否是新入店车辆
            if (bxUserinfo == null)
            {
                //5，添加分配通知
                var msgRequest = new AddMessageRequest
                {
                    Body = request.LicenseNo,
                    MsgType = 6,
                    ToAgentId = advAgent.Id,
                    MsgLevel = 1,
                    SendTime = DateTime.Now.ToString(),
                    ChildAgent = request.ChildAgent,
                    LicenseNo = request.LicenseNo,
                    Buid = newBxUserinfo.Id
                };
                _messageService.AddMessage(msgRequest);
            }
            //6，添加续保通知，向bx_notice_xb插记录，商业险或交强险在90天范围之内
            //long noticeBuid = bxUserinfo != null ? bxUserinfo.Id : newBxUserinfo.Id;
            //if (viewModel.UserInfo != null)
            //{
            //    _noticexbService.AddNoticexb(viewModel.UserInfo.LicenseNo, viewModel.UserInfo.BusinessExpireDate,
            //        viewModel.UserInfo.ForceExpireDate, viewModel.UserInfo.NextBusinessStartDate,
            //        viewModel.UserInfo.NextForceStartDate, viewModel.SaveQuote.Source, advAgent.Id, noticeBuid, 1);
            //}
            return viewModel;
        }

        public ChangeReInfoAgentResponse ChangeReInfoAgent(ChangeReInfoAgentRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new ChangeReInfoAgentResponse();
            var agentModel = GetAgentModelFactory(request.Agent);
            //参数校验
            if (agentModel == null)// || childAgent == null)
            {
                response.Status = HttpStatusCode.BadRequest;
                return response;
            }
            if (!AppValidateReqest(pairs, request.SecCode))
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }
            try
            {
                //根据OpenId、车牌号、代理人Id找Buid
                bx_userinfo bxUserInfo = _userInfoRepository.FindByAgentLicense(request.LicenseNo, request.OwnerAgent.ToString());
                if (bxUserInfo == null)
                {
                    response.ErrCode = 0;
                    response.ErrMsg = "查不到此续保信息";
                    return response;
                }
                bx_agent bxAgent = _agentRepository.GetAgent(request.AssignId);
                if (bxAgent == null)
                {
                    response.ErrCode = 0;
                    response.ErrMsg = "查不到分配人信息";
                    return response;
                }
                bxUserInfo.Agent = request.AssignId.ToString();
                bxUserInfo.OpenId = bxAgent.OpenId;
                bxUserInfo.UpdateTime = DateTime.Now;
                if (_userInfoRepository.Update(bxUserInfo) > 0 || request.OwnerAgent == request.AssignId)
                {
                    AddChangeLog(bxUserInfo.Id, request.OwnerAgent, request.AssignId, 2); //第二步

                    //5，添加分配通知
                    //AddMessageRequest msgRequest = new AddMessageRequest();
                    //msgRequest.Body = request.LicenseNo;
                    //msgRequest.MsgType = 6;
                    //msgRequest.ToAgentId = request.AssignId;
                    //msgRequest.MsgLevel = 1;
                    //msgRequest.SendTime = DateTime.Now.ToString();
                    //msgRequest.ChildAgent = request.OwnerAgent;
                    //_messageService.AddMessage(msgRequest);

                    //bx_notice_xb xbmodel = _noticexbService.Find(bxUserInfo.Id);
                    bx_car_renewal bxCarRenewal = _carRenewalRepository.FindByLicenseno(bxUserInfo.LicenseNo);
                    if (bxCarRenewal != null)
                    {
                        _noticexbService.AddNoticexb(0, bxUserInfo.LicenseNo,
                            bxCarRenewal.LastBizEndDate.HasValue ? bxCarRenewal.LastBizEndDate.Value.ToString() : "",
                        bxCarRenewal.LastForceEndDate.HasValue ? bxCarRenewal.LastForceEndDate.Value.ToString() : "",
                        bxCarRenewal.NextBizStartDate.HasValue ? bxCarRenewal.NextBizStartDate.Value.ToString() : "",
                        bxCarRenewal.NextForceStartDate.HasValue ? bxCarRenewal.NextForceStartDate.Value.ToString() : "",
                        bxCarRenewal.LastYearSource.HasValue ? bxCarRenewal.LastYearSource.Value : -1, int.Parse(bxUserInfo.Agent), request.Agent, bxUserInfo.Id, 0);
                    }
                    response.ErrCode = 1;
                    response.ErrMsg = "分配成功";
                    return response;
                }
                else
                {
                    response.ErrCode = 0;
                    response.ErrMsg = "分配失败";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.ErrCode = -10003;
                logError.Info("分发接口异常，分发信息：" + request.ToJson() + "\n 异常信息:" + ex.StackTrace + " \n " + ex.Message);
            }
            return response;
        }

        public ReVisitedListViewModel WorkOrderList(long buid)
        {
            var viewModel = new ReVisitedListViewModel();
            bx_userinfo_renewal_info model = _userinfoRenewalInfoRepository.FindByBuid(buid);
            List<bx_consumer_review> list = _consumerReviewRepository.FindDetails(buid);
            if (!list.Any())//model == null && 
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "获取信息失败";
                return viewModel;
            }
            viewModel.BusinessStatus = 1;
            if (model != null)
            {
                var info = new VisitedInfo
                {
                    CustomerName = model.client_name,
                    Phone = model.client_mobile
                };
                viewModel.VisitedInfo = info;
            }
            if (list.Any())
            {
                var visitedList = new List<VisitedDetail>();
                var item = new VisitedDetail();
                foreach (var obj in list)
                {
                    item.Id = obj.id;
                    item.RecallTime = obj.next_review_date.HasValue ? obj.next_review_date.Value.ToString() : "";
                    item.ResultStatus = obj.result_status;
                    item.CreateTime = obj.create_time.HasValue ? obj.create_time.Value.ToString() : "";
                    item.Remark = obj.content;
                    visitedList.Add(item);
                }
                viewModel.VisitedList = visitedList;
            }
            return viewModel;
        }

        public BaseViewModel AddReVisited(AddReVisitedRequest request)
        {
            var viewModel = new BaseViewModel();
            try
            {
                DateTime dateTime = DateTime.Now;

                var bxUserInfo = new bx_userinfo();
                if (request.Buid.HasValue && request.Buid.Value != 0)
                {//根据buid获取userinfo对象
                    bxUserInfo = _userInfoRepository.FindByBuid(request.Buid.Value);
                }

                if (bxUserInfo == null)
                {
                    viewModel.BusinessStatus = -10002;
                    viewModel.StatusMessage = "获取信息失败";
                    return viewModel;
                }
                int workOrderId = 0;
                bx_userinfo_renewal_info workorder = _userinfoRenewalInfoRepository.FindByBuid(request.Buid.Value);
                if (workorder == null)
                {
                    workorder = new bx_userinfo_renewal_info();
                    workorder.sa_id = request.ChildAgent;
                    workorder.sa_name = request.ChildName;
                    workorder.client_name = request.CustomerName;
                    workorder.client_mobile = request.Phone;
                    workorder.b_uid = request.Buid.Value;
                    workorder.client_intention = 3;//默认添加，其他情况
                    workorder.create_time = dateTime;
                    workorder.xubao_id = request.ChildAgent;
                    workorder.xubao_name = request.ChildName;
                    workOrderId = _userinfoRenewalInfoRepository.Add(workorder);
                }
                else
                {
                    workOrderId = workorder.id;
                    if (!string.IsNullOrWhiteSpace(request.CustomerName) || !string.IsNullOrWhiteSpace(request.Phone))
                    {
                        workorder.client_name = request.CustomerName;
                        workorder.client_mobile = request.Phone;
                        if (_userinfoRenewalInfoRepository.Update(workorder) == 0)
                        {
                            //将失败信息插入到error里
                        }
                    }
                }
                if (workOrderId > 0)
                {
                    var workOrderDetail = new bx_consumer_review
                    {
                        b_uid = (int)bxUserInfo.Id,
                        content = request.Remark,
                        operatorId = request.ChildAgent,
                        operatorName = request.ChildName,
                        read_status = 0,//0未读，1已读
                        status = request.Status,//跟进类型,0回访,1投诉,2跟进..
                        result_status = 4,//受理结果，默认 其他。目前暂不使用
                        create_time = dateTime
                    };
                    if (!string.IsNullOrWhiteSpace(request.NextReviewDate))
                    {
                        workOrderDetail.next_review_date = DateTime.Parse(request.NextReviewDate);
                    }
                    int workOrderDetailId = _consumerReviewRepository.AddDetail(workOrderDetail);
                    if (workOrderDetailId > 0)
                    {
                        viewModel.BusinessStatus = 1;
                    }
                    else
                    {
                        viewModel.BusinessStatus = -10002;
                        viewModel.StatusMessage = "添加信息失败";
                    }
                }
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务器发生异常";
                logError.Info("添加受理记录异常，受理记录信息：" + request.ToJson() + "\n 异常信息:" + ex.StackTrace + " \n " + ex.Message);
            }
            return viewModel;
        }

        private bx_userinfo IsExistLicense(GetReInfoRequest request, out bool isInAssign)
        {
            //是否在未分配的池子里
            isInAssign = false;
            string topAgents = string.Empty;
            var listRecord = new List<bx_transferrecord>();
            //根据车牌取出所有的当前agent
            var userinfos = _userInfoRepository.FindAgentListByLicenseNo(request.LicenseNo);

            var listTopAgents = _agentRepository.GetTopAgentByIds(string.Join(",", userinfos.Select(x => x.Agent).ToList()));
            //取出所有的agent的顶级代理
            foreach (var item in userinfos)
            {
                if (!string.IsNullOrWhiteSpace(item.Agent))
                {
                    var agentAndTopAgent = listTopAgents.FirstOrDefault(x => x.Id == Convert.ToInt32(item.Agent));
                    topAgents = agentAndTopAgent != null ? agentAndTopAgent.TopAgentId.ToString() : "";
                    //topAgents = _agentRepository.GetTopAgentId(int.Parse(item.Agent));
                    if (topAgents.Equals(request.Agent.ToString()))
                    {
                        listRecord = new List<bx_transferrecord>();
                        listRecord = _transferRecordRepository.FindByBuid(item.Id);
                        if (listRecord.Any())
                        {
                            if (listRecord[0].StepType == 6)
                            {
                                isInAssign = true;
                            }
                        }
                        return item;
                    }
                }
            }
            return null;
        }

        private bx_agent AssignAdvAgent(int agent)
        {
            bx_agent bxAgent = null;
            //取出所有的续保顾问
            List<bx_agent_distributed> list = _agentDistributedRepository.FindByParentAgent(agent);
            if (list.Any())
            {
                //随机取一条
                //bx_agent_distributed bxAgentDistributed = list.OrderBy(_ => Guid.NewGuid()).First();
                bx_agent_distributed bxAgentDistributed = list.First();
                //根据agentid取出agent对象
                bxAgent = _agentRepository.GetAgent(bxAgentDistributed.AgentId);
            }
            else
            {
                //如果没有续保顾问的资源池，就分配给顶级
                bxAgent = _agentRepository.GetAgent(agent);
            }
            return bxAgent;
        }

        /// <summary>
        /// 添加历史记录
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="saAgent"></param>
        /// <param name="advAgent"></param>
        /// <param name="stepType">当前步骤类型  1:数据录入、2:分配(sa到续保顾问)、3:报价、4:预约、5:保单</param>
        /// <returns></returns>
        private bool AddChangeLog(long buid, int saAgent, int advAgent, int stepType)
        {
            var model = new bx_transferrecord
            {
                BuId = buid,
                CreateAgentId = saAgent,
                CreateTime = DateTime.Now,
                Deleted = false,
                FromAgentId = saAgent,
                StepType = stepType,
                ToAgentId = advAgent
            };
            return _transferRecordRepository.Add(model) > 0;//如果添加的记录>0，返回true
        }

        private bx_userinfo GetUserInfo(string licenseNo, string agentId)
        {
            return _userInfoRepository.FindByAgentLicense(licenseNo, agentId);
        }

    }
}
