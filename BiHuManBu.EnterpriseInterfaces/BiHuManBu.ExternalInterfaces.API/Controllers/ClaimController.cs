using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System.Collections.Generic;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class ClaimController : ApiController
    {
        private readonly IClaimService _claimService;
        private readonly IAppVerifyService _appVerifyService;
        public ClaimController(IClaimService claimService, IAppVerifyService appVerifyService)
        {
            _claimService = claimService;
            _appVerifyService = appVerifyService;
        }

        /// <summary>
        /// 获取pc端的订单列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public PcOrderPageResponse GetPcOrderPage([FromBody]PcOrderPageRequest request)
        {
            if (request.PageIndex == 0) request.PageIndex = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            return _claimService.GetPcOrderPage(request);
        }

        /// <summary>
        /// 获取pc端订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        public PcOrderDetail GetPcOrderDetail(int orderId)
        {
            return _claimService.GetPcOrderDetail(orderId);
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<PcClaimOrderModel> ExportOrder(int agentId)
        {
            return _claimService.ExportOrder(agentId);
        }

        /// <summary>
        /// 获取业务员
        /// </summary>
        /// <param name="agentId">当前代理人id</param>
        /// <param name="salesmanName">业务员姓名</param>
        /// <returns></returns>
        [HttpGet]
        public List<Salesman> GetSalesman(int agentId, string salesmanName)
        {
            return _claimService.GetSalesman(agentId, salesmanName);
        }

        /// <summary>
        /// 获取台账列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public PcSetTlementPage GetPcSetTlementPage(PcSetTlementPageRequest request)
        {
            return _claimService.GetPcSetTlementPage(request);
        }

        /// <summary>
        /// 获取台账导出数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<SetTlement> GetExportSetTlementList(int agentId)
        {
            if (agentId < 1) return new List<SetTlement>();
            return _claimService.GetExportSetTlementList(agentId);
        }

        /// <summary>
        /// 车商台账列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public PcCarDealerSetTlementPage GetCarDealerSetTlementPage(PcCarDealerRequest request)
        {
            return _claimService.GetCarDealerSetTlementPage(request);
        }
    }
}

