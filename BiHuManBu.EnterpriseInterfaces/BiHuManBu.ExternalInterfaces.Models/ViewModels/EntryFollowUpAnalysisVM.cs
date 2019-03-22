using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class EntryFollowUpAnalysisVM
    {
        public List<EntryFollowUpAnalysisVM_PropertyClass> EntryFollowUpDayAnalysisVM { get; set; }
        public List<EntryFollowUpAnalysisVM_PropertyClass> EntryFollowUpMonthAnalysisVM { get; set; }
        public List<EntryFollowUpAnalysisVM_PropertyClass> EntryFollowUpYearAnalysisVM { get; set; }
    }
    public class EntryFollowUpAnalysisVM_PropertyClass
    {

        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 进场数量
        /// </summary>
        public int CameraCount { get; set; }
        /// <summary>
        /// 当前报价数量
        /// </summary>
        public int NowQuoteCount { get; set; }
        /// <summary>
        /// 当前投保数量
        /// </summary>
        public int NowInsureCount { get; set; }
        /// <summary>
        ///非当前投保数量
        /// </summary>
        public int NotNowInsureCount { get; set; }
        /// <summary>
        /// 当前报价率
        /// </summary>
        public int NowQuoteRate { get; set; }

        /// <summary>
        /// 当前投保率
        /// </summary>
        public int NowInsureRate { get; set; }
    }
}
