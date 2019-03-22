using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    /// <summary>
    /// 修改机器人账号的账号和密码
    /// 涉及到bx_agent和manager_user_role_relation的修改
    /// </summary>
    public class UpdateManagerUserAccountRequest : BaseRequest
    {
        /// <summary>
        /// 要修改的代理人的id
        /// </summary>
        [Range(1, int.MaxValue)]
        public int AgentId { get; set; }
        /// <summary>
        /// 机器人账号
        /// </summary>
        [Required]
        public string AccountName { get; set; }

        private string accountPassword;

        /// <summary>
        /// 机器人密码，在更新时密码可以为空，在添加时密码不能为空
        /// </summary>
        public string AccountPassword
        {
            get
            {
                return accountPassword;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    accountPassword = "";
                }
                else
                {
                    accountPassword = CommonHelper.GetMd5(value);
                }
            }
        }
        /// <summary>
        /// 代理人名称
        /// </summary>
        [Required]
        public string AgentName { get; set; }
        /// <summary>
        /// 代理人手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 操作人id
        /// </summary>
        [Range(1, int.MaxValue)]
        public int OperatorId { get; set; }
        /// <summary>
        /// 操作人的名称
        /// </summary>
        [Required]
        public string OperatorName { get; set; }
        /// <summary>
        /// 编辑类型  Add:添加  Update:更新
        /// </summary>
        [Required]
        [RegularExpression("^(Add)|(Update)$", ErrorMessage = "EditType值不正确")]
        public string EditType { get; set; }

        /// <summary>
        /// 0待审核,1可用,2禁用,3删除
        /// </summary>
        //[RegularExpression("^[0-3]{1}$",ErrorMessage ="IsUsed值不正确")]
        public int IsUsed { get; set; }
        /// <summary>
        /// 来源 Manager:运营后台  CRM:机器人后台
        /// </summary>
        [Required]
        [RegularExpression("^(Manager)|(CRM)$", ErrorMessage = "Source值不正确")]
        public string Source { get; set; }
        public string ChargePerson { get; set; }
        public int IsRequote { get; set; }
    }
}
