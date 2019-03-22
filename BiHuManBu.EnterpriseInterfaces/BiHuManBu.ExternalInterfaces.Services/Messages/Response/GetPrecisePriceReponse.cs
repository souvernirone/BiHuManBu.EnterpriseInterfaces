using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public class GetPrecisePriceReponse
    {
        public bx_userinfo UserInfo { get; set; }
        public bx_lastinfo LastInfo { get; set; }
        public bx_savequote SaveQuote { get; set; }
        public bx_quoteresult QuoteResult { get; set; }

        public bx_submit_info SubmitInfo { get; set; }
        public HttpStatusCode Status { get; set; }

        public bx_car_renewal Renewal { get; set; }
    }
}
