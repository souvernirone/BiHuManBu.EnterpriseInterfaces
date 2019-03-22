using System;
namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    /// <summary>
    /// 战败分析
    /// </summary>
    public class DefeatAnalysis
    {
        public int? Id { get; set; }
        public int? DefeatReasonId { get; set; }

        private string defeatReason;
        public string DefeatReason
        {
            get { return defeatReason; }
            set { defeatReason = value; }
        }
        public int? AgentId { get; set; }
        public string AgentName { get; set; }

        /// <summary>
        /// 不去重
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// 去重
        /// </summary>
        public DateTime ? CreateTime { get; set; }

        public DateTime DataInTime { get; set; }
    }
}
