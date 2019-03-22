using System;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class SfH5Request
    {
        public SfH5Request()
        {
            IsDesc = false;
            PageIndex = 1;
            PageSize = 20;
            OrderBy = "agentid";
        }
        /// <summary>
        /// 代理人ID
        /// </summary>
        [Range(1, 1000000)]
        public int AgentId { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 是否降序
        /// </summary>
        public bool IsDesc { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 单页条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 搜索文本
        /// </summary>
        public string SerachText { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Required]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 安全校验码
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }
    }
}