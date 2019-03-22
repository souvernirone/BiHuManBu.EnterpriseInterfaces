using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class ClaimService: IClaimService
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        private ILog logInfo = LogManager.GetLogger("INFO");
        private readonly IClaimRepository _claimRepository;
       public ClaimService(IClaimRepository claimRepository)
        {
            _claimRepository = claimRepository;
        }

        /// <summary>
        /// 获取pc端订单列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PcOrderPageResponse GetPcOrderPage(PcOrderPageRequest request)
        {
            var result = new PcOrderPageResponse { Data = new PageModel() };
            var totalCount = 0;
            var data = _claimRepository.GetPcClaimOrderPage(request.AgentId, out totalCount, request.OrderState, request.LicenseNo, request.Moblie, request.Salesman, request.OrderNo, request.StartCreateTime, request.EndCreateTime, request.SalesmanMoblie, request.PageIndex, request.PageSize);
            result.Data.TotalCount = totalCount;
            result.Data.OrderList = data;
            result.Data.PageIndex = request.PageIndex;
            result.Data.PageSize = request.PageSize;
            result.Code = 1;
            result.Message = "获取成功";

            return result;
        }

        /// <summary>
        /// 获取pc端订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public PcOrderDetail GetPcOrderDetail(int orderId)
        {
            return _claimRepository.GetPcOrderDetail(orderId);
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<PcClaimOrderModel> ExportOrder(int agentId)
        {
            if (agentId < 1) return new List<PcClaimOrderModel>();
            return _claimRepository.ExportOrder(agentId);
        }

        /// <summary>
        /// 获取业务员
        /// </summary>
        /// <param name="agentId">当前代理人id</param>
        /// <param name="salesmanName">业务员姓名</param>
        /// <returns></returns>
        public List<Salesman> GetSalesman(int agentId, string salesmanName)
        {
            if (agentId < 1 || string.IsNullOrWhiteSpace(salesmanName))
                return new List<Salesman>();
            return _claimRepository.GetSalesman(agentId,salesmanName);
        }

        /// <summary>
        /// 获取台账列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PcSetTlementPage GetPcSetTlementPage(PcSetTlementPageRequest request)
        {
            if (request.PageIndex == 0) request.PageIndex = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            var result = new PcSetTlementPage();
            result.PageIndex = request.PageIndex;
            result.PageSize = request.PageSize;
            var totalCount = 0;
            result.PageList = _claimRepository.GetSetTlementPage(request.AgentId, out totalCount, request.PageIndex, request.PageSize, request.OrderNo, request.LicenseNo, request.Salesman, request.TopSalesman, request.SettledState, request.StartSettledTime, request.EndSettledTime, request.Mobile, request.StartCreateTime, request.EndCreateTime,request.ToSettlementId);
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 获取台账导出数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<SetTlement> GetExportSetTlementList(int agentId)
        {
            return _claimRepository.GetExportSetTlementList(agentId);
        }

        /// <summary>
        /// 车商台账列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PcCarDealerSetTlementPage GetCarDealerSetTlementPage(PcCarDealerRequest request)
        {
            if (request.PageIndex == 0) request.PageIndex = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            var result = new PcCarDealerSetTlementPage();
            result.PageIndex = request.PageIndex;
            result.PageSize = request.PageSize;
            var totalCount = 0;
            result.PageList = _claimRepository.GetCarDealerSetTlementPage(request.TopAgentId, out totalCount, request.PageIndex, request.PageSize, request.OrderNo, request.LicenseNo, request.State, request.StartTime, request.EndTime, request.StartOrderTime, request.EndOrderTime,request.FromSettlementId);
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 结算单列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SettlementSheetResponse GetSettlementPage(PCSettlementSheetRequest request)
        {
            if (request.PageIndex == 0) request.PageIndex = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            var result = new SettlementSheetResponse();
            result.PageIndex = request.PageIndex;
            result.PageSize = request.PageSize;
            var totalCount = 0;
            result.PageList = _claimRepository.GetSettlementsPage(request.AgentId,out totalCount,request.PageIndex,request.PageSize,request.SettledState,request.SettledTime);
            result.TotalCount = totalCount;
            return result;
        }

    }
}
