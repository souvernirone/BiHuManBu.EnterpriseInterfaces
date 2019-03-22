using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.Net.Http;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;


namespace BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService
{
    public interface IGetRenewalInfoService
    {
        GetRenewalInfoViewModel GetRenewalInfo(GetRenewalRequest request);
    }
}
