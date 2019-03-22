using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using AppHelpers = BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppResponse = BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse;
using log4net;
using ServiceStack.Text;
using AppInterfaces = BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using AppIRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class AgentService : CommonBehaviorService, AppInterfaces.IAgentService
    {
        private static readonly string _rateCenter = System.Configuration.ConfigurationManager.AppSettings["RateCenter"];

        private AppIRepository.IAgentRepository _agentRepository;
        private ISmsContentRepository _smsContentRepository;
        private ICityRepository _cityRepository;

        private AppIRepository.IAgentConfigRepository _agentConfig;
        private IHeBaoDianWeiRepository _hebaoDianwei;
        private ILog logError = LogManager.GetLogger("ERROR");

        private AppIRepository.ITagFlagRepository _tagFlagRepository;

        public AgentService(AppIRepository.IAgentRepository agentRepository, ICacheHelper cacheHelper,
            AppIRepository.IAgentConfigRepository agentConfigRepository, IHeBaoDianWeiRepository hebaoDianweiRepository, ISmsContentRepository smsContentRepository, ICityRepository cityRepository, AppIRepository.ITagFlagRepository tagFlagRepository)
            : base(agentRepository, cacheHelper)
        {
            _tagFlagRepository = tagFlagRepository;
            _agentRepository = agentRepository;
            _agentConfig = agentConfigRepository;
            _hebaoDianwei = hebaoDianweiRepository;
            _smsContentRepository = smsContentRepository;
            _cityRepository = cityRepository;
        }

        /// <summary>
        /// 添加代理人信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public AppResponse.GetAgentResponse AddAgent(AppRequest.PostAddAgentRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.GetAgentResponse();
            bx_agent agent = new bx_agent();
            //备注：临时屏蔽，暂时先不上
            agent = _agentRepository.GetAgentIsTop(request.TopParentAgent);
            if (agent == null)
            {
                response.ErrCode = -4;//顶级代理人不存在
                return response;
            }
            if (!_agentRepository.IsContainSon(request.TopParentAgent, request.AgentId))
            {
                response.ErrCode = -5;//顶级代理下面不包含子集代理
                return response;
            }
            agent = new bx_agent();
            agent = _agentRepository.GetAgentByPhoneTopAgent(request.Mobile, request.TopParentAgent);
            if (agent != null)
            {
                response.ErrCode = -1;//手机号已存在
                return response;
            }
            agent = new bx_agent();
            agent = _agentRepository.GetAgentByTopParentAgent(request.OpenId, request.TopParentAgent);
            if (agent != null)
            {
                response.ErrCode = -2;//OpenId已存在
                response.agent = ToModel(agent, request.TopParentAgent);
                return response;
            }
            int agentlevel = _agentRepository.GetAgentLevel(GetAgent(request.AgentId).ParentAgent);
            if (agentlevel > 2)
            {
                response.ErrCode = -3;//不允许新增下一级代理
                return response;
            }
            //插入bx_agent
            bx_agent model = new bx_agent();
            model.AgentName = request.AgentName;
            model.Mobile = request.Mobile;
            model.OpenId = request.OpenId;
            model.ParentAgent = request.AgentId;//有问题
            #region 分配默认值
            model.CreateTime = DateTime.Now;
            model.IsBigAgent = 1;
            model.CommissionType = 0;
            model.IsUsed = 0;
            model.IsGenJin = 0;
            model.IsDaiLi = 0;
            model.IsShow = 0;
            model.IsShowCalc = 0;
            model.IsLiPei = 0;
            model.AgentType = 0;
            model.MessagePayType = 1;//发短信走当前代理人
            model.RegType = 0;//小龙说默认走单店 暂无实际需要
            model.IsQuote = 1;
            model.IsSubmit = 1;
            #endregion
            long agentId = _agentRepository.Add(model);

            if (agentId > 0)
            {
                //根据agentId更新ShareCode
                int update = 0;
                update = _agentRepository.Update(agentId);
                if (update > 0)
                {
                    //存入缓存，并读取该记录
                    //string agentCacheKey = string.Format("agent_cacke_key-{0}-{1}", agentId, request.AgentId
                    //    );
                    var agentCache = _agentRepository.GetAgent((int)agentId);
                    //HttpRuntime.Cache.Insert(agentCacheKey, agentCache, null, DateTime.Now.AddHours(12), TimeSpan.Zero, CacheItemPriority.High, null);
                    response.agent = ToModel(agentCache, request.TopParentAgent);
                }
            }
            return response;
        }

        /// <summary>
        /// 根据OpenId和顶级代理Id获取当前代理人
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public AppResponse.GetAgentResponse GetAgentByOpenId(AppRequest.GetByOpenIdRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.GetAgentResponse();
            var agent = _agentRepository.GetAgentByTopParentAgent(request.OpenId, request.TopParentAgent);
            if (agent != null)
            {
                response.agent = ToModel(agent, request.TopParentAgent);
                return response;
            }
            else
            {
                response.ErrCode = -1;//无值
                return response;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public AppResponse.GetAgentIdentityAndRateResponse GetAgent(AppRequest.GetAgentIdentityAndRateRequestAboutApp request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.GetAgentIdentityAndRateResponse();
            try
            {
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
                //var curAgent = _agentRepository.GetAgent(request.OpenId);
                var curAgent = _agentRepository.GetAgentByTopParentAgent(request.OpenId, request.ParentAgent);

                //直客身份
                if (curAgent == null)
                {
                    response.IsAgent = 0;

                }
                else
                {
                    response.IsAgent = 1;
                    #region

                    var allids = _agentRepository.GetSonId(request.ParentAgent);
                    var allsons = allids.Split(',');

                    //modify.by.gpj.0419.判断操作重复
                    //当前试算人即是经纪人自己
                    //if (curAgent.Id == request.Agent)
                    //{
                    //看看是否是在这个顶级经纪人的归属下，才能按照经纪人身份展示,否则就按照直客处理

                    var isExist = allsons.Count(x => x == curAgent.Id.ToString()) > 0;
                    if (isExist)
                    {
                        response.IsAgent = 1;
                    }
                    else
                    {
                        response.IsAgent = 0;
                    }

                    //}
                    //else
                    //{
                    //    //Request.Agent有可能是试算人的上级代理,是就按照经纪人身份处理，否则 按照直客处理
                    //    var isExist = allsons.Count(x => x == curAgent.Id.ToString()) > 0;
                    //    if (isExist)
                    //    {
                    //        response.IsAgent = 1;
                    //    }
                    //    else
                    //    {
                    //        response.IsAgent = 0;
                    //    }
                    //}
                }

                //后加逻辑
                if (request.ParentAgent == request.Agent)
                {
                    response.IsAgent = 0;
                }

                if (response.IsAgent == 0)
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            string method = string.Format("{0}/api/EnterpriseAgent/WechatZhiKeRate?agentid={1}&buid={2}", _rateCenter, request.Agent, request.Buid);
                            //string method = string.Format("{0}/api/EnterpriseAgent/WechatZhiKeRate?agentid={1}&buid={2}",_rateCenter, 102, 109154);
                            logError.Info("获取费率请求串:" + method);
                            HttpResponseMessage responserate = client.GetAsync(method).Result;
                            var rrrate = responserate.Content.ReadAsStringAsync().Result;
                            //var resultrate = JsonConvert.DeserializeObject<ReturnRate>(rrrate);
                            var resultrate = rrrate.FromJson<List<ReturnRate>>();
                            if (resultrate != null)
                            {
                                response.ZhiKeRate = new List<Rate>();
                                for (var i = 0; i < resultrate.Count; i++)
                                {
                                    Rate rate = new Rate
                                    {
                                        //20160908兼容新旧source将0123->1248
                                        //Source = resultrate[i].Source,
                                        Source = SourceGroupAlgorithm.GetNewSource(resultrate[i].Source),
                                        BizRate = resultrate[i].zhike_biz_rate
                                    };
                                    response.ZhiKeRate.Add(rate);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logError.Info("获取费率发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                        }

                    }
                }
                else
                {
                    //20160908兼容新旧source将1248->0123
                    var hebaoitem = _hebaoDianwei.GetHeBao(request.Buid, AppHelpers.SourceGroupAlgorithm.GetOldSource(request.Source));
                    if (hebaoitem != null)
                    {
                        var rate = new Rate
                        {
                            BizRate = hebaoitem.agent_biz_rate ?? 0,
                            ForceRate = hebaoitem.agent_force_rate ?? 0,
                            TaxRate = 0
                        };
                        response.AgentRate = rate;
                    }

                }

                    #endregion
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return response;
        }

        /// <summary>
        /// 获取代理人资源
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public AppResponse.GetAgentSourceResponse GetAgentSource(AppRequest.GetAgentResourceRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.GetAgentSourceResponse();

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
            var sourceList = _agentConfig.Find(int.Parse(_agentRepository.GetTopAgentId(request.Agent)));
            var list = sourceList.Select(x => (x.source ?? 0)).ToList().Distinct();

            response.ComList = list.ToList();

            return response;
        }
        /// <summary>
        /// 获取代理人资源
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public AppResponse.GetAgentSourceResponse GetAgentNewSource(AppRequest.GetAgentResourceRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.GetAgentSourceResponse();

            //根据经纪人获取手机号 
            bx_agent agentModel = GetAgent(request.Agent);
            //logInfo.Info("获取到的经纪人信息:"+agentModel.ToJson());
            //参数校验
            if (agentModel == null)
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }
            if (agentModel.IsUsed != 1)
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }

            if (!ValidateReqest(pairs, agentModel.SecretKey, request.SecCode))
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }

            //var agentsourcekey = string.Format("agent_source_key_{0}", request.Agent);
            var agentsourcekey = string.Format("agent_source_key_{0}_{1}", request.Agent, request.CityCode);
            var list = CacheProvider.Get<List<int>>(agentsourcekey);
            if (list == null)
            {
                var sourceList = _agentConfig.FindCities(int.Parse(_agentRepository.GetTopAgentId(request.Agent)), request.CityCode);
                // var sourceList = _agentConfig.FindNewCity(int.Parse(_agentRepository.GetTopAgentId(request.Agent)));
                list = sourceList.Where(x => x.is_used == 1).Select(x => (x.source ?? 0)).Distinct().ToList();
                CacheProvider.Set(agentsourcekey, list, 600);
            }
            response.ComList = new List<int>();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i] == 0)
                {
                    response.ComList.Add(2);
                }
                else if (list[i] == 1)
                {
                    response.ComList.Add(1);
                }
                else if (list[i] == 2)
                {
                    response.ComList.Add(4);
                }
                else if (list[i] == 3)
                {
                    response.ComList.Add(8);
                }
                else if (list[i] == 4)
                {
                    response.ComList.Add(16);
                }
                else if (list[i] == 5)
                {
                    response.ComList.Add(32);
                }
                else if (list[i] == 6)
                {
                    response.ComList.Add(64);
                }
                else if (list[i] == 7)
                {
                    response.ComList.Add(128);
                }
                else if (list[i] == 8)
                {
                    response.ComList.Add(256);
                }
                else if (list[i] == 9)
                {
                    response.ComList.Add(512);
                }
            }
            response.ComList = response.ComList.OrderBy(x => x).ToList();
            return response;
        }
        public AppResponse.GetAgentListResponse GetAgentList(AppRequest.GetAgentRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.GetAgentListResponse();
            try
            {
                //根据经纪人获取手机号 
                bx_agent agentModel = GetAgent(request.Agent);
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

                int totalCount = 0;
                var item = _agentRepository.GetAgentList(request.CurAgent, request.Agent, request.Search, request.AgentStatus, request.CurPage, request.PageSize, out totalCount);

                if (item.Count > 0)
                {
                    response.Status = HttpStatusCode.OK;
                    response.totalCount = totalCount;
                    response.AgentList = item;
                }
                else
                {
                    response.Status = HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                response.Status = HttpStatusCode.InternalServerError;
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return response;
        }

        public AppResponse.ApproveAgentResponse ApproveAgent(AppRequest.ApproveAgentRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.ApproveAgentResponse();
            try
            {
                //根据经纪人获取手机号 
                bx_agent topAgent = GetAgent(request.Agent);
                bx_agent curAgent = GetAgent(request.CurAgent);
                //参数校验
                if (topAgent == null || curAgent == null || curAgent.IsUsed != request.OriStatus)
                {
                    response.Status = HttpStatusCode.Forbidden;
                    return response;
                }
                if (!ValidateReqest(pairs, topAgent.SecretKey, request.SecCode))
                {
                    response.Status = HttpStatusCode.Forbidden;
                    return response;
                }
                curAgent.IsUsed = request.CurStatus;
                if (_agentRepository.UpdateModel(curAgent) > 0)
                {
                    response.Status = HttpStatusCode.OK;
                }

            }
            catch (Exception ex)
            {
                response.Status = HttpStatusCode.InternalServerError;
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return response;
        }
        public AppViewModels.GetAgentTagViewModel GetAgentTag(int agentId)
        {
            var model = new AppViewModels.GetAgentTagViewModel();
            model.Content = _tagFlagRepository.GetByAgentId(agentId).Select(a => new TagFlag() { TagId = a.Id, AgentId = a.AgentId, Content = a.Content, CreateTime = a.CreateTime, ValidFlag = a.ValidFlag }).ToList();
            return model;
        }
        public AppViewModels.GetAgentTagViewModel GetAgentTagForCustomer(string tagIds)
        {
            var model = new AppViewModels.GetAgentTagViewModel();
            long[] ids = tagIds.Split(new char[] { ',' }).Select(a => long.Parse(a)).ToArray();
            var list = _tagFlagRepository.GetById(ids).Select(a => new TagFlag() { TagId = a.Id, AgentId = a.AgentId, Content = a.Content, CreateTime = a.CreateTime, ValidFlag = a.ValidFlag }).ToList();
            model.Content = list;
            return model;
        }
        public AppViewModels.GetAgentTagViewModel AddTagFlag(AddTagRequest request)
        {
            var model = new AppViewModels.GetAgentTagViewModel();
            if (request.Tags.Contains("^"))
            {
                model.BusinessStatus = 0;
                model.StatusMessage = "标签不能包含该特殊字符^";
                return model;
            }
            else
            {
                if (request.Tags.Length > 10)
                {
                    model.BusinessStatus = 0;
                    model.StatusMessage = "标签长度不能大于10个字";
                    return model;
                }
            }
            string[] tags = request.Tags.Split(new char[] { '^' }).ToArray();
            foreach (var x in tags)
            {
                if (x.Length > 10)
                {
                    model.BusinessStatus = 0;
                    model.StatusMessage = "标签长度不能大于10个字";
                    return model;
                }
            }
            var list = _tagFlagRepository.GetByContent(tags, request.ChildAgent);
            string[] ta = list.Select(a => a.Content).ToArray();
            List<string> str = new List<string>();
            foreach (var item in tags)
            {
                if (ta.Contains(item))
                {
                    continue;
                }
                else str.Add(item);
            }
            var tagList = _tagFlagRepository.GetByAgentId(request.ChildAgent);
            if ((tagList.Count + str.Count) > 20)
            {
                model.BusinessStatus = 0;
                model.StatusMessage = "一个代理人最多可以设置20个标签";
                return model;
            }
            else
            {
                var addTagList = new List<bx_tagflag>();
                foreach (var item in str)
                {
                    addTagList.Add(new bx_tagflag() { AgentId = request.ChildAgent, Content = item, CreateTime = DateTime.Now, ValidFlag = 1 });
                }
                bool isTrue = _tagFlagRepository.AddTagFlag(addTagList);
                if (isTrue)
                {
                    model.BusinessStatus = 1;
                    model.StatusMessage = "添加成功";
                }
                else
                {
                    model.BusinessStatus = 0;
                    model.StatusMessage = "添加失败";
                    return model;
                }
            }
            model.Content = _tagFlagRepository.GetByContent(tags, request.ChildAgent).Select(a => new TagFlag() { TagId = a.Id, AgentId = a.AgentId, Content = a.Content, CreateTime = a.CreateTime, ValidFlag = a.ValidFlag }).ToList();
            return model;
        }
        public AppViewModels.BaseViewModel DelTagFlag(int Id)
        {
            var model = new AppViewModels.BaseViewModel();
            var entity = _tagFlagRepository.GetById(Id);
            if (entity != null)
            {
                entity.ValidFlag = 0;
                if (_tagFlagRepository.Update(entity) > 0)
                {
                    model.BusinessStatus = 1;
                    model.StatusMessage = "删除成功";
                    return model;
                }
                else
                {
                    model.BusinessStatus = 0;
                    model.StatusMessage = "删除失败";
                    return model;
                }
            }
            else
            {
                model.BusinessStatus = 0;
                model.StatusMessage = "未找到该标签";
                return model;
            }
        }

        #region 基础方法

        /// <summary>
        /// 根据agentId获取Agent对象
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
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
            if (agent != null)
            {
                //HttpRuntime.Cache.Insert(agentCacheKey, agent, null, DateTime.Now.AddHours(6), TimeSpan.Zero, CacheItemPriority.High, null);
                return agent;
            }
            return null;
            //}
        }


        /// <summary>
        /// bx_agent转AgentModel
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public Models.ReportModel.AgentModel ToModel(bx_agent agent, int topParentAgent)
        {
            var topAgent = _agentRepository.GetAgent(topParentAgent);
            var parentAgent = _agentRepository.GetAgent(agent.ParentAgent);
            var smsContent = _smsContentRepository.Find(agent.MessagePayType == 0 ? topParentAgent : agent.Id);
            Models.ReportModel.AgentModel agentmodel = new Models.ReportModel.AgentModel()
            {
                Id = agent.Id,
                AgentName = agent.AgentName,
                Mobile = agent.Mobile,
                OpenId = agent.OpenId,
                ShareCode = agent.ShareCode,
                CreateTime = agent.CreateTime,
                IsBigAgent = agent.IsBigAgent,
                FlagId = agent.FlagId,
                ParentAgent = agent.ParentAgent,
                ParentName = parentAgent != null ? parentAgent.AgentName : string.Empty,
                ParentMobile = parentAgent != null ? parentAgent.Mobile : string.Empty,
                ParentRate = agent.ParentRate,
                AgentRate = agent.AgentRate,
                ReviewRate = agent.ReviewRate,
                PayType = agent.PayType,
                AgentGetPay = agent.AgentGetPay,
                CommissionType = agent.CommissionType,
                ParentShareCode = agent.ParentShareCode,
                IsUsed = agent.IsUsed,
                AgentAccount = agent.AgentAccount,
                AgentPassWord = agent.AgentPassWord,
                IsGenJin = agent.IsGenJin,
                IsDaiLi = agent.IsDaiLi,
                IsShow = agent.IsShow,
                IsShowCalc = agent.IsShowCalc,
                SecretKey = agent.SecretKey,
                IsLiPei = agent.IsLiPei,
                AgentType = agent.AgentType,
                //获取代理级别
                AgentLevel = _agentRepository.GetAgentLevel(agent.ParentAgent),
                TopAgentId = topParentAgent,
                //扣短信账户是否走顶级代理
                MessagePayType = agent.MessagePayType,
                //获取顶级代理人名字
                TopAgentName = topAgent != null ? topAgent.AgentName : string.Empty,
                TopAgentMobile = topAgent != null ? topAgent.Mobile : string.Empty,
                SmsAccount = smsContent != null ? smsContent.sms_account : string.Empty,
                TotalTimes = smsContent != null ? smsContent.total_times : 0,
                AvailTimes = smsContent != null ? smsContent.avail_times : 0
            };
            return agentmodel;
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

        /// <summary>
        /// 获取渠道的source集合
        /// </summary>
        /// <param name="url"></param>
        /// <param name="agentId">顶级代理Id</param>
        /// <returns></returns>
        public List<AppViewModels.AgentCity> GetSourceList(string url, int agentId)
        {
            var viewModel = new AppViewModels.AppAgentSourceViewModel();

            var agentNewCityKey = string.Format("ExternalApi_ConfigCity_FindByAgent_{0}", agentId);

            var cityAgentList = CacheProvider.Get<List<AppViewModels.AgentCity>>(agentNewCityKey);
            if (cityAgentList == null)
            {
                cityAgentList = new List<AppViewModels.AgentCity>();
                var agentListSource = _agentConfig.FindAgentConfigByAgent(agentId);
                var listCitys = agentListSource.Select(x => x.CityId).Distinct().ToList();
                foreach (int city in listCitys)
                {
                    var agentSourceList = agentListSource.Where(i => i.CityId == city).OrderBy(o => o.Id).ToList();
                    var sourcelists = new List<AppViewModels.Source>();
                    if (agentSourceList.Any())
                    {
                        foreach (var s in agentSourceList)
                        {
                            if (sourcelists.Where(x => x.Id == s.Id).Count() == 0)
                            {
                                var agentSources = new AppViewModels.Source
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
                    var agentCitys = new AppViewModels.AgentCity
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

            #region 老逻辑代码 2018-01-29 L
            ////获取城市列表
            //var agentCityKey = string.Format("ExternalApi_{0}_ConfigCity_Find", agentId);
            //var listTempCity = CacheProvider.Get<List<bx_agent_config>>(agentCityKey);
            //var listCity = new List<int>();
            //if (listTempCity == null)
            //{
            //    listTempCity = _agentConfig.FindNewCity(agentId);
            //    CacheProvider.Set(agentCityKey, listTempCity, 10800);
            //}
            //if (listTempCity != null)
            //    listCity = listTempCity.Select(i => i.city_id.Value).Distinct().ToList();

            ////获取所有source列表
            //var agentSourceKey = string.Format("agent_source_key_{0}", agentId);
            //var listSource = CacheProvider.Get<List<bx_agent_config>>(agentSourceKey);
            //if (listSource == null)
            //{
            //    listSource = _agentConfig.Find(agentId).ToList();
            //    CacheProvider.Set(agentSourceKey, listSource, 10800);
            //}
            //var citylist = new List<AppViewModels.AgentCity>();
            //var agentCity = new AppViewModels.AgentCity();
            //var sourcelist = new List<AppViewModels.Source>();
            //var agentSource = new AppViewModels.Source();
            //var tempSourceList = new List<bx_agent_config>();
            ////根据城市获取source列表
            //if (!listCity.Any()) return citylist;
            //foreach (var c in listCity)
            //{
            //    tempSourceList = new List<bx_agent_config>();
            //    tempSourceList = listSource.Where(i => i.city_id == c && i.source.HasValue).OrderBy(o => o.source.Value).Distinct().ToList();
            //    sourcelist = new List<AppViewModels.Source>();
            //    if (tempSourceList.Any())
            //    {
            //        foreach (var s in tempSourceList)
            //        {
            //            agentSource = new AppViewModels.Source();
            //            agentSource.Id = s.source.Value;
            //            agentSource.Name = s.source.Value.ToEnumDescriptionString(typeof(AppViewModels.EnumSource));
            //            agentSource.NewId = SourceGroupAlgorithm.GetNewSource(s.source.Value);
            //            agentSource.ImageUrl = string.Format("{0}/Images/company/{1}.png", url, s.source.Value);
            //            sourcelist.Add(agentSource);
            //        }
            //    }
            //    agentCity = new AppViewModels.AgentCity();
            //    agentCity.AgentSource = sourcelist;
            //    agentCity.CityId = c;
            //    agentCity.CityName = _cityRepository.FindCity(c).city_name;
            //    citylist.Add(agentCity);
            //}
            //return citylist;
            #endregion


        }

        #endregion


    }
    public class ReturnRate
    {
        public int Source { get; set; }
        public double zhike_biz_rate { get; set; }
    }
    public class Rate
    {

        public double BizRate { get; set; }
        public double ForceRate { get; set; }
        public double TaxRate { get; set; }

        public long Source { get; set; }


    }
}
