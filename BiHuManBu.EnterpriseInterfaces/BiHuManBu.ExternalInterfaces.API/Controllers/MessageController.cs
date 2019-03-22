using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Mapper;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using ServiceStack.Text;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Message;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Message;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 消息系统接口
    /// </summary>
    public class MessageController : ApiController
    {
        private IMessageService _messageService;
        private ILog _logAppInfo;
        private ILog _logInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageService"></param>
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
            _logAppInfo = LogManager.GetLogger("APP");
            _logInfo = LogManager.GetLogger("INFO");
        }

        #region 新消息系统 消息列表、消息详情、最后一条消息详情、读消息修改标识
        /// <summary>
        /// 未读消息总数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage MsgCount([FromUri]MsgLastDetailRequest request)
        {
            _logInfo.Info(string.Format("获取消息总数接口请求串：{0}", Request.RequestUri));
            MsgCountViewModel viewModel = new MsgCountViewModel();
            var response = _messageService.MsgCount(request, Request.GetQueryNameValuePairs());
            //模型转换
            viewModel.TotalCount = response.TotalCount;
            viewModel.BusinessStatus = response.BusinessStatus;
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 消息列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage MsgList([FromUri]MessageListRequest request)
        {
            _logInfo.Info(string.Format("获取消息列表接口请求串：{0}", Request.RequestUri));
            MsgListViewModel viewModel = new MsgListViewModel();
            var response = _messageService.MsgList(request, Request.GetQueryNameValuePairs());
            viewModel.BusinessStatus = response.ErrCode;
            viewModel.StatusMessage = response.ErrMsg;
            //模型转换
            var displaylist = response.MsgList.ConvertToViewModel(request.MsgMethod);
            viewModel.BusinessStatus = 1;
            viewModel.MsgList = displaylist;
            viewModel.TotalCount = response.TotalCount;
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 消息详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ModelVerify]
        [HttpGet]
        public HttpResponseMessage MsgDetail([FromUri]MsgDetailRequest request)
        {
            _logInfo.Info(string.Format("获取消息详情接口请求串：{0}", Request.RequestUri));
            MsgInfoViewModel viewModel = new MsgInfoViewModel();
            var response = _messageService.MsgDetail(request, Request.GetQueryNameValuePairs());
            viewModel.BusinessStatus = response.ErrCode;
            viewModel.StatusMessage = response.ErrMsg;
            if (viewModel.BusinessStatus == 1)
            {
                //模型转换
                viewModel.Info = response.MsgInfo.ConvertToViewModel();
            }
            
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 最新一条消息详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage MsgLastDetail([FromUri]MsgLastDetailRequest request)
        {
            _logInfo.Info(string.Format("获取最新一条消息接口请求串：{0}", Request.RequestUri));
            MsgInfoViewModel viewModel = new MsgInfoViewModel();
            var response = _messageService.MsgLastDetail(request, Request.GetQueryNameValuePairs());
            viewModel.BusinessStatus = response.ErrCode;
            viewModel.StatusMessage = response.ErrMsg;
            //模型转换
            viewModel.Info = response.MsgInfo.ConvertToViewModel();
     
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 读消息修改标识
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage MsgRead([FromUri]MsgDetailRequest request)
        {
            _logInfo.Info(string.Format("修改消息状态接口请求串：{0}", Request.RequestUri));
            BaseViewModel viewModel = new BaseViewModel();
            var response = _messageService.MsgRead(request, Request.GetQueryNameValuePairs());
            if (response.Status == HttpStatusCode.OK)
            {
                viewModel.BusinessStatus = 1;
            }
            else
            {
                viewModel.BusinessStatus = 0;
            }
            return viewModel.ResponseToJson();
        }

        #endregion

        #region 迁移过来的消息模块
        [HttpPost]
        public HttpResponseMessage AddMessage([FromBody]AddMessageRequest request)
        {
            _logAppInfo.Info(string.Format("添加消息接口请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson()));
            var response = _messageService.AddMessage(request, Request.GetQueryNameValuePairs());
            _logAppInfo.Info(string.Format("添加消息接口返回值：{0}", response.ToJson()));
            return response.ResponseToJson();
        }

        [HttpGet]
        public HttpResponseMessage ReadMessage([FromUri]ReadMessageRequest request)
        {
            _logAppInfo.Info(string.Format("修改消息状态接口请求串：{0}", Request.RequestUri));
            BaseViewModel viewModel = new BaseViewModel();
            var response = _messageService.ReadMessage(request, Request.GetQueryNameValuePairs());
            _logAppInfo.Info(string.Format("修改消息状态接口返回值：{0}", response.ToJson()));
            if (response.Status == HttpStatusCode.OK)
            {
                viewModel.BusinessStatus = 1;
            }
            else
            {
                viewModel.BusinessStatus = 0;
            }
            return viewModel.ResponseToJson();
        }

        [HttpGet]
        public HttpResponseMessage MessageList([FromUri]MessageListRequest request)
        {
            _logAppInfo.Info(string.Format("消息列表获取接口请求串：{0}", Request.RequestUri));
            MessageListViewModel viewModel = new MessageListViewModel();
            var response = _messageService.MessageListNew(request, Request.GetQueryNameValuePairs());
            viewModel.BusinessStatus = response.ErrCode;
            viewModel.StatusMessage = response.ErrMsg;
            viewModel.MsgList = response.MsgList;
            viewModel.TotalCount = response.TotalCount;
            return viewModel.ResponseToJson();
        }

        [HttpGet]
        public HttpResponseMessage MsgClosedOrder([FromUri]MsgClosedOrderRequest request)
        {
            _logAppInfo.Info(string.Format("已出单详情接口请求串：{0}", Request.RequestUri));
            MsgClosedOrderViewModel viewModel = new MsgClosedOrderViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var response = _messageService.MsgClosedOrder(request, Request.GetQueryNameValuePairs());
            _logAppInfo.Info(string.Format("已出单详情接口返回值：{0}", response.ToJson()));
            if (response.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.ExpectationFailed)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                return viewModel.ResponseToJson();
            }
            viewModel.BusinessStatus = response.ErrCode;
            viewModel.StatusMessage = response.ErrMsg;
            if (response.ErrCode == 1)
            {
                viewModel.LicenseNo = response.LicenseNo;
                viewModel.MoldName = response.MoldName;
                viewModel.ReviewContent = response.ReviewContent;
                viewModel.SaAgent = response.SaAgent;
                viewModel.SaAgentName = response.SaAgentName;
                viewModel.AdvAgent = response.AdvAgent;
                viewModel.AdvAgentName = response.AdvAgentName;
                viewModel.SourceName = response.SourceName;
            }
            return viewModel.ResponseToJson();
        }

        [HttpGet]
        public HttpResponseMessage LastDayReInfoTotal([FromUri]LastDayReInfoTotalRequest request)
        {
            _logAppInfo.Info(string.Format("续保统计接口请求串：{0}", Request.RequestUri));
            LastDayReInfoTotalViewModel viewModel = new LastDayReInfoTotalViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var response = _messageService.LastDayReInfoTotal(request, Request.GetQueryNameValuePairs());
            _logAppInfo.Info(string.Format("续保统计接口返回值：{0}", response.ToJson()));
            if (response.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.ExpectationFailed)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                return viewModel.ResponseToJson();
            }
            viewModel.BusinessStatus = response.ErrCode;
            viewModel.StatusMessage = response.ErrMsg;
            if (response.ErrCode == 1)
            {
                viewModel.InStoreNum = response.InStoreNum;
                viewModel.ExpireNum = response.ExpireNum;
                viewModel.IntentionNum = response.IntentionNum;
                viewModel.OrderNum = response.OrderNum;
                viewModel.ReInfo = response.ReInfo.ConvertViewModel();
            }
            return viewModel.ResponseToJson();
        }
        #endregion

        /// <summary>
        /// 根据消息的id设置接收人的
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ModelVerify]
        public HttpResponseMessage SetMsgAgent(AddMsgAgentRequet request)
        {
            //当什么也不传时，ModelVerify可以通过，所以加这个验证
            if (request==null)
            {
                return new BaseViewModel
                {
                    BusinessStatus = -10000,
                    StatusMessage = "缺少必要参数"
                }.ResponseToJson();
            }

            //if (request.ToPostSecCode() == request.SecCode)
            //{
                var result = _messageService.SetMsgAgent(request);
                return result.ResponseToJson();
            //}
            //else
            //{
            //    return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamVerifyError).ResponseToJson();
            //}
        }
    }
}
