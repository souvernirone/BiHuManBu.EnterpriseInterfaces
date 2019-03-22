using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using ServiceStack.Text;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 顶级客户列表相关业务
    /// </summary>
    public class CustomerTopLevelController : ApiController
    {
        private readonly ICustomerTopLevelService _customerTopLevelService;
        private readonly IEnterpriseAgentService _enterpriseAgentService;
        private readonly IVerifyService _verifyService;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private readonly ILog logErro = LogManager.GetLogger("ERROR");
        private readonly IConsumerDetailService _consumerDetailService;
        private readonly IQuoteReqCarInfoService _quoteReqCarInfoService;
        private readonly IUserinfoRenewalInfoService _userinfoRenewalInfoService;
        private readonly ICustomerCategories _customercategoriesService;
        private readonly IAgentService _agentService;
        private readonly IConsumerReviewService _consumerReviewService;
        private readonly ICustomerStatusService _customerStatusService;

        public CustomerTopLevelController(
            ICustomerTopLevelService customerTopLevelService
            , IVerifyService verifyService
            , IEnterpriseAgentService enterpriseAgentService
            , IConsumerDetailService consumerDetailService
            , IQuoteReqCarInfoService quoteReqCarInfoService
            , IUserinfoRenewalInfoService userinfoRenewalInfoService
            , ICustomerCategories customercategoriesService
            , IAgentService agentService
            , IConsumerReviewService consumerReviewService
            , ICustomerStatusService customerStatusService
            )
        {
            _customerTopLevelService = customerTopLevelService;
            _enterpriseAgentService = enterpriseAgentService;
            _verifyService = verifyService;
            _consumerDetailService = consumerDetailService;
            _quoteReqCarInfoService = quoteReqCarInfoService;
            _userinfoRenewalInfoService = userinfoRenewalInfoService;
            _customercategoriesService = customercategoriesService;
            _agentService = agentService;
            _consumerReviewService = consumerReviewService;
            _customerStatusService = customerStatusService;
        }

        /// <summary>
        /// 获取每个标签下客户数量的接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, CustomizedRequestAuthorize]
        public async Task<HttpResponseMessage> GetCustomerCount([FromBody]GetCustomerCountRequest request)
        {
            var obj = await _customerTopLevelService.GetCustomerCountAsync(request);
            var viewModelReult = new
            {
                BusinessStatus = 1,
                TotalCount = obj.TotalCount,
                DistributedCount = obj.DistributedCount,
                NoDistributeCount = obj.TotalCount - obj.DistributedCount
            };
            return viewModelReult.ResponseToJson();
        }

        #region 客户列表数据与总条数分离
        /// <summary>
        /// 客户列表接口
        /// 陈亮 2017-12-4 /PC /WeiXin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, CustomizedRequestAuthorize]
        public async Task<HttpResponseMessage> GetCustomerListNew([FromBody]GetCustomerListRequest request)
        {
            var result = await _customerTopLevelService.GetCustomerListAsync(request);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 获取剩余条数
        /// 判断是否有下一页时使用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, CustomizedRequestAuthorize]
        public HttpResponseMessage GetCustomerCountNew([FromBody]GetCustomerListRequest request)
        {
            var lastCount = _customerTopLevelService.GetCustomerCountNew(request);
            var viewModelReult = new
            {
                BusinessStatus = 1,
                LastCount = lastCount
            };
            return viewModelReult.ResponseToJson();
        }

        #endregion

        /// <summary>
        /// 导出顶级代理的客户列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, CustomizedRequestAuthorize]
        public HttpResponseMessage GetExportCustomerList([FromBody]GetCustomerListRequest request)
        {

            var response = _customerTopLevelService.ExportCustomerList(request);
            return response.ResponseToJson();
        }

        /// <summary>
        /// 获取摄像头新进店数量，按X分钟轮循，调用频率很高
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, CustomizedRequestAuthorize]
        public HttpResponseMessage GetCountLoop([FromUri]GetCustomerListRequest request)
        {
            return _customerTopLevelService.GetCountLoop(request).ResponseToJson();
        }

        /// <summary>
        /// crm客户批量删除功能
        /// 回收站清空：一次将所有数据全部删除掉
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("客户数据批量删除"), CustomizedRequestAuthorize]
        public HttpResponseMessage MultipleDelete([FromBody]MultipleDeleteRequest request)
        {
            return _customerTopLevelService.MultipleDeleteUserInfo(request).ResponseToJson();
        }
        /// <summary>
        /// 根据当前页选中的数据进行删除,可选中1条,可选中多条
        /// 2018-06-28
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("回收站批量删除"), CustomizedRequestAuthorize]
        public HttpResponseMessage  BatchDeleteRecycle([FromBody] DeleteRepeatCustomerRequest request)
        {
            var result = _customerTopLevelService.BatchDeleteRecycle(request);
            return result.ResponseToJson();            
        }

        /// <summary>
        /// 回收站批量撤销
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("回收站批量撤销")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage BatchBackout([FromBody] DeleteRepeatCustomerRequest request)
        {
            var result = _customerTopLevelService.BatchBackout(request);
            return result.ResponseToJson();
        }
        /// <summary>
        /// 转移数据
        /// 陈亮 2017-10-17 /PC
        /// </summary>
        /// <returns></returns>
        [HttpPost, CustomizedRequestAuthorize]
        public async Task<HttpResponseMessage> TransferData([FromBody]ExcuteDistributeRequest request)
        {
            var result = await _customerTopLevelService.TransferDataAsync(request);
            return result.ResponseToJson();
        }

        #region 客户列表重复数据

        /// <summary>
        /// 获得客户列表重复数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("获得客户列表重复数据")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetCustomerRepeatList([FromBody]GetCustomerRepeatListRequest request)
        {
            BiHuManBu.ExternalInterfaces.Models.ViewModels.UserInfoRepeatViewModel viewModel = new Models.ViewModels.UserInfoRepeatViewModel();

            /*2018-09-04 张克亮 只有顶级和管理员才有权限获取重复数据修改为开放给所有角色使用*/
            //if (request.RoleType != 3 && request.RoleType != 4)
            //{
            //    viewModel.BusinessStatus = -10002;
            //    viewModel.StatusMessage = "当前代理人不是系统管理员或者管理员";
            //    return viewModel.ResponseToJson(); ;
            //}
            viewModel = _customerTopLevelService.GetCustomerRepeatList(request);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 删除客户列表重复数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("删除客户列表重复数据")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage MultipleDeleteRepeat([FromBody]DeleteRepeatCustomerRequest request)
        {
            var viewModel = _customerTopLevelService.MultipleDeleteRepeat(request.Buids, request.Agent, request.ChildAgent);
            return viewModel.ResponseToJson();
        }
        #endregion


        /// <summary>
        /// 已批改车牌存储到用户表中
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("已批改车牌存储到用户表中")]
        public HttpResponseMessage AddCorrectCarUserInfo([FromBody] CorrectCarRequest request)
        {
            return _customerTopLevelService.AddUserInfo(request.RecGuid, request.ChildAgent).ResponseToJson();
        }

    }
}
