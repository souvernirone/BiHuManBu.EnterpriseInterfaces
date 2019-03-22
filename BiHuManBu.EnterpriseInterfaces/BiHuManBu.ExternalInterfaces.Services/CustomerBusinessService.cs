using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Enums;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using log4net;
using ServiceStack.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.Services
{
    /// <summary>
    /// 客户业务员列表页服务类
    /// </summary>
    public class CustomerBusinessService : CommonBehaviorService, ICustomerBusinessService
    {

        private ILog logError;
        private ILog logInfo;
        private ICacheHelper _cacheHelper;
        private IAgentRepository _agentRepository;
        private IUserInfoRepository _userInfoRepository;
        private ICustomerBusinessRepository _customerbusinessRepository;
        private ICustomerTopLevelService _customerTopLevelService;
        private IRecycleHistoryService _recycleHistoryService;
        private IConsumerDetailService _iConsumerDetailService;
        private IAgentService _agentService;
        private IAuthorityService _authorityService;
        private IAgentSpecialRateRepository _agentSpecialRateRepository;
        private IDeleteAgentLogRepository _deleteAgentLogRepository;
        private IManagerRoleRepository _managerRoleRepository;
        private IManagerRoleModuleRelationRepository _managerRoleModuleRelationRepository;
        private ILoginService _loginService;
        private IGroupAuthenRepository _groupAuthenRepository;
        private readonly IAgentUkeyRepository _agentUkeyRepository;
        private readonly IConsumerDetailRepository _consumerDetailRepository;

        private static readonly string _apicenter = ConfigurationManager.AppSettings["BaoJiaJieKou"];
        private readonly string _keyCode = ConfigurationManager.AppSettings["keyCode"];
        /// <summary>
        /// 下级代理人数量超过两千的顶级代理人Id
        /// </summary>
        private static int[] HasMoreThan2000ChildAgentTopAgentId = new[] { 8031, 2668, 4245, 12457 };

        public CustomerBusinessService(IUserInfoRepository userInfoRepository
            , IConsumerDetailService IConsumerDetailService
            , ICustomerTopLevelService customerTopLevelService
            , ICacheHelper cacheHelper
            , IAgentRepository agentRepository
            , IRecycleHistoryService recycleHistoryService
            , ICustomerBusinessRepository customerbusinessRepository
            , IAgentService agentService
            , IAuthorityService authorityService
            , IAgentSpecialRateRepository agentSpecialRateRepository
            , IDeleteAgentLogRepository deleteAgentLogRepository
            , IManagerRoleRepository managerRoleRepository
            , ILoginService loginService

            , IAgentUkeyRepository agentUkeyRepository
            , IConsumerDetailRepository consumerDetailRepository
            , IGroupAuthenRepository groupAuthenRepository
            )
            : base(agentRepository, cacheHelper)
        {
            _iConsumerDetailService = IConsumerDetailService;
            _cacheHelper = cacheHelper;
            _userInfoRepository = userInfoRepository;
            _agentRepository = agentRepository;
            _customerTopLevelService = customerTopLevelService;
            logError = LogManager.GetLogger("ERROR");
            logInfo = LogManager.GetLogger("INFO");
            _customerbusinessRepository = customerbusinessRepository;
            _recycleHistoryService = recycleHistoryService;
            _agentService = agentService;
            _authorityService = authorityService;
            _agentSpecialRateRepository = agentSpecialRateRepository;
            _deleteAgentLogRepository = deleteAgentLogRepository;
            _managerRoleRepository = managerRoleRepository;
            _loginService = loginService;
            _groupAuthenRepository = groupAuthenRepository;
            _agentUkeyRepository = agentUkeyRepository;
            _consumerDetailRepository = consumerDetailRepository;
        }

        /// <summary>
        /// 顶级分配功能，获取下级业务员列表方法
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <param name="isHas">1:包含启用和禁用业务员，0：只包含启用业务员</param>
        /// <returns></returns>
        public CTopLevelAgentListViewModel GetBusinessAgentList(int currentAgentId, int isHas)
        {
            var agentList = new CTopLevelAgentListViewModel();
            // 从数据库中获取匹配的业务员信息
            List<CTopLevelAgentViewModel> list = _customerbusinessRepository.GetBusinessAgentList(currentAgentId);
            //默认只查询启用业务员
            if (isHas == 0)
            {
                list = list.Where(a => a.IsUsed == 1).ToList();
            }
            if (list != null)
            {
                agentList.AgentList = list;
                agentList.TotalCount = list.Count;
                agentList.BusinessStatus = 1;
                agentList.StatusMessage = "获取成功";
            }

            return agentList;
        }

        public async Task<bool> UpdateDistributeRecycle(List<long> buids, int topAgent, int childAgent)
        {
            bool isSuccess = false;

            childAgent = childAgent == 0 ? topAgent : childAgent;
            // 取出所有要回收的数据
            var list = await _userInfoRepository.GetDistributedRecycleAsync(buids);

            //查询代理人用到的信息
            List<bx_crm_steps> lstStemps = new List<bx_crm_steps>();
            string[] agentids = list.Select(n => n.AgentId).ToArray();
            agentids = agentids.Concat(new string[] { topAgent.ToString(), childAgent.ToString() }).ToArray();
            string whereStr = string.Join(",", agentids);
            List<AgentNameViewModel> lst = _agentRepository.FindAgentList(whereStr);

            // 插入回收记录表
            var taskInsertRecycle = _recycleHistoryService.InsertRecycleHistoryAsync(list, topAgent, childAgent, 0);

            AgentNameViewModel AgentView = lst.Where(n => n.AgentId == childAgent).FirstOrDefault();

            #region 新增步骤记录
            // 新增步骤记录
            foreach (var item in list)
            {
                AgentNameViewModel aName = lst.Where(n => n.AgentId == Convert.ToInt32(item.AgentId)).FirstOrDefault();
                if (aName != null)
                {
                    DistributeBackViewModel Content = new DistributeBackViewModel
                    {
                        AgentId = topAgent,
                        AgentName = "未分配",
                        OperateName = AgentView != null ? AgentView.AgentName : "",
                        OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        OriId = Convert.ToInt32(item.AgentId),
                        OriName = aName.AgentName
                    };

                    string jsonContent = CommonHelper.TToString<DistributeBackViewModel>(Content);
                    bx_crm_steps step = new bx_crm_steps
                    {
                        agent_id = Convert.ToInt32(item.AgentId),
                        b_uid = item.Buid,
                        type = 7,
                        create_time = DateTime.Now,
                        json_content = jsonContent
                    };
                    lstStemps.Add(step);
                }
            }
            var taskInsertStep = _iConsumerDetailService.InsertBySqlAsync(lstStemps);
            #endregion

            int isDistributed = 0;
            int resultAgent = topAgent;
            if (topAgent != childAgent)
            {
                // 判断是否是管理员
                var roleType = _managerRoleRepository.GetRoleTypeByAgentId(childAgent);
                if (roleType != (int)RoleType.Admin)
                {
                    // 是否有分配权限
                    if (_authorityService.IsHasDistributeAuth(childAgent))
                    {
                        isDistributed = 3;
                        resultAgent = childAgent;
                    }
                }
            }

            _customerbusinessRepository.UpdateDistributeRecycle(string.Join(",", buids), resultAgent, topAgent.ToString().GetMd5().ToUpper(), isDistributed);

            Task.WaitAll(taskInsertRecycle, taskInsertStep);

            isSuccess = true;

            return isSuccess;
        }

        public DistributedDataVm GetDistributeForAgent(GetDistributeForAgentRequest request)
        {
            var dvVm = new DistributedDataVm();
            List<BxMessageViewModel> bxMessages = _customerbusinessRepository.GetBxMessages(request.AgentId);
            if (!bxMessages.Any())
            {
                return dvVm;
            }
            string buids = string.Join(",", bxMessages.Select(c => c.Body).ToList());
            string messageid = string.Join(",", bxMessages.Select(c => c.Id).ToList());
            dvVm.AgentId = request.AgentId;
            dvVm.BuidsString = buids;
            dvVm.MessageIds = messageid;
            var composite = new List<CompositeBuldLicenseNo>();
            string[] sArray = buids.Split(',');
            if (sArray.Count() == 1)
            {
                var cbNo = new CompositeBuldLicenseNo()
                {
                    BuId = bxMessages.FirstOrDefault().Buid ?? 0,
                    LicenseNo = bxMessages.FirstOrDefault().License_No ?? ""
                };
                composite.Add(cbNo);
                dvVm.Data = composite;
            }

            // 判断是否需要特殊处理分配
            var isAdmin = _authorityService.IsSystemAdminOrAdmin(request.AgentId);
            if (!isAdmin)
            {
                dvVm.IsManager = _authorityService.IsHasDistributeAuth(request.AgentId);
            }

            return dvVm;
        }

        public int UpdateBxMessage(string messageIds)
        {
            return _customerbusinessRepository.UpdateBxMessage(messageIds);
        }

        /// <summary>
        /// 根据传入的标签获取数量
        /// 陈亮  17-11-17  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="topLabel"></param>
        /// <returns></returns>
        private async Task<int> GetTopLabelCountAsync(GetCustomerListRequest request, int topLabel)
        {
            request.TopLabel = topLabel;

            var search = _customerTopLevelService.GetWhereAndJoinType(request, request.OrderBy);
            return await _userInfoRepository.FindCustomerCountAsync(search);


        }

        /// <summary>
        /// 获取标签下客户数据的数量
        /// 陈亮  
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LabelCountViewModel GetLabelCount(GetCustomerListRequest request)
        {
            var vm = LabelCountViewModel.GetModel(BusinessStatusType.OK);
            var allCount = 0;
            var noReview = 0;
            //var taskOne = GetTopLabelCountAsync(request, 1);//已分配未回访
            var taskTwo = GetTopLabelCountAsync(request, 2);
            var taskThree = GetTopLabelCountAsync(request, 3);
            var taskFour = GetTopLabelCountAsync(request, 4);
            var taskFive = GetTopLabelCountAsync(request, 7);//车主询价
            var taskEight = GetTopLabelCountAsync(request, 8);//逾期客户
            var taskNine = GetTopLabelCountAsync(request, 9);//续保期未回访
            if (request.FindAllCount)
            {
                var taskAll = GetTopLabelCountAsync(request, 0);
                var taskSix = GetTopLabelCountAsync(request, 6);
                Task.WaitAll(taskAll, taskTwo, taskThree, taskFour, taskSix, taskFive, taskEight, taskNine);
                allCount = taskAll.Result;
                noReview = taskSix.Result;
            }
            else
            {
                Task.WaitAll(taskTwo, taskThree, taskFour, taskFive, taskEight, taskNine);
            }
            vm.Counts.Add(-1, allCount);
            //vm.Counts.Add(1, taskOne.Result);
            vm.Counts.Add(2, taskTwo.Result);
            vm.Counts.Add(3, taskThree.Result);
            vm.Counts.Add(4, taskFour.Result);
            vm.Counts.Add(6, noReview);
            vm.Counts.Add(7, taskFive.Result);
            vm.Counts.Add(8, taskEight.Result);
            vm.Counts.Add(9, taskNine.Result);
            return vm;
        }

        /// <summary>
        /// 获取今日计划回访下级标签的数量 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="timespan"></param>
        /// <returns></returns>
        private async Task<int> GetTodayReviewSonCountAsync(GetTodayReviewCountRequest request, int timespan)
        {
            request.LabelTimeSpan = timespan;

            var search = _customerTopLevelService.GetWhereAndJoinType(request);

            return await _userInfoRepository.FindCustomerCountAsync(search);
        }

        /// <summary>
        /// 获取今日计划回访下面的子标签的数量
        /// 陈亮 17-08-04 /pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetTodayReviewCountViewModel> GetTodayReviewCountAsync(GetTodayReviewCountRequest request)
        {
            var today = GetTodayReviewSonCountAsync(request, 0);
            var tomorrow = GetTodayReviewSonCountAsync(request, 1);
            var afterTomorrow = GetTodayReviewSonCountAsync(request, 2);
            var third = GetTodayReviewSonCountAsync(request, 3);
            var fourth = GetTodayReviewSonCountAsync(request, 4);
            var fifth = GetTodayReviewSonCountAsync(request, 5);
            var sixth = GetTodayReviewSonCountAsync(request, 6);
            var seventh = GetTodayReviewSonCountAsync(request, 7);
            var expire = GetTodayReviewSonCountAsync(request, -1);

            var todayResult = await today;
            var tomorrowResult = await tomorrow;
            var afterTomorrowResult = await afterTomorrow;
            var thirdResult = await third;
            var fourthResult = await fourth;
            var fifthResult = await fifth;
            var sixthResult = await sixth;
            var seventhResult = await seventh;
            var expireResult = await expire;

            return new GetTodayReviewCountViewModel
            {
                Today = todayResult,
                Tomorrow = tomorrowResult,
                AfterTomorrow = afterTomorrowResult,
                Third = thirdResult,
                Fourth = fourthResult,
                Fifth = fifthResult,
                Sixth = sixthResult,
                Seventh = seventhResult,
                Expire = expireResult,
                BusinessStatus = 1,
                StatusMessage = "成功"
            };

            //return await Task.Factory.StartNew(() =>
            //{
            //    var today = GetTodayReviewSonCountAsync(request, 0);
            //    var tomorrow = GetTodayReviewSonCountAsync(request, 1);
            //    var afterTomorrow = GetTodayReviewSonCountAsync(request, 2);
            //    var third = GetTodayReviewSonCountAsync(request, 3);
            //    var fourth = GetTodayReviewSonCountAsync(request, 4);
            //    var fifth = GetTodayReviewSonCountAsync(request, 5);
            //    var sixth = GetTodayReviewSonCountAsync(request, 6);
            //    var seventh = GetTodayReviewSonCountAsync(request, 7);
            //    var expire = GetTodayReviewSonCountAsync(request, -1);
            //    Task.WaitAll(today, tomorrow, afterTomorrow, third, fourth, fifth, sixth, seventh, expire);
            //    return new GetTodayReviewCountViewModel
            //    {
            //        Today = today.Result,
            //        Tomorrow = tomorrow.Result,
            //        AfterTomorrow = afterTomorrow.Result,
            //        Third = third.Result,
            //        Fourth = fourth.Result,
            //        Fifth = fifth.Result,
            //        Sixth = sixth.Result,
            //        Seventh = seventh.Result,
            //        Expire = expire.Result,
            //        BusinessStatus = 1,
            //        StatusMessage = "成功"
            //    };
            //});
        }

        /// <summary>
        /// 业务员列表批量审核 zky 2017-08-31 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool AgentBatchAudit(Messages.Request.AgentBatchAuditRequest request)
        {
            bool result = false;
            //业务员批量审核不能操作自己本身,如果包含移除
            if (request.AgentIds.Contains(request.AgentId))
            {
                request.AgentIds.Remove(request.AgentId);
            }

            var curAgent = _agentRepository.GetAgent(request.AgentId);
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    bool agentUpdate = _agentRepository.AgentBatchAudit(request.AgentIds, request.MessagePayType, request.UsedStatus, request.IsShowRate, request.IsSubmit, curAgent.zhen_bang_type);

                    ts.Complete();

                    result = agentUpdate;
                }
                catch (Exception ex)
                {
                    logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    Transaction.Current.Rollback();
                }
            }
            return result;
        }

        /// <summary>
        /// 更新业务员信息 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="request"></param>
        public BaseViewModel UpdateCustomerInfo([FromBody]UpdateCustomerRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();

            var currentAgent = _agentRepository.GetAgent(request.AgentId);
            var parentAgent = _agentRepository.GetAgent(request.ParentShareCode - 1000);
            var groupAuthen = _groupAuthenRepository.GetByAgentId(request.AgentId);
            if (currentAgent == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "代理人不存在";
                return viewModel;
            }

            //是否修改上级邀请码
            if (request.IsModifyParentShareCode)
            {
                string msg = string.Empty;
                //代理人是否可以更换上级代理
                if (_agentService.CanChangeParentAgent(currentAgent, request.ParentShareCode, out msg))
                {
                    //根据ParentShareCode获取上级代理人
                    //var parentAgent = _agentRepository.GetAgent(request.ParentShareCode - 1000);
                    if (parentAgent == null)
                    {
                        viewModel.BusinessStatus = 0;
                        viewModel.StatusMessage = "邀请码不存在，请重新输入";
                        return viewModel;
                    }
                    if (parentAgent.agent_level >= 3)
                    {
                        viewModel.BusinessStatus = 0;
                        viewModel.StatusMessage = "邀请码不正确，请重新输入";
                        return viewModel;
                    }
                    //管理员不能变更为三级代理人
                    var curRole = _managerRoleRepository.GetList(t => t.id == currentAgent.ManagerRoleId).FirstOrDefault();
                    var changeRole = _managerRoleRepository.GetList(t => t.id == request.RoleId).FirstOrDefault();
                    if (parentAgent.agent_level == 2 && (curRole.role_type == 4 || changeRole.role_type == 4))
                    {
                        viewModel.BusinessStatus = 0;
                        viewModel.StatusMessage = "请更换上级邀请码，管理员不能变更为三级代理人";
                        return viewModel;
                    }
                }
                else
                {
                    viewModel.BusinessStatus = -10009;
                    viewModel.StatusMessage = msg;
                    return viewModel;
                }
            }

            //需要更新的代理人信息
            currentAgent.IsShow = request.IsShow;
            currentAgent.IsShowCalc = request.IsShowCalc;
            currentAgent.IsUsed = request.IsUsed;
            currentAgent.MessagePayType = request.MessagePayType;
            currentAgent.ManagerRoleId = request.RoleId;
            currentAgent.IsSubmit = request.IsSubmit;

            if (request.IsModifyParentShareCode)
            {
                currentAgent.agent_level = parentAgent.agent_level + 1;
                currentAgent.platform = parentAgent.platform;
                currentAgent.ParentAgent = parentAgent.Id;
                currentAgent.repeat_quote = parentAgent.repeat_quote;
                currentAgent.AgentType = parentAgent.AgentType;
                currentAgent.accountType = parentAgent.accountType;
                currentAgent.endDate = parentAgent.endDate;
                currentAgent.commodity = parentAgent.commodity;
                currentAgent.quoteCompany = parentAgent.quoteCompany;
                currentAgent.IsUsed = parentAgent.IsUsed;
            }

            bool updateResult = _agentRepository.UpdateAgent(currentAgent);
            _agentRepository.UpdateCallMoblie(request.TopAgentId, request.AgentId, request.Mobile, request.IsGrabOrder);
            //更新表bx_group_authen的考试状态  add by qidakang 2018-04-08 17:33:00
            //如果用户是为认证状态，则向表中插入一条数据
            if (groupAuthen == null)
            {
                bx_group_authen model = new bx_group_authen()
                {
                    agentId = request.AgentId,
                    authen_state = 0,
                    bank_id = -1,
                    gender = 0,
                    is_complete_task = 0,
                    level_id = 0,
                    TestState = request.TestState
                };
                bool updateGroupAuthen = _groupAuthenRepository.AddModel(model);
            }
            else if (request.TestState != -1)
            {
                groupAuthen.TestState = request.TestState;
                bool updateGroupAuthenResult = _groupAuthenRepository.UpdateModel(groupAuthen);
            }

            if (updateResult)
            {
                if (request.IsModifyParentShareCode)
                {
                    //更新Redis
                    _agentService.UpdateAgentParentShareCodeNotifyRedis(request.AgentId, request.ParentShareCode, currentAgent.ParentAgent);
                }
                string method = string.Format("{0}/api/CacheManager/DelAgentPropertiyCache?agent={1}", _apicenter, request.AgentId);
                var clearResult = HttpWebAsk.HttpClientGetAsync(method);

                #region 当禁用业务员，提醒被禁用业务员
                if (request.IsUsed == 2)
                {
                    try
                    {
                        //禁用代理人是推送消息
                        _agentRepository.PushSignal(request.AgentId, 2);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("UpdateCustomerInfo_service:" + JsonHelper.Serialize(request) + ";【禁用业务员】推送发送异常：" + ex);
                    }
                }
                #endregion

                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "保存成功";
                return viewModel;
            }

            viewModel.BusinessStatus = 0;
            viewModel.StatusMessage = "保存失败";
            return viewModel;
        }

        /// <summary> 
        /// 查询代理人级别接口  zky 2017-12-11 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public GetAgentViewModel GetAgentLevel(int agentId)
        {
            GetAgentViewModel viewModel = new GetAgentViewModel();
            var agentModel = _agentRepository.GetList(t => t.Id == agentId).FirstOrDefault();
            if (agentModel == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "代理人不存在";
                return viewModel;
            }
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            viewModel.AgentLevel = agentModel.agent_level;
            return viewModel;
        }

        /// <summary>
        /// 删除代理人检查代理人状态  zky 2017-12-11 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public CheckAgentStatusViewModel CheckAgentStatus(int agentId)
        {
            CheckAgentStatusViewModel viewModel = new CheckAgentStatusViewModel();
            viewModel.isHasVal = 2;
            var agent = _agentRepository.GetAgent(agentId);
            if (agent == null)
            {
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "代理人不存在。";
                viewModel.IsCanDeleted = false;
                return viewModel;
            }
            if (agent.agent_level == 1)
            {
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "顶级代理人不能删除。";
                viewModel.IsCanDeleted = false;
                return viewModel;
            }
            else //非顶级代理人，判断是否有下级代理人 是否有客户数据
            {
                bool isHasChildAgent = _agentRepository.GetList(t => t.ParentAgent == agentId).Any();
                if (isHasChildAgent)//有下级代理人
                {
                    viewModel.BusinessStatus = 200;
                    viewModel.StatusMessage = "代理人有下级代理，请将下级代理人转移后再删除。";
                    viewModel.IsCanDeleted = false;
                    return viewModel;
                }
                else //没有下级代理人，
                {
                    string strId = agent.Id.ToString();
                    var hasCustomerData = _userInfoRepository.GetList(t => t.Agent == strId && t.IsTest == 0).Any();

                    //判断有没有客户数据
                    if (hasCustomerData)
                    {
                        viewModel.IsCanDeleted = false;
                        viewModel.BusinessStatus = 200;
                        viewModel.isHasVal = 1;
                        viewModel.StatusMessage = "系统检测到该用户账号名下存在客户数据，需要删除/回收数据后执行操作：是否去删除/回收数据？";
                    }
                    else
                    {
                        viewModel.IsCanDeleted = true;
                        viewModel.BusinessStatus = 200;
                        viewModel.StatusMessage = "删除该用户账号后不可恢复，该用户将无法登陆系统：确认执行？";
                    }
                    return viewModel;
                }
            }
        }

        public BaseViewModel EditAgentRate(int agentId, int companyId, double agentRate, double three, double four, int createPeople, string createPeopleName, List<NameValue> list, int isQudao, int qudaoId)
        {
            BaseViewModel viewModel = new BaseViewModel();

            int result = 0;
            using (TransactionScope ts = new TransactionScope())
            {
                if (_agentSpecialRateRepository.CheckAgentRate(agentId, companyId, isQudao, qudaoId) > 0)
                {
                    _agentSpecialRateRepository.InsertBxAgentSpecialRateLow(list, companyId, agentId, createPeople, createPeopleName, isQudao, qudaoId);
                    result = _agentSpecialRateRepository.UpdateAgentRate(agentId, companyId, agentRate, three, four, isQudao, qudaoId);
                }
                else
                {
                    _agentSpecialRateRepository.InsertBxAgentSpecialRateLow(list, companyId, agentId, createPeople, createPeopleName, isQudao, qudaoId);
                    result = _agentSpecialRateRepository.InsertAgentRate(agentId, companyId, agentRate, three, four, createPeople, createPeopleName, isQudao, qudaoId);
                }
                ts.Complete();
            }

            if (result > 0)
            {
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "保存成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "保存失败";
            }
            return viewModel;
        }

        public QueryAgentViewModel GetAgentNameByShareCode(string shareCode, int topAgentId)
        {
            QueryAgentViewModel viewModel = new QueryAgentViewModel();
            if (string.IsNullOrWhiteSpace(shareCode))
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "代理人分享码不能为空";
                return viewModel;
            }
            var agentModel = _agentRepository.GetList(t => t.ShareCode == shareCode && t.TopAgentId == topAgentId).FirstOrDefault();
            if (agentModel == null || string.IsNullOrWhiteSpace(agentModel.AgentName))
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "分享码不存在";
            }
            else
            {
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "查询成功";
                viewModel.AgentName = agentModel.AgentName;
                viewModel.agent_level = agentModel.agent_level;
            }
            return viewModel;
        }

        public QueryAgentViewModel GetShareCodeByAgentName(string agentName, int topAgentId)
        {
            QueryAgentViewModel viewModel = new QueryAgentViewModel();
            if (string.IsNullOrWhiteSpace(agentName))
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "代理人姓名不能为空";
                return viewModel;
            }
            var agentList = _agentRepository.GetList(t => t.AgentName == agentName && t.TopAgentId == topAgentId);
            if (agentList.Count > 1)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "该姓名不唯一，请手动输入上级邀请码";
                return viewModel;
            }
            var agentModel = agentList.FirstOrDefault();
            if (agentModel == null || string.IsNullOrWhiteSpace(agentModel.ShareCode))
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "业务员姓名不存在";
                return viewModel;
            }
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            viewModel.ShareCode = agentModel.ShareCode;
            return viewModel;
        }

        public CheckAgentStatusViewModel IsCanEditParentAgent(IsCanEditParentAgentRequest request)
        {
            CheckAgentStatusViewModel viewModel = new CheckAgentStatusViewModel();
            if (request.IsDaiLi == 1) //是顶级
            {
                var tuple = IsCanEditParentAgent(request.AgentId.ToString());
                viewModel.BusinessStatus = 200;
                viewModel.IsCanEdit = tuple.Item1;
                viewModel.StatusMessage = tuple.Item2;
            }
            else
            {
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "只有顶级业务员可修改其下级账号之间的上下级关系";
                viewModel.IsCanEdit = false;
            }
            return viewModel;
        }

        public Tuple<bool, string> IsCanEditParentAgent(string agentId)
        {
            int iAgentId;
            if (!int.TryParse(agentId, out iAgentId))
            {
                return new Tuple<bool, string>(false, "业务员格式错误");
            }
            //判断是否是三级
            if (_agentRepository.IsThreeAgent(iAgentId))
            {
                return new Tuple<bool, string>(true, "");
            }
            //判断是否是二级并且没有三级
            if (_agentRepository.IsTwoAgentAndNoSon(iAgentId))
            {
                return new Tuple<bool, string>(true, "");
            }
            return new Tuple<bool, string>(false, "该账号下有下级业务员，自己不能被设置为三级业务员");
        }

        public BaseViewModel ZhengBangAddUser(AddZhenBangUserRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            var curAgent = _agentRepository.GetAgent(request.AgentId); //当前代理人
            if (curAgent == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "参数错误";
                return viewModel;
            }

            string custKey = _keyCode;
            int region = int.Parse(curAgent.Region);

            bx_agent outAgent = new bx_agent();

            //调用注册接口
            var result = _loginService.Register(request.AgentAccount, request.AgentPwd.GetMd5(), request.Mobile, request.AgentName, region, curAgent.AgentType.ToString(), 0, curAgent.Id + 1000, false, 0, curAgent.AgentAddress, custKey, false, curAgent.commodity, curAgent.platform, curAgent.repeat_quote, 0, curAgent.robotCount, curAgent.agentBrand, curAgent.contractEndDate, curAgent.quoteCompany, curAgent.picc_account, curAgent.hide_phone, out outAgent);

            if (result.BusinessStatus == 1)
            {
                //获取角色列表
                var roleList = _managerRoleRepository.GetList(t => t.top_agent_id == curAgent.TopAgentId);

                //更新配置
                outAgent.MessagePayType = 1;    //从本身扣费
                outAgent.IsShowCalc = 0;        //展示微信计算器
                outAgent.IsSubmit = 1;          //可以核保
                outAgent.IsShow = 1;            //不展示费率

                if (curAgent.IsDaiLi == 1)
                {
                    if (request.ZhenBangType == 2)//机构添加网点
                    {
                        //机构添加网点类型的账号角色属于网点角色
                        outAgent.ManagerRoleId = roleList.Where(t => t.role_type == 6).FirstOrDefault().id;
                        outAgent.charge_person = request.ChargePerson;
                        outAgent.zhen_bang_type = request.ZhenBangType;
                    }
                    else if (request.ZhenBangType == 3 || request.ZhenBangType == 4) //机构注册 内部员工、外部助理
                    {
                        outAgent.zhen_bang_type = request.ZhenBangType;
                    }
                }
                else//网点添加 内部员工
                {
                    if (curAgent.zhen_bang_type == 2)
                    {
                        outAgent.zhen_bang_type = 3;
                    }
                    else
                    {
                        outAgent.zhen_bang_type = curAgent.zhen_bang_type;
                    }
                }
                _agentRepository.UpdateAgent(outAgent);
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "注册成功";

            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = result.StatusMessage;
            }
            return viewModel;
        }

        /// <summary>
        /// 启用禁用代理人账号
        /// </summary>
        /// <param name="agentId">代理人id</param>
        /// <param name="isUsed">1:启用 2：禁用</param>
        /// <returns></returns>
        public BaseViewModel EidtAgentIsUsed(List<int> agentId, int isUsed)
        {
            BaseViewModel viewModel = new BaseViewModel();
            if (agentId.Count == 0)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "请选择要修改的业务员";
                return viewModel;
            }
            string agentIds = string.Join(",", agentId.ToArray());
            _agentRepository.UpdateAgentIsUsed(agentIds, isUsed);
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "修改成功";
            return viewModel;
        }

        public BaseViewModel CanEditParentShareCode(IsCanEditParentAgentRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();

            var agentItem = _agentRepository.GetAgent(request.AgentId);
            var parentAgent = _agentRepository.GetAgent(request.ParentShareCode - 1000);
            var topAgent = _agentRepository.GetAgent(request.TopAgentId);
            if (parentAgent == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "上级邀请码错误";
                return viewModel;
            }

            if (agentItem.TopAgentId != request.TopAgentId || agentItem.TopAgentId != parentAgent.TopAgentId)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "只允许在同一机构下变更";
                return viewModel;
            }

            if (agentItem.zhen_bang_type == 3 || agentItem.zhen_bang_type == 4)
            {
                //变更的上级只能是机构或者网点
                if (parentAgent.zhen_bang_type == 1 || parentAgent.zhen_bang_type == 2)
                {
                    viewModel.BusinessStatus = 1;
                    return viewModel;
                }
                else
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "需要变更的上级只能是机构或网点";
                    return viewModel;
                }
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "网点不能变更上级邀请码";
                return viewModel;
            }
        }

        /// <summary>
        /// 1.判断是否存在刷新续保的数据
        /// 2.检查刷新续保的数量是否达到上限（60条/人/天）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CheckRefreshRenewalViewModel CheckRefreshRenewal(CheckRefreshRenewalRequest request)
        {
            CheckRefreshRenewalViewModel viewModel = new CheckRefreshRenewalViewModel();
            try
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "校验成功";

                List<bx_batchrefreshrenewal> batchList = _customerbusinessRepository.GetTodayBatchRefRenewalList(request.Agent, request.ChildAgent);

                int maxSum = 0;
                if (batchList == null || batchList.Count == 0)
                {
                    viewModel.IsRenewal = 1;
                    viewModel.RemainCount = 60;
                }
                //已经删除的数据
                int delCount = 0;
                delCount = batchList.Where(a => a.is_deleted == 1).Sum(a => a.refreshtimes);
                maxSum += delCount;
                //未删除的数据
                List<bx_batchrefreshrenewal> noDelList = batchList.Where(a => a.is_deleted == 0).ToList();

                //未删除数量
                int noDelCount = 0;
                noDelCount = noDelList.Sum(a => a.sendtimes);
                maxSum += noDelCount;
                //未续报完的数据
                int remain = 0;
                remain = noDelList.Where(a => a.refrenewalstatus == 6 || a.refrenewalstatus == 5 || a.sendtimes > a.refreshtimes).ToList().Count;

                if (maxSum >= 60)
                {
                    viewModel.IsRenewal = 2;
                    viewModel.RemainCount = 0;
                }
                else
                {
                    viewModel.IsRenewal = remain > 0 ? 2 : 1;
                    viewModel.RemainCount = 60 - maxSum;
                }

            }
            catch (Exception ex)
            {
                logError.Error("发送异常：" + ex);
                viewModel.BusinessStatus = 10003;
                viewModel.StatusMessage = "服务发生异常";
            }
            return viewModel;
        }

        /// <summary>
        /// 批量刷新续保
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>       
        public BatchRefreshRenewalViewModel BatchRefreshRenewal(BatchRefreshRenewalRequest request)
        {
            BatchRefreshRenewalViewModel viewModel = new BatchRefreshRenewalViewModel();
            try
            {
                if (request.RefreshType == 1)
                {
                    if (request.Buids == null || request.Buids.Count <= 0)
                    {
                        return new BatchRefreshRenewalViewModel() { BusinessStatus = -10000, StatusMessage = "参数错误" };
                    }
                    string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    List<long> buids = request.Buids;
                    List<long> allBuids = new List<long>();
                    allBuids.AddRange(buids);
                    //1.0判断今天是否已经存在刷新的数据
                    List<long> tempBuids = _customerbusinessRepository.GetAndUpdateBatchRefreshRenewal(allBuids, request.Agent, request.ChildAgent, date);
                    //1.1如果当天存在数据，则不进行添加
                    if (tempBuids != null && tempBuids.Count > 0)
                    {
                        allBuids.RemoveAll(a => tempBuids.Contains(a));
                    }
                    //证明本次的数据已经进行刷新过了
                    if (allBuids.Count <= 0)
                    {
                        List<BatchRefreshRenewalModel> list3 = new List<BatchRefreshRenewalModel>(); ;
                        List<bx_batchrefreshrenewal> tempList = _customerbusinessRepository.GetBatchRefreshRenewalByAllBuid(buids, request.Agent, request.ChildAgent, date);
                        foreach (bx_batchrefreshrenewal model in tempList)
                        {
                            list3.Add(new BatchRefreshRenewalModel() { licenseno = model.licenseno, buid = model.buid, refrenewalstatus = model.refrenewalstatus });
                        }
                        return new BatchRefreshRenewalViewModel() { BusinessStatus = 1, StatusMessage = "可以批量刷新续保数据", RefreshType = 1, BatchRefRenewalList = list3 };
                    }
                    //2.0将今天第一次批量续保的数据插入到表中
                    //获得userinfo数据
                    List<bx_userinfo> userList = _userInfoRepository.GetUserListByBuid(allBuids);
                    bool result = false;
                    List<BatchRefreshRenewalModel> list2 = new List<BatchRefreshRenewalModel>();
                    if (userList != null && userList.Count > 0)
                    {
                        result = _customerbusinessRepository.AddBatchRefreshRenewal(userList, request.Agent, request.ChildAgent, date);

                        List<bx_batchrefreshrenewal> tempList = _customerbusinessRepository.GetBatchRefreshRenewalByAllBuid(buids, request.Agent, request.ChildAgent, date);
                        foreach (bx_batchrefreshrenewal model in tempList)
                        {
                            list2.Add(new BatchRefreshRenewalModel() { licenseno = model.licenseno, buid = model.buid, refrenewalstatus = model.refrenewalstatus });
                        }
                    }
                    return result ? new BatchRefreshRenewalViewModel() { BusinessStatus = 1, StatusMessage = "可以批量刷新续保数据", RefreshType = 1, BatchRefRenewalList = list2 } : new BatchRefreshRenewalViewModel { BusinessStatus = 0, StatusMessage = "不能批量刷新续保数据", RefreshType = 1 };
                }
                else
                {
                    viewModel.RefreshType = 2;
                    //获得续保数据的状态，1：传buid，2：不传buid
                    List<BatchRefreshRenewalModel> list = _customerbusinessRepository.GetBatchRefreshRenewalList(request.Buids, request.Agent, request.ChildAgent);
                    return list != null || list.Count > 0 ? new BatchRefreshRenewalViewModel() { BusinessStatus = 1, RefreshType = 2, BatchRefRenewalList = list, StatusMessage = "获得续保数据的状态成功" } : new BatchRefreshRenewalViewModel { BusinessStatus = 0, RefreshType = 2, StatusMessage = "获得续保数据的状态失败" };
                }
            }
            catch (Exception ex)
            {
                logError.Error("发生异常：" + ex);
                return new BatchRefreshRenewalViewModel() { BusinessStatus = -10003, StatusMessage = "服务发生异常" };
            }
        }

        /// <summary>
        /// 详情批量刷新续保状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BatchRefreshRenewalDetailViewModel BatchRefreshRenewalDetail(BatchRefreshRenewalDetailRequest request)
        {
            BatchRefreshRenewalDetailViewModel viewModel = new BatchRefreshRenewalDetailViewModel();
            try
            {
                //没有进行批量刷新续保；进行批量刷新续保：成功、失败、排队中           

                do
                {
                    if (request.Buid <= 0)
                    {
                        viewModel.BusinessStatus = -10000;
                        viewModel.StatusMessage = "参数错误";
                        viewModel.RenewalType = 0;
                        return viewModel;
                    }

                    bx_batchrefreshrenewal model = _customerbusinessRepository.BatchRefreshRenewalDetail(request.Agent, request.ChildAgent, request.Buid);
                    if (model == null)//成功
                    {
                        viewModel.BusinessStatus = 1;
                        viewModel.RenewalType = 0;
                        viewModel.StatusMessage = "没有要续保的数据";
                        break;
                    }
                    if (model.refrenewalstatus != 5)//成功
                    {
                        viewModel.BusinessStatus = 1;
                        viewModel.RenewalType = model.refrenewalstatus;
                        viewModel.StatusMessage = "没有要续保的数据";
                        break;//成功、失败、排队中
                    }
                    Thread.Sleep(5000);
                } while (true);
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务异常";
                viewModel.RenewalType = 0;
                logError.Error("发生异常：" + ex);
            }
            return viewModel;

        }
        /// <summary>
        /// 删除排队中的数据，先判断该数据是否已经进入续保中的
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LineUpRenewalViewModel DeleteLineUpRenewal(LineUpRenewalRequest request)
        {
            LineUpRenewalViewModel viewModel = new LineUpRenewalViewModel();
            try
            {
                //1.0先判断该数据是否不在排队中,//如果在排队中可以删除
                bool result = _customerbusinessRepository.DeleteLineUpRenewal(request.Buid, request.Agent, request.ChildAgent);
                //在排队中，删除成功
                if (result)
                {
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "删除成功";
                    viewModel.RenewalType = 1;
                    return viewModel;
                }
                //已在续保中，请刷新页面
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "已经进行续保";
                viewModel.RenewalType = 2;
            }
            catch (Exception ex)
            {
                logError.Error("发生异常：" + ex);
                viewModel.BusinessStatus = 10003;
                viewModel.StatusMessage = "服务发生异常";
            }
            return viewModel;
        }
        public DistributeTimeTagViewModel GetDistributedTimeTag(int ChildAgent)
        {
            DistributeTimeTagViewModel viewModel = new DistributeTimeTagViewModel();
            try
            {
                List<DistributeTimeTagModel> list = _customerbusinessRepository.GetDistributedTimeTag(ChildAgent);
                viewModel.TagList = list;
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功";
            }
            catch (Exception ex)
            {
                logError.Error("GetDistributedTimeTag_service,ChildAgent=" + ChildAgent + "发生异常：" + ex);
                viewModel.BusinessStatus = 10003;
                viewModel.StatusMessage = "服务发生异常";
            }
            return viewModel;
        }
        public List<CustomerInfoesVM> CustomerInfoes(string mobile, int agentId)
        {
            return _customerbusinessRepository.CustomerInfoes(mobile, agentId);
        }


        /// <summary>
        /// 批改车牌时候，客户列表去重
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CorrectRepeatViewModel GetCorrectRepeatList(CorrectRepeatRequest request)
        {
            CorrectRepeatViewModel viewModel = new CorrectRepeatViewModel();
            try
            {

                #region 第二次请求
                if (request.IsAgain == 2)
                {
                    var crmMultipleDelete = int.Parse(ConfigurationManager.AppSettings["crmMultipleDelete"]);
                    CorrectRepeatViewModel viewModel2 = new CorrectRepeatViewModel();
                    viewModel2.BusinessStatus = 1;
                    viewModel2.StatusMessage = "更新成功";
                    //先删除数据，在更新对应的数据
                    List<CorrectCustomerViewModel> delList = request.CList.Where(a => a.IsDeleted == 1).ToList();
                    List<long> delBuids = delList.Select(a => a.Buid).ToList();
                    string buids = string.Join(",", delBuids);
                    _userInfoRepository.UpdateAgent(string.Join(",", delBuids), crmMultipleDelete);
                    //记录跟进记录表
                    _consumerDetailRepository.BatchAddCrmStepsByBuid(buids, long.Parse(request.ChildAgent.ToString()));

                    //dz_correct_license_plate cLPModel2 = _userInfoRepository.GetCorrectLicensePlate(request.LicenseNo, request.CarVIN);
                    dz_correct_license_plate cLPModel2 = _userInfoRepository.GetCorrectLicensePlate(request.Id);
                    //var cusModel2= request.CList.Where(a => a.IsDeleted == 2).FirstOrDefault();
                    //UpdateUserInfo(request.LicenseNo, cLPModel2, cusModel2);
                    var cusModel2 = _userInfoRepository.GetUserByVehLicense(cLPModel2.id).FirstOrDefault();
                    #region 保单信息、uk信息
                    //1.0获取保单信息
                    dz_baodanxinxi baodanModel2 = _userInfoRepository.BaoDanXinXiModelByRecDuid(cLPModel2.guid);
                    if (baodanModel2 == null)
                    {
                        viewModel.BusinessStatus = 0;
                        viewModel.StatusMessage = "没有保单信息！";
                        viewModel.BizCheckNo = "";
                        viewModel.ForceCheckNo = "";
                        return viewModel;
                    }
                    //2.0获取ukey信息
                    bx_agent_ukey uKeyModel2 = _agentUkeyRepository.GetAgentUKeyModel(baodanModel2.UKeyId.Value);
                    if (uKeyModel2 == null)
                    {
                        viewModel.BusinessStatus = 0;
                        viewModel.StatusMessage = "没有ukey信息";
                        viewModel.BizCheckNo = "";
                        viewModel.ForceCheckNo = "";
                        return viewModel;
                    }
                    #endregion
                    UpdateUserInfo(request.LicenseNo, cLPModel2, cusModel2,baodanModel2, uKeyModel2);
                    viewModel2.BizCheckNo = string.IsNullOrEmpty(cLPModel2.biz_check_no) ? "" : cLPModel2.biz_check_no;
                    viewModel2.ForceCheckNo = string.IsNullOrEmpty(cLPModel2.force_check_no) ? "" : cLPModel2.force_check_no;
                    return viewModel2;
                }
                #endregion

                #region 判断权限
                var roleType = _managerRoleRepository.GetRoleTypeByAgentId(request.ChildAgent);
                if (roleType != 3 && roleType != 4 && roleType != 5)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "该账号没有批改车牌的权限";
                    viewModel.BizCheckNo = "";
                    viewModel.ForceCheckNo = "";
                    return viewModel;
                }
                #endregion

                #region 第一次请求
                bool result = false;
                //保存车牌到台账的接口
                string url = ConfigurationManager.AppSettings["SettleModifyStateUrl"] + "/api/CorrectPlate/Untreated";
                //排序
                string queryString = "Agent=" + request.ChildAgent + "&Id=" + request.Id + "&IsCorrectSource=" + request.IsCorrectSource + "&LicenseNo=" + request.LicenseNo + "&TopAgent=" + request.Agent;
                string secCode = queryString.GetMd5();
                UntreatedModel model = new UntreatedModel() { Id = request.Id, Agent = request.ChildAgent, TopAgent = request.Agent, LicenseNo = request.LicenseNo, IsCorrectSource = request.IsCorrectSource, secCode = secCode };
                //string postResult = HttpWebReqHelper.PostHttp(url, JsonHelper.Serialize(model), "application/json");
                string statusCode = string.Empty;
                string postResult = string.Empty;
                postResult=HttpHelper.PostHttpClientAsync(url, JsonHelper.Serialize(model), out statusCode);
                LogHelper.Info("url="+url+";参数："+ JsonHelper.Serialize(model) + ";返回信息："+postResult);
                //如果失败则返回
                #region 调用院院接口失败，不在走下面的逻辑，直接返回到前端
                if (string.IsNullOrEmpty(postResult) || statusCode != "200")         
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "无批改数据";
                    viewModel.BizCheckNo = "";
                    viewModel.ForceCheckNo = "";
                    return viewModel;
                }
                BaseViewModel baseViewModel = JsonHelper.DeSerialize<BaseViewModel>(postResult);
                if (baseViewModel == null || baseViewModel.BusinessStatus != 1)
                {
                    viewModel.BusinessStatus = baseViewModel.BusinessStatus;
                    viewModel.StatusMessage = baseViewModel.StatusMessage;
                    viewModel.BizCheckNo = "";
                    viewModel.ForceCheckNo = "";
                    return viewModel;
                }
                #endregion

                //车牌号
                string licenseNo = request.LicenseNo;
                //发动机号
                string engineNo = request.EngineNo;
                //车架号
                string carVIN = request.CarVIN;
                dz_correct_license_plate cLPModel = _userInfoRepository.GetCorrectLicensePlate(request.Id);
                if (cLPModel == null || cLPModel.id == 0)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "未批改车牌信息不存在";
                    viewModel.BizCheckNo = "";
                    viewModel.ForceCheckNo = "";
                    return viewModel;
                }
                //车牌号不能为null或者空字符串
                if (string.IsNullOrEmpty(cLPModel.license_no))
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "无车牌号";
                    viewModel.BizCheckNo = "";
                    viewModel.ForceCheckNo = "";
                    return viewModel;
                }
        
                #region 保单信息、uk信息
                //1.0获取保单信息
                dz_baodanxinxi baodanModel = _userInfoRepository.BaoDanXinXiModelByRecDuid(cLPModel.guid);
                if (baodanModel == null)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "没有保单信息！";
                    viewModel.BizCheckNo = "";
                    viewModel.ForceCheckNo = "";
                    return viewModel;
                }
                //2.0获取ukey信息
                bx_agent_ukey uKeyModel = _agentUkeyRepository.GetAgentUKeyModel(baodanModel.UKeyId.Value);
                if (uKeyModel == null)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "没有ukey信息";
                    viewModel.BizCheckNo = "";
                    viewModel.ForceCheckNo = "";
                    return viewModel;
                }
                #endregion

                viewModel.ForceCheckNo = string.IsNullOrEmpty(cLPModel.force_check_no) || cLPModel.force_check_no == "0" ? "" : cLPModel.force_check_no;
                viewModel.BizCheckNo = string.IsNullOrEmpty(cLPModel.biz_check_no) || cLPModel.biz_check_no == "0" ? "" : cLPModel.biz_check_no;
                //当前代理人
                bx_agent agentModel = _agentRepository.GetAgent(request.ChildAgent);
                #region 拼接代理人参数
                //顶级下的所有代理人
                var tupleAgent = GetStringAgent(request.ChildAgent, request.Agent, roleType);
                string agentWhere = string.Empty;
                agentWhere = tupleAgent.Item2;
                if (string.IsNullOrEmpty(tupleAgent.Item2))
                {
                    if (tupleAgent.Item1 != null && tupleAgent.Item1.Count > 0 && tupleAgent.Item1[0] != -1)
                    {
                        agentWhere = string.Format(" and ui.Agent in ('{0}') ", string.Join("','", tupleAgent.Item1));
                    }
                    else
                    {
                        agentWhere = " and ui.Agent ='" + request.Agent + "' ";
                    }

                }
                #endregion
                //根据车架号、发动机号查询重复数据
                List<GetCustomerViewModel> repeatList = _userInfoRepository.GetUserByVehLicense(agentWhere, carVIN, engineNo);
                //客户列表中没有重复数据，直接新增
                if (repeatList == null || repeatList.Count == 0)
                {
                    //直接新增:cLPModel.guid
                    result = AddCorUserInfo(cLPModel, baodanModel, uKeyModel, request.Agent);
                }
                else if (repeatList.Count == 1)//客户列表中有1条数据
                {
                    Tuple<bool, List<CorrectCustomerViewModel>> temp1 = HandleSingleUser(request.Agent, licenseNo, cLPModel, agentModel, repeatList, baodanModel, uKeyModel);
                    if (temp1.Item2 != null && temp1.Item2.Count > 0)
                    {
                        viewModel.CList = temp1.Item2;
                    }
                }
                else//客户列表中有多条重复数据
                {
                    Tuple<bool, List<CorrectCustomerViewModel>> temp2 = HandleRepeatUser(request.Agent, licenseNo, cLPModel, agentModel, repeatList, baodanModel, uKeyModel);
                    if (temp2.Item2 != null && temp2.Item2.Count > 0)
                    {
                        viewModel.CList = temp2.Item2;
                    }
                }
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "更新成功";
                return viewModel;
                #endregion
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务异常";
                viewModel.BizCheckNo = "";
                viewModel.ForceCheckNo = "";
                LogHelper.Error("请求信息：" + JsonHelper.Serialize(request) + "；发生异常：" + ex);
            }
            return viewModel;
        }
        /// <summary>
        /// 拼接查询语句，查询子集代理
        /// </summary>
        /// <param name="agentId">顶级AgentId</param>
        /// <param name="topAgentId">顶级代理人Id</param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        private Tuple<List<int>, string> GetStringAgent(int agentId, int topAgentId, int roleType)
        {
            var sqlAgent = string.Empty;
            var listAgent = new List<int>();

            // 根据下级代理人的数量判断是关联bx_agent还是in
            if (HasMoreThan2000ChildAgentTopAgentId.Contains(agentId))
            {
                // 助理（roleType=5）查看的列表同顶级,当助理查询子集时需要通过顶级代理人的Id
                if (roleType == 5 || roleType == 4 || roleType == 3)
                {
                    sqlAgent = " AND bx_agent.TopAgentId=" + topAgentId;
                }
                else
                {
                    sqlAgent = string.Format(" AND (bx_agent.ParentAgent={0} OR bx_agent.Id={0}) ", agentId);
                }
            }
            else
            {
                // 助理（roleType=5）查看的列表同顶级,当助理查询子集时需要通过顶级代理人的Id
                if (roleType == 5 || roleType == 4 || roleType == 3)
                {
                    listAgent = _agentService.GetSonsListFromRedis(topAgentId);
                }
                else
                {
                    listAgent = _agentService.GetSonsListFromRedis(agentId);
                }
            }
            // 判断是否有搜索到代理人，没有的话设置为-1
            if (string.IsNullOrEmpty(sqlAgent) && !listAgent.Any())
                listAgent.Add(-1);

            return Tuple.Create(listAgent, sqlAgent);
        }
        /// <summary>
        /// 处理重复数据
        /// </summary>
        /// <param name="cLPModel"></param>
        /// <param name="agentModel"></param>
        /// <param name="repeatList"></param>
        /// <returns></returns>
        private Tuple<bool, List<CorrectCustomerViewModel>> HandleRepeatUser( int topAgentId,string licenseNo,dz_correct_license_plate cLPModel, bx_agent agentModel, List<GetCustomerViewModel> repeatList, dz_baodanxinxi baodanModel,bx_agent_ukey uKeyModel)
        {
            List<CorrectCustomerViewModel> cList = new List<CorrectCustomerViewModel>();
            bool result = false;
            //1.0以前有多条重复数据，有的有车牌，有的无车牌--只要有车牌，就不认为是未批改新车数据
            int tempCount = repeatList.Where(a =>BatchRenewalHelper.ExitCarNumber(a.LicenseNo)).ToList().Count;
            if (tempCount > 0)
            {
                //不是未批改新车数据
                return Tuple.Create(result, cList);
            }
            else
            {
                //2.0以前有多条重复数据，有的已分配，有的未分配，且台账里无业务员--判断到期时间直接更新未分配的那条
                if (cLPModel.agent_id == 0)
                {
                    List<GetCustomerViewModel> repeatList2 = repeatList.Where(a => a.IsDistributed == 0).ToList();
                    //直接更新未分配的那条
                    if (repeatList2 != null && repeatList2.Count > 0)
                    {
                        foreach (GetCustomerViewModel item in repeatList2)
                        {
                            result = UpdateUserInfo2(licenseNo, cLPModel, item, baodanModel,uKeyModel);
                        }
                    }
                    return Tuple.Create(result, cList);
                }

                List<GetCustomerViewModel> repeatList1 = repeatList.Where(a => a.Agent == cLPModel.agent_id.ToString()).ToList();
                List<bx_agent> agentList = _agentRepository.GetAgentById(repeatList1.Select(a => int.Parse(a.Agent)).ToList());
                //3.0以前有多条重复数据，有的已分配，有的未分配，且已分配的与台账里业务员一致--判断到期时间直接更新同一个业
                if (repeatList1 != null && repeatList1.Count > 0)
                {
                    //且已分配的与台账里业务员一致
                    result = UpdateUserInfo(licenseNo, cLPModel, repeatList1[0], baodanModel, uKeyModel);
                    return Tuple.Create(result, cList);
                }
                //4.0以前有多条重复数据，有的已分配，有的未分配，且已分配的与台账里业务员都不一致：
                else
                {
                    //5.0不允许重复报价--提示业务员不一致，让客户自己选择保留哪个业务员，删除其他数据
                    //0不允许重复报价、1允许重复报价、2允许二级之间重复
                    if (agentModel.repeat_quote == 0)
                    {
                        foreach (GetCustomerViewModel item in repeatList1)
                        {
                            cList.Add(new CorrectCustomerViewModel() { IsDeleted = 1, Buid = item.Id, LicenseNo = item.LicenseNo, CarVIN = item.CarVIN, ClientName = "", ClientMobile = "", MoldName = item.MoldName, RegisterDate = item.RegisterDate, LicenseOwner = item.LicenseOwner, LastReviewTime = "", AgentName = agentList.Where(a => a.Id == int.Parse(item.Agent)).FirstOrDefault().AgentName });
                        }
                    }
                    //6.0允许重复报价--直接新增一条数据，业务员为台账中的业务员，并更新其他业务员的续保信息
                    else if (agentModel.repeat_quote == 1)
                    {
                        result = AddCorUserInfo(cLPModel, baodanModel, uKeyModel, topAgentId);
                        foreach (var item in repeatList1)
                        {
                            result = UpdateUserInfo(licenseNo, cLPModel, repeatList1[0], baodanModel, uKeyModel);
                        }
                    }
                    //7.0允许二级重复报价--同一个二级下选一个业务员，不是一个二级下就新增（a2和a3需要选一个，a2和b3需要新增，a2和b2需要新增）
                    else if (agentModel.repeat_quote == 2)
                    {
                        foreach (GetCustomerViewModel cusModel in repeatList1)
                        {
                            //cList.Add(new CorrectCustomerViewModel() { Buid = item.Id, LicenseNo = item.LicenseNo, CarVIN = item.CarVIN, ClientName = "", ClientMobile = "", MoldName = item.MoldName, RegisterDate = item.RegisterDate, LicenseOwner = item.LicenseOwner, LastReviewTime = "", AgentName = agentList.Where(a => a.Id == int.Parse(item.Agent)).FirstOrDefault().AgentName });
                            foreach (var cAgent in agentList)
                            {
                                if (cAgent.agent_level == 2)
                                {
                                    //三级代理人
                                    if (cLPModel.parent_agent_id != cLPModel.top_agent_id && cLPModel.parent_agent_id > 0 && cLPModel.parent_agent_id == cAgent.Id)
                                    {
                                        //返回前端
                                        //cList.Add(new CorrectCustomerViewModel() { CId = cLPModel.id, LicenseNo = cLPModel.license_no, CarVIN = cLPModel.car_vin, ClientName = "", ClientMobile = "", MoldName = cLPModel.car_brand_model, RegisterDate = cLPModel.create_time.Value.ToString("yyyy-MM-dd HH:mm:ss"), LicenseOwner = cLPModel.car_owner, LastReviewTime = "", AgentName = cLPModel.agent_name });
                                        cList.Add(new CorrectCustomerViewModel() { IsDeleted = 1, Buid = cusModel.Id, LicenseNo = cusModel.LicenseNo, CarVIN = cusModel.CarVIN, ClientName = "", ClientMobile = "", MoldName = cusModel.MoldName, RegisterDate = cusModel.RegisterDate, LicenseOwner = cusModel.LicenseOwner, LastReviewTime = "", AgentName = cAgent.AgentName });
                                    }
                                }
                                else if (cAgent.agent_level == 3)
                                {
                                    //二级代理人
                                    if (cLPModel.parent_agent_id == cLPModel.top_agent_id && cLPModel.agent_id == cAgent.ParentAgent)
                                    {
                                        //返回前端
                                        // cList.Add(new CorrectCustomerViewModel() { CId = cLPModel.id, LicenseNo = cLPModel.license_no, CarVIN = cLPModel.car_vin, ClientName = "", ClientMobile = "", MoldName = cLPModel.car_brand_model, RegisterDate = cLPModel.create_time.Value.ToString("yyyy-MM-dd HH:mm:ss"), LicenseOwner = cLPModel.car_owner, LastReviewTime = "", AgentName = cLPModel.agent_name });
                                        cList.Add(new CorrectCustomerViewModel() { IsDeleted = 1, Buid = cusModel.Id, LicenseNo = cusModel.LicenseNo, CarVIN = cusModel.CarVIN, ClientName = "", ClientMobile = "", MoldName = cusModel.MoldName, RegisterDate = cusModel.RegisterDate, LicenseOwner = cusModel.LicenseOwner, LastReviewTime = "", AgentName = cAgent.AgentName });
                                    }
                                }
                            }
                        }
                        if (cList.Count == 0)
                        {
                            result = AddCorUserInfo(cLPModel, baodanModel, uKeyModel, topAgentId);
                        }

                    }
                }
            }
            return Tuple.Create(result, cList); ;
        }

        /// <summary>
        /// 处理单条数据
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="licenseNo"></param>
        /// <param name="cLPModel"></param>
        /// <param name="agentModel"></param>
        /// <param name="repeatList"></param>
        /// <param name="baodanModel"></param>
        /// <param name="uKeyModel"></param>
        /// <returns></returns>
        private Tuple<bool, List<CorrectCustomerViewModel>> HandleSingleUser(int topAgentId, string licenseNo, dz_correct_license_plate cLPModel, bx_agent agentModel, List<GetCustomerViewModel> repeatList, dz_baodanxinxi baodanModel, bx_agent_ukey uKeyModel)
        {
            //Tuple.Create
            List<CorrectCustomerViewModel> cList = new List<CorrectCustomerViewModel>();
            bool result = false;
            var cusModel = repeatList[0];
            //判断是否已经是车牌号，如果是，直接返回
            if (BatchRenewalHelper.ExitCarNumber(cusModel.LicenseNo))
            {
                Tuple.Create(false, cList);//该数据已经补充车牌号
            }
            //1.0以前有一条数据，但车架号发动机号与台账一致，注册日期不一致--只要行驶本信息不一致，就新增一条数据
            if (cusModel.CarVIN == cLPModel.car_vin && cusModel.EngineNo == cLPModel.car_engine_no && DateTime.Parse(cusModel.RegisterDate) != cLPModel.car_register_date.Value)
            {
                result = AddCorUserInfo(cLPModel, baodanModel, uKeyModel, topAgentId);
            }
            //2.0以前有一条数据，没有车牌号，未分配，且台账里有业务员--判断到期时间直接更新，分配给新业务员，跟进记录保留
            else if (cusModel.IsDistributed == 0 && cLPModel.agent_id > 0)//没有车牌号，未分配，且台账里有业务员
            {
                result = UpdateUserInfo(licenseNo, cLPModel, cusModel, baodanModel, uKeyModel);
            }
            else if (cusModel.IsDistributed > 0)
            {
                //3.0以前有一条数据，没有车牌号，已分配，且台账里无业务员--判断到期时间直接更新
                if (cLPModel.agent_id == 0)
                {
                    result = UpdateUserInfo2(licenseNo, cLPModel, cusModel, baodanModel, uKeyModel);
                }
                //4.0以前有一条数据，没有车牌号，已分配，且与台账里业务员一致--判断到期时间直接更新
                else if (cLPModel.agent_id.ToString() == cusModel.Agent)//且与台账里业务员一致
                {
                    //客户列表商业险到期时间 < 台账里商业险到期时间 || 客户列表交强险到期时间 < 台账交强险到期时间                    
                    result = UpdateUserInfo(licenseNo, cLPModel, cusModel, baodanModel, uKeyModel);
                }
                //5.0以前有一条数据，没有车牌号，已分配，且与台账里业务员不一致：
                else
                {
                    //0不允许重复报价、1允许重复报价、2允许二级之间重复
                    //6.0
                    if (agentModel.repeat_quote == 0)
                    {
                        //将客户列表业务员和台账业务员返回前端
                        bx_agent cAgent = _agentRepository.GetAgent(int.Parse(cusModel.Agent));
                        cList.Add(new CorrectCustomerViewModel() { Buid = cusModel.Id, LicenseNo = cusModel.LicenseNo, CarVIN = cusModel.CarVIN, ClientName = "", ClientMobile = "", MoldName = cusModel.MoldName, RegisterDate = cusModel.RegisterDate, LicenseOwner = cusModel.LicenseOwner, LastReviewTime = "", AgentName = cAgent.AgentName, IsDeleted = 1 });
                    }
                    //7.0允许重复报价时--直接新增一条数据，分配给台账中的业务员，并更新其他业务员的续保信息
                    else if (agentModel.repeat_quote == 1)
                    {
                        result = AddCorUserInfo(cLPModel, baodanModel, uKeyModel, topAgentId);
                        //更新其他业务员的续保信息
                        result = UpdateOtherUserInfo(cLPModel, cusModel, licenseNo, baodanModel, uKeyModel);
                    }
                    else if (agentModel.repeat_quote == 2)
                    {
                        //客户列表代理人和台账代理人是几级代理人
                        bx_agent cAgent = _agentRepository.GetAgent(int.Parse(cusModel.Agent));
                        if (cAgent.agent_level == 2)
                        {
                            //三级代理人
                            if (cLPModel.parent_agent_id != cLPModel.top_agent_id && cLPModel.parent_agent_id > 0 && cLPModel.parent_agent_id == cAgent.Id)
                            {
                                //返回前端
                                //cList.Add(new CorrectCustomerViewModel() { CId = cLPModel.id, LicenseNo = cLPModel.license_no, CarVIN = cLPModel.car_vin, ClientName = "", ClientMobile = "", MoldName = cLPModel.car_brand_model, RegisterDate = cLPModel.create_time.Value.ToString("yyyy-MM-dd HH:mm:ss"), LicenseOwner = cLPModel.car_owner, LastReviewTime = "", AgentName = cLPModel.agent_name });
                                cList.Add(new CorrectCustomerViewModel() { Buid = cusModel.Id, LicenseNo = cusModel.LicenseNo, CarVIN = cusModel.CarVIN, ClientName = "", ClientMobile = "", MoldName = cusModel.MoldName, RegisterDate = cusModel.RegisterDate, LicenseOwner = cusModel.LicenseOwner, LastReviewTime = "", AgentName = cAgent.AgentName, IsDeleted = 1 });
                            }
                            else
                            {
                                //直接新增
                                result = AddCorUserInfo(cLPModel, baodanModel, uKeyModel, topAgentId);
                            }
                        }
                        else if (cAgent.agent_level == 3)
                        {
                            //二级代理人
                            if (cLPModel.parent_agent_id == cLPModel.top_agent_id && cLPModel.agent_id == cAgent.ParentAgent)
                            {
                                //返回前端
                                // cList.Add(new CorrectCustomerViewModel() { CId = cLPModel.id, LicenseNo = cLPModel.license_no, CarVIN = cLPModel.car_vin, ClientName = "", ClientMobile = "", MoldName = cLPModel.car_brand_model, RegisterDate = cLPModel.create_time.Value.ToString("yyyy-MM-dd HH:mm:ss"), LicenseOwner = cLPModel.car_owner, LastReviewTime = "", AgentName = cLPModel.agent_name });
                                cList.Add(new CorrectCustomerViewModel() { Buid = cusModel.Id, LicenseNo = cusModel.LicenseNo, CarVIN = cusModel.CarVIN, ClientName = "", ClientMobile = "", MoldName = cusModel.MoldName, RegisterDate = cusModel.RegisterDate, LicenseOwner = cusModel.LicenseOwner, LastReviewTime = "", AgentName = cAgent.AgentName, IsDeleted = 1 });
                            }
                            else
                            {
                                //直接新增
                                result = AddCorUserInfo(cLPModel, baodanModel, uKeyModel, topAgentId);
                            }
                        }
                    }
                }
            }
            return Tuple.Create(result, cList);
        }

        private bool UpdateUserInfo2(string licenseNo, dz_correct_license_plate cLPModel, GetCustomerViewModel cusModel, dz_baodanxinxi baodanModel, bx_agent_ukey uKeyModel)
        {
            bool result = false;
            //客户列表商业险到期时间 < 台账里商业险到期时间 || 客户列表交强险到期时间 < 台账交强险到期时间
            if (cusModel.LastBizEndDate.Value < cLPModel.biz_end_date.Value || cusModel.LastForceEndDate.Value < cLPModel.force_end_date.Value)
            {
                //更新被保险人等信息，然后分配给新业务员
                result = UpdateCorUserInfo(cLPModel, cusModel, licenseNo, baodanModel, uKeyModel);
            }
            else
            {
                result = UpdateUserByLicenseNo(cLPModel, cusModel, licenseNo, baodanModel, uKeyModel);
            }
            return result;
        }


        private bool UpdateUserInfo(string licenseNo, dz_correct_license_plate cLPModel, GetCustomerViewModel cusModel, dz_baodanxinxi baodanModel, bx_agent_ukey uKeyModel)
        {
            bool result = false;
            //判断到期时间（哪个时间是最新的保留哪个时间）直接更新，分配给新业务员，跟进记录保留
            //客户列表商业险到期时间 < 台账里商业险到期时间 || 客户列表交强险到期时间 < 台账交强险到期时间
            if (cusModel.LastBizEndDate.Value < cLPModel.biz_end_date.Value || cusModel.LastForceEndDate.Value < cLPModel.force_end_date.Value)
            {
                //更新被保险人等信息，然后分配给新业务员
                result = UpdateCorUserInfo(cLPModel, cusModel, licenseNo, baodanModel, uKeyModel);
            }
            else
            {
                result = UpdateUserByLicenseNo(cLPModel, cusModel, licenseNo, baodanModel, uKeyModel);
            }
            return result;
        }

        /// <summary>
        /// 客户列表商业险到期时间 > 台账商业险到期时间 && 客户列表交强险到期时间 > 台账交强险到期时间
        /// </summary>
        /// <param name="recGuid"></param>
        /// <param name="buid"></param>
        /// <param name="licenseNo"></param>
        /// <returns></returns>
        private bool UpdateUserByLicenseNo(dz_correct_license_plate cLPModel, GetCustomerViewModel cusModel, string licenseNo, dz_baodanxinxi baodanModel, bx_agent_ukey uKeyModel)
        {
            //判断是大小车,当时"02"的时候是小车，其余的是大车
            int renewalCarType = baodanModel.CarLicenseTypeValue == "02" ? 0 : 1;
            //int agentId = cLPModel.agent_id>0?cLPModel.agent_id:cLPModel.top_agent_id.Value;
            int agentId = cLPModel.agent_id;
            long buid = cusModel.Id;
            //3.0获取表中是否存在数据
            bx_userinfo userInfoModel = _userInfoRepository.GetCustomerById(buid);
            userInfoModel.LicenseNo = licenseNo;
            userInfoModel.UpdateTime = DateTime.Now;
            bx_agent agentTemp = new bx_agent();
            if (agentId > 0)
            {
                userInfoModel.agent_id = agentId;
                userInfoModel.Agent = agentId.ToString();
                //判断业务员是否是顶级
                agentTemp = _agentRepository.GetAgent(agentId);
                //下级业务员更新分配信息
                if (agentTemp.agent_level > 1 && cusModel.Agent != cLPModel.agent_id.ToString())
                {
                    userInfoModel.IsDistributed = 3;
                    userInfoModel.DistributedTime = DateTime.Now;
                }
            }
            bool resultTemp = _userInfoRepository.UpdateUserInfo(userInfoModel);
            #region 记录分配跟进记录
            if (resultTemp && agentTemp.agent_level > 1 && cusModel.Agent != cLPModel.agent_id.ToString())
            {
                DistributeBackViewModel Content = new DistributeBackViewModel
                {
                    AgentId = agentId,
                    AgentName = agentTemp.AgentName,
                    OperateName = cLPModel.top_agent_name,
                    OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    OriId = agentTemp.TopAgentId,
                    OriName = "未分配"
                };
                string jsonContent = CommonHelper.TToString<DistributeBackViewModel>(Content);
                bx_crm_steps step = new bx_crm_steps
                {
                    agent_id = Convert.ToInt32(agentId),
                    b_uid = buid,
                    type = 7,
                    create_time = DateTime.Now,
                    json_content = jsonContent
                };
                _iConsumerDetailService.AddCrmSteps(step);
            }
            #endregion
            return resultTemp;
        }

        /// <summary>
        /// 更新其他业务员的续保信息
        /// </summary>
        /// <param name="cLPModel"></param>
        /// <param name="cusModel"></param>
        /// <param name="licenseNo"></param>
        /// <param name="baodanModel"></param>
        /// <param name="uKeyModel"></param>
        /// <returns></returns>
        private bool UpdateOtherUserInfo(dz_correct_license_plate cLPModel, GetCustomerViewModel cusModel, string licenseNo, dz_baodanxinxi baodanModel, bx_agent_ukey uKeyModel)
        {
            //判断是大小车,当时"02"的时候是小车，其余的是大车
            int renewalCarType = baodanModel.CarLicenseTypeValue == "02" ? 0 : 1;
            int agentId = cLPModel.agent_id;
            long buid = cusModel.Id;
            //3.0获取表中是否存在数据
            bx_userinfo userInfoModel = _userInfoRepository.GetCustomerById(buid);
            userInfoModel.LicenseNo = licenseNo;
            userInfoModel.UpdateTime = DateTime.Now;

            bool resultTemp = _userInfoRepository.UpdateUserInfo(userInfoModel);

            return resultTemp;
        }

        /// <summary>
        /// 客户列表商业险到期时间 小于 台账商业险到期时间 || 客户列表交强险到期时间 小于 台账交强险到期时间
        /// </summary>
        /// <param name="cLPModel"></param>
        /// <param name="cusModel"></param>
        /// <param name="licenseNo"></param>
        /// <param name="baodanModel"></param>
        /// <param name="uKeyModel"></param>
        /// <returns></returns>
        private bool UpdateCorUserInfo(dz_correct_license_plate cLPModel, GetCustomerViewModel cusModel, string licenseNo, dz_baodanxinxi baodanModel, bx_agent_ukey uKeyModel)
        {
            //agentId=0,台账数据无业务员
            //int agentId = cLPModel.agent_id>0?cLPModel.agent_id:cLPModel.top_agent_id.Value;
            int agentId = cLPModel.agent_id;
            long buid = cusModel.Id;
            //判断是大小车,当时"02"的时候是小车，其余的是大车
            int renewalCarType = baodanModel.CarLicenseTypeValue == "02" ? 0 : 1;
            //3.0获取表中是否存在数据
            bx_userinfo userInfoModel = _userInfoRepository.GetCustomerById(buid);
            #region 更新
            //4.0表中存在相同数据，需要更新
            userInfoModel.LicenseNo = licenseNo;
            userInfoModel.UpdateTime = DateTime.Now;
            userInfoModel.RenewalStatus = 1;
            if (uKeyModel != null && uKeyModel.city_id.HasValue)
            {
                userInfoModel.CityCode = uKeyModel.city_id.ToString();
            }
            userInfoModel.EngineNo = baodanModel.CarEngineNo;
            userInfoModel.CarVIN = baodanModel.CarVIN;
            userInfoModel.LastYearSource = baodanModel.CompanyId;
            userInfoModel.MoldName = baodanModel.CarBrandModel;
            userInfoModel.NeedEngineNo = 0;
            #region 关系人信息
            //判断关系人信息是否存在，如果存在更新到bx_userinfo
            if (!string.IsNullOrEmpty(baodanModel.CarOwner))
            {
                userInfoModel.LicenseOwner = baodanModel.CarOwner;
            }
            if (!string.IsNullOrEmpty(baodanModel.InsuredName))
            {
                userInfoModel.InsuredName = baodanModel.InsuredName;//被保险人姓名                 
            }
            //if (!string.IsNullOrEmpty(baodanModel.InsureMoblie))  modify by qdk 2018-12-06  测试说去掉被被保险人手机号
            //{
            //    userInfoModel.InsuredMobile = baodanModel.InsureMoblie;//被保险人电话
            //}
            if (!string.IsNullOrEmpty(baodanModel.InsureIdNum))
            {
                userInfoModel.InsuredIdCard = baodanModel.InsureIdNum;
            }
            if (!string.IsNullOrEmpty(baodanModel.InsureIdType))
            {
                userInfoModel.InsuredIdType = GetType(baodanModel.InsureIdType);
            }
            if (!string.IsNullOrEmpty(baodanModel.CarOwnerIdNoType))
            {
                userInfoModel.OwnerIdCardType = GetType(baodanModel.CarOwnerIdNoType);
            }
            if (baodanModel.CarOwnerIdNoType == "身份证" || baodanModel.CarOwnerIdNoType == "居民身份证")
            {
                userInfoModel.OwnerIdCardType = 1;
                int len = baodanModel.CarOwnerIdNo.Length;
                if (!string.IsNullOrEmpty(baodanModel.CarOwnerIdNo))
                {
                    userInfoModel.SixDigitsAfterIdCard = baodanModel.CarOwnerIdNo.Substring(len - 6);
                }
            }
            if (!string.IsNullOrEmpty(baodanModel.PolicyHoderName))
            {
                userInfoModel.HolderName = baodanModel.PolicyHoderName;
            }
            //if (!string.IsNullOrEmpty(baodanModel.PolicyHoderMoblie)) modify by qdk 2018-12-06  测试说去掉被被保险人手机号
            //{
            //    userInfoModel.HolderMobile = baodanModel.PolicyHoderMoblie;
            //}
            if (!string.IsNullOrEmpty(baodanModel.PolicyHoderIdNum))
            {
                userInfoModel.HolderIdCard = baodanModel.PolicyHoderIdNum;
            }
            if (!string.IsNullOrEmpty(baodanModel.PolicyHoderIdType))
            {
                userInfoModel.HolderIdType = GetType(baodanModel.PolicyHoderIdType);

            }
            //if (!string.IsNullOrEmpty(baodanModel.PolicyHoderEmail)) modify by qdk 2018-12-06  测试、产品说去掉
            //{
            //    userInfoModel.HolderEmail = baodanModel.PolicyHoderEmail;
            //}
            #endregion
            bx_agent agentTemp = new bx_agent();
            if (agentId > 0)
            {
                userInfoModel.Agent = agentId.ToString();
                userInfoModel.agent_id = agentId;
                //判断业务员是否是顶级
                agentTemp = _agentRepository.GetAgent(agentId);
                //下级业务员更新分配信息(不是顶级、与台账业务员不一致)
                if (agentTemp.agent_level > 1 && cusModel.Agent != cLPModel.agent_id.ToString())
                {
                    userInfoModel.IsDistributed = 3;
                    userInfoModel.DistributedTime = DateTime.Now;
                }
            }
            #endregion
            bool resultTemp = _userInfoRepository.UpdateUserInfo(userInfoModel);
            #region 记录分配跟进记录
            if (resultTemp && agentTemp.agent_level > 1 && cusModel.Agent != cLPModel.agent_id.ToString())
            {
                DistributeBackViewModel Content = new DistributeBackViewModel
                {
                    AgentId = agentId,
                    AgentName = agentTemp.AgentName,
                    OperateName = cLPModel.top_agent_name,
                    OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    OriId = agentTemp.TopAgentId,
                    OriName = "未分配"
                };
                string jsonContent = CommonHelper.TToString<DistributeBackViewModel>(Content);
                bx_crm_steps step = new bx_crm_steps
                {
                    agent_id = Convert.ToInt32(agentId),
                    b_uid = buid,
                    type = 7,
                    create_time = DateTime.Now,
                    json_content = jsonContent
                };
                _iConsumerDetailService.AddCrmSteps(step);
            }
            #endregion

            return resultTemp;
        }
        /// <summary>
        /// 新增批改车到客户列表
        /// </summary>
        /// <param name="recGuid"></param>
        /// <param name="agentId"></param>
        /// <param name="licenseNo"></param>
        /// <param name="dzclpId">dz_correct_license_plate.id</param>
        /// <returns></returns>
        private bool AddCorUserInfo(string recGuid, int agentId, string licenseNo = "", int dzclpId = 0)
        {
            //1.0获取保单信息
            dz_baodanxinxi baodanModel = _userInfoRepository.BaoDanXinXiModelByRecDuid(recGuid);
            if (baodanModel == null)
            {
                return false;
            }
            //2.0获取ukey信息
            bx_agent_ukey uKeyModel = _agentUkeyRepository.GetAgentUKeyModel(baodanModel.UKeyId.Value);
            if (uKeyModel == null)
            {
                return false;
            }
            //判断是大小车,当时"02"的时候是小车，其余的是大车
            int renewalCarType = baodanModel.CarLicenseTypeValue == "02" ? 0 : 1;
            //表中不存在相同数据，直接添加
            //5.0表中不存在相同数据，直接添加
            bx_userinfo userModel = new bx_userinfo();
            #region 生成对象           
            userModel.LicenseNo = string.IsNullOrEmpty(licenseNo) ? baodanModel.CarLicense : licenseNo;
            userModel.OpenId = agentId.ToString().GetMd5();
            if (uKeyModel != null && uKeyModel.city_id.HasValue)
            {
                //城市id
                userModel.CityCode = uKeyModel.city_id.Value.ToString();
            }
            userModel.EngineNo = baodanModel.CarEngineNo;
            userModel.CarVIN = baodanModel.CarVIN;
            userModel.Source = 0;
            userModel.LastYearSource = baodanModel.CompanyId;
            userModel.MoldName = baodanModel.CarBrandModel;
            userModel.RegisterDate = baodanModel.CarRegisterDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            userModel.NeedEngineNo = 0;
            userModel.CreateTime = DateTime.Now;
            userModel.UpdateTime = DateTime.Now;
            userModel.QuoteStatus = -1;
            userModel.OrderStatus = 0;
            userModel.IsReView = 0;
            userModel.Agent = agentId.ToString();
            userModel.agent_id = agentId;

            userModel.LicenseOwner = baodanModel.CarOwner;
            userModel.IsTest = 0;
            userModel.InsuredName = baodanModel.InsuredName;
            //userModel.InsuredMobile = baodanModel.InsureMoblie; modify by qdk 2018-12-06  测试说去掉被被保险人手机号
            //被保险人idcard
            userModel.InsuredIdCard = baodanModel.InsureIdNum;
            userModel.InsuredIdType = GetType(baodanModel.InsureIdType);
            userModel.IsSingleSubmit = 0;//写死0
            userModel.RenewalType = 4;
            userModel.RenewalStatus = 1;
            userModel.IsDistributed = 0;
            //车主证件类型
            //userModel.IdCard = baodanModel.CarOwnerIdNo;
            if (baodanModel.CarOwnerIdNoType == "身份证" || baodanModel.CarOwnerIdNoType == "居民身份证")
            {
                userModel.OwnerIdCardType = 1;
                int len = baodanModel.CarOwnerIdNo.Length;
                userModel.SixDigitsAfterIdCard = baodanModel.CarOwnerIdNo.Substring(len - 6);
            }
            else
            {
                userModel.OwnerIdCardType = GetType(baodanModel.CarOwnerIdNoType);
            }

            userModel.HolderName = baodanModel.PolicyHoderName;
            //userModel.HolderMobile = baodanModel.PolicyHoderMoblie; modify by qdk 2018 - 12 - 06  测试说去掉被被保险人手机号
            userModel.HolderIdCard = baodanModel.PolicyHoderIdNum;
            //投保人证件类型
            userModel.HolderIdType = GetType(baodanModel.PolicyHoderIdType);
            //userModel.HolderEmail = baodanModel.PolicyHoderEmail; modify by qdk 2018-12-06  测试说去掉被被保险人手机号
            userModel.CategoryInfoId = 0;
            userModel.IsCamera = false;
            userModel.CameraTime = DateTime.MinValue;
            userModel.DistributedTime = DateTime.MinValue;
            userModel.IsChangeRelation = 0;
            userModel.CustomerStatus = 0;
            userModel.top_agent_id = agentId == 0 ? _agentRepository.GetAgent(userModel.agent_id).TopAgentId : agentId;
            userModel.IsBatchRenewalData = 0;
            userModel.RenewalCarType = renewalCarType;//先默认后，等后面再处理
            #endregion

            //bool result = _userInfoRepository.AddUserInfo(userModel);
            bool result = _userInfoRepository.AddUserInfo(userModel, dzclpId, baodanModel);
            return result;
        }
        /// <summary>
        /// 将批改车牌的数据添加到客户列表
        /// </summary>
        /// <param name="cLPModel"></param>
        /// <param name="baodanModel"></param>
        /// <param name="uKeyModel"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        private bool AddCorUserInfo(dz_correct_license_plate cLPModel, dz_baodanxinxi baodanModel, bx_agent_ukey uKeyModel, int topAgentId)
        {
            //先判断dz_correct_license_plate.agent_id是否有值,如果有值直接使用；否则判断dz_correct_license_plate.top_agent_id是否有值，如果有值直接使用；否则使用request.Agent
            int agentId = (cLPModel.agent_id > 0) ? cLPModel.agent_id : (cLPModel.top_agent_id.HasValue && cLPModel.top_agent_id.Value > 0 ? cLPModel.top_agent_id.Value : topAgentId);
            //判断是大小车,当时"02"的时候是小车，其余的是大车
            int renewalCarType = baodanModel.CarLicenseTypeValue == "02" ? 0 : 1;
            //表中不存在相同数据，直接添加
            //5.0表中不存在相同数据，直接添加
            bx_userinfo userModel = new bx_userinfo();
            #region 生成对象           
            userModel.LicenseNo = cLPModel.license_no;//
            userModel.OpenId = agentId.ToString().GetMd5();//
            if (uKeyModel != null && uKeyModel.city_id.HasValue)
            {
                //城市id
                userModel.CityCode = uKeyModel.city_id.Value.ToString();//
            }
            userModel.EngineNo = baodanModel.CarEngineNo;//
            userModel.CarVIN = baodanModel.CarVIN;//
            userModel.Source = 0;//
            userModel.LastYearSource = baodanModel.CompanyId;//
            userModel.MoldName = baodanModel.CarBrandModel;//
            userModel.RegisterDate = baodanModel.CarRegisterDate.Value.ToString("yyyy-MM-dd HH:mm:ss");//
            userModel.NeedEngineNo = 0;//
            userModel.CreateTime = DateTime.Now;//
            userModel.UpdateTime = DateTime.Now;
            userModel.QuoteStatus = -1;//
            userModel.OrderStatus = 0;
            userModel.IsReView = 0;//
            userModel.Agent = agentId.ToString();
            userModel.agent_id = agentId;

            userModel.LicenseOwner = baodanModel.CarOwner;
            userModel.IsTest = 0;//
            userModel.InsuredName = baodanModel.InsuredName;
            //userModel.InsuredMobile = baodanModel.InsureMoblie; modify by qdk 2018-12-06  测试说去掉被被保险人手机号
            //被保险人idcard
            userModel.InsuredIdCard = baodanModel.InsureIdNum;
            userModel.InsuredIdType = GetType(baodanModel.InsureIdType);
            userModel.IsSingleSubmit = 0;//写死0//
            userModel.RenewalType = 4;//
            userModel.RenewalStatus = 1;//
            userModel.IsDistributed = agentId != topAgentId ? 3 : 0;//
            //车主证件类型 //???
            //userModel.IdCard = baodanModel.CarOwnerIdNo;
            if (baodanModel.CarOwnerIdNoType == "身份证" || baodanModel.CarOwnerIdNoType == "居民身份证")
            {
                userModel.OwnerIdCardType = 1;
                //int len = baodanModel.CarOwnerIdNo.Length;
                //userModel.SixDigitsAfterIdCard = baodanModel.CarOwnerIdNo.Substring(len - 6);
            }
            else
            {
                userModel.OwnerIdCardType = GetType(baodanModel.CarOwnerIdNoType);
            }

            userModel.HolderName = baodanModel.PolicyHoderName;
            //userModel.HolderMobile = baodanModel.PolicyHoderMoblie; modify by qdk 2018-12-06  测试说去掉被被保险人手机号
            userModel.HolderIdCard = baodanModel.PolicyHoderIdNum;
            //投保人证件类型
            userModel.HolderIdType = GetType(baodanModel.PolicyHoderIdType);
            //userModel.HolderEmail = baodanModel.PolicyHoderEmail; modify by qdk 2018-12-06  测试说去掉被被保险人手机号
            userModel.CategoryInfoId = 0;
            userModel.IsCamera = false;
            userModel.CameraTime = DateTime.MinValue;
            userModel.DistributedTime = agentId != topAgentId ? DateTime.Now : DateTime.MinValue;
            userModel.IsChangeRelation = 0;//
            userModel.CustomerStatus = 0;
            userModel.top_agent_id = topAgentId;//
            userModel.IsBatchRenewalData = 0;
            userModel.RenewalCarType = renewalCarType;//先默认后，等后面再处理//
            userModel.IsLastYear = 0;//
            userModel.LatestQuoteTime = DateTime.Now;//
            #endregion

            //bool result = _userInfoRepository.AddUserInfo(userModel);
            bool result = _userInfoRepository.AddUserInfo(userModel, cLPModel.id, baodanModel);
            return result;
        }
        
        
        /// <summary>
        /// 获得证件类型
        /// </summary>
        /// <param name="strType"></param>
        /// <returns></returns>
        private int GetType(string strType)
        {
            int result = 0;
            if (string.IsNullOrWhiteSpace(strType))
            {
                return result;
            }
            switch (strType)
            {
                case "身份证":
                    result = 1;
                    break;
                case "居民身份证":
                    result = 1;
                    break;
                case "组织机构代码证":
                    result = 2;
                    break;
                case "护照":
                    result = 3;
                    break;
                case "军官证":
                    result = 4;
                    break;
                case "港澳居民来往内地通行证":
                    result = 5;
                    break;
                case "其他":
                    result = 6;
                    break;
                case "港澳通行证":
                    result = 7;
                    break;
                case "出生证":
                    result = 8;
                    break;
                case "营业执照":
                    result = 9;
                    break;
                case "税务登记证":
                    result = 10;
                    break;
                case "港澳身份证":
                    result = 14;
                    break;
                default:
                    result = 6;
                    break;
            }
            return result;
        }

    }
}
