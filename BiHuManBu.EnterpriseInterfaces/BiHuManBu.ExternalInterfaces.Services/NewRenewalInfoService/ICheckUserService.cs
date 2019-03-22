using System;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService
{
    public interface ICheckUserService
    {
        ResultMessage CheckUser(GetRenewalRequest request);
    }
}
