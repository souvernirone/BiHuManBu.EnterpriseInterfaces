using BiHuManBu.ExternalInterfaces.Infrastructure.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    /// <summary>
    /// 发送短信请求模型
    /// </summary>
    public class AddOrUpdateSmsAccountContentRequest
    {
        /// <summary>
        /// 代理人id
        /// </summary>
        [Range(1,int.MaxValue)]
        public int agent_id { get; set; }
        /// <summary>
        /// 代理人名称
        /// </summary>
        [Required]
        public string agent_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string content { get; set; }
        /// <summary>
        /// 发送手机号
        /// </summary>
        [Mobile]
        public string sent_mobile { get; set; }
        /// <summary>
        /// 0是pc端1是微信端
        /// </summary>
        [RegularExpression("^[0,1]$",ErrorMessage = "sent_type参数不正确")]
        public int sent_type { get; set; }
        /// <summary>
        /// 车牌
        /// </summary>
        [MaxLength(20,ErrorMessage ="车牌号超过长度限制")]
        public string license_no { get; set; }
    }
}
