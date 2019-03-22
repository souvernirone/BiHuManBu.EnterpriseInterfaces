
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public  class GetAgentResourceRequest:BaseRequest
    {
        public int CityCode { get; set; }
    }
}
