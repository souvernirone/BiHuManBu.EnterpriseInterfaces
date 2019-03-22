using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService
{
    public interface IReWriteRelationInfoService
    {
        RenewalInfo ReWriteUserInfoService(RenewalInfo renewalInfo, int topAgentId);
    }
}
