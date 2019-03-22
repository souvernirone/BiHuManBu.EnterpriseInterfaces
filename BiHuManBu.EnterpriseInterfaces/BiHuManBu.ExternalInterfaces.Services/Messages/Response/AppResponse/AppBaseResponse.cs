
namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse
{
    public class AppBaseResponse : BaseResponse
    {
        public string CustKey { get; set; }
        public long? Buid { get; set; }
        public int Agent { get; set; }
        public string AgentName { get; set; }
    }
}
