using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService;
using log4net;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class RenewalInfoController : ApiController
    {

        private readonly IRenewalInfoService _renewalInfoService;

        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        private readonly ILog _logError = LogManager.GetLogger("ERROR");
        private readonly IGetRenewalInfoService _getRenewalInfoService;



        public RenewalInfoController(IRenewalInfoService renewalInfoService, IConsumerDetailService consumerDetailService, ITempUserService tempUserService, IBatchRenewalService _batchRenewalService, ICustomerCategories _customerCategories, IAgentService _agentService, ICallAnalyticsService _callAnalyticsService, ICarModelRepository carModelRepository, IDefeatReasonHistoryService _defeatReasonHistoryService, IGetRenewalInfoService getRenewalInfoService)
        {
        
            _getRenewalInfoService = getRenewalInfoService;
     
            this._renewalInfoService = renewalInfoService;
            //this._cacheClient = new CacheClient(new RedisHashCache(13));
        }
        /// <summary>
        /// 获取续保详情
        /// </summary>
        /// <param name="buId">bx_userinfo.id</param>
        /// <param name="currentAgentId">bx_agent.Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetRenewalInfo(long buId, int currentAgentId, int topAgentId, int roleType)
        {
            var renewalRequest = new GetRenewalRequest()
            {
                Buid = buId,
                CurrentAgentId = currentAgentId,
                TopAgentId = topAgentId,
                RoleType = roleType
            };

            return _getRenewalInfoService.GetRenewalInfo(renewalRequest).ResponseToJson();
        }
        /// <summary>
        /// 保存客户信息
        /// </summary>
        /// <param name="customerInfo">CarInfo</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> SaveCustomerInfo([FromBody]CustomerInfo customerInfo)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "保存失败" };
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            }
            else
            {
                try
                {
                    var isSuccess = await _renewalInfoService.SaveCustomerInfoAsync(customerInfo);
                    if (isSuccess)
                    {
                        baseViewModel.BusinessStatus = 1;
                        baseViewModel.StatusMessage = "保存成功";

                    }
                    _logInfo.Info(string.Format(baseViewModel.StatusMessage + "请求url为：{0}，请求数据为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(customerInfo)));



                }
                catch (Exception ex)
                {
                    baseViewModel.BusinessStatus = -10003;
                    baseViewModel.StatusMessage = "服务器异常";
                    _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));

                }

            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 保存车辆信息
        /// </summary>
        /// <param name="carInfo">carInfo</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> SaveCarInfo([FromBody]CarInfo carInfo)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "保存失败" };
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            }
            else
            {
                try
                {
                    var isSuccess = await _renewalInfoService.SaveCarInfoAsync(carInfo);
                    if (isSuccess)
                    {
                        baseViewModel.BusinessStatus = 1;
                        if (carInfo.CarInfoId <= 0)
                        {
                            baseViewModel.BusinessStatus = 2;
                        }
                        baseViewModel.StatusMessage = "保存成功";

                    }
                    _logInfo.Info(string.Format(baseViewModel.StatusMessage + "请求url为：{0}，请求数据为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(carInfo)));
                }
                catch (Exception ex)
                {
                    baseViewModel.BusinessStatus = -10003;
                    baseViewModel.StatusMessage = "服务器异常";
                    _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                }
            }
            return baseViewModel.ResponseToJson();
        }



        /// <summary>
        /// 更新UserInfo主表接口 2017.07.26
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveUserInfo([FromBody]UserInfo userInfo)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "保存失败" };
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            }
            else
            {
                try
                {
                    //执行更新UserInfo接口
                    var isSuccess = _renewalInfoService.SaveUserInfo(userInfo);
                    if (isSuccess)
                    {
                        baseViewModel.BusinessStatus = 1;
                        baseViewModel.StatusMessage = "保存成功";

                    }
                    _logInfo.Info(string.Format(baseViewModel.StatusMessage + "请求url为：{0}，请求数据为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(userInfo)));
                }
                catch (Exception ex)
                {
                    baseViewModel.BusinessStatus = -10003;
                    baseViewModel.StatusMessage = "服务器异常";
                    _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                }
            }
            return baseViewModel.ResponseToJson();
        }

        public HttpResponseMessage SavePreRenewalInfo()
        {
            throw new Exception();
        }
        /// <summary>
        /// 录入回访信息
        /// </summary>
        /// <param name="consumerReview">ConsumerReview</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> SaveConsumerReview([FromBody]ConsumerReview consumerReview)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "保存失败" };
            int categoryId = 0;
            bool isDelete = false;
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            }
            else
            {
                var result = _renewalInfoService.SaveConsumerReviewAsync(consumerReview);
                categoryId = result.Item1;
                isDelete = result.Item2;
                baseViewModel.BusinessStatus = 1;
                baseViewModel.StatusMessage = "保存成功";


            }
            return new { baseViewModel.BusinessStatus, baseViewModel.StatusMessage, CategoryId = categoryId, IsDelete = isDelete }.ResponseToJson();
        }

        /// <summary>
        /// 更改精油码
        /// </summary>
        /// <param name="buId">bx_userinfo.id</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateReqCarInfoForAutoModelCode([FromUri]long buId)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "更改成功" };
            try
            {
                var isSuccess = _renewalInfoService.UpdateReqCarInfo(buId);
                if (!isSuccess)
                {
                    baseViewModel.BusinessStatus = 0;
                    baseViewModel.StatusMessage = "更新失败";
                }
                _logInfo.Info(string.Format("更改精油码请求url为：{0}，请求数据为：{1}", Request.RequestUri.ToString(), buId));
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error("更改精油码发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return baseViewModel.ResponseToJson();
        }
        [HttpGet]
        public HttpResponseMessage UpdateConsumerReviewBizEndDate() {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "更改成功" };
            _renewalInfoService.UpdateConsumerReviewBizEndDate();
            return baseViewModel.ResponseToJson();
        }

        public string GetSourceName(long newSource)
        {
            if (newSource == 2147483648)
            {
                return "恒邦车险";
            }
            if (newSource == 4294967296)
            {
                return "中铁车险";
            }
            if (newSource == 8589934592)
            {
                return "美亚车险";
            }
            if (newSource == 17179869184)
            {
                return "富邦车险";
            }
            if (newSource == 34359738368)
            {
                return "众诚车险";
            }
            return ToEnumDescription(newSource, typeof(EnumSourceNew));
        }

        public static String ToEnumDescription(long value, Type enumType)
        {
            NameValueCollection nvc = GetNvcFromEnumValue(enumType);
            return nvc[value.ToString()];
        }
        public static NameValueCollection GetNvcFromEnumValue(Type enumType)
        {
            var nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        var aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = "";
                    }
                    nvc.Add(strValue, strText);
                }
            }
            return nvc;
        }

        private DateTime GetBizEndTime(DateTime newBizEndTime, DateTime nextBizEndTime, DateTime sourceTime)
        {
            DateTime useBizEndTime = DateTime.Now;
            if (sourceTime >= nextBizEndTime.AddDays(-180))
            {
                useBizEndTime = nextBizEndTime;
            }
            else if (sourceTime >= newBizEndTime.AddDays(-180))
            {

                useBizEndTime = newBizEndTime;
            }
            else
            {
                useBizEndTime = newBizEndTime.AddYears(-1);
            }
            return useBizEndTime;
        }


    }
}