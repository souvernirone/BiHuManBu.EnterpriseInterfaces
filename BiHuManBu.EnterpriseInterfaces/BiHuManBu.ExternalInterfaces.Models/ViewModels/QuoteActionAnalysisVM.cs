using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class QuoteActionAnalysisVM
    {
        public List<QuoteActionAnalysisVM_PropertyClass> QuoteActionDayAnalysisVM { get; set; }
        public List<QuoteActionAnalysisVM_PropertyClass> QuoteActionMonthAnalysisVM { get; set; }
        public List<QuoteActionAnalysisVM_PropertyClass> QuoteActionYearAnalysisVM { get; set; }

    }
    public class QuoteActionAnalysisVM_PropertyClass
    {
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }

        /// <summary>
        /// 9-11时段报价数
        /// </summary>
        public int NineToElevenQuoteTimes { get; set; }
        /// <summary>
        /// 11-13时段报价数
        /// </summary>
        public int ElevenToThirteenQuoteTimes { get; set; }
        /// <summary>
        /// 13-17时段报价数
        /// </summary>
        public int ThirteenToSeventeenQuoteTimes { get; set; }
        /// <summary>
        /// 其他时段报价数
        /// </summary>
        public int OtherHourQuoteTimes { get; set; }
    }
}
