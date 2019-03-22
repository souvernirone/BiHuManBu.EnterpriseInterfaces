using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class ConsumerDetailSmsViewModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [RegularExpression(@"^1[^012]\d{9}$", ErrorMessage ="手机号格式不正确")]
        public string mobile { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        [Required(ErrorMessage ="短信内容不能为空")]
        public string content { get; set; }
        /// <summary>
        /// 车牌
        /// </summary>
        [MaxLength(20, ErrorMessage = "车牌号超过长度限制")]
        public string licenseNo { get; set; }
        public int buid { get; set; }
        public int agentId { get; set; }
        public int source { get; set; }
        public int IsNewSource { get; set; }
        public double bizRate { get; set; }
        public double forceRate { get; set; }
        public int TopAgentId { get; set; }
        /// <summary>
        /// 手机号是否与微信同号：0不同号、1同号
        /// </summary>
        public int IsWeChat { get; set; }
        /// <summary>
        /// 优惠活动ID
        /// </summary>
        public int ActivityId { get; set; }
        /// <summary>
        /// 短信签名
        /// </summary>
        public string SmsSign { get; set; }
    }

    public class ConsumerDetailRequest
    {
        /// <summary>
        /// 短信内容
        /// </summary>
        [Required(ErrorMessage = "短信内容不能为空")]
        public string content { get; set; }
        /// <summary>
        /// 车牌
        /// </summary>
        [MaxLength(20, ErrorMessage = "车牌号超过长度限制")]
        public string licenseNo { get; set; }
        public int buid { get; set; }
        public int agentId { get; set; }
        public int source { get; set; }
        public int IsNewSource { get; set; }
        public double bizRate { get; set; }
        public double forceRate { get; set; }
        public int TopAgentId { get; set; }
    }
}
