using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IAgentSpecialRateService
    {
        IList<BxAgentSpecialRate> GetAgentSpecialRate(int agentId, int companyId, int isQudao, int qudaoId);

        BxAgentRate GetAgentRate(int agentId, int companyId, int isQudao, int qudaoId);
    }
}
