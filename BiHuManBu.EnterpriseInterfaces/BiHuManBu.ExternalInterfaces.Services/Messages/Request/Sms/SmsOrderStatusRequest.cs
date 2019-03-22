using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Sms
{
    public class SmsOrderStatusRequest//:BaseRequest
    {
        [Required]
        [StringLength(16, MinimumLength = 16)]
        public string OrderNum { get; set; }
        [Range(-5, 5)]
        public int OrderStatus { get; set; }
        //[Range(0,1000000)]
        //public int CurAgent { get; set; }

        /// <summary>
        /// 第三方交易流水号
        /// </summary>
        [StringLength(32, MinimumLength = 20)]
        public string TradeNo { get; set; }
    }
}
