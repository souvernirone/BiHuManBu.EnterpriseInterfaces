using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class AgentBatchAuditRequest : BaseRequest
    {
        /// <summary>
        /// 批量更新的代理id
        /// </summary>
        [Required]
        public List<int> AgentIds { get; set; }

        /// <summary>
        /// 当前代理人Id
        /// </summary>
        [Range(1, 1000000)]
        public int AgentId { get; set; }

        /// <summary>
        /// 短信扣费方式
        /// </summary>
        [Range(0, 2)]
        public int MessagePayType { get; set; }

        /// <summary>
        /// 启用状态
        /// </summary>
        [Range(0, 3)]
        public int UsedStatus { get; set; }

        /// <summary>
        /// 是否展示费率
        /// </summary>
        [Range(0, 1)]
        public int IsShowRate { get; set; }

        /// <summary>
        /// 是否可核保
        /// </summary>
        [Range(1, 2)]
        public int IsSubmit { get; set; }
    }

    public class EditAgentIsUsedRequest
    {
        public List<int> AgentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Range(1,2,ErrorMessage ="启用状态错误")]
        public int IsUsed { get; set; }
    }
}
