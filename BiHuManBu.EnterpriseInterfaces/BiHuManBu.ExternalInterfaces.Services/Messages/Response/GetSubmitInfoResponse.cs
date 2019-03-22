using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public class GetSubmitInfoResponse:BaseResponse
    {
        public bx_submit_info SubmitInfo { get; set; }

        public string CustKey { get; set; }
        
    }
}
