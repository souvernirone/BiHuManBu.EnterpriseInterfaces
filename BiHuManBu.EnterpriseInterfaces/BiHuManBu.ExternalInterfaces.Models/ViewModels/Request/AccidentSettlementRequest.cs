using System;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AccidentSettlementRequest
    {
        /// <summary>
        /// 当前用户id
        /// </summary>
        [Range(1, 2100000000)]
        public int AgentId { get; set; }
        /// <summary>
        /// 上级id
        /// </summary>
        [Range(1, 1000000)]
        public int TopAgentId { get; set; }

        public int RoleType { get; set; }

        [Required]
        public int PageSize { get; set; }

        [Required]
        public int PageIndex { get; set; }

        public DateTime SettledStartTime { get; set; }

        public DateTime SettledEndTime { get; set; }

        public int SettledState { get; set; }
    }
}
