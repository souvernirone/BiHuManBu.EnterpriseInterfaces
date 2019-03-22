using System.Net;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse
{
    public class GetAgentResponse : BaseResponse
    {
        public Models.ReportModel.AgentModel agent { get; set; }

        //public HttpStatusCode Status { get; set; }
    }
}
