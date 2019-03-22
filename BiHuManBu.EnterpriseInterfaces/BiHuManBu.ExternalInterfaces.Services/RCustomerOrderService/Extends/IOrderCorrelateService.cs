using System;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends
{
    public interface IOrderCorrelateService
    {
        void CreateOrderSuccessCorrelate(CreateOrderDetailRequest request, dd_order_related_info orderRelatedInfo,
            bx_userinfo userinfo, bx_quoteresult quoteresult, long orderId, dd_order order, bx_submit_info submitInfo, int sourceValue);
    }
}
