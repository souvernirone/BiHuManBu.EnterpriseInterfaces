using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class AgentSpecialRateService : IAgentSpecialRateService
    {
        private IAgentSpecialRateRepository _agentSpecialRateRepository;

        public AgentSpecialRateService(IAgentSpecialRateRepository agentSpecialRateRepository)
        {
            _agentSpecialRateRepository = agentSpecialRateRepository;
        }

        public IList<BxAgentSpecialRate> GetAgentSpecialRate(int agentId, int companyId, int isQudao, int qudaoId)
        {
            return _agentSpecialRateRepository.GetAgentSpecialRate(agentId, companyId, isQudao, qudaoId);
        }

        public BxAgentRate GetAgentRate(int agentId, int companyId, int isQudao, int qudaoId)
        {
            return _agentSpecialRateRepository.GetAgentRate(agentId, companyId, isQudao, qudaoId);
        }
    }
}
