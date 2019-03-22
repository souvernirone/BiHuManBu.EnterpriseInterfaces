using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class InsureDistributionAnalysisTempVM
    {
        public InsureDistributionAnalysisTempVM_PropertyClass InsureDistributionLastNewCarAnalysisTempVM { get; set; }
        public InsureDistributionAnalysisTempVM_PropertyClass InsureDistributionNotLastNewCarAnalysisTempVM { get; set; }

    }
    public class InsureDistributionAnalysisTempVM_PropertyClass
    {
        public List<InsureDistributionAnalysisTempVM_PropertyClass_PropertyClass> InsureDistributionDayAnalysisTempVM_PropertyClass { get; set; }
        public List<InsureDistributionAnalysisTempVM_PropertyClass_PropertyClass> InsureDistributionMonthAnalysisTempVM_PropertyClass { get; set; }
        public List<InsureDistributionAnalysisTempVM_PropertyClass_PropertyClass> InsureDistributionYearAnalysisTempVM_PropertyClass { get; set; }
    }
    public class InsureDistributionAnalysisTempVM_PropertyClass_PropertyClass
    {
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 人保报价次数
        /// </summary>
        public int RenBaoTimes { get; set; }
        /// <summary>
        /// 平安报价次数
        /// </summary>
        public int PingAnTimes { get; set; }

        /// <summary>
        /// 太平洋报价次数
        /// </summary>
        public int TaiPingYangTimes { get; set; }

        /// <summary>
        /// 国寿财报价次数
        /// </summary>
        public int GuoShouCaiTimes { get; set; }
        /// <summary>
        /// 其他保司报价次数
        /// </summary>
        public int OtherSourceTimes { get; set; }
        /// <summary>
        /// 人保环比报价次数
        /// </summary>
        public int RenBaoAgoTimes { get; set; }

        /// <summary>
        /// 平安环比报价次数
        /// </summary>
        public int PingAnAgoTimes { get; set; }
        /// <summary>
        /// 太平洋环比报价次数
        /// </summary>
        public int TaiPingYangAgoTimes { get; set; }

        /// <summary>
        /// 国寿财环比报价次数
        /// </summary>
        public int GuoShouCaiAgoTimes { get; set; }


        /// <summary>
        ///其他保司环比报价次数
        /// </summary>
        public int OtherSourceAgoTimes { get; set; }
    }
}
