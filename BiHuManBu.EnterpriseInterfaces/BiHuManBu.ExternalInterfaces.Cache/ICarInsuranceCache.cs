using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;

namespace BiHuManBu.ExternalInterfaces.Cache
{
    public interface ICarInsuranceCache
    {
        GetReInfoResponse GetReInfo(GetReInfoRequest request);

        GetPrecisePriceReponse GetPrecisePrice(GetPrecisePriceRequest request);

        GetSubmitInfoResponse GetSubmitInfo(GetSubmitInfoRequest request);
    }
}
