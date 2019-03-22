using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.BindBusiuser
{
    public interface ICheckCanBindBusiuserListService
    {
        Task<Tuple<BaseViewModel, int>> CheckRequestAsync(CanBandBusiuserRequest request);
    }
}
