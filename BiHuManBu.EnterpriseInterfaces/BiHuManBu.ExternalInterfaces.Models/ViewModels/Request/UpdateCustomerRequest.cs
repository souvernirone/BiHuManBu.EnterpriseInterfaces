using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class UpdateCustomerRequest
    {
        /// <summary>
        /// 要修改的代理人ID
        /// </summary>
        [Range(1, 100000000)]
        public int AgentId { get; set; }
        /// <summary>
        /// 是否展示费率 0：是 1：否
        /// </summary>
        [Range(0, 1)]
        public int IsShow { get; set; }
        /// <summary>
        /// 微信是否展示计算器 0：是 1：否
        /// </summary>
        [Range(0, 1)]
        public int IsShowCalc { get; set; }
        /// <summary>
        /// 账号状态 0：待审核、1：启用、2：禁用
        /// </summary>
        [Range(0, 2)]
        public int IsUsed { get; set; }
        /// <summary>
        /// 短信扣费方式 0：从顶级扣费、1：从业务员本身扣费、2：三级代理从二级扣费
        /// </summary>
        [Range(0, 2)]
        public int MessagePayType { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        [Range(1, 100000000)]
        public int RoleId { get; set; }
        /// <summary>
        /// 是否可核保 1：:是  2：否
        /// </summary>
        //[Range(1, 2)]
        public int IsSubmit { get; set; }
        /// <summary>
        /// 上级邀请码
        /// </summary>
        [Range(1, 100000000)]
        public int ParentShareCode { get; set; }
        /// <summary>
        /// 是否修改上级邀请码
        /// </summary>
        public bool IsModifyParentShareCode { get; set; }
        /// <summary>
        /// 考试状态：默认-1，传1和0 通过和不通过     add by qidakang 2018-4-4 16:06:00
        /// </summary>
        [Range(-1,1)]
        public int TestState { get; set; }
        public int TopAgentId { get; set; }

        public string Mobile { get; set; }


        public int IsGrabOrder { get; set; }
    }
}
