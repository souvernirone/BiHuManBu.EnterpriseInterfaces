using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces
{
    public interface IIsHaveLicensenoToQuoteService
    {
        IsHaveLicensenoToQuoteViewModel HaveLicensenoToQuote(IsHaveLicensenoToQuoteRequest request);
    }
}
