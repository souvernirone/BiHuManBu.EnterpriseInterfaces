using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class QuoteTimesAnalysisTempVM
    {
        public List<QuoteTimesAnalysisTempVM_PropertyClass> QuoteTimesDayAnalysisTempVM { get; set; }
        public List<QuoteTimesAnalysisTempVM_PropertyClass> QuoteTimesMonthAnalysisTempVM { get; set; }
        public List<QuoteTimesAnalysisTempVM_PropertyClass> QuoteTimesYearAnalysisTempVM { get; set; }

    }
    public class QuoteTimesAnalysisTempVM_PropertyClass
    {
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 人保报价次数
        /// </summary>
        public int RenBaoQuoteTimes { get; set; }
        /// <summary>
        /// 平安报价次数
        /// </summary>
        public int PingAnQuoteTimes { get; set; }

        /// <summary>
        /// 太平洋报价次数
        /// </summary>
        public int TaiPingYangQuoteTimes { get; set; }

        /// <summary>
        /// 国寿财报价次数
        /// </summary>
        public int GuoShouCaiQuoteTimes { get; set; }
        /// <summary>
        /// 其他保司报价次数
        /// </summary>
        public int OtherSourceQuoteTimes { get; set; }
        /// <summary>
        /// 人保环比报价次数
        /// </summary>
        public int RenBaoAgoQuoteTimes { get; set; }

        /// <summary>
        /// 平安环比报价次数
        /// </summary>
        public int PingAnAgoQuoteTimes { get; set; }
        /// <summary>
        /// 太平洋环比报价次数
        /// </summary>
        public int TaiPingYangAgoQuoteTimes { get; set; }

        /// <summary>
        /// 国寿财环比报价次数
        /// </summary>
        public int GuoShouCaiAgoQuoteTimes { get; set; }


        /// <summary>
        ///其他保司环比报价次数
        /// </summary>
        public int OtherSourceAgoQuoteTimes { get; set; }
    }
}
