using System.Collections.Generic;
using System.Net;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public  class GetCityListResponse
    {
        public HttpStatusCode Status { get; set; }
        public List<bx_city> Cities { get; set; } 
    }
}
