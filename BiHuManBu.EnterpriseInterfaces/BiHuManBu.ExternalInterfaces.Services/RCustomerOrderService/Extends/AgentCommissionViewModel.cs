using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends
{
    public class AgentCommissionViewModel
    {
        /// <summary>
        /// 商业费率
        /// </summary>
        public double BizRate { get; set; }
        /// <summary>
        /// 交强费率
        /// </summary>
        public double ForceRate { get; set; }
        /// <summary>
        /// 商业险或交强险费率超过此值的部分，自动转化为积分
        /// </summary>
        public double OverTransferCredits { get; set; }
    }
}
