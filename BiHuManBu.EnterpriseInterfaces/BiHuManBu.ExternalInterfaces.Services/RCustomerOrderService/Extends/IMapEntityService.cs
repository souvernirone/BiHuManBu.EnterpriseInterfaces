using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends
{
    public interface IMapEntityService
    {
        dd_order CreateMapOrder(CreateOrderDetailRequest request, dd_order order, bx_submit_info submitInfo,
            bx_userinfo userinfo, DateTime orderLapsetime, int sourceValue, bx_agent agent);

        dd_order_related_info CreateMapOrderRelatedInfo(CreateOrderDetailRequest request, bx_userinfo userinfo);


        OrderDetail GetMapOrder(GetOrderDetailRequest request, dd_order order, bx_userinfo userinfo,
            PayWayBanksModel bank, bx_agent_config agentConfig, dd_order_quoteresult quoteresult,
            dd_order_savequote savequote, List<dd_order_steps> listSteps, List<dd_order_commission> listCommission,
            List<bx_agent> listAgents, List<dd_order_paymentresult> listoOrderPaymentresults);
        

        PrecisePrice GetMapPrecisePrice(dd_order_quoteresult quoteresult, dd_order_savequote savequote, dd_order order,
            bx_submit_info submitinfo);

        RelatedInfo GetMapRelatedInfo(dd_order_related_info relatedInfo);

        PaymentResult GetMapPaymentResult(List<dd_order_paymentresult> paymentresult);

        OrderCarInfo GetPayOrderCarInfo(bx_quotereq_carinfo carinfo, dd_order order, bx_userinfo userinfo,dd_order_quoteresult quoteresult);

        ChangeStr GetChangeStr(CreateOrderDetailRequest request);

        /// <summary>
        /// 生成订单号
        /// </summary>
        /// <param name="fountain"></param>
        /// <returns></returns>
        string GenerateOrderNum(int fountain);
    }
}
