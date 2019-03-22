using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using ServiceStack.Text;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class PrecisePriceController : ApiController
    {
        public IVerifyService _verifyService;
        public IPrecisePriceService _precisePriceService;
        private ILog logInfo = LogManager.GetLogger("INFO");

        /// <summary>
        /// 报价相关业务
        /// </summary>
        /// <param name="verifyService"></param>
        public PrecisePriceController(IPrecisePriceService precisePriceService, IVerifyService verifyService)
        {
            _precisePriceService = precisePriceService;
            _verifyService = verifyService;
        }

        /// <summary>
        /// 判断是否是新进店的车
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetUserInfoStatus([FromUri]GetUserInfoStatusRequest request)
        {
            logInfo.Info(string.Format("判断是否是新进店车辆请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson()));
            var viewModel = new UserInfoStatusViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            //验证参数的对象转换
            var baseRequest = new BaseVerifyRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _verifyService.Verify(baseRequest, Request.GetQueryNameValuePairs());
            if (baseResponse.ErrCode != 1)
            {//校验失败，返回错误码
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel.ResponseToJson();
            }
            var response = _precisePriceService.GetUserInfoStatus(request);
            viewModel = response;
            viewModel.BusinessStatus = 1;
            return viewModel.ResponseToJson();
        }

    }
}
