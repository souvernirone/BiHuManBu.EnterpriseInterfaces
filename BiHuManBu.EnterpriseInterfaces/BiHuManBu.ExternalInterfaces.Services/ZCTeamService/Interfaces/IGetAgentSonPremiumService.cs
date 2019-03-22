using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces
{
    public interface IGetAgentSonPremiumService
    {
        /// <summary>
        /// 获取下级代理人单人净保费列表
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        BaseViewModel GetAgentSonPremium(int agentId,string createTime);
    }
}
