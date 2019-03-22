using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class EntryPeriodAnalysisVM
    {
        public List<EntryPeriodAnalysisVM_PropertyClass> EntryPeriodDayAnalysisVM { get; set; }
        public List<EntryPeriodAnalysisVM_PropertyClass> EntryPeriodMonthAnalysisVM { get; set; }
        public List<EntryPeriodAnalysisVM_PropertyClass> EntryPeriodYearAnalysisVM { get; set; }

    }
    public class EntryPeriodAnalysisVM_PropertyClass
    {
     
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 9-11时段进场数
        /// </summary>
        public int NineToElevenCameraCount { get; set; }
        /// <summary>
        /// 11-13时段进场数
        /// </summary>
        public int ElevenToThirteenCameraCount { get; set; }
        /// <summary>
        /// 13-17时段进场数
        /// </summary>
        public int ThirteenToSeventeenCameraCount { get; set; }
        /// <summary>
        /// 其他时段进场数
        /// </summary>
        public int OtherHourCameraCount { get; set; }
    }
}
