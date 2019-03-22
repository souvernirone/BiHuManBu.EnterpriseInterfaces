using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class InsureBizAvgAnalysisVM
    {

        public List<InsureBizAvgAnalysisVM_PropertyClass> InsureBizAvgDayAnalysisVM { get; set; }
        public List<InsureBizAvgAnalysisVM_PropertyClass> InsureBizAvgMonthAnalysisVM { get; set; }
        public List<InsureBizAvgAnalysisVM_PropertyClass> InsureBizAvgYearAnalysisVM { get; set; }
    }
    public class InsureBizAvgAnalysisVM_PropertyClass {
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 人保商业险均单
        /// </summary>
        public int RenBaoBizAvg { get; set; }
        /// <summary>
        /// 平安商业险均单
        /// </summary>
        public int PingAnBizAvg { get; set; }

        /// <summary>
        /// 太平洋商业险均单
        /// </summary>
        public int TaiPingYangBizAvg { get; set; }
        /// <summary>
        /// 国寿财商业险均单
        /// </summary>
        public int GuoShouCaiBizAvg { get; set; }


        /// <summary>
        /// 其他保司商业险均单
        /// </summary>
        public int OtherSourceBizAvg { get; set; }
    }

}
