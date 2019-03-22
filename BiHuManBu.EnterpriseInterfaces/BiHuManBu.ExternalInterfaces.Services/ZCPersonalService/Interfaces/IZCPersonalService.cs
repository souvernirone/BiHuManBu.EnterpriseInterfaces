using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;

namespace BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces
{
    public interface IZCPersonalService
    {
        IList<BillInfo> GetTotalIncomeList(int agentId, int yearTime, int monthTime);
        /// <summary>
        /// 查询每个月收益总金额 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        IList<MonthAndTotalMoney> GetMonthAndTotalMoney(int agentId);
    }
}