using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces
{
    public interface ITeamIncomeService
    {
        /// <summary>
        /// 获取当前代理人下级和下下级的总人数和总收益 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        IList<SonAndGrandSonIncome> GetSonAndGrandsonIncome(int agentId);

        /// <summary>
        /// 获取团队级别、总净保费、预计收益 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        TeamIncomeModel GetTeamLevelMoneyExpectedIncome(int agentId);
    }
}
