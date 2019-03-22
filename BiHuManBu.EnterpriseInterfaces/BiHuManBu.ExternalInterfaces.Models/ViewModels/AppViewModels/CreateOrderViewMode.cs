
using System;
using System.Net;
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public  class CreateOrderViewMode:BaseViewModel
    {
        public long OrderId { get; set; }
        public HttpStatusCode Status { get; set; }
        public string TradeNum { get; set; }
    }
}
