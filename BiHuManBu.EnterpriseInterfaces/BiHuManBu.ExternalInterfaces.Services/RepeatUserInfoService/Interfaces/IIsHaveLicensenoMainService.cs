using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;

namespace BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces
{
    public interface IIsHaveLicensenoMainService
    {
        IsHaveLicenseNoResult GetInfo(IsHaveLicensenoRequest request);
        IsHaveLicenseNoResult GetRepeatQuoteInfo(IsHaveLicensenoRequest request);
    }
}
