using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using ServiceStack.Text;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 预约单
    /// </summary>
    public class AppoinmentController : ApiController
    {
        private readonly ILog _logInfo;
        private readonly ILog _logError;
        private readonly IAppoinmentService _appoinmentService;
        private readonly IVerifyService _verifyService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public AppoinmentController(IAppoinmentService appoinmentService, IVerifyService verifyService)
        {
            try
            {
                _logInfo = LogManager.GetLogger("INFO");
                _logError = LogManager.GetLogger("ERROR");
                _appoinmentService = appoinmentService;
                _verifyService = verifyService;
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取预约单列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetAppoinmentInfoListData(AppointmentOrderRequest request)
        {
            _logInfo.InfoFormat("获取预约单请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson());
            AppoinmentViewModel viewModel = new AppoinmentViewModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    viewModel.BusinessStatus = -10000;
                    string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    viewModel.StatusMessage = "输入参数错误，" + msg;
                    return viewModel.ResponseToJson();
                }
                //安全校验
                var baseResponse = _verifyService.Verify(request.SecCode, Request.GetQueryNameValuePairs());
                if (baseResponse.ErrCode != 1)
                {//校验失败，返回错误码
                    viewModel.BusinessStatus = baseResponse.ErrCode;
                    viewModel.StatusMessage = baseResponse.ErrMsg;
                    return viewModel.ResponseToJson();
                }
                int totalCount;
                var appoinmentInfoListData = _appoinmentService.GetOrderList(request, out totalCount); //.Select(x => new { Source = x.Source, OrderStaus = x.OrderStaus, BuId = x.BuId, Id = x.OrderId, LicenseNo = x.LicenseNo, MoldName = x.MoldName, SourceName = x.SourceName, BusinessRisks_StartTime = x.BusinessRisks_StartTime.HasValue ? (x.BusinessRisks_StartTime.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "--" : x.BusinessRisks_StartTime.Value.ToString("yyyy-MM-dd")) : "--", ForceRisks_StartTime = x.ForceRisks_StartTime.HasValue ? (x.ForceRisks_StartTime.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "--" : x.ForceRisks_StartTime.Value.ToString("yyyy-MM-dd")) : "--", AgentName = x.AgentName, OrderTime = x.OrderTime.ToString() });

                //appoinmentInfoListData.ToList().ForEach(arg => arg.Source = ConvertSourceToPow(arg.Source));
                appoinmentInfoListData.ToList().ForEach(arg =>
                    {
                        if (arg.Source == 0 || !arg.Source.HasValue)
                        {
                            arg.Source = 2;
                        }
                        else if (arg.Source > 1 && arg.Source != 9999)
                        {
                            arg.Source = Convert.ToInt64(Math.Pow(2, arg.Source.Value));
                        }
                    }
                );
                viewModel.AppointmentOrderResponse = appoinmentInfoListData;
                viewModel.BusinessStatus = 1;
                viewModel.TotalCount = totalCount;
                viewModel.StatusMessage = "查询成功！";
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 预约单列表 -- 查看详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAppoinmentInfo([FromUri]GetAppoinmentInfoListRequest request)
        {
            _logInfo.InfoFormat("获取预约单详情请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson());
            AppoinmentInfoViewModel viewModel = new AppoinmentInfoViewModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    viewModel.BusinessStatus = -10000;
                    string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    viewModel.StatusMessage = "输入参数错误，" + msg;
                    return viewModel.ResponseToJson();
                }
                //安全校验
                var baseResponse = _verifyService.Verify(request.SecCode, Request.GetQueryNameValuePairs());
                if (baseResponse.ErrCode != 1)
                {//校验失败，返回错误码
                    viewModel.BusinessStatus = baseResponse.ErrCode;
                    viewModel.StatusMessage = baseResponse.ErrMsg;
                    return viewModel.ResponseToJson();
                }
                viewModel.AppoinmentInfo = _appoinmentService.GetCarOrderByOrderId(request.OrderId);
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功！";
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 更新预约单信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UpdateAppoinmentInfo([FromBody] AppoinmentInfoRequest request)
        {
            AppoinmentInfoUpdateViewModel viewModel = new AppoinmentInfoUpdateViewModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    viewModel.BusinessStatus = -10000;
                    string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    viewModel.StatusMessage = "输入参数错误，" + msg;
                    return viewModel.ResponseToJson();
                }
                //安全校验
                var baseResponse = _verifyService.Verify(request.SecCode, Request.GetQueryNameValuePairs());
                if (baseResponse.ErrCode != 1)
                {//校验失败，返回错误码
                    viewModel.BusinessStatus = baseResponse.ErrCode;
                    viewModel.StatusMessage = baseResponse.ErrMsg;
                    return viewModel.ResponseToJson();
                }
                if (_appoinmentService.UpdateAppoinmentInfo(request))
                {
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "更新成功！";
                }
                else
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "更新失败！";
                }
            }
            catch (Exception ex)
            {
                _logError.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return viewModel.ResponseToJson();
        }

        //private int ConvertSourceToPow(int? source)
        //{
        //    if (source == 0 || !source.HasValue)
        //    {
        //        source = 2;
        //    }
        //    else if (source > 1 && source != 9999)
        //    {
        //        source = Convert.ToInt32(Math.Pow(2, source.Value));
        //    }
        //    return source.Value;
        //}
    }
}
