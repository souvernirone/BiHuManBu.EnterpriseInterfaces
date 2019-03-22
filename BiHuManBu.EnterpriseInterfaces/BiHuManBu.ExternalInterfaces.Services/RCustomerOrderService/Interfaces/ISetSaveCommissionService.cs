using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces
{
    public interface ISetSaveCommissionService
    {
        /// <summary>
        /// 更具参数保存佣金积分信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel SaveCommission(SaveCommissionRequest request);
        /// <summary>
        /// 根据订单号和状态保存佣金和积分信息
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        BaseViewModel Save(string orderNum, int status);

        /// <summary>
        /// 团队收益保存
        /// </summary>
        /// <param name="listOrder"></param>
        /// <param name="agentId"></param>
        /// <param name="ratio">收益比例</param>
        /// <param name="grade">团队等级</param>
        BaseViewModel TeamSave(List<OrderAgentAmountViewModel> listOrder, int agentId, double ratio, int grade);

        /// <summary>
        /// 获取已保存的团队收益数据  用来避免服务异常的情况  重新刷的时候重新添加
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        List<OrderCommissionViewModel> GetTeamCommission(string orderIds);
    }
}
