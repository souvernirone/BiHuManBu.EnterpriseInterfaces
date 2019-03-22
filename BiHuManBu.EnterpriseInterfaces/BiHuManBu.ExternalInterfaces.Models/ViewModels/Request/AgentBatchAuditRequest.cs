using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AgentBatchAuditRequest : BaseRequest
    {
        /// <summary>
        /// 批量更新的代理id
        /// </summary>
        public List<int> AgentIds { get; set; }

        /// <summary>
        /// 当前代理人Id
        /// </summary>
        [Range(1, 1000000)]
        public int AgentId { get; set; }

        /// <summary>
        /// 短信扣费方式
        /// </summary>
        [Required]
        public int MessagePayType { get; set; }

        /// <summary>
        /// 启用状态
        /// </summary>
        [Required]
        public int UsedStatus { get; set; }

        /// <summary>
        /// 是否展示费率
        /// </summary>
        [Required]
        public int IsShowRate { get; set; }

        /// <summary>
        /// 是否可核保
        /// </summary>
        [Required]
        public int IsSubmit { get; set; }
    }
}
