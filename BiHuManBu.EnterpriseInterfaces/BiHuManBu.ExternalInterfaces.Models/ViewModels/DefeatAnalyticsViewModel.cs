using System;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 战败分析
    /// </summary>
    public class DefeatAnalyticsViewModel : BaseViewModel
    {

        public DefeatAnalyticsViewModel()
        {
            defeatReasons = new Dictionary<string, int>();
        }

        public int? AgentId { get; set; }
        public string AgentName { get; set; }
        public int? Count { get; set; }
        public DateTime? DataInTime { get; set; }

        public string StrCount { get; set; }
        public string StrDataInTime { get; set; }

        private Dictionary<string, int> defeatReasons;
        public Dictionary<string, int> DefeatReasons
        {
            get
            {
                return defeatReasons;
            }
            set
            {
                defeatReasons = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DefeatReasonList { get; set; }
        public int? TotalCount { get; set; }

        public string AgentAnalyticJson { get; set; }
    }

    public class DefeatAnalytics
    {
        public int AgentId { get; set; }

        public int ParentAgentId { get; set; }

        public int TopAgentId { get; set; }

        public int DefeatReasonId { get; set; }

        public string DefeatReasonContent { get; set; }

        public int DefeatCount { get; set; }
    }
}
