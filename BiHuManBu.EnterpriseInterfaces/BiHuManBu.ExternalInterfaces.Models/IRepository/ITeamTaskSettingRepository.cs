using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ITeamTaskSettingRepository
    {
        bx_zc_team_task_setting FindModel();

        int Add(bx_zc_team_task_setting model);

        int Del();
    }
}
