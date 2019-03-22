using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IAgentWithdrawalRepository
    {
        /// <summary>
        /// 新增提现
        /// </summary>
        /// <param name="withdrawal"></param>
        /// <returns></returns>
        int AddWithdrawal(bx_agent_withdrawal withdrawal);

        /// <summary>
        /// 更新佣金积分关联
        /// </summary>
        /// <param name="withdrawalId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        int UpdateListCommissionIdStatus(int withdrawalId, string ids);
        /// <summary>
        /// 查询佣金积分余额
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<dd_order_commission> GetListCommissions(BaseRequest2 request);
        /// <summary>
        /// 获取提现列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="listAgentIds"></param>
        /// <returns></returns>
        List<WithdrawalViewModel> GetPageListWithdrawal(PageListWithdrawalRequest request, List<string> listAgentIds);
        /// <summary>
        /// 修改提现状态（0：审核不通过，1：审核通过，2：已出账）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        int UpdateWithdrawalAuditStatus(int id, int status);
        /// <summary>
        /// 获取佣金详情列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<MoneyWithdrawalDetailViewModel> GetMoneyWithdrawalDetial(int id);
        /// <summary>
        /// 获取团队收益详情列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<TeamWithdrawalDetailViewModel> GetTeamMoneyWithdrawalDetial(int id);
    }
}
