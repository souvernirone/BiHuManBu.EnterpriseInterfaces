using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class ZuoXiService : IZuoXiService
    {
        private IZuoXiRepository zuoXiRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zuoXiRepository"></param>
        public ZuoXiService(IZuoXiRepository zuoXiRepository)
        {
            this.zuoXiRepository = zuoXiRepository;
        }

        /// <summary>
        /// 判断能否打电话，并且获取加拨号码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetZuoXiViewModel> GetZuoXi(GetZuoXiRequest request)
        {
            var result = GetZuoXiViewModel.GetModel(BusinessStatusType.OK);
            var zuoXi = await zuoXiRepository.FirstOrDefaultAsync(o => o.agent_id == request.AgentId && o.status == 1);
            if (zuoXi == null)
            {
                result.CanCall = false;
                return result;
            }
            
            result.CanCall = true;
            result.AddNumber = zuoXi.add_number;
            return result;
        }
    }
}
