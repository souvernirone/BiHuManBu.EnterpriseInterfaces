using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces
{
    public interface ISaveTeamTaskSettingService
    {
        BaseViewModel SaveTeamTaskSetting(int agentCount, int orderCount);
    }
}
