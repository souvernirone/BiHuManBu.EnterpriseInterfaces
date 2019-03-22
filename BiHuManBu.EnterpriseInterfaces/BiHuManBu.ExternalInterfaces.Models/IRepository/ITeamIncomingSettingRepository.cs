using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ITeamIncomingSettingRepository
    {
        List<bx_zc_team_incoming_setting> FindList();
        int Del();
        int Add(string sql);
    }
}
