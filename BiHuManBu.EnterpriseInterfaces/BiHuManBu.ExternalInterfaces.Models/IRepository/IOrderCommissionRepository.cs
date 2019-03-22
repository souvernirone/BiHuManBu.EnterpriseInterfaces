using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IOrderCommissionRepository : IRepositoryBase<dd_order_commission>
    {
        /// <summary>
        /// 根据顶级代理人获取下级代理人的佣金
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="mobile"></param>
        /// <param name="agentName"></param>
        /// <returns></returns>
        List<MoneyListInfo> GetCommissionList(int topAgentId, int pageIndex, int pageSize, string mobile, string agentName);

        /// <summary>
        /// 根据顶级代理人获取下级代理人的佣金总条数
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="mobile"></param>
        /// <param name="agentName"></param>
        /// <returns></returns>
        int GetCommissionListCount(int topAgentId, string mobile, string agentName);

        /// <summary>
        /// 根据代理人和时间段获取数量
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        int GetCount(OrderCommissionSearchParam search);

        /// <summary>
        /// 获取代理人的佣金
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        List<OrderCommissionBillInfo> GetBillInfoByAgentId(int pageIndex, int pageSize,OrderCommissionSearchParam search);

        /// <summary>
        /// 根据顶级代理人获取合计
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        Task<TopAgentStatistics> GetStatisticsByTopAgentAsync(int topAgentId);

        /// <summary>
        /// 根据当前代理人获取合计
        /// </summary>
        /// <param name="curAgent"></param>
        /// <param name="monthStart"></param>
        /// <param name="monthEnd"></param>
        /// <returns></returns>
        Task<CurAgentStatistics> GetStatisticsByCurAgentAsync(int curAgent, DateTime monthStart, DateTime monthEnd);

        /// <summary>
        /// 收益记录列表查询 2018-02-03 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="yearTime"></param>
        /// <param name="monthTime"></param>
        /// <returns></returns>
        IList<BillInfo> GetTotalIncomeList(int agentId, int yearTime, int monthTime);

        /// <summary>
        /// 获取团队收益订单集合
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        List<OrderCommissionViewModel> GetTeamCommission(string orderIds);

        /// <summary>
        /// 保存团队收益设置
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int SaveTeamCommission(string sql);

        /// <summary>
        /// 查询每个月收益总金额 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        IList<MonthAndTotalMoney> GetMonthAndTotalMoney(int agentId);

        /// <summary>
        /// 获取当前代理人下级和下下级的总人数和总收益 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        IList<SonAndGrandSonIncome> GetSonAndGrandsonCountAndTotalMoney(int agentId);

        /// <summary>
        /// 根据代理人获取当前有效的的佣金和收益记录
        /// </summary>
        /// <param name="childAgent"></param>
        /// <returns></returns>
        List<dd_order_commission> GetMyAmountList(int childAgent);
    }
}
