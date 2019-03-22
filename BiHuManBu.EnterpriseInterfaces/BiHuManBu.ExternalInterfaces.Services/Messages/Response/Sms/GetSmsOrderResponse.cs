using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.Sms
{
    public class GetSmsOrderResponse : BaseResponse
    {
        public bx_sms_order SmsOrder { get; set; }
    }
}
