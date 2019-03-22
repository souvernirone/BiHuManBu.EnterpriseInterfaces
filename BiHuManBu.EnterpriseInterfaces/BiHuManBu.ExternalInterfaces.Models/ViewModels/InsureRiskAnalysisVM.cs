using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class InsureRiskAnalysisVM
    {
        public List<InsureRiskAnalysisVM_PropertyClass> InsureRiskDayAnalysisVM { get; set; }
        public List<InsureRiskAnalysisVM_PropertyClass> InsureRiskMonthAnalysisVM { get; set; }
        public List<InsureRiskAnalysisVM_PropertyClass> InsureRiskYearAnalysisVM { get; set; }


    }
    public class InsureRiskAnalysisVM_PropertyClass
    {
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 人保：交强+车损+三者+司乘+盗抢险种组合
        /// </summary>
        public int RenBaoInsuranceGroup1 { get; set; }
        /// <summary>
        /// 人保：交强+车损+三者险种组合
        /// </summary>
        public int RenBaoInsuranceGroup2 { get; set; }
        /// <summary>
        /// 人保：交强+三者险种组合
        /// </summary>
        public int RenBaoInsuranceGroup3 { get; set; }
        /// <summary>
        /// 人保：单交强
        /// </summary>
        public int RenBaoInsuranceDanJiaoQiang { get; set; }
        /// <summary>
        /// 人保：其他险种组合
        /// </summary>
        public int RenBaoOtherInsuranceGroup { get; set; }

        /// <summary>
        /// 平安：交强+车损+三者+司乘+盗抢险种组合
        /// </summary>
        public int PingAnInsuranceGroup1 { get; set; }
        /// <summary>
        /// 平安：交强+车损+三者险种组合
        /// </summary>
        public int PingAnInsuranceGroup2 { get; set; }
        /// <summary>
        /// 平安：交强+三者险种组合
        /// </summary>
        public int PingAnInsuranceGroup3 { get; set; }
        /// <summary>
        /// 平安：单交强
        /// </summary>
        public int PingAnInsuranceDanJiaoQiang { get; set; }
        /// <summary>
        /// 平安：其他险种组合
        /// </summary>
        public int PingAnOtherInsuranceGroup { get; set; }

        /// <summary>
        /// 太平洋：交强+车损+三者+司乘+盗抢险种组合
        /// </summary>
        public int TaiPingYangInsuranceGroup1 { get; set; }
        /// <summary>
        /// 太平洋：交强+车损+三者险种组合
        /// </summary>
        public int TaiPingYangInsuranceGroup2 { get; set; }
        /// <summary>
        /// 太平洋：交强+三者险种组合
        /// </summary>
        public int TaiPingYangInsuranceGroup3 { get; set; }
        /// <summary>
        /// 太平洋：单交强
        /// </summary>
        public int TaiPingYangInsuranceDanJiaoQiang { get; set; }
        /// <summary>
        /// 太平洋：其他险种组合
        /// </summary>
        public int TaiPingYangOtherInsuranceGroup { get; set; }

        /// <summary>
        /// 国寿财：交强+车损+三者+司乘+盗抢险种组合
        /// </summary>
        public int GuoShouCaiInsuranceGroup1 { get; set; }
        /// <summary>
        /// 国寿财：交强+车损+三者险种组合
        /// </summary>
        public int GuoShouCaiInsuranceGroup2 { get; set; }
        /// <summary>
        /// 国寿财：交强+三者险种组合
        /// </summary>
        public int GuoShouCaiInsuranceGroup3 { get; set; }
        /// <summary>
        /// 国寿财：单交强
        /// </summary>
        public int GuoShouCaiInsuranceDanJiaoQiang { get; set; }
        /// <summary>
        /// 国寿财：其他险种组合
        /// </summary>
        public int GuoShouCaiOtherInsuranceGroup { get; set; }


        /// <summary>
        /// 其他保司：交强+车损+三者+司乘+盗抢险种组合
        /// </summary>
        public int OtherSourceInsuranceGroup1 { get; set; }
        /// <summary>
        /// 其他保司：交强+车损+三者险种组合
        /// </summary>
        public int OtherSourceInsuranceGroup2 { get; set; }
        /// <summary>
        /// 其他保司：交强+三者险种组合
        /// </summary>
        public int OtherSourceInsuranceGroup3 { get; set; }
        /// <summary>
        /// 其他保司：单交强
        /// </summary>
        public int OtherSourceInsuranceDanJiaoQiang { get; set; }
        /// <summary>
        /// 其他保司：其他险种组合
        /// </summary>
        public int OtherSourceOtherInsuranceGroup { get; set; }
    }

}
