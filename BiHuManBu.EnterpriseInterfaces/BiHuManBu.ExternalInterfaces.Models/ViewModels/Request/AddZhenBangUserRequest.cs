using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AddZhenBangUserRequest
    {
        /// <summary>
        /// 账号类型 (1机构、2网点、3内部员工、4外部代理)
        /// </summary>
        [Range(1, 4)]
        public int ZhenBangType { get; set; }

        /// <summary>
        /// 登录账号
        /// </summary>
        [StringLength(20, MinimumLength = 6, ErrorMessage = "账号位数在6-20个字符之间")]
        public string AgentAccount { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码位数在6-20个字符之间")]
        public string AgentPwd { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(11, MinimumLength = 11, ErrorMessage = "手机号位数错误")]
        public string Mobile { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [StringLength(30, MinimumLength = 1, ErrorMessage = "姓名位数应在1-30个字符之间")]
        public string AgentName { get; set; }
        /// <summary>
        /// 顶级代理人id
        /// </summary>
        [Range(1, 100000000)]
        public int TopAgentId { get; set; }
        /// <summary>
        /// 代理人id
        /// </summary>
        [Range(1, 100000000)]
        public int AgentId { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string ChargePerson { get; set; }
    }
}
