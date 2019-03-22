using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class ClueStatisticalViewModel
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 未处理总数
        /// </summary>
        public int UnhandleCount { get; set; }
        /// <summary>
        ///  流失总数
        /// </summary>
        public int LossCount { get; set; }
        /// <summary>
        /// 跟进中总数
        /// </summary>
        public int FollowUpCount { get; set; }
        /// <summary>
        /// 车辆到店总数
        /// </summary>
        public int ReachDealersCount { get; set; }
        /// <summary>
        /// 已定损数量
        /// </summary>
        public int MaintainCount { get; set; }

        /// <summary>
        /// 已交车数量
        /// </summary>
        public int HandOverCount { get; set; }
        /// <summary>
        /// 饼状图信息
        /// </summary>
        public List<ClueStatisticalWithCompany> ClueStatisticalWithCompanies { get; set; }
        /// <summary>
        ///  响应度分析
        /// </summary>
        public List<ClueResponsivity> ClueResponsivities { get; set; }
        /// <summary>
        /// 流失原因分析
        /// </summary>
        public List<LossStatistical> LossStatisticals { get; set; }

        public ClueOrderStatistical OrderCount { get; set; }
    }

    public class ClueOrderStatistical
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 抢单成功数量
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 抢单失败数量
        /// </summary>
        public int FailCount { get; set; }
    }

    public class ClueStatisticalWithCompany
    {
        /// <summary>
        /// 保险公司id
        /// </summary>
        public int CompanyId { get; set; }
        private string _companyName;
        /// <summary>
        /// 保险公司名称
        /// </summary>
        public string CompanyName
        {
            get
            {
                var sName = "";
                switch (CompanyId)
                {
                    case 0:
                        sName = "平安";
                        break;
                    case 1:
                        sName = "太平洋";
                        break;
                    case 2:
                        sName = "人保";
                        break;
                    case 3:
                        sName = "人寿";
                        break;
                    default:
                        sName = "其它";
                        break;
                }
                return sName;
            }
            set { _companyName = value; }
        }
        public List<ClueStatisticalWithState> clueStatisticalWithStates { get; set; }        
    }

    public class ClueStatisticalWithState
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 到店数
        /// </summary>
        public int ReachDealersCount { get; set; }
        /// <summary>
        /// 1.送修 2.返修
        /// </summary>
        public int CaseType { get; set; }
        public string CaseName { get; set; }
        public double SuccessRate { get { return Math.Round(Convert.ToDouble(ReachDealersCount) / Convert.ToDouble(TotalCount), 2); } }
    }

    public class ClueResponsivity
    {
        /// <summary>
        /// 业务员id
        /// </summary>
        public int AgentId { get; set; }
        private string _agentName;
        /// <summary>
        /// 业务员名称
        /// </summary>
        public string AgentName { get { return _agentName != null ? _agentName : ""; } set { _agentName = value; } }
        /// <summary>
        /// 总台次
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 1-3分钟   
        /// </summary>
        public int OneToThreeMinutesCount { get; set; }
        /// <summary>
        ///  3-5分钟
        /// </summary>
        public int ThreeToFiveMinutesCount { get; set; }
        /// <summary>
        ///  5分钟以上 
        /// </summary>
        public int OverFiveMinutesCount { get; set; }
    }

    public class LossStatistical
    {
        private string _lossOfReason;
        /// <summary>
        /// 流失原因
        /// </summary>
        public string LossOfReason { get { return _lossOfReason != null ? _lossOfReason : ""; } set { _lossOfReason = value; } }
        /// <summary>
        /// 台次数
        /// </summary>
        public int LossCount { get; set; }
    }
}
