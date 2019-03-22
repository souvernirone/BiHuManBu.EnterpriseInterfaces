using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public  class GetEscapedInfoResponse
    {
        public List<bx_claim_detail> List { get; set; }
        public HttpStatusCode Status { get; set; }
    }
}
