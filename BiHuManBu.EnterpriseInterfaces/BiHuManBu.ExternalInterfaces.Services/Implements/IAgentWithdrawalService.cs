using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public interface IAgentWithdrawalService
    {
        /// <summary>
        /// 新增提现记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel AddWithdrawalList(ListWithdrawalRequest request);

        /// <summary>
        /// 新增提现记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="agentId"></param>
        /// <param name="remark"></param>
        /// <param name="bankId"></param>
        /// <returns></returns>
        AddWithdrawalViewModel AddWithdrawal(AddWithdrawalRequest request, int agentId, string remark, int bankId);

        /// <summary>
        /// 获取个人佣金，和团队收益金额
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ListCommissionsWithdrawalViewModel GetListCommissions(BaseRequest2 request);
        /// <summary>
        /// 获取提现列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ListWithdrawalViewModel GetPageListWithdrawal(PageListWithdrawalRequest request);
        /// <summary>
        /// 设置提箱状态（审核状态：默认-1，通过1，未通过0，已支付2）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel UpdateWithdrawalAuditStatus(WithdrawalAuditRequset request);
        /// <summary>
        /// 获取佣金提现详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ListMoneyWithdrawalDetailViewModel GetMoneyWithdrawalDetial(WithdrawalAuditRequset request);
        /// <summary>
        /// 获取佣金提现详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ListCreditWithdrawalDetailViewModel GetTeamMoneyWithdrawalDetial(WithdrawalAuditRequset request);
    }
}
