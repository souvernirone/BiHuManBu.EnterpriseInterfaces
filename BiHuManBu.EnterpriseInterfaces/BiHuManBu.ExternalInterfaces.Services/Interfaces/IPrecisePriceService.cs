
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IPrecisePriceService
    {
        UserInfoStatusViewModel GetUserInfoStatus(GetUserInfoStatusRequest request);
    }
}
