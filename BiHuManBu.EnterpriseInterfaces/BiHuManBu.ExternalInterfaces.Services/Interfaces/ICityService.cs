using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// bx_city逻辑
    /// </summary>
    public interface ICityService
    {
        /// <summary>
        /// 获取可用渠道的城市
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetCanUseUkeyCityViewModel GetCanUseUkeyCity(GetCanUseUkeyCityRequest request);

        Task<GetCityListResponse> GetCityList(BaseRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
    }
}
