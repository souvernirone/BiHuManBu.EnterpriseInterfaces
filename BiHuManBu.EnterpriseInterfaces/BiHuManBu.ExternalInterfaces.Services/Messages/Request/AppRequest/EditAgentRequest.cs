using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class EditAgentRequest: AppBaseRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [StringLength(15, MinimumLength = 6)]
        [RegularExpression(@"^[a-zA-Z0-9]*", ErrorMessage = "帐户名由数字和字母组成")]
        public string AgentAccount { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string AgentPwd { get; set; }

        /// <summary>
        /// 账号状态
        /// </summary>
        [Range(0, 3)]
        public int IsUsed { get; set; }
        /// <summary>
        /// 要修改的代理人id
        /// </summary>
        public int EditAgent { get; set; }
    }
}
