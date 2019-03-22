using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 坐席控制器
    /// </summary>
    public class ZuoXiController : ApiController
    {
        private IZuoXiService zuoXiService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zuoXiService"></param>
        public ZuoXiController(IZuoXiService zuoXiService)
        {
            this.zuoXiService = zuoXiService;
        }

        /// <summary>
        /// 获取打电话权限和加拨号码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ModelVerify]
        public async Task<HttpResponseMessage> GetZuoXi([FromUri]GetZuoXiRequest request)
        {
            var encryptStr = string.Format("AgentId={0}&Agent={1}", request.AgentId, request.Agent);

            if (encryptStr.GetUrl().GetMd5() != request.SecCode)
            {
                return GetZuoXiViewModel.GetModel(Models.BusinessStatusType.ParamVerifyError).ResponseToJson();
            }

            return (await zuoXiService.GetZuoXi(request)).ResponseToJson();

        }
    }
}
