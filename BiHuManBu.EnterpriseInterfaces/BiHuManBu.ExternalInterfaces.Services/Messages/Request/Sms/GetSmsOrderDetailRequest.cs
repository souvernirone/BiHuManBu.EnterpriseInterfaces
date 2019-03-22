using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Sms
{
    public class GetSmsOrderDetailRequest : BaseRequest
    {
        /// <summary>
        /// 短信订单Id
        /// </summary>
        [Range(1, 2100000000)]
        public int SmsOrderID { get; set; }
    }
}
