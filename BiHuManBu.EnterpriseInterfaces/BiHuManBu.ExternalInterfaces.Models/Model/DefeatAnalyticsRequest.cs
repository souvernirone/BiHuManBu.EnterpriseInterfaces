using System;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    /// <summary>
    /// 战败统计
    /// </summary>
    public class DefeatAnalyticsRequest
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
        /// 角色类型3为系统管理，4管理员
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


    /// <summary>
    /// 原因分析
    /// </summary>
    public class ReasonAnalyticsRequest
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
        /// 角色类型3为系统管理，4管理员
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

    public class ReasonAnalyticsRequest4Mobile : ReasonAnalyticsRequest
    {
        public string categoryName { get; set; }
        public int IsViewAllData { get; set; }
    }


    /// <summary>
    /// 业务员战败数据统计
    /// </summary>
    public class AgentAnalyticsRequest
    {
        public AgentAnalyticsRequest()
        {
            //PageIndex = 1;
        }

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
        /// 角色类型3为系统管理，4管理员
        /// </summary>
        public int RoleType { get; set; }
        /// <summary>
        /// 校验参数
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }


        /// <summary>
        /// 当前页
        /// </summary>
        public int CurPage { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchTxt { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 是否倒序
        /// </summary>
        public bool IsDesc { get; set; }
    }
}
