using BiHuManBu.ExternalInterfaces.Models.ViewModels.Enum;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Sms
{
    public class SmsRequest : BaseRequest
    {
        [Required]
        [RegularExpression(@"^[1][3-9]+\d{9}")]
        public string Mobile { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string SmsContent { get; set; }

        /// <summary>
        /// 发送类型：0 pc端发送，1微信端发送
        /// </summary>
        [Range(0, 100)]
        public int SentType { get; set; }

        public string LicenseNo { get; set; }

        [Range(0, 100)]
        public EnumSmsBusinessType BusinessType { get; set; }

        [Range(0, 2100000000)]
        public int CurAgent { get; set; }
    }
}
