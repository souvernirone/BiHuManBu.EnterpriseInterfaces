using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
   public class GetCountsViewModel:BaseViewModel
    {
        /// <summary>
        /// 预约单数量
        /// </summary>
        public int AppoinmentOrdersCount { get; set; }
        /// <summary>
        /// 已出单数量
        /// </summary>

        public int QuotationReceiptOrdersCount { get; set; }
        /// <summary>
        /// 报价单数量
        /// </summary>
        public int QuoteOrdersCount { get; set; }
    }
}
