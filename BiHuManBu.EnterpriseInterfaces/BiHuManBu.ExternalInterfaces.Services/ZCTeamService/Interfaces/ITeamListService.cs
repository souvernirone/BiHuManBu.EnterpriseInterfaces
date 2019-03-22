using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.ZCTeam;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces
{
    public interface ITeamListService
    {
        TeamListViewModel GetTeamList(GetTeamManagerRequest request);
        TeamChildLevelListViewModel GetTeamChildLevelList(GetTeamChildLevelListRequest request);
        NextLevelAgentListViewModel GetNextLevelAgentList(GetNextLevelAgentListRequest request);
    }
}
