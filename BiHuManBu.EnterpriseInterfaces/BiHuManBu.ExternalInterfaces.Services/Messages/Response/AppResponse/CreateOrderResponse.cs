using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse
{
    public class CreateOrderResponse
    {
        public HttpStatusCode Status { get; set; }
        public Int64 OrderId { get; set; }
        public string TradeNum { get; set; }

    }
}
