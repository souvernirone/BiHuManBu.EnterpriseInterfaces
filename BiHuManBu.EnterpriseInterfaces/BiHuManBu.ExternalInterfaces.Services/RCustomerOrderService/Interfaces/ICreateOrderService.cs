using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces
{
    public interface ICreateOrderService
    {
        CreateOrderDetailViewModel CreateOrderDetail(CreateOrderDetailRequest request);
    }
}
