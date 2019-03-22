using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IRenewalQuoteConfigService
    {
        Task<List<bx_quoterenewal_agent_config>> GetQuoteRenewalAgentConfigList();
        /// <summary>
        /// 获取代理人相关城市的配置集合
        /// </summary>
        /// <returns></returns>
        Task<List<bx_quoterenewal_city_config>> GetQuoteRenewalCityConfigList();
        /// <summary>
        /// 获取代理人相关配置
        /// </summary>
        /// <param name="agentId">bx_agent.Id</param>
        /// <returns></returns>
        Task<bx_quoterenewal_agent_config> GetQuoteRenewalAgentConfig(int agentId);
        /// <summary>
        /// 获取代理人相关城市配置
        /// </summary>
        /// <param name="agentId">bx_agent.Id</param>
        /// <param name="id">bx_quoterenewal_city_config.Id</param>
        /// <param name="isEdit">是否为编辑</param>
        /// <returns></returns>
        Task<bx_quoterenewal_city_config> GetQuoteRenewalCityConfig(int agentId, int id, bool isEdit);
    }
}
