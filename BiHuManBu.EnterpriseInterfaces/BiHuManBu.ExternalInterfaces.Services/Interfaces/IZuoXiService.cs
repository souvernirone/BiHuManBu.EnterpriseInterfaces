using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZuoXiService
    {
        /// <summary>
        /// 判断能否打电话，并且获取加拨号码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<GetZuoXiViewModel> GetZuoXi(GetZuoXiRequest request);
    }
}
