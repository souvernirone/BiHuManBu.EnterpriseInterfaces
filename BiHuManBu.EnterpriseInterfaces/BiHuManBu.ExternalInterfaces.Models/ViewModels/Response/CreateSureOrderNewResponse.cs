using System.Net;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
    public class CreateSureOrderNewResponse
    {
        public HttpStatusCode Status { get; set; }
        public long OrderId { get; set; }
        public string TradeNum { get; set; }
    }
}
