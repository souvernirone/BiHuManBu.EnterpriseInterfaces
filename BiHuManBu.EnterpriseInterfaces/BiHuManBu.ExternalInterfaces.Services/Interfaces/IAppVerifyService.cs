using System.Collections.Generic;
using AppRequest=BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppResponse = BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces
{
    public interface IAppVerifyService
    {
        AppResponse.AppBaseResponse Verify(AppRequest.AppBaseRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        AppResponse.AppBaseResponse VerifyParams(AppRequest.AppBaseRequest request, IList<KeyValuePair<string, string>> pairs);
    }
}
