using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class EntryOverViewModel
    {
        /// <summary>
        /// 进店数量
        /// </summary>
        public int EntryCount { get; set; }

        /// <summary>
        /// 续保期内数量
        /// </summary>
        public int RenewalPeriodCount { get; set; }

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
        /// 数据时间
        /// </summary>
        public string DataInTime { get; set; }
    }

    public class GroupEntryViewModel : EntryOverViewModel
    {
        public int AgentId { get; set; }

        public string AgentName { get; set; }
    }
}
