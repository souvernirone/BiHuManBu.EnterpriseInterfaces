using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ServiceStack.Text;

using ServiceStack.Text;


namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 客户业务员列表功能模块
    /// </summary>
    public class CustomerBusinessController : ApiController
    {
        private ICustomerBusinessService _customerbusinessService;
        private IEnterpriseAgentService _enterpriseAgentService;
        private ICustomerTopLevelService _customerTopLevelService;
        private IQuoteReqCarInfoService _quoteReqCarInfoService;
        private IVerifyService _verifyService;
        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        private readonly ILog _logError = LogManager.GetLogger("ERROR");
        private IConsumerReviewService _consumerReviewService;
        private IAgentService _agentService;
        private IAgentSpecialRateService _agentSpecialRateService;
        private IManagerRoleService _managerRoleService;
        private readonly IAuthorityService _authorityService;
        private readonly ILoginService _loginService;
        public CustomerBusinessController(
            ICustomerBusinessService customerbusinessService
            , IEnterpriseAgentService enterpriseAgentService
            , ICustomerTopLevelService customerTopLevelService
            , IVerifyService verifyService
            , IQuoteReqCarInfoService quoteReqCarInfoService
            , IConsumerReviewService consumerReviewService
            , IAgentService agentService
            , IAgentSpecialRateService agentSpecialRateService
            , IManagerRoleService managerRoleService
            , IAuthorityService authorityService,
            ILoginService loginService
            )
        {
            _customerbusinessService = customerbusinessService;
            _enterpriseAgentService = enterpriseAgentService;
            _customerTopLevelService = customerTopLevelService;
            _verifyService = verifyService;
            _quoteReqCarInfoService = quoteReqCarInfoService;
            _consumerReviewService = consumerReviewService;
            _agentService = agentService;
            _agentSpecialRateService = agentSpecialRateService;
            _managerRoleService = managerRoleService;
            _authorityService = authorityService;
            _loginService = loginService;
        }
        /// <summary>
        /// 获取顶级客户下所有业务员列表信息
        /// 在分配时获取可分配代理人
        /// </summary>
        /// <param name="request">当前用户的ID</param>
        /// <returns></returns>
        [HttpGet]
        [ActionTrace]
        public HttpResponseMessage GetBusinessAgentList([FromUri]GetBusinessAgentListRequest request)
        {
            var viewModel = new CTopLevelAgentListViewModel();

            //参数验证
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
            if (request.IsHas.HasValue)
            {
                baseRequest.IsHas = request.IsHas.Value;
            }
            //校验返回值
            var baseResponse = _verifyService.Verify(baseRequest, Request.GetQueryNameValuePairs());
            if (baseResponse.ErrCode != 1)
            {//校验失败，返回错误码
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel.ResponseToJson();
            }

            var agent = _authorityService.IsSystemAdminOrAdmin(request.ChildAgent) ? request.Agent : request.ChildAgent;
            int isHas = request.IsHas.HasValue ? request.IsHas.Value : 0;
            viewModel = _customerbusinessService.GetBusinessAgentList(agent, isHas);

            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获得业务员通知标签
        /// </summary>
        /// <param name="ChildAgent">当前代理人</param>
        /// <returns></returns>
        [HttpGet]
        [ActionTrace]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetDistributedTimeTag(int ChildAgent)
        {
            //参数验证
            if (!ModelState.IsValid)
            {
                DistributeTimeTagViewModel viewModel = new DistributeTimeTagViewModel();
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            return _customerbusinessService.GetDistributedTimeTag(ChildAgent).ResponseToJson();           
        }
        /// <summary>
        /// 顶级客户将车辆分配给业务员接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, CustomizedRequestAuthorize]
        public async Task<HttpResponseMessage> GetExcuteDistribute([FromBody]ExcuteDistributeRequest request)
        {
            var viewModel = new CBusinessDistributeViewModel();

            // 默认初始化该接口的值为 未分配
            request.IsDistributed = new List<int>() { 1 };

            // 分配前检查参数
            var checkResult = _customerTopLevelService.DistributeCheck(request);
            if (checkResult.Status != 1)
            {
                viewModel.BusinessStatus = -10009;
                viewModel.StatusMessage = checkResult.Msg;
                return viewModel.ResponseToJson();
            }

            //调用分配操作
            var response = await _customerTopLevelService.UpdateGroupDistributeAsync(request.AgentIds, 2, checkResult.Data,
                request.AverageCount, request.Agent, 1, request.ChildAgent);
            viewModel.BusinessStatus = response.BusinessStatus;
            viewModel.StatusMessage = response.StatusMessage;
            viewModel.isSucess = response.BusinessStatus == 1;

            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 登录时获取分配通知
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDistributeForAgent([FromUri] GetDistributeForAgentRequest request)
        {
            var viewModel = new DistributedDataVm();

            viewModel = _customerbusinessService.GetDistributeForAgent(request);
            if (viewModel.BuidsString == null)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "无数据";
            }
            else
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "获取成功";
            }

            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 分配回收接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns>是否回收成功</returns>
        [HttpPost, CustomizedRequestAuthorize]
        public async Task<HttpResponseMessage> UpdateDistributeRecycle([FromBody] ExcuteDistributeRequest request)
        {
            var viewModel = new CBusinessDistributeViewModel();

            var buids = request.IsAll == 1
                ? _customerTopLevelService.GetBuidsList(request)
                : request.Buids;
            if (!buids.Any())
            {
                viewModel.BusinessStatus = -10009;
                viewModel.StatusMessage = "没有可回收数据";
                return viewModel.ResponseToJson();
            }
            var response = await _customerbusinessService.UpdateDistributeRecycle(buids, request.Agent, request.ChildAgent);
            if (response)
            {
                viewModel.isSucess = true;
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "回收数据成功";
            }
            else
            {
                viewModel.isSucess = false;
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "回收数据失败！";
            }

            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 更新分配消息为已读
        /// </summary>
        /// <param name="messageIds">分配的批次Id在获取分配通知的时候得到</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateBxMessage(string messageIds)
        {
            var bsModel = new BaseViewModel();
            try
            {
                int result = _customerbusinessService.UpdateBxMessage(messageIds);
                if (result > 0)
                {
                    bsModel.BusinessStatus = 1;
                    bsModel.StatusMessage = "更新成功";
                }
            }
            catch (Exception ex)
            {
                bsModel.BusinessStatus = -10003;
                bsModel.StatusMessage = "服务器异常";
                _logError.Error("更新分配通知:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bsModel.ResponseToJson();
        }

        /// <summary>
        /// 获取标签下客户数据的数量，这个只有在点击客户列表菜单时才会调用
        /// 陈亮  /pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify, CustomizedRequestAuthorize]
        public HttpResponseMessage GetTopLabelCount([FromBody]GetCustomerListRequest request)
        {
            LabelCountViewModel result = _customerbusinessService.GetLabelCount(request);

            return result.ResponseToJson();

        }

        //    /// <summary>
        ///// 新的获取标签下客户数据的数量
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost, ModelVerify, Log("执行客户列表-业务员普通查询数量")]
        //public HttpResponseMessage GetLabelCountNew([FromBody]GetCustomerListRequest request)
        //{
        //    try
        //    {
        //        var lastCount = _customerbusinessService.GetLabelCountNew(request);
        //        var viewModelReult = new
        //        {
        //            BusinessStatus = 1,
        //            LastCount = lastCount
        //        };
        //        return viewModelReult.ResponseToJson();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logError.Error("CustomerBusinessController=>GetLabelCount发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
        //        var viewModelReult = new
        //        {
        //            BusinessStatus = 1,
        //            LastCount = 0
        //        };
        //        return viewModelReult.ResponseToJson();
        //    }
        //}

        /// <summary>
        /// 获取今日计划回访下面的子标签的数量
        /// 陈亮 17-08-04 /pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify, CustomizedRequestAuthorize]
        public async Task<HttpResponseMessage> GetTodayReviewCount([FromBody]GetTodayReviewCountRequest request)
        {
            return (await _customerbusinessService.GetTodayReviewCountAsync(request)).ResponseToJson();
        }

        /// <summary>
        /// 业务员列表批量审核 zky 2017-08-31 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify, Log("业务员列表批量审核")]
        public HttpResponseMessage AgentBatchAudit([FromBody]AgentBatchAuditRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();

            if (request.ToPostSecCode() != request.SecCode)
            {
                viewModel.BusinessStatus = -10004;
                viewModel.StatusMessage = "校验失败";
                return viewModel.ResponseToJson();
            }

            if (_customerbusinessService.AgentBatchAudit(request))
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "审核成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "审核失败";
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 更新业务员信息 zky  2017-11-20 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify, Log("修改业务员信息")]
        public HttpResponseMessage UpdateCustomerInfo([FromBody]UpdateCustomerRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            viewModel = _customerbusinessService.UpdateCustomerInfo(request);
            return viewModel.ResponseToJson();
        }


        /// <summary>
        /// 新增理赔人员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify, Log("新增理赔人员")]
        public HttpResponseMessage AddPayPeople([FromBody]SavePayPeopleRequest request)
        {
            var viewModel = new GetLoginViewModel();
            bx_agent registedAgent;
            viewModel = _loginService.Register(request.Account, request.Pwd.GetMd5(), request.Mobile, request.Name, 0, "0", 0, request.ParentShareCode, false, 0, null, "", false, 0, 0, 0, 0, 0, null, null, 0, 0, 0, out registedAgent, 0, null, 0, 0, peopleType:1);
            if (viewModel != null)
            {
                return viewModel.ResponseToJson();
            }


            //viewModel = _customerbusinessService.UpdateCustomerInfo(request);
            return viewModel.ResponseToJson();
        }





        /// <summary>
        /// 获取业务员列表 zky  2017-12-08 /crm
        /// </summary>
        /// <returns></returns>
        [HttpPost, ModelVerify, Log("获取业务员列表")]
        //[CustomizedRequestAuthorize]
        public HttpResponseMessage GetCustomerList([FromBody]CustomerRequest request)
        {
            CusListViewModel viewModel = new CusListViewModel();
            viewModel = _agentService.GetCustomerList(request);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 获取业务员列表总条数 zky  2017-12-26 /crm
        /// </summary>
        /// <returns></returns>
        [HttpPost, ModelVerify, Log("获取业务员列表总条数")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetCustomerListCount([FromBody]CustomerRequest request)
        {
            CusListViewModel viewModel = new CusListViewModel();
            viewModel = _agentService.GetCustomerListCount(request);
            return viewModel.ResponseToJson();
        }


        /// <summary>
        /// 获取代理人级别  zky 2017-12-11 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet, Log("获取代理人级别")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetAgentLevel(int agentId)
        {
            GetAgentViewModel viewModel = new GetAgentViewModel();
            viewModel = _customerbusinessService.GetAgentLevel(agentId);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 获取代理人费率列表  zky 2017-12-11 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("获取代理人费率列表"), ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetAgentSpecialRate([FromBody]GetAgentRateRequest request)
        {
            GetAgentRateViewModel viewModel = new GetAgentRateViewModel();
            viewModel.RateList = _agentSpecialRateService.GetAgentSpecialRate(request.AgentId, request.CompanyId, request.IsQuDao, request.QuDaoId);
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 获取代理人费率对象  zky 2017-12-11 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("获取代理人费率对象"), ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetAgentRate([FromBody]GetAgentRateRequest request)
        {
            GetAgentRateViewModel viewModel = new GetAgentRateViewModel();
            viewModel.AgentRate = _agentSpecialRateService.GetAgentRate(request.AgentId, request.CompanyId, request.IsQuDao, request.QuDaoId);
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 删除代理人检查代理人状态  zky 2017-12-11 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet, Log("删除代理人检查代理人状态")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage CheckAgentStatus(int agentId)
        {
            CheckAgentStatusViewModel viewModel = new CheckAgentStatusViewModel();
            viewModel = _customerbusinessService.CheckAgentStatus(agentId);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        ///更新代理人费率 zky 2017-12-11 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("更新代理人费率"), ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage EditAgentRate([FromBody]EditAgentRateRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            List<NameValue> tableJson = new List<NameValue>();
            if (!string.IsNullOrWhiteSpace(request.TableJson))
            {
                tableJson = JsonHelper.DeSerialize<List<NameValue>>(request.TableJson);
            }

            viewModel = _customerbusinessService.EditAgentRate(request.AgentId, request.CompanyId, request.AgentRate, request.Three, request.Four, request.CreateUserId, request.CreateUserName, tableJson, request.IsQuDao, request.QuDaoId);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        ///根据代理人分享码查询代理人名称 zky 2017-12-13 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("根据代理人名称查询代理人分享码"), ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetShareCodeByAgentName([FromBody]QueryAgentRequest request)
        {
            QueryAgentViewModel viewModel = new QueryAgentViewModel();
            viewModel = _customerbusinessService.GetShareCodeByAgentName(request.AgentName, request.TopAgentId);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 根据代理人名称查询代理人分享码 zky 2017-12-13 /crm
        /// </summary>
        /// <returns></returns>
        [HttpPost, Log("根据代理人分享码查询代理人名称"), ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetAgentNameByShareCode([FromBody]QueryAgentRequest request)
        {
            QueryAgentViewModel viewModel = new QueryAgentViewModel();
            viewModel = _customerbusinessService.GetAgentNameByShareCode(request.ShareCode, request.TopAgentId);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 是否可以修改上下级业务员关系 zky 2017-12-13 /crm
        /// </summary>
        /// <returns></returns>
        [HttpPost, Log("是否可以修改上下级业务员关系"), ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage IsCanEditParentAgent([FromBody]IsCanEditParentAgentRequest request)
        {
            CheckAgentStatusViewModel viewModel = new CheckAgentStatusViewModel();
            viewModel = _customerbusinessService.IsCanEditParentAgent(request);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 振邦账号添加用户 zky 2017-12-26 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("振邦机构添加用户"), ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage ZhengBangAddUser([FromBody]AddZhenBangUserRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            viewModel = _customerbusinessService.ZhengBangAddUser(request);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 修改代理人账号使用状态 zky 2018-02-04 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage EidtAgentIsUsed([FromBody]EditAgentIsUsedRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            viewModel = _customerbusinessService.EidtAgentIsUsed(request.AgentId, request.IsUsed);
            return viewModel.ResponseToJson();
        }

        [HttpPost]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage CanEditParentShareCode([FromBody]IsCanEditParentAgentRequest request)
        {
            return _customerbusinessService.CanEditParentShareCode(request).ResponseToJson();
        }

        #region 批量刷新续保
        /// <summary>
        /// 1.判断是否存在未完成刷新续保（续保中、排队中）
        /// 2.判断是否达到60条/人/天
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("是否达到60条/人/天,是否存在未完成刷新续保（续保中、排队中）")]
        public HttpResponseMessage CheckRefreshRenewal([FromBody] CheckRefreshRenewalRequest request)
        {
            // LogHelper.Info("是否达到60条/人/天,是否存在未完成刷新续保（续保中、排队中）:" + Request.RequestUri + ",json参数：" + request.ToJson());
            return _customerbusinessService.CheckRefreshRenewal(request).ResponseToJson();
        }
        /// <summary>
        /// 1.批量刷新续保：添加
        /// 2.定时刷新前端续保状态;如果是客户详情刷新状态,则需要传单个buid
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("批量刷新续保")]
        public HttpResponseMessage BatchRefreshRenewal([FromBody] BatchRefreshRenewalRequest request)
        {
            LogHelper.Info("批量刷新续保:" + Request.RequestUri + ",json参数：" + request.ToJson());
            return _customerbusinessService.BatchRefreshRenewal(request).ResponseToJson();
        }
        [HttpGet, Log("客户详情批量刷新续保")]
        public HttpResponseMessage BatchRefreshRenewalDetail([FromUri] BatchRefreshRenewalDetailRequest request)
        {
            // LogHelper.Info("客户详情批量刷新续保:" + Request.RequestUri + ",json参数：" + request.ToJson());
            return _customerbusinessService.BatchRefreshRenewalDetail(request).ResponseToJson();
        }
        /// <summary>
        /// 删除（批量刷新续保）排队中的数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, Log("删除（批量刷新续保）排队中的数据")]
        public HttpResponseMessage DeleteLineUpRenewwal([FromUri] LineUpRenewalRequest request)
        {
            //LogHelper.Info("删除（批量刷新续保）排队中的数据:" + Request.RequestUri + ",json参数：" + request.ToJson());
            return _customerbusinessService.DeleteLineUpRenewal(request).ResponseToJson();
        }
        #endregion
        [HttpGet]
        public HttpResponseMessage CustomerInfoes(string mobile,int agentId) {


            return _customerbusinessService.CustomerInfoes(mobile, agentId).ResponseToJson();
        }

        /// <summary>
        /// modify by qdk 2018-11-09 
        /// 批改车牌去重接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost,Log("批改车牌去重接口")]
        //[CustomizedRequestAuthorize]
        public HttpResponseMessage GetCorrectRepeatList([FromBody] CorrectRepeatRequest request)
        {
            LogHelper.Info("批改车牌去重接口:" + Request.RequestUri + "参数" + request.ToJson());
            return _customerbusinessService.GetCorrectRepeatList(request).ResponseToJson();      
        }
    }
}
