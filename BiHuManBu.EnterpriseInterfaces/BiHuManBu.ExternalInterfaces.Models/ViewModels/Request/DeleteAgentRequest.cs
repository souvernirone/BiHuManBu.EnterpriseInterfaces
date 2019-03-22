using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class DeleteAgentRequest
    {
        /// <summary>
        /// 要删除的代理人id
        /// </summary>
        [Range(1,100000000)]
        public int AgentId { get; set; }
        /// <summary>
        /// 删除账号id
        /// </summary>
        [Range(1, 100000000)]
        public int DeleteUserId { get; set; }
        /// <summary>
        /// 删除账号
        /// </summary>
        [Required]
        public string DeleteAccount { get; set; }
        /// <summary>
        /// 1运营后台删除、2机器人删除
        /// </summary>
        [Range(1, 2)]
        public int DeletePlatfrom { get; set; }
        /// <summary>
        /// 安全校验码
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }
    }
}
