
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;


namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse
{
    public class GetAgentListResponse:BaseResponse
    {
        public int totalCount { get; set; }
        public List<bx_agent> AgentList { get; set; }
    }
}
