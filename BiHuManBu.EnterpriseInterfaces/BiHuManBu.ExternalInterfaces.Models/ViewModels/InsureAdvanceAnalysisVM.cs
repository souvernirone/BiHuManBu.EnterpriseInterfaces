using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class InsureAdvanceAnalysisVM
    {
        public List<InsureAdvanceAnalysisVM_PropertyClass> InsureAdvanceDayAnalysisVM { get; set; }
        public List<InsureAdvanceAnalysisVM_PropertyClass> InsureAdvanceMonthAnalysisVM { get; set; }

        public List<InsureAdvanceAnalysisVM_PropertyClass> InsureAdvanceYearAnalysisVM { get; set; }

    }
    public class InsureAdvanceAnalysisVM_PropertyClass
    {
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 人保提前0-30天数量
        /// </summary>
        public int RenBaoAdvance_0to30 { get; set; }
        /// <summary>
        /// 人保提前31-60天数量
        /// </summary>
        public int RenBaoAdvance_31to60 { get; set; }

        /// <summary>
        /// 人保提前61-90天数量
        /// </summary>
        public int RenBaoAdvance_61to90 { get; set; }

        /// <summary>
        /// 平安提前0-30天数量
        /// </summary>
        public int PingAnAdvance_0to30 { get; set; }
        /// <summary>
        /// 平安提前31-60天数量
        /// </summary>
        public int PingAnAdvance_31to60 { get; set; }

        /// <summary>
        /// 平安提前61-90天数量
        /// </summary>
        public int PingAnAdvance_61to90 { get; set; }


        /// <summary>
        /// 太平洋提前0-30天数量
        /// </summary>
        public int TaiPingYangAdvance_0to30 { get; set; }
        /// <summary>
        ///  太平洋提前31-60天数量
        /// </summary>
        public int TaiPingYangAdvance_31to60 { get; set; }

        /// <summary>
        ///  太平洋提前61-90天数量
        /// </summary>
        public int TaiPingYangAdvance_61to90 { get; set; }

        /// <summary>
        /// 国寿财提前0-30天数量
        /// </summary>
        public int GouShouCaiAdvance_0to30 { get; set; }
        /// <summary>
        ///  国寿财提前31-60天数量
        /// </summary>
        public int GouShouCaiAdvance_31to60 { get; set; }

        /// <summary>
        ///  国寿财提前61-90天数量
        /// </summary>
        public int GouShouCaiAdvance_61to90 { get; set; }

        /// <summary>
        /// 其他保司提前0-30天数量
        /// </summary>
        public int OtherSourceAdvance_0to30 { get; set; }
        /// <summary>
        ///  其他保司提前31-60天数量
        /// </summary>
        public int OtherSourceAdvance_31to60 { get; set; }

        /// <summary>
        ///  其他保司提前61-90天数量
        /// </summary>
        public int OtherSourceAdvance_61to90 { get; set; }
    }
}
