using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;

namespace BiHuManBu.ExternalInterfaces.Services.MapperService.Interfaces
{
    public interface ITeamIncomingSettingMapperService
    {
        List<TeamIncomingSetting> ConvertToViewModel(List<bx_zc_team_incoming_setting> list);
    }
}
