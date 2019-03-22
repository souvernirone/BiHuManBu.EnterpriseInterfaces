using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class SfAgentRequest
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "团队经理名称最多20个字符")]
        public string AgentName { get; set; }

        [Required]
        [RegularExpression(@"^[\da-zA-Z]{6,20}$", ErrorMessage = "账号为6~20位字母或数字组合")]
        public string AgentAccount { get; set; }

        public string AgentPassWord { get; set; }

        [RegularExpression(@"^\d+(,\d+)*$", ErrorMessage = "车商ID输入有误")]
        public string TopAgentIds { get; set; }

        [Range(0, 1, ErrorMessage = "账号状态有误")]
        public int IsUsed { get; set; }
        public int IsViewAllData { get; set; }
    }
}
