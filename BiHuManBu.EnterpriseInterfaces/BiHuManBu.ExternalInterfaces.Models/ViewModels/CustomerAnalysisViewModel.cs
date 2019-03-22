using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CustomerAnalysisViewModel
    {
        /// <summary>
        /// 客户总量
        /// </summary>
        public int CustomerCount { get; set; }

        /// <summary>
        /// 已分配数量
        /// </summary>
        public int DistributedCount { get; set; }

        /// <summary>
        /// 未分配数量
        /// </summary>
        public int UndistributeCount { get; set; }

        /// <summary>
        /// 已报价数量
        /// </summary>
        public int QuoteCount { get; set; }

        /// <summary>
        /// 未报价数量
        /// </summary>
        public int NotQuoteCount { get; set; }

        /// <summary>
        /// 已跟进数量
        /// </summary>
        public int ReviewCount { get; set; }

        /// <summary>
        /// 未跟进数量
        /// </summary>
        public int NotReviewCount { get; set; }

        /// <summary>
        /// 已出单数量
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// 已战败数量
        /// </summary>
        public int FailedCount { get; set; }

        /// <summary>
        /// 代理人ID
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 代理人名称
        /// </summary>
        public string AgentName { get; set; }

        public int ParentAgentId { get; set; }

        public int TopAgentId { get; set; }
    }
}
