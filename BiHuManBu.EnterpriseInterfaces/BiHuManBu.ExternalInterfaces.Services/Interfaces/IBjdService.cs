using System.Collections.Generic;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppResponse = BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces
{
    public interface IBjdService
    {
        AppResponse.AppGetReInfoResponse GetReInfoDetail(AppRequest.GetReInfoDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
    }
}
