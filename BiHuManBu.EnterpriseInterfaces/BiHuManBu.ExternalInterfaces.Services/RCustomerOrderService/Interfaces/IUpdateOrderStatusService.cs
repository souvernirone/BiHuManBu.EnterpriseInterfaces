using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces
{
    public interface IUpdateOrderStatusService
    {
        /// <summary>
        /// 更新订单状态以及相关显示
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel UpdateOrderStatus(GetOrderDetailRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel UpdateOrderSubmitStatusAndPno(UpdateOrderStatusAndPnoRequest request);
    }
}
