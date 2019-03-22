
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class GetHebaoRateRequest:BaseVerifyRequest
    {
        public int Source { get; set; }
        public long Buid { get; set; }
    }
}
