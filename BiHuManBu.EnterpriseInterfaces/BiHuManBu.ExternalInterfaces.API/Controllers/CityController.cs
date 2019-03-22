using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Mapper.AppMapper;
using ServiceStack.Text;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using log4net;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class CityController : ApiController
    {
        private ICityService _cityService;
        private ILog _logInfo;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
            _logInfo = LogManager.GetLogger("INFO");
        }
        /// <summary>
        /// 获取城市列表 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetCityList([FromUri]BaseRequest request)
        {
            _logInfo.Info(string.Format("获取城市接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.CityViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                return viewModel.ResponseToJson();
            }

            var response = await _cityService.GetCityList(request, Request.GetQueryNameValuePairs());
            //添加日志
            _logInfo.Info(response.ToJson());

            if (response.Status == HttpStatusCode.BadRequest || response.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                _logInfo.Info(viewModel.ResponseToJson());
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.ExpectationFailed)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
            }
            else
            {
                viewModel = response.Cities.ConvertViewModel();
                viewModel.BusinessStatus = 1;
            }
            return viewModel.ResponseToJson();
        }



        /// <summary>
        /// 指定代理人Id删除
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        [HttpGet]
        public Models.ViewModels.BaseViewModel ResetAgent(int agent)
        {
            string agentKey = "agent_key_cache_" + agent;
            CacheProvider.Remove(agentKey);
            return new Models.ViewModels.BaseViewModel
            {
                BusinessStatus = 1,
                StatusMessage = "清理成功"
            };
        }
        /// <summary>
        /// 批量删除代理人缓存Id
        /// </summary>
        /// <param name="fromid"></param>
        /// <param name="toid"></param>
        /// <returns></returns>
        [HttpGet]
        public Models.ViewModels.BaseViewModel ResetMoreAgent(int fromid, int toid)
        {
            string agentKey = string.Empty;
            for (int i = fromid; i <= toid; i++)
            {
                agentKey = "agent_key_cache_" + i;
                CacheProvider.Remove(agentKey);
            }
            return new Models.ViewModels.BaseViewModel
            {
                BusinessStatus = 1,
                StatusMessage = "清理成功"
            };
        }
        /// <summary>
        /// 自定义删除缓存Key
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        [HttpGet]
        public Models.ViewModels.BaseViewModel ResetCustom(string cacheKey)
        {
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                CacheProvider.Remove(cacheKey);
            }
            return new Models.ViewModels.BaseViewModel
            {
                BusinessStatus = 1,
                StatusMessage = "清理成功"
            };
        }
    }
}
