using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IClaimRepository
    {
        /// <summary>
        /// pc 获取订单列表
        /// </summary>
        /// <param name="agentId">当前代理人id</param>
        /// <param name="totalCount">总数</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="licenseNo">车牌</param>
        /// <param name="mobile">车主手机号</param>
        /// <param name="salesman">业务员名称或id</param>
        /// <param name="orderNo">订单编号</param>
        /// <param name="times">开始提交时间</param>
        /// <param name="endCreateTime">结束提交时间</param>
        /// <param name="salesmanMoblie">业务员手机号</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<PcClaimOrderModel> GetPcClaimOrderPage(int agentId, out int totalCount, int orderState, string licenseNo = "", string mobile = "", string salesman = "", string orderNo = "", string startCreateTime = "", string endCreateTime = "", string salesmanMoblie = "", int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 获取pc端订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        PcOrderDetail GetPcOrderDetail(int orderId);

       tx_agent GetAgent(int topAgentId);

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
       List<PcClaimOrderModel> ExportOrder(int agentId);

        /// <summary>
        /// 获取业务员
        /// </summary>
        /// <param name="agentId">当前代理人id</param>
        /// <param name="salesmanName">业务员姓名</param>
        /// <returns></returns>
       List<Salesman> GetSalesman(int agentId, string salesmanName);

        /// <summary>
        /// 获取代理商台账列表
        /// </summary>
        /// <param name="agentId">代理人id</param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderNo">订单编号</param>
        /// <param name="licenseNo">车牌号</param>
        /// <param name="salesman">业务员名称或id</param>
        /// <param name="topSalesman">上级业务员名称或id</param>
        /// <param name="settledState">结算状态</param>
        /// <param name="startSettledTime">结算开始时间</param>
        /// <param name="endSettledTime">结算结束时间</param>
        /// <param name="moblie">业务员手机号</param>
        /// <param name="startCreateTime">台账开始生成时间</param>
        /// <param name="endCreateTime">台账结束生成时间</param>
        /// <param name="toSettlementId">结算单id</param>
        /// <returns></returns>
        List<SetTlement> GetSetTlementPage(int agentId, out int totalCount, int pageIndex = 1, int pageSize = 10, string orderNo = "", string licenseNo = "", string salesman = "", string topSalesman = "", int settledState = 0, string startSettledTime = "", string endSettledTime = "", string moblie = "", string startCreateTime = "", string endCreateTime = "", int toSettlementId = 0);

        /// <summary>
        /// 获取台账导出数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
       List<SetTlement> GetExportSetTlementList(int agentId);

        /// <summary>
        /// 车商台账列表
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderNo">订单编号</param>
        /// <param name="licenseNo">车牌号</param>
        /// <param name="state">结算状态</param>
        /// <param name="startTime">开始结算时间</param>
        /// <param name="endTime">结束结算时间</param>
        /// <param name="startCreateTime">开始结算单生成时间</param>
        /// <param name="endCreateTime">结束结算单生成时间</param>
        /// <param name="fromSettlementId">车商结算单id</param>
        /// <returns></returns>
        List<CarDealerSetTlement> GetCarDealerSetTlementPage(int topAgentId, out int totalCount, int pageIndex = 1, int pageSize = 10, string orderNo = "", string licenseNo = "", int state = 0, string startTime = "", string endTime = "", string startCreateTime = "", string endCreateTime = "", int fromSettlementId = 0);

        /// <summary>
        /// 结算单列表
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="state"></param>
        /// <param name="settledTime"></param>
        /// <returns></returns>
        List<SetTlementPage> GetSettlementsPage(int agentId, out int totalCount, int pageIndex = 1, int pageSize = 10, int state = 0, string settledTime = "");
    }
}
