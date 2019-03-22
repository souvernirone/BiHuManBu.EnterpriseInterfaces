using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends
{
    public interface ICheckEntityService
    {
        CheckOrderView CheckGetOrder(GetOrderDetailRequest request);
    }
}
