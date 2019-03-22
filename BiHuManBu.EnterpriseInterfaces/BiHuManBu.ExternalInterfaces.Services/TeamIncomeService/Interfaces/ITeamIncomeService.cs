using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Extends;

namespace BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Interfaces
{
    public interface ITeamIncomeService
    {
        /// <summary>
        /// 获取团队收益（时间区间）：团队二级、三级人数、团队时间内等级、团队预计收益
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        TeamIncomeViewModel GetTeamIncome(GetTeamIncomeRequest request);

        /// <summary>
        /// 防止服务异常，备用数据重新计算用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel SetTeamIncomeByDay(GetTeamIncomeByDayRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<AgentSonPremium> GetTeamAgentSonPremium(GetTeamIncomeRequest request);
    }
}
