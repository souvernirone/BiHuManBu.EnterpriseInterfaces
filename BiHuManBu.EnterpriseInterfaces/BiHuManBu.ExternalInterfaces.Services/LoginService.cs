using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using ServiceStack.Text;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using System.Configuration;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Enum;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Models.Enums;
using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using Newtonsoft.Json;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class LoginService : CommonBehaviorService, ILoginService
    {
        private static readonly string _userCenter = System.Configuration.ConfigurationManager.AppSettings["UserCenter"];

        private ILog logError;
        private ILog logInfo;
        private ICacheHelper _cacheHelper;
        private ILoginRepository _loginRepository;
        private IAgentRepository _agentRepository;
        private IConsumerDetailRepository _consumerDetailRepository;
        private IUserInfoRepository _iUserInfoRepository;
        private IZuoXiRepository zuoXiRepository;
        private ILoginLogRepository _loginLogRepository;
        private IAgentService _agentService;
        private IManagerModuleRepository _managerModuleRepository;
        private IManagerModuleButtonRepository _managerModuleButtonRepository;
        private IAuthorityService _authorityService;
        private ICustomerStatusService _customerStatusService;
        private IUpdateCompleteTaskService _updateCompleteTaskService;
        private IAgentUkeyRepository _agentUkeyRepository;
        private IManagerRoleService _managerRoleService;
        private IGroupAuthenService _groupAuthenService;
        private string zcTopAgentId = ConfigurationManager.AppSettings["autoOpenUsedId"].ToString();
        private string vmCanLoginPcTopAgentId = ConfigurationManager.AppSettings["VmCanLoginPcTopAgentId"].ToString();

        public LoginService(
            ILoginRepository loginRepository,
            ICacheHelper cacheHelper,
            IAgentRepository agentRepository,
            IConsumerDetailRepository consumerDetailRepository,
            IUserInfoRepository iUserInfoRepository,
            IZuoXiRepository zuoXiRepository,
            IAgentService agentService,
            ILoginLogRepository loginLogRepository,
            IManagerModuleRepository managerModuleRepository,
            IManagerModuleButtonRepository managerModuleButtonRepository,
            IAuthorityService authorityService,
            ICustomerStatusService customerStatusService,
            IUpdateCompleteTaskService updateCompleteTaskService,
            IAgentUkeyRepository agentUkeyRepository,
            IManagerRoleService managerRoleService,
            IGroupAuthenService groupAuthenService
            )
            : base(agentRepository, cacheHelper)
        {
            _cacheHelper = cacheHelper;
            _loginRepository = loginRepository;
            _agentRepository = agentRepository;
            _consumerDetailRepository = consumerDetailRepository;
            _iUserInfoRepository = iUserInfoRepository;
            logError = LogManager.GetLogger("ERROR");
            logInfo = LogManager.GetLogger("INFO");
            this.zuoXiRepository = zuoXiRepository;
            _agentService = agentService;
            _loginLogRepository = loginLogRepository;
            _managerModuleRepository = managerModuleRepository;
            _managerModuleButtonRepository = managerModuleButtonRepository;
            _authorityService = authorityService;
            _customerStatusService = customerStatusService;
            _updateCompleteTaskService = updateCompleteTaskService;
            _agentUkeyRepository = agentUkeyRepository;
            _managerRoleService = managerRoleService;
            _groupAuthenService = groupAuthenService;
        }

        public async Task<Account> LoginAccount(string mobile)
        {
            const int cacheTime = 360;
            string accountCacheKey = string.Format("account_cache_key_{0}", mobile);
            string accountCacheTokenKey = string.Format("{0}_{1}__", accountCacheKey, "token");
            var accountCacheToken = _cacheHelper.Get(accountCacheTokenKey);
            var account = _cacheHelper.Get(accountCacheKey) as Account;

            if (accountCacheToken != null)
            {
                return account;
            }
            lock (accountCacheTokenKey)
            {
                accountCacheToken = _cacheHelper.Get(accountCacheTokenKey);
                if (accountCacheToken != null)
                {
                    return account;
                }
                _cacheHelper.Add(accountCacheTokenKey, "1", cacheTime);

                ThreadPool.QueueUserWorkItem(async (arg) =>
                {
                    using (
               HttpClient client =
                   new HttpClient(new HttpClientHandler
                   {
                       AutomaticDecompression =
                           System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                   }))
                    {

                        client.BaseAddress = new Uri(_userCenter);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var getLoginUrl = string.Format("api/Account/GetWapLogin?mobile={0}&code={1}", mobile, "");
                        HttpResponseMessage responseLogin = await client.GetAsync(getLoginUrl);
                        if (responseLogin.IsSuccessStatusCode)
                        {
                            var resultLogin = await responseLogin.Content.ReadAsStringAsync();
                            account = resultLogin.FromJson<Account>();
                            _cacheHelper.Add(accountCacheKey, account, cacheTime * 2);
                        }
                        else
                        {
                            string loginErrorMsg = string.Format("手机号登录失败:" + mobile);
                            logError.Info(loginErrorMsg);
                        }
                    }
                });
            }

            if (account == null)
            {
                using (
              HttpClient client =
                  new HttpClient(new HttpClientHandler
                  {
                      AutomaticDecompression =
                          System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                  }))
                {

                    client.BaseAddress = new Uri(_userCenter);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var getLoginUrl = string.Format("api/Account/GetWapLogin?mobile={0}&code={1}", mobile, "");
                    HttpResponseMessage responseLogin = await client.GetAsync(getLoginUrl);
                    if (responseLogin.IsSuccessStatusCode)
                    {
                        var resultLogin = await responseLogin.Content.ReadAsStringAsync();
                        account = resultLogin.FromJson<Account>();
                    }
                    else
                    {
                        string loginErrorMsg = string.Format("手机号登录失败:" + mobile);
                        logError.Info(loginErrorMsg);
                    }
                }
            }


            return account;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">加密后的密码</param>
        /// <param name="uniqueIdentifier">客户端唯一标识</param>
        /// <param name="FromMethod">登陆来源1PC 2微信 4APP</param>
        /// <param name="GroupId">集团Id,非集团下属账号不能登录</param>
        /// <param name="agentId">敏梅客户的App客户 登录的时候带的值 用来判断只能是敏梅的代理人登录 敏梅的app</param>
        /// <param name="isWChat">0不是微信登录，1是微信登录,2是APP</param>
        /// <param name="topAgentId">微信登录代理人判断是否在顶级下面</param>
        /// <param name="checkPwd"></param>
        /// <returns></returns>
        public GetLoginViewModel Login(string name, string pwd, string uniqueIdentifier, int FromMethod, int agentId = 0, int isWChat = 0, int topAgentId = 0, bool checkPwd = true, int GroupId = 0)
        {
            GetLoginViewModel viewModel = new GetLoginViewModel();
            try
            {
                var item = _loginRepository.Find(name, pwd, checkPwd);
                #region 1

                if (item != null)//登录成功
                {
                    #region 2

                    #region 3
                    var agentItem = _loginRepository.GetAgentItemByAgentAccount(item.Name) ?? new AagentGroupAuthen();

                    var pAgentItem = _agentRepository.GetAgent(agentItem.TopAgentId) ?? new bx_agent();

                    var parentItem = _agentRepository.GetAgent(agentItem.ParentAgent) ?? new bx_agent();
                    if (isWChat == 1)
                    {
                        if (agentItem.TopAgentId != topAgentId)
                        {
                            viewModel.BusinessStatus = 0;//失败
                            viewModel.StatusMessage = "用户名或密码错误";
                            return viewModel;

                        }
                    }
                    if (GroupId != 0)
                    {
                        if (!_agentRepository.AgentInGroup(agentItem.TopAgentId, GroupId))
                        {
                            viewModel.BusinessStatus = 0;//失败
                            viewModel.StatusMessage = "用户名或密码错误";
                            return viewModel;
                        }
                    }
                    //9121敏梅客户的App 其他人的账号不能登录敏梅app,84659孔雀保险、78641振邦 一样逻辑
                    if (agentId == 9121)
                    {
                        if (agentItem.TopAgentId != 9121)
                        {
                            viewModel.BusinessStatus = 0;//失败
                            viewModel.StatusMessage = "用户名或密码错误";
                            return viewModel;

                        }
                    }
                    if (agentId == 84659)
                    {
                        if (agentItem.TopAgentId != 84659)
                        {
                            viewModel.BusinessStatus = 0;//失败
                            viewModel.StatusMessage = "用户名或密码错误";
                            return viewModel;

                        }
                    }
                    if (agentId == 78641)
                    {
                        if (agentItem.TopAgentId != 78641)
                        {
                            viewModel.BusinessStatus = 0;//失败
                            viewModel.StatusMessage = "用户名或密码错误";
                            return viewModel;

                        }
                    }
                    int smsAgentId = agentItem.Id;
                    if (agentItem.MessagePayType == 0)
                    {
                        smsAgentId = pAgentItem.Id;
                    }

                    var smsAccount = _consumerDetailRepository.GetBxSmsAccount(smsAgentId);
                    viewModel.isUsed = agentItem.IsUsed;
                    viewModel.topAgentName = pAgentItem.AgentName;
                    viewModel.agentMobile = agentItem.Mobile ?? "";
                    viewModel.agentId = agentItem.Id;
                    viewModel.agentName = agentItem.AgentName ?? "";
                    viewModel.parentAgentName = parentItem.AgentName;
                    viewModel.parentAgentMobile = parentItem.Mobile;
                    viewModel.parentAgentId = parentItem.Id;
                    viewModel.messagePayType = agentItem.MessagePayType;
                    viewModel.avail_times = smsAccount != null ? smsAccount.avail_times : 0;
                    viewModel.TopAgentMobile = pAgentItem.Mobile;
                    viewModel.PhoneIsWechat = agentItem.phone_is_wechat;
                    viewModel.HidePhone = agentItem.hide_phone;
                    //齐大康 modify 2018-04-18
                    viewModel.IsCompleteTask = agentItem.IsCompleteTask.HasValue ? agentItem.IsCompleteTask : 0;
                    viewModel.AuthenState = agentItem.AuthenState.HasValue ? agentItem.AuthenState : 0;
                    viewModel.TestState = (agentItem.TestState == null || agentItem.TestState == -1) ? 0 : agentItem.TestState;
                    //add by qdk 2018-11-09 顶级代理人集团id
                    viewModel.GroupId = pAgentItem!=null&& pAgentItem.group_id.HasValue ? pAgentItem.group_id.Value : 0 ;
                    //viewModel.BaseModules = _consumerDetailRepository.GetBaseModuleDbs();
                    var agentList = _agentRepository.GetAgentByAgentAccount(item.Name, item.PwdMd5);
                    List<ParentAgent> parentAgent = new List<ParentAgent>();

                    if (agentItem.IsDaiLi == 0)
                    {
                        if (pAgentItem.IsUsed != 1)
                        {
                            viewModel.BusinessStatus = 0;//失败
                            viewModel.StatusMessage = "登录失败，顶级账号被禁用";
                            return viewModel;
                        }
                    }
                    if (agentItem.IsUsed == 2)
                    {
                        viewModel.BusinessStatus = 0;//失败
                        viewModel.StatusMessage = "账号被禁用";
                        return viewModel;
                    }
                    if (agentItem.IsUsed == 0)
                    {
                        viewModel.BusinessStatus = 0;//失败
                        viewModel.StatusMessage = "账号待审核";
                        return viewModel;
                    }
                    if (agentItem.IsUsed == 3)
                    {
                        viewModel.BusinessStatus = 0;//失败
                        viewModel.StatusMessage = "该账号已被管理员删除,无法使用";
                        return viewModel;
                    }
                    //如果是试用账号 并且试用期已过禁止登陆(endDate为空的老数据允许登陆不作判定)
                    if (agentItem.accountType != 1 && agentItem.endDate.HasValue && agentItem.endDate < DateTime.Now)
                    {
                        viewModel.BusinessStatus = 0;
                        viewModel.StatusMessage = "该账号已过期";
                        return viewModel;
                    }

                    //判断当前登陆来源是否和数据的登陆来源一致(FromMethod= -1兼容老数据，老数据没有来源允许登陆)
                    if (FromMethod != -1 && (FromMethod & agentItem.platform) != FromMethod)
                    {
                        viewModel.BusinessStatus = 0;
                        viewModel.StatusMessage = "该账号没有登录权限";
                        return viewModel;
                    }
                    //增城人保的下级，微信可以登录、其他禁止登录//车易通例外，pc微信均可登录12438是线上id
                    if (agentItem.IsDaiLi == 0 && zcTopAgentId.Contains(agentItem.TopAgentId.ToString()) && FromMethod != 2 && !vmCanLoginPcTopAgentId.Contains(agentItem.TopAgentId.ToString()))
                    {
                        viewModel.BusinessStatus = 0;
                        viewModel.StatusMessage = "该账号没有登录权限";
                        return viewModel;
                    }

                    viewModel.BusinessStatus = 1;//成功
                    viewModel.StatusMessage = "登录成功";
                    viewModel.id = item.ManagerUserId;
                    viewModel.name = item.Name;
                    viewModel.RepeatQuote = agentItem.repeat_quote;
                    viewModel.OpenQuote = agentItem.openQuote;
                    string token;
                    _loginRepository.CreateUserToken(agentItem.Id, uniqueIdentifier, out token);
                    viewModel.token = token;
                    //viewModel.managerRoleId = item.ManagerRoleId;
                    viewModel.managerRoleId = agentItem.ManagerRoleId;
                    viewModel.mobile = item.Mobile;
                    viewModel.agentId = agentItem.Id;
                    //事故线索部门id
                    viewModel.DepartmentId = Convert.ToInt32(item.department_id);
                    var listAgent = _agentService.GetSonsListFromRedisToString(agentItem.Id);
                    listAgent.Remove(agentItem.Id.ToString());
                    if (listAgent.Any())
                    {
                        viewModel.IsHasChildAgents = 1;
                    }
                    viewModel.agentName = agentItem.AgentName ?? "";
                    viewModel.agentMobile = agentItem.Mobile ?? "";
                    viewModel.agentType = agentItem.AgentType.HasValue ? agentItem.AgentType : -1;
                    //顶级代理类型
                    viewModel.TopAgentType = pAgentItem.AgentType.HasValue//(pAgentItem.AgentType.HasValue && pAgentItem.AgentType.Value != 0)
                        ? pAgentItem.AgentType.Value : viewModel.agentType.Value;
                    viewModel.isShow = agentItem.IsShow.HasValue ? agentItem.IsShow : -1;

                    viewModel.IsShowCalc = agentItem.IsShowCalc.HasValue ? agentItem.IsShowCalc : -1;
                    viewModel.secretKey = agentItem.SecretKey;
                    viewModel.isDaiLi = agentItem.IsDaiLi;
                    viewModel.topAgentId = agentItem.TopAgentId;
                    viewModel.regType = agentItem.RegType;//0单店1集团
                    viewModel.OpenId = agentItem.OpenId;
                    viewModel.IsSubmit = agentItem.IsSubmit == 1 ? 1 : 0; //可否核保 1可以：2否
                    viewModel.lastForceDays = 90;
                    viewModel.BizDaysNum = 90;
                    List<bx_agent_config> bxAgentConfigs = _loginRepository.GetAgentConfigs(agentItem.TopAgentId);
                    //开通城市Id
                    //viewModel.CityList = bxAgentConfigs.Select(c => c.city_id).ToList();
                    if (bxAgentConfigs.Count == 1)
                    {
                        var bxAgentConfig = bxAgentConfigs.FirstOrDefault();
                        if (bxAgentConfig != null)
                        {
                            bx_cityquoteday bxCityquoteday = _loginRepository.GetBxCityquoteday(bxAgentConfig.city_id ?? 0);
                            viewModel.lastForceDays = bxCityquoteday.quotedays ?? 90;
                            viewModel.BizDaysNum = bxCityquoteday.bizquotedays ?? 90;
                        }

                    }

                    //var pAgentItem = _agentRepository.GetAgentTopParentInfo(agentItem.Id);
                    viewModel.parentSecretKey = pAgentItem.SecretKey;

                    // 张克亮 2018-09-28 返回头像信息
                    var groupAuthen = _groupAuthenService.GetGroupAuthen(agentItem.Id);
                    if (groupAuthen.BusinessStatus == 1 && groupAuthen.Data != null)
                    {
                        viewModel.HeadPortrait = (groupAuthen.Data as GroupAuthenModel).HeadPortrait;
                    }

                    #endregion


                    agentList.ForEach(x =>
                    {
                        ParentAgent pAgent = new ParentAgent();

                        //var itemAgent = _agentRepository.GetAgentTopParentInfo(x.Id);
                        var itemAgent = _agentRepository.GetAgent(x.TopAgentId);
                        //var itemAgent = _agentRepository.GetAgentTopParentInfo(x.Id);2017-07-13 lzl 优化不需要GetAgentTopParentInfo 获取顶级太消耗资源了
                        if (itemAgent != null)
                        {
                            pAgent.id = itemAgent.Id;
                            pAgent.agentName = itemAgent.AgentName;
                            pAgent.parentAgent = itemAgent.ParentAgent;
                            pAgent.secretKey = !string.IsNullOrEmpty(itemAgent.SecretKey) ? itemAgent.SecretKey : "";
                            pAgent.ManagerRoleId = itemAgent.ManagerRoleId;
                        }

                        parentAgent.Add(pAgent);
                    });



                    viewModel.parentAgent = parentAgent;


                    #region 4
                    var roleInfo = _loginRepository.GetRoleInfo(agentItem.ManagerRoleId);
                    var role = new ManagerRole()
                    {
                        id = roleInfo.id,
                        role_name = roleInfo.role_name,
                        role_status = roleInfo.role_status,
                        role_type = agentItem.ParentAgent == 0 ? 3 : roleInfo.role_type,
                        top_agent_id = roleInfo.top_agent_id,
                        isRequote = roleInfo.isRequote == null ? 2 : roleInfo.isRequote
                    };
                    viewModel.roleType = agentItem.ParentAgent == 0 ? 3 : role.role_type;
                    viewModel.isDistribute = _agentRepository.GetAgentIsSue(agentItem.Id);

                    if (viewModel.isDistribute == 0 && agentItem.IsDaiLi == 1)
                    {
                        viewModel.isDistribute = 1;
                    }
                    viewModel.roleInfo = role;
                    viewModel.ZhenBangType = agentItem.zhen_bang_type;

                    var agentSetting = _agentRepository.GetAgentSettingModelByAgentId(agentItem.Id);

                    if (agentSetting != null)
                    {
                        viewModel.SettlementType = agentSetting.settlement_type;
                        viewModel.desensitization = agentSetting.desensitization;
                        viewModel.OpenTuiXiu = Convert.ToInt32(agentSetting.is_open_tuixiu);
                    }
                    else
                    {
                        viewModel.SettlementType = 0;
                        viewModel.desensitization = 0;
                        viewModel.OpenTuiXiu = 0;
                    }

                    if (agentItem.agent_level != 1)
                    {
                        var topAgentSetting = _agentRepository.GetAgentSettingModelByAgentId(agentItem.TopAgentId);
                        if (topAgentSetting != null)
                        {
                            viewModel.desensitization = topAgentSetting.desensitization;
                            viewModel.OpenTuiXiu = Convert.ToInt32(topAgentSetting.is_open_tuixiu);
                        }
                        else
                        {
                            viewModel.desensitization = 2;
                            viewModel.OpenTuiXiu = 0;
                        }
                    }
                    tx_agent txAgent = _agentRepository.GetTxAgent(agentItem.TopAgentId);
                    if (txAgent != null)
                    {
                        viewModel.SignedState = txAgent.SignedState;
                    }

                    //获取结算模式(顶级获取自己的模式，下级获取顶级的模式)
                    viewModel.ModelType = GetModelType(agentItem, pAgentItem, viewModel.SettlementType);

                    var roleIds = new List<int>() { agentItem.ManagerRoleId };
                    var roleModule = _loginRepository.GetManagerRoleModuleRelation(roleIds);//角色跟模块关系
                    var moduleCode = roleModule.Where(x => x.module_code != "system_all").Select(x => x.module_code).Distinct().ToList();

                    #region 坐席权限
                    var isHasZuoXi = zuoXiRepository.CheckSelfOrChildAgentHasZuoXi(agentItem.Id, agentItem.agent_level);
                    if (isHasZuoXi)
                    {
                        moduleCode.AddRange(new List<string> { "callstatistics_business", "callstatistics_CallRecords", "callstatistics_Index" });
                        var zuoxi = zuoXiRepository.GetZXByAgentId(agentItem.Id);
                        if (zuoxi != null)
                        {
                            viewModel.ServiceType = zuoxi.service_type;
                        }

                    }

                    #endregion

                    //模块信息--2017-06-14 lzl 去掉远程出单渠道管理
                    var module = _loginRepository.GetManagerModule(moduleCode).Where(x => !string.IsNullOrEmpty(x.pater_code) && x.module_code != "agent_xia_ji_qudao").ToList();

                    #region 获取安装摄像头的代理人,如果没有安装摄像头则没有左侧的摄像头菜单
                    List<AgentIdParentIdTopIdViewModel> cameraAgent = _loginRepository.GetAgentHaveCamera();
                    var isCamera = cameraAgent.Any(c => c.TopAgentId == agentItem.TopAgentId);
                    if (isCamera)
                    {
                        viewModel.IsDisplayCamera = 1;//摄像头
                    }
                    else
                    {
                        module = module.Where(c => c.module_code != "camera_list").ToList();
                    }
                    //当前代理人有没有绑定摄像头列表
                    viewModel.HasCamera = cameraAgent.Any(c => c.Id == agentItem.Id) ? 1 : 0;
                    #endregion

                    //振邦的机构账号 不给财务管理（finance_management）
                    if (agentItem.IsDaiLi == 1)
                    {
                        module = AddModuleByModelType(viewModel.ModelType, module, agentItem);
                    }

                    List<manager_module_db> viewModels;


                    //一级菜单
                    viewModels = module.Where(x => x.module_type == 1 && x.pater_code == "system_all" && x.module_code != "system_all" && x.module_code != "statistical_report").OrderBy(x => x.order_by).ToList();
                    if (agentItem.RegType == 1)//集团账号
                    {
                        var addModule = _managerModuleRepository.GetList(t => t.module_code == "account_statistic" || t.module_code == "org_list").ToList();
                        module = module.Union(addModule).ToList();
                        //深圳人保账号单独去掉account_statistic， lzl 2017-12-28
                        if (agentItem.Id == 102247)
                        {
                            module = module.Where(c => c.module_code != "account_statistic").ToList();
                        }

                        //集团账号隐藏去掉网点列表
                        module = module.Where(t => t.module_code != "lattice_point").ToList();
                        viewModel.roleInfo.role_name = "集团管理员";
                    }

                    //振邦账号业务员列表action_url指向新的路径(Agent/ZhenBangAgentList)
                    if (agentItem.zhen_bang_type > 0)
                    {
                        var agentListModel = module.Where(t => t.module_code == "agent_list").FirstOrDefault();
                        if (agentListModel != null)
                        {
                            agentListModel.action_url = "Agent/ZhenBangAgentList";
                        }
                        //振邦账号隐藏业务员注册菜单  Solicitor_registration
                        module = module.Where(t => t.module_code != "Solicitor_registration").ToList();
                    }
                    //增城人保业务员列表action_url指向新的路径(Agent/ZengChengAgentList)
                    //2018-09-20 张克亮 小V盟项目也用此权限故配置文件中已经配置后此处需要做包含处理为的是不影响原先的业务
                    if (zcTopAgentId.Contains(pAgentItem.Id.ToString()))
                    {
                        var agentListModel = module.Where(t => t.module_code == "agent_list").FirstOrDefault();
                        if (agentListModel != null)
                        {
                            agentListModel.action_url = "Agent/ZengChengAgentList";

                            #region 添加团队规则菜单
                            /*团队设置*/
                            manager_module_db teamRole = new manager_module_db();
                            teamRole.action_url = "ChannelRatePolicy/TeamRules";
                            teamRole.pater_code = agentListModel.pater_code;
                            teamRole.module_code = "team_rules";
                            teamRole.module_name = "团队设置";
                            teamRole.module_level = 2;
                            teamRole.is_action = 1;
                            teamRole.is_menu = 1;
                            teamRole.order_by = agentListModel.order_by + 1;
                            teamRole.module_status = 1;
                            teamRole.module_type = 1;
                            module.Add(teamRole);
                            /*团队管理*/
                            teamRole = new manager_module_db();
                            teamRole.action_url = "Team/Index";
                            teamRole.pater_code = agentListModel.pater_code;
                            teamRole.module_code = "agent_zc_team";
                            teamRole.module_name = "团队管理";
                            teamRole.module_level = 2;
                            teamRole.is_action = 1;
                            teamRole.is_menu = 1;
                            teamRole.order_by = agentListModel.order_by + 1;
                            teamRole.module_status = 1;
                            teamRole.module_type = 1;
                            module.Add(teamRole);
                            #endregion
                        }
                    }
                    #region 2018-11-10 add by qdk 角色权限不是3,4,5的账号没有批改车牌模块
                    if (viewModel.roleType!=3&& viewModel.roleType!=4&& viewModel.roleType != 5)
                    {
                        module.RemoveAll(a => a.module_code == "Not_Correcting_Newcar"); 
                    }
                    #endregion

                    viewModel.module = viewModels.Select(x => new ManagerModule()
                    {
                        module_code = x.module_code,
                        module_name = x.module_name,
                        pater_code = x.pater_code,
                        module_level = x.module_level,
                        is_menu = x.is_menu,
                        is_action = x.is_action,
                        action_url = x.action_url,
                        module_status = x.module_status,
                        child = GetModuleChild(module, x.module_code),
                        crm_module_type = x.crm_module_type
                    }).ToList();//模块

                    //初始化三级按钮权限状态
                    viewModel.ButtonState = InitializeButtons(agentItem.ManagerRoleId);

                    var zhudian = _managerRoleService.GetRoleList(agentItem.Id).roleInfo.Where(x => x.RoleName == "驻店员").FirstOrDefault();
                    var zhudianrole = _managerRoleService.RoleExistByAgentId(agentItem.Id) == "驻店员";
                    if (zhudianrole)
                    {
                        if (agentItem.agent_level != 1)
                        {
                            viewModel.HasZhuDianYuanRole = true;
                        }

                    }
                    if (zhudian == null)
                    {
                        if (agentItem.agent_level == 1)
                        {
                            var aukey = _agentUkeyRepository.GetUkeyCityByAgentId(agentItem.Id);
                            if (aukey != null)
                            {//是深圳客户
                                ShenZhenCustomer shenZhenCustomer = new ShenZhenCustomer();
                                var managerModuleList = JsonConvert.DeserializeObject<List<ManagerModuleViewModel>>(shenZhenCustomer.zhumadianjson);
                                if (managerModuleList.Any())
                                {
                                    _managerRoleService.AddOrUpdateRole(-1, "驻店员", agentItem.Id, agentItem.AgentName, 0, managerModuleList);
                                }
                            }


                        }

                    }

                    #region 未分配标签控制
                    if (viewModel.roleType == 3 || viewModel.roleType == 4)
                    {
                        // 管理员和系统管理员始终显示未分配标签
                        viewModel.ShowNoDistributedLabel = true;
                    }
                    else
                    {
                        // 判断是否有批续 batchRenewal_list
                        var customer_module = viewModel.module.Where(o => o.module_code == "customer_module").FirstOrDefault();
                        if (customer_module != null && customer_module.child.Where(o => o.module_code == "batchRenewal_list").Any())
                        {
                            viewModel.ShowNoDistributedLabel = true;
                        }
                        else
                        {
                            // 判断是否有分配
                            if (viewModel.ButtonState.ContainsKey(BtnAuthType.btn_recycle.ToString()))
                            {
                                viewModel.ShowNoDistributedLabel = viewModel.ButtonState[BtnAuthType.btn_recycle.ToString()];
                            }
                        }
                    }
                    #endregion

                    #endregion
                    #endregion
                }
                else
                {
                    viewModel.BusinessStatus = 0;//失败
                    viewModel.StatusMessage = "用户名或密码错误";
                }
                #endregion
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;
            }


            return viewModel;
        }

        /// <summary>
        /// 微信授权登录 2018-09-18 张克亮  做小V盟项目时加入
        /// </summary>
        /// <param name="uniqueIdentifier">客户端唯一标识</param>
        /// <param name="topAgentId">顶级经济人ID</param>
        /// <returns></returns>
        public GetLoginViewModel WeChatLogin(string uniqueIdentifier, int topAgentId = 0)
        {
            GetLoginViewModel viewModel = new GetLoginViewModel();
            try
            {
                //检查授权信息并返回manageruser用户信息
                var userInfo = _loginRepository.WeChatFind(uniqueIdentifier);
                //授权失败
                if (userInfo.BusinessStatus != 1)
                {
                    //返回失败原因
                    viewModel.BusinessStatus = userInfo.BusinessStatus;
                    viewModel.StatusMessage = userInfo.StatusMessage;
                    return viewModel;
                }

                var item = userInfo.Data == null ? null : userInfo.Data as manageruser;

                #region 1

                if (item != null)//登录成功
                {
                    #region 2

                    #region 3
                    var agentItem = _loginRepository.GetAgentItemByAgentAccount(item.Name) ?? new AagentGroupAuthen();
                    var pAgentItem = _agentRepository.GetAgent(agentItem.TopAgentId) ?? new bx_agent();
                    var parentItem = _agentRepository.GetAgent(agentItem.ParentAgent) ?? new bx_agent();
                    int smsAgentId = agentItem.Id;
                    if (agentItem.MessagePayType == 0)
                    {
                        smsAgentId = pAgentItem.Id;
                    }
                    var smsAccount = _consumerDetailRepository.GetBxSmsAccount(smsAgentId);
                    viewModel.isUsed = agentItem.IsUsed;
                    viewModel.topAgentName = pAgentItem.AgentName;
                    viewModel.agentMobile = agentItem.Mobile ?? "";
                    viewModel.agentId = agentItem.Id;
                    viewModel.agentName = agentItem.AgentName ?? "";
                    viewModel.parentAgentName = parentItem.AgentName;
                    viewModel.parentAgentMobile = parentItem.Mobile;
                    viewModel.parentAgentId = parentItem.Id;
                    viewModel.messagePayType = agentItem.MessagePayType;
                    viewModel.avail_times = smsAccount != null ? smsAccount.avail_times : 0;
                    viewModel.TopAgentMobile = pAgentItem.Mobile;
                    viewModel.PhoneIsWechat = agentItem.phone_is_wechat;
                    viewModel.HidePhone = agentItem.hide_phone;
                    //齐大康 modify 2018-04-18
                    viewModel.IsCompleteTask = agentItem.IsCompleteTask.HasValue ? agentItem.IsCompleteTask : 0;
                    viewModel.AuthenState = agentItem.AuthenState.HasValue ? agentItem.AuthenState : 0;
                    viewModel.TestState = (agentItem.TestState == null || agentItem.TestState == -1) ? 0 : agentItem.TestState;
                    var agentList = _agentRepository.GetAgentByAgentAccount(item.Name, null);
                    List<ParentAgent> parentAgent = new List<ParentAgent>();
                    if (agentItem.IsDaiLi == 0)
                    {
                        if (pAgentItem.IsUsed != 1)
                        {
                            viewModel.BusinessStatus = 0;//失败
                            viewModel.StatusMessage = "登录失败，顶级账号被禁用";
                            return viewModel;
                        }
                    }
                    if (agentItem.IsUsed == 2)
                    {
                        viewModel.BusinessStatus = 0;//失败
                        viewModel.StatusMessage = "账号被禁用";
                        return viewModel;
                    }
                    if (agentItem.IsUsed == 0)
                    {
                        viewModel.BusinessStatus = 0;//失败
                        viewModel.StatusMessage = "账号待审核";
                        return viewModel;
                    }
                    if (agentItem.IsUsed == 3)
                    {
                        viewModel.BusinessStatus = 0;//失败
                        viewModel.StatusMessage = "该账号已被管理员删除,无法使用";
                        return viewModel;
                    }
                    //如果是试用账号 并且试用期已过禁止登陆(endDate为空的老数据允许登陆不作判定)
                    if (agentItem.accountType != 1 && agentItem.endDate.HasValue && agentItem.endDate < DateTime.Now)
                    {
                        viewModel.BusinessStatus = 0;
                        viewModel.StatusMessage = "该账号已过期";
                        return viewModel;
                    }

                    //增城人保的下级，微信可以登录、其他禁止登录
                    //if (agentItem.IsDaiLi == 0 && agentItem.TopAgentId.ToString() == zcTopAgentId && FromMethod != 2)
                    //{
                    //    viewModel.BusinessStatus = 0;
                    //    viewModel.StatusMessage = "该账号没有登录权限";
                    //    return viewModel;
                    //}

                    viewModel.BusinessStatus = 1;//成功
                    viewModel.StatusMessage = "登录成功";
                    viewModel.id = item.ManagerUserId;
                    viewModel.name = item.Name;
                    viewModel.RepeatQuote = agentItem.repeat_quote;
                    viewModel.OpenQuote = agentItem.openQuote;
                    string token;
                    _loginRepository.CreateUserToken(agentItem.Id, uniqueIdentifier, out token);
                    viewModel.token = token;
                    //viewModel.managerRoleId = item.ManagerRoleId;
                    viewModel.managerRoleId = agentItem.ManagerRoleId;
                    viewModel.mobile = item.Mobile;
                    viewModel.agentId = agentItem.Id;
                    //事故线索部门id
                    viewModel.DepartmentId = Convert.ToInt32(item.department_id);
                    var listAgent = _agentService.GetSonsListFromRedisToString(agentItem.Id);
                    listAgent.Remove(agentItem.Id.ToString());
                    if (listAgent.Any())
                    {
                        viewModel.IsHasChildAgents = 1;
                    }
                    viewModel.agentName = agentItem.AgentName ?? "";
                    viewModel.agentMobile = agentItem.Mobile ?? "";
                    viewModel.agentType = agentItem.AgentType.HasValue ? agentItem.AgentType : -1;
                    //顶级代理类型
                    viewModel.TopAgentType = pAgentItem.AgentType.HasValue//(pAgentItem.AgentType.HasValue && pAgentItem.AgentType.Value != 0)
                        ? pAgentItem.AgentType.Value : viewModel.agentType.Value;
                    viewModel.isShow = agentItem.IsShow.HasValue ? agentItem.IsShow : -1;

                    viewModel.IsShowCalc = agentItem.IsShowCalc.HasValue ? agentItem.IsShowCalc : -1;
                    viewModel.secretKey = agentItem.SecretKey;
                    viewModel.isDaiLi = agentItem.IsDaiLi;
                    viewModel.topAgentId = agentItem.TopAgentId;
                    viewModel.regType = agentItem.RegType;//0单店1集团
                    viewModel.OpenId = agentItem.OpenId;
                    viewModel.IsSubmit = agentItem.IsSubmit == 1 ? 1 : 0; //可否核保 1可以：2否
                    viewModel.lastForceDays = 90;
                    viewModel.BizDaysNum = 90;
                    List<bx_agent_config> bxAgentConfigs = _loginRepository.GetAgentConfigs(agentItem.TopAgentId);
                    //开通城市Id
                    //viewModel.CityList = bxAgentConfigs.Select(c => c.city_id).ToList();
                    if (bxAgentConfigs.Count == 1)
                    {
                        var bxAgentConfig = bxAgentConfigs.FirstOrDefault();
                        if (bxAgentConfig != null)
                        {
                            bx_cityquoteday bxCityquoteday = _loginRepository.GetBxCityquoteday(bxAgentConfig.city_id ?? 0);
                            viewModel.lastForceDays = bxCityquoteday.quotedays ?? 90;
                            viewModel.BizDaysNum = bxCityquoteday.bizquotedays ?? 90;
                        }
                    }

                    //var pAgentItem = _agentRepository.GetAgentTopParentInfo(agentItem.Id);
                    viewModel.parentSecretKey = pAgentItem.SecretKey;

                    // 张克亮 2018-09-19 返回头像信息
                    var groupAuthen = _groupAuthenService.GetGroupAuthen(agentItem.Id);
                    if (groupAuthen.BusinessStatus == 1 && groupAuthen.Data != null)
                    {
                        viewModel.HeadPortrait = (groupAuthen.Data as GroupAuthenModel).HeadPortrait;
                    }

                    #endregion

                    agentList.ForEach(x =>
                    {
                        ParentAgent pAgent = new ParentAgent();
                        var itemAgent = _agentRepository.GetAgent(x.TopAgentId);
                        if (itemAgent != null)
                        {
                            pAgent.id = itemAgent.Id;
                            pAgent.agentName = itemAgent.AgentName;
                            pAgent.parentAgent = itemAgent.ParentAgent;
                            pAgent.secretKey = !string.IsNullOrEmpty(itemAgent.SecretKey) ? itemAgent.SecretKey : "";
                            pAgent.ManagerRoleId = itemAgent.ManagerRoleId;
                        }
                        parentAgent.Add(pAgent);
                    });

                    viewModel.parentAgent = parentAgent;

                    #region 4
                    var roleInfo = _loginRepository.GetRoleInfo(agentItem.ManagerRoleId);
                    var role = new ManagerRole()
                    {
                        id = roleInfo.id,
                        role_name = roleInfo.role_name,
                        role_status = roleInfo.role_status,
                        role_type = agentItem.ParentAgent == 0 ? 3 : roleInfo.role_type,
                        top_agent_id = roleInfo.top_agent_id,
                        isRequote = roleInfo.isRequote == null ? 2 : roleInfo.isRequote
                    };
                    viewModel.roleType = agentItem.ParentAgent == 0 ? 3 : role.role_type;
                    viewModel.isDistribute = _agentRepository.GetAgentIsSue(agentItem.Id);

                    if (viewModel.isDistribute == 0 && agentItem.IsDaiLi == 1)
                    {
                        viewModel.isDistribute = 1;
                    }
                    viewModel.roleInfo = role;
                    viewModel.ZhenBangType = agentItem.zhen_bang_type;

                    var agentSetting = _agentRepository.GetAgentSettingModelByAgentId(agentItem.Id);

                    if (agentSetting != null)
                    {
                        viewModel.SettlementType = agentSetting.settlement_type;
                        viewModel.desensitization = agentSetting.desensitization;
                        viewModel.OpenTuiXiu = Convert.ToInt32(agentSetting.is_open_tuixiu);
                    }
                    else
                    {
                        viewModel.SettlementType = 0;
                        viewModel.desensitization = 0;
                        viewModel.OpenTuiXiu = 0;
                    }

                    if (agentItem.agent_level != 1)
                    {
                        var topAgentSetting = _agentRepository.GetAgentSettingModelByAgentId(agentItem.TopAgentId);
                        if (topAgentSetting != null)
                        {
                            viewModel.desensitization = topAgentSetting.desensitization;
                            viewModel.OpenTuiXiu = Convert.ToInt32(topAgentSetting.is_open_tuixiu);
                        }
                        else
                        {
                            viewModel.desensitization = 2;
                            viewModel.OpenTuiXiu = 0;
                        }
                    }


                    //获取结算模式(顶级获取自己的模式，下级获取顶级的模式)
                    viewModel.ModelType = GetModelType(agentItem, pAgentItem, viewModel.SettlementType);

                    var roleIds = new List<int>() { agentItem.ManagerRoleId };
                    var roleModule = _loginRepository.GetManagerRoleModuleRelation(roleIds);//角色跟模块关系
                    var moduleCode = roleModule.Where(x => x.module_code != "system_all").Select(x => x.module_code).Distinct().ToList();

                    #region 坐席权限
                    var isHasZuoXi = zuoXiRepository.CheckSelfOrChildAgentHasZuoXi(agentItem.Id, agentItem.agent_level);
                    if (isHasZuoXi)
                    {
                        moduleCode.AddRange(new List<string> { "callstatistics_business", "callstatistics_CallRecords", "callstatistics_Index" });
                        var zuoxi = zuoXiRepository.GetZXByAgentId(agentItem.Id);
                        if (zuoxi != null)
                        {
                            viewModel.ServiceType = zuoxi.service_type;
                        }

                    }

                    #endregion

                    //模块信息--2017-06-14 lzl 去掉远程出单渠道管理
                    var module = _loginRepository.GetManagerModule(moduleCode).Where(x => !string.IsNullOrEmpty(x.pater_code) && x.module_code != "agent_xia_ji_qudao").ToList();

                    #region 获取安装摄像头的代理人,如果没有安装摄像头则没有左侧的摄像头菜单
                    List<AgentIdParentIdTopIdViewModel> cameraAgent = _loginRepository.GetAgentHaveCamera();
                    var isCamera = cameraAgent.Any(c => c.TopAgentId == agentItem.TopAgentId);
                    if (isCamera)
                    {
                        viewModel.IsDisplayCamera = 1;//摄像头
                    }
                    else
                    {
                        module = module.Where(c => c.module_code != "camera_list").ToList();
                    }
                    //当前代理人有没有绑定摄像头列表
                    viewModel.HasCamera = cameraAgent.Any(c => c.Id == agentItem.Id) ? 1 : 0;
                    #endregion

                    //振邦的机构账号 不给财务管理（finance_management）
                    if (agentItem.IsDaiLi == 1)
                    {
                        module = AddModuleByModelType(viewModel.ModelType, module, agentItem);
                    }

                    List<manager_module_db> viewModels;


                    //一级菜单
                    viewModels = module.Where(x => x.module_type == 1 && x.pater_code == "system_all" && x.module_code != "system_all" && x.module_code != "statistical_report").OrderBy(x => x.order_by).ToList();
                    if (agentItem.RegType == 1)//集团账号
                    {
                        var addModule = _managerModuleRepository.GetList(t => t.module_code == "account_statistic" || t.module_code == "org_list").ToList();
                        module = module.Union(addModule).ToList();
                        //深圳人保账号单独去掉account_statistic， lzl 2017-12-28
                        if (agentItem.Id == 102247)
                        {
                            module = module.Where(c => c.module_code != "account_statistic").ToList();
                        }

                        //集团账号隐藏去掉网点列表
                        module = module.Where(t => t.module_code != "lattice_point").ToList();
                        viewModel.roleInfo.role_name = "集团管理员";
                    }

                    //振邦账号业务员列表action_url指向新的路径(Agent/ZhenBangAgentList)
                    if (agentItem.zhen_bang_type > 0)
                    {
                        var agentListModel = module.Where(t => t.module_code == "agent_list").FirstOrDefault();
                        if (agentListModel != null)
                        {
                            agentListModel.action_url = "Agent/ZhenBangAgentList";
                        }
                        //振邦账号隐藏业务员注册菜单  Solicitor_registration
                        module = module.Where(t => t.module_code != "Solicitor_registration").ToList();
                    }
                    //2018-09-20 张克亮 小V盟项目也用此权限故配置文件中已经配置后此处需要做包含处理为的是不影响原先的业务
                    if (zcTopAgentId.Contains(pAgentItem.Id.ToString()))
                    {
                        var agentListModel = module.Where(t => t.module_code == "agent_list").FirstOrDefault();
                        if (agentListModel != null)
                        {
                            agentListModel.action_url = "Agent/ZengChengAgentList";

                            #region 添加团队规则菜单
                            /*团队设置*/
                            manager_module_db teamRole = new manager_module_db();
                            teamRole.action_url = "ChannelRatePolicy/TeamRules";
                            teamRole.pater_code = agentListModel.pater_code;
                            teamRole.module_code = "team_rules";
                            teamRole.module_name = "团队设置";
                            teamRole.module_level = 2;
                            teamRole.is_action = 1;
                            teamRole.is_menu = 1;
                            teamRole.order_by = agentListModel.order_by + 1;
                            teamRole.module_status = 1;
                            teamRole.module_type = 1;
                            module.Add(teamRole);
                            /*团队管理*/
                            teamRole = new manager_module_db();
                            teamRole.action_url = "Team/Index";
                            teamRole.pater_code = agentListModel.pater_code;
                            teamRole.module_code = "agent_zc_team";
                            teamRole.module_name = "团队管理";
                            teamRole.module_level = 2;
                            teamRole.is_action = 1;
                            teamRole.is_menu = 1;
                            teamRole.order_by = agentListModel.order_by + 1;
                            teamRole.module_status = 1;
                            teamRole.module_type = 1;
                            module.Add(teamRole);
                            #endregion
                        }

                    }

                    viewModel.module = viewModels.Select(x => new ManagerModule()
                    {
                        module_code = x.module_code,
                        module_name = x.module_name,
                        pater_code = x.pater_code,
                        module_level = x.module_level,
                        is_menu = x.is_menu,
                        is_action = x.is_action,
                        action_url = x.action_url,
                        module_status = x.module_status,
                        child = GetModuleChild(module, x.module_code),
                        crm_module_type = x.crm_module_type
                    }).ToList();//模块

                    //初始化三级按钮权限状态
                    viewModel.ButtonState = InitializeButtons(agentItem.ManagerRoleId);

                    var zhudian = _managerRoleService.GetRoleList(agentItem.Id).roleInfo.Where(x => x.RoleName == "驻店员").FirstOrDefault();
                    var zhudianrole = _managerRoleService.RoleExistByAgentId(agentItem.Id) == "驻店员";
                    if (zhudianrole)
                    {
                        if (agentItem.agent_level != 1)
                        {
                            viewModel.HasZhuDianYuanRole = true;
                        }

                    }
                    if (zhudian == null)
                    {
                        if (agentItem.agent_level == 1)
                        {
                            var aukey = _agentUkeyRepository.GetUkeyCityByAgentId(agentItem.Id);
                            if (aukey != null)
                            {//是深圳客户
                                ShenZhenCustomer shenZhenCustomer = new ShenZhenCustomer();
                                var managerModuleList = JsonConvert.DeserializeObject<List<ManagerModuleViewModel>>(shenZhenCustomer.zhumadianjson);
                                if (managerModuleList.Any())
                                {
                                    _managerRoleService.AddOrUpdateRole(-1, "驻店员", agentItem.Id, agentItem.AgentName, 0, managerModuleList);
                                }
                            }


                        }

                    }

                    #region 未分配标签控制
                    if (viewModel.roleType == 3 || viewModel.roleType == 4)
                    {
                        // 管理员和系统管理员始终显示未分配标签
                        viewModel.ShowNoDistributedLabel = true;
                    }
                    else
                    {
                        // 判断是否有批续 batchRenewal_list
                        var customer_module = viewModel.module.Where(o => o.module_code == "customer_module").FirstOrDefault();
                        if (customer_module != null && customer_module.child.Where(o => o.module_code == "batchRenewal_list").Any())
                        {
                            viewModel.ShowNoDistributedLabel = true;
                        }
                        else
                        {
                            // 判断是否有分配
                            if (viewModel.ButtonState.ContainsKey(BtnAuthType.btn_recycle.ToString()))
                            {
                                viewModel.ShowNoDistributedLabel = viewModel.ButtonState[BtnAuthType.btn_recycle.ToString()];
                            }
                        }
                    }
                    #endregion

                    #endregion

                    #endregion
                }
                else
                {
                    viewModel.BusinessStatus = 0;//失败
                    viewModel.StatusMessage = "用户名或密码错误";
                }
                #endregion
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;
            }

            return viewModel;
        }

        /// <summary>
        /// 生成经济人授权信息 2018-09-19 张克亮  做小V盟项目时加入
        /// </summary>
        /// <param name="uniqueIdentifier">客户端唯一标识</param>
        /// <param name="topAgentId">顶级经济人ID</param>
        /// <returns>授权成功后生成的token</returns>
        public BaseViewModel SetAgentToken(string uniqueIdentifier, int topAgentId)
        {
            BaseViewModel baseView = new BaseViewModel();
            try
            {
                string outToken = string.Empty;
                int setResult = _loginRepository.CreateUserToken(topAgentId, uniqueIdentifier, out outToken);
                if (setResult == 1)
                {
                    baseView.BusinessStatus = 1;
                    baseView.StatusMessage = "生成授权成功";
                    baseView.Data = outToken;
                }
                else
                {
                    baseView.BusinessStatus = 0;
                    baseView.StatusMessage = "生成授权失败";
                }
            }
            catch (Exception ex)
            {
                baseView.BusinessStatus = -999;
                baseView.StatusMessage = "生成授权发生异常";
                logError.Error("LoginService-SetAgentToken,参数{uniqueIdentifier=" + uniqueIdentifier + ",topAgentId=" + topAgentId + "}，异常信息:" + ex.Message);
            }
            return baseView;
        }

        /// <summary>
        /// 获取子级模块
        /// </summary>
        /// <param name="moduleAll"></param>
        /// <param name="parentModuleCode"></param>
        /// <returns></returns>
        public List<ManagerModule> GetModuleChild(List<manager_module_db> moduleAll, string parentModuleCode)
        {
            var item = moduleAll.Where(x => x.pater_code == parentModuleCode).OrderBy(x => x.order_by).ToList();
            return item.Select(x => new ManagerModule()
            {
                module_code = x.module_code,
                module_name = x.module_name,
                pater_code = x.pater_code,
                module_level = x.module_level,
                is_menu = x.is_menu,
                is_action = x.is_action,
                action_url = x.action_url,
                module_status = x.module_status,
                orderBy = x.order_by,
                child = GetModuleChild(moduleAll, x.module_code).OrderBy(t => t.orderBy).ToList()
            }).ToList();
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="isDaiLi">是否是顶级经纪人</param>
        /// <param name="mobile">手机</param>
        /// <param name="agentName">名称(公司名称或姓名)</param>
        /// <param name="agentType">类型(修理厂、4s店)</param>
        /// <param name="region">地域</param>
        /// <param name="shareCode">邀请码</param>
        /// <param name="isCheck"></param>
        /// <param name="regType">单店集团</param>
        /// <param name="address">地址</param>
        /// <param name="uniqueIdentifier">客户端唯一标识</param>
        /// <param name="isUsed">是否启用</param>
        /// <param name="commodity">已购商品</param>
        /// <param name="platfrom">可以使用的平台</param>
        /// <param name="repeatQuote">重复报价</param>
        /// <param name="loginType">0普通注册登陆,1联合注册登陆</param>
        /// <param name="robotCount">安装机器人数量</param>
        /// <param name="brand">4S店品牌</param>
        /// <param name="contractEnd">收费账号合同截止日期</param>
        /// <param name="quoteCompany">可报价保险公司</param>
        /// <param name="addRenBao">人保专用</param>
        /// <param name="hidePhone">手机号加星</param>
        /// <param name="registedAgent"></param>
        /// <param name="accountType">0测试账号，1付费账号</param>
        /// <param name="endDate">账号有效期</param>
        /// <param name="openQuote">开通新车报价</param>
        /// <param name="zhenBangType">振邦机构类型</param>
        /// <param name="quoteCompany">可报价保险公司</param>
        /// <param name="hidePhone">电话号码加星</param>
        /// <returns></returns>
        public GetLoginViewModel Register(string name, string pwd, string mobile, string agentName, int agentType, string region, int isDaiLi, int shareCode, bool isCheck, int regType, string address, string uniqueIdentifier, bool isUsed, int commodity, int platfrom, int repeatQuote, int loginType, int robotCount, string brand, DateTime? contractEnd, int quoteCompany, int addRenBao, int hidePhone, out bx_agent registedAgent, int accountType = 0, string endDate = "", int openQuote = 0, int zhenBangType = 0, Dictionary<int, int> dicSource = null, int configCityId = -1, int openMultiple = 0, int settlement = 0, int structType = 0, int desensitization = 0, int peopleType = 0, int ceditOpenTuiXiu = 0)
        {
            registedAgent = new bx_agent();
            GetLoginViewModel viewModel = new GetLoginViewModel();
            try
            {

                if (_loginRepository.IsExist(name))//已经存在此用户
                {
                    viewModel.BusinessStatus = -1;
                    viewModel.StatusMessage = "账号已存在，请重新输入";
                    return viewModel;
                }
                //注册判断手机号是否存在
                if (isDaiLi == 1 && _agentRepository.IsExistMobileForTopAgent(mobile, 0))
                {
                    viewModel.BusinessStatus = -1;
                    viewModel.StatusMessage = "手机号已存在，请重新输入";
                    return viewModel;
                }
                else if (isDaiLi == 0)
                {
                    var parentAgentModel = _agentRepository.GetAgent(shareCode - 1000);
                    //验证上级代理人
                    viewModel = CheckParentAgent(parentAgentModel, mobile);
                    if (viewModel.BusinessStatus != 1)
                    {
                        return viewModel;
                    }
                }
                string isExistAgent = string.Empty;
                var result = _loginRepository.AddManagerUser(name, pwd, mobile, agentName, agentType, region, isDaiLi, shareCode, regType, address, isUsed, out isExistAgent, commodity, platfrom, repeatQuote, accountType, endDate, openQuote, loginType, robotCount, brand, contractEnd, quoteCompany, addRenBao, hidePhone, zhenBangType, dicSource, configCityId, openMultiple, settlement, structType, desensitization, out registedAgent, peopleType, ceditOpenTuiXiu);
                if (!result)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "注册失败";
                    if (!string.IsNullOrEmpty(isExistAgent))
                    {
                        viewModel.StatusMessage = isExistAgent;
                    }
                }
                else
                {


                    // 注册成功，向redis中添加数据
                    // _agentService.AddAgentGroupToRedis(registedAgent.Id, registedAgent.ParentAgent, registedAgent.TopAgentId, registedAgent.agent_level);
                    //RAgent ra = new RAgent(registedAgent.Id, registedAgent.ParentAgent, registedAgent.TopAgentId, registedAgent.agent_level);
                    //_agentService.AddAgentHotInfoToRedis(ra);



                    //注册成功 给顶级带人添加默认的客户状态
                    if (registedAgent.IsDaiLi == 1)
                    {
                        _customerStatusService.NewDailiStatus(registedAgent.Id);
                    }
                    else if (registedAgent.IsDaiLi == 0 && zcTopAgentId.Contains(registedAgent.TopAgentId.ToString()))
                    {
                        //增城下级注册时，看他的上级是否完成任务决定当前代理人能否创建团队
                        int parentAgentId = registedAgent.ParentAgent;
                        _updateCompleteTaskService.UpdateCompleteTask(parentAgentId);
                    }

                    if (!isCheck)
                    {
                        //1默认PC端登陆
                        viewModel = Login(name, pwd, uniqueIdentifier, 1);
                        if (viewModel.BusinessStatus != -10003)
                        {
                            viewModel.BusinessStatus = 1;
                            //外部注册时没有传mobile,但是Remove(null)会报异常,所以改成了mobile??""
                            HttpRuntime.Cache.Remove(mobile ?? "");
                            viewModel.StatusMessage = "注册成功";
                        }
                    }
                    else
                    {
                        viewModel.BusinessStatus = 1;
                        viewModel.StatusMessage = "注册成功";
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;
            }
            return viewModel;
        }



        /// <summary>
        /// 注册  发送验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="smsContent"></param>
        /// <param name="businessType"></param>
        /// <param name="smsAccount"></param>
        /// <param name="smsPassword"></param>
        /// <returns></returns>
        public SmsResultViewModel SendSms(string mobile, string smsContent, EnumSmsBusinessType businessType)
        {
            string SmsAccount = ConfigurationManager.AppSettings["SmsAccount"];
            string SmsPassword = ConfigurationManager.AppSettings["SmsPassword"];

            string url = string.Format("{0}/{1}", ConfigurationManager.AppSettings["SmsCenter"],
                ConfigurationManager.AppSettings["SmsCenterSendSmsMethod"]);
            string postData = string.Format("account={0}&password={1}&mobile={2}&smscontent={3}&businessType={4}",
                SmsAccount,
                SmsPassword, mobile, smsContent, (int)businessType);
            string result;
            int ret = HttpWebAsk.Post(url, postData, out result);
            return JsonHelper.DeSerialize<SmsResultViewModel>(result);
        }



        /// <summary>
        /// 校验邀请码适合合法
        /// </summary>
        /// <param name="parentAgent"></param>
        /// <returns>0 校验失败，1校验成功，2</returns>
        public int IsInvitationCode(int parentAgent)
        {
            int agentId = parentAgent - 1000;
            bx_agent bxAgent = _agentRepository.GetAgent(agentId);

            if (bxAgent == null)
            {
                return 0;
            }
            if (zcTopAgentId.Contains(bxAgent.TopAgentId.ToString()))//增城注册到6级
            {
                if (bxAgent.agent_level >= 6)//普通账号注册到6级
                {
                    return 3;
                }
            }
            else
            {
                if (bxAgent.agent_level >= 3)//普通账号注册到3级
                {
                    return 2;
                }
            }

            return 1;

            //return _loginRepository.IsInvitationCode(parentAgent);
        }


        /// <summary>
        /// 运营 销售登录
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public CqaLoginResultModel CqaLogin(string name, string pwd)
        {
            var viewModel = new CqaLoginResultModel();
            try
            {
                viewModel = _loginRepository.FindCqa(name, pwd);
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;
            }
            return viewModel;
        }

        /// <summary>
        /// 获取城市的商业交强的报价有效时间
        /// </summary>

        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public AgentDredgeCityRequest GetAgentDredgeCity(int topAgentId)
        {
            AgentDredgeCityRequest viewModel = new AgentDredgeCityRequest();
            try
            {

                viewModel.AgentDredgeCities = _loginRepository.AgentDredgeCityList(topAgentId);
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;
            }
            return viewModel;
        }

        #region 微信账号统一

        public int IsExistMobile(int topAgentId, string mobile)
        {
            return _loginRepository.IsExistMobile(topAgentId, mobile);
        }

        /// <summary>
        /// 外部调用的登录接口
        /// </summary>
        /// <param name="request"></param>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public GetLoginViewModel ExternalLogin(ExternalLoginRequest request, string keyCode)
        {
            string name;
            string pwd;
            //是否输入了密码
            bool isHasPwd = !string.IsNullOrEmpty(request.Password);
            if (isHasPwd)
            {
                name = request.UserName;
                pwd = request.Password;
            }
            else
            {
                name = request.UserName + request.AgentId;
                pwd = (request.UserName + request.AgentId).GetMd5();
            }

            int shareCode = request.AgentId + 1000;
            //先调登录,登录失败后再掉注册
            GetLoginViewModel result = Login(name, pwd, keyCode, request.FromMethod);
            if (result.BusinessStatus == 0)
            {
                if (isHasPwd)
                {
                    return result;
                }
                if (result.StatusMessage == "用户名或密码错误")
                {
                    //找见topagentid对应得region
                    bx_agent topAgent = _agentRepository.GetAgent(request.AgentId);
                    if (topAgent == null)
                    {
                        result.BusinessStatus = (int)BusinessStatusType.ParamError;
                        result.StatusMessage = "顶级代理人ID不正确";
                        return result;
                    }
                    bx_agent registedAgent;

                    int agentType = int.Parse(topAgent.AgentType.ToString());
                    string endDate = topAgent.endDate.HasValue ? Convert.ToDateTime(topAgent.endDate).ToString("yyyy-MM-dd HH:mm:ss") : null;
                    int loginType = 1;//联合登陆注册
                    //注册再登录
                    return Register(name: name, pwd: pwd, mobile: null, agentName: request.UserName, agentType: agentType, region: topAgent.Region, isDaiLi: 0, shareCode: shareCode, isCheck: false, regType: 0, address: topAgent.AgentAddress, uniqueIdentifier: keyCode, isUsed: true, commodity: topAgent.commodity, platfrom: topAgent.platform, repeatQuote: topAgent.repeat_quote, loginType: loginType, robotCount: topAgent.robotCount, brand: topAgent.agentBrand, contractEnd: topAgent.contractEndDate, quoteCompany: topAgent.quoteCompany, addRenBao: 0, registedAgent: out registedAgent, hidePhone: topAgent.hide_phone, accountType: topAgent.accountType, endDate: endDate, openQuote: topAgent.openQuote, peopleType: 0);
                }

            }
            return result;
        }

        public GetTopAgentInfoResult TopAgentInfoByAccount(string agentAccount, int topAgentId)
        {
            GetTopAgentInfoResult viewModel = new GetTopAgentInfoResult();
            try
            {

                bx_agent agentinfo = _agentRepository.GetAgentByAgentAccount(agentAccount);
                if (agentinfo == null)
                {
                    viewModel.BusinessStatus = -10000;
                    viewModel.StatusMessage = "输入的账号不存在";
                    return viewModel;
                }
                if (agentinfo.ParentAgent == 0 && agentinfo.Id == topAgentId)
                {
                    viewModel.AgentName = agentinfo.AgentName;
                    viewModel.Mobile = agentinfo.Mobile;
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "获取成功";
                    return viewModel;
                }
                //bx_agent topAgent= _agentRepository.GetAgentTopParentInfo(agentinfo.Id);

                if (agentinfo.TopAgentId == topAgentId)
                {
                    bx_agent agent = _agentRepository.GetAgent(agentinfo.ParentAgent);
                    viewModel.AgentName = agent.AgentName;
                    viewModel.Mobile = agent.Mobile;
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "获取成功";
                }
                else
                {
                    viewModel.BusinessStatus = -10002;
                    viewModel.StatusMessage = "输入的账号和代理人不匹配";
                    return viewModel;
                }
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;

            }
            return viewModel;

        }


        public IsUpdateAccountResult IsUpdateAccount(string openId, int topAgent)
        {
            IsUpdateAccountResult result = new IsUpdateAccountResult();

            try
            {

                List<bx_agent> agentResult = _loginRepository.CheckWChantUser(openId);
                if (agentResult.Count > 1)
                {
                    foreach (var item in agentResult)
                    {

                        if (topAgent == item.TopAgentId)
                        {

                            result.IsNeedUpdateAccount = string.IsNullOrEmpty(item.AgentAccount);
                            result.AgentId = item.Id;
                        }
                    }
                }
                else if (agentResult.Count == 1)
                {
                    var firstOrDefault = agentResult.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        if (topAgent == firstOrDefault.TopAgentId)
                        {

                            result.IsNeedUpdateAccount = string.IsNullOrEmpty(firstOrDefault.AgentAccount);
                            result.AgentId = firstOrDefault.Id;
                        }
                    }
                }
                else
                {
                    result.IsNeedUpdateAccount = false;
                }




                result.BusinessStatus = 1;
                result.StatusMessage = "获取成功";
                return result;
            }
            catch (Exception ex)
            {

                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                result.BusinessStatus = -10003;
                result.StatusMessage = "服务发生异常:" + ex.Message;
            }
            return result;

        }

        public CreateWCahtAgentAccountResult CreateWCahtAgentAccount(string opendId, string account, string passWord, int agentId)
        {
            CreateWCahtAgentAccountResult result = new CreateWCahtAgentAccountResult();
            try
            {
                bx_agent bxAgent = _agentRepository.GetAgent(agentId);




                if (_loginRepository.IsExist(account))//已经存在此用户
                {
                    result.BusinessStatus = -1;
                    result.StatusMessage = "用户名已存在，请重新输入";
                    return result;
                }
                if (string.IsNullOrEmpty(bxAgent.AgentAccount))
                {
                    manageruser user = new manageruser()
                    {
                        Name = account,
                        PwdMd5 = passWord.GetMd5(),
                        CreateTime = DateTime.Now.ToString(),

                        Remarks = "微信创建账号",
                        ManagerRoleId = 0,
                        AccountType = 1
                    };
                    _loginRepository.CreateWCahtAgentAccount(opendId, account, passWord.GetMd5(), user, agentId);
                    result.BusinessStatus = 1;
                    result.StatusMessage = "创建成功";

                    return result;
                }
                else
                {
                    result.BusinessStatus = -14010;
                    result.StatusMessage = "绑定账号的代理人已存在账号";

                    return result;
                }

            }
            catch (Exception ex)
            {

                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                result.BusinessStatus = -10003;
                result.StatusMessage = "服务发生异常:" + ex.Message;
            }
            return result;
        }



        public GetUserInfoByShareCodeResult GettUserInfoByShareCode(int shareCode)
        {
            GetUserInfoByShareCodeResult result = new GetUserInfoByShareCodeResult();
            try
            {
                if (shareCode <= 1000)
                {
                    result.BusinessStatus = -10000;
                    result.StatusMessage = "参数错误";
                }
                int agentid = shareCode - 1000;
                bx_agent agent = _agentRepository.GetAgent(agentid);
                result.ParentAgentName = agent.AgentName;
                result.BusinessStatus = 1;
                result.StatusMessage = "获取成功";
                return result;
            }
            catch (Exception ex)
            {

                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                result.BusinessStatus = -10003;
                result.StatusMessage = "服务发生异常:" + ex.Message;
            }

            return result;
        }

        #endregion


        #region 数据刷新
        /// <summary>
        /// 
        /// </summary>
        public void DataUpdate()
        {

            //List<dateupdate>listdate=  _iUserInfoRepository.UdapteDateupdate();

            //  foreach (var item in listdate)
            //  {
            //     bx_agent agentItem=new bx_agent();

            //     _agentRepository.AddAgent(item.Name, item.Mobile, 0, "地域", item.Name, item.PwdMd5,
            //      0, item.top_agent_id, 0, "更新数据", true, out  agentItem, item.ManagerRoleId);
            //  }awosaofdsof
            List<bx_userinfo> bxUserinfos = _iUserInfoRepository.UdapteDateupdate();
            int i = 0;
            int m = 0;
            foreach (var items in bxUserinfos)
            {
                i++;
                m++;
                List<bx_car_renewal> bxCarRenewals = _iUserInfoRepository.GetBxCarRenewals(items.LicenseNo);

                bx_userinfo_renewal_index bxUserinfoRenewalIndex = _iUserInfoRepository.GetBxUserinfoRenewalIndex(items.Id);
                var bxCarRenewal = bxCarRenewals.FirstOrDefault();
                if (bxCarRenewal != null && bxUserinfoRenewalIndex != null)
                {
                    bxUserinfoRenewalIndex.car_renewal_id = bxCarRenewal.Id;
                    _iUserInfoRepository.UpdatebxUserinfoRenewalIndex(bxUserinfoRenewalIndex, items.Id);
                }
                logError.Error("成功修改:" + i + "失败修改" + m + "buid:" + items.Id);
            }

        }
        public void UpdateCarRenewalIndex()
        {
            _iUserInfoRepository.UpdateCarRenewalIndex();
        }
        public GetLoginViewModel UpdateInfo()
        {
            GetLoginViewModel viewModel = new GetLoginViewModel();
            try
            {
                //var result = _loginRepository.UpdateInfo();

                List<bx_agent> agents = _agentRepository.FindList().Where(c => c.TopAgentId <= 0).ToList();
                foreach (var item in agents)
                {
                    int topagent = item.TopAgentId;
                    _loginRepository.UpdateTopAgentId(item.Id, topagent);
                }




            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;
            }
            return viewModel;
        }
        #endregion


        public bool IsExistMobile(string mobile, int parentAgent)
        {
            return _agentRepository.GetAgentByPhoneTopAgent(mobile, parentAgent);
        }


        /// <summary>
        /// 初始化拥有三级按钮权限的状态
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Dictionary<string, bool> InitializeButtons(int roleId)
        {
            //已经拥有权限的按钮
            var roleBtnCode = _managerModuleButtonRepository.GetRoleButtonList(roleId).Select(t => t.button_code);
            //读取按钮枚举成员
            var enumName = Enum.GetNames(typeof(BtnAuthType));

            GetLoginViewModel viewModel = new GetLoginViewModel();
            viewModel.ButtonState = new Dictionary<string, bool>();
            //初始化viewModel的ButtonState键值对
            foreach (var item in enumName)
            {
                if (roleBtnCode.Contains(item))
                {
                    viewModel.ButtonState.Add(item, true);
                }
                else
                {
                    viewModel.ButtonState.Add(item, false);
                }
            }
            return viewModel.ButtonState;
        }

        /// <summary>
        /// 获取代理人的结算方式
        /// </summary>
        /// <param name="agentItem">当前登录的代理人</param>
        /// <param name="topAgent">顶级代理人</param>
        /// <param name="settlement">当前集团账号的结算方式</param>
        /// <returns></returns>
        public int GetModelType(bx_agent agentItem, bx_agent topAgent, int settlement)
        {
            if (agentItem.RegType == 1)//集团
            {
                return GetModelBySettlement(settlement);
            }
            else
            {
                if (agentItem.IsDaiLi == 1)
                {
                    if (agentItem.zhen_bang_type == 1)//机构账号  拿自己集团的结算方式
                    {
                        if (agentItem.group_id == null)
                        {
                            return 3;
                        }
                        var setting = _agentRepository.GetAgentSettingModelByAgentId(agentItem.group_id.Value);
                        if (setting == null)
                        {
                            return 3;
                        }
                        return GetModelBySettlement(setting.settlement_type);
                    }
                    else //普通的顶级账号
                    {
                        return 3;
                    }
                }
                else //下级拿顶级的
                {
                    if (topAgent.RegType == 1)
                    {
                        return GetModelBySettlement(settlement);
                    }
                    if (topAgent.zhen_bang_type == 1)
                    {
                        if (topAgent.group_id == null)
                        {
                            return 3;
                        }
                        var setting = _agentRepository.GetAgentSettingModelByAgentId(topAgent.group_id.Value);
                        if (setting == null)
                        {
                            return 3;
                        }
                        return GetModelBySettlement(setting.settlement_type);
                    }
                    return 3;
                }
            }
        }

        public int GetModelBySettlement(int settlement)
        {
            if (settlement == 1)//只给机构结算
            {
                return 2;
            }
            else if (settlement == 2)//给机构、代理人、网点结算
            {
                return 1;
            }
            else
            {
                return 3;
            }
        }

        /// <summary>
        /// 振邦结算通过模式类型添加结算菜单
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="moduleList"></param>
        public List<manager_module_db> AddModuleByModelType(int modelType, List<manager_module_db> moduleList, bx_agent agentItem)
        {
            string paterCode = "finance_management";
            //待结算列表
            manager_module_db waittingList = new manager_module_db()
            {
                module_code = "WaitingList",
                module_name = "审核通过待结算",
                pater_code = paterCode,
                module_level = 2,
                order_by = 1,
                module_status = 0,
                is_menu = 1,
                is_action = 1,
            };

            //结算单-代理人
            manager_module_db agentSettleOrder = new manager_module_db()
            {
                module_code = "AgentSettleOrder",
                module_name = "代理人结算",
                pater_code = paterCode,
                module_level = 2,
                order_by = 6,
                module_status = 0,
                is_menu = 1,
                is_action = 1,
            };
            //结算单-网点
            manager_module_db dotSettleOrder = new manager_module_db()
            {
                module_code = "DotSettleOrder",
                module_name = "网点结算",
                pater_code = paterCode,
                module_level = 22,
                order_by = 4,
                module_status = 0,
                is_menu = 1,
                is_action = 1,
            };
            //结算单-机构
            manager_module_db orgSettleOrder = new manager_module_db()
            {
                module_code = "OrgSettleOrder",
                module_name = "机构结算",
                pater_code = paterCode,
                module_level = 23,
                order_by = 5,
                module_status = 0,
                is_menu = 1,
                is_action = 1,
            };
            //结算单-保险公司
            manager_module_db sourceSettleOrder = new manager_module_db()
            {
                module_code = "SourceSettleOrder",
                module_name = "保司结算",
                pater_code = paterCode,
                module_level = 24,
                order_by = 3,
                module_status = 0,
                is_menu = 1,
                is_action = 1,
            };
            moduleList.Add(waittingList);
            if (modelType == 1)
            {
                waittingList.action_url = "Finance/WaitingList/group1-1";
                agentSettleOrder.action_url = "Finance/SettleOrder/group1-1";
                dotSettleOrder.action_url = "Finance/SettleOrder/group1-2";
                orgSettleOrder.action_url = "Finance/SettleOrder/group1-3";
                sourceSettleOrder.action_url = "Finance/SettleOrder/group1-5";

                moduleList.Add(agentSettleOrder);
                moduleList.Add(dotSettleOrder);
                moduleList.Add(orgSettleOrder);
            }
            else if (modelType == 2)
            {
                //模式2 集团 和 机构的结算对象不一样，需要区分
                if (agentItem.RegType == 1)
                {
                    waittingList.action_url = "Finance/WaitingList/group2r1-3";

                    orgSettleOrder.action_url = "Finance/SettleOrder/group2r1-3";
                    sourceSettleOrder.action_url = "Finance/SettleOrder/group2r1-5";

                    moduleList.Add(waittingList);
                    moduleList.Add(orgSettleOrder);
                    moduleList.Add(sourceSettleOrder);

                }
                else if (agentItem.zhen_bang_type == 1)
                {
                    waittingList.action_url = "Finance/WaitingList/group2r2-1";

                    orgSettleOrder.action_url = "Finance/SettleOrder/group2r2-1";
                    sourceSettleOrder.action_url = "Finance/SettleOrder/group2r2-2";

                    moduleList.Add(waittingList);
                    moduleList.Add(orgSettleOrder);
                    moduleList.Add(sourceSettleOrder);
                }
            }
            else if (modelType == 3)
            {
                waittingList.action_url = "Finance/WaitingList/singlestore-1";
                agentSettleOrder.action_url = "Finance/SettleOrder/singlestore-1";
                sourceSettleOrder.action_url = "Finance/SettleOrder/singlestore-5";

                moduleList.Add(agentSettleOrder);
            }
            moduleList.Add(sourceSettleOrder);
            return moduleList;
        }

        /// <summary>
        /// 验证上级代理人
        /// </summary>
        /// <param name="parentAgentModel"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public GetLoginViewModel CheckParentAgent(bx_agent parentAgentModel, string mobile)
        {
            GetLoginViewModel viewModel = new GetLoginViewModel();
            if (parentAgentModel == null)
            {
                viewModel.BusinessStatus = -1;
                viewModel.StatusMessage = "邀请码错误";
                return viewModel;
            }
            if (_agentRepository.IsExistMobileForTopAgent(mobile, parentAgentModel.TopAgentId))
            {
                viewModel.BusinessStatus = -1;
                viewModel.StatusMessage = "手机号已存在，请重新输入";
                return viewModel;
            }
            if (parentAgentModel == null)
            {
                viewModel.BusinessStatus = -1;
                viewModel.StatusMessage = "邀请码错误";
                return viewModel;
            }


            if (!zcTopAgentId.Contains(parentAgentModel.TopAgentId.ToString())) //非增城人保下级
            {
                if (parentAgentModel.agent_level > 2)
                {
                    viewModel.BusinessStatus = -1;
                    viewModel.StatusMessage = "不能注册4级代理人";
                    return viewModel;
                }
            }
            else
            {
                if (parentAgentModel.agent_level > 5) //增城人保下级可以注册到6级
                {
                    viewModel.BusinessStatus = -1;
                    viewModel.StatusMessage = "不能注册7级代理人";
                    return viewModel;
                }
            }

            viewModel.BusinessStatus = 1;
            return viewModel;
        }

        public SfMobileLoginViewModel SfMobileLogin(string agentAccount, string agentPassWord)
        {
            return _loginRepository.SfMobileLogin(agentAccount, agentPassWord);
        }
    }
}
