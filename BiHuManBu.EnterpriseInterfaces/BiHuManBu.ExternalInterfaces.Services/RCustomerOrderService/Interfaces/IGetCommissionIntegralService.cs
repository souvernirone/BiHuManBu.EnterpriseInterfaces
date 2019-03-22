using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces
{
    public interface IGetCommissionIntegralService
    {
        /// <summary>
        /// 计算获取佣金和积分
        /// 2018-01-08 L
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CommissionIntegralViewModel GetCommissionIntegral(CommissionIntegralRequest request);
    }
}
