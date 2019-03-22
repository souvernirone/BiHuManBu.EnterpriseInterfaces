using System;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages
{
    public class AgentDataRequest 
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public AgentDataRequest()
        {
            IsDesc = false;
            CurPage = 1;
            PageSize = 10;
            OrderBy = "agentid";
            RoleType = 0;
            TopAgentId = 0;
            IsByLevel = false;
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
        /// 角色类别
        /// </summary>
        public int RoleType { get; set; }
        /// <summary>
        /// 安全校验码
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

        /// <summary>
        /// 排序参数
        /// </summary>
        [Required]
        public string OrderBy { get; set; }

        /// <summary>
        /// 是否排序
        /// </summary>
        public bool IsDesc { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurPage { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 搜索字段
        /// </summary>
        public string SearchTxt { get; set; }

        /// <summary>
        /// 是否根据层级查询
        /// </summary>
        public bool IsByLevel { get; set; }
    }
}
