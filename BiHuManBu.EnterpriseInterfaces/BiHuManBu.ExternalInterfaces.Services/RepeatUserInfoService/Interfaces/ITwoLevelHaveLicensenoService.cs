using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces
{
    public interface ITwoLevelHaveLicensenoService
    {
        AgentNameViewModel GetLevel2LicenseNo(int topAgentId, int agentId, List<AgentNameViewModel> list);
    }
}
