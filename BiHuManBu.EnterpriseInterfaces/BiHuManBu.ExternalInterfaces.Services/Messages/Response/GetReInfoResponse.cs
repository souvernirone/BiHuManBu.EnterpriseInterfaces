using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public  class GetReInfoResponse
    {
        public bx_userinfo UserInfo { get; set; }
        public bx_car_renewal SaveQuote { get; set; }
        public bx_carinfo CarInfo { get; set; }
        public HttpStatusCode Status { get; set; }
        public int BusinessStatus { get; set; }
        public string BusinessMessage { get; set; }
      
    }
}
