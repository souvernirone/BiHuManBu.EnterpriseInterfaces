using BiHuManBu.ExternalInterfaces.Infrastructure.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 添加代理人接口
    /// </summary>
    public class AddAgentRequest:BaseRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [Mobile]
        public string Mobile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string AgentName { get; set; }
        /// <summary>
        /// 添加成功后是否需要登录
        /// </summary>
        public bool IsCheckCode { get; set; }
        /// <summary>
        /// 注册类型:AddAgent
        /// </summary>
        public string RegisterType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string CustKey { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SetPhoneAndWechatAgentRequest : BaseVerifyRequest 
    {
        /// <summary>
        /// 手机是否与微信同号  默认：0不同号，设置1为同号
        /// </summary>
        public int PhoneAndWechat { get; set; }
    }
}
