using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class EntryProportionAnalysisVM
    {
        public List<EntryProportionAnalysisVM_PropertyClass> EntryProportionDayAnalysisVM { get; set; }

        public List<EntryProportionAnalysisVM_PropertyClass> EntryProportionMonthAnalysisVM { get; set; }
        public List<EntryProportionAnalysisVM_PropertyClass> EntryProportionYearAnalysisVM { get; set; }
    }
    public class EntryProportionAnalysisVM_PropertyClass
    {

        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 人保进场数
        /// </summary>
        public int RenBaoCameraCount { get; set; }
        /// <summary>
        /// 平安进场数
        /// </summary>
        public int PingAnCameraCount { get; set; }
        /// <summary>
        /// 太平洋进场数
        /// </summary>
        public int TaiPingYangCameraCount { get; set; }
        /// <summary>
        /// 国寿财进场数
        /// </summary>
        public int GuoShouCaiCameraCount { get; set; }
        /// <summary>
        /// 其他保司进场数
        /// </summary>
        public int OtherSourceCameraCount { get; set; }
    }
}
