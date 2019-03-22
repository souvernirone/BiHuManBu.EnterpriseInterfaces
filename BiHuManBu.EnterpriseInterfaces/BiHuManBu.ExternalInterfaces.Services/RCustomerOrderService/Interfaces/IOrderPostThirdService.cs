using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces
{
    public interface IOrderPostThirdService
    {
        void SendPost(int agent, string secretKey, int urlType, dd_order_quoteresult orderQuoteresult, dd_order_savequote orderSavequote, dd_order order,
            dd_order_related_info orderRelatedInfo, List<dd_order_paymentresult> listoOrderPaymentresults, bx_userinfo userinfo);
    }
}
