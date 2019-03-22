using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    /// <summary>
    /// 外部登录输入模型
    /// </summary>
    public class ExternalLoginRequest
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required(ErrorMessage = "账号不能为空")]
        [MaxLength(80,ErrorMessage = "账号不能操作80个字符")]
        public string UserName { get; set; }

        /// <summary>
        /// 顶级代理人id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "顶级搭理人Id不能小于1")]
        public int AgentId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        private int _fromMethod = -1;
        /// <summary>
        /// 1:PC 2:微信 4:APP
        /// </summary>
        public int FromMethod
        {
            get { return _fromMethod; }
            set { _fromMethod = value; }
        }
    }
}
