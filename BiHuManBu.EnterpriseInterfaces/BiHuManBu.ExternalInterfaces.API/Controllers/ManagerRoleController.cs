using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class ManagerRoleController : ApiController
    {
        private IManagerRoleService _managerRoleService;
        private IAgentService _agentService;
        private IManagerModuleService _moduleService;
        private IVerifyService _verifyService;
        private IAuthorityService _authorityService;

        private ILog logError = LogManager.GetLogger("ERROR");
        private string keyCode = ConfigurationManager.AppSettings["keyCode"].ToString();
        public ManagerRoleController(IManagerRoleService managerRoleService
            , IAgentService agentService
            , IManagerModuleService moduleService
            , IVerifyService verifyService
            , IAuthorityService authorityService
            )
        {
            _managerRoleService = managerRoleService;
            _agentService = agentService;
            _moduleService = moduleService;
            _verifyService = verifyService;
            _authorityService = authorityService;
        }

        /// <summary>
        /// 根据顶级经纪人 获取角色信息
        /// </summary>
        /// <param name="TopAgentId"></param>
        /// <param name="CustKey"></param>
        /// <param name="SecCode"></param>
        /// <returns></returns>
        [HttpGet]

        public HttpResponseMessage GetManagerRoleInfo(int TopAgentId, string CustKey, string SecCode)
        {
            ManagerRoleListViewModel item = new ManagerRoleListViewModel();
            try
            {
                CustKey = !string.IsNullOrEmpty(CustKey) ? CustKey : keyCode;
                var strUrl = string.Format("TopAgentId={0}&CustKey={1}&KeyCode={2}", TopAgentId, CustKey, keyCode);
                if (strUrl.GetUrl().GetMd5() == SecCode)
                {
                    item = _managerRoleService.GetManagerRoleInfo(TopAgentId);
                }
                else
                {
                    item.BusinessStatus = -10004;
                    item.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务发生异常";
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item.ResponseToJson();
        }

        /// <summary>
        /// 将非顶级代理人设置为顶级代理人
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage EditAgentChangeTopAgent(int agentId)
        {
            BaseViewModel item = new BaseViewModel();
            try
            {
                var result = _agentService.EditAgentChangeTopAgent(agentId);
                if (result == 1)
                {
                    item.BusinessStatus = 1;
                    item.StatusMessage = "修改成功";
                }
                else if (result == 0)
                {
                    item.BusinessStatus = 0;
                    item.StatusMessage = "修改失败";
                }
                else if (result == -1)
                {
                    item.BusinessStatus = -1;
                    item.StatusMessage = "没有机器人账号，不能升级代理";
                }
                else if (result == -2)
                {
                    item.BusinessStatus = -10003;
                    item.StatusMessage = "服务发生异常";
                }
            }
            catch (Exception ex)
            {
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务发生异常";
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item.ResponseToJson();
        }

        /// <summary>
        /// 根据顶级代理人Id获取角色列表 zky 2017-08-03 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetRoleList([FromUri]ManagerRoleRequest request)
        {
            ManagerRoleListViewModel item = new ManagerRoleListViewModel();
            try
            {
                var CustKey = !string.IsNullOrEmpty(request.CustKey) ? request.CustKey : keyCode;
                var url = string.Format("CustKey={0}&TopAgentId={1}", request.CustKey, request.TopAgentId);
                if (url.GetMd5() == request.SecCode)
                {
                    item = _managerRoleService.GetRoleList(request.TopAgentId);
                }
                else
                {
                    item.BusinessStatus = -10004;
                    item.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务发生异常";
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item.ResponseToJson();
        }

        /// <summary>
        /// 获取所有需要展示的菜单 zky 2017-08-03 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ModelVerify]
        public HttpResponseMessage GetDisplayModule([FromUri]ManagerRoleRequest request)
        {
            ManagerModuleListViewModel item = new ManagerModuleListViewModel();
            try
            {
                request.CustKey = !string.IsNullOrEmpty(request.CustKey) ? request.CustKey : keyCode;
                if (request.ToPostSecCode() == request.SecCode)
                {
                    item.ModuleList = _moduleService.LoadManagerModule(request.RoleId, request.RoleType);
                    item.BusinessStatus = 1;
                    item.StatusMessage = "查询成功";
                }
                else
                {
                    item.BusinessStatus = -10004;
                    item.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务发生异常";
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item.ResponseToJson();
        }

        /// <summary>
        /// 修改或添加角色 zky 2017-08-03 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ModelVerify]
        public HttpResponseMessage AddOrUpdateRole([FromBody]AddOrUpdateRoleRequest request)
        {
            request.CustKey = !string.IsNullOrEmpty(request.CustKey) ? request.CustKey : keyCode;
            BaseViewModel viewModel = new BaseViewModel();
            try
            {
                if (request.ToPostSecCode() != request.SecCode)
                {
                    viewModel.BusinessStatus = -10001;
                    viewModel.StatusMessage = "参数校验失败";
                    return viewModel.ResponseToJson();
                }
                var list = !string.IsNullOrEmpty(request.JsonString) ? JsonConvert.DeserializeObject<List<ManagerModuleViewModel>>(request.JsonString) : null;

                //请求主方法
                viewModel = _managerRoleService.AddOrUpdateRole(request.RoleId, request.RoleName, request.Agent, request.AgentName, request.IsRequote, list);
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务器异常";
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 删除角色
        /// 陈亮  2017-8-11  /pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public HttpResponseMessage DeleteRole(DeleteRoleRequest request)
        {
            var strUrl = string.Format("Agent={0}&RoleId={1}", request.Agent, request.RoleId);
            if (strUrl.GetUrl().GetMd5() != request.SecCode)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamVerifyError).ResponseToJson();
            }

            return _managerRoleService.DeleteRole(request).ResponseToJson();
        }

        /// <summary>
        /// 给顶级代理人添加助理角色 zky 2017-09-01 /crm
        /// </summary>
        /// <param name="SecCode"></param>
        /// <returns></returns>
        [HttpGet, Log("给顶级代理人添加助理角色")]
        public HttpResponseMessage AddHelperRole(string SecCode)
        {
            BaseViewModel viewModel = new BaseViewModel();
            if (SecCode != "EC8956637A99787BD197EACD77ACCZKY")
            {
                viewModel.BusinessStatus = -10004;
                viewModel.StatusMessage = "校验失败";
                return viewModel.ResponseToJson();
            }
            int roleSuccess = 0;
            int moduleSuccess = 0;
            _managerRoleService.OldAgentAddHepler(0, out roleSuccess, out moduleSuccess);
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = string.Format("添加角色{0}，添加菜单{1}", roleSuccess, moduleSuccess);

            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 判断不是系统角色，但是有分配权限
        /// 陈亮  2017-12-8  /pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, CustomizedRequestAuthorize]
        public HttpResponseMessage HasBtnDistribute([FromUri]BaseRequest2 request)
        {
            BaseViewModel vm = new BaseViewModel();
            //if (request.Agent == request.ChildAgent)
            //{
            //    vm.Data = new { HasDistribute = false };
            //}
            //else
            //{
            vm.Data = new { HasDistribute = _authorityService.IsHasDistributeAuth(request.ChildAgent) };
            //}
            return vm.ResponseToJson();
        }

        /// <summary>
        /// 查询有分配和批量续保权限的角色名称 zky 2017-12-8 /crm
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetDistributedRoleList(int topAgentId)
        {
            GetRoleListViewModel viewModel = new GetRoleListViewModel();
            var btnRoleList = _managerRoleService.GetRoleListByBtnCode("btn_recycle", topAgentId);//获取拥有 分配按钮的角色
            var moduleRoleList = _managerRoleService.GetRoleListByModule("batchRenewal_list", topAgentId);//获取拥有 批量续保的角色
            viewModel.RoleList = btnRoleList.Union(moduleRoleList).Distinct().ToList();
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 保存是否代报价
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage UpdateRequoteById([FromBody] UpdateRoleRequest request)
        {
            return _managerRoleService.UpdateRequoteById(request).ResponseToJson();
        }






    }
}
