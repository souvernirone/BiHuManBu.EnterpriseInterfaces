using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class CreateOrderReturnModel
    {
        public dd_order ddorder { get; set; }
        public dd_order_related_info ddorderrelatedinfo { get; set; }
        public dd_order_quoteresult ddorderquoteresult { get; set; }
        public dd_order_savequote ddordersavequote { get; set; }
    }
}
