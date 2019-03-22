using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 事故线索发送短信
    /// </summary>
    public class AccidentSendMessageRequest
    {
        /// <summary>
        /// 当前用户id
        /// </summary>
        [Range(1, 2100000000)]
        public int AgentId { get; set; }
        /// <summary>
        /// 上级id
        /// </summary>
        [Range(1, 1000000, ErrorMessage = "TopAgentId参数错误")]
        public int TopAgentId { get; set; }

        /// <summary>
        /// 线索id
        /// </summary>
        [Required]
        public int ClueId { get; set; }

        /// <summary>
        /// 报案人手机号
        /// </summary>
        [Required]
        [RegularExpression(@"^[1][3-9]+\d{9}",ErrorMessage ="手机号不正确")]
        public string Phone { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string SmsContent { get; set; }

        [Required]
        public string Token { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }
    }
}
