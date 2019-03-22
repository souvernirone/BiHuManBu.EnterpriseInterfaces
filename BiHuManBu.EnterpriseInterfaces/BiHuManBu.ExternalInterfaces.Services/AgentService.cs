using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.Enums;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using AgentCity = BiHuManBu.ExternalInterfaces.Models.ViewModels.AgentCity;
using AgentSourceViewModel = BiHuManBu.ExternalInterfaces.Models.ViewModels.AgentSourceViewModel;
using EnumSource = BiHuManBu.ExternalInterfaces.Models.ViewModels.EnumSource;
using Source = BiHuManBu.ExternalInterfaces.Models.ViewModels.Source;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class AgentService : CommonBehaviorService, IAgentService
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IAgentRateRepository _rateRepository;
        private readonly IAgentSpecialRateRepository _specialRateRepository;
        private readonly IManagerUserRepository _manageruserRepository;
        private readonly IAgentConfigRepository _agentConfigRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IHeBaoDianWeiRepository _heBaoDianWeiRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IManagerRoleButtonRelationRepository _managerRoleButtonRelationRepository;
        private Models.AppIRepository.IAgentConfigRepository _agentConfig;

        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private const string logSeparator = "\r\n---------------------------------------------------------------------------------------- \r\n";
        private readonly ILog logErro = LogManager.GetLogger("ERROR");
        private readonly ILog logMsg = LogManager.GetLogger("MSG");

        readonly CacheClient _cacheClient;
        public AgentService(
            IAgentRepository agentRepository
            , IAgentConfigRepository agentConfigRepository
            , IHeBaoDianWeiRepository heBaoDianWeiRepository
            , ICityRepository cityRepository
            , IAgentRateRepository rateRepository
            , IManagerUserRepository manageruserRepository
            , IAgentSpecialRateRepository specialRateRepository
            , ICacheHelper cacheHelper
            , IUserInfoRepository userInfoRepository
            , IConsumerDetailRepository consumerDetailRepository
            , IManagerRoleButtonRelationRepository managerRoleButtonRelationRepository, Models.AppIRepository.IAgentConfigRepository agentConfig)
            : base(agentRepository, cacheHelper)
        {
            _agentRepository = agentRepository;
            _rateRepository = rateRepository;
            _specialRateRepository = specialRateRepository;
            _manageruserRepository = manageruserRepository;
            _cityRepository = cityRepository;
            _agentConfigRepository = agentConfigRepository;
            _heBaoDianWeiRepository = heBaoDianWeiRepository;
            _userInfoRepository = userInfoRepository;
            _managerRoleButtonRelationRepository = managerRoleButtonRelationRepository;
            _agentConfig = agentConfig;

            _cacheClient = new CacheClient(new RedisHashCache(Convert.ToInt32(GetAppSettings("dbNum"))));
        }

        public string GetAppSettings(string key)
        {
            var val = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(val))
                return "";
            return val;
        }
        public new bx_agent GetAgent(int agentId)
        {
            //string agentCacheKey = string.Format("agent_cacke_key-{0}", agentId);
            //bx_agent bxAgent = HttpRuntime.Cache.Get(agentCacheKey) as bx_agent;
            //if (bxAgent != null)
            //{
            //    return bxAgent;
            //}
            //else
            //{
            var agent = _agentRepository.GetAgent(agentId);
            //    HttpRuntime.Cache.Insert(agentCacheKey, agent, null, DateTime.Now.AddHours(6), TimeSpan.Zero, CacheItemPriority.High, null);
            return agent;
            //}

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public GetAgentIdentityAndRateResponse GetAgent(GetAgentIdentityAndRateRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new GetAgentIdentityAndRateResponse();
            //根据经纪人获取手机号 
            bx_agent agentModel = GetAgent(request.Agent);
            //logInfo.Info("获取到的经纪人信息:"+agentModel.ToJson());
            //参数校验
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
            //当前经济人 
            var curAgent = _agentRepository.GetAgent(request.OpenId);
            //直客身份
            if (curAgent == null)
            {
                response.IsAgent = 0;
            }
            else
            {
                var allids = GetSonsListFromRedis(request.ParentAgent);
                //_agentRepository.GetSonId(request.ParentAgent);
                //var allsons = allids.Split(',');
                //当前试算人即是经纪人自己
                if (curAgent.Id == request.Agent)
                {
                    //看看是否是在这个顶级经纪人的归属下，才能按照经纪人身份展示,否则就按照直客处理

                    var isExist = allids.Count(x => x == curAgent.Id) > 0;
                    if (isExist)
                    {
                        response.IsAgent = 1;
                    }
                    else
                    {
                        response.IsAgent = 0;
                    }

                }
                else
                {
                    //Request.Agent有可能是试算人的上级代理,是就按照经纪人身份处理，否则 按照直客处理
                    var isExist = allids.Count(x => x == curAgent.Id) > 0;
                    if (isExist)
                    {
                        response.IsAgent = 1;
                    }
                    else
                    {
                        response.IsAgent = 0;
                    }
                }

                if (response.IsAgent == 1)
                {
                    allids.Remove(request.ParentAgent);

                    //allsons.ToList().RemoveAt(0);
                    //List<int> allAgentIdExceptTopAgent = Enumerable.Cast<int>(allsons).ToList();
                    List<int> allAgentIdExceptTopAgent = allids;
                    // allsons.Select(int.Parse).ToList();
                    var allAgentRate = _rateRepository.GetAgentRates(allAgentIdExceptTopAgent, request.Source);
                    var allSpecailAgentRate = _specialRateRepository.GeAgentSpecialRates(allAgentIdExceptTopAgent,
                        request.Source);
                    List<double> bizList = new List<double>();
                    for (var i = 0; i < allAgentIdExceptTopAgent.Count; i++)
                    {

                        var curSpecialRates = allSpecailAgentRate.Where(x => x.agent_id == allAgentIdExceptTopAgent[i]).ToList();
                        var curRates = allAgentRate.Where(x => x.agent_id == allAgentIdExceptTopAgent[i]).ToList();
                        var currentRate = 0.0;
                        if (curSpecialRates.Any())
                        {
                            foreach (var specialRate in curSpecialRates)
                            {
                                if (specialRate.system_rate == request.BizSysRate)
                                {
                                    currentRate = specialRate.budian_rate ?? 0;
                                }
                                if (currentRate == 0)
                                {
                                    if (curRates.Any())
                                    {
                                        var v = curRates.FirstOrDefault().agent_rate ?? 0;
                                        currentRate = v;
                                    }
                                    else
                                    {
                                        currentRate = 0;
                                    }
                                }
                                bizList.Add(currentRate);
                            }
                        }
                    }

                    if (request.BizSysRate > 0)
                    {
                        response.BizRate = request.BizSysRate - bizList.Sum(x => x);
                    }
                    else
                    {
                        response.BizRate = 0;
                    }
                    if (allAgentRate.Any())
                    {
                        var t = allAgentRate.FirstOrDefault(x => x.agent_id == curAgent.Id);
                        if (t != null)
                        {
                            response.ForceRate = t.rate_three ?? 0;
                            response.TaxRate = t.rate_four ?? 0;
                        }
                        else
                        {
                            response.ForceRate = 0;
                            response.TaxRate = 0;
                        }

                    }

                }
            }

            return response;

        }

        public bool EditAgentAndManagerUserRoleId(int agentId, int isUsed, int roleId)
        {
            return _agentRepository.EditAgentAndManagerUserRoleId(agentId, isUsed, roleId);
        }
        public GetAgentViewModel QueryAgentInfo(int topAgentId, int agentId, int? isUsed, string search, int pageSize, int pageNum, out int totalNum)
        {
            GetAgentViewModel viewModel = new GetAgentViewModel();
            totalNum = 0;
            try
            {
                var agentInfo = _agentRepository.QueryAgentInfo(topAgentId, agentId, isUsed, search, pageSize, pageNum, out totalNum);
                viewModel.AgentInfo = null;
                if (agentInfo != null)
                {
                    viewModel.AgentInfo = agentInfo.Select(x => new QueryAgentInfo()
                    {
                        AgentId = x.Id,
                        AgentName = x.AgentName,
                        Mobile = x.Mobile,
                        IsUsed = x.IsUsed,
                        IsUsedName = x.IsUsed.HasValue ? (x.IsUsed == 0 ? "待审核" : (x.IsUsed == 1 ? "已启用" : "已禁用")) : "",
                        RoleName = _manageruserRepository.GetUserRoleName(x.ManagerRoleId),
                        RoleId = x.ManagerRoleId,
                        Time = x.CreateTime.HasValue ? x.CreateTime.Value.ToString("yyyy-MM-dd HH:mm") : "",
                        AgentAccount = x.AgentAccount,
                        AgentLevel = x.agent_level
                    }).ToList();
                }
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功";
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;
            }
            return viewModel;
        }

        public GetAgentAddViewModel CopyAgentInfoAdd(int id, int ShareCode)
        {
            GetAgentAddViewModel viewModel = new GetAgentAddViewModel();
            try
            {
                var result = _agentRepository.CopyAgentInfoAdd(id, ShareCode);
                if (result)
                {
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "加入成功";
                }
                else
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "加入失败";
                }
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;
            }
            return viewModel;
        }
        public int EditAgentChangeTopAgent(int id)
        {
            var listAgent = GetSonsListFromRedisToString(id);
            listAgent.Remove(id.ToString());
            return _agentRepository.EditAgentChangeTopAgent(id, listAgent);
        }

        public AgentDistributedViewModel GetAgentDistributedInfo(int parentId)
        {
            AgentDistributedViewModel viewModel = new AgentDistributedViewModel();
            try
            {
                var listSonAgent = GetSonsListFromRedis(parentId);

                var result = _agentRepository.GetAgentDistributedInfo(parentId, listSonAgent).Select(x => new QueryAgentInfo() { AgentId = x.Id, AgentName = x.AgentName }).ToList();
                viewModel.AgentDistributedInfo = result;
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功";
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;
            }
            return viewModel;
        }

        //public int UpdateAgenrReport(string topAgentId, DateTime startTime, DateTime endTime)
        //{
        //    return _agentRepository.UpdateAgenrReport(topAgentId, startTime, endTime);
        //}

        public string GetToken(int agentid, string uniqueIdentifier)
        {
            return _agentRepository.GetToken(agentid, uniqueIdentifier);
        }
        public IEnumerable<bx_agent> GetAgentChildrenList(string blurname, int agentid)
        {
            var childrenlist = _agentRepository.GetAllChildrenByAgentId(agentid);
            var agentList = _agentRepository.FindAgentChildrenList(childrenlist, blurname);
            return agentList;
        }
        /// <summary>
        /// 添加经纪人信息
        /// </summary>
        public int AddAgentInfo(string agentName, int sourcce, out bxAgent item)
        {
            bx_agent agentItem = new bx_agent();
            item = new bxAgent();
            var result = _agentRepository.AddAgentInfo(agentName, sourcce, out agentItem);
            item.Id = agentItem.Id;
            item.IsUsed = agentItem.IsUsed;
            item.IsDaiLi = agentItem.IsDaiLi;
            item.IsBigAgent = agentItem.IsBigAgent;
            item.AgentName = agentItem.AgentName;
            return result;
        }
        /// <summary>
        /// 删除代理人
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public int DelAgentInfo(DeleteAgentRequest request)
        {
            var delAgent = _agentRepository.GetAgent(request.AgentId);
            if (delAgent == null)
            {
                // 代理人已经删除
                return 3;
            }
            var result = _agentRepository.DelAgentInfo(delAgent.Id, delAgent.AgentName, request.DeleteUserId, request.DeleteAccount, request.DeletePlatfrom);
            if (result == 1)
            {
                // 移除redis中的缓存
                DeleteAgentGroupFromRedis(delAgent.Id, delAgent.ParentAgent, delAgent.TopAgentId, delAgent.agent_level);
                try
                {
                    //删除代理人是推送消息
                    _agentRepository.PushSignal(request.AgentId, 3);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("DelAgentInfo_servise:"+JsonHelper.Serialize(request)+";【删除业务员】推送消息发送异常：" + ex);
                }
            }
            return result;
        }

        public AgentSourceViewModel GetAgentSource(BaseVerifyRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AgentSourceViewModel();
            //request.IsBj = 1;
            List<AgentCity> agentCity = GetSourceList(string.Format("http://{0}:{1}/", uri.Host, uri.Port), request.Agent);
            viewModel.AgentCity = agentCity;
            return viewModel;
        }


        /// <summary>
        /// 获取渠道的source集合
        /// </summary>
        /// <param name="url"></param>
        /// <param name="agentId">顶级代理Id</param>
        /// <param name="isBj">是否报价</param>
        /// <returns></returns>
        public List<AgentCity> GetSourceList(string url, int agentId)
        {
            //var viewModel = new AgentSourceViewModel();

            var cityData = "ExternalApi_City_Find";
            var citys = CacheProvider.Get<List<bx_city>>(cityData);
            if (citys == null)
            {
                citys = _cityRepository.FindAllCity();
                CacheProvider.Set(cityData, citys, 10800);
            }

            //当前agent下所有的数据
            var agentAllData = string.Format("agent_source_key_{0}", agentId);
            var agentConfigs = CacheProvider.Get<List<bx_agent_config>>(agentAllData);
            if (agentConfigs == null)
            {
                agentConfigs = _agentConfigRepository.Find(agentId);
                CacheProvider.Set(agentAllData, agentConfigs, 10800);
            }

            //取其所有的city
            var listCity = new List<int>();
            if (agentConfigs != null)
                listCity = agentConfigs.Select(i => i.city_id.Value).Distinct().ToList();

            //结果
            var citylist = new List<AgentCity>();
            AgentCity agentCity;
            //城市下的city循环取source
            if (!listCity.Any())
            {
                return citylist;
            }
            List<int> listSource;
            Source agentSource;
            List<Source> sourcelist;
            for (int i = 0; i < listCity.Count; i++)
            {
                listSource = new List<int>();
                listSource =
                    agentConfigs.Where(m => m.city_id.Value == listCity[i])
                        .Select(s => s.source.Value)
                        .Distinct()
                        .ToList();
                if (!listSource.Any()) continue;
                sourcelist = new List<Source>();
                for (int j = 0; j < listSource.Count; j++)
                {
                    agentSource = new Source()
                    {
                        Id = listSource[j],
                        Name = listSource[j].ToEnumDescription(typeof(EnumSource)),
                        NewId = SourceGroupAlgorithm.GetNewSource(listSource[j])
                    };
                    sourcelist.Add(agentSource);
                }

                if (citys.FirstOrDefault(n => n.id == listCity[i]) == null) continue;

                agentCity = new AgentCity
                {
                    AgentSource = sourcelist,
                    CityId = listCity[i],
                    CityName = citys.FirstOrDefault(n => n.id == listCity[i]).city_name
                };

                citylist.Add(agentCity);
            }

            ////获取城市列表
            //var agentCityKey = string.Format("ExternalApi_{0}_ConfigCity_Find", agentId);
            //var listTempCity = CacheProvider.Get<List<bx_agent_config>>(agentCityKey);
            //var listCity = new List<int>();
            //if (listTempCity == null)
            //{
            //    listTempCity = _agentConfigRepository.FindFullCity(agentId, isBj);
            //    CacheProvider.Set(agentCityKey, listTempCity, 10800);
            //}
            //if (listTempCity != null)
            //    listCity = listTempCity.Select(i => i.city_id.Value).Distinct().ToList();

            ////获取所有source列表
            //var agentSourceKey = string.Format("agent_source_key_{0}", agentId);
            //var listSource = CacheProvider.Get<List<bx_agent_config>>(agentSourceKey);
            //if (listSource == null)
            //{
            //    listSource = _agentConfigRepository.Find(agentId, isBj).ToList();
            //    CacheProvider.Set(agentSourceKey, listSource, 10800);
            //}
            //var citylist = new List<AgentCity>();
            //var agentCity = new AgentCity();
            //var sourcelist = new List<Source>();
            //var agentSource = new Source();
            //var tempSourceList = new List<bx_agent_config>();
            ////根据城市获取source列表
            //if (!listCity.Any()) return citylist;
            //foreach (var c in listCity)
            //{
            //    tempSourceList = new List<bx_agent_config>();
            //    tempSourceList = listSource.Where(i => i.city_id == c && i.source.HasValue).OrderBy(o => o.source.Value).Distinct().ToList();
            //    sourcelist = new List<Source>();
            //    if (tempSourceList.Any())
            //    {
            //        foreach (var s in tempSourceList)
            //        {
            //            agentSource = new Source();
            //            agentSource.Id = s.source.Value;
            //            agentSource.Name = s.source.Value.ToEnumDescription(typeof(EnumSource));
            //            agentSource.NewId = AutoSource.GetNewSource(s.source.Value);
            //            //agentSource.ImageUrl = string.Format("{0}/Images/company/{1}.png", url, s.source.Value);
            //            sourcelist.Add(agentSource);
            //        }
            //    }
            //    agentCity = new AgentCity();
            //    agentCity.AgentSource = sourcelist;
            //    agentCity.CityId = c;
            //    agentCity.CityName = _cityRepository.FindCity(c).city_name;
            //    citylist.Add(agentCity);
            //}
            return citylist;
        }


        public HebaoRateViewModel GetHebaoRate(GetHebaoRateRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new HebaoRateViewModel();

            //展示费率，默认不展示
            bx_agent bxAgent = _agentRepository.GetAgent(request.ChildAgent);
            viewModel.IsShowCalc = bxAgent != null ? (bxAgent.IsShowCalc ?? 1) : 1;

            if (viewModel.IsShowCalc == 0)
            {//0展示
                bx_hebaodianwei hebao = _heBaoDianWeiRepository.GetHeBao(request.Buid, request.Source);
                if (hebao != null)
                {
                    viewModel.BizSysRate = hebao.system_biz_rate.HasValue
                                ? Convert.ToDecimal(hebao.system_biz_rate.Value)
                                       : 0;
                    viewModel.ForceSysRate = hebao.system_force_rate.HasValue
                        ? Convert.ToDecimal(hebao.system_force_rate.Value)
                        : 0;
                    //优惠费率
                    if (hebao.agent_id == hebao.parent_agent_id)
                    {
                        viewModel.BenefitRate = hebao.zhike_biz_rate.HasValue
                            ? Convert.ToDecimal(hebao.zhike_biz_rate.Value)
                            : 0;
                    }
                    else
                    {
                        viewModel.BenefitRate = hebao.agent_biz_rate.HasValue
                            ? Convert.ToDecimal(hebao.agent_biz_rate.Value)
                            : 0;
                    }
                }
            }

            return viewModel;
        }

        public bool IsExistAgentInfo(int id)
        {
            return _agentRepository.IsExistAgentInfo(id);
        }

        public bool HasAgent(string mobile, string shareCode)
        {
            return _agentRepository.HasAgent(mobile, shareCode);
        }

        /// <summary>
        /// 获取顶级经纪人Id
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public int GetTopAgentIdByAgentId(int agentId)
        {
            return _agentRepository.GetTopAgentIdByAgentId(agentId);
        }

        public List<AgentData> GetAgentData(IEnumerable<string> agentIds)
        {
            return _agentRepository.GetAgentData(agentIds);
        }


        public BusinessStatisticsViewModel GetBusinessStatistics(int agentId, DateTime startTime, DateTime endTime)
        {
            var agentIds = GetSonsListFromRedisToString(agentId);// GetSonsList(agentId);
            if (agentIds.Count == 0) return new BusinessStatisticsViewModel();
            return _agentRepository.GetBusinessStatistics(agentIds, startTime, endTime);
        }

        /// <summary>
        /// 战败统计
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DefeatAnalyticsViewModel GetDefeatAnalytics(int agentId, DateTime startTime, DateTime endTime)
        {
            var agentIds = GetSonsListFromRedisToString(agentId);// GetSonsList(agentId);

            var defeatanalysis = _agentRepository.GetDefeatAnalytics(agentIds, startTime, endTime);
            for (var date = startTime.Date; date <= endTime; date = date.AddDays(1))
            {
                if (!defeatanalysis.Exists(i => i.DataInTime == date))
                    defeatanalysis.Add(new DefeatAnalysis
                    {
                        DataInTime = date,
                        Count = 0
                    });
            }
            defeatanalysis = defeatanalysis.OrderBy(i => i.DataInTime).ToList();

            var viewModel = new DefeatAnalyticsViewModel
            {
                StrCount = string.Join(",", defeatanalysis.ToArray(x => x.Count)),
                StrDataInTime = string.Join(",", defeatanalysis.ToArray(x => x.DataInTime))
            };
            return viewModel;

        }

        /// <summary>
        /// 原因分析
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DefeatAnalyticsViewModel GetReasonAnalytics(int agentId, DateTime startTime, DateTime endTime)
        {
            var agentIds = GetSonsListFromRedisToString(agentId);// GetSonsList(agentId);

            var defeatanalysis = _agentRepository.GetReasonAnalytics(agentIds, startTime, endTime);

            var viewModel = new DefeatAnalyticsViewModel();
            foreach (var i in defeatanalysis)
            {
                viewModel.DefeatReasons.Add(i.DefeatReason, i.Count ?? 0);
            }
            return viewModel;
        }

        /// <summary>
        /// 业务员战败数据统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DefeatAnalyticsViewModel GetAgentAnalytics(AgentAnalyticsRequest request)
        {
            //查询所有已启用的代理人
            IList<string> agentIds = _agentRepository.GetUsedSons(request.AgentId);
            int totalCount;
            if (string.IsNullOrWhiteSpace(request.SearchTxt))
                totalCount = agentIds.Count;
            else
                using (EntityContext context = new EntityContext())
                {
                    var list = agentIds.ToList().ConvertAll(int.Parse);
                    totalCount = context.bx_agent.Count(x => x.AgentName.Contains(request.SearchTxt) && list.Contains(x.Id));
                }
            var defeatAnalysis = string.Empty;
            var json = _agentRepository.GetAgentAnalytics(request, agentIds, ref defeatAnalysis);

            var viewModel = new DefeatAnalyticsViewModel
            {
                DefeatReasons = null,
                AgentAnalyticJson = json,
                TotalCount = totalCount,
                DefeatReasonList = defeatAnalysis
            };
            return viewModel;
        }

        /// <summary>
        /// 查询趋势图统计数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public TrendMapViewModel GetTrendMap(int agentId, DateTime startTime, DateTime endTime)
        {
            var agentIds = GetSonsListFromRedisToString(agentId);// GetSonsList(agentId);
            if (agentIds.Count == 0) return new TrendMapViewModel();

            var listAgent = _agentRepository.GetTrendMap(agentIds, startTime, endTime);
            for (var date = startTime.Date; date <= endTime; date = date.AddDays(1))
            {
                if (!listAgent.Exists(i => i.DataInTime == date))
                    listAgent.Add(new AgentData { DataInTime = date });
            }
            listAgent = listAgent.OrderBy(i => i.DataInTime).ToList();
            var viewModel = new TrendMapViewModel
            {
                QuotaCars = string.Join(",", listAgent.ToArray(i => i.QuoteCarCount)),
                Quotes = string.Join(",", listAgent.ToArray(i => i.QuoteCount)),
                MessagesSent = string.Join(",", listAgent.ToArray(i => i.SmsSendCount)),
                VisitsCount = string.Join(",", listAgent.ToArray(i => i.ReturnVisitCount)),
                AppointmentToStore = string.Join(",", listAgent.ToArray(i => i.AppointmentCount)),
                OrderCount = string.Join(",", listAgent.ToArray(i => i.OrderCount)),
                BatchRenewalCount = string.Join(",", listAgent.ToArray(i => i.BatchRenewalCount)),
                SingleCount = string.Join(",", listAgent.ToArray(i => i.SingleCount)),
                FailedOrderCount = string.Join(",", listAgent.ToArray(i => i.DefeatCount)),
                StatisticsDate = string.Join(",", listAgent.ToArray(i => i.DataInTime.ToString("yyyy-MM-dd"))),
            };

            return viewModel;
        }

        /// <summary>
        /// 获取分页的agent统计数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isDesc"></param>
        /// <param name="orderBy"></param>
        /// <param name="curPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchTxt"></param>
        /// <param name="isByLevel"></param>
        /// <returns></returns>
        public AgentStatisticsViewModel GetAgentDataByPage(int agentId, DateTime startTime, DateTime endTime,
            bool isDesc, string orderBy, int curPage, int pageSize, string searchTxt, bool isByLevel)
        {
            var viewModel = new AgentStatisticsViewModel();
            switch (orderBy.ToLower())
            {
                case "quotecarcount":
                    orderBy = "QuoteCarCount";
                    break;
                case "quotecount":
                    orderBy = "QuoteCount";
                    break;
                case "smssendcount":
                    orderBy = "SmsSendCount";
                    break;
                case "returnvisitcount":
                    orderBy = "ReturnVisitCount";
                    break;
                case "appointmentcount":
                    orderBy = "AppointmentCount";
                    break;
                case "singlecount":
                    orderBy = "SingleCount";
                    break;
                case "ordercount":
                    orderBy = "OrderCount";
                    break;
                case "failedordercount":
                    orderBy = "DefeatCount";
                    break;
                case "batchrenewalcount":
                    orderBy = "BatchRenewalCount";
                    break;
                case "agentlevel":
                    orderBy = "AgentLevel";
                    break;
                default:
                    orderBy = "AgentId";
                    break;
            }
            //var childAgent = _agentRepository.GetSonsList(agentId);
            int totalCount = 0;
            var bsList = _agentRepository.GetAgentDataByPage(agentId, startTime, endTime, isDesc, orderBy, curPage, pageSize, searchTxt, isByLevel, ref totalCount);
            //if (isByLevel)
            //{
            //    foreach (var item in bsList)
            //    {
            //        var childAgentList = new List<int> { 102, 123 };
            //        var childList = bsList.Where(t => childAgentList.Contains(t.AgentId)).ToList();
            //        if (childList.Count > 0)
            //        {
            //            item.BatchRenewalCount += childList.Sum(t => t.BatchRenewalCount);
            //            item.QuoteCarCount += childList.Sum(t => t.QuoteCarCount);
            //            item.QuoteCount += childList.Sum(t => t.QuoteCount);
            //            item.SmsSendCount += childList.Sum(t => t.SmsSendCount);
            //            item.ReturnVisitCount += childList.Sum(t => t.ReturnVisitCount);
            //            item.AppointmentCount += childList.Sum(t => t.AppointmentCount);
            //            item.SingleCount += childList.Sum(t => t.SingleCount);
            //            item.DefeatCount += childList.Sum(t => t.DefeatCount);
            //            item.OrderCount += childList.Sum(t => t.OrderCount);
            //        }
            //    }
            //    bsList = Infrastructure.Helper.Extends.IListOrderBy<AgentData>(bsList, orderBy, isDesc).ToList<AgentData>().Skip((curPage - 1) * pageSize).Take(pageSize).ToList();
            //}
            viewModel.TotalCount = totalCount;
            viewModel.AgentDataList = bsList;
            return viewModel;
        }

        /// <summary>
        /// 获取单个业务员的统计
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public AgentStatisticsViewModel GetSingleAgentData(int agentId, DateTime startTime, DateTime endTime)
        {
            var viewModel = new AgentStatisticsViewModel();
            var bsList = _agentRepository.GetSingleAgentData(agentId, startTime, endTime);

            viewModel.TotalCount = bsList.Count;
            viewModel.AgentDataList = bsList;

            return viewModel;
        }

        public SfH5ViewModel GetAgentData4SfH5ByPage(int groupId, DateTime startTime, DateTime endTime, bool isDesc, string orderBy, int pageIndex, int pageSize, string serachText)
        {
            var viewModel = new SfH5ViewModel();
            switch (orderBy.ToLower())
            {
                case "insurecount":
                    orderBy = "InsureCount";
                    break;
                case "quotecount":
                    orderBy = "RenewalCount";
                    break;
                case "ratio":
                    orderBy = "Ratio";
                    break;
                default:
                    orderBy = "AgentId";
                    break;
            }
            int totalCount = 0;
            endTime = endTime.AddDays(1);
            var dataList = _agentRepository.GetAgentData4SfH5ByPage(groupId, startTime, endTime, isDesc, orderBy, pageIndex, pageSize, serachText, ref totalCount);
            viewModel.SfAgentDatas = dataList;
            viewModel.PageIndex = pageIndex;
            viewModel.PageSize = pageSize;
            viewModel.TotalCount = totalCount;
            return viewModel;
        }

        public SfH5AverageViewModel GetAverageData(int groupId, DateTime startTime, DateTime endTime)
        {
            endTime = endTime.AddDays(1);
            return _agentRepository.GetAverageData(groupId, startTime, endTime);
        }

        public int GetAgentEffectiveCallDuration(int agentId)
        {
            return _agentRepository.GetAgentEffectiveCallDuration(agentId);
        }
        public bool UpdateAgentEffectiveCallDuration(int agentId, int effectiveCallDuration)
        {
            return _agentRepository.UpdateAgentEffectiveCallDuration(agentId, effectiveCallDuration);
        }

        public void InitAgentGroupToRedis()
        {
            var agentGroups = _agentRepository.GetAgentGroups();
            _cacheClient.Remove("agentGroupKey");
            foreach (var item in agentGroups)
            {
                _cacheClient.Set("agentGroupKey", item.AgentIdKey.ToString(), item.AgentIdsValue);
            }
        }
        public List<int> GetSonsListFromRedis(int currentAgentId, bool isHasSelf = true)
        {
            List<int> agentIds = new List<int>();
            if (ConfigurationManager.AppSettings["IsGetFromRedis"].ToString() != "1")
            {
                agentIds = _agentRepository.GetSonListByDb(currentAgentId, isHasSelf);
            }
            else
            {
                agentIds = AddSpecifiedAgentGroupToRedis(new List<int> { currentAgentId }, () =>
                   {
                       return _cacheClient.Get<List<int>>("agentGroupKey", currentAgentId.ToString());
                   });
                if (!isHasSelf)
                {
                    agentIds.Remove(currentAgentId);
                }
            }
            return agentIds;
        }
        public List<int> GetDelSonListByDb(int currentAgent)
        {
            return _agentRepository.GetDelSonListByDb(currentAgent);
        }
        public List<int> GetAllSonsListFromRedis(int currentAgentId, bool isHasSelf = true)
        {
            List<int> agentIds = new List<int>();
            if (ConfigurationManager.AppSettings["IsGetFromRedis"].ToString() != "1")
            {
                agentIds = _agentRepository.GetAllSonListByDb(currentAgentId, isHasSelf);
            }
            else
            {
                agentIds = AddSpecifiedAgentGroupToRedis(new List<int> { currentAgentId }, () =>
                {
                    return _cacheClient.Get<List<int>>("agentGroupKey", currentAgentId.ToString());
                });
                if (!isHasSelf)
                {
                    agentIds.Remove(currentAgentId);
                }
            }
            return agentIds;
        }
        /// <summary>
        /// 将GetSonsListFromRedis返回的int转换成string
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <param name="isHasSelf"></param>
        /// <returns></returns>
        public List<string> GetSonsListFromRedisToString(int currentAgentId, bool isHasSelf = true)
        {
            return GetSonsListFromRedis(currentAgentId, isHasSelf).Select(o => o.ToString()).ToList();
        }
        public List<string> GetAllSonsListFromRedisToString(int currentAgentId, bool isHasSelf = true)
        {
            return GetAllSonsListFromRedis(currentAgentId, isHasSelf).Select(o => o.ToString()).ToList();
        }
        public void AddAgentGroupToRedis(int currentAgentId, int parentAgentId, int topAgentId, int agentLevel)
        {

            if (agentLevel == 1 || agentLevel == 2 || agentLevel == 3)
            {
                _cacheClient.Set("agentGroupKey", currentAgentId.ToString(), new List<int>() { currentAgentId });
            }
            if (agentLevel == 2 || agentLevel == 3)
            {
                SetAfterAdd(currentAgentId, parentAgentId);
            }
            if (agentLevel == 3)
            {

                SetAfterAdd(currentAgentId, topAgentId);
            }
        }










        public void DeleteAgentGroupFromRedis(int currentAgentId, int parentAgentId, int topAgentId, int agentLevel)
        {
            if (agentLevel == 2 || agentLevel == 3)
            {
                SetAfterRemove(currentAgentId, new List<int> { parentAgentId });
            }
            if (agentLevel == 3)
            {

                SetAfterRemove(currentAgentId, new List<int> { topAgentId });
            }
            if (agentLevel == 2 || agentLevel == 1)
            {
                _cacheClient.Remove("agentGroupKey", currentAgentId.ToString());
            }
        }

        public void UpdateAgentGroupInRedis(int currentAgentId, List<int> fromParentAgentIdKeys, int toAgentIdKey, int operationType)
        {
            if (operationType == 1)
            {
                _cacheClient.Set("agentGroupKey", currentAgentId.ToString(), new List<int>() { currentAgentId });
                SetAfterRemove(currentAgentId, fromParentAgentIdKeys);
            }
            else if (operationType == 0)
            {
                _cacheClient.Remove("agentGroupKey", currentAgentId.ToString());
                SetAfterAdd(currentAgentId, toAgentIdKey);
            }
            else
            {
                SetAfterRemove(currentAgentId, fromParentAgentIdKeys);
                SetAfterAdd(currentAgentId, toAgentIdKey);
            }
        }
        private List<int> SetAfterRemove(int currentAgentId, List<int> fromParentAgentIdKeys)
        {
            return AddSpecifiedAgentGroupToRedis(fromParentAgentIdKeys, () =>
            {
                List<int> fromAgentIdValues = new List<int>();
                foreach (var item in fromParentAgentIdKeys)
                {
                    fromAgentIdValues = _cacheClient.Get<List<int>>("agentGroupKey", item.ToString());
                    fromAgentIdValues.Remove(currentAgentId);
                    if (_cacheClient.Remove("agentGroupKey", item.ToString()))
                    {
                        _cacheClient.Set("agentGroupKey", item.ToString(), fromAgentIdValues);
                    }
                }

                return fromAgentIdValues;
            });

        }
        private List<int> SetAfterAdd(int currentAgentId, int toAgentIdKey)
        {
            return AddSpecifiedAgentGroupToRedis(new List<int> { toAgentIdKey }, () =>
            {
                var toAgentValue = _cacheClient.Get<List<int>>("agentGroupKey", toAgentIdKey.ToString());
                toAgentValue.Add(currentAgentId);
                if (_cacheClient.Remove("agentGroupKey", toAgentIdKey.ToString()))
                {
                    _cacheClient.Set("agentGroupKey", toAgentIdKey.ToString(), toAgentValue);
                }

                return toAgentValue;
            });

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <param name="parentAgentShareCode">新的父级代理人分享码</param>
        /// <param name="oldParentAgentId">原来的代理人分享码</param>
        public void UpdateAgentParentShareCodeNotifyRedis(int currentAgentId, int parentAgentShareCode, int oldParentAgentId)
        {
            var currentAgent = _agentRepository.GetAgent(currentAgentId);
            if (currentAgent == null)
                return;
            var parentAgentId = parentAgentShareCode - 1000;
            if (parentAgentId <= 0)
                return;
            var parentAgent = _agentRepository.GetAgent(parentAgentId);
            if (parentAgent == null)
                return;

            var fromParentAgentIdKeys = new List<int>() { oldParentAgentId };

            // 操作类型：1->升级，0->降级，-1->不剩不降 
            var operationType = -1;

            if (currentAgent.agent_level == 2 && parentAgent.agent_level == 2)
            {
                operationType = 0;
            }
            else if (currentAgent.agent_level == 3 && parentAgent.agent_level == 1)
            {
                operationType = 1;
            }

            UpdateAgentGroupInRedis(currentAgentId, fromParentAgentIdKeys, parentAgentId, operationType);
            return;
        }

        /// <summary>
        /// 设置代理手机号与微信同号  李金友 2017-09-08 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool SetAgentsPhoneIsWechat(SetPhoneAndWechatAgentRequest request)
        {
            bx_agent model = _agentRepository.GetAgent(request.ChildAgent);
            model.phone_is_wechat = request.PhoneAndWechat;
            return _agentRepository.UpdateAgent(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetSonAgentsViewModel GetSonAgents(BaseVerifyRequest request)
        {
            List<AgentIdAndAgentName> list = new List<AgentIdAndAgentName>();

            if (request.Agent == request.ChildAgent)
            {
                list = _agentRepository.GetByTopAgentId(request.Agent);
                // 移除顶级代理人
                list.RemoveAll(o => o.AgentId == request.Agent);
            }
            else
            {
                list = _agentRepository.GetByParentAgentId(request.ChildAgent);
            }

            var result = GetSonAgentsViewModel.GetModel(BusinessStatusType.OK);
            result.ListAgent = list;
            return result;
        }

        /// <summary>
        /// 修改代理人账号密码 2017-08-06 zky /app
        /// </summary>
        /// <param name="account"></param>
        /// <param name="passWord"></param>
        /// <param name="isUsed"></param>
        /// <param name="editAgent"></param>
        /// <returns></returns>
        public bool EditAgentInfo(string account, string passWord, int isUsed, int editAgent)
        {

            var agent = _agentRepository.GetAgent(editAgent);
            if (agent != null)
            {
                var manageruser = _manageruserRepository.Find(t => t.Name == agent.AgentAccount);
                if (manageruser != null)
                {
                    if (!string.IsNullOrEmpty(passWord))
                    {
                        manageruser.PwdMd5 = agent.AgentPassWord = passWord.Trim().ToMd5().ToLower();
                    }
                    manageruser.Name = agent.AgentAccount = account.Trim();
                    agent.IsUsed = isUsed;
                    return _manageruserRepository.Update(manageruser) && _agentRepository.UpdateAgent(agent);
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 除了当前代理人以外其他用户使用这个账号的数量 
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public int SameAccountCount(int agentId, string account)
        {
            return _agentRepository.SameAccountCount(agentId, account);
        }
        private List<int> AddSpecifiedAgentGroupToRedis(List<int> currentAgentIds, Func<List<int>> GetAgentGroupFunc)
        {
            List<int> agentIds = new List<int>();
            if (!_cacheClient.KeyExists("agentGroupKey"))
            {
                InitAgentGroupToRedis();
            }
            foreach (var item in currentAgentIds)
            {
                if (!_cacheClient.KeyExists("agentGroupKey", item.ToString()) || !_cacheClient.Get<List<int>>("agentGroupKey", item.ToString()).Any())
                {
                    agentIds = _agentRepository.GetSonListByDb(item);

                    _cacheClient.Set("agentGroupKey", item.ToString(), agentIds);
                }
                else
                {
                    agentIds = GetAgentGroupFunc();

                }
            }
            return agentIds;
        }
        /// <summary>
        /// 获取source
        /// </summary>
        /// <param name="source">source值</param>
        /// <param name="sourceType"> 0:传递新source  1：传递老source</param>
        /// <returns></returns>
        public companyRelationModel GetSource(long source, int sourceType)
        {
            var RelationModel = new companyRelationModel();
            var companyRelationModel = _agentRepository.GetSource(source, sourceType);
            //if (!_cacheClient.KeyExists("SourceKeySource") || !_cacheClient.KeyExists("SourceKeyFlags"))
            //{
            //    var companyRelationModel = _agentRepository.GetSource(source, sourceType);
            //    //0:传递新source  1：传递老source
            //    if (sourceType == 0)
            //    {
            //        if (!_cacheClient.KeyExists("SourceKeySource"))
            //        {
            //            RelationModel.source = companyRelationModel.flags;
            //            RelationModel.name = companyRelationModel.name;
            //            _cacheClient.Set("SourceKeySource", source.ToString(), RelationModel);
            //        }

            //    }
            //    else
            //    {
            //        RelationModel.source = companyRelationModel.source;
            //        RelationModel.name = companyRelationModel.name;
            //        _cacheClient.Set("SourceKeyFlags", source.ToString(), RelationModel);
            //    }
            //}
            //else
            //{
            //    if (sourceType == 0)
            //    {
            //       var SourceKeySourceModel= _cacheClient.Get<companyRelationModel>("SourceKeySource", source.ToString());
            //       RelationModel.source = SourceKeySourceModel.source;
            //       RelationModel.name = SourceKeySourceModel.name;
            //    }
            //    else
            //    {
            //        var SourceKeyFlagsModel = _cacheClient.Get<companyRelationModel>("SourceKeyFlags", source.ToString());
            //        RelationModel.source = SourceKeyFlagsModel.source;
            //        RelationModel.name = SourceKeyFlagsModel.name;
            //    }
            //}
            //0:传递新source  1：传递老source
            if (sourceType == 0)
            {
                RelationModel.source = companyRelationModel.flags;
            }
            else
            {
                RelationModel.source = companyRelationModel.source;
            }
            RelationModel.name = companyRelationModel.name;

            return RelationModel;
        }

        public List<AgentIdAndAgentName> GetAgentNames(List<int> listAgentId)
        {
            return _agentRepository.GetAgentName(listAgentId);
        }

        public IList<bx_agent> GetList(Expression<Func<bx_agent, bool>> where)
        {
            return _agentRepository.GetList(where);
        }

        /// <summary>
        /// 业务员列表批量审核 zky 2017-08-31 /crm
        /// </summary>
        /// <param name="agentIds">批量更新的代理id</param>
        /// <param name="messagePayType">短信扣费方式</param>
        /// <param name="usedStatus">启用状态</param>
        /// <param name="isShowRate">是否展示费率</param>
        /// <param name="isSubmit">是否可核保</param>
        /// <returns></returns>
        public bool AgentBatchAudit(List<int> agentIds, int messagePayType, int usedStatus, int isShowRate, int isSubmit)
        {
            return _agentRepository.AgentBatchAudit(agentIds, messagePayType, usedStatus, isShowRate, isSubmit, 0);
        }
        public List<int> GetAllAgentIds()
        {
            return _agentRepository.GetAllAgentIds();
        }
        public GetSonAgentsViewModel GetSonAgents2(int agentId, int childAgent)
        {
            List<AgentIdAndAgentName> resultList = new List<AgentIdAndAgentName>();
            List<AgentIdAndAgentName> list = new List<AgentIdAndAgentName>();
            //使用中的
            List<AgentIdAndAgentName> usedList = new List<AgentIdAndAgentName>();
            //禁用和删除的
            List<AgentIdAndAgentName> disAndDelList = new List<AgentIdAndAgentName>();
            if (agentId == childAgent)
            {
                list = _agentRepository.GetByTopAgentId2(agentId);
            }
            else
            {
                list = _agentRepository.GetByParentAgentId2(childAgent);
            }
            usedList = list.Where(a => a.IsUsed == 1).ToList();
            resultList.AddRange(usedList);
            disAndDelList = list.Where(a => a.IsUsed > 1).ToList();//删除、禁用
            //根据删除、禁用的在bx_userinfo判断是否还有数据
            List<int> disAndDel = disAndDelList.Select(a => a.AgentId).Distinct().ToList();
            List<int> disAndDelTemp = null;
            if (disAndDel!=null&&disAndDel.Count>0)
            {
                disAndDelTemp = _userInfoRepository.GetUserByAgentId(disAndDel);
            }
            List<AgentIdAndAgentName> disAndDelTempList = null;
            if (disAndDelTemp!=null&&disAndDelTemp.Count>0)
            {
               disAndDelTempList= disAndDelList.Where(a => disAndDelTemp.Contains(a.AgentId)).ToList();
               
            }
            if (disAndDelTempList != null && disAndDelTempList.Count > 0)
            {
                resultList.AddRange(disAndDelTempList);
            }

            var result = GetSonAgentsViewModel.GetModel(BusinessStatusType.OK);
            result.AgentIds = resultList.Select(x => x.AgentId).ToList();
            result.ListAgent = resultList;
            return result;
        }
        public GetSonAgentsViewModel GetSonAgents(int agentId, int childAgent)
        {
            List<AgentIdAndAgentName> list = new List<AgentIdAndAgentName>();

            if (agentId == childAgent)
            {
                list = _agentRepository.GetByTopAgentId(agentId);
                // 移除顶级代理人
                //list.RemoveAll(o => o.AgentId == request.Agent);
            }
            else
            {
                list = _agentRepository.GetByParentAgentId(childAgent);
            }

            var result = GetSonAgentsViewModel.GetModel(BusinessStatusType.OK);
            result.AgentIds = list.Select(x => x.AgentId).ToList();
            result.ListAgent = list;
            return result;
        }

        /// <summary>
        /// 获取集团账号下的机构列表 zky 2017-11-13 /crm、统计
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="authenState"></param>
        /// <param name="groupId"></param>
        /// <param name="needPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList<OrgListDto> GetOrgList(string orgName, int authenState, int groupId, bool needPage, int orgId, int pageIndex, int pageCount, out int total)
        {
            return _agentRepository.GetOrgList(orgName, authenState, groupId, needPage, orgId, pageIndex, pageCount, out total);
        }

        public IList<OrgListDto> GetAgentIdNameList(int groupId)
        {
            return _agentRepository.AgentNameIdList(groupId);
        }


        /// <summary>
        /// 是否可以更换上级代理 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="curAgent"></param>
        /// <param name="shareCode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CanChangeParentAgent(bx_agent curAgent, int shareCode, out string msg)
        {
            msg = "";
            var parentAgent = _agentRepository.GetAgent(shareCode - 1000);

            //判断shareCode是否是自己的
            if (curAgent.Id + 1000 == shareCode)
            {
                msg = "您不能设置自己为自己的邀请人，请重新输入";
                return false;
            }

            //是否在顶级下面
            if (!_agentRepository.IsTopAgentSonShareCode(curAgent.Id, shareCode))
            {
                msg = "邀请码不存在，请重新输入";
                return false;
            }
            //是否是三级
            if (_agentRepository.IsThreeShareCode(shareCode))
            {
                msg = "三级账号不能成为上级业务员，请先将其改为二级业务员";
                return false;
            }

            if (parentAgent == null)
            {
                msg = "邀请码不存在，请重新输入";
                return false;
            }
            //二级转移成三级时，判断其是否有分配权限，如果有，则无法转移
            if (curAgent.agent_level == 2 && parentAgent.agent_level == 2 && _managerRoleButtonRelationRepository.HasBtnAuth(curAgent.Id, BtnAuthType.btn_recycle.ToString()))
            {
                msg = "该业务员有分配功能，不能被转移为三级业务员，请修改角色后再转移";
                return false;
            }

            //判断这个人是否是三级
            if (_agentRepository.IsThreeAgent(curAgent.Id))
            {
                return true;
            }
            //是二级并且没有三级代理
            if (_agentRepository.IsTwoAgentAndNoSon(curAgent.Id))
            {
                return true;
            }
            msg = "该账号下有下级业务员，自己不能被设置为三级业务员";
            return false;
        }

        public CusListViewModel GetCustomerList(CustomerRequest request)
        {
            CusListViewModel viewModel = new CusListViewModel();
            var agentModel = _agentRepository.GetList(t => t.Id == request.AgentId).FirstOrDefault();
            if (agentModel == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "查询失败，用户信息不存在";
                return viewModel;
            }
            int recordCount = 0;
            var list = _agentRepository.GetAgentList(request.Name, request.Mobile, request.StateDateTime, request.EndDateTime, request.IsUsed, request.ParentAgentName, request.ParentAgentId, request.TopAgentId, request.AgentId, request.AuthenState, request.ZhenBangType, request.OnlySite, agentModel.zhen_bang_type, request.PageIndex, request.PageSize, request.TestState, out recordCount);

            foreach (var item in list)
            {
                item.RegTime = Convert.ToDateTime(item.CreateTime).ToString("yyyy-MM-dd");
                item.TestState = item.TestState == -1 ? 0 : item.TestState;//当考试状态是默认值-1时，返回0
            }

            viewModel.CustomerList = list;
            viewModel.PageIndex = request.PageIndex;
            viewModel.PageSize = request.PageSize;
            viewModel.RecordCount = recordCount;
            viewModel.NoAuditCount = list.Where(t => t.IsUsed == 0).Count();
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";

            return viewModel;
        }

        public CusListViewModel GetCustomerListCount(CustomerRequest request)
        {
            CusListViewModel viewModel = new CusListViewModel();
            var agentModel = _agentRepository.GetAgent(request.AgentId);
            if (agentModel == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "查询失败，用户信息不存在";
                return viewModel;
            }
            viewModel.RecordCount = _agentRepository.GetAgetListCount(request.Name, request.Mobile, request.StateDateTime, request.EndDateTime, request.IsUsed, request.ParentAgentName, request.ParentAgentId, request.TopAgentId, request.AgentId, request.AuthenState, request.ZhenBangType, request.OnlySite, agentModel.zhen_bang_type);
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            return viewModel;
        }

        /// <summary>
        /// 判断是否是顶级代理
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool IsTopAgentId(int agentId)
        {
            string topAgent = string.Empty;
            //获取顶级代理
            topAgent = _agentRepository.GetTopAgentId(agentId);
            if (!string.IsNullOrEmpty(topAgent))
            {
                //跟自己比较，如果是顶级，返回true
                return topAgent.Contains(agentId.ToString());
            }
            return false;
        }

        public List<bx_agent> GetAgentIdAndNameByGroupId(string groupIds)
        {
            return _agentRepository.GetAgentIdAndNameByGroupId(groupIds);
        }


        public NewAgentSourceViewModel NewGetAgentSource(BaseVerifyRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new NewAgentSourceViewModel();
            //request.IsBj = 1;
            List<NewAgentCity> agentCity = GetNewSourceList(string.Format("http://{0}:{1}/", uri.Host, uri.Port), request.Agent);
            viewModel.AgentCity = agentCity;
            return viewModel;
        }

        /// <summary>
        /// 获取渠道的source集合
        /// </summary>
        /// <param name="url"></param>
        /// <param name="agentId">顶级代理Id</param>
        /// <returns></returns>
        public List<NewAgentCity> GetNewSourceList(string url, int agentId)
        {
            var viewModel = new AppAgentSourceViewModel();

            var agentNewCityKey = string.Format("ExternalApi_ConfigCity_FindByAgent_{0}", agentId);

            var cityAgentList = CacheProvider.Get<List<NewAgentCity>>(agentNewCityKey);
            if (cityAgentList == null)
            {
                cityAgentList = new List<NewAgentCity>();
                var agentListSource = _agentConfig.FindAgentConfigByAgent(agentId);
                var listCitys = agentListSource.Select(x => x.CityId).Distinct().ToList();
                foreach (int city in listCitys)
                {
                    var agentSourceList = agentListSource.Where(i => i.CityId == city).OrderBy(o => o.Id).ToList();
                    var sourcelists = new List<NewSource>();
                    if (agentSourceList.Any())
                    {
                        foreach (var s in agentSourceList)
                        {
                            if (sourcelists.Where(x => x.Id == s.Id).Count() == 0)
                            {
                                var agentSources = new NewSource
                                {
                                    Id = s.Id,
                                    Name = s.Name,
                                    NewId = SourceGroupAlgorithm.GetNewSource(s.Id),
                                    ImageUrl = string.Format("{0}/Images/company/{1}.png", url, s.Id)
                                };
                                sourcelists.Add(agentSources);
                            }

                        }
                    }
                    var agentCitys = new NewAgentCity
                    {
                        AgentSource = sourcelists,
                        CityId = city,
                        CityName = agentListSource.FirstOrDefault(i => i.CityId == city).CityName
                    };
                    cityAgentList.Add(agentCitys);
                }

                if (cityAgentList.Count > 0)
                {
                    CacheProvider.Set(agentNewCityKey, cityAgentList, 10800);
                }
            }

            return cityAgentList;
        }

        public int GetSonAgentCountByTopAgentId(int topAgentId, bool hasSelf = true)
        {
            //var key = "ChildAgentCount" + topAgentId;

            //var childAgentCount = CacheProvider.Get<int>(key);
            //if (childAgentCount == 0)
            //{
            var childAgentCount = GetSonsListFromRedis(topAgentId).Count;
            //_agentRepository.GetChildAgentCountByTopAgentId(topAgentId);
            //    CacheProvider.Set(key, childAgentCount, 60 * 30);
            //}
            return childAgentCount;
        }

        /// <summary>
        /// 获取代理人配置信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bx_agent_setting GetAgentSettingModel(int agentId)
        {
            return _agentRepository.GetAgentSettingModel(agentId);
        }

        /// <summary>
        /// 获取tx_agent表中的顶级信息
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public tx_agent GetBusinessModel(int topAgentId)
        {
            return _agentRepository.GetBusinessModel(topAgentId);
        }

        public List<bx_agent_config> GetAgentConfigs(int agentId, int modelType)
        {

            return _agentRepository.GetAgentConfigs(agentId, modelType);
        }

        public List<bx_agent> GetAgentsByAgentIdAndModelTypeAndSearchType(int agentId, int modelType, int searchType)
        {

            return
                _agentRepository.GetAgentsByAgentIdAndModelTypeAndSearchType(agentId, modelType, searchType);


        }

        public SonAndGrandsonViewModel AgentSonAndGrandson(int agentId, string createTime)
        {
            SonAndGrandsonViewModel viewModel = new SonAndGrandsonViewModel();
            var agentItem = _agentRepository.GetList(t => t.Id == agentId).FirstOrDefault();
            if (agentItem == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "代理人不存在";
                return viewModel;
            }
            DateTime tempTime = DateTime.Now;
            if (!string.IsNullOrEmpty(createTime) && DateTime.TryParse(createTime, out tempTime))
            {
                //生产开始注册时间和结束注册时间 
                int days = DateTime.DaysInMonth(tempTime.Year, tempTime.Month);
                DateTime startCreateTime = DateTime.Parse(createTime + "-01 00:00:00");
                DateTime endCreateTime = DateTime.Parse(createTime + "-" + days + " 23:59:59");
                viewModel.SonList = _agentRepository.GetList(t => t.ParentAgent == agentItem.Id && t.CreateTime >= startCreateTime && t.CreateTime <= endCreateTime).Select(t => new LowerAgent
                {
                    Id = t.Id,
                    AgentLevel = t.agent_level,
                    LevelInTeam = 2
                }).ToList();
            }
            else
            {
                viewModel.SonList = _agentRepository.GetList(t => t.ParentAgent == agentItem.Id).Select(t => new LowerAgent
                {
                    Id = t.Id,
                    AgentLevel = t.agent_level,
                    LevelInTeam = 2
                }).ToList();
            }

            var sonIdList = viewModel.SonList.Select(t => t.Id).ToList();
            viewModel.GrandsonList = _agentRepository.GetList(t => sonIdList.Contains(t.ParentAgent)).Select(t => new LowerAgent
            {
                Id = t.Id,
                AgentLevel = t.agent_level,
                LevelInTeam = 3
            }).ToList();

            viewModel.BusinessStatus = 200;
            return viewModel;
        }



        public int VerificationThirdAccount(int agentId)
        {
            return _agentRepository.VerificationThirdAccount(agentId);
        }

        /// <summary>
        /// 放在where后面，用于筛选数据是属于当前代理人和他下级代理人的数据
        /// </summary>
        /// <param name="RoleType">角色类型：3系统管理员，4：管理员</param>
        /// <param name="ChildAgent">当前代理人</param>
        /// <param name="Agent">顶级代理人</param>
        /// <returns></returns>
        public string GetAgentWhere(int RoleType, int ChildAgent, int Agent)
        {
            return _agentRepository.GetAgentWhere(RoleType, ChildAgent, Agent);
        }
        public MobileStatisticsBaseVM<MobileDefeatAnalyticsVM> GetDefeatAnalytics4Mobile(DateTime startTime, DateTime endTime, int agentId, int pageIndex, int pageSize, string searchText, string categoryName)
        {
            int totalCount = 0;
            var result = _agentRepository.GetDefeatAnalytics4Mobile(startTime, endTime, agentId, pageIndex, pageSize, searchText, categoryName, out totalCount);
            return new MobileStatisticsBaseVM<MobileDefeatAnalyticsVM> { TotalCount = totalCount, DataList = result, PageIndex = pageIndex, PageSize = pageSize };
        }

        /// <summary>
        /// 查询的时候根据agent集合来查询。
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public string GetConditionSql(Tuple<List<int>, string> tuple)
        {
            return _agentRepository.GetConditionSql(tuple);
        }
        public DefeatAnalyticsViewModel GetReasonAnalytics4Mobile(int agentId, DateTime startTime, DateTime endTime, string categoryName, int isViewAllData)
        {
            var defeatanalysis = _agentRepository.GetReasonAnalytics4Mobile(agentId, startTime, endTime, categoryName, isViewAllData);

            var viewModel = new DefeatAnalyticsViewModel();
            foreach (var i in defeatanalysis)
            {
                viewModel.DefeatReasons.Add(i.DefeatReason, i.Count ?? 0);
            }
            return viewModel;
        }

        public List<string> GetOtherAgentIdList(int currentAgentId, int flag, bool isHasSelf = true)
        {
            List<int> agentIds = new List<int>();
            agentIds = _agentRepository.GetJuniorAgentIdList(currentAgentId, flag, isHasSelf);
            return agentIds.Select(o => o.ToString()).ToList();
        }
        public List<string> GetOtherAgentList(int AgentId, int TopAgentId)
        {
            List<int> agentIds = new List<int>();
            agentIds = _agentRepository.GetOtherAgentList(AgentId, TopAgentId);
            return agentIds.Select(o => o.ToString()).ToList();
        }
        public bool VerifySecond(int agentId, int agentLevel)
        {
            bx_agent model = _agentRepository.VerifySecond(agentId, agentLevel);
            if (model == null || model.Id <= 0)
            {
                return false;
            }
            return true;
        }

        public bx_agent VerifyLevel(int agentId)
        {
            bx_agent model = _agentRepository.VerifyLevel(agentId);
            return model;
        }

        #region 短信设置:微信同号、短信抬头展示门店名称、展示免责提示

        /// <summary>
        /// 获取短信设置
        /// </summary>
        /// <param name="agentId">经济人ID</param>
        /// <returns></returns>
        public ShortMsgSettingResponse GetShortMsgSetting(int agentId)
        {
            ShortMsgSettingResponse response = new ShortMsgSettingResponse();
            try
            {
                //根据agentId从bx_agent表中获取phone_is_wechat（手机号是否与微信同号）
                //从bx_agent_setting表中获取StoreName（门店名称）、DisclaimerTips（免责提示）
                response = _agentRepository.GetShortMsgSetting(agentId);
            }
            catch (Exception ex)
            {
                logInfo.Error(string.Format("AgentService-GetShortMsgSetting：入参【agentid={0}】 发生异常[{1}]", agentId, ex.Message));
                response = null;

            }
            return response;
        }

        /// <summary>
        /// 设置短信设置
        /// </summary>
        /// <param name="request">短信设置数据模型</param>
        /// <returns></returns>
        public bool SetShortMsgSetting(ShortMsgSettingRequest request)
        {
            bool setResult = false;
            try
            {
                //参数null值都设置为空
                if (request == null)
                {
                    logInfo.Info("AgentService-SetShortMsgSetting：传入参数为null");
                    return false;
                }
                request.StoreName = request.StoreName == null ? "" : request.StoreName;
                request.DisclaimerTips = request.DisclaimerTips == null ? "" : request.DisclaimerTips;

                //bx_agent_setting表中是否有数据记录，无需要插入新数据后再更新，有直接更新即可
                var agent = _agentRepository.GetAgent(request.AgentId);
                var agentSetting = _agentRepository.GetAgentSettingModel(request.AgentId);
                if (agent != null && agentSetting == null)
                {
                    //向bx_agent_setting表中插入新数据
                    bx_agent_setting setting = new bx_agent_setting();
                    //2018-09-07 张克亮 修复agent.RegType为null时取默认值0
                    setting.reg_type = agent.RegType == null ? 0 : (int)agent.RegType;
                    setting.agent_id = request.AgentId;
                    setting.settlement_type = 0;
                    setting.struct_type = 0;
                    setting.hide_carVIN = 0;
                    setting.hide_certificate = 0;
                    setting.can_multiple_quote = 0;
                    setting.desensitization = 0;
                    setting.TestState = -1;
                    setting.hebao_insure = 0;
                    setting.is_open_tuixiu = 0;
                    setting.create_time = DateTime.Now;
                    _agentRepository.AddAgentSetting(setting);
                }

                //根据agentId更新bx_agent表中phone_is_wechat（手机号是否与微信同号）
                //更新bx_agent_setting表中StoreName（门店名称）、DisclaimerTips（免责提示）
                setResult = _agentRepository.SetShortMsgSetting(request);
            }
            catch (Exception ex)
            {
                logInfo.Error(string.Format("AgentService-SetShortMsgSetting：入参【agentid={0},PhoneIsWechat={1},StoreName={2},DisclaimerTips={3},】 发生异常[{4}]", request.AgentId, request.PhoneIsWechat, request.StoreName, request.DisclaimerTips, ex.Message));
                setResult = false;

            }
            return setResult;
        }

        public List<int> GetChildAgentIdByTopAgentIds(List<int> topAgentIds)
        {
            return _agentRepository.GetChildAgentIdByTopAgentIds(topAgentIds);
        }

        #endregion
    }
}
