using System.Collections.Generic;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppResponse = BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse;
using System;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces
{
    public interface IOrderService
    {
        Task<AppResponse.CreateOrderResponse> NewCreateOrder(AppRequest.CreateOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        List<Models.ViewModels.AppViewModels.CarOrderModel> GetOrders(AppRequest.GetOrdersRequest request, int status, out int TotalCount);
        List<Models.ViewModels.AppViewModels.CarOrderModel> GetOrdersForApp(AppRequest.GetOrdersRequest request, int status, out int totalCount);

        AppResponse.UpdateOrderResponse UpdateOrder(AppRequest.ModifyOrderRequest request);

        Models.ViewModels.AppViewModels.CarOrderModel FindCarOrderBy(AppRequest.GetOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 根据orderId获取订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bx_car_order FindOrderByOrderId(long orderId, string orderNum);

        Task<CreateSureOrderNewResponse> CreateSureOrderNew(Models.ViewModels.Request.CreateSureOrderNewRequest request);


    }
}
