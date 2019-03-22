using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using ServiceStack.Text;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using ApiCustomizedAuthorize.CustomizedAuthorizes;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// UKey控制器
    /// </summary>
    public class AgentUKeyController : ApiController
    {
        private IAgentUKeyService agentUKeyService;
        private ICityService cityService;
        private IAgentConfigService _agentConfigService;
        private IAgentService _agentService;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private readonly ILog _logError = LogManager.GetLogger("ERROR");
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="agentUKeyService"></param>
        /// <param name="cityService"></param>
        /// <param name="agentConfigService"></param>
        public AgentUKeyController(IAgentUKeyService agentUKeyService, ICityService cityService, IAgentConfigService agentConfigService, IAgentService agentService)
        {
            this.agentUKeyService = agentUKeyService;
            this.cityService = cityService;
            _agentConfigService = agentConfigService;
            _agentService = agentService;
        }

        /// <summary>
        /// 批量更新UKey是否为报价渠道
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ModelVerify]
        public async Task<HttpResponseMessage> MultipleUpdateIsUsed(MultipleUpdateIsUsedRequest request)
        {
            logInfo.Info("批量更新UKey是否为报价渠道MultipleUpdateIsUsed请求串：" + Request.RequestUri + "，请求参数：" + request.ToJson());

            try
            {
                var result = await agentUKeyService.MultipleUpdateIsUsedAsync(request);
                return result.ResponseToJson();
            }
            catch (System.Exception ex)
            {
                _logError.Error("报价渠道批量开启关闭异常：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

                return BaseViewModel.GetBaseViewModel(Models.BusinessStatusType.SystemError).ResponseToJson();
            }

        }

        /// <summary>
        /// 获取可用渠道的城市
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ModelVerify]
        public HttpResponseMessage GetCanUseUkeyCity([FromUri]GetCanUseUkeyCityRequest request)
        {
            logInfo.Info("获取可用渠道的城市GetCanUseUkeyCity请求串：" + Request.RequestUri + "，请求参数：" + request.ToJson());
            return cityService.GetCanUseUkeyCity(request).ResponseToJson();
        }

        /// <summary>
        /// 分页获取ukey
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ModelVerify]
        //[CustomizedRequestAuthorize]
        public HttpResponseMessage GetPageUKey([FromUri]GetPageUKeyRequest request)
        {
            logInfo.Info("获取报价渠道GetPageUKey请求串：" + Request.RequestUri + "，请求参数：" + request.ToJson());
            return agentUKeyService.GetPageUKey(request).ResponseToJson();
        }

        /// <summary>
        /// 单个开启或关闭报价渠道 zky 2017-09-09 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify, Log("开启或关闭报价渠道")]
        public HttpResponseMessage EidtAgentConfigUsed([FromBody]EditAgentUsedRequest request)
        {
            if (request.ToPostSecCode() != request.SecCode)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamVerifyError).ResponseToJson();
            }
            BaseViewModel viewModel = new BaseViewModel();
            viewModel = _agentConfigService.UpdateCongfigUsed(request);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 获取Ukey备用密码 zky 2017-10-25/crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify, Log("获取Ukey备用密码")]
        public HttpResponseMessage GetUkeyBackupPwd([FromBody]GetUkeyBackupPwdRequest request)
        {
            UkeyBackupPwdViewModel viewModel = new UkeyBackupPwdViewModel();
            if (request.SecCode != request.ToPostSecCode())
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                return viewModel.ResponseToJson();
            }
            viewModel = _agentConfigService.GetUkeyBackupPwd(request.UkeyId);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 根据ukey信息获取所有渠道的状态
        /// </summary>
        /// <param name="ukeySourceRequest"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify, Log("获取所有渠道的状态")]
        public HttpResponseMessage GetUkeySource([FromBody]GetUkeySourceRequest ukeySourceRequest)
        {
            List<AgentCacheChannelModel> viewModel = new List<AgentCacheChannelModel>();
            //if (request.SecCode != request.ToPostSecCode())
            //{
            //    viewModel.BusinessStatus = -10001;
            //    viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
            //    return viewModel.ResponseToJson();
            //}
            viewModel = _agentConfigService.GetUkeySource(ukeySourceRequest.UkeyList);
            return viewModel.ResponseToJson();
        }
    }
}
