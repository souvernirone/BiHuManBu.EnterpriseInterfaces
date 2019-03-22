using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Sms
{
    public class SmsCreateOrderRequest : BaseRequest
    {
        [Range(1, 1000000)]
        public int CurAgent { get; set; }
        public decimal Amount { get; set; }
        [Range(1, 1000000)]
        public int Quantity { get; set; }
        [Range(0, 10)]
        public int PayType { get; set; }

        /// <summary>
        /// 渠道来源，1微信申请；2PC申请
        /// </summary>
        [Range(1, 2)]
        public int Method { get; set; }
    }
}
