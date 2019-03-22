using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.ZCTeam
{
    public class SaveTeamIncomingSettingRequest:BaseRequest
    {
        public List<TeamIncomingSetting> TeamIncomingSetting { get; set; }
    }
}
