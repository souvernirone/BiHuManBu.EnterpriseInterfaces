using System.Collections.Generic;
using System.Net;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public  class GetAgentResponse
    {
        public HttpStatusCode Status { get; set; }
        public List<int> CompList { get; set; } 
    }
}
