using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order
{
    public class UpdateOrderDetailRequest : BaseRequest2
    {
        public string OrderNum { get; set; }
    }
}
