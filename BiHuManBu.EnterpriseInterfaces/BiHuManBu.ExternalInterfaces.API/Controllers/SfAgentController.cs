using System;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class SfAgentController : ApiController
    {
        private readonly ISfAgentService _sfAgentService;
        public SfAgentController(ISfAgentService sfAgentService)
        {
            this._sfAgentService = sfAgentService;
        }

        /// <summary>
        /// 分页获取深分账号列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="agentName"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSfAgentList(int pageIndex = 1, int pageSize = 10, string agentName = null)
        {
            var result = _sfAgentService.GetSfAgentListByPage(pageIndex, pageSize, agentName);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 获取单个账号详情
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSfAgentDetails(int agentId)
        {
            return _sfAgentService.GetSfAgentDetails(agentId).ResponseToJson();
        }

        /// <summary>
        /// 获取集团下所有车商
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCarDealers()
        {
            return _sfAgentService.GetCarDealers().ResponseToJson();
        }

        /// <summary>
        /// 添加/修改账号
        /// </summary>
        /// <param name="sfAgentRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddOrEditSfAgent([FromBody]SfAgentRequest sfAgentRequest)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "保存成功" };
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            }
            else
            {
                var passwordIsVaild = true;
                //是否修改密码（为空不修改）
                if (!string.IsNullOrEmpty(sfAgentRequest.AgentPassWord) || string.IsNullOrEmpty(sfAgentRequest.Id.ToString()))
                {
                    //验证密码是否合法
                    passwordIsVaild = System.Text.RegularExpressions.Regex.IsMatch(sfAgentRequest.AgentPassWord, @"^[\da-zA-Z]{6,20}$");
                }
                if (passwordIsVaild)
                {
                    var result = _sfAgentService.AddOrEditSfAgent(sfAgentRequest);
                    if (result == -1)
                    {
                        baseViewModel.BusinessStatus = 0;
                        baseViewModel.StatusMessage = "此账号已存在";
                    }
                }
                else
                {
                    baseViewModel.BusinessStatus = 0;
                    baseViewModel.StatusMessage = "密码为6~20位字母或数字组合";
                }
            }
            return baseViewModel.ResponseToJson();
        }

        /// <summary>
        /// 删除账号
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DeleteSfAgent([FromBody] int agentId)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "删除成功" };
            var count = _sfAgentService.DeleteSfAgent(agentId);
            if (count == 0)
            {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = "删除失败";
            }
            return baseViewModel.ResponseToJson();
        }
    }
}
