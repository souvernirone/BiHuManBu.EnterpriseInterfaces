using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IWorkOrderService
    {
        Messages.Response.AppResponse.AddOrUpdateWorkOrderResponse AddOrUpdateWorkOrder(Messages.Request.AppRequest.AddOrUpdateWorkOrderRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs);

        Messages.Response.AppResponse.AddOrUpdateWorkOrderDetailResponse AddOrUpdateWorkOrderDetail(Messages.Request.AppRequest.AddOrUpdateWorkOrderDetailRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs);
        Messages.Response.AppResponse.WorkOrderDetailListResponse WorkOrderDetailList(Messages.Request.AppRequest.WorkOrderDetailListRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs);

        Task<GetReInfoNewViewModel> GetReInfo(Messages.Request.AppRequest.GetReInfoRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        Messages.Response.AppResponse.ChangeReInfoAgentResponse ChangeReInfoAgent(Messages.Request.AppRequest.ChangeReInfoAgentRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs);

        Models.ViewModels.AppViewModels.ReVisitedListViewModel WorkOrderList(long buid);

        /// <summary>
        /// 添加回访记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Models.ViewModels.AppViewModels.BaseViewModel AddReVisited(Messages.Request.AppRequest.AddReVisitedRequest request);
    }
}
