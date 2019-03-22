
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class PostSendSmsRequest:AppBaseRequest
    {
        [Required]
        [RegularExpression(@"^[1][3-9]+\d{9}")]
        public string Mobile { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string SmsContent { get; set; }

        ///// <summary>
        ///// 发送类型：0 pc端发送，1微信端发送，2APP
        ///// </summary>
        //[Range(0, 100)]
        //public int SentType { get; set; }

        public string LicenseNo { get; set; }

        ///// <summary>
        ///// APP默认传8（发报价用），如果登陆传1，注册传5，找回密码6
        ///// </summary>
        //[Range(0, 100)]
        //public EnumSmsBusinessType BusinessType { get; set; }
        
        public int source { get; set; }
        public double bizRate { get; set; }
        public double forceRate { get; set; }
    }
}
