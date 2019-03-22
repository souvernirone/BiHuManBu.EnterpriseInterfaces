using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Infrastructure;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class GroupController : ApiController
    {
        private readonly IAgentService _agentService;
        private readonly IGroupAuthenService _groupAuthenService;

        public GroupController(IAgentService agentService, IGroupAuthenService groupAuthenService)
        {
            _agentService = agentService;
            _groupAuthenService = groupAuthenService;
        }

        /// <summary>
        ///  获取集团账号下的机构列表 zky 2017-11-13 /crm、统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Log("集团账号的机构列表查询")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetOrgList([FromBody]OrgListRequest request)
        {
            OrgListViewModel viewModel = new OrgListViewModel();
            int total = 0;
            viewModel.OrgList = _agentService.GetOrgList(request.OrgName, request.AuthenState, request.GroupId, request.NeedPage, request.OrgId, request.PageIndex, request.PageSize, out total);
            viewModel.PageIndex = request.PageIndex;
            viewModel.PageSize = request.PageSize;
            viewModel.Total = total;
            viewModel.BusinessStatus = 200;

            return viewModel.ResponseToJson();
        }

        [HttpPost, Log("集团账号机构名称查询")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage AgentIdNameList([FromBody]OrgListRequest request)
        {
            OrgListViewModel viewModel = new OrgListViewModel();
            viewModel.IdNameList = _agentService.GetAgentIdNameList(request.GroupId);
            viewModel.BusinessStatus = 200;
            return viewModel.ResponseToJson();
        }

        [HttpGet, Log("查询集团账号认证信息")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetGroupAuthenModel(int Id)
        {
            GroupAuthenViewModel viewModel = new GroupAuthenViewModel();
            viewModel.GroupAuthen = _groupAuthenService.GetModel(Id) ?? new GroupAuthenModel();
            viewModel.BusinessStatus = 200;
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 根据经济人查询集团账号认证信息 2018-09-19 张克亮 做小V盟项目时加入
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet, Log("根据经济人查询集团账号认证信息")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetGroupAuthen(int agentId)
        {
            return _groupAuthenService.GetGroupAuthen(agentId).ResponseToJson();
        }

        [HttpPost, Log("更新集团账号认证信息"), ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage AddOrUpdateGroupAuthen([FromBody]UpdateGroupAuthenRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            if (request.Id > 0)//更新
            {
                viewModel = _groupAuthenService.UpdateModel(request);
            }
            else //添加
            {
                viewModel = _groupAuthenService.AddModel(request);
            }

            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 机构保存 内部员工、外部代理信息 zky 2017-12-28 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage ZhenBangUpdateUser([FromBody]ZBUpdateUserRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            viewModel = _groupAuthenService.ZhenBangUpdate(request.TopAgentId, request.AgentId, request.IsUsed, request.IsSubmit, request.IsShowCalc, request.MessagePayType, request.RoleId, request.ZhenBangType, "User",request.ParentShareCode);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 机构更新网点信息 zky 2017-12-28 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage ZhenBangUpdateSite([FromBody]ZBUpdateSiteRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            viewModel = _groupAuthenService.ZhenBangUpdate(request.TopAgentId, request.AgentId, request.IsUsed, request.IsSubmit, request.IsShowCalc, request.MessagePayType, request.RoleId, 0, "Site", request.ParentShareCode);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 机构、网点更新内部员工信息 zky 2017-12-28 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage ZhenBangUpdateMember([FromBody]ZBUpdateMemberRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            viewModel = _groupAuthenService.ZhenBangUpdate(request.TopAgentId, request.AgentId, request.IsUsed, request.IsSubmit, request.IsShowCalc, request.MessagePayType, 0, request.ZhenBangType, "Woker", request.ParentShareCode);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 查询所有的银行 zky 2018-1-2
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage BankList()
        {
            BankListViewModel viewModel = new BankListViewModel();
            viewModel.BankList = _groupAuthenService.GetAllBank();
            viewModel.StatusMessage = "查询成功";
            viewModel.BusinessStatus = 200;
            return viewModel.ResponseToJson();
        }

        /// <summary>
        ///  更新机构账号信息 zky 2018-1-2 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage SaveOrgInfo([FromBody]SaveOrgInfoRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            viewModel = _groupAuthenService.UpdateOrgInfo(request);
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 工号管理列表 zky 2018-1-2 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetJobNumberList([FromBody]JobNumberRequest request)
        {
            JobNumberListViewModel viewModel = new JobNumberListViewModel();
            int total = 0;
            viewModel.JobNumberList = _groupAuthenService.JobNumberList(request.OrgId, request.UkeyName, request.CityName, request.OrgName, request.PageIndex, request.PageSize, request.CityId, request.UkeyId, request.GroupId, out total);
            viewModel.PageIndex = request.PageIndex;
            viewModel.PageSize = request.PageSize;
            viewModel.TotalCount = total;
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 工号管理获取城市列表、ukey列表  zky 2018-1-3 /crm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage CityInfoAndUkeyInfo(int groupId)
        {
            JobNumberListViewModel viewModel = new JobNumberListViewModel();
            viewModel = _groupAuthenService.CityInfoAndUkeyInfo(groupId);

            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 认证不通过 zky 2018-02-04 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage AuditFailed([FromBody]AuditFailedRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            viewModel = _groupAuthenService.AuditFailed(request);
            return viewModel.ResponseToJson();
        }
    }
}
