using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class InsureDistributionAnalysisVM
    {
        public InsureDistributionAnalysisVM_PropertyClass InsureDistributionLastNewCarAnalysisVM_PropertyClass { get; set; }

        public InsureDistributionAnalysisVM_PropertyClass InsureDistributionNotLastNewCarAnalysisVM_PropertyClass { get; set; }

    }
    public class InsureDistributionAnalysisVM_PropertyClass
    {

       
        public List<InsureDistributionAnalysisVM_PropertyClass_PropertyClass> InsureDistributionDayAnalysisVM_PropertyClass { get; set; }
        public List<InsureDistributionAnalysisVM_PropertyClass_PropertyClass> InsureDistributionMonthAnalysisVM_PropertyClass { get; set; }
        public List<InsureDistributionAnalysisVM_PropertyClass_PropertyClass> InsureDistributionYearAnalysisVM_PropertyClass { get; set; }

    }
    public class InsureDistributionAnalysisVM_PropertyClass_PropertyClass
    {
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 人保当前数据和之前环比
        /// </summary>
        public object RenBaoTimesGroup { get; set; }
        /// <summary>
        /// 平安当前数据和之前环比
        /// </summary>
        public object PingAnTimesGroup { get; set; }
        /// <summary>
        /// 太平洋当前数据和之前环比
        /// </summary>
        public object TaiPingYangTimesGroup { get; set; }
        /// <summary>
        /// 国寿财当前数据和之前环比
        /// </summary>
        public object GuoShouCaiTimesGroup { get; set; }
        /// <summary>
        /// 其他保司当前数据和之前环比
        /// </summary>
        public object OtherSourceTimesGroup { get; set; }
    }
}
