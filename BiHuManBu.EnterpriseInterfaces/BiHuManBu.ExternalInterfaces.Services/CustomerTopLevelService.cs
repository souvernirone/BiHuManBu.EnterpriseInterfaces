using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.Enums;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.CrmStepsService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.MapperService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using MetricsLibrary;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BiHuManBu.ExternalInterfaces.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerTopLevelService : CommonBehaviorService, ICustomerTopLevelService
    {
        private ILog logError;
        private ILog logInfo;
        private IAgentRepository _agentRepository;
        private IUserInfoRepository _userInfoRepository;
        private IRenewalInfoRepository _renewalInfoRepository;
        private readonly IAgentService _agentService;
        private readonly IUserInfoService _userInfoService;
        private readonly ICrmStepsService _crmStepsService;
        private readonly IManagerRoleRepository _managerRoleRepository;
        private readonly ICustomerBusinessRepository _customerbusinessRepository;
        private readonly IDistributedHistoryService _distributedHistoryService;
        private readonly IRecycleHistoryService _recycleHistoryService;
        private readonly ICustomerStatusRepository _customerStatusRepository;
        private readonly IAuthorityService _authorityService;
        private ICustomerListMapperService _customerListMapperService;
        private readonly IBatchRenewalRepository _batchRenewalRepository;
        private readonly IConsumerDetailService _consumerDetailService;
        private readonly ICustomerCategoriesRepository _customerCategoriesRepository;
        private readonly IUserinfoExpandService _userinfoExpandService;
        private readonly IUserinfoExpandRepository _userinfoExpandRepository;
        private readonly IAgentUkeyRepository _agentUkeyRepository;

        /// <summary>
        /// 战败和成功出单的IsReview
        /// </summary>
        private static HashSet<int> _failAndSuccessIsReview = new HashSet<int>() { 4, 9, 16 };

        /// <summary>
        /// 战败、成功出单、预约到店、未回访的IsReview
        /// </summary>
        private static HashSet<int> _failSuccessOrderArriveNoIsReview = new HashSet<int>(_failAndSuccessIsReview) { 0, 20 };
        /// <summary>
        /// 下级代理人数量超过两千的顶级代理人Id
        /// </summary>
        private static int[] HasMoreThan2000ChildAgentTopAgentId = new[] { 8031, 2668, 4245, 12457 };

        public CustomerTopLevelService(
            IUserInfoRepository userInfoRepository
            , ICacheHelper cacheHelper
            , IAgentRepository agentRepository
            , IRenewalInfoRepository renewalInfoRepository
            , IUserInfoService userInfoService
            , IAgentService agentService
            , ICrmStepsService crmStepsService
            , IManagerRoleRepository managerRoleRepository
            , ICustomerBusinessRepository customerbusinessRepository
            , IDistributedHistoryService distributedHistoryService
            , IRecycleHistoryService recycleHistoryService
            , IAuthorityService authorityService
            , ICustomerListMapperService customerListMapperService
            , ICustomerStatusRepository customerStatusRepository
            , IBatchRenewalRepository batchRenewalRepository
            , IConsumerDetailService consumerDetailService
            , ICustomerCategoriesRepository customerCategoriesRepository
            , IUserinfoExpandService userinfoExpandService
            , IUserinfoExpandRepository userinfoExpandRepository
            , IAgentUkeyRepository agentUkeyRepository
            )
            : base(agentRepository, cacheHelper)
        {
            _userInfoRepository = userInfoRepository;
            _agentRepository = agentRepository;
            _renewalInfoRepository = renewalInfoRepository;
            logError = LogManager.GetLogger("ERROR");
            logInfo = LogManager.GetLogger("INFO");
            _agentService = agentService;
            _userInfoService = userInfoService;
            _crmStepsService = crmStepsService;
            _managerRoleRepository = managerRoleRepository;
            _customerbusinessRepository = customerbusinessRepository;
            _distributedHistoryService = distributedHistoryService;
            _recycleHistoryService = recycleHistoryService;
            _authorityService = authorityService;
            _customerListMapperService = customerListMapperService;
            _customerStatusRepository = customerStatusRepository;
            _batchRenewalRepository = batchRenewalRepository;
            _consumerDetailService = consumerDetailService;
            _customerCategoriesRepository = customerCategoriesRepository;
            _userinfoExpandService = userinfoExpandService;
            _userinfoExpandRepository = userinfoExpandRepository;
            _agentUkeyRepository = agentUkeyRepository;
        }

        #region new 列表与总条数分离
        public async Task<CustomerListViewModel> GetCustomerListAsync(GetCustomerListRequest request)
        {
            var search = GetWhereByRequest2(request);
            search.PageSize = request.PageSize;
            search.CurPage = request.CurPage;
            search.OrderBy = request.OrderBy;
            // 这里写死要关联bx_consumer_review表，因为要查几个回访字段 
            search.JoinType = 1;
            var list = _userInfoRepository.FindCustomerListJoinConsumerReview(search);
            string temp = JsonHelper.Serialize(list);
            int agentLevel = request.Agent == request.ChildAgent ? 1 : 0;
            // 执行转换
            var convertVM = await _customerListMapperService.ConvertToViewModelTopLevelAsync(list, search.HasDistribute, search.CurrentAgent, agentLevel);
            var result = new CustomerListViewModel
            {
                BusinessStatus = 1,
                CustomerList = convertVM
            };
            return result;
        }

        /// <summary>
        /// 无客户电话，将该条件加入bx_userinfo_renewal_info后边
        /// </summary>
        /// <param name="IsHasClientMobile"></param>
        /// <returns></returns>
        public string clientMobileCondition(List<int> IsHasClientMobile)
        {
            string clientMobileStr = string.Empty;
            if (IsHasClientMobile != null && IsHasClientMobile.Any() && IsHasClientMobile[0] == 2)
            {
                clientMobileStr = HandleHasClientMobile(IsHasClientMobile);
            }
            return clientMobileStr;
        }

        public SearchCustomerListDto GetWhereAndJoinType(BaseCustomerSearchRequest request, int orderBy = -1)
        {
            var search = GetWhereByRequest2(request);
            var joinType = GetJoinType(request, orderBy);
            search.JoinType = joinType;
            return search;
        }

        public int GetCustomerCount(BaseCustomerSearchRequest request)
        {
            var search = GetWhereAndJoinType(request);
            return _userInfoRepository.FindCustomerCount(search);
        }

        /// <summary>
        /// 查询总条数 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DistributedCountViewModel> GetCustomerCountAsync(GetCustomerCountRequest request)
        {
            var specialDistribute = false;
            // 判断是否进行了分配条件的搜索，搜索已分配，未分配数据为0，搜索未分配，已分配数据为0
            var roleType = _managerRoleRepository.GetRoleTypeByAgentId(request.ChildAgent);
            if (roleType != 3 && roleType != 4)
            {
                // 不是系统管理员和管理员，判断是否有分配权限
                specialDistribute = _authorityService.IsHasDistributeAuth(request.ChildAgent);
            }

            var search = GetWhereAndJoinType(request);
            if (request.Buids.Any())
            {
                var buidIsDistri = _userInfoRepository.GetIsDistributed(search);
                if (specialDistribute)
                {
                    return new DistributedCountViewModel
                    {
                        TotalCount = buidIsDistri.Count,
                        DistributedCount = buidIsDistri.Where(o => o.Agent != request.ChildAgent.ToString()).Count()
                    };
                }
                else
                {
                    return new DistributedCountViewModel
                    {
                        TotalCount = buidIsDistri.Count,
                        DistributedCount = buidIsDistri.Where(o => o.IsDistributed > 0).Count()
                    };
                }
            }
            else
            {
                if (specialDistribute)
                {
                    var totalCount = await _userInfoRepository.FindCustomerCountAsync(search);
                    var distriCount = 0;
                    // 已分配的数量的重新获取
                    if (request.IsDistributed.Count == 1)
                    {
                        if (request.IsDistributed.Contains(4))
                        {
                            distriCount = totalCount;
                        }
                    }
                    else
                    {
                        request.IsDistributed = new List<int> { 4 };
                        search = GetWhereAndJoinType(request);
                        distriCount = await _userInfoRepository.FindCustomerCountAsync(search);
                    }
                    //if (!request.IsDistributed.Contains(4))
                    //{
                    //    request.IsDistributed.Add(4);
                    //}
                    //var distriCount = await _userInfoRepository.FindCustomerCountAsync(search);

                    return new DistributedCountViewModel
                    {
                        TotalCount = totalCount,
                        DistributedCount = distriCount
                    };
                }
                else
                {
                    //根据以上条件综合查询
                    var count = _userInfoRepository.FindCustomerCountContainDistributedCount(search);
                    return count;
                }
            }

        }

        /// <summary>
        /// 新的查询总条数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int GetCustomerCountNew(GetCustomerListRequest request)
        {
            //string joinWhere = string.Empty;

            ////拼接where语句
            //string strWhere = GetWhereByRequest(request, out joinWhere).ToString();
            //// 判断关联类型
            //var joinType = GetJoinType(request, request.OrderBy);

            var search = GetWhereAndJoinType(request, request.OrderBy);
            SetPage(ref search, request.PageSize, request.CurPage, request.ShowPageNum);

            //根据以上条件综合查询
            return _userInfoRepository.FindCustomerCountNew(search);
        }
        #endregion

        public void SetPage(ref SearchCustomerListDto search, int pageSize, int curPage, int showPageNum = -1)
        {
            search.PageSize = pageSize;
            search.CurPage = curPage;
            if (showPageNum != -1)
            {
                search.ShowPageNum = showPageNum;
            }
        }

        /// <summary>
        /// 顶级导出客户列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CustomerListExportViewModel ExportCustomerList(GetCustomerListRequest request)
        {
            var search = GetWhereAndJoinType(request, request.OrderBy);
            search.OrderBy = request.OrderBy;
            //根据以上条件综合查询
            var list = _userInfoRepository.FindCustomerListForExport(search);

            var viewModel = new CustomerListExportViewModel()
            {
                BusinessStatus = 1,
                TotalCount = list.Count,
            };
            if (request.OnlyCount == 0 && list.Any())
            {
                viewModel.CustomerList = _customerListMapperService.ConvertToViewModelExport(list, request.Agent, search.HasDistribute, search.CurrentAgent);
            }


            return viewModel;
        }

        public List<long> GetBuidsList(ExcuteDistributeRequest request)
        {
            var search = GetWhereAndJoinType(request, request.OrderBy);
            return _userInfoRepository.FindCustomerBuid(search);
        }
        /// <summary>
        /// 2018-07-19
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<long> GetBuidsList2(ExcuteDistributeRequest request)
        {
            var search = GetWhereAndJoinType(request, request.OrderBy);
            return _userInfoRepository.FindCustomerBuid2(search, request.AgentIds.Count, request.AverageCount, 3);
        }
        ///// <summary>
        ///// 快速查询90天内交强险到期客户
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public int GetExpiredCustomerCount(GetCustomerListRequest request)
        //{
        //    string joinWhere = string.Empty;
        //    string strWhere = GetWhereByRequest(request, 2, out joinWhere).ToString();
        //    // 判断关联类型
        //    var joinType = GetJoinType(request, 2, request.OrderBy);
        //    //根据以上条件综合查询
        //    return _userInfoRepository.FindCustomerCount(joinType, strWhere, joinWhere);
        //}

        /// <summary>
        /// 凭借代理人的搜索条件
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="currentAgentId">当前登录的代理人</param>
        /// <param name="searchAgentId">搜索的代理人</param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        private Tuple<List<int>, string> BuildAgent(int topAgentId, int currentAgentId, int searchAgentId, int roleType)
        {
            var sqlAgent = string.Empty;
            var listAgent = new List<int>();

            var sbAgent = new StringBuilder();
            if (new[] { 0, -1 }.Contains(searchAgentId))
            {
                var tuple = GetStringAgent(currentAgentId, topAgentId, roleType);
                sqlAgent = tuple.Item2;
                listAgent = tuple.Item1;
                //sbAgent.Append(GetStringAgent(currentAgentId, topAgentId));
            }
            else
            {
                listAgent.Add(searchAgentId);
                //sbAgent.Append(" AND ui.Agent ='" + searchAgentId + "' ");
            }
            return Tuple.Create(listAgent, sqlAgent);
        }

        //public StringBuilder GetWhereByRequest(BaseCustomerSearchRequest request, out string joinWhere)
        //{
        //    var search = GetWhereByRequest2(request);

        //    joinWhere = search.JoinWhere;
        //    if (search.IsReviewHashSet.Any())
        //    {
        //        search.SqlBuilder.Append(string.Format(" AND ui.IsReView in ({0}) ", string.Join(",", search.IsReviewHashSet)));
        //    }
        //    return search.SqlBuilder;
        //}

        /// <summary>
        /// GetWhereByRequest的第二个版本
        /// 这里只是组装搜索参数，不拼接SQL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SearchCustomerListDto GetWhereByRequest2(BaseCustomerSearchRequest request)
        {
            SearchCustomerListDto search = new SearchCustomerListDto();
            search.CurrentAgent = request.ChildAgent != 0 ? request.ChildAgent : request.Agent;
            StringBuilder joinBuilder = new StringBuilder();

            var roleType = _managerRoleRepository.GetRoleTypeByAgentId(search.CurrentAgent);
            if (roleType != 3 && roleType != 4)
            {
                // 不是系统管理员和管理员，判断是否有分配权限
                search.HasDistribute = _authorityService.IsHasDistributeAuth(search.CurrentAgent);
            }

            var sbWhere = new StringBuilder();
            // 拼接查询代理人
            var tupleAgent = BuildAgent(request.Agent, search.CurrentAgent, request.SearchAgentId, roleType);
            if (!string.IsNullOrEmpty(tupleAgent.Item2))
            {
                sbWhere.Append(tupleAgent.Item2);
                search.HasMoreThan2000ChildAgent = true;
            }
            // 代理人list
            var listAgent = tupleAgent.Item1;


            if (!request.Buids.Any())
            {
                #region 录入方式
                if (!string.IsNullOrEmpty(request.RenewalType) && request.RenewalType != "-1" && request.RenewalType != "0")
                {
                    search.NeedForceIndex = true;
                    var renewalTypeSql = HandRenewalType(request.RenewalType);
                    sbWhere.Append(renewalTypeSql);
                }

                #endregion

                #region 是否不取报价
                if (request.NoQuote == 1)
                {
                    search.NeedForceIndex = true;
                    //是否不取报价
                    sbWhere.Append(" AND ui.QuoteStatus=-1 ");
                }

                #endregion

                #region 是否报过价
                if (request.IsQuote.Any())
                {
                    search.NeedForceIndex = true;
                    var param = HandleIsQuote(request.IsQuote);
                    sbWhere.Append(param);
                }

                #endregion

                #region 有无客户电话
                if (request.IsHasClientMobile != null && request.IsHasClientMobile.Any())
                {
                    search.NeedForceIndex = true;
                    var param = HandleHasClientMobile(request.IsHasClientMobile);
                    sbWhere.Append(param);
                }
                #endregion

                #region 去年投保公司
                if (request.LastYearSource != "-1")
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(string.Format(" AND {1} in ({0}) ", SourceGroupAlgorithm.GetOldSources(request.LastYearSource), CompareBatchAndRenewalDateHelpler.GetLastYearSource()));
                }
                if (request.HasZhuDianYuanRole == 1 && request.LastYearSource == "-1")
                {
                    //人保
                    search.NeedForceIndex = true;
                    request.LastYearSource = "4";
                    sbWhere.Append(string.Format(" AND {1} in ({0}) ", SourceGroupAlgorithm.GetOldSources(request.LastYearSource), CompareBatchAndRenewalDateHelpler.GetLastYearSource()));
                }

                #endregion

                #region 续保状态 1=续保成功、 2=续保失败、 3=只取到行驶本、 4=未处理
                if (request.RenewalStatus != "-1" && !string.IsNullOrWhiteSpace(request.RenewalStatus))
                {
                    string str = "(";
                    if (("," + request.RenewalStatus + ",").Contains("," + 1 + ","))
                    {
                        str = str + " ui.RenewalStatus=1 ";
                        //续保成功
                        //sbWhere.Append(" AND ui.RenewalStatus=1 ");
                    }
                    if (("," + request.RenewalStatus + ",").Contains("," + 2 + ","))
                    {
                        if (str == "(")
                        {
                            str = str + " ui.NeedEngineNo=1 AND ui.RenewalStatus=0 ";
                        }
                        else
                        {
                            str = str + " OR (ui.NeedEngineNo=1 AND ui.RenewalStatus=0) ";
                        }
                        //续保失败
                        //sbWhere.Append(" AND ui.NeedEngineNo=1 AND ui.RenewalStatus=0 ");
                    }
                    if (("," + request.RenewalStatus + ",").Contains("," + 3 + ","))
                    {
                        if (str == "(")
                        {
                            str = str + " ui.NeedEngineNo=0 AND ui.RenewalStatus=0 ";
                        }
                        else
                        {
                            str = str + " OR (ui.NeedEngineNo=0 AND ui.RenewalStatus=0) ";
                        }
                        //只获取到车辆信息，未取到险种
                        //sbWhere.Append(" AND ui.NeedEngineNo=0 AND ui.RenewalStatus=0 ");
                    }
                    if (("," + request.RenewalStatus + ",").Contains("," + 4 + ","))
                    {
                        if (str == "(")
                        {
                            str = str + " ui.RenewalStatus=-1 ";
                        }
                        else
                        {
                            str = str + " OR ui.RenewalStatus=-1 ";
                        }
                        //未处理
                        //sbWhere.Append(" AND ui.RenewalStatus=-1 ");
                    }
                    if (str != "(")
                    {
                        search.NeedForceIndex = true;
                        sbWhere.Append(" AND " + str + ") ");
                    }
                }
                #endregion

                #region 客户状态

                if (request.CustomerStatus != "-1")
                {
                    search.NeedForceIndex = true;
                    var CustomerStatus = StringHandleHelper.TrimCommaAndMustInt(request.CustomerStatus);
                    if (CustomerStatus.Item1)
                    {
                        var intArr = Array.ConvertAll(CustomerStatus.Item2.Split(','), o => Convert.ToInt32(o));
                        search.IsReviewHashSet = HandleIsReivew(search.IsReviewHashSet, intArr);
                    }
                }

                #endregion

                #region  查交强险到期日期在范围内
                //2018-09-13 前端在客户列表查询接口和获取数量接口：续保期未回访时给传值DaysNum，其他接口时用到Lable3DaysNum,做下判断
                if (request.DaysNum > 0 && request.TopLabel != 9)
                {
                    search.NeedForceIndex = true;
                    var aStart = DateTime.Now;
                    var aEnd = DateTime.Now.AddDays(request.DaysNum - 1);
                    // 选了90天到期并且输入了交强险到期时间时执行这里
                    if (!string.IsNullOrWhiteSpace(request.ForceEndDateStart) &&
                        !string.IsNullOrWhiteSpace(request.ForceEndDateEnd))
                    {
                        var timeSpan = CommonHelper.CompareTime(aStart, aEnd, Convert.ToDateTime(request.ForceEndDateStart), Convert.ToDateTime(request.ForceEndDateEnd));
                        sbWhere.Append(string.Format(" AND {2} between '{0} 00:00:00' and '{1} 23:59:59' ", timeSpan.Item1.ToString("yyyy-MM-dd"), timeSpan.Item2.ToString("yyyy-MM-dd"), CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                    }
                    else
                    {
                        sbWhere.Append(string.Format(" AND {2} between '{0} 00:00:00' and '{1} 23:59:59' ", aStart.ToString("yyyy-MM-dd"), aEnd.ToString("yyyy-MM-dd"), CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                    }
                }
                else if (!string.IsNullOrWhiteSpace(request.ForceEndDateStart) &&
                        !string.IsNullOrWhiteSpace(request.ForceEndDateEnd))
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(string.Format(" AND {2} between '{0} 00:00:00' and '{1} 23:59:59' ", request.ForceEndDateStart.Trim(), request.ForceEndDateEnd.Trim(), CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                }
                #endregion

                #region 查商业险到期日期在范围内
                if (request.BizDaysNum > 0)
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(string.Format(" AND {2} between '{0} 00:00:00' and '{1} 23:59:59' ", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(request.BizDaysNum - 1).ToString("yyyy-MM-dd"), CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
                }
                else if (!string.IsNullOrWhiteSpace(request.BusinessEndDateStart) &&
                    !string.IsNullOrWhiteSpace(request.BusinessEndDateEnd))
                {
                    search.NeedForceIndex = true;
                    //查商业险到期日期在范围内
                    sbWhere.Append(
                        string.Format(" AND {2} between '{0} 00:00:00' and '{1} 23:59:59' ",
                            request.BusinessEndDateStart.Trim(), request.BusinessEndDateEnd.Trim(), CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
                }
                #endregion

                #region 创建日期在范围内
                if (!string.IsNullOrWhiteSpace(request.UpdateDateStart) &&
                    !string.IsNullOrWhiteSpace(request.UpdateDateEnd))
                {
                    search.NeedForceIndex = true;
                    //创建日期在范围内
                    sbWhere.Append(string.Format(" AND ui.UpdateTime between '{0}' and '{1}' ",
                        request.UpdateDateStart.Trim(), request.UpdateDateEnd.Trim()));
                }
                #endregion

                #region 车辆注册日期在范围内
                if (!string.IsNullOrWhiteSpace(request.RegistDateStart) &&
                    !string.IsNullOrWhiteSpace(request.RegistDateEnd))
                {
                    search.NeedForceIndex = true;
                    //车辆注册日期在范围内
                    sbWhere.Append(string.Format(" AND ui.RegisterDate between '{0}' and '{1}' ",
                        DateTime.Parse(request.RegistDateStart.Trim()).ToString("yyyy-MM-dd"), DateTime.Parse(request.RegistDateEnd.Trim()).ToString("yyyy-MM-dd")));
                }
                #endregion

                #region 查车辆品牌型号
                if (!string.IsNullOrWhiteSpace(request.MoldName))
                {
                    search.NeedForceIndex = true;
                    //查车辆品牌型号
                    sbWhere.Append(string.Format(" AND {1} like '%{0}%' ", request.MoldName, CompareBatchAndRenewalDateHelpler.GetMoldName()));
                }

                #endregion

                #region 查车架号
                if (!string.IsNullOrWhiteSpace(request.CarVIN))
                {
                    search.NeedForceIndex = true;
                    //查车架号
                    sbWhere.Append(string.Format(" AND ui.CarVIN like '%{0}%' ", request.CarVIN));
                }

                #endregion

                #region 查车牌号
                if (!string.IsNullOrWhiteSpace(request.LicenseNo))
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(string.Format(" AND ui.licenseNo like '{0}%' ", request.LicenseNo.Trim().ToUpper()));
                }

                #endregion

                #region 录回访时间查询条件范围
                if (!string.IsNullOrWhiteSpace(request.InputVisitTimeStart) &&
                    !string.IsNullOrWhiteSpace(request.InputVisitTimeEnd))
                {
                    search.NeedForceIndex = true;
                    //sbWhere.Append(string.Format(" AND ui.Id in (SELECT b_uid from bx_crm_steps where type=1 and  create_time between '{0} 00:00:00' and '{1} 23:59:59')",request.InputVisitTimeStart.Trim(), request.InputVisitTimeEnd.Trim()));

                    var visiteSql = string.Format(" AND creview.create_time between '{0} 00:00:00' and '{1} 23:59:59' ", request.InputVisitTimeStart.Trim(), request.InputVisitTimeEnd.Trim());
                    joinBuilder.Append(visiteSql);
                    // search.IsReviewHashSetNotIn.UnionWith(_failSuccessOrderArriveNoIsReview);//解决录入回访时间筛选条件与其他条件同时查， 其他筛选条件失效
                }

                #endregion

                #region 是否分配
                if (request.IsDistributed != null && request.IsDistributed.Any())
                {
                    search.NeedForceIndex = true;

                    var distributeParam = HandleDistributed(request.IsDistributed, search.CurrentAgent, ref listAgent, search.HasDistribute);
                    sbWhere.Append(distributeParam);
                }

                #endregion

                #region 批量续保的批次号
                if (request.BatchId > 0)
                {
                    search.NeedForceIndex = true;
                    //根据批次号查询
                    //sbWhere.Append(
                    //    string.Format(" AND ui.Id in (SELECT BUid FROM bx_batchrenewal_item WHERE BatchId={0} AND ItemStatus IN (1,2,4)) AND ui.IsTest=0 ",
                    //        request.BatchId));
                    var listBuid = _batchRenewalRepository.GetBuidByBatchId(request.BatchId);
                    if (listBuid.Any())
                    {
                        sbWhere.Append(string.Format(" AND ui.Id in ({0}) ", string.Join(",", listBuid)));
                    }
                    else
                    {
                        sbWhere.Append(" AND ui.Id=-1 ");
                    }
                    //sbWhere.Append(string.Format(" AND bx_batchrenewal_item.BatchId={0} ", request.BatchId));
                }
                #endregion

                #region 是否来自回收站
                if (request.FormType != -1)
                {
                    sbWhere.Append(" AND ui.IsTest=0 ");
                }
                else
                {
                    sbWhere.Append(" AND ui.isTest=3 ");
                }
                #endregion

                #region 下级代理人搜索标签 目前不用了，全部使用TopLabel
                //var childLableSearch = GetWhereSqlByLableForChildAgent(request);
                //sbWhere.Append(childLableSearch.SqlBuilder);
                //search.IsReviewHashSet = HandleIsReivew(search.IsReviewHashSet, childLableSearch.IsReviewHashSet);
                //if (childLableSearch.SqlBuilder.Length > 0 || childLableSearch.IsReviewHashSet.Any())
                //    search.NeedForceIndex = true;
                #endregion

                #region 顶级代理人搜索标签
                var topLableSearch = GetWhereSqlByLableForTopAgent(request, search.CurrentAgent, ref listAgent, search.HasDistribute);
                sbWhere.Append(topLableSearch.SqlBuilder);
                search.IsReviewHashSet = HandleIsReivew(search.IsReviewHashSet, topLableSearch.IsReviewHashSet);
                search.IsReviewHashSetNotIn.UnionWith(topLableSearch.IsReviewHashSetNotIn);

                if (topLableSearch.IsOwnerInquiry == 1)
                {
                    search.IsOwnerInquiry = 1;
                }

                if (topLableSearch.SqlBuilder.Length > 0 || topLableSearch.IsReviewHashSet.Any() || topLableSearch.IsReviewHashSetNotIn.Any())
                    search.NeedForceIndex = true;
                #endregion

                #region 分配时间
                if (!string.IsNullOrWhiteSpace(request.DistriStartTime) && !string.IsNullOrWhiteSpace(request.DistriEndTime))
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(string.Format(" and ui.DistributedTime between '{0} 00:00:00' and '{1} 23:59:59' ", request.DistriStartTime, request.DistriEndTime));
                }

                #endregion

                #region 摄像头进店时间
                if (!string.IsNullOrWhiteSpace(request.CameraStartTime) &&
                    !string.IsNullOrWhiteSpace(request.CameraEndTime))
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(string.Format(" AND ui.CameraTime between '{0}' and '{1}' ",
                        request.CameraStartTime.Trim(), request.CameraEndTime.Trim()));
                }
                #endregion 摄像头进店时间

                #region 客户类别
                if (request.ClientCategoryID != "-1" && !string.IsNullOrWhiteSpace(request.ClientCategoryID))
                {
                    search.NeedForceIndex = true;
                    var ClientCategoryID = StringHandleHelper.TrimCommaAndMustInt(request.ClientCategoryID);
                    if (ClientCategoryID.Item1)
                    {
                        sbWhere.Append(string.Format(" AND ui.CategoryInfoId in ({0}) ", ClientCategoryID.Item2));
                    }
                }
                #endregion 客户类别

                #region 客户电话号码

                if (!string.IsNullOrEmpty(request.ClientMobile))
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(
                        string.Format(" AND ui.Id in (SELECT b_uid FROM bx_userinfo_renewal_info WHERE client_mobile='{0}' OR client_mobile_other='{0}') ",
                            request.ClientMobile));
                }
                #endregion

                #region 客户名称 2018-04-17
                if (!string.IsNullOrEmpty(request.ClientName))
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(string.Format(" AND ui.Id in (SELECT b_uid FROM bx_userinfo_renewal_info WHERE client_name='{0}') ", request.ClientName));
                }
                #endregion

                #region 删除类型 2018-06-13
                if (request.DeleteType > -1)
                {
                    search.NeedForceIndex = true;
                    if (request.DeleteType == 0)
                    {
                        sbWhere.Append(string.Format(" AND (ue.delete_type IS NULL OR ue.delete_type=0)", request.DeleteType));
                    }
                    else
                    {
                        sbWhere.Append(string.Format(" AND ue.delete_type={0} ", request.DeleteType));
                    }
                    search.IsJoinExpand = 1;
                }
                #endregion

                #region 删除时间 2018-06-13

                if (!string.IsNullOrWhiteSpace(request.StartDeleteTime) && !string.IsNullOrWhiteSpace(request.EndDeleteTime))
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(string.Format(" AND ue.delete_time BETWEEN '{0}' AND '{1}' ", request.StartDeleteTime.Trim(), request.EndDeleteTime.Trim()));
                    search.IsJoinExpand = 1;
                }
                #endregion
                #region 摄像头Id
                if (string.IsNullOrWhiteSpace(request.CameraId))
                {
                    search.IsJoinExpandCameraConfig = 0;
                }
                else if (!string.IsNullOrWhiteSpace(request.CameraId) && request.CameraId != "-1")
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(string.Format(" AND ue.CameraId='{0}' ", request.CameraId.Trim()));
                    search.IsJoinExpandCameraConfig = 1;
                }
                else
                {
                    search.NeedForceIndex = true;
                    search.IsJoinExpandCameraConfig = 1;
                }
                #endregion
                #region 车主名称 2018-08-16
                if (!string.IsNullOrWhiteSpace(request.LicenseOwner))
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(string.Format(" AND ui.LicenseOwner like '{0}%' ", request.LicenseOwner.Trim()));
                }
                #endregion

                #region 按月商业险到期时间 modify qdk 2018-11-08  加时间2000-01-01是用于过滤掉默认时间
                if (request.BizEndMonth > 0)
                {
                    search.NeedForceIndex = true;
                    if (request.BizEndMonth == 1)
                    {
                        sbWhere.Append(string.Format(" AND {0}={1} and {2} > '2000-01-01'  ", CompareBatchAndRenewalDateHelpler.GetLastBizEndMonth(), request.BizEndMonth, CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
                    }
                    else
                    {
                        sbWhere.Append(string.Format(" AND {0}={1}  ", CompareBatchAndRenewalDateHelpler.GetLastBizEndMonth(), request.BizEndMonth));
                    }
                }
                #endregion

                #region 按月交强险到期时间  modify qdk 2018-11-08  加时间2000-01-01是用于过滤掉默认时间
                if (request.ForceEndMonth > 0)
                {
                    search.NeedForceIndex = true;
                    if (request.ForceEndMonth == 1)
                    {
                        sbWhere.Append(string.Format(" AND {0}={1} and {2} > '2000-01-01' ", CompareBatchAndRenewalDateHelpler.GetLastForceEndMonth(), request.ForceEndMonth, CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                    }
                    else
                    {
                        sbWhere.Append(string.Format(" AND {0}={1} ", CompareBatchAndRenewalDateHelpler.GetLastForceEndMonth(), request.ForceEndMonth));
                    }
                }
                #endregion
            }
            else
            {
                #region 摄像头Id
                if (string.IsNullOrWhiteSpace(request.CameraId))
                {
                    search.IsJoinExpandCameraConfig = 0;
                }
                else if (!string.IsNullOrWhiteSpace(request.CameraId) && request.CameraId != "-1")
                {
                    search.NeedForceIndex = true;
                    sbWhere.Append(string.Format(" AND ue.CameraId='{0}' ", request.CameraId.Trim()));
                    search.IsJoinExpandCameraConfig = 1;
                }
                else
                {
                    search.NeedForceIndex = true;
                    search.IsJoinExpandCameraConfig = 1;
                }
                #endregion
                search.NeedForceIndex = true;
                sbWhere.Append(string.Format(" AND ui.Id in ({0}) ", string.Join(",", request.Buids)));
            }

            #region 计划回访时间
            if (!string.IsNullOrWhiteSpace(request.VisitTimeStart) &&
                !string.IsNullOrWhiteSpace(request.VisitTimeEnd))
            {
                search.NeedForceIndex = true;
                //HashSet<int> set = new HashSet<int>() { 4, 5, 6, 9, 13, 14, 16, 17, 20, 21 };
                //search.IsReviewHashSet = HandleIsReivew(search.IsReviewHashSet, set);
                var visitSql = string.Format(" AND creview.next_review_date between '{0}' and '{1}' ",
                    request.VisitTimeStart.Trim(), request.VisitTimeEnd.Trim());
                joinBuilder.Append(visitSql);
            }
            #endregion

            #region 是否展示已出单的和战败数据
            search.IsReviewHashSet = DisplayFailAndSuccessIsReview(search.IsReviewHashSet, request.DisplayHandledData, search.IsReviewHashSetNotIn, request.CustomerIsReview, request.Agent);
            #endregion

            search.SqlBuilder = sbWhere;
            search.ListAgent = listAgent;
            search.JoinWhere = joinBuilder.ToString();
            return search;
        }
        #region 列表搜索条件

        /// <summary>
        /// 录入方式
        /// </summary>
        /// <param name="renewalType"></param>
        private string HandRenewalType(string renewalType)
        {
            var result = string.Empty;
            if (string.IsNullOrEmpty(renewalType) || renewalType == "-1" || renewalType == "0")
            {
                return result;
            }

            var arrRenewalType = renewalType.Split(',');
            List<string> paramList = new List<string>();
            HashSet<int> renewalTypeList = new HashSet<int>();
            foreach (var item in arrRenewalType)
            {
                switch (item)
                {
                    case "6":
                        // APP来源
                        renewalTypeList.Add(6);
                        renewalTypeList.Add(7);
                        break;
                    case "3":
                        // 摄像头
                        paramList.Add("ui.IsCamera=1");
                        break;
                    case "5":
                        // 批量续保
                        paramList.Add("ui.IsBatchRenewalData=1");
                        break;
                    default:
                        renewalTypeList.Add(Convert.ToInt32(item));
                        break;
                }
            }

            if (renewalTypeList.Any())
            {
                var renewalTypeSql = string.Format("ui.RenewalType in ({0})", string.Join(",", renewalTypeList));
                paramList.Add(renewalTypeSql);
            }

            if (paramList.Any())
            {
                if (paramList.Count == 1)
                {
                    result = " AND " + paramList.FirstOrDefault();
                }
                else
                {
                    result = " AND (" + string.Join(" or ", paramList) + ") ";
                }

            }
            return result;
        }

        /// <summary>
        /// 处理IsReview的集合
        /// 取nowIsReviewList和compareIsReviewList的交集
        /// 
        /// </summary>
        /// <param name="nowIsReviewList"></param>
        /// <param name="compareIsReviewList"></param>
        /// <returns></returns>
        public HashSet<int> HandleIsReivew(IEnumerable<int> nowIsReviewList, IEnumerable<int> compareIsReviewList)
        {
            HashSet<int> result = new HashSet<int>();
            if (!nowIsReviewList.Any())
            {
                result.UnionWith(compareIsReviewList);
            }
            else
            {
                if (compareIsReviewList.Any())
                {
                    var compared = nowIsReviewList.Intersect(compareIsReviewList);
                    result.UnionWith(compared);
                    if (!result.Any())
                    {
                        result.Add(-1);
                    }
                }
                else
                {
                    result.UnionWith(nowIsReviewList);
                }
            }
            return result;
        }

        /// <summary>
        /// 顶级代理人标签搜索
        /// </summary>
        /// <param name="request"></param>
        /// <param name="currentAgentId"></param>
        /// <param name="listAgent"></param>
        /// <param name="specialDistribute"></param>
        /// <returns></returns>
        public SearchCustomerListDto GetWhereSqlByLableForTopAgent(BaseCustomerSearchRequest request, int currentAgentId, ref List<int> listAgent, bool specialDistribute)
        {
            SearchCustomerListDto search = new SearchCustomerListDto();
            switch (request.TopLabel)
            {
                case 1:
                    search = DistributedNoReview(currentAgentId, ref listAgent, specialDistribute);
                    break;
                case 2:
                    search = PlanTodayReview(request);
                    break;
                case 3:
                    search = CameraDuringTime(request);
                    break;
                case 4:
                    search = OrderArriveStore();
                    break;
                case 5:
                    search = GetExpiredWhereSql(request, currentAgentId, ref listAgent, specialDistribute);
                    break;
                case 6:
                    var sqlPart = NoDistribute(currentAgentId, ref listAgent, specialDistribute);
                    if (!string.IsNullOrEmpty(sqlPart))
                    {
                        search.SqlBuilder.Append(string.Format(" and {0} ", sqlPart));
                    }
                    break;
                case 7://代表车主询价
                    string ownerInqStr = OwnerInquiry(request);
                    if (!string.IsNullOrEmpty(ownerInqStr))
                    {
                        search.SqlBuilder.Append(ownerInqStr);
                        search.IsOwnerInquiry = 1;
                    }
                    break;
                case 8:
                    //逾期客户
                    search = OverdueNoReview(request);
                    break;
                case 9:
                    //续保期未回访
                    search = RenewalNoReview(request, currentAgentId, ref listAgent, specialDistribute);
                    break;
            }
            return search;
        }
        /// <summary>
        /// 逾期客户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private SearchCustomerListDto OverdueNoReview(BaseCustomerSearchRequest request)
        {
            StringBuilder builder = new StringBuilder();
            var date = DateTime.Now;
            // 逾期未回访的数据
            builder.Append(string.Format(" and creview.next_review_date between '2015-01-01 ' and '{0}' ", date.ToShortDateString() + " 00:00:00"));
            SearchCustomerListDto search = new SearchCustomerListDto
            {
                SqlBuilder = builder,
                // IsReviewHashSet = new HashSet<int> { 5, 13, 14, 17 }
                // 录入回访的数据中不显示预约到店，战败成功出单的
                IsReviewHashSetNotIn = _failSuccessOrderArriveNoIsReview
            };
            return search;
        }
        /// <summary>
        /// 续保期
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <param name="listAgent"></param>
        /// <param name="specialDistribute"></param>
        /// <param name="isMastDistri">是否必须是上级分配的数据</param>
        /// <returns></returns>
        private string RenewalWhere(BaseCustomerSearchRequest request, int currentAgentId, ref List<int> listAgent, bool specialDistribute)
        {
            StringBuilder builder = new StringBuilder();
            var now = DateTime.Now;
            //这块设计到很多，前端各个接口传的值不同，做下判断！
            int lable9DaysNum = request.Label3DaysNum > 0 ? request.Label3DaysNum : request.DaysNum;
            if (lable9DaysNum > 0)
            {
                builder.Append(string.Format(" AND ({2} between '{0} 0:00:00' and '{1} 23:59:59' ", now.ToString("yyyy-MM-dd"), now.AddDays(lable9DaysNum - 1).ToString("yyyy-MM-dd"), CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                builder.Append(string.Format(" OR {2} between '{0} 0:00:00' and '{1} 23:59:59') ", now.ToString("yyyy-MM-dd"), now.AddDays(lable9DaysNum - 1).ToString("yyyy-MM-dd"), CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
            }
            if (specialDistribute)
            {
                listAgent.Remove(currentAgentId);
            }
            return builder.ToString();
        }
        /// <summary>
        /// 续保期未回访
        /// </summary>
        /// <returns></returns>
        private SearchCustomerListDto RenewalNoReview(BaseCustomerSearchRequest request, int currentAgentId, ref List<int> listAgent, bool specialDistribute)
        {
            var search = new SearchCustomerListDto();

            var hasReview = RenewalWhere(request, currentAgentId, ref listAgent, specialDistribute);
            if (!string.IsNullOrEmpty(hasReview))
            {
                search.SqlBuilder.Append(hasReview);
            }

            search.IsReviewHashSet.Add(0);
            return search;
        }
        /// <summary>
        /// 未分配条件
        /// </summary>
        /// <returns></returns>
        private string NoDistribute(int currentAgentId, ref List<int> listAgent, bool specialDistribute)
        {
            var sqlPart = string.Empty;
            if (specialDistribute)
            {
                listAgent.RemoveAll(o => o != currentAgentId);
                //listAgent.Clear();
                //listAgent.Add(currentAgentId);
            }
            else
            {
                sqlPart = " ui.IsDistributed = 0 ";
            }
            return sqlPart;
        }

        /// <summary>
        /// 车主报价
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string OwnerInquiry(BaseCustomerSearchRequest request)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(" and bue.CarOwnerStatus=1 ");
            return builder.ToString();
        }

        /// <summary>
        /// 已分配
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <param name="listAgent"></param>
        /// <param name="specialDistribute"></param>
        /// <param name="isMastDistri">是否必须是上级分配的数据</param>
        /// <returns></returns>
        private string HasDistribute(int currentAgentId, ref List<int> listAgent, bool specialDistribute, bool isMastDistri)
        {
            var sqlPart = string.Empty;
            if (isMastDistri)
            {
                sqlPart = " ui.IsDistributed = 3 ";
            }
            else
            {
                sqlPart = " ui.IsDistributed > 0 ";
            }
            if (specialDistribute)
            {
                listAgent.Remove(currentAgentId);
            }
            return sqlPart;
        }

        /// <summary>
        /// 处理是否显示战败和成功出单逻辑
        /// </summary>
        /// <param name="hashSetIsReview"></param>
        /// <param name="displayHandledData"></param>
        /// <param name="isReviewNotIn">not in 的IsReview</param>
        /// <param name="customerStatus">前端传入的IsReview</param>
        /// <param name="topAgent"></param>
        private HashSet<int> DisplayFailAndSuccessIsReview(HashSet<int> hashSetIsReview, bool displayHandledData, HashSet<int> isReviewNotIn, List<int> customerStatus, int topAgent)
        {
            if (isReviewNotIn.Any() && !hashSetIsReview.Any() && !customerStatus.Any())
            {
                // 前端没有传值就从库里获取
                customerStatus = _customerStatusRepository.GetCustomerStatus(topAgent, -1, false, true).Select(o => o.T_Id).ToList();
            }

            if (hashSetIsReview.Any())
            {
                // in 有值
                if (isReviewNotIn.Any())
                {
                    // not in 也有值，这是从in中排除not in的值
                    hashSetIsReview.ExceptWith(isReviewNotIn);
                }
            }
            else
            {
                // in没有值
                if (isReviewNotIn.Any())
                {
                    // not in有值，从全部中过滤掉not in的值
                    hashSetIsReview = new HashSet<int>(customerStatus.Except(isReviewNotIn));
                }
            }


            // 这里只处理不显示战败和出单的
            if (!displayHandledData)
            {
                if (hashSetIsReview.Any())
                {
                    // 移除战败和出单
                    hashSetIsReview.ExceptWith(_failAndSuccessIsReview);
                }
                else
                {
                    // 这里是没有筛选IsReview的情况，从全部中去掉了已出单的情况
                    var listIsReview = customerStatus.Except(_failAndSuccessIsReview);
                    hashSetIsReview = new HashSet<int>(listIsReview);
                }
            }
            return hashSetIsReview;
        }

        /// <summary>
        /// 是否分配
        /// </summary>
        /// <param name="isDistributed"></param>
        /// <param name="currentAgentId"></param>
        /// <param name="listAgent">代理人集合</param>
        /// <param name="specialDistribute">是否特殊处理分配搜索</param>
        /// <returns></returns>
        private string HandleDistributed(List<int> isDistributed, int currentAgentId, ref List<int> listAgent, bool specialDistribute)
        {
            var result = string.Empty;
            if (isDistributed == null || !isDistributed.Any())
                return result;
            // 已分配和未分配同时搜索时不做处理
            if (isDistributed.Contains(1) && isDistributed.Contains(4))
                return string.Empty;

            List<string> paramList = new List<string>();
            foreach (var item in isDistributed)
            {
                switch (item)
                {
                    case 1:
                        // 未分配
                        var sqlPart = NoDistribute(currentAgentId, ref listAgent, specialDistribute);
                        if (!string.IsNullOrEmpty(sqlPart))
                        {
                            paramList.Add(sqlPart);
                        }
                        break;
                    case 4:
                        // 已分配 
                        var distri = HasDistribute(currentAgentId, ref listAgent, specialDistribute, false);
                        if (!string.IsNullOrEmpty(distri))
                        {
                            paramList.Add(distri);
                        }
                        break;
                }
            }

            if (paramList.Any())
            {
                if (paramList.Count() == 1)
                {
                    result = string.Format(" and {0} ", paramList.FirstOrDefault());
                }
                else
                {
                    result = string.Format(" and ({0}) ", string.Join(" or ", paramList));
                }
            }
            return result;
        }

        /// <summary>
        /// 处理是否报过价
        /// </summary>
        /// <param name="isQuote"></param>
        /// <returns></returns>
        private string HandleIsQuote(List<int> isQuote)
        {
            var result = string.Empty;
            if (isQuote != null && isQuote.Any())
            {
                //当存在已报价和未报价，则不加条件
                if (isQuote.Count >= 2)
                {
                    return result;
                }
                List<string> paramList = new List<string>();
                foreach (var item in isQuote)
                {
                    switch (item)
                    {
                        case 1:
                            paramList.Add(" ui.QuoteStatus>-1 ");
                            break;
                        case 2:
                            paramList.Add(" IFNULL( ui.QuoteStatus,-1)=-1 ");
                            break;
                    }
                }
                if (paramList.Any())
                    result = string.Format(" and ({0}) ", string.Join(" or ", paramList));
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 处理有无电话号码
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string HandleHasClientMobile(List<int> list)
        {
            string result = string.Empty;
            if (list == null || list.Count == 0 || list.Count >= 2)
            {
                return result;
            }
            if (list[0] == 1)
            {
                return result = " AND ui.Id in (SELECT b_uid FROM bx_userinfo_renewal_info WHERE client_mobile<>'' AND client_mobile IS NOT NULL) ";
            }
            return result = " AND (bx_userinfo_renewal_info.client_mobile ='' OR bx_userinfo_renewal_info.client_mobile IS NULL) ";
            //return result = " AND ui.Id in (SELECT b_uid FROM bx_userinfo_renewal_info WHERE client_mobile='' OR client_mobile IS NULL) "; ;
        }

        #region 标签搜索类型

        /// <summary>
        /// 0-90天内到期（未分配）拼接sql
        /// 根据商业险判断0-90天
        /// </summary>
        /// <param name="request"></param>
        /// <param name="currentAgentId"></param>
        /// <param name="listAgent"></param>
        /// <param name="specialDistribute"></param>
        /// <returns></returns>
        private SearchCustomerListDto GetExpiredWhereSql(BaseCustomerSearchRequest request, int currentAgentId, ref List<int> listAgent, bool specialDistribute)
        {
            SearchCustomerListDto search = new SearchCustomerListDto();

            var noDistri = NoDistribute(currentAgentId, ref listAgent, specialDistribute);
            if (!string.IsNullOrEmpty(noDistri))
            {
                search.SqlBuilder.Append(string.Format(" AND {0} ", noDistri));
            }

            return search;
        }

        /// <summary>
        /// 已分配未回访
        /// </summary>
        /// <returns></returns>
        private SearchCustomerListDto DistributedNoReview(int currentAgentId, ref List<int> listAgent, bool specialDistribute)
        {
            var search = new SearchCustomerListDto();

            var hasDistri = HasDistribute(currentAgentId, ref listAgent, specialDistribute, true);
            if (!string.IsNullOrEmpty(hasDistri))
            {
                search.SqlBuilder.Append(string.Format(" AND {0}", hasDistri));
            }

            search.IsReviewHashSet.Add(0);
            return search;
        }

        /// <summary>
        /// 计划今日回访
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private SearchCustomerListDto PlanTodayReview(BaseCustomerSearchRequest request)
        {
            StringBuilder builder = new StringBuilder();
            var date = DateTime.Now;
            if (request.LabelTimeSpan == -1)
            {
                // 预期未回访的数据
                builder.Append(string.Format(" and creview.next_review_date between '2015-01-01 ' and '{0}' ", date.ToShortDateString() + " 00:00:00"));
            }
            else
            {
                var timespan = date.AddDays(request.LabelTimeSpan).ToShortDateString();
                builder.Append(string.Format(" and creview.next_review_date between '{0}' and '{1}' ",
                    timespan + " 0:00:00"
                    , timespan + " 23:59:59"
                    ));
            }

            SearchCustomerListDto search = new SearchCustomerListDto
            {
                SqlBuilder = builder,
                // IsReviewHashSet = new HashSet<int> { 5, 13, 14, 17 }
                // 录入回访的数据中不显示预约到店，战败成功出单的
                IsReviewHashSetNotIn = _failSuccessOrderArriveNoIsReview
            };
            return search;
        }

        /// <summary>
        /// 今日进店（续保期）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private SearchCustomerListDto CameraDuringTime(BaseCustomerSearchRequest request)
        {
            StringBuilder builder = new StringBuilder();
            var now = DateTime.Now;
            builder.Append(string.Format(" and ui.CameraTime between '{0}' and '{1}' ", now.Date.ToString(), now.ToShortDateString() + " 23:59:59"));
            if (request.Label3DaysNum > 0)
            {
                builder.Append(string.Format(" AND ({2} between '{0} 0:00:00' and '{1} 23:59:59' ", now.ToString("yyyy-MM-dd"), now.AddDays(request.Label3DaysNum - 1).ToString("yyyy-MM-dd"), CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                builder.Append(string.Format(" OR {2} between '{0} 0:00:00' and '{1} 23:59:59') ", now.ToString("yyyy-MM-dd"), now.AddDays(request.Label3DaysNum - 1).ToString("yyyy-MM-dd"), CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
            }

            SearchCustomerListDto search = new SearchCustomerListDto
            {
                //IsReviewHashSet = new HashSet<int> { 0, 5, 6, 13, 14, 17, 20 },
                // 不显示战败和成功出单
                IsReviewHashSetNotIn = _failAndSuccessIsReview,
                SqlBuilder = builder
            };
            return search;
        }

        /// <summary>
        /// 预约到店
        /// </summary>
        /// <returns></returns>
        private SearchCustomerListDto OrderArriveStore()
        {
            return new SearchCustomerListDto
            {
                IsReviewHashSet = new HashSet<int> { 20 }
            };
        }
        #endregion

        /// <summary>
        /// 获取摄像头新进店数量
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetCountLoopViewModel GetCountLoop(GetCustomerListRequest request)
        {
            var result = GetCountLoopViewModel.GetModel(BusinessStatusType.OK);

            int agentId = 0;
            if (request.RoleType == 4)
            {
                agentId = request.Agent;
            }
            else
            {
                agentId = request.ChildAgent != 0 ? request.ChildAgent : request.Agent;
            }

            var listAgent = _agentService.GetSonsListFromRedisToString(agentId);
            //20170218增加需求：结果返回到期天数要求
            List<long> buids = _userInfoRepository.FindBuidLoop(request.UpdateDateStart, request.UpdateDateEnd, agentId, 3, listAgent);
            if (buids.Count == 0)
                return result;

            var renewals = _renewalInfoRepository.GetCarRenewals(buids, request.DaysNum, request.BizDaysNum);

            renewals = renewals.DistinctBy(o => o.LicenseNo).ToList();

            result.Count = renewals.Count;
            result.Buids = string.Join(",", renewals.Select(o => o.Buid));

            if (result.Count == 1)
            {
                var first = renewals.FirstOrDefault();
                var now = DateTime.Now;
                // 有可能时间是1900 和0001/1/1 0:00:00
                if (new int[] { 0001, 1900, 1970 }.Contains(first.LastBizEndDate.Year))
                {
                    first.DaysNum = (first.LastForceEndDate - now).Days;
                }
                else if (new int[] { 0001, 1900, 1970 }.Contains(first.LastForceEndDate.Year))
                {
                    first.DaysNum = (first.LastBizEndDate - now).Days;
                }
                else
                {
                    first.DaysNum = Math.Min((first.LastBizEndDate - now).Days
                    , (first.LastForceEndDate - now).Days);
                }
                // 脱保的车不提醒
                if (first.DaysNum < 0)
                {
                    result.Buids = string.Empty;
                    result.Count = 0;
                    result.RenewalInfo = null;
                    return result;
                }
                result.RenewalInfo = first;
            }
            return result;
        }

        /// <summary>
        /// 拼接查询语句，查询子集代理
        /// </summary>
        /// <param name="agentId">顶级AgentId</param>
        /// <param name="topAgentId">顶级代理人Id</param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public Tuple<List<int>, string> GetStringAgent(int agentId, int topAgentId, int roleType)
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
        /// 查询业务员出单员的子级
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public List<int> GetListAgent(int agentId, int topAgentId)
        {
            #region 搜索Agent
            //查自己及下级，针对顶级
            var sonIds = new List<int>();
            //Stopwatch sw_agent = new Stopwatch();
            //sw_agent.Start();
            // 获取agentId对应的角色类型

            var agentRoleType = _managerRoleRepository.GetRoleTypeByAgentId(agentId);

            //助理（roleType=5）查看的列表同顶级,当助理查询子集时需要通过顶级代理人的Id
            if (agentRoleType == 5 || agentRoleType == 4 || agentRoleType == 3)
            {
                var lstStr = _agentService.GetSonsListFromRedisToString(topAgentId);
                foreach (var item in lstStr)
                {
                    sonIds.Add(Convert.ToInt32(item));
                }
            }
            else
            {
                var lstStr = _agentService.GetSonsListFromRedisToString(agentId);
                foreach (var item in lstStr)
                {
                    sonIds.Add(Convert.ToInt32(item));
                }
            }
            return sonIds;
            #endregion
        }

        public int GetJoinType(BaseCustomerSearchRequest request, int orderBy = -1)
        {
            var result = 0;
            if ((!string.IsNullOrEmpty(request.VisitTimeEnd) && !string.IsNullOrEmpty(request.VisitTimeStart))
                || request.TopLabel == 2
                || orderBy == 4 || orderBy == 41
                || request.TopLabel == 2
                || request.TopLabel == 8
                || (!string.IsNullOrEmpty(request.InputVisitTimeEnd) && !string.IsNullOrEmpty(request.InputVisitTimeStart))
                )
            {
                result = 1;
            }
            return result;
        }

        /// <summary>
        /// crm用户批量删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel MultipleDelete(MultipleDeleteRequest request)
        {
            var listBuid = new List<long>();
            var listAgent = new List<string>();
            // 判断删除类型，区分客户列表和回收站
            var deleteType = (DeleteType)Enum.Parse(typeof(DeleteType), request.DeleteType);

            // 是否是全部
            if (request.IsAll == 1)
            {
                if (deleteType == DeleteType.Soft)
                {
                    var search = GetWhereAndJoinType(request);
                    var listBuidAgent = _userInfoRepository.FindCustomerBuidAndAgent(search);
                    listBuid = listBuidAgent.Select<UserInfoIdAgentModel, long>(a => a.Id).ToList();
                    listAgent = listBuidAgent.Select<UserInfoIdAgentModel, string>(a => a.Agent).ToList().Distinct().ToList();
                    //listBuid = _userInfoRepository.FindCustomerBuid(search);
                }
                else if (deleteType == DeleteType.Thorough)
                {
                    // 回收站，找出顶级下所有回收站的数据
                    var buidAgentList = _userInfoService.GetBuidsByTopAgentAndIsTest(request.Agent, 3);
                    listBuid = buidAgentList.Select<UserInfoIdAgentModel, long>(a => a.Id).ToList();
                    listAgent = buidAgentList.Select<UserInfoIdAgentModel, string>(a => a.Agent).ToList().Distinct().ToList();
                }
            }
            else
            {
                listBuid = request.Buids;
            }

            if (listBuid.Count == 0)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            if (listBuid.Count > 20000)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "每次删除的数量不要超过20000");
            }
            var strBuids = string.Join(",", listBuid);
            var strAgents = string.Empty;
            if (listAgent.Count > 0)
            {
                strAgents = "'" + string.Join("','", listAgent) + "'";
            }

            LogHelper.Info("删除操作，记录删除的buid\r\n请求参数:" + request.ToJson() + "\r\n删除的buid：\r\n" + strBuids);

            var result = false;
            switch (deleteType)
            {
                case DeleteType.Soft:
                    result = UpdateIsTestAndInsertIntoSteps(listBuid, strBuids, request.ChildAgent, 3, strAgents);
                    break;
                case DeleteType.Thorough:
                    // 彻底删除，就是放入公司自己的代理人下面
                    result = MoveToOurAgentAndInsertIntoSteps(listBuid, request.Agent);
                    break;
                default:
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "删除类型错误");
            }

            if (result)
            {
                return BaseViewModel.GetBaseViewModel(1, "删除成功");
            }
            else
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "删除失败");
            }
        }
        /// <summary>
        /// 将Buids循环拼接
        /// </summary>
        /// <param name="listBuid"></param>
        /// <returns></returns>
        private List<string> CycleBuid(List<long> listBuid)
        {
            List<string> cycleBuids = new List<string>();
            if (listBuid.Count < 5000)
            {
                cycleBuids.Add(string.Join(",", listBuid));
                return cycleBuids;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < listBuid.Count; i++)
            {
                sb.Append(listBuid[i]).Append(",");
                if ((i + 1) % 5000 == 0)
                {
                    cycleBuids.Add(sb.ToString().TrimEnd(','));
                    sb.Clear();
                    continue;
                }
                if ((i + 1) == listBuid.Count)
                {
                    cycleBuids.Add(sb.ToString().TrimEnd(','));
                    sb.Clear();
                }
            }
            return cycleBuids;
        }
        /// <summary>
        /// 只判断是客户列表删除还是回收站
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel MultipleDeleteUserInfo(MultipleDeleteRequest request)
        {
            // 判断删除类型，区分客户列表和回收站
            var deleteType = (DeleteType)Enum.Parse(typeof(DeleteType), request.DeleteType);
            if (deleteType == DeleteType.Soft)
            {
                return MultipleDelete(request);//只处理软删除
            }
            else
            {
                return RecycleBinMultipleDelete(request);//处理回收站
            }
        }

        /// <summary>
        /// crm用户批量删除:回收站
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel RecycleBinMultipleDelete(MultipleDeleteRequest request)
        {
            var result = false;
            //获取代理人集合
            var listAgent = _userInfoService.GetBuidsByTopAgentAndIsTest2(request.Agent, 3);//不能放在事务里面
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var strAgents = string.Empty;
                    if (listAgent != null && listAgent.Count > 0)
                    {
                        var crmMultipleDelete = int.Parse(ConfigurationManager.AppSettings["crmMultipleDelete"]);
                        strAgents = "'" + string.Join("','", listAgent) + "'";
                        //添加到记录表
                        bool isCrmSteps = _crmStepsService.ClearRecycleBinAddSteps(3, strAgents);
                        result = _userInfoRepository.UpdateAgent(crmMultipleDelete, strAgents);
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    MetricUtil.UnitReports("RecycleBinMultipleDelete_service");
                    logError.Error("发生异常:清空回收站失败 \n" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" +
                                  ex.InnerException + "\n" + request.ToJson());
                }
                finally
                {
                    scope.Dispose();
                }
            }

            if (result)
            {
                return BaseViewModel.GetBaseViewModel(1, "删除成功");
            }
            else
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "删除失败");
            }
        }

        public bool MoveToOurAgentAndInsertIntoSteps(List<long> listBuid, int agentId)
        {
            var buids = string.Join(",", listBuid);
            var crmMultipleDelete = int.Parse(ConfigurationManager.AppSettings["crmMultipleDelete"]);
            var result = _userInfoRepository.UpdateAgent(buids, crmMultipleDelete);
            if (result)
            {
                _crmStepsService.DeleteUserInfoAddSteps(listBuid, agentId);
                try
                {
                    _userInfoRepository.UpdateReviewdetailRecord(buids);
                }
                catch (Exception ex)
                {
                    MetricUtil.UnitReports("MoveToOurAgentAndInsertIntoSteps_UpdateReviewdetailRecord_service");
                    logError.Error("MoveToOurAgentAndInsertIntoSteps_service;" + "Buids=" + buids + "发生异常：" + ex);
                }
            }
            return result;
        }
        /// <summary>
        /// 删除后记录到步骤表
        /// </summary>
        /// <param name="listBuid"></param>
        /// <param name="strBuids"></param>
        /// <param name="agentId"></param>
        /// <param name="isTest"></param>
        /// <returns></returns>
        public bool UpdateIsTestAndInsertIntoSteps(List<long> listBuid, string strBuids, int agentId, int isTest, string strAgents)
        {
            bool result = false;
            foreach (var partStrBuids in CycleBuid(listBuid))
            {
                result = _userInfoRepository.UpdateIsTest(partStrBuids, agentId, 3, strAgents);
                var tempList = partStrBuids.Split(',').Select(a => long.Parse(a)).ToList();
                if (result)
                {
                    try
                    {
                        _crmStepsService.DeleteUserInfoAddSteps(tempList, agentId);
                    }
                    catch (Exception ex)
                    {
                        MetricUtil.UnitReports("DeleteUserInfoAddSteps_service");
                        logError.Error("【批量删除】插入到记录表失败： " + ex.Message + partStrBuids);
                    }

                    try
                    {
                        //插入到扩展表，记录删除类型和删除时间
                        AddUserInfoExpandList(tempList, partStrBuids, 0);
                    }
                    catch (Exception ex)
                    {
                        MetricUtil.UnitReports("AddUserInfoExpandList_service");
                        LogHelper.Error("要删除的Buid：" + partStrBuids + ";插入到扩展表发生异常：" + ex);
                    }

                    try
                    {
                        _userInfoRepository.UpdateReviewdetailRecord(partStrBuids);
                    }
                    catch (Exception ex)
                    {
                        MetricUtil.UnitReports("UpdateReviewdetailRecord_service");
                        logError.Error("发生异常：" + ex);
                    }
                }
            }

            return result;
        }
        public async Task<BaseViewModel> TransferDataAsync(ExcuteDistributeRequest request)
        {
            if ((request.IsAll == 1 && request.SearchAgentId < 1) || (request.IsAll == 0 && request.Buids.Count == 0))
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ErrorCanShowCustomer, "请先筛选业务员，并选中要回收的数据");
            }

            // 检测是不是把自己的数据转移给自己
            if (request.AgentIds.Contains(request.SearchAgentId))
            {
                request.AgentIds.Remove(request.SearchAgentId);
            }

            if (!request.AgentIds.Any())
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            }

            // 查找所有已分配的数据 陈亮于2018-1-8日注释，因为转移时有一条自己录入的数据，导致搜索已分配后的数量和只搜代理人的数量对不上
            //request.IsDistributed = new List<int>() { 4 };

            var checkResult = DistributeCheck(request);
            if (checkResult.Status != 1)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ErrorCanShowCustomer, "可转移数量不能少于需要转移数量");
            }

            var distributedModel = new DistributeDto()
            {
                DistributeAgentIds = request.AgentIds,
                AverageCount = request.AverageCount,
                DistributeBuids = checkResult.Data,
                OperageAgentId = request.ChildAgent,
                TopAgentId = request.Agent
            };
            // 分配核心逻辑
            var distributedResult = await DistributeKernelAsync(distributedModel);
            if (distributedResult.Status != 1)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "转移失败");
            }

            // 插入转移记录表
            var query = from u in distributedResult.Data
                        select new TransferRecycleDto
                        {
                            Buid = u.Id,
                            NewAgentId = u.Agent,
                            OldAgentId = u.OldAgent
                        };
            _recycleHistoryService.InsertTransferRecycleHistory(query.ToList(), request.Agent, request.ChildAgent, 1);

            // 转移成功，处理通知逻辑
            TransferDataSuccessSendNotice(distributedResult.Data);

            // 插入转移的跟进记录
            await _crmStepsService.TransferDataInsertCrmStepsAsync(distributedResult.Data, request.Agent, request.ChildAgent);

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
        }

        /// <summary>
        /// 转移数据成功后发送通知
        /// </summary>
        /// <param name="userInfoModels">转移成功的buid对应的新代理人集合</param>
        private void TransferDataSuccessSendNotice(List<UpdateUserInfoModel> userInfoModels)
        {
            #region 构建发送通知的数据
            List<TransferDataDto> noticeList = new List<TransferDataDto>();

            var agentGroup = userInfoModels.GroupBy(o => o.Agent);

            foreach (var agentUserinfo in agentGroup)
            {
                var query = from userinfo in agentUserinfo
                            select new TransferDataDetail
                            {
                                BuId = userinfo.Id,
                                LicenseNo = userinfo.LicenseNo
                            };

                var notice = new TransferDataDto
                {
                    AgentId = int.Parse(agentUserinfo.Key),
                    TransferDataDetailList = query.ToList()
                };

                noticeList.Add(notice);
            }
            #endregion


            if (noticeList.Any())
            {
                var sendMessageUrl = ConfigurationManager.AppSettings["SendMessage"];
                var _url = string.Format("{0}/api/Message/SendTransferMessage", sendMessageUrl);
                var data = JsonHelper.Serialize(noticeList);

                string resultMessage = HttpWebAsk.HttpClientPostAsync(data, _url);
            }
        }

        public CheckDto<List<long>> DistributeCheck(ExcuteDistributeRequest request)
        {
            var result = new CheckDto<List<long>>
            {
                // 根据前端传的条件，搜索需要分配的buid
                Data = request.IsAll == 1
                ? GetBuidsList2(request)
                : request.Buids
            };

            if (result.Data.Count <= 0)
            {
                result.Status = 0;
                result.Msg = "没有可分配数据";
                return result;
            }
            if ((request.AverageCount * request.AgentIds.Count) > result.Data.Count)
            {
                result.Status = 0;
                result.Msg = "可分配数量不能少于需要分配数量";
                return result;
            }
            result.Status = 1;
            return result;
        }

        public async Task<CheckDto<List<UpdateUserInfoModel>>> DistributeKernelAsync(DistributeDto model)
        {
            var result = new CheckDto<List<UpdateUserInfoModel>>();

            var takeCount = model.DistributeAgentIds.Count * model.AverageCount;
            var distributeUserinfo = await Task.Factory.StartNew(() =>
            {
                return _userInfoRepository.FindCustomerBuidOrderBy(model.DistributeBuids, 3, takeCount);
            });

            if (distributeUserinfo.Count < takeCount)
            {
                result.Status = 0;
                result.Msg = "可分配数量不能少于需要分配数量";
                return result;
            }

            // 需要放入公司指定的代理人下面的顶级数据（要分配的数据）：下级代理人中存在顶级要分配的数据
            List<long> updateTestBuids;
            // 需要分配的数据并更新代理人的数据，由顶级代理人改成分配后的下级代理人
            List<UpdateUserInfoModel> userInfoModels;
            // 批量续保数据有重复的则更新批量续保对应数据为老数据。
            Dictionary<long, long> updateBratch;
            //更新摄像头进店时间最新的插入跟进记录表
            List<CrmStepsUserInfoModel> crmStepsCamera;
            // 查询上述对象的值，以便下面进行分配
            userInfoModels = DistributedAgent(model.DistributeAgentIds, distributeUserinfo, model.AverageCount, model.TopAgentId, out updateTestBuids, out updateBratch, out crmStepsCamera);

            var isSuccess = false;

            // 如果buids是空的，就不分配了，因为没值
            if (!userInfoModels.Any())
            {
                result.Status = 0;
                result.Msg = "分配失败";
                return result;
            }
            else
            {
                isSuccess = _customerbusinessRepository.BulkUpdateByList(userInfoModels);
            }
            if (updateBratch.Any())
            {
                //modify by qdk 20181220 当要分配的数据和业务员的数据重复，并且都上传过批续，则保留业务员自己的批续数据，删除要分配的批续数据
                List<long>  noBuidList= _customerbusinessRepository.GetBatchRenewalItemBuidList(updateBratch);                
                // 这里在这种情况下会有问题：顶级要分配的数据大于1条，并且其中只存在一条是批续的数据，这时下级代理人中也存在一条相同车牌号的数据，但是不是批续的，这样这个地方返回的所影响行数是0，isSuccess=false;导致前端显示分配失败，但是数据是已经分配成功的
                // 这里去掉了isSuccess =               
                _customerbusinessRepository.UpdatebratchItem(updateBratch, noBuidList);
            }
            if (updateTestBuids.Any())
            {
                MoveToOurAgentAndInsertIntoSteps(updateTestBuids, model.TopAgentId);
                string buidsTest = string.Join(",", updateTestBuids);
                logInfo.Info("下级已有的数据转移到公司自己的代理人下：" + buidsTest + "\n" + "执行时间：" + DateTime.Now);
            }
            //将摄像头进店时间插入跟进记录表
            if (crmStepsCamera != null && crmStepsCamera.Count > 0)
            {
                _crmStepsService.AddCrmStepsOfCamera(crmStepsCamera);
            }


            if (isSuccess)
            {
                result.Status = 1;
                result.Data = userInfoModels;
            }
            else
            {
                result.Status = 0;
                result.Msg = "分配失败";
            }
            return result;
        }

        private List<UpdateUserInfoModel> DistributedAgent(List<int> agentIds
            , List<DistributeUserinfoDto> distributeUserinfo
            , int AverageCount
            , int topAgent
            , out List<long> updateTestBuids
            , out Dictionary<long, long> updateBratch
            , out List<CrmStepsUserInfoModel> crmStepsCamera)
        {
            var panal = new Dictionary<int, List<DistributeUserinfoDto>>();
            var userInfoModels = new List<UpdateUserInfoModel>();
            //移除顶级
            agentIds.Remove(topAgent);

            // 需要分配的代理人已试算的数据
            List<DistributeUserinfoDto> userinfoAgents = _customerbusinessRepository.GetUserinfosForagents(agentIds.ConvertAll(x => x.ToString())).OrderByDescending(c => c.UpdateTime).DistinctBy(c => c.LicenseNo).ToList();

            // 已试算数据的车牌号
            string[] licensenosAgent = userinfoAgents.Select(c => c.LicenseNo).ToArray();
            // 要分配数据的车牌号
            string[] licensenoDistribute = distributeUserinfo.Select(c => c.LicenseNo).ToArray();
            // 要分配数据中是批量续保的车牌号
            string[] licensenoDistributeBratch = distributeUserinfo.Where(c => c.RenewalType == 5).Select(c => c.LicenseNo).ToArray();

            // 需要分配的数据中存在下级代理人已试算的数据，则将下级代理人的数据放入公司指定的代理人下面
            //updateTestBuids = userinfoAgents.Where(c => licensenoDistribute.Contains(c.LicenseNo)).Select(c => c.Id).ToList();
            #region 2018-08-22
            #region 更新批续
            var mm = from s in userinfoAgents
                     join c in distributeUserinfo on s.LicenseNo equals c.LicenseNo
                     where c.RenewalType == 5
                     select new
                     {
                         newBuid = s.Id,
                         oldBuid = c.Id
                     };

            // 更新批续
            updateBratch = mm.ToDictionary(k => k.newBuid, v => v.oldBuid);
            #endregion
            // 需要分配的数据中存在下级代理人已试算的数据，则将要分配的数据放入公司指定的代理人下面（删除）
            updateTestBuids = distributeUserinfo.Where(c => licensenosAgent.Contains(c.LicenseNo)).Select(c => c.Id).ToList();

            //从要分配的数据中找到下级已试算的数据，并找到下级已试算的buid
            var existDataTemp = from agent in userinfoAgents
                                join distribute in distributeUserinfo on agent.LicenseNo equals distribute.LicenseNo
                                select new
                                {
                                    oldAgent = distribute.Agent,
                                    oldData = distribute,
                                    oldId = distribute.Id,
                                    oldIsCamera = distribute.IsCamera,
                                    oldCameraTime = distribute.CameraTime,
                                    newId = agent.Id,
                                    newAgent = agent.Agent,
                                    newLicenseNo = agent.LicenseNo,
                                    newIsCamera = distribute.IsCamera == true ? distribute.IsCamera : agent.IsCamera,
                                    newCameraTime = (distribute.IsCamera == true && distribute.CameraTime > agent.CameraTime) ? distribute.CameraTime : agent.CameraTime,
                                    isCrmSteps = (distribute.IsCamera == true && distribute.CameraTime > agent.CameraTime) ? 1 : 0  //判断要分配的是否是摄像头进店的，并且进店时间是最新的,则插入摄像头进店跟进记录
                                };
            var tempData = existDataTemp.ToList();
            //从要分配的数据中删除下级代理人中已经存在的数据
            distributeUserinfo.RemoveAll(o => existDataTemp.Select(i => i.oldData).Contains(o));
            var tempRepeatData = tempData.Select(a => new UpdateUserInfoModel()
            {
                Agent = a.newAgent,
                OldAgent = a.oldAgent,
                Id = a.newId,
                IsDistributed = 3,
                DistributedTime = DateTime.Now,
                OpenId = a.newAgent.ToMd5(),
                LicenseNo = a.newLicenseNo,
                TopAgentId = topAgent,
                IsCamera = a.newIsCamera,
                CameraTime = a.newCameraTime
            }).ToList();
            #endregion
            #region 摄像头进店插入进店时间到跟进记录
            crmStepsCamera = tempData.Where(a => a.isCrmSteps == 1).Select(a => new CrmStepsUserInfoModel()
            {
                agent_id = int.Parse(a.newAgent),
                b_uid = a.newId,
                camertime = a.oldCameraTime
            }).ToList();

            #endregion
            var listIndex = GetIndex(distributeUserinfo.Count, agentIds.Count);
            foreach (var currentAgent in agentIds)
            {
                var listDistribute = distributeUserinfo.Where((o, i) => listIndex.Contains(i)).ToList();
                var currentAgentStr = currentAgent.ToString();
                // 将下级代理人存在的数据还分配给原来的代理人，数据是顶级代理人的             
                userInfoModels.AddRange(tempRepeatData.Where(a => a.Agent == currentAgentStr).ToList());
                if (listDistribute.Count > 0)
                {
                    userInfoModels.AddRange(listDistribute.Select(o => new UpdateUserInfoModel
                    {
                        Agent = currentAgentStr,
                        OldAgent = o.Agent,
                        Id = o.Id,
                        IsDistributed = 3,
                        DistributedTime = DateTime.Now,
                        OpenId = currentAgentStr.ToMd5(),
                        LicenseNo = o.LicenseNo,
                        TopAgentId = topAgent,
                        IsCamera = o.IsCamera,
                        CameraTime = o.CameraTime
                    }));
                }

                for (int i = 0; i < listIndex.Count; i++)
                {
                    listIndex[i]++;
                }
            }
            return userInfoModels;
        }

        private List<int> GetIndex(int listCount, int agentCount)
        {
            List<int> result = new List<int>();
            int i = 0;
            while (i < listCount)
            {
                result.Add(i);
                i += agentCount;
            }
            return result;
        }

        /// <summary>
        /// 是否需要特殊处理分配通知
        /// 注：顶级分配给管理员时，管理员收到的分配通知应该和没有分配权限的代理人一样--大鹏
        /// </summary>
        /// <param name="agentList"></param>
        /// <returns></returns>
        private Dictionary<int, bool> IsSpecialNotice(List<int> agentList)
        {
            var hasDistribute = _authorityService.HasDistributeAuth(agentList);
            var agentRole = _managerRoleRepository.GetRoleTypeByAgentId(agentList);
            foreach (var item in agentRole)
            {
                if (item.RoleType == (int)RoleType.Admin)
                    hasDistribute[item.AgentId] = false;
            }
            return hasDistribute;
        }

        /// <summary>
        /// 根据分配到代理人的数据发送分配通知
        /// </summary>
        /// <param name="userInfoModels">分配到代理人的数据</param>
        /// <param name="distributedAgents">分配数据的代理人</param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        private void DistributeSuccessSendNotice(List<UpdateUserInfoModel> userInfoModels, List<int> distributedAgents, int topAgentId)
        {
            try
            {
                // 分配通知
                var dataVms = new List<DistributedDataVm>();
                var now = DateTime.Now;

                var hasDistribute = IsSpecialNotice(distributedAgents);

                foreach (var agent in distributedAgents)
                {
                    // 分配给当前代理人的数据
                    var currentAgentUserinfos = userInfoModels.Where(c => c.Agent == agent.ToString()).ToList();

                    // 分配到当前代理人的buid
                    var currentAgentBuids = currentAgentUserinfos.Select(c => c.Id).ToList();
                    // 将分配到当前代理人的buid拼接成字符串
                    var currentAgentBuidStr = string.Join(",", currentAgentBuids);

                    // 没有给当前代理人分配的数据
                    if (!currentAgentUserinfos.Any())
                    {
                        break;
                    }

                    //插入分配通知表bx_message
                    bx_message bxMessage = new bx_message
                    {
                        Agent_Id = agent,
                        Body = currentAgentBuidStr,
                        Create_Time = now,
                        Title = "分配通知",
                        Msg_Status = 0,
                        Msg_Level = 0,
                        Send_Time = now,
                        Create_Agent_Id = topAgentId,
                        Msg_Type = 6,
                    };
                    if (currentAgentUserinfos.Count > 1)
                    {
                        bxMessage.Buid = 0;
                        bxMessage.License_No = "";
                    }
                    else
                    {
                        var firstOrDefault = currentAgentUserinfos.FirstOrDefault();
                        if (firstOrDefault != null)
                        {
                            bxMessage.Buid = firstOrDefault.Id;
                            bxMessage.License_No = firstOrDefault.LicenseNo;
                        }
                    }

                    int insertNum = _customerbusinessRepository.InsertBxMessage(bxMessage);

                    // 分配横幅通知
                    var dataVm = new DistributedDataVm();
                    var tempLicenseNoes = new List<CompositeBuldLicenseNo>();

                    dataVm.BuidsString = currentAgentBuidStr;
                    dataVm.IsManager = hasDistribute[agent];
                    dataVm.AgentId = agent;
                    foreach (var item in currentAgentUserinfos)
                    {
                        var dataLicenseNo = new CompositeBuldLicenseNo
                        {
                            BuId = item.Id,
                            LicenseNo = item.LicenseNo
                        };
                        tempLicenseNoes.Add(dataLicenseNo);
                    }
                    dataVm.Data = tempLicenseNoes;
                    dataVm.MessageIds = insertNum.ToString();

                    dataVms.Add(dataVm);
                } //foreach (var agent in DistributedAgents)

                var sendMessageUrl = ConfigurationManager.AppSettings["SendMessage"];
                if (dataVms.Any())
                {
                    var _url = string.Format("{0}/api/Message/SendDistributeMessage", sendMessageUrl);
                    var data = JsonHelper.Serialize(dataVms);

                    logInfo.Info("分配通知内容" + data + "\n" + "执行时间：" + DateTime.Now);
                    string resultMessage = HttpWebAsk.HttpClientPostAsync(data, _url);
                }
            }
            catch (Exception ex)
            {
                logError.Error("分配的代理人Id：" + distributedAgents.ToJson());
                logError.Error("请求分配通知发生异常:" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
        }





        public bool BatchUpdateCustomerStatusAndCategories(BatchUpdateCustomerStatusAndCategoriesModel model)
        {
            string statusconvert = string.Empty;
            if (model.IsAll)
            {
                //var search = GetWhereByRequest2(model);
                //// 这里写死要关联bx_consumer_review表，因为要查几个回访字段 
                //search.JoinType = 1;
                ////search.PageSize = model.;
                ////search.CurPage = 0;
                //search.OrderBy = 1;
                // var search = GetWhereAndJoinType(model);
                var search = GetWhereAndJoinType(model, 1);
                var list = _userInfoRepository.FindCustomerBuid(search);
                model.UserIdList = list;
            }
            List<UpdateCustomerStatusAndCategoriesJson> listJson = _userInfoRepository.GetCustomerStatusAndCategories(model.UserIdList);
            bx_customercategories bc = new bx_customercategories();
            List<bx_crm_steps> listStep = new List<bx_crm_steps>();

            if (model.CategoryInfo != -1)//当CategoryInfo=-1时，代表前端勾选的是保持不变
            {

                bc = _customerCategoriesRepository.GetCirculationCategoryRecord(model.CategoryInfo);
            }

            if (model.Status != -1)
            {

                statusconvert = ((CustomerStatus)model.Status).ToString();
            }
            if (listJson.Count > 0)
            {


                foreach (var item in listJson)
                {
                    string json = string.Empty;

                    if (model.Status != -1 && item.OldStatus != model.Status)
                    {

                        json = string.Format("\"OldStatusInfo\":\"{0}\",\"NewStatusInfo\":\"{1}\"", item.OldStatusInfo, statusconvert);//item.OldStatusInfo.ToString(), statusconvert);
                    }


                    if (model.CategoryInfo != -1 && item.OldCategoryInfo != bc.CategoryInfo)
                    {
                        if (json != string.Empty)
                        {
                            json += ",";
                        }
                        json += string.Format("\"OldCategoryInfo\":\"{0}\",\"NewCategoryInfo\":\"{1}\"", item.OldCategoryInfo == null ? "" : item.OldCategoryInfo, bc.CategoryInfo);
                    }



                    if (json == string.Empty)
                    {
                        continue;
                    }

                    json = "{" + json + "}";

                    listStep.Add(new bx_crm_steps { agent_id = model.ChildAgent, create_time = DateTime.Now, b_uid = item.UserId, json_content = json, type = 13 });//13批量修改客户状态和类别
                }

                if (listStep.Count > 0)
                {
                    _consumerDetailService.InsertBySqlAsync(listStep);
                }
                else
                {
                    return true;
                }

            }


            return _customerStatusRepository.BatchUpdateCustomerStatusAndCategories(model.UserIdList, model.Status, model.CategoryInfo);
        }


        public async Task<BaseViewModel> UpdateGroupDistributeAsync(List<int> agentIds, int stepType, List<long> listBuid, int averageCount, int topAgent, int fountain, int operateAgentId)
        {
            var distributedModel = new DistributeDto()
            {
                DistributeAgentIds = agentIds,
                AverageCount = averageCount,
                DistributeBuids = listBuid,
                OperageAgentId = operateAgentId,
                TopAgentId = topAgent
            };

            var distributeResult = await DistributeKernelAsync(distributedModel);

            if (distributeResult.Status != 1)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, distributeResult.Msg);
            }
            if (distributeResult.Data.Count == 0)
            {
                // 这里表示所有的数据都是修改时间的，也是分配成功给的。
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            }

            // 插入分配通知表，这个原来是写在分配核心逻辑里面的，2017-10-17
            await _distributedHistoryService.InsertDistributedAsync(distributeResult.Data, topAgent, operateAgentId, 1);

            // 发送分配通知
            DistributeSuccessSendNotice(distributeResult.Data, agentIds, topAgent);

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
        }


        /// <summary>
        /// 获取客户列表重复数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UserInfoRepeatViewModel GetCustomerRepeatList(GetCustomerRepeatListRequest request)
        {
            int roleType = request.RoleType;
            int topAgentId = request.Agent;
            int childAgent = request.ChildAgent;
            int groupByType = request.GroupByType;
            int pageIndex = request.PageIndex;
            int pageSize = request.PageSize;
            UserInfoRepeatViewModel viewModel = new UserInfoRepeatViewModel()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                GroupByType = groupByType,
                TotalCount = 0
            };
            try
            {
                //1.1 拼接查询代理人
                var tupleAgent = BuildAgent(topAgentId, childAgent, 0, roleType);
                //1.2 (1)当前代理人下级数量没有查过2000(2)根据当前代理人没有搜索到代理人，包括它自己。
                if (string.IsNullOrEmpty(tupleAgent.Item2) && tupleAgent.Item1.Count == 1 && tupleAgent.Item1[0] == -1)
                {
                    viewModel.BusinessStatus = -10016;
                    viewModel.StatusMessage = "没有搜索到代理人";
                    return viewModel;
                }
                string agentContidion = _agentService.GetConditionSql(tupleAgent);
                string joinCondition = tupleAgent.Item2;
                //1.3 放在where后边的代理人条件
                string agentWhere = _agentService.GetAgentWhere(roleType, childAgent, topAgentId);
                //2. 获得客户列表重复数据总数量
                int repeatCount = _userInfoRepository.GetCustomerRepeatListCount(pageIndex, pageSize, groupByType, agentContidion, joinCondition);
                viewModel.TotalCount = repeatCount;
                ////根据查重条件，查询总数量是0
                //if (repeatCount <= 0)
                //{
                //    return viewModel;
                //}
                //3. 查询重复数据列表
                List<UserInfoRepeatModel> repeatList = _userInfoRepository.GetCustomerRepeatList(childAgent, topAgentId, groupByType, pageIndex, pageSize, agentContidion, joinCondition, agentWhere);
                //4. 将客户列表重复数据转换成分组格式            
                ConvertCustomerRepeatList(groupByType, viewModel, repeatList);
                viewModel.BusinessStatus = 1;
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("GetCustomerRepeatList 发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return viewModel;
        }

        /// <summary>
        /// 将客户列表重复数据转换成分组格式
        /// </summary>
        /// <param name="groupByType"></param>
        /// <param name="viewModel"></param>
        /// <param name="repeatList"></param>
        private void ConvertCustomerRepeatList(int groupByType, UserInfoRepeatViewModel viewModel, List<UserInfoRepeatModel> repeatList)
        {
            var repeatTempList = repeatList.Select<UserInfoRepeatModel, UserInfoRepeatModel>(a =>
                new UserInfoRepeatModel
                {
                    Buid = a.Buid,
                    LicenseNo = string.IsNullOrEmpty(a.LicenseNo) ? "" : a.LicenseNo,
                    LicenseOwner = string.IsNullOrEmpty(a.LicenseOwner) ? "" : a.LicenseOwner,
                    CarVIN = string.IsNullOrEmpty(a.CarVIN) ? "" : a.CarVIN,
                    MoldName = string.IsNullOrEmpty(a.MoldName) ? "" : a.MoldName,
                    RegisterDate = string.IsNullOrEmpty(a.RegisterDate) ? "" : a.RegisterDate,
                    NextReviewDate = string.IsNullOrEmpty(a.NextReviewDate) ? "" : a.NextReviewDate,
                    AgentName = string.IsNullOrEmpty(a.AgentName) ? "" : a.AgentName,
                    ClientMobile = string.IsNullOrEmpty(a.ClientMobile) ? "" : a.ClientMobile,
                    ClientName = string.IsNullOrEmpty(a.ClientName) ? "" : a.ClientName
                });
            List<UserRepModel> list = new List<UserRepModel>();
            if (groupByType == 3)
            {
                var clientList = repeatTempList.GroupBy(a => a.ClientName + a.ClientMobile);
                foreach (var item in clientList)
                {
                    var clientTemp = item.FirstOrDefault();
                    list.Add(new UserRepModel()
                    {
                        //ClientMobile = string.IsNullOrEmpty(clientTemp.ClientMobile) ? "" : clientTemp.ClientMobile,
                        //ClientName = string.IsNullOrEmpty(clientTemp.ClientName) ? "" : clientTemp.ClientName,
                        UserList = item.ToList()
                    });
                }
                viewModel.UserInfoRepeatList = list;// repeatTempList.GroupBy(a => a.ClientName + a.ClientMobile);
            }
            else if (groupByType == 2)
            {
                var vinList = repeatTempList.GroupBy(a => a.CarVIN);
                //viewModel.UserRepList = vinList.Select(a => new UserRepModel { CarVIN = string.IsNullOrEmpty(a.Key) ? "" : a.Key, UserList = a.ToList() }).ToList();
                foreach (var item in vinList)
                {
                    list.Add(new UserRepModel()
                    {
                        //CarVIN = string.IsNullOrEmpty(item.Key) ? "" : item.Key,
                        UserList = item.ToList()
                    });
                }
                viewModel.UserInfoRepeatList = list;// repeatTempList.GroupBy(a => a.CarVIN);
            }
            else
            {
                var licenseNoList = repeatTempList.GroupBy(a => a.LicenseNo).ToList();
                //viewModel.UserRepList = licenseNoList.Select(a => new UserRepModel { LicenseNo = string.IsNullOrEmpty(a.Key) ? "" : a.Key, UserList = a.ToList() }).ToList();
                foreach (var item in licenseNoList)
                {
                    list.Add(new UserRepModel()
                    {
                        //LicenseNo = string.IsNullOrEmpty(item.Key) ? "" : item.Key,
                        UserList = item.ToList()
                    });
                }
                viewModel.UserInfoRepeatList = list;//repeatTempList.GroupBy(a => a.LicenseNo).ToList();
            }
        }

        /// <summary>
        /// 删除客户列表重复数据
        /// </summary>
        /// <param name="Buids">要删除的buid</param>
        /// <param name="TopAgentId">顶级代理人</param>
        /// <param name="ChildAgent">当前代理人</param>
        /// <returns></returns>
        public BaseViewModel MultipleDeleteRepeat(List<long> Buids, int TopAgentId, int ChildAgent)
        {
            try
            {
                if (Buids == null || Buids.Count() <= 0)
                {
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "Buids为空");
                }
                var strBuids = string.Join(",", Buids);
                LogHelper.Info("删除客户列表【重复数据】操作，记录删除的buid\r\n请求参数:" + Buids.ToJson());
                var result = false;
                result = MultipleDeleteRepeatByBuids(Buids, TopAgentId, ChildAgent, 3);
                if (result)
                {
                    return BaseViewModel.GetBaseViewModel(1, "删除成功");
                }
                else
                {
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "删除失败");
                }
            }
            catch (Exception ex)
            {
                logError.Error("MultipleDeleteRepeat发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.SystemError, "服务发生异常");
            }
        }
        private bool MultipleDeleteRepeatByBuids(List<long> Buids, int TopAgentId, int ChildAgent, int IsTest)
        {
            List<string> tempBuids = CycleBuid(Buids);
            bool result = false;
            //获取代理人集合
            var listAgent = _agentService.GetSonsListFromRedis(TopAgentId);
            var strAgents = string.Empty;
            if (listAgent != null && listAgent.Count > 0)
            {
                strAgents = "'" + string.Join("','", listAgent) + "'";
            }

            foreach (string temp in tempBuids)
            {
                result = TranMultipleDeleteRepeatByBuids(temp, ChildAgent, IsTest, strAgents);
            }
            return result;
        }
        /// <summary>
        /// 事务：1删除bx_userinfo表数据
        ///      2.在bx_userinfo_expand表添加删除标记
        ///      3.添加进步骤表
        /// </summary>
        /// <param name="partStrBuids"></param>
        /// <param name="agentId"></param>
        /// <param name="IsTest"></param>
        /// <param name="strAgents"></param>
        /// <returns></returns>
        private bool TranMultipleDeleteRepeatByBuids(string partStrBuids, int agentId, int IsTest, string strAgents)
        {
            bool isSucess = false;
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //1.更新bx_userinfo表IsTest=3（删除，放到回收站）
                    isSucess = _userInfoRepository.UpdateIsTest(partStrBuids, agentId, 3, strAgents);
                    var tempList = partStrBuids.Split(',').Select(a => long.Parse(a)).ToList();
                    //2.添加bx_userinfo_expand中删除类型和删除时间字段
                    if (isSucess)
                    {
                        AddUserInfoExpandList(tempList, partStrBuids, 1);
                    }
                    //3.添加进步骤表
                    if (isSucess)
                    {
                        _crmStepsService.BatchAddCrmStepsByBuid(partStrBuids, 3);
                    }
                    //删除数据更新统计表，统计使用
                    if (isSucess)
                    {
                        try
                        {
                            _userInfoRepository.UpdateReviewdetailRecord(partStrBuids);
                        }
                        catch (Exception ex)
                        {
                            MetricUtil.UnitReports("TranMultipleDeleteRepeatByBuids_UpdateReviewdetailRecord_service");
                            logError.Error("partStrBuids=" + partStrBuids + ";发生异常：" + ex);
                        }
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    logError.Error("删除重复数据出错,Buids：" + partStrBuids + "  \n发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                }
                finally
                {
                    scope.Dispose();
                }
            }
            return isSucess;
        }
        /// <summary>
        /// 将删除数据插入到bx_userinfo_expand表，记录删除类型和删除时间
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="partStrBuids">buid集合</param>
        /// <param name="deleteType">删除类型：0.普通删除，1.去重删除，2.摄像头过滤</param>
        /// <returns></returns>
        private bool AddUserInfoExpandList(List<long> buids, string partStrBuids, int deleteType)
        {
            bool result = false;
            try
            {
                List<bx_userinfo_expand> list = new List<bx_userinfo_expand>();
                List<long> buidList = _userinfoExpandService.GetExistBuidList(partStrBuids);
                //string strBuids = string.Join("','", buidList);
                if (buidList != null && buidList.Count > 0)
                {
                    //将已将存在的数据进行更新
                    _userinfoExpandService.UpdateUserExpandByBuid(buidList, deleteType, DateTime.Now);
                }
                //去除已经存在的数据
                buids.RemoveAll(a => buidList.Contains(a));
                //将剩余的数据新增到扩展表中
                foreach (long buid in buids)
                {
                    list.Add(new bx_userinfo_expand() { b_uid = buid, is_temp_mobile = -1, is_temp_email = -1, update_time = DateTime.Parse("1970-01-01 00:00:00"), delete_type = deleteType, delete_time = DateTime.Now });
                }
                result = _userinfoExpandService.AddRangeBySql(list);
            }
            catch (Exception ex)
            {
                MetricUtil.UnitReports("AddUserInfoExpandList_service");
                LogHelper.Error("AddUserInfoExpandList:buids(" + partStrBuids + ")；deleteType=" + deleteType + ";发生异常：" + ex);
            }
            return result;
        }

        /// <summary>
        /// 回收站批量撤销
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BatchBackViewModel BatchBackout(DeleteRepeatCustomerRequest request)
        {
            //1.0 存储撤销失败的数据
            int failCount = 0;
            //2.0 存储IsTest=3和IsTest=0的数据
            List<UserInfoPartModel> userinfoAllList = _userInfoRepository.GetUserInfoList(request.Buids);
            if (userinfoAllList == null || userinfoAllList.Count <= 0)
            {
                return new BatchBackViewModel() { BusinessStatus = -10019, StatusMessage = "数据不存在", FailCount = failCount };
            }
            //2.1 存储IsTest=0的正式数据,不用进行撤销操作
            List<UserInfoPartModel> userinfoList0 = userinfoAllList.Where(a => a.IsTest == 0).ToList();
            if (userinfoList0 != null & userinfoList0.Count > 0)
            {
                failCount += userinfoList0.Count;
            }
            //2.2 存储IsTest=3的回收站数据
            List<UserInfoPartModel> userinfoList3 = userinfoAllList.Where(b => b.IsTest == 3).ToList();
            //2.3 当userinfoList3为空时候，不用做撤销工作
            if (userinfoList3 == null || userinfoList3.Count <= 0)
            {
                failCount += userinfoList3.Count;//modify by qdk 2018-12-21
                //没有要撤销的数据
                return new BatchBackViewModel() { BusinessStatus = 0, FailCount = failCount };
            }
            //3.0 获得回收站数据的车牌号集合
            List<string> licenseNoList = userinfoList3.Select(a => a.LicenseNo).Distinct().ToList();
            string licenseNos = "'" + string.Join("','", licenseNoList) + "'";
            //4.0 获取顶级所有的代理人
            var listAgent = _agentService.GetSonsListFromRedis(request.Agent);
            string strAgents = "'" + string.Join("','", listAgent) + "'";
            //5.0 获得代理人、车牌号重复数据
            List<UserInfoPartModel> userList = _userInfoRepository.FindByAgentsAndLicenseNos(strAgents, licenseNos);
            if (userList == null || userList.Count <= 0)
            {
                //将userinfoList3中的数据直接撤销
                bool result2 = RevokeUserInfo(userinfoList3);
                return new BatchBackViewModel() { BusinessStatus = (result2 ? 1 : 0), FailCount = (result2 ? failCount : request.Buids.Count) };
            }
            //属于自己的不允许撤销
            foreach (var item in userList)
            {
                var myTemp = userinfoList3.Where(a => a.AgentId == item.AgentId && a.LicenseNo == item.LicenseNo).ToList();
                if (myTemp != null && myTemp.Count > 0)
                {
                    failCount += 1;//modify by qdk 2018-12-21
                    userinfoList3.RemoveAll(a => myTemp.Contains(a));
                }
            }
            //7.0 根据顶级判断是否允许重复报价（即：是否允许撤销）
            bx_agent agentModel = _agentRepository.GetAgent(request.Agent);
            if (agentModel.repeat_quote == 1)
            {
                //将userinfoList3中的数据直接撤销
                bool result2 = RevokeUserInfo(userinfoList3);
                return new BatchBackViewModel() { BusinessStatus = (result2 ? 1 : 0), FailCount = (result2 ? failCount : request.Buids.Count) };
            }
            //5.1 重复数据中的车牌号
            List<string> userLicenseNoList = userList.Select(a => a.LicenseNo).Distinct().ToList();
            //5.2 重复数据对应接收buid的数据
            List<UserInfoPartModel> userInfoRepeatList3 = userinfoList3.Where(a => userLicenseNoList.Contains(a.LicenseNo)).ToList();
            //5.3 将重复的车牌号从userinfoList3中移除
            //userinfoList3.RemoveAll(a => userLicenseNoList.Contains(a.LicenseNo));
            userinfoList3.RemoveAll(a => userInfoRepeatList3.Contains(a));

            if (userinfoList3.Count == 0)
            {
                failCount += userInfoRepeatList3.Count;
                //没有要撤销的数据
                return new BatchBackViewModel() { BusinessStatus = 0, FailCount = failCount };
            }

            //8.0 不允许重复报价,则重复数据不允许进行撤销
            if (agentModel.repeat_quote == 0)
            {
                failCount += userInfoRepeatList3.Count;
                bool result3 = RevokeUserInfo(userinfoList3);
                return new BatchBackViewModel() { BusinessStatus = (result3 ? 1 : 0), FailCount = (result3 ? failCount : request.Buids.Count) };
            }
            string topAgentId = agentModel.TopAgentId.ToString();
            //9.0 筛选出重复数据允许撤销的数据
            List<UserInfoPartModel> allowRepeatList = new List<UserInfoPartModel>();
            //10.0【全局不允许重复报价】允许二级之间重复
            if (agentModel.repeat_quote == 2)
            {
                //10.1 顶级不允许重复报价，则不允许撤销
                //failCount += userInfoRepeatList3.Where(a => a.AgentId == topAgentId).ToList().Count;
                userInfoRepeatList3.RemoveAll(a => a.AgentId == topAgentId);
                foreach (var model in userInfoRepeatList3)
                {
                    //10.2 同重复数据车牌号相同的数据
                    List<UserInfoPartModel> tempList = userList.Where(a => a.LicenseNo == model.LicenseNo).ToList();
                    if (tempList == null || tempList.Count <= 0)
                    {
                        allowRepeatList.Add(model);
                        continue;
                    }
                    //10.3 判断是否是顶级的数据
                    var topList = tempList.Where(a => a.AgentId == topAgentId).ToList();
                    if (topList != null && topList.Count > 0)
                    {
                        //顶级存在数据，不允许重复报价。
                        failCount += 1;
                        continue;
                    }
                    //10.4 判断是否是自己的数据
                    var myList = tempList.Where(a => a.AgentId == model.AgentId.ToString()).ToList();
                    if (myList != null && myList.Count > 0)
                    {
                        //属于自己，不允许重复报价
                        failCount += 1;
                        continue;
                    }
                    //10.5 剩下其他的（二级，三级）
                    //当前车牌号对应的代理人是二级或者三级,查询出二级和三级代理人数据后,agentTempList代理人数据包含tempList中数据,不允许重复报价,则不允许撤销
                    List<string> agentTempList = _agentRepository.GetSonLevelAgent(int.Parse(model.AgentId));
                    var agentSonList = tempList.Where(a => agentTempList.Contains(a.AgentId)).ToList();
                    if (agentSonList != null && agentSonList.Count > 0)
                    {
                        failCount += 1;
                        continue;
                    }
                    allowRepeatList.Add(model);
                }
            }
            //allowRepeatList存储的是允许重复报价的数据，所以允许进行撤销
            if (allowRepeatList.Count > 0)
            {
                userinfoList3.AddRange(allowRepeatList);
            }
            //9. 进行撤销
            bool result = RevokeUserInfo(userinfoList3);
            //成功状态码返回1，失败返回0，Data中存储的是撤销失败的数量
            return new BatchBackViewModel() { BusinessStatus = (result ? 1 : 0), FailCount = (result ? (request.Buids.Count - userinfoList3.Count) : request.Buids.Count) };
        }
        /// <summary>
        /// 批量撤销
        /// </summary>
        /// <param name="buidList"></param>
        /// <param name="buids"></param>
        /// <returns></returns>
        private bool RevokeUserInfo(List<UserInfoPartModel> userinfoList3)
        {
            bool result = false;
            using (TransactionScope scope = new TransactionScope())
            {
                List<long> buidList = userinfoList3.Select(a => a.Buid).ToList();
                string buids = string.Join(",", buidList);
                try
                {
                    //1.0撤销
                    bool reuserinfo = _userInfoRepository.BatchRevokeFiles(buids);
                    if (reuserinfo)
                    {
                        //2.0执行成功之后，调用批续模块的恢复
                        _batchRenewalRepository.RevertBatchRenewalItemByBuIdList(buidList);
                    }
                    if (reuserinfo)
                    {
                        //3.0将bx_userinfo_expand恢复默认
                        _userinfoExpandRepository.UpdateUserExpandByBuid(buids, -1, DateTime.Parse("1970-01-01"));
                    }
                    result = reuserinfo;
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    logError.Error("回收站批量撤销,Buids：" + buids + "  \n发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                }
                finally
                {
                    scope.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// 回收站批量删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel BatchDeleteRecycle(DeleteRepeatCustomerRequest request)
        {
            bool result = false;
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    string buids = string.Join(",", request.Buids);
                    var crmMultipleDelete = int.Parse(ConfigurationManager.AppSettings["crmMultipleDelete"]);
                    //添加到记录表
                    bool isCrmSteps = _crmStepsService.BatchAddCrmStepsByBuid(buids, 3);
                    //回收站批量删除
                    result = _userInfoRepository.UpdateAgentByBuid(crmMultipleDelete, buids, 3);
                    scope.Complete();

                }
                catch (Exception ex)
                {
                    logError.Error("回收站批量删除,发生异常: \n" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" +
                                  ex.InnerException);
                }
                finally
                {
                    scope.Dispose();
                }
            }

            if (result)
            {
                return BaseViewModel.GetBaseViewModel(1, "删除成功");
            }
            else
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "删除失败");
            }
        }

        /// <summary>
        /// 将批改车牌添加到用户表
        /// </summary>
        /// <param name="recGuid">dz_reconciliation.guid</param>
        /// <param name="agentId">车牌属于的代理人</param>
        /// <returns></returns>
        public CorrectCarViewModel AddUserInfo(string recGuid, int agentId)
        {
            //1.0获取保单信息
            dz_baodanxinxi baodanModel = _userInfoRepository.BaoDanXinXiModelByRecDuid(recGuid);
            if (baodanModel == null)
            {
                return new CorrectCarViewModel() { BusinessStatus = -10002, StatusMessage = "没有在保单信息表中找到数据", BuId = 0 };
            }
            //2.0获取ukey信息
            bx_agent_ukey uKeyModel = _agentUkeyRepository.GetAgentUKeyModel(baodanModel.UKeyId.Value);

            if (uKeyModel == null)
            {
                return new CorrectCarViewModel() { BusinessStatus = -10002, StatusMessage = "请检查渠道是否正常", BuId = 0 };
            }

            //判断是大小车,当时"02"的时候是小车，其余的是大车
            int renewalCarType = baodanModel.CarLicenseTypeValue == "02" ? 0 : 1;


            //3.0获取表中是否存在数据
            bx_userinfo userInfoModel = _userInfoRepository.GetUserInfoByCondition(baodanModel.CarLicense, agentId.ToString().GetMd5(), agentId.ToString(), renewalCarType);

            //4.0表中存在相同数据，需要更新
            if (userInfoModel != null && userInfoModel.Id > 0)
            {
                userInfoModel.Id = userInfoModel.Id;
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
                if (!string.IsNullOrEmpty(baodanModel.InsureMoblie))
                {
                    userInfoModel.InsuredMobile = baodanModel.InsureMoblie;//被保险人电话
                }
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
                if (!string.IsNullOrEmpty(baodanModel.PolicyHoderMoblie))
                {
                    userInfoModel.HolderMobile = baodanModel.PolicyHoderMoblie;
                }
                if (!string.IsNullOrEmpty(baodanModel.PolicyHoderIdNum))
                {
                    userInfoModel.HolderIdCard = baodanModel.PolicyHoderIdNum;
                }
                if (!string.IsNullOrEmpty(baodanModel.PolicyHoderIdType))
                {
                    userInfoModel.HolderIdType = GetType(baodanModel.PolicyHoderIdType);

                }
                if (!string.IsNullOrEmpty(baodanModel.PolicyHoderEmail))
                {
                    userInfoModel.HolderEmail = baodanModel.PolicyHoderEmail;
                }
                #endregion
                bool resultTemp = _userInfoRepository.UpdateUserInfo(userInfoModel);
                return resultTemp ? new CorrectCarViewModel() { BusinessStatus = 1, StatusMessage = "更新成功", BuId = userInfoModel.Id } : new CorrectCarViewModel() { BusinessStatus = -10017, StatusMessage = "更新失败", BuId = userInfoModel.Id };

            }

            //表中不存在相同数据，直接添加
            //5.0表中不存在相同数据，直接添加
            bx_userinfo userModel = new bx_userinfo();
            #region 生成对象
            userModel.LicenseNo = baodanModel.CarLicense;
            userModel.OpenId = baodanModel.AgentId.Value.ToString().GetMd5();
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
            userModel.InsuredMobile = baodanModel.InsureMoblie;
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
            userModel.HolderMobile = baodanModel.PolicyHoderMoblie;
            userModel.HolderIdCard = baodanModel.PolicyHoderIdNum;
            //投保人证件类型
            userModel.HolderIdType = GetType(baodanModel.PolicyHoderIdType);
            userModel.HolderEmail = baodanModel.PolicyHoderEmail;
            userModel.CategoryInfoId = 0;
            userModel.IsCamera = false;
            userModel.CameraTime = DateTime.MinValue;
            userModel.DistributedTime = DateTime.MinValue;
            userModel.IsChangeRelation = 0;
            userModel.CustomerStatus = 0;
            userModel.top_agent_id = _agentRepository.GetAgent(userModel.agent_id).TopAgentId;
            userModel.IsBatchRenewalData = 0;

            userModel.RenewalCarType = renewalCarType;//先默认后，等后面再处理
            #endregion

            bool result = _userInfoRepository.AddUserInfo(userModel);
            return result ? new CorrectCarViewModel() { BusinessStatus = 1, StatusMessage = "添加成功", BuId = userModel.Id } : new CorrectCarViewModel() { BusinessStatus = -10017, StatusMessage = "添加失败", BuId = 0 };
            //return result ? BaseViewModel.GetBaseViewModel(1, "添加成功") : BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "添加失败");
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
