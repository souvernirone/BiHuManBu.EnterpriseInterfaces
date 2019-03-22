using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System.Net.Http;
using System.Web.Http;
using log4net;
using ServiceStack.Text;
using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// manageruser的增加修改
    /// </summary>
    public class ManagerUserController : ApiController
    {
        private readonly IManagerUserService _managerUserService;
        private readonly IManagerModuleService _managerModuleService;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
       
        public ManagerUserController(IManagerUserService managerUserService, IManagerModuleService managerModuleService)
        {
            _managerUserService = managerUserService;
            _managerModuleService = managerModuleService;
        }

        /// <summary>
        /// 更新经纪人平台账号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ModelVerify]
        public HttpResponseMessage UpdateManagerUserAccount(UpdateManagerUserAccountRequest request)
        {
            logInfo.Info(string.Format("crm修改用户信息请求串：{0}，请求参数：{1}", Request.RequestUri, request.ToJson()));
            return _managerUserService.UpdateManagerUserAccount(request).ResponseToJson();
        }

        [HttpGet, Log("获取菜单列表")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetModule(int platform)
        {
            ManagerModuleListViewModel viewModel = new ManagerModuleListViewModel();
            viewModel.ModuleList = _managerUserService.GetModuleList(platform);
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            return viewModel.ResponseToJson();
        }

        [HttpPost, Log("添加编辑菜单"), ModelVerify]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage AddEditSonMenu(AddEditModuleRequest request)
        {

            BaseViewModel viewModel = new BaseViewModel();
            if (request.saveType != "add" && request.saveType != "edit")
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "保存失败，未知的操作类型";
                return viewModel.ResponseToJson();
            }
            viewModel = _managerModuleService.AddEditModule(request);
            return viewModel.ResponseToJson();
        }

        [HttpGet, Log("删除菜单")]
        [CustomizedRequestAuthorize]
        public HttpResponseMessage DeleteModule(string moduleCode)
        {
            BaseViewModel viewModel = new BaseViewModel();
            if (string.IsNullOrWhiteSpace(moduleCode))
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "删除失败，module_code为空";
                return viewModel.ResponseToJson();
            }
            if (_managerModuleService.DeleteModule(moduleCode))
            {
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "删除成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "删除失败";
            }
            return viewModel.ResponseToJson();
        }
    }
}
