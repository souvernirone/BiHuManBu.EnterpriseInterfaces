using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using System.Net.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper;
using log4net;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using System.Configuration;



namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class RenewalQuoteConfigController : ApiController
    {
        private readonly CacheClient _cacheClient;
        private readonly ILog _logInfo;
        private readonly ILog _logError;
        private readonly IRenewalQuoteConfigService _renewalQuoteConfigService;
        private readonly string aboutAgentConfigKey;
        private readonly string aboutAgentCityConfigKey;
        
        public RenewalQuoteConfigController(IRenewalQuoteConfigService renewalQuoteConfigService)
        {
            this._cacheClient = new CacheClient(new RedisHashCache(Convert.ToInt32(ConfigurationManager.AppSettings["dbNum"])));
            this._logInfo = LogManager.GetLogger("INFO");
            this._logError = LogManager.GetLogger("ERROR");
            this._renewalQuoteConfigService = renewalQuoteConfigService;
            this.aboutAgentConfigKey =ConfigurationManager.AppSettings["aboutAgentConfigKey"];
            this.aboutAgentCityConfigKey = ConfigurationManager.AppSettings["aboutAgentCityConfigKey"];
        }
        /// <summary>
        /// 将有关代理人总的配置添加到redis
        /// </summary>
        /// <param name="agentId">bx_agent.Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> AddAgentRenewalQuoteDataToRedis(int agentId)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "添加成功" };
            try
            {

                if (_cacheClient.KeyExists(aboutAgentConfigKey))
                {
                    var agentRenewalQuote = await _renewalQuoteConfigService.GetQuoteRenewalAgentConfig(agentId);
                    _cacheClient.Set(aboutAgentConfigKey, agentId.ToString(), agentRenewalQuote);

                }
                else
                {
                    var agentRenewalQuoteList = await _renewalQuoteConfigService.GetQuoteRenewalAgentConfigList();
                    foreach (var item in agentRenewalQuoteList)
                    {
                        _cacheClient.Set(aboutAgentConfigKey, item.AgentId.ToString(), item);
                    }
                }
                _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), agentId));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }

        /// <summary>
        /// 将代理人相关城市的配置添加到redis
        /// </summary>
        /// <param name="agentId">bx_agent.Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> AddAgentCityRenewalQuoteDataToRedis(int agentId)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "添加成功" };
            try
            {

                if (_cacheClient.KeyExists(aboutAgentCityConfigKey))
                {
                    var agentCityRenewalQuote = await _renewalQuoteConfigService.GetQuoteRenewalCityConfig(agentId, -1, false);
                    _cacheClient.Set(aboutAgentCityConfigKey, agentCityRenewalQuote.QuoteRenewalAgentId + "_" + agentCityRenewalQuote.CityId, agentCityRenewalQuote);
                }
                else
                {
                    await SetAgentCityTotalRenewalQuoteDataToRedis();
                }
                _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), agentId));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }

        /// <summary>
        /// 更新代理人总的配置
        /// </summary>
        /// <param name="agentId">bx_agent.Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> UpdateAgentRenewalQuoteDataInRedis(int agentId)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "更新成功" };
            try
            {

                if (_cacheClient.KeyExists(aboutAgentConfigKey))
                {
                    var agentRenewalQuote = await _renewalQuoteConfigService.GetQuoteRenewalAgentConfig(agentId);
                    if (_cacheClient.Remove(aboutAgentConfigKey, agentId.ToString()))
                    {
                        _cacheClient.Set(aboutAgentConfigKey, agentId.ToString(), agentRenewalQuote);
                    }
                }
                else
                {
                    await SetAgentTotalRenewalQuoteDataToRedis();
                }
                _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), agentId));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }

        /// <summary>
        /// 更新代理人相关的城市配置
        /// </summary>
        /// <param name="id">bx_quoterenewal_city_config.Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> UpdateAgentCityRenewalQuoteDataInRedis(int id)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "更新成功" };
            try
            {

                if (_cacheClient.KeyExists(aboutAgentCityConfigKey))
                {
                    var agentCityRenewalQuote = await _renewalQuoteConfigService.GetQuoteRenewalCityConfig(-1, id, true);
                    if (DeleteFromRedis(agentCityRenewalQuote.QuoteRenewalAgentId + "_" + agentCityRenewalQuote.CityId))
                    {
                        _cacheClient.Set(aboutAgentCityConfigKey, agentCityRenewalQuote.QuoteRenewalAgentId + "_" + agentCityRenewalQuote.CityId, agentCityRenewalQuote);
                    }
                }
                else
                {
                    await SetAgentCityTotalRenewalQuoteDataToRedis();
                }
                _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), id));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }

        /// <summary>
        /// 删除代理人相关城市的配置
        /// </summary>
        /// <param name="id">bx_quoterenewal_city_config.Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> DeleteAgentCityRenewalQuoteDataFromRedis(int id)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "删除成功" };
            try
            {

                if (_cacheClient.KeyExists(aboutAgentCityConfigKey))
                {
                    var agentCityRenewalQuote = await _renewalQuoteConfigService.GetQuoteRenewalCityConfig(-1, id, true);
                    DeleteFromRedis(agentCityRenewalQuote.QuoteRenewalAgentId + "_" + agentCityRenewalQuote.CityId);
                }
                else
                {
                    await SetAgentCityTotalRenewalQuoteDataToRedis();
                }
                _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), id));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
            }
            return baseViewModel.ResponseToJson();
        }
        private bool DeleteFromRedis(string key)
        {
            return _cacheClient.Remove(aboutAgentCityConfigKey, key);
        }
        private async Task<bool> SetAgentTotalRenewalQuoteDataToRedis()
        {
            var agentRenewalQuoteList = await _renewalQuoteConfigService.GetQuoteRenewalAgentConfigList();
            foreach (var item in agentRenewalQuoteList)
            {
                _cacheClient.Set(aboutAgentConfigKey, item.AgentId.ToString(), item);
            }
            return true;
        }
        private async Task<bool> SetAgentCityTotalRenewalQuoteDataToRedis()
        {
            var agentCityRenewalQuoteList = await _renewalQuoteConfigService.GetQuoteRenewalCityConfigList();
            foreach (var item in agentCityRenewalQuoteList)
            {
                _cacheClient.Set(aboutAgentCityConfigKey, item.QuoteRenewalAgentId + "_" + item.CityId, item);
            }
            return true;
        }
    }
}
