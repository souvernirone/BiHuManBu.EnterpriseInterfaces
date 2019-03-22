using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces
{
    public interface IUpdateGroupLevelService
    {
        /// <summary>
        /// 更新团队等级
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool UpdateGroupLevel(List<TeamLevelViewModel> list);
    }
}
