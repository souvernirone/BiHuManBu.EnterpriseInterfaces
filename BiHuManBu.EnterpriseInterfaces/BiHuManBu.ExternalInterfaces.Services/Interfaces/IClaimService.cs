using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IClaimService
    {
        /// <summary>
        /// 获取pc端订单列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PcOrderPageResponse GetPcOrderPage(PcOrderPageRequest request);

        /// <summary>
        /// 获取pc端订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        PcOrderDetail GetPcOrderDetail(int orderId);

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
        /// 获取台账列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
       PcSetTlementPage GetPcSetTlementPage(PcSetTlementPageRequest request);

        /// <summary>
        /// 获取台账导出数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
       List<SetTlement> GetExportSetTlementList(int agentId);

        /// <summary>
        /// 车商台账列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
       PcCarDealerSetTlementPage GetCarDealerSetTlementPage(PcCarDealerRequest request);

        /// <summary>
        /// 结算单列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
       SettlementSheetResponse GetSettlementPage(PCSettlementSheetRequest request);
    }
}
