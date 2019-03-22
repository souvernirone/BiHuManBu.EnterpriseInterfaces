using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 费率政策设置 
    /// </summary>
    //[CustomizedRequestAuthorize]
    public class RatePolicyController : ApiController
    {
        private readonly IRatePolicySettingService _ratePolicySettingService;

        public RatePolicyController(IRatePolicySettingService ratePolicySettingService)
        {
            _ratePolicySettingService = ratePolicySettingService;
        }

        /// <summary>
        /// 修改商业险或交强险费率自动转化为积分的阈值
        /// 陈亮 2017-12-29 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public async Task<HttpResponseMessage> SetOverTransferCredits([FromBody]SetOverTransferCreditsRequest request)
        {
            var result = await _ratePolicySettingService.SetOverTransferCreditsAsync(request);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 获取自动转化为积分的阈值
        /// 陈亮 2017-12-29 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public async Task<HttpResponseMessage> GetOverTransferCredits([FromUri]BaseRequest2 request)
        {
            var result = await _ratePolicySettingService.GetOverTransferCreditsAsync(request);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 新增和编辑费率接口
        /// 陈亮 2017-12-29 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> AddRate([FromBody]AddRateRequest request)
        {
            var result = await _ratePolicySettingService.AddRateAsync(request);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 获取费率列表
        /// 陈亮 2017-12-29 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public async Task<HttpResponseMessage> GetRateList([FromUri]BaseRequest2 request)
        {
            var result = await _ratePolicySettingService.GetRateListAsync(request);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 删除费率
        /// 陈亮 2017-12-29 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public async Task<HttpResponseMessage> DeleteRate([FromBody]DeleteRateRequest request)
        {
            var result = await _ratePolicySettingService.DeleteRateAsync(request);
            return result.ResponseToJson();
        }
    }
}
