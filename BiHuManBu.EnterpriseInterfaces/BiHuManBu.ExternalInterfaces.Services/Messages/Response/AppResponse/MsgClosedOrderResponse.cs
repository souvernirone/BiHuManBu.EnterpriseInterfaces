
namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse
{
    public class MsgClosedOrderResponse:BaseResponse
    {
        public string LicenseNo { get; set; }
        public string MoldName { get; set; }
        public string SourceName { get; set; }
        public string ReviewContent { get; set; }

        public int SaAgent { get; set; }
        public string SaAgentName { get; set; }
        public int AdvAgent { get; set; }
        public string AdvAgentName { get; set; }

    }
}
