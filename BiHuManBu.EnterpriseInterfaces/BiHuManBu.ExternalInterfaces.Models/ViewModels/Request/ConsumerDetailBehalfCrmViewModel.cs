using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class ConsumerDetailBehalfCrmViewModel
    {
        public int AgentId { get; set; }
        public long BUid { get; set; }
        /// <summary>
        /// 代报价人Id
        /// </summary>
        //public int BehalfAgent { get; set; }
        public int ReQuoteAgent { get; set; }
        /// <summary>
        /// 代报价人Name
        /// </summary>
        //public string BehalfName { get; set; }
        public string ReQuoteName { get; set; }
        /// <summary>
        /// 报价成功source组合
        /// </summary>
        public int QuoteGroup { get; set; }
    }
}
