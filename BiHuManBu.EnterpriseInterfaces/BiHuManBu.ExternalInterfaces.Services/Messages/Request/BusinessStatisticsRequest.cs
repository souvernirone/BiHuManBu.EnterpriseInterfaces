using System;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    /// <summary>
    /// 业务统计
    /// </summary>
    public class BusinessStatisticsRequest
    {
        
        /// <summary>
        /// 代理人id
        /// </summary>
        [Range(1, 1000000)]
        public int AgentId { get; set; }

        /// <summary>
        /// 顶级代理人id
        /// </summary>
        [Range(1, 1000000)]
        public int TopAgentId { get; set; }

        /// <summary>
        /// 角色类型3顶级系统管理,4管理员
        /// </summary>
        public int RoleType { get; set; }

        /// <summary>
        /// 校验参数
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}
