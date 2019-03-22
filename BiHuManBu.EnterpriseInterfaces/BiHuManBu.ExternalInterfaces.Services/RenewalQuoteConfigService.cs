using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class RenewalQuoteConfigService : IRenewalQuoteConfigService
    {
        private readonly IRenewalQuoteConfigRepository _renewalQuoteConfigRepository;
        public RenewalQuoteConfigService(IRenewalQuoteConfigRepository renewalQuoteConfigRepository)
        {
            this._renewalQuoteConfigRepository = renewalQuoteConfigRepository;
        }
        public async Task<List<bx_quoterenewal_city_config>> GetQuoteRenewalCityConfigList()
        {
            return await _renewalQuoteConfigRepository.GetQuoteRenewalCityConfigList();
        }

        public async Task<bx_quoterenewal_agent_config> GetQuoteRenewalAgentConfig(int agentId)
        {
            return await _renewalQuoteConfigRepository.GetQuoteRenewalAgentConfig(agentId);
        }

        public async Task<bx_quoterenewal_city_config> GetQuoteRenewalCityConfig(int agentId, int id, bool isEdit)
        {
            return await _renewalQuoteConfigRepository.GetQuoteRenewalCityConfig(agentId, id, isEdit);
        }

        public async Task<List<bx_quoterenewal_agent_config>> GetQuoteRenewalAgentConfigList()
        {
            return await _renewalQuoteConfigRepository.GetQuoteRenewalAgentConfigList();
        }
    }
}
