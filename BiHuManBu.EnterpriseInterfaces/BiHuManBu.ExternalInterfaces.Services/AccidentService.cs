using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using System.Configuration;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using BiHuManBu.ExternalInterfaces.Models;
using System.Text.RegularExpressions;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using AppIRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository;
using log4net;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System.Reflection;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Sms;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Sms;
using Newtonsoft.Json.Linq;
using Baidu.Aip.Nlp;
using System.Globalization;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using Newtonsoft.Json;
using BiHuManBu.ExternalInterfaces.Services.Common;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class AccidentService : IAccidentService
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        private ILog logInfo = LogManager.GetLogger("INFO");
        private readonly ILoginService _loginService;
        private readonly string _keyCode = ConfigurationManager.AppSettings["keyCode"];
        private readonly INoticeMessageRepository _noticeMessageRepository;
        private readonly Interfaces.AppInterfaces.IAppVerifyService _appVerifyService;
        private readonly IAccidentRepository _accidentRepository;
        private readonly IBxConfigRepository _bxConfigRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IAgentService _agentService;
        private readonly IAccidentSettingRepository _accidentSettingRepository;
        private readonly IClueManagerRepository _clueManagerRepository;
        private readonly ITXCluesAgentRelationshipRepository _tXCluesAgentRelationship;
        private readonly IAccidentMqMessageRepository _accidentMqMessageRepository;

        private readonly AppIRepository.IMessageRepository _messageAppRepository;
        private readonly ITXClueFollowRecordRepository _tXClueFollowRecord;
        private readonly IAccidentMqMessageService _accidentMqMessageService;
        private readonly ISmsService _smsService;
        private readonly IConsumerDetailService _consumerDetailService;
        private readonly ISmsBulkSendManageService _smsBulkSendManageService;
        private readonly IEnterpriseAgentService _enterpriseAgentService;
        private readonly IRenewalInfoRepository _renewalInfoRepository;
        private readonly IAccidentOrderService _accidentOrderService;
        public string AccidentClueAppVersionKey = "AccidentClueApp_Version_Key";
        public string AccidentClueAppVersion = "AccidentClueApp_Version";
        public string AccidentClueAppUploadKey = "AccidentClueApp_upload_Key";
        public string AccidentClueAppIsUpload = "AccidentClueApp_IsUpload";
        public string AccidentClueServiceAppVersion = "AccidentClueServiceApp_Version";

        private string apiKey = "vcYPedVFMxHPg8RuH5YjLln5";
        private string cecretKey = "feAfk7njkuMM4lamZVdD54wLGmsNIhlC";

        private Nlp client = null;
        public AccidentService(ILoginService loginService, INoticeMessageRepository noticeMessageRepository, Interfaces.AppInterfaces.IAppVerifyService appVerifyService, IAccidentRepository accidentRepository, IBxConfigRepository bxConfigRepository, IMessageRepository messageRepository, IAgentService agentService,
            IAccidentSettingRepository accidentSettingRepository, IClueManagerRepository clueManagerRepository,
            AppIRepository.IMessageRepository messageAppRepository, ITXClueFollowRecordRepository tXClueFollowRecord,
            ITXCluesAgentRelationshipRepository tXCluesAgentRelationship, IAccidentMqMessageService accidentMqMessageService, IAccidentMqMessageRepository accidentMqMessageRepository, ISmsService smsService,
            IConsumerDetailService consumerDetailService, ISmsBulkSendManageService smsBulkSendManageService, IEnterpriseAgentService enterpriseAgentService, IRenewalInfoRepository renewalInfoRepository,
            IAccidentOrderService accidentOrderService)
        {
            _loginService = loginService;
            _noticeMessageRepository = noticeMessageRepository;
            _appVerifyService = appVerifyService;
            _accidentRepository = accidentRepository;
            _messageRepository = messageRepository;
            _bxConfigRepository = bxConfigRepository;
            _agentService = agentService;
            _accidentSettingRepository = accidentSettingRepository;
            _clueManagerRepository = clueManagerRepository;
            _tXClueFollowRecord = tXClueFollowRecord;
            _tXCluesAgentRelationship = tXCluesAgentRelationship;
            _accidentMqMessageRepository = accidentMqMessageRepository;
            _messageAppRepository = messageAppRepository;
            _accidentMqMessageService = accidentMqMessageService;
            _smsService = smsService;
            _consumerDetailService = consumerDetailService;
            _smsBulkSendManageService = smsBulkSendManageService;
            _enterpriseAgentService = enterpriseAgentService;
            _renewalInfoRepository = renewalInfoRepository;
            _accidentOrderService = accidentOrderService;
        }

        /// <summary>
        /// 事故线索app登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccidentResponse Login(AccidentLoginRequest request)
        {
            var accidentResponse = new AccidentResponse();
            accidentResponse.Data = new AccidentLoginModel();
            try
            {
                var dic = new Dictionary<string, string>();
                dic.Add("Account", request.Account);
                dic.Add("Password", request.Password);
                dic.Add("CustKey", request.CustKey);
                dic.Add("LoginType", request.LoginType);
                dic.Add("KeyCode", _keyCode);
                Dictionary<string, string> ascdic = dic.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value.ToString());
                var parms = new StringBuilder();
                foreach (var item in ascdic)
                {
                    if (parms.Length == 0)
                        parms.Append(item.Key + "=" + item.Value);
                    else
                        parms.Append("&" + item.Key + "=" + item.Value);
                }

                if (parms.ToString().GetUrl().GetMd5() == request.SecCode)
                {
                    var model = _loginService.Login(request.Account, request.Password.GetMd5(), request.CustKey, 4, 0, 0, 0, true, 0);
                    if (model != null)
                    {
                        var exit = 0;
                        var cityId = 0;
                        if (model.BusinessStatus > 0)
                        {
                            var agentModel = _agentService.GetAgent(Convert.ToInt32(model.topAgentId));
                            if (agentModel == null || agentModel.Id < 1)
                                exit = exit + 1;
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(agentModel.Region))
                                {
                                    if (agentModel.Region.Equals("0"))
                                        exit = exit + 1;
                                    else
                                    {
                                        try
                                        {
                                            cityId = Convert.ToInt32(agentModel.Region);
                                            if (cityId < 1)
                                                exit = exit + 1;
                                        }
                                        catch
                                        {
                                            exit = exit + 1;
                                        }
                                    }
                                }
                                else
                                {
                                    exit = exit + 1;
                                }
                            }
                        }

                        var resultModel = new AccidentLoginModel();
                        resultModel.Name = model.agentName;
                        resultModel.AgentId = model.agentId;
                        resultModel.Account = request.Account;
                        resultModel.TopAgentId = Convert.ToInt32(model.topAgentId);
                        resultModel.RoleId = model.managerRoleId;
                        resultModel.RoleType = Convert.ToInt32(model.roleType);
                        resultModel.RoleName = GetRoleName(model.managerRoleId);
                        resultModel.DepartmentId = model.DepartmentId;
                        resultModel.SecretKey = model.secretKey;
                        resultModel.Token = model.token;
                        resultModel.CityId = cityId;
                        resultModel.FunctionCode = _accidentRepository.GetFunctionCodeByAgentId(model.agentId);
                        accidentResponse.Code = model.BusinessStatus < 1 ? -1 : (exit > 0 ? -1 : model.BusinessStatus);
                        accidentResponse.Message = exit > 0 ? "该帐号所属城市为空，需要联系壁虎进行完善" : model.StatusMessage;
                        accidentResponse.Data = resultModel;

                        if (accidentResponse.Code > 0)
                        {
                            var settingModel = _agentService.GetAgentSettingModel(resultModel.TopAgentId);
                            var txAgentModel = _agentService.GetBusinessModel(resultModel.TopAgentId);
                            var is_open_tuixiu = 0;
                            var singedState = 0;
                            if (settingModel != null && settingModel.id > 0)
                            {
                                is_open_tuixiu = Convert.ToInt32(settingModel.is_open_tuixiu);
                            }
                            if (txAgentModel != null && txAgentModel.Id > 0)
                            {
                                singedState = txAgentModel.SignedState;
                            }
                            if (is_open_tuixiu != 1 && singedState != 1)
                            {
                                accidentResponse.Code = -1;
                                accidentResponse.Message = "该账号没有理赔权限";
                                accidentResponse.Data = new AccidentLoginModel();
                                return accidentResponse;
                            }
                            #region 留修机器人登录规则，顶级账号以及理赔主管，有理赔权限的账号可以登录留修机器人。如果承保权限的账号登录，提示：该账号没有理赔权限。
                            var exitModule = 0;
                            if (model.module == null)
                                exitModule = exitModule + 1;
                            else
                            {
                                var module = model.module.Where(o => o.crm_module_type == 2).ToList();//crm菜单类型1承保2推修
                                if (module == null || module.Count() < 1)
                                    exitModule = exitModule + 1;
                            }
                            if (exitModule > 0)
                            {
                                accidentResponse.Code = -1;
                                accidentResponse.Message = "该账号没有理赔权限";
                                accidentResponse.Data = new AccidentLoginModel();
                                return accidentResponse;
                            }
                            #endregion

                            var custKey = CacheProvider.Get<string>("Accident_Login_Agent_" + resultModel.AgentId);
                            if (!string.IsNullOrWhiteSpace(custKey))
                            {
                                if (!request.CustKey.Equals(custKey))
                                {
                                    accidentResponse.Code = -2;
                                    accidentResponse.Message = "该账号已登录，请在原有设备上退出该账号后再次进行登录";
                                    accidentResponse.Data = new AccidentLoginModel();
                                    return accidentResponse;
                                }
                            }
                            TimeSpan span = DateTime.Now.AddDays(30) - DateTime.Now;
                            var seconds = Convert.ToInt64(span.TotalSeconds);
                            CacheProvider.Set("Accident_Login_Agent_" + resultModel.AgentId, request.CustKey, seconds);

                        }
                    }
                    else
                    {
                        accidentResponse.Code = -1;
                        accidentResponse.Message = "登录失败";
                    }
                }
                else
                {
                    accidentResponse.Code = -1;
                    accidentResponse.Message = "校验失败";
                }
            }
            catch (Exception)
            {
                accidentResponse.Code = -1;
                accidentResponse.Message = "服务器发生异常";
            }
            if (accidentResponse.Code == -1)
                accidentResponse.Data = new AccidentLoginModel();

            return accidentResponse;
        }

        public string GetRoleName(int roleId)
        {
            var roleName = string.Empty;
            if (roleId < 1) return roleName;
            var model = _accidentRepository.GetRoleModelById(roleId);
            if (model != null && model.id > 0)
                roleName = model.role_name;
            return roleName;
        }

        /// <summary>
        /// 获取事故线索app消息列表
        /// </summary>
        /// <param name="reqest"></param>
        /// <returns></returns>
        public AccidentMessageResponse GetMessage(AccidentMessageRequest reqest, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var result = new AccidentMessageResponse();
            result.Data = new AccidentMessageModel();
            if (reqest.PageIndex == 0)
                reqest.PageIndex = 1;
            if (reqest.PageSize == 0)
                reqest.PageSize = 5;
            var baseRequest = new AppBaseRequest()
            {
                Agent = reqest.TopAgentId,
                SecCode = reqest.SecCode,
                BhToken = reqest.Token,
                ChildAgent = reqest.AgentId
            };

            var exitModel = _appVerifyService.Verify(baseRequest, pairs);
            if (exitModel.ErrCode != 1)
            {
                result.Code = (exitModel.ErrCode == -13000 || exitModel.ErrCode == -13020) ? -13000 : -1;
                result.Message = exitModel.ErrMsg;
                return result;
            }

            var model = new AccidentMessageModel();
            var totalCount = 0;
            if (!string.IsNullOrWhiteSpace(reqest.Version) && reqest.Version.Equals("2.3.0"))
            {
                model.MessageList = _noticeMessageRepository.GetMessageListV23(reqest.PageIndex, reqest.PageSize, reqest.AgentId, out totalCount);
                var code = _accidentRepository.GetFunctionCodeByAgentId(reqest.AgentId);
                if (code != null && code.Count > 0)
                {
                    var exitResult = code.Where(o => o.FunctionCode.ToLower().Equals("graborder")).ToList();
                    if (exitResult != null && exitResult.Count > 0)
                        model.RobbingMessageList = _noticeMessageRepository.GetRobbingMessageList(reqest.AgentId);
                }
            }
            else
            {
                model.MessageList = _noticeMessageRepository.GetMessageList(reqest.PageIndex, reqest.PageSize, reqest.AgentId, out totalCount);
            }
            if (model.RobbingMessageList == null)
                model.RobbingMessageList = new List<RobbingModel>();
            model.TotalCount = totalCount;
            model.PageIndex = reqest.PageIndex;
            model.PageSize = reqest.PageSize;
            result.Data = model;
            result.Code = 1;
            if (model.MessageList == null || model.MessageList.Count == 0)
            {
                result.Message = "暂无数据";
            }
            else
            {
                result.Message = "获取成功";
            }

            return result;
        }


        /// <summary>
        /// 获取线索列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public AccidentClueResponse GetClueList(AccidentClueRequest request)
        {
            var result = new AccidentClueResponse();
            result.Data = new AccidentClueDataModel();
            if (request.PageIndex == 0)
                request.PageIndex = 1;
            if (request.PageSize == 0)
                request.PageSize = 5;
            var model = new AccidentClueDataModel();
            var totalCount = 0;
            model.ClueList = _accidentRepository.GetClueList(request.PageIndex, request.PageSize, out totalCount, request.ClueState, request.TimeType, request.CarInfo, request.AgentId, request.TopAgentId, request.RoleType);
            model.PageIndex = request.PageIndex;
            model.PageSize = request.PageSize;
            model.TotalCount = totalCount;
            result.Data = model;
            result.Code = 1;
            if (model.ClueList == null || model.ClueList.Count == 0)
            {
                result.Message = "暂无数据";
            }
            else
            {
                result.Message = "获取成功";
            }
            return result;
        }

        /// <summary>
        /// 获取线索列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccidentClueResponse GetClueList(AccidentListRequest request)
        {
            var result = new AccidentClueResponse();
            result.Data = new AccidentClueDataModel();
            if (request.PageIndex == 0)
                request.PageIndex = 1;
            if (request.PageSize == 0)
                request.PageSize = 10;
            var model = new AccidentClueDataModel();
            var totalCount = 0;
            model.ClueList = _accidentRepository.GetClueList(request, out totalCount);
            model.PageIndex = request.PageIndex;
            model.PageSize = request.PageSize;
            model.TotalCount = totalCount;
            result.Data = model;
            result.Code = 1;
            if (model.ClueList == null || model.ClueList.Count == 0)
            {
                result.Message = "暂无数据";
            }
            else
            {
                result.Message = "获取成功";
            }
            return result;
        }

        public AccidentClueTotalModel GetCluesCountWithState(AccidentListRequest accidentListRequest)
        {
            var dt = _accidentRepository.GetCluesCountWithState(accidentListRequest);
            var accidentClueTotal = new AccidentClueTotalModel();
            var totalCount = 0;
            var sumFollowUpTotalCount = 0;
            foreach (var dc in dt)
            {
                totalCount += dc.Value;
                switch (dc.Key)
                {
                    case -1:
                        accidentClueTotal.UntreatedTotalCount = dc.Value;
                        break;
                    case 1:
                        accidentClueTotal.FollowUpTotalCount = dc.Value;
                        sumFollowUpTotalCount += dc.Value;
                        break;
                    case 2:
                        accidentClueTotal.PickUpTotalCount = dc.Value;
                        sumFollowUpTotalCount += dc.Value;
                        break;
                    case 5:
                        accidentClueTotal.SMSTotalCount = dc.Value;
                        sumFollowUpTotalCount += dc.Value;
                        break;
                    case 6:
                        accidentClueTotal.CallTotalCount = dc.Value;
                        sumFollowUpTotalCount += dc.Value;
                        break;
                    case 7:
                        accidentClueTotal.StoreReceivedCount = dc.Value;
                        sumFollowUpTotalCount += dc.Value;
                        break;
                    case 3:
                        accidentClueTotal.VehicleToStoreTotalCount = dc.Value;
                        break;
                    case 4:
                        accidentClueTotal.LossTotalCount = dc.Value;
                        break;
                    case 8:
                        accidentClueTotal.MaintainCount = dc.Value;
                        break;
                    case 9:
                        accidentClueTotal.HandOverCount = dc.Value;
                        break;
                }
            }
            accidentClueTotal.SumFollowUpTotalCount = sumFollowUpTotalCount;
            accidentClueTotal.TotalCount = totalCount;
            return accidentClueTotal;
        }

        public bool AnalysisSms(AnalysisSmsRequest request, ref ClueNotificationDto dto)
        {
            //获取顶级代理
            string message = string.Empty;
            var asm = _accidentSettingRepository.GetPhoneSetting(request.Moblie);
            if (asm == null)
            {
                logInfo.Info(request.Moblie + "手机号没收绑定");
                return false;
            }
            if (_accidentRepository.SMSIsExist(request.Moblie, request.SmsContent))
            {
                return false;
            }




            //解析短信
            var clueM = RegAnalysisSms(request, asm.agentid);

            if (clueM == null)
            {
                logInfo.Info(request.Moblie + "无此类短信，解析失败");
                return false;
            }
            if (string.IsNullOrEmpty(clueM.mobile))
            {
                logInfo.Info(request.Moblie + "手机号为空，解析失败");
                return false;
            }

            //保存原始短信
            int smsId = _accidentRepository.SaveSMS(AnalysisSmsCTxsms(request, asm.agentid));
            if (smsId <= 0)
            {
                logInfo.Info(request.Moblie + "保存短信失败");
                return false;
            }
            clueM.smsid = smsId;

            if (_clueManagerRepository.CluesIsExist(clueM.mobile)) {
                logInfo.Info(request.Moblie + "今天已存在该线索");
                return false;
            }
            //保存线索
            int clueId = _clueManagerRepository.Add(clueM);
            if (clueId <= 0)
            {
                logInfo.Info(request.Moblie + "保存线索失败");
                return false;
            }

            //获取推送方式
            var dism = _accidentSettingRepository.GetClueDistributeRuleSetting(asm.agentid);
            if (dism == null)
            {
                logInfo.Info(request.Moblie + "获取推送方式失败");
                return false;
            }

            //获取推送人员
            List<TXPushPeople> tpp = _clueManagerRepository.GetPushPeople(asm.agentid, dism.DistributeType);

            List<TXPushPeople> tppLeader = _clueManagerRepository.GetPushLeader(asm.agentid);
            if (tpp == null || tpp.Count == 0)
            {
                logInfo.Info(request.Moblie + "获取推送人员失败，原因轮询且无线索人员");
                if (tppLeader != null || tppLeader.Count > 0)
                {
                    foreach (var item in tppLeader)
                    {
                        //保存跟进记录
                        _tXClueFollowRecord.Add(new tx_cluefollowuprecord()
                        {
                            ArrivalType = 0,//未知
                            fromagentid = item.Id,
                            toagentid = item.Id,
                            CreateTime = DateTime.Now,
                            Deleted = 0,
                            clueid = clueId,
                            IsNotice = 0,
                            loss_reason_id = 0,
                            nextfollowuptime = null,
                            nextstate = -1,
                            parentstate = -1,
                            state = -1,
                            UpdateTime = DateTime.Now,
                            Source = clueM.source,
                            roletype = item.RoleType,
                            ReciveCarType = 3,//未知
                        });
                        _tXCluesAgentRelationship.Add(new tx_clues_agent_relationship()
                        {
                            AgentId = item.Id,
                            ClueId = clueM.id,
                            TopAgentId = asm.agentid
                        });

                    }
                    
                }
                return false;
            }


            //推送实体
            // ClueNotificationDto cluen = new ClueNotificationDto();
            dto.Licenseno = clueM.licenseno;
            dto.MoldName = clueM.MoldName;
            dto.AcceptanceAgentInfoes = new List<AcceptanceAgentInfo>();
            dto.OprateAgentId = clueM.agentid;//解析短信的场景为顶级代理人
                                              // dto.MessageType = 0;//出险通知
            dto.ClueId = clueId;
            dto.ClueState = -1;//未处理
            dto.ClueCreateTime = clueM.CreateTime.ToString();



            if (dism.DistributeType == 2|| dism.DistributeType==0)//广播
            {

                foreach (var agent in tpp)
                {
                    AcceptanceAgentInfo info = new AcceptanceAgentInfo();
                    info.AcceptanceAgentId = agent.Id;
                    info.TopAgentId = clueM.agentid;
                    info.RoleType = agent.RoleType;


                    //保存跟进记录
                    int followId = _tXClueFollowRecord.Add(new tx_cluefollowuprecord()
                    {
                        ArrivalType = 0,//未知
                        fromagentid = agent.Id,
                        toagentid = agent.Id,
                        CreateTime = DateTime.Now,
                        Deleted = 0,
                        clueid = clueId,
                        IsNotice = 0,
                        loss_reason_id = 0,
                        nextfollowuptime = null,
                        nextstate = -1,
                        parentstate = -1,
                        state = -1,
                        UpdateTime = DateTime.Now,
                        Source = clueM.source,
                        roletype = agent.RoleType,
                        ReciveCarType = 3,//未知
                    });
                    info.FollowId = followId;
                    dto.AcceptanceAgentInfoes.Add(info);
                    _tXCluesAgentRelationship.Add(new tx_clues_agent_relationship()
                    {
                        AgentId = agent.Id,
                        ClueId = clueM.id,
                        TopAgentId = asm.agentid
                    });
                }
            }
            else if(dism.DistributeType == 1)
            {//轮询
                string ids = string.Empty;
                foreach (var item in tpp)
                {
                    ids += item.Id + ",";
                }
                ids = ids.Substring(0, ids.Length - 1);
                List<tx_noticemessage> nms = _accidentMqMessageRepository.GetPollingPeople(ids, tpp.Count);
                int aid = 0;
                int managerRoleId = 0;
                if (nms != null)
                {
                    if (nms.Count != tpp.Count)
                    {
                        foreach (var item in nms)
                        {
                            tpp.Remove(tpp.Where(x => x.Id == item.reciveaentId).FirstOrDefault());
                        }
                        if (tpp != null && tpp.Count > 0)
                        {
                            aid = tpp[0].Id;
                            managerRoleId = tpp[0].RoleType;
                        }

                    }
                    else
                    {
                        if (tpp != null && tpp.Count > 0)
                        {

                            foreach (var item in tpp)
                            {
                                //取最早推送过的人
                                if (item.Id == nms[nms.Count - 1].reciveaentId)
                                {
                                    managerRoleId = item.RoleType;
                                }
                            }
                            aid = nms[nms.Count - 1].reciveaentId;



                        }

                    }



                }

                //保存跟进记录
                int followId = _tXClueFollowRecord.Add(new tx_cluefollowuprecord()
                {
                    ArrivalType = 0,//未知
                    fromagentid = aid,
                    toagentid = aid,
                    CreateTime = DateTime.Now,
                    Deleted = 0,
                    clueid = clueId,
                    IsNotice = 0,
                    loss_reason_id = 0,
                    nextfollowuptime = null,
                    nextstate = -1,
                    parentstate = -1,
                    state = -1,
                    UpdateTime = DateTime.Now,
                    Source = clueM.source,
                    roletype = managerRoleId,
                    ReciveCarType = 3,//未知
                });
                _tXCluesAgentRelationship.Add(new tx_clues_agent_relationship()
                {
                    AgentId = aid,
                    ClueId = clueM.id,
                    TopAgentId = asm.agentid
                });
                AcceptanceAgentInfo info = new AcceptanceAgentInfo();
                info.AcceptanceAgentId = aid;
                info.TopAgentId = clueM.agentid; info.RoleType = managerRoleId;
                info.FollowId = followId;
                dto.AcceptanceAgentInfoes.Add(info);
                if (tppLeader != null || tppLeader.Count > 0)
                {
                    foreach (var item in tppLeader)
                    {
                        _tXClueFollowRecord.Add(new tx_cluefollowuprecord()
                        {
                            ArrivalType = 0,//未知
                            fromagentid = item.Id,
                            toagentid = item.Id,
                            CreateTime = DateTime.Now,
                            Deleted = 0,
                            clueid = clueId,
                            IsNotice = 0,
                            loss_reason_id = 0,
                            nextfollowuptime = null,
                            nextstate = -1,
                            parentstate = -1,
                            state = -1,
                            UpdateTime = DateTime.Now,
                            Source = clueM.source,
                            roletype = item.RoleType,
                            ReciveCarType = 3,//未知
                        });
                        _tXCluesAgentRelationship.Add(new tx_clues_agent_relationship()
                        {
                            AgentId = item.Id,
                            ClueId = clueM.id,
                            TopAgentId = asm.agentid
                        });

                        AcceptanceAgentInfo infoLeader = new AcceptanceAgentInfo();
                        infoLeader.AcceptanceAgentId = item.Id;
                        infoLeader.TopAgentId = clueM.agentid;
                        infoLeader.RoleType = item.RoleType;
                        infoLeader.FollowId = 0;
                        dto.AcceptanceAgentInfoes.Add(infoLeader);
                    }
                }
            }

            foreach (var item in dto.AcceptanceAgentInfoes)
            {
                var setting = _accidentSettingRepository.GetOverNoticeSetting(asm.agentid, item.RoleType);
                if (setting != null)
                {
                    item.CumulativeTimeout = setting.overtime;
                }
            }


            return true;

        }
        tx_sms AnalysisSmsCTxsms(AnalysisSmsRequest request, int agentId)
        {
            tx_sms sms = new tx_sms();
            sms.agentid = agentId;
            sms.smscontent = request.SmsContent;
            sms.mobile = request.Moblie;
            sms.createtime = DateTime.Now;
            sms.casetime = DateTime.Now.Date;
            return sms;
        }



        tx_clues RegAnalysisSms(AnalysisSmsRequest request, int agentId)
        {
            string sc = request.SmsContent;
            sc = sc.Replace("[", "【").Replace("]", "】");
            Regex regSource = new Regex("【(.*?)】");
            Match match = regSource.Match(sc);
            string source = match.Groups[1].Value;

            if (string.IsNullOrEmpty(source))
            {
                if (request.SmsContent.Contains("人保财险"))
                {
                    source = "人保财险";
                }
            }
            tx_clues clue = new tx_clues();
            SMSAHelp sMSAHelp = new SMSAHelp();
            switch (source)
            {
                case "中国平安":
                case "平安产险":
                    clue.source = 0; break;
                case "太平洋保险":
                case "太平洋产险":
                case "中国太平": clue.source = 1;  break;
                case "人保财险":
                case "深圳人保":
                case "人保广州市公司":
                    clue.source = 2;  break;
                case "中国人寿财险":
                    clue.source = 3; 
                    break; 
                case "中华保险":
                    clue.source = 4; 
                    break;
                case "安邦保险":
                    clue.source = 5; 
                    break;

                default: clue.source = -1; break;
            }
            clue.sourcename = source;
            //目前解析的字段：保险公司,案件类型，车牌号，手机号，报案人，报案号，报案时间，品牌型号，事故描述，车架号，事故地点
            clue.ReportCasePeople = string.Empty;
            //兜底
            if (string.IsNullOrEmpty(clue.licenseno))
            {
                Regex regLicenseno = new Regex(@"([京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼翼使领]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1})|([京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼翼使领]{1}[A-Z]{1}[-][A-Z0-9]{4}[A-Z0-9挂学警港澳]{1})|([京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼翼使领]{1}[A-Z]{1}[ ][A-Z0-9]{4}[A-Z0-9挂学警港澳]{1})");
                match = regLicenseno.Match(request.SmsContent);
                clue.licenseno = SerachPurpose(match).Replace("-", "").Replace(" ","").Trim();
            }
            if (string.IsNullOrEmpty(clue.mobile))
            {//兜手机号
                Regex regLicenseno = new Regex(@"(?<!\d)(?:(?:1[358]\d{9})|(?:861[358]\d{9}))(?!\d)");
                match = regLicenseno.Match(request.SmsContent);
                clue.mobile = match.Groups[0].Value;
            }
            if (string.IsNullOrEmpty(clue.CarVIN))
            {//兜车架号

                Regex regCarVIN = new Regex(ConfigurationManager.AppSettings["TXCarVIN"]);
                match = regCarVIN.Match(request.SmsContent);
                clue.CarVIN = match.Groups[2].Value;
            }
            if (string.IsNullOrEmpty(clue.dangerarea))
            {//兜地点
                client = new Nlp(apiKey, cecretKey);
                client.Timeout = 60000;
                var result = client.Lexer(request.SmsContent);
                JObject jresult = JObject.Parse(result.ToString());

                JArray itemData = JArray.Parse(jresult["items"].ToString());

                List<string> addressTxt = new List<string>();

                int loopIndex = 0;
                foreach (var item in itemData)
                {
                    if (item["ne"] != null && item["ne"].ToString() == "PER")
                    {
                        if (string.IsNullOrEmpty(clue.ReportCasePeople))
                        {
                            clue.ReportCasePeople = item["item"].ToString();
                        }
                    }
                    if (item["ne"] != null && item["ne"].ToString() == "LOC")
                    {
                        addressTxt.Add(item["item"].ToString());
                    }
                    if (item["pos"] != null && item["pos"].ToString() == "ns")
                    {
                        addressTxt.Add(item["item"].ToString());
                    }

                    if (item["pos"] != null && item["pos"].ToString() == "f")
                    {
                        if (itemData[loopIndex - 1]["pos"].ToString() == "ns" || (item["ne"] != null && item["ne"].ToString() == "ORG"))
                        {
                            addressTxt.Add(item["item"].ToString());
                        }
                    }

                    if (item["ne"] != null && (item["ne"].ToString() == "ORG"))
                    {
                        if (loopIndex == 0)
                        {
                            continue;
                        }
                        if (itemData[loopIndex - 1]["ne"] != null && (itemData[loopIndex - 1]["ne"].ToString() == "LOC"))
                        {
                            addressTxt.Add(item["item"].ToString());
                        }

                        if (itemData[loopIndex - 1]["pos"] != null && (itemData[loopIndex - 1]["pos"].ToString() == "ns"))
                        {
                            addressTxt.Add(item["item"].ToString());
                        }
                    }

                    loopIndex++;
                }



                foreach (var item in addressTxt)
                {
                    clue.dangerarea += item;
                }
            }
            if (string.IsNullOrEmpty(clue.smsrecivedtime))
            {//兜报案时间
                Regex regReportTime = new Regex(ConfigurationManager.AppSettings["TXReportTime"]);
                match = regReportTime.Match(request.SmsContent);
                clue.smsrecivedtime = match.Groups[0].Value;
            }
            if (string.IsNullOrEmpty(clue.accidentremark))
            {//兜事故描述
                Regex regAccidentremark = new Regex(ConfigurationManager.AppSettings["TXAccidentremark"]);
                match = regAccidentremark.Match(request.SmsContent);
                clue.accidentremark = SerachPurpose(match);
            }
            if (string.IsNullOrEmpty(clue.MoldName))
            {//兜品牌
                Regex regMode = new Regex(ConfigurationManager.AppSettings["TXMoldName"]);
                match = regMode.Match(request.SmsContent);
                clue.MoldName = SerachPurpose(match);
            }
            if (string.IsNullOrEmpty(clue.ReportCaseNum))
            {//兜报案号
                Regex regReportCaseNum = new Regex(ConfigurationManager.AppSettings["TXReportCaseNum"]);
                match = regReportCaseNum.Match(request.SmsContent);

                clue.ReportCaseNum = SerachPurpose(match);
            }
        
           
            clue.agentid = agentId;
            clue.CreateTime = DateTime.Now;
            clue.UpdateTime = DateTime.Now;
            clue.casetype = 0;
            clue.followupstate = -1;
            clue.HasInsureInfo = 0;//初始没有保险
            clue.Deleted = 0;//未删除
            clue.last_follow_id = 0;


            sc = sc.Replace("请回复：0 - 不送修，1 - 送修。", "").Replace("请回复：0-不送修，1-送修，2-不确定", "");
            if (sc.Contains("送修") || sc.Contains("新承保推送"))
            {
                clue.casetype = 1;
            }
            else if (sc.Contains("返修"))
            {
                clue.casetype = 2;
            }
            else if (sc.Contains("三者"))
            {
                clue.casetype = 3;
            }
            return clue;

        }


        private string SerachPurpose(Match match)
        {
            string v = string.Empty;
            for (int i = 0; i < match.Groups.Count; i++)
            {
                if (!string.IsNullOrEmpty(v))
                {
                    break;
                }
                if (!string.IsNullOrEmpty(match.Groups[i].Value.ToString()) && i != 0)
                {
                    v = match.Groups[i].Value.ToString();
                }
            }
            return v;
        }







        /// <summary>
        /// 事故线索App 版本更新比较
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AccidentVersionResponse CompareVersion(string requestType)
        {
            var model = new AccidentVersionResponse();
            model.Data = new AccidentVersionModel();
            try
            {
                var type = 0;
                if (requestType.ToLower().Equals("android"))
                    type = 7;
                else if (requestType.ToLower().Equals("ios"))
                    type = 6;
                else
                {
                    model.Code = -1;
                    model.Message = "类型参数错误";
                    return model;
                }
                //var cachValue_version = CacheProvider.Get<string>(AccidentClueAppVersionKey);
                bx_config configObj = new bx_config();
                //if (cachValue_version == null)
                //{
                configObj = _bxConfigRepository.FindByConfigKey(AccidentClueAppVersion).FirstOrDefault();
                if (configObj == null)
                {
                    model.Code = -1;
                    model.Message = "获取版本信息失败";
                    return model;
                }
                var cachValue_version = configObj.config_value;
                //CacheProvider.Set(AccidentClueAppVersionKey, cachValue_version, 10800);
                // }
                List<AccidentConfigModel> list = CommonHelper.ToListT<AccidentConfigModel>(cachValue_version);
                AccidentConfigModel configModel = list.FirstOrDefault(n => n.Type == type);
                if (configModel == null || configModel.Type < 1)
                {
                    model.Code = -1;
                    model.Message = "获取版本信息失败";
                    return model;
                }
                var versionModel = new AccidentVersionModel();
                var message = _messageRepository.FindById(configModel.MsgId);
                versionModel.Version = configModel.Ver;
                versionModel.UpContent = message == null ? "" : message.Body;
                versionModel.CompulsoryRenewal = configModel.CompulsoryRenewal;
                model.Data = versionModel;
                model.Code = 1;
                model.Message = "获取成功";
                return model;
            }
            catch (Exception ex)
            {
                logError.Info("CompareVersion:发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                model.Code = -1;
                model.Message = "获取失败";
                return model;
            }
        }

        /// <summary>
        /// 事故线索app 服务版本信息
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        public AccidentVersionResponse CompareClueServiceVersion(string requestType)
        {
            var model = new AccidentVersionResponse();
            model.Data = new AccidentVersionModel();
            try
            {
                var type = 0;
                if (requestType.ToLower().Equals("android"))
                    type = 7;
                else
                {
                    model.Code = -1;
                    model.Message = "类型参数错误";
                    return model;
                }
                bx_config configObj = new bx_config();
                configObj = _bxConfigRepository.FindByConfigKey(AccidentClueServiceAppVersion).FirstOrDefault();
                if (configObj == null)
                {
                    model.Code = -1;
                    model.Message = "获取版本信息失败";
                    return model;
                }
                var cachValue_version = configObj.config_value;
                List<AccidentConfigModel> list = CommonHelper.ToListT<AccidentConfigModel>(cachValue_version);
                AccidentConfigModel configModel = list.FirstOrDefault(n => n.Type == type);
                if (configModel == null || configModel.Type < 1)
                {
                    model.Code = -1;
                    model.Message = "获取版本信息失败";
                    return model;
                }
                var versionModel = new AccidentVersionModel();
                var message = _messageRepository.FindById(configModel.MsgId);
                versionModel.Version = configModel.Ver;
                versionModel.UpContent = message == null ? "" : message.Body;
                versionModel.CompulsoryRenewal = configModel.CompulsoryRenewal;
                model.Data = versionModel;
                model.Code = 1;
                model.Message = "获取成功";
                return model;
            }
            catch (Exception ex)
            {
                logError.Info("CompareVersion:发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                model.Code = -1;
                model.Message = "获取失败";
                return model;
            }
        }

        /// <summary>
        /// 修改/添加是否需要版本更新的设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool EditIsuploadByKey(RequestKeyConfig request)
        {
            bool result = false;
            try
            {
                var exit = _bxConfigRepository.FindByConfigKey(AccidentClueAppIsUpload).FirstOrDefault();
                if (exit != null && exit.id > 0)
                {
                    result = _bxConfigRepository.UpdateByConfigKey_Isupload(request);
                    CacheProvider.Remove(AccidentClueAppUploadKey);
                }
                else
                {
                    var model = new bx_config
                    {
                        config_key = request.ConfigKey,
                        config_value = request.KeyValue,
                        config_type = 1,
                        is_delete = 0,
                        create_time = DateTime.Now
                    };
                    var add = _bxConfigRepository.Add(model);
                    if (add > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 事故线索版本更新
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int EditVersion(AccidentEditVersionConfig request)
        {
            int result = 0;
            try
            {
                var configObj = _bxConfigRepository.FindByConfigKey(AccidentClueAppVersion).FirstOrDefault();
                string oldConfigValue = configObj == null ? "" : configObj.config_value;
                if (configObj == null || configObj.id < 1)
                {
                    configObj = new bx_config();
                    var config = new List<AccidentConfigModel>();
                    config.Add(new AccidentConfigModel { Ver = "0.0.0", Type = request.Type, MsgId = 0, CompulsoryRenewal = 0 });
                    config.Add(new AccidentConfigModel { Ver = "0.0.0", Type = request.Type == 6 ? 7 : 6, MsgId = 0, CompulsoryRenewal = 0 });
                    oldConfigValue = CommonHelper.TToString<List<AccidentConfigModel>>(config);
                    configObj.config_key = AccidentClueAppVersion;
                    configObj.config_value = oldConfigValue;
                    configObj.config_type = 1;
                    configObj.is_delete = 0;
                    configObj.create_time = DateTime.Now;
                    _bxConfigRepository.Add(configObj);
                }

                List<AccidentConfigModel> lst = CommonHelper.ToListT<AccidentConfigModel>(oldConfigValue);
                AccidentConfigModel mo = lst.Where(n => n.Type == request.Type).FirstOrDefault();

                request.UpContent = request.UpContent.Replace("\\n", "\r\n");

                if (mo.Ver != request.Version)
                {
                    bx_message bxMessage = new bx_message()
                    {
                        Title = "升级通知",
                        Body = request.UpContent,
                        Msg_Type = 8,
                        Create_Time = DateTime.Now,
                        Update_Time = DateTime.Now,
                        Msg_Status = 1,
                        Msg_Level = 1,
                        Send_Time = DateTime.Now,
                        Create_Agent_Id = 2668,
                        Agent_Id = 2668,
                        MsgStatus = "1"
                    };

                    int msgId = _messageRepository.Add(bxMessage);

                    List<AccidentConfigModel> listConfig = new List<AccidentConfigModel>();
                    foreach (var item in lst)
                    {
                        AccidentConfigModel mm = new AccidentConfigModel();
                        mm.Type = item.Type;
                        mm.Ver = item.Type == request.Type ? request.Version : item.Ver;
                        mm.MsgId = item.Type == request.Type ? msgId : item.MsgId;
                        mm.CompulsoryRenewal = item.Type == request.Type ? request.CompulsoryRenewal : item.CompulsoryRenewal;
                        listConfig.Add(mm);
                    }
                    configObj.config_value = CommonHelper.TToString<List<AccidentConfigModel>>(listConfig);
                    result = _bxConfigRepository.Update(configObj);

                    if (result > 0)
                    {
                        CacheProvider.Remove(AccidentClueAppVersionKey);
                    }
                }
                else
                {
                    result = 1;
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 事故线索app 服务版本更新
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int EditClueServiceVersion(AccidentEditVersionConfig request)
        {
            int result = 0;
            try
            {
                var configObj = _bxConfigRepository.FindByConfigKey(AccidentClueServiceAppVersion).FirstOrDefault();
                string oldConfigValue = configObj == null ? "" : configObj.config_value;
                if (configObj == null || configObj.id < 1)
                {
                    configObj = new bx_config();
                    var config = new List<AccidentConfigModel>();
                    config.Add(new AccidentConfigModel { Ver = "1.0.0", Type = request.Type, MsgId = 0, CompulsoryRenewal = 0 });
                    oldConfigValue = CommonHelper.TToString<List<AccidentConfigModel>>(config);
                    configObj.config_key = AccidentClueServiceAppVersion;
                    configObj.config_value = oldConfigValue;
                    configObj.config_type = 1;
                    configObj.is_delete = 0;
                    configObj.create_time = DateTime.Now;
                    _bxConfigRepository.Add(configObj);
                }

                List<AccidentConfigModel> lst = CommonHelper.ToListT<AccidentConfigModel>(oldConfigValue);
                AccidentConfigModel mo = lst.Where(n => n.Type == request.Type).FirstOrDefault();

                request.UpContent = request.UpContent.Replace("\\n", "\r\n");

                if (mo.Ver != request.Version)
                {
                    bx_message bxMessage = new bx_message()
                    {
                        Title = "升级通知",
                        Body = request.UpContent,
                        Msg_Type = 8,
                        Create_Time = DateTime.Now,
                        Update_Time = DateTime.Now,
                        Msg_Status = 1,
                        Msg_Level = 1,
                        Send_Time = DateTime.Now,
                        Create_Agent_Id = 2668,
                        Agent_Id = 2668,
                        MsgStatus = "1"
                    };

                    int msgId = _messageRepository.Add(bxMessage);

                    List<AccidentConfigModel> listConfig = new List<AccidentConfigModel>();
                    foreach (var item in lst)
                    {
                        AccidentConfigModel mm = new AccidentConfigModel();
                        mm.Type = item.Type;
                        mm.Ver = item.Type == request.Type ? request.Version : item.Ver;
                        mm.MsgId = item.Type == request.Type ? msgId : item.MsgId;
                        mm.CompulsoryRenewal = item.Type == request.Type ? request.CompulsoryRenewal : item.CompulsoryRenewal;
                        listConfig.Add(mm);
                    }
                    configObj.config_value = CommonHelper.TToString<List<AccidentConfigModel>>(listConfig);
                    result = _bxConfigRepository.Update(configObj);
                }
                else
                {
                    result = 1;
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        public AccidentClueTotalResponse GetTotalCount(AccidentTotalRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var result = new AccidentClueTotalResponse();
            result.Data = new AccidentClueTotalModel();
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.TopAgentId,
                SecCode = request.SecCode,
                BhToken = request.Token,
                ChildAgent = request.AgentId
            };
            var exitModel = _appVerifyService.Verify(baseRequest, pairs);
            if (exitModel.ErrCode != 1)
            {
                result.Code = (exitModel.ErrCode == -13000 || exitModel.ErrCode == -13020) ? -13000 : -1;
                result.Message = exitModel.ErrMsg;
                return result;
            }
            result.Data = _accidentRepository.GetTotalCount(request.TimeType, request.AgentId, request.TopAgentId, request.RoleType);
            if (result.Data == null)
            {
                result.Code = -1;
                result.Message = "获取失败";
            }
            else
            {
                result.Code = 1;
                result.Message = "获取成功";
            }
            return result;
        }

        public ClueStatisticalViewModel ClueStatistical(int agentId, int topAgentId, DateTime startTime, DateTime endTime, int roleType)
        {
            endTime = endTime.AddDays(1);
            var clueStatisticalViewModel = _accidentRepository.GetClueStatisticalViewModel(roleType == 7 || roleType == 3 ? topAgentId : agentId, startTime, endTime, roleType);
            clueStatisticalViewModel.TotalCount = clueStatisticalViewModel.UnhandleCount + clueStatisticalViewModel.ReachDealersCount +
                                                  clueStatisticalViewModel.FollowUpCount + clueStatisticalViewModel.LossCount +
                                                  clueStatisticalViewModel.MaintainCount + clueStatisticalViewModel.HandOverCount;
            List<int> GrabOrderPeopleIds = _accidentOrderService.GetGrabOrderPeoples(topAgentId).Select(x => x.AgentId).ToList();
            if (roleType == 7 || roleType == 3)
            {
                clueStatisticalViewModel.ClueStatisticalWithCompanies = _accidentRepository.GetClueStatisticalWithCompany(topAgentId, startTime, endTime).OrderByDescending(x => x.CompanyId).ToList();
                clueStatisticalViewModel.LossStatisticals = _accidentRepository.GetLossStatistical(topAgentId, startTime, endTime);
                clueStatisticalViewModel.ClueResponsivities = _accidentRepository.GetClueResponsivity(topAgentId, startTime, endTime);
            }
            if (roleType == 7 || roleType == 3 || GrabOrderPeopleIds.Contains(agentId))
            {
                clueStatisticalViewModel.OrderCount = _accidentRepository.ClueOrderStatistical(topAgentId, startTime, endTime);
            }
            else
            {
                clueStatisticalViewModel.ClueStatisticalWithCompanies = new List<ClueStatisticalWithCompany>();
                clueStatisticalViewModel.LossStatisticals = new List<LossStatistical>();
                clueStatisticalViewModel.ClueResponsivities = new List<ClueResponsivity>();
                clueStatisticalViewModel.OrderCount = new ClueOrderStatistical();
            }
            return clueStatisticalViewModel;
        }

        /// <summary>
        /// 保存极光账号
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public AccidentJGResponse SaveJgAccount(AccidentJGRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var result = new AccidentJGResponse();
            result.Data = new { };
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.TopAgentId,
                SecCode = request.SecCode,
                BhToken = request.Token,
                ChildAgent = request.AgentId
            };
            var exitModel = _appVerifyService.Verify(baseRequest, pairs);
            if (exitModel.ErrCode != 1)
            {
                result.Code = (exitModel.ErrCode == -13000 || exitModel.ErrCode == -13020) ? -13000 : -1;
                result.Message = exitModel.ErrMsg;
                return result;
            }
            var type = 0;
            if (request.LoginType.ToLower().Equals("android"))
                type = 3;
            else if (request.LoginType.ToLower().Equals("ios"))
                type = 4;
            else
            {
                result.Code = -1;
                result.Message = "请求类型不正确";
                return result;
            }
            var saveResult = _messageAppRepository.SaveAgent_XGAccount_RelationShip(new bx_agent_xgaccount_relationship() { Account = request.Account, AgentId = request.AgentId, CreateTime = DateTime.Now, Deleted = false, UpdateTime = DateTime.Now, DeviceType = type });
            if (saveResult > 0)
            {
                result.Code = 1;
                result.Message = "保存成功";
            }
            else
            {
                result.Code = -1;
                result.Message = "保存失败";
            }
            return result;
        }

        public AccidentClueModel GetClusDetails(int clueId)
        {
            var viewModel = _accidentRepository.GetClusDetails(clueId);
            viewModel.CluesImage = _accidentRepository.GetClusImage(clueId);
            return viewModel;
        }

        public List<AccidentFollowRecordVM> GetFollowUpRecords(int clueId)
        {
            var records = _accidentRepository.GetFollowUpRecords(clueId).GroupBy(x => Convert.ToDateTime(x.CreateTime).ToString("yyyy-MM-dd"));
            var result = new List<AccidentFollowRecordVM>();
            foreach (IGrouping<string, AccidentFollowRecord> info in records)
            {
                result.Add(new AccidentFollowRecordVM { CreateDate = info.Key, Data = info.ToList<AccidentFollowRecord>() });
            }
            return result;
        }


        public GetClusDetailsResponse GetClusDetails(GetClusDetailsRequest request)
        {
            GetClusDetailsResponse response = new GetClusDetailsResponse();
            response.Data = _accidentRepository.GetClusDetail(request.clueId);
            if (response.Data != null)
            {
                TimeSpan span = DateTime.Now - response.Data.ClueTime;
                response.Data.TimeDifference = span.TotalSeconds;
                response.Data.UpdateTime = response.Data.ClueTime.ToString("yyyy-MM-dd HH:mm:ss");
                response.Data.MoldName = response.Data.MoldName == null ? "" : response.Data.MoldName;
                response.Data.LicenseNo = response.Data.LicenseNo == null ? "" : response.Data.LicenseNo;
                response.Data.SourceName = response.Data.SourceName == null ? "" : response.Data.SourceName;
                response.Data.ReportCaseNum = response.Data.ReportCaseNum == null ? "" : response.Data.ReportCaseNum;
                response.Data.ReportCasePeople = response.Data.ReportCasePeople == null ? "" : response.Data.ReportCasePeople;
                response.Data.LastFollowAgent = response.Data.LastFollowAgent == null ? "" : response.Data.LastFollowAgent;
                response.Data.LastFollowTime = response.Data.LastFollowTime == null ? "" : response.Data.LastFollowTime;
            }
            else
            {
                response.Data = new AccidentClueModel();
            }
            return response;
        }






        public AccidentFollowRecordResponse GetFollowReport(GetFollowReportRequest request)
        {
            AccidentFollowRecordResponse response = new AccidentFollowRecordResponse();
            response.Data = new AccidentFollowRecordModel();
            response.Data.list = _accidentRepository.GetFollowUpRecords(request.clueId);
            return response;
        }

        /// <summary>
        /// 获取短信模板
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public AccidentSmsTempResponse GetSmsTempList(AccidentSmsTemplateRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var result = new AccidentSmsTempResponse();
            result.Data = new AccidentSmsListModel();
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.TopAgentId,
                SecCode = request.SecCode,
                BhToken = request.Token,
                ChildAgent = request.AgentId
            };
            var exitModel = _appVerifyService.Verify(baseRequest, pairs);
            if (exitModel.ErrCode != 1)
            {
                result.Code = (exitModel.ErrCode == -13000 || exitModel.ErrCode == -13020) ? -13000 : -1;
                result.Message = exitModel.ErrMsg;
                return result;
            }
            var model = new AccidentSmsListModel();
            model.SmsTemplateList = _accidentRepository.GetTempList(request.TopAgentId);
            result.Code = 1;
            result.Data = model;
            if (model.SmsTemplateList == null || model.SmsTemplateList.Count == 0)
            {
                result.Message = "暂无数据";
            }
            else
            {
                result.Message = "获取成功";
            }
            return result;
        }

        /// <summary>
        /// 发送短信、录入跟进
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public AccidentSmsSendResponse SendSms(AccidentSendMessageRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var result = new AccidentSmsSendResponse();
            result.Data = new { };
            try
            {
                var baseRequest = new AppBaseRequest()
                {
                    Agent = request.TopAgentId,
                    SecCode = request.SecCode,
                    BhToken = request.Token,
                    ChildAgent = request.AgentId
                };
                var exitModel = _appVerifyService.Verify(baseRequest, pairs);
                if (exitModel.ErrCode != 1)
                {
                    result.Code = (exitModel.ErrCode == -13000 || exitModel.ErrCode == -13020) ? -13000 : -1;
                    result.Message = exitModel.ErrMsg;
                    return result;
                }

                var sendResult = SendAccidentSms(request.TopAgentId, request.AgentId, request.Phone, request.SmsContent);
                //短信发送成功
                if (sendResult.Code == 1)
                {
                    var model = new AccidentClueFollowUpRequest
                    {
                        TopAgentId = request.TopAgentId,
                        AgentId = request.AgentId,
                        Token = request.Token,
                        SecCode = request.SecCode,
                        ClueId = request.ClueId,
                        FollowUpState = 5,
                        SmsContent = request.SmsContent
                    };
                    InputFollowUp(model, pairs);
                }
                result.Code = sendResult.Code > 0 ? 1 : -1;
                result.Message = sendResult.Message;
            }
            catch (Exception)
            {
                result.Code = -1;
                result.Message = "发送失败";
            }
            return result;
        }

        /// <summary>
        /// 推修短信 新方法
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <param name="phone"></param>
        /// <param name="smsContent"></param>
        /// <returns></returns>
        public SendSmsResultModel SendAccidentSms(int topAgentId, int agentId, string phone, string smsContent)
        {
            var result = new SendSmsResultModel();
            try
            {
                if (string.IsNullOrWhiteSpace(smsContent))
                {
                    result.Code = -1;
                    result.Message = "短信内容不能为空";
                    return result;
                }
                if (string.IsNullOrWhiteSpace(phone))
                {
                    result.Code = -1;
                    result.Message = "手机号不能为空";
                    return result;
                }
                int agentSelfId = agentId;
                bx_agent bxAgent = _agentService.GetAgent(agentSelfId);
                if (bxAgent == null)
                {
                    result.Code = -1;
                    result.Message = "代理人信息不存在";
                    return result;
                }
                if (bxAgent.ParentAgent != 0)
                {
                    if (bxAgent.MessagePayType == 0)
                        agentSelfId = int.Parse(_consumerDetailService.GetTopAgent(agentSelfId));
                    else if (bxAgent.MessagePayType == 2)
                        agentSelfId = _agentService.GetAgent(bxAgent.ParentAgent).Id;//MessagePayType==2 三级代理 从父级扣费
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
                    result.Message = "短信余额不足，请前往微信代理人中心充值后重试！";
                    result.Code = -1;
                    return result;
                }
                if (smsAccountInfo.status == 0)
                {
                    result.Message = "短信账号不可用！";
                    result.Code = -1;
                    return result;
                }
                if (smsAccountInfo.avail_times < _smsBulkSendManageService.SDmessageCount(smsContent, 0))
                {
                    result.Message = "短信余额不足，请充值后重试！";
                    result.Code = -1;
                    return result;
                }
                var sendModel = new SmsRequest
                {
                    Agent = topAgentId,
                    CurAgent = agentId,
                    BusinessType = Models.ViewModels.Enum.EnumSmsBusinessType.Login,
                    Mobile = phone,
                    SmsContent = smsContent,
                    SecCode = "",
                    LicenseNo = "",
                    SentType = 1
                };
                var sendResult = _smsService.SendSms(sendModel, null);
                if (sendResult != null && sendResult.BusinessStatus == 1)
                {
                    var smsCount = _smsBulkSendManageService.UpdateSmsAccountUseCount(agentSelfId, _smsBulkSendManageService.SDmessageCount(smsContent, 0), 2);
                    if (smsCount)
                    {
                        result.Code = 1;
                        result.Message = "短信发送成功";
                        return result;
                    }
                    result.Code = -1;
                    result.Message = "更新短信条数失败";
                    return result;
                }
                result.Message = "短信发送失败";
                result.Code = -1;
                return result;
            }
            catch (Exception)
            {
                result.Code = -1;
                result.Message = "发送短信失败";
                return result;
            }
        }

        public AccidentClueStatesResponse GetFollowUpStates(AccidentSmsTemplateRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var result = new AccidentClueStatesResponse();
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.TopAgentId,
                SecCode = request.SecCode,
                BhToken = request.Token,
                ChildAgent = request.AgentId
            };
            var exitModel = _appVerifyService.Verify(baseRequest, pairs);
            if (exitModel.ErrCode != 1)
            {
                result.Code = -1;
                result.Message = exitModel.ErrMsg;
                result.Data = new List<ClueState>();
                return result;
            }
            result.Code = 1;
            result.Message = "获取成功";
            result.Data = _accidentRepository.GetFollowUpStates(request.TopAgentId);
            return result;
        }

        public AccidentClueLossReasonsResponse GetLossReasons(AccidentSmsTemplateRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var result = new AccidentClueLossReasonsResponse();
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.TopAgentId,
                SecCode = request.SecCode,
                BhToken = request.Token,
                ChildAgent = request.AgentId
            };
            var exitModel = _appVerifyService.Verify(baseRequest, pairs);
            if (exitModel.ErrCode != 1)
            {
                result.Code = -1;
                result.Message = exitModel.ErrMsg;
                result.Data = new List<ClueLossReason>();
                return result;
            }
            result.Code = 1;
            result.Message = "获取成功";
            result.Data = _accidentRepository.GetLossReasons(request.TopAgentId);
            return result;
        }

        public AccidentSmsSendResponse InputFollowUp(AccidentClueFollowUpRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var result = new AccidentSmsSendResponse();
            result.Data = new { };
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.TopAgentId,
                SecCode = request.SecCode,
                BhToken = request.Token,
                ChildAgent = request.AgentId
            };
            var exitModel = _appVerifyService.Verify(baseRequest, pairs);
            if (exitModel.ErrCode != 1)
            {
                result.Code = -1;
                result.Message = exitModel.ErrMsg;
                return result;
            }
            var exit = 0;
            var followModel = new tx_cluefollowuprecord
            {
                clueid = request.ClueId,
                fromagentid = request.AgentId,
                toagentid = request.ReciveCaragentid <= 0 ? request.AgentId : request.ReciveCaragentid,
                state = request.FollowUpState,
                remark = request.Remark,
                parentstate = request.FollowUpState == 2 || request.FollowUpState == 5 || request.FollowUpState == 6 ? 1 : request.FollowUpState,
                nextfollowuptime = request.NextFollowUpTime,
                ReciveCarType = request.ReciveCarType,
                ReciveCarAea = request.ReciveCarAea,
                ReciveCaragentid = request.ReciveCaragentid,
                ArrivalType = request.ArrivalType,
                SmsContent = request.SmsContent,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Deleted = 0,
                loss_reason_id = request.LossReasonId,
                nextstate = -1
            };
            var clueAgentRelationship = new tx_clues_agent_relationship
            {
                AgentId = (request.FollowUpState == 2 && request.ReciveCaragentid > 0) ? request.ReciveCaragentid : request.AgentId,
                TopAgentId = request.TopAgentId,
                ClueId = request.ClueId,
                ParentId = 0
            };
            exit = _tXClueFollowRecord.Add(followModel);

            if (exit > 0)
            {
                _accidentRepository.UpdateClue(request.FollowUpState, request.ClueId, exit); //更新线索状态
                _tXClueFollowRecord.AddClueAgentRelationship(clueAgentRelationship); //线索与代理人关系表添加数据
                _tXClueFollowRecord.UpdatePrevFollowUpRecord(request.ClueId, request.AgentId, exit, request.FollowUpState); //更新上一条跟进记录的NextState
                var clue = _accidentRepository.GetClusDetails(request.ClueId);
                ClueNotificationDto clueNotificationDto = new ClueNotificationDto();
                clueNotificationDto.ClueId = request.ClueId;
                clueNotificationDto.Id = exit;
                clueNotificationDto.Licenseno = clue.LicenseNo;
                clueNotificationDto.MoldName = clue.MoldName;
                clueNotificationDto.ClueState = request.FollowUpState;
                clueNotificationDto.OprateAgentId = request.AgentId;
                if (request.FollowUpState == 2) //上门接车通知(即时)
                {
                    clueNotificationDto.MessageType = 3;
                    clueNotificationDto.AcceptanceAgentInfoes = new List<AcceptanceAgentInfo>() { new AcceptanceAgentInfo { TopAgentId = request.TopAgentId, AcceptanceAgentId = request.ReciveCaragentid, FollowId = exit } };
                    _accidentMqMessageService.ImmediatePushClueNotification(clueNotificationDto);
                }
                else if (request.FollowUpState == 1) //回访提醒(延时)
                {
                    clueNotificationDto.MessageType = 2;
                    clueNotificationDto.AcceptanceAgentInfoes = new List<AcceptanceAgentInfo>() { new AcceptanceAgentInfo { TopAgentId = request.TopAgentId, AcceptanceAgentId = request.AgentId, FollowId = exit, CumulativeTimeout = Convert.ToInt32((request.NextFollowUpTime - DateTime.Now).TotalMinutes) } };
                    _accidentMqMessageService.DelayPushClueNotification(clueNotificationDto);
                }
                else if (request.FollowUpState == 4) //流失通知(即时)
                {
                    clueNotificationDto.MessageType = 4;
                    var managerIds = _accidentRepository.GetManagerIdByTopAgentId(request.TopAgentId);
                    clueNotificationDto.AcceptanceAgentInfoes = new List<AcceptanceAgentInfo>() { new AcceptanceAgentInfo { TopAgentId = request.TopAgentId, AcceptanceAgentId = request.TopAgentId, FollowId = exit } };
                    foreach (int id in managerIds)
                    {
                        clueNotificationDto.AcceptanceAgentInfoes.Add(new AcceptanceAgentInfo { TopAgentId = request.TopAgentId, AcceptanceAgentId = id, FollowId = exit });
                    }
                    _accidentMqMessageService.ImmediatePushClueNotification(clueNotificationDto);
                }
                else if (request.FollowUpState == 3) //车辆到店通知(即时)
                {
                    clueNotificationDto.MessageType = 5;
                    var managerIds = _accidentRepository.GetManagerIdByTopAgentId(request.TopAgentId);
                    clueNotificationDto.AcceptanceAgentInfoes = new List<AcceptanceAgentInfo>() { new AcceptanceAgentInfo { TopAgentId = request.TopAgentId, AcceptanceAgentId = request.TopAgentId, FollowId = exit } };
                    foreach (int id in managerIds)
                    {
                        clueNotificationDto.AcceptanceAgentInfoes.Add(new AcceptanceAgentInfo { TopAgentId = request.TopAgentId, AcceptanceAgentId = id, FollowId = exit });
                    }
                    _accidentMqMessageService.ImmediatePushClueNotification(clueNotificationDto);
                }
            }
            result.Code = exit > 0 ? 1 : -1;
            result.Message = exit > 0 ? "录入成功" : "录入失败";
            return result;
        }

        public RecivesCarPeopleResponse GetRecivesCarPeoples(AccidentSmsTemplateRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var result = new RecivesCarPeopleResponse();
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.TopAgentId,
                SecCode = request.SecCode,
                BhToken = request.Token,
                ChildAgent = request.AgentId
            };
            var exitModel = _appVerifyService.Verify(baseRequest, pairs);
            if (exitModel.ErrCode != 1)
            {
                result.Code = -1;
                result.Message = exitModel.ErrMsg;
                result.Data = new List<RecivesCarPeople>();
                return result;
            }
            result.Code = 1;
            result.Message = "获取成功";
            result.Data = _accidentRepository.GetRecivesCarPeoples(request.TopAgentId);
            return result;
        }

        public AccidentRenewalInfo GetReInfo(int clueId)
        {
            var reInfo = _accidentRepository.GetReInfo(clueId);
            if (reInfo == null)
            {
                return new AccidentRenewalInfo();
            }
            var reInfoVM = JsonConvert.DeserializeObject<AccidentRenewalInfo>(reInfo);
            if (!string.IsNullOrEmpty(reInfoVM.Buid))
            {
                var preRenwal = _renewalInfoRepository.GetCarRenwalInfo(long.Parse(reInfoVM.Buid));
                reInfoVM.PreRenewal = preRenwal;
            }
            return reInfoVM;
        }


        public bool ExistMobileAgentRelationship(string mobile)
        {
            return _accidentSettingRepository.GetPhoneSetting(mobile) != null;
        }

        public bool ExistMobileAgentRelationship(string mobile, string mobileCode)
        {
            return _accidentSettingRepository.GetPhoneSetting(mobile, mobileCode) != null;
        }

        /// <summary>
        /// 事故线索退出app登录
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool SignOut(string agentId)
        {
            return CacheProvider.Remove("Accident_Login_Agent_" + agentId) != null;
        }
        /// <summary>
        /// 添加或修改推修手机运行状态
        /// </summary>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public AccidentMoblieServiceResponse MobileServiceAddOrUpdate(AccidentMoblieServiceStateRequest request, string url)
        {
            var result = new AccidentMoblieServiceResponse();
            /*根据app反馈，在手机号为空的时候，直接返回成功*/
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                result.Code = 1;
                result.Message = "保存成功";
                return result;
            }
            var dic = new Dictionary<string, string>();
            dic.Add("BatteryCapacity", request.BatteryCapacity);
            dic.Add("CustKey", request.CustKey);
            dic.Add("IsAvailable", request.IsAvailable.ToString());
            dic.Add("IsConnectSupply", request.IsConnectSupply.ToString());
            dic.Add("NetWorkType", request.NetWorkType.ToString());
            dic.Add("PhoneNumber", request.PhoneNumber);
            Dictionary<string, string> ascdic = dic.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value.ToString());
            var parms = new StringBuilder();
            foreach (var item in ascdic)
            {
                if (parms.Length == 0)
                    parms.Append(item.Key + "=" + item.Value);
                else
                    parms.Append("&" + item.Key + "=" + item.Value);
            }
            var secCode = (url + parms.ToString().GetUrl()).GetMd5();
            if (secCode == request.SecCode)
            {
                var dataModel = new tx_mobileservicestatus
                {
                    BatteryCapacity = request.BatteryCapacity,
                    CreateTime = DateTime.Now,
                    CustKey = request.CustKey,
                    IsAvailable = request.IsAvailable,
                    IsConnectSupply = request.IsConnectSupply,
                    NetWorkType = request.NetWorkType,
                    PhoneNumber = request.PhoneNumber,
                    UpdateTime = DateTime.Now
                };
                var id = _accidentRepository.MobileServiceAddOrUpdate(dataModel);
                result.Code = id > 0 ? 1 : -1;
                result.Message = id > 0 ? "保存成功" : "保存失败";
            }
            else
            {
                result.Code = -1;
                result.Message = "校验失败";
            }
            return result;
        }

        /// <summary>
        /// 添加或修改门店信息
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public int InsertAddressModel(int topAgentId, string address)
        {
            return _accidentRepository.InsertAddressModel(topAgentId, address);
        }


        public tx_storeaddress GetAddressModel(int topAentId)
        {
            return _accidentRepository.GetAddressModel(topAentId);
        }

        public ClueOrderStatistical ClueOrderStatistical(int agentId, int topAgentId, DateTime startTime, DateTime endTime)
        {
            endTime = endTime.AddDays(1);
            var viewModel = _accidentRepository.ClueOrderStatistical(topAgentId, startTime, endTime);
            return viewModel;
        }
    }
}
