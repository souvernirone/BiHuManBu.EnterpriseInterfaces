using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Sms
{
    public class SmsAccountRequest : BaseRequest
    {
        [Range(1, 2100000000)]
        public int CurAgent { get; set; }
    }
}
