using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends
{
    public class CheckOrderView
    {
        public bx_agent Agent { get; set; }
        public int OrderId { get; set; }
        public string OrderNum { get; set; }
        public int BusinessStatus { get; set; }
        public string StatusMessage { get; set; }
        public DateTime OrderLapsetime { get; set; }
        public int SourceValue { get; set; }
        public bx_userinfo Userinfo { get; set; }
        public bx_submit_info SubmitInfo { get; set; }
        public bx_agent_config AgentConfig { get; set; }
        public bx_savequote Savequote { get; set; }
        public bx_quoteresult Quoteresult { get; set; }
        public dd_order Order { get; set; }
        public int TopAgent { get; set; }
    }
}
