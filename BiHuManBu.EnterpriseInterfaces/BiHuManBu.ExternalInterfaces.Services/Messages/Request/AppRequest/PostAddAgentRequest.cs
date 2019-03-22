using System.ComponentModel.DataAnnotations;


namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class PostAddAgentRequest
    {
        /// <summary>
        /// 经纪人姓名
        /// </summary>
        [Required]
        [StringLength(10, MinimumLength = 2)]
        public string AgentName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        [RegularExpression(@"^[1][3-9]+\d{9}")]
        public string Mobile { get; set; }
        /// <summary>
        /// 微信的唯一标识
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 16)]
        public string OpenId { get; set; }
        /// <summary>
        /// 父级代理人
        /// </summary>
        [Range(0, 21000000000)]
        public int TopParentAgent { get; set; }
        /// <summary>
        /// 当前代理人Id
        /// </summary>
        [Range(0, 21000000000)]
        public int AgentId { get; set; }
    }
}
