using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.Sms
{
    public class GetSmsAccountResponse : BaseResponse
    {
        public bx_sms_account SmsAccount { get; set; }
    }
}
