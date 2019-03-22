using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Services.Implements;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using System.Web.Http.Controllers;
using log4net;
using ServiceStack.Text;
using AppInterfaces = BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    [AuthorizeFilter]
    public class WorkOrderController : ApiController
    {
        //
        // GET: /WorkOrder/

        public IWorkOrderService _workOrderService;
        private ILog _logAppInfo;
        private IAgentService _agentService;


        public WorkOrderController(IWorkOrderService workOrderService, IAgentService agentService)
        {
            _workOrderService = workOrderService;
            _agentService = agentService;
            _logAppInfo = LogManager.GetLogger("APP");
        }

        /// <summary>
        /// 废弃-1.0版APP请求续保接口 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetReInfo")]
        // [EnableThrottling()]
        public async Task<HttpResponseMessage> FetchReInsuranceInfo([FromUri]AppRequest.GetReInfoRequest request)
        {
            _logAppInfo.Info(string.Format("App请求续保接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.GetReInfoNewViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            //if (request.RenewalType == 6 || request.RenewalType == 7)
            //{
            //    viewModel.BusinessStatus = 0;
            //    return viewModel.ResponseToJson();
            //}
            viewModel = await _workOrderService.GetReInfo(request, Request.GetQueryNameValuePairs(), Request.RequestUri);
            _logAppInfo.Info(string.Format("App请求续保接口返回值：{0}", viewModel.ToJson()));
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 废弃-1.0版APP添加投保意向接口 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddOrUpdateWorkOrder([FromBody]AppRequest.AddOrUpdateWorkOrderRequest request)
        {
            _logAppInfo.Info(string.Format("添加投保意向接口请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson()));

            var viewModel = new AppViewModels.AddOrUpdateWorkOrderViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var response = _workOrderService.AddOrUpdateWorkOrder(request, Request.GetQueryNameValuePairs());
            _logAppInfo.Info(string.Format("添加投保意向接口返回值：{0}", response.ToJson()));
            if (response.Status == HttpStatusCode.BadRequest || response.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                return viewModel.ResponseToJson();
            }
            if (response.ErrCode == -1)
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = response.ErrMsg;
                return viewModel.ResponseToJson();
            }
            else if (response.ErrCode == 1)
            {
                viewModel.BusinessStatus = 1;
                viewModel.WorkOrderId = response.WorkOrderId;
                viewModel.AdvAgentId = response.AdvAgentId;
                viewModel.StatusMessage = "创建成功";
            }
            else
            {
                viewModel.BusinessStatus = response.ErrCode;
                viewModel.StatusMessage = response.ErrMsg;
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 废弃-1.0版APP添加受理记录接口 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddOrUpdateWorkOrderDetail([FromBody]AppRequest.AddOrUpdateWorkOrderDetailRequest request)
        {
            _logAppInfo.Info(string.Format("添加受理记录接口请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson()));
            var viewModel = new AppViewModels.AddOrUpdateWorkOrderDetailViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var response = _workOrderService.AddOrUpdateWorkOrderDetail(request, Request.GetQueryNameValuePairs());
            _logAppInfo.Info(string.Format("添加受理记录接口返回值：{0}", response.ToJson()));
            if (response.Status == HttpStatusCode.BadRequest || response.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                return viewModel.ResponseToJson();
            }
            if (response.ErrCode == -1)
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = response.ErrMsg;
                return viewModel.ResponseToJson();
            }
            else if (response.ErrCode == 1)
            {
                viewModel.BusinessStatus = 1;
                viewModel.WorkOrderDetailId = response.WorkOrderDetailId;
                viewModel.StatusMessage = "创建成功";
            }
            else
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "创建失败";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 废弃-1.0版APP分发续保记录接口 光鹏洁 2016-?-? /APP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage ChangeReInfoAgent([FromUri]AppRequest.ChangeReInfoAgentRequest request)
        {
            _logAppInfo.Info(string.Format("分发续保记录接口请求串：{0}", Request.RequestUri));
            var viewModel = new AppViewModels.BaseViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var response = _workOrderService.ChangeReInfoAgent(request, Request.GetQueryNameValuePairs());
            _logAppInfo.Info(string.Format("分发续保记录接口返回值：{0}", response.ToJson()));
            viewModel.BusinessStatus = response.ErrCode;
            viewModel.StatusMessage = response.ErrMsg;
            return viewModel.ResponseToJson();
        }
    }
}
