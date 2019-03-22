using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using AppIRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    /// <summary>
    /// bx_city逻辑
    /// </summary>
    public class CityService :CommonBehaviorService, ICityService
    {
        private readonly ICityRepository _cityRepository;
        private AppIRepository.IAgentRepository _agentRepository;
        private ICacheHelper _cacheHelper;
        private ILoginService _loginService;
        private ILog logError = LogManager.GetLogger("ERROR");
        private readonly IAgentConfigRepository _agentConfigRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityRepository"></param>
        /// <param name="agentRepository"></param>
        public CityService(ICityRepository cityRepository, AppIRepository.IAgentRepository agentRepository, ICacheHelper cacheHelper, ILoginService loginService, IAgentConfigRepository agentConfigRepository)
            : base(agentRepository, cacheHelper)
        {
            _cityRepository = cityRepository;
            _agentRepository = agentRepository;
            _cacheHelper = cacheHelper;
            _loginService = loginService;
            _agentConfigRepository = agentConfigRepository;
        }

        /// <summary>
        /// 获取可用渠道的城市
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetCanUseUkeyCityViewModel GetCanUseUkeyCity(GetCanUseUkeyCityRequest request)
        {
            var listCity = _cityRepository.GetCanUseUkeyCity(request.Agent, request.IsUsed);
            if (listCity.Count == 0)
                return GetCanUseUkeyCityViewModel.GetModel(BusinessStatusType.NoData);
            var result = GetCanUseUkeyCityViewModel.GetModel(BusinessStatusType.OK);
            result.ListCity = listCity;
            return result;
        }

        public async Task<GetCityListResponse> GetCityList(BaseRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new GetCityListResponse();
            //根据经纪人获取手机号 
            //IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            ////logInfo.Info("获取到的经纪人信息:"+agentModel.ToJson());
            ////参数校验
            //if (!agentModel.AgentCanUse())
            //{
            //    response.Status = HttpStatusCode.Forbidden;
            //    return response;
            //}

            //if (!ValidateReqest(pairs, agentModel.SecretKey, request.SecCode))
            //{
            //    response.Status = HttpStatusCode.Forbidden;
            //    return response;
            //}
            try
            {
                var cities = FindList();
                var configedCities = new List<bx_city>();
                var configCities = FindConfigCityList(int.Parse(_agentRepository.GetTopAgentId(request.Agent)));
                var configedCityIds = configCities.Select(x => x.city_id).Distinct();
                if (configedCityIds.Any())
                {
                    foreach (var i in configedCityIds)
                    {
                        var config = (int)i;
                        var tmpcity = cities.FirstOrDefault(x => x.id == config);
                        if (tmpcity != null)
                        {
                            configedCities.Add(tmpcity);
                        }
                    }
                    response.Cities = configedCities;
                }

                response.Status = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {

                response.Status = HttpStatusCode.ExpectationFailed;
                logError.Info("续保请求发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return response;
        }

        public List<bx_city> FindList()
        {
            var key = "ExternalApi_City_Find";
            var cachelst = CacheProvider.Get<List<bx_city>>(key);
            if (cachelst == null)
            {
                var lst = _cityRepository.FindAllCity();
                CacheProvider.Set(key, lst, 86400);
                return lst;
            }
            return cachelst;
        }

        public List<bx_agent_config> FindConfigCityList(int agent)
        {
            var key = string.Format("ExternalApi_{0}_ConfigCity_Find", agent);
            var cachelst = CacheProvider.Get<List<bx_agent_config>>(key);
            if (cachelst == null)
            {
                var lst = _agentConfigRepository.FindNewCity(agent);
                CacheProvider.Set(key, lst, 10800);
                return lst;
            }
            return cachelst;

        }
    }
}
