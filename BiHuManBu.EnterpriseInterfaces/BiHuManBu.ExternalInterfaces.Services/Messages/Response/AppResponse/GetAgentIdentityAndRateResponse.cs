using System.Collections.Generic;
using System.Net;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse
{
    public  class GetAgentIdentityAndRateResponse
    {
        /// <summary>
        /// 是否是经纪人 1:经纪人 0：直客
        /// </summary>
        public int IsAgent { get; set; }

        public Implements.Rate AgentRate { get; set; }

        public List<Implements.Rate> ZhiKeRate { get; set; }


        public HttpStatusCode Status { get; set; }

    }
}
