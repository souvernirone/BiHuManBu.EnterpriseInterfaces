using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IOrderCommissionService
    {
        /// <summary>
        /// 分页获取资金列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel GetMoneyList(GetMoneyListRequest request);

        /// <summary>
        /// 账单明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel GetBillInfo(BillInfoRequest request);

        /// <summary>
        /// 根据顶级代理人获取佣金统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> GetStatisticsByTopAgentAsync(BaseRequest2 request);

        /// <summary>
        /// 根据当前代理人获取佣金统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> GetStatisticsByCurAgentAsync(BaseRequest2 request);
    }
}
