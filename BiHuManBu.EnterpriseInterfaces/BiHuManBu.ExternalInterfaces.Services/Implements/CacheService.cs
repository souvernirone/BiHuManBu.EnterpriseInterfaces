using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.CacheRequest;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheService: ICacheService
    {
        private ILog logError = LogManager.GetLogger("ERROR");

        /// <summary>
        /// ukey更新后清除缓存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel ClearUKeyCache(ClearUKeyCacheRequest request)
        {
            var arrAgent= request.AgentIds.Split(',');

            if (arrAgent.Length < 1)
                return BaseViewModel.GetBaseViewModel(Models.BusinessStatusType.OK) ;
            var strAgent = string.Join(",", arrAgent);
            try
            {
                logError.Info(string.Format("经纪人agent：{0}清空了资源配置缓存", strAgent));
                foreach (var item in arrAgent)
                {
                    var key = string.Format("agent_source_key_{0}_{1}", item, request.CityId);
                    CacheProvider.Remove(key);
                    var keyAllCity = string.Format("agent_source_key_{0}", item);//App和Crm用到了此缓存，获取某代理下所有城市的渠道
                    CacheProvider.Remove(keyAllCity);
                    var ExternalApi_Key = string.Format("ExternalApi_{0}_ConfigCity_Find", item);
                    CacheProvider.Remove(ExternalApi_Key);
                    var keyAgentCity = string.Format("agent_config_{0}_{1}_list", item, request.CityId);
                    CacheProvider.Remove(keyAgentCity);
                    var keyGroupbyList = string.Format("multiChannels_{0}_{1}_list", item, request.CityId);
                    CacheProvider.Remove(keyGroupbyList);
                    var keyByAgent = string.Format("ExternalApi_ConfigCity_FindByAgent_{0}", item);
                    CacheProvider.Remove(keyByAgent);
                }
                return BaseViewModel.GetBaseViewModel(Models.BusinessStatusType.OK);
            }
            catch (Exception ex)
            {
                logError.Error(string.Format("经纪人agent：{0},城市{1}清空了资源配置缓存，发生异常，异常信息：{2}", strAgent, request.CityId, ex.ToString()));
                return BaseViewModel.GetBaseViewModel(Models.BusinessStatusType.OperateError);
            }
        }
    }
}
