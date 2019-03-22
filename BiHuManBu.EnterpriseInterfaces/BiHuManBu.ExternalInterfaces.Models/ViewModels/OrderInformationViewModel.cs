using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class OrderInformationViewModel
    {
        public int BusinessStatus { get; set; }
        public string StatusMessage { get; set; }
        public dd_order_quoteresult OrderQuoteresult { get; set; }
        public dd_order_savequote OrderSavequote { get; set; }
        public bx_submit_info SubmitInfo { get; set; }
        public dd_order_related_info OrderRelatedInfo { get; set; }
        public List<dd_order_paymentresult> ListoOrderPaymentresults { get; set; }
        public bx_quotereq_carinfo QuotereqCarinfo { get; set; }
        public List<dd_order_steps> ListOrderStepses { get; set; }
        public bx_agent_config AgentConfig { get; set; }
        public List<bx_agent> ListAgent { get; set; }
        public List<dd_order_commission> ListCommissions { get; set; }
    }
}
