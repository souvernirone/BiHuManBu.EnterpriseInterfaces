using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.CacheRequest;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 缓存控制器
    /// </summary>
    public class CacheController : ApiController
    {
        private ICacheService cacheService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheService"></param>
        public CacheController(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        /// <summary>
        /// ukey更新后清除缓存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ModelVerify]
        public HttpResponseMessage ClearUKeyCache([FromUri]ClearUKeyCacheRequest request)
        {
            return cacheService.ClearUKeyCache(request).ResponseToJson();
        }
    }
}
