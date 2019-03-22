using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Mapper;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AgentController : ApiController
    {
        private IAgentService _agentService;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        private IVerifyService _verifyService;
        private string keyCode = ConfigurationManager.AppSettings["keyCode"].ToString();
        private ITableHeaderService _tableHeaderService;

        public AgentController(IAgentService agentService, IVerifyService verifyService, ITableHeaderService tableHeaderService)
        {
            _agentService = agentService;
            _verifyService = verifyService;
            _tableHeaderService = tableHeaderService;
        }
        /// <summary>
        /// 获取经纪人标示
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAgentIdentity([FromUri]GetAgentIdentityAndRateRequest request)
        {
            var viewModel = new AgentIdentityAndRateViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            var response = _agentService.GetAgent(request, Request.GetQueryNameValuePairs());
            if (response.Status == HttpStatusCode.BadRequest || response.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
                return viewModel.ResponseToJson();
            }
            if (response.Status == HttpStatusCode.ExpectationFailed)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
            }
            else
            {
                viewModel = response.ConvertToViewModel();
                viewModel.BusinessStatus = 1;
            }
            return new HttpResponseMessage();
        }
        /// <summary>
        /// 修改经纪人信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditAgentSave([FromBody]GetAgentViewModel model)
        {
            GetAgentViewModel item = new GetAgentViewModel();
            try
            {
                model = model ?? new GetAgentViewModel();

                string CustKey = !string.IsNullOrEmpty(model.CustKey) ? model.CustKey : keyCode;
                string SecCode = model.SecCode;
                var strUrl = string.Format("AgentId={0}&CustKey={1}&KeyCode={2}&IsUsed={3}&RoleId={4}", model.AgentId, CustKey, keyCode, model.IsUsed, model.RoleId);
                if (model.IsUsed != 0 && model.IsUsed != 1 && model.IsUsed != 2)
                {
                    item.BusinessStatus = -10004;
                    item.StatusMessage = "参数错误:IsUsed只能是0，1，2";
                    return item.ResponseToJson();
                }
                if (strUrl.GetUrl().GetMd5() == SecCode)
                //if (!string.IsNullOrEmpty(SecCode))
                {
                    try
                    {
                        var result = _agentService.EditAgentAndManagerUserRoleId(model.AgentId, model.IsUsed.Value, model.RoleId);
                        if (result)
                        {
                            item.BusinessStatus = 1;
                            item.StatusMessage = "修改成功";
                        }
                        else
                        {
                            item.BusinessStatus = 0;
                            item.StatusMessage = "修改失败";
                        }
                    }
                    catch (Exception ex)
                    {
                        item.BusinessStatus = -10003;
                        item.StatusMessage = "服务发生异常";
                        logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    }
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
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item.ResponseToJson();
        }
        /// <summary>
        /// 查询经纪人列表信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage QueryAgentInfo([FromBody]GetAgentViewModel model)
        {
            GetAgentViewModel viewModel = new GetAgentViewModel();
            try
            {
                model = model ?? new GetAgentViewModel();
                string CustKey = !string.IsNullOrEmpty(model.CustKey) ? model.CustKey : keyCode;
                string SecCode = model.SecCode;
                var strUrl = string.Format("CustKey={0}&IsUsed={1}&KeyCode={2}&PageNum={3}&PageSize={4}&Search={5}&TopAgentId={6}",
                    model.CustKey, model.IsUsed, keyCode, model.PageNum, model.PageSize, model.Search, model.TopAgentId);
                if (model.AgentId > 0)
                {
                    strUrl += "&AgentId=" + model.AgentId;
                }
                if (strUrl.GetUrl().GetMd5() == SecCode)
                {
                    try
                    {
                        if (model.TopAgentId == model.AgentId)
                        {
                            model.AgentId = 0;
                        }

                        var totalNum = 0;
                        model.IsUsed = model.IsUsed == -1 ? null : model.IsUsed;
                        viewModel = _agentService.QueryAgentInfo(model.TopAgentId, model.AgentId, model.IsUsed, model.Search, model.PageSize, model.PageNum, out totalNum);
                        viewModel.TotalNum = totalNum;
                    }
                    catch (Exception ex)
                    {
                        viewModel.BusinessStatus = -10003;
                        viewModel.StatusMessage = "服务发生异常";
                        logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    }
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return viewModel.ResponseToJson();
        }

        [HttpPost]
        public HttpResponseMessage AddAgent([FromBody]GetAgentAddViewModel model)
        {
            model = model ?? new GetAgentAddViewModel();
            GetAgentAddViewModel viewModel = new GetAgentAddViewModel();

            try
            {

                string CustKey = !string.IsNullOrEmpty(model.CustKey) ? model.CustKey : keyCode;
                string SecCode = model.SecCode;
                var strUrl = string.Format("Id={0}&CustKey={1}&KeyCode={2}&ShareCode={3}", model.Id, CustKey, keyCode, model.ShareCode);
                if (strUrl.GetUrl().GetMd5() == SecCode)
                //if (!string.IsNullOrEmpty(SecCode))
                {
                    try
                    {
                        viewModel = _agentService.CopyAgentInfoAdd(model.Id, model.ShareCode);
                    }
                    catch (Exception ex)
                    {
                        viewModel.BusinessStatus = -10003;
                        viewModel.StatusMessage = "服务发生异常";
                        logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    }
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                }

            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return viewModel.ResponseToJson();
        }

        [HttpPost]
        public HttpResponseMessage GetAgentDistributedInfo([FromBody]AgentDistributedViewModel model)
        {
            AgentDistributedViewModel viewModel = new AgentDistributedViewModel();
            try
            {
                model = model ?? new AgentDistributedViewModel();
                string CustKey = !string.IsNullOrEmpty(model.CustKey) ? model.CustKey : keyCode;
                string SecCode = model.SecCode;
                var strUrl = string.Format("ParentAgentId={0}&CustKey={1}&KeyCode={2}", model.ParentAgentId, CustKey, keyCode);
                if (strUrl.GetUrl().GetMd5() == SecCode)
                {
                    viewModel = _agentService.GetAgentDistributedInfo(model.ParentAgentId.HasValue ? model.ParentAgentId.Value : -1);
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                }
                return viewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new HttpResponseMessage();
        }

        #region 集团报表初始化业务，目前不用了
        //[HttpGet]
        //public HttpResponseMessage UpdateAgentReport(string topAgentId, DateTime? startTime, DateTime? endTime)
        //{
        //    BaseViewModel viewModel = new BaseViewModel();
        //    try
        //    {
        //        if (startTime.HasValue && endTime.HasValue)
        //        {
        //            var result = _agentService.UpdateAgenrReport(topAgentId, startTime.Value, endTime.Value);
        //            if (result == 1)
        //            {
        //                viewModel.BusinessStatus = 1;
        //                viewModel.StatusMessage = "更新成功";
        //            }
        //            else
        //            {
        //                viewModel.BusinessStatus = 0;
        //                viewModel.StatusMessage = "更新失败";
        //            }
        //        }
        //        else
        //        {
        //            viewModel.BusinessStatus = 0;
        //            viewModel.StatusMessage = "时间参数不正确";
        //        }
        //        return viewModel.ResponseToJson();
        //    }
        //    catch (Exception ex)
        //    {
        //        viewModel.BusinessStatus = -10003;
        //        viewModel.StatusMessage = "服务发生异常";
        //        logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
        //    }
        //    return viewModel.ResponseToJson();
        //} 
        #endregion

        /// <summary>
        /// 校验登陆token 光鹏洁 2017-?-? /APP
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetToken([FromBody]GetAgentTokenViewModel model)
        {
            logInfo.Info("获取验证Token请求串：" + Request.RequestUri + "，请求参数：" + model.ToJson());
            GetAgentTokenViewModel viewModel = new GetAgentTokenViewModel();
            try
            {
                model = model ?? new GetAgentTokenViewModel();
                string CustKey = !string.IsNullOrEmpty(model.CustKey) ? model.CustKey : keyCode;
                string SecCode = model.SecCode;
                var strUrl = string.Format("AgentId={0}&CustKey={1}&KeyCode={2}", model.AgentId, CustKey, keyCode);
                if (strUrl.GetUrl().GetMd5() == SecCode)
                {
                    try
                    {
                        var token = _agentService.GetToken(model.AgentId.Value, CustKey);
                        if (string.IsNullOrEmpty(token))
                        {
                            viewModel.BusinessStatus = 0;
                            viewModel.StatusMessage = "未读取到token值";
                            return viewModel.ResponseToJson();
                        }
                        viewModel.BusinessStatus = 1;
                        string[] result = token.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        viewModel.token = result[0];
                        viewModel.IsUsed = result[1];
                        viewModel.AgentId = model.AgentId;
                    }
                    catch (Exception ex)
                    {
                        viewModel.BusinessStatus = -10003;
                        viewModel.StatusMessage = "服务发生异常";
                        logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    }
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blurname"></param>
        /// <param name="agentid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAgentChildrenList(string blurname, int agentid)
        {
            GetAgentChildrenListViewMode response = new GetAgentChildrenListViewMode();
            try
            {
                response.BusinessStatus = 1;
                var model = _agentService.GetAgentChildrenList(blurname, agentid).ToList();
                response.Items = model.Select(x => new AgentViewModel
                {
                    Id = x.Id,
                    Agentname = x.AgentName,
                    ShareCode = x.ShareCode
                }).ToList();
            }
            catch (Exception ex)
            {

                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                response.BusinessStatus = -10003;
                response.StatusMessage = "服务发生异常";
            }
            return response.ResponseToJson();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddAgentInfo([FromBody]GetAgentAddViewModel model)
        {
            model = model ?? new GetAgentAddViewModel();
            GetAgentAddViewModel viewModel = new GetAgentAddViewModel();
            try
            {
                bxAgent agent = new bxAgent();
                var result = _agentService.AddAgentInfo(model.AgentName, model.source, out agent);
                if (result == 1)
                {
                    viewModel.item = agent;
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "创建成功";
                }
                else if (result == 2)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "已存在";
                }
                else
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "创建失败";
                }
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return viewModel.ResponseToJson();
        }

        [HttpPost, Log("删除代理人"), ModelVerify]
        public HttpResponseMessage DelAgentInfo(DeleteAgentRequest request)
        {
            BaseViewModel model = new BaseViewModel();
            if (request.SecCode != request.ToPostSecCode())
            {
                model.BusinessStatus = -10001;
                model.StatusMessage = "参数校验失败";
                return model.ResponseToJson();
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                var result = _agentService.DelAgentInfo(request);
                model.BusinessStatus = result;
                model.StatusMessage = result == 1 ? "删除成功" : "删除失败";               
                if (result == 3)
                {
                    model.StatusMessage = "顶级代理人已经删除，不用重复操作";
                }
                else if (result == 2)
                {
                    model.StatusMessage = "存在下级代理人，不能删除";
                }
                else if (result == 3)
                {
                    model.StatusMessage = "代理人有Ukey，不能删除";
                }

                logInfo.Info("Agent->DelAgentInfo（删除代理人），参数：agentId=" + request.AgentId.ToString());
            }
            catch (Exception ex)
            {
                model.BusinessStatus = 0;
                model.StatusMessage = "删除报错";
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            watch.Stop();
            logInfo.Info("代理人彻底删除时间：" + watch.ElapsedMilliseconds);
            return model.ResponseToJson();
        }

        /// <summary>
        /// 获取代理渠道列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAgentSource([FromUri]BaseVerifyRequest request)
        {
            logInfo.Info(string.Format("获取代理渠道列表请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson()));
            var viewModel = new NewAgentSourceViewModel();
            //var viewModel = new AgentSourceViewModel();
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
            //校验返回值
            var baseResponse = _verifyService.Verify(baseRequest, Request.GetQueryNameValuePairs());
            if (baseResponse.ErrCode != 1)
            {//校验失败，返回错误码
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel.ResponseToJson();
            }
            var response = _agentService.NewGetAgentSource(request, Request.GetQueryNameValuePairs(), Request.RequestUri);
            logInfo.Info(string.Format("获取代理渠道列表接口返回值：{0}", viewModel.ToJson()));
            viewModel.BusinessStatus = 1;
            viewModel.AgentCity = response.AgentCity;
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 通过核保之后，获取核保费率
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetHebaoRate([FromUri]GetHebaoRateRequest request)
        {
            logInfo.Info(string.Format("获取核保费率请求串：{0}，参数：{1}", Request.RequestUri, request.ToJson()));
            var viewModel = new HebaoRateViewModel();
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
            //校验返回值
            var baseResponse = _verifyService.Verify(baseRequest, Request.GetQueryNameValuePairs());
            if (baseResponse.ErrCode != 1)
            {//校验失败，返回错误码
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel.ResponseToJson();
            }
            var response = _agentService.GetHebaoRate(request, Request.GetQueryNameValuePairs());
            viewModel = response;
            viewModel.BusinessStatus = 1;
            logInfo.Info(string.Format("获取核保费率返回值：{0}", viewModel.ToJson()));
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 获取代理人通话有效时长
        /// </summary>
        /// <param name="agentId">代理人编号</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAgentEffectiveCallDuration(int agentId)
        {

            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "获取成功" };
            int effectiveCallDuration = 0;
            try
            {
                effectiveCallDuration = _agentService.GetAgentEffectiveCallDuration(agentId);
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error("获取代理人通话有效时长发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new { EffectiveCallDuration = effectiveCallDuration, baseViewModel.BusinessStatus, baseViewModel.StatusMessage }.ResponseToJson();
        }

        /// <summary>
        /// 更新代理人通话有效时长
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UpdateAgentEffectiveCallDuration([FromBody]UpdateAgentEffectiveCallDurationRequest request)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "更新成功" };

            try
            {
                if (!_agentService.UpdateAgentEffectiveCallDuration(request.AgentId, request.EffectiveCallDuration))
                {
                    baseViewModel.BusinessStatus = 0;
                    baseViewModel.StatusMessage = "更新失败";
                }
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error("获取代理人通话有效时长发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return baseViewModel.ResponseToJson();
        }

        /// <summary>
        /// 初始化父级代理人对应子代理人集合到redis
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage InitAgentGroupToRedis()
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "初始化成功" };

            try
            {
                _agentService.InitAgentGroupToRedis();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error("初始化代理人到redis发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return baseViewModel.ResponseToJson();
        }

        /// <summary>
        /// 获取当前代理人所有下级
        /// </summary>
        /// <param name="currentAgentId">当前代理人编号</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSonsListFromRedis(int currentAgentId)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "获取成功" };
            List<int> agentIds = new List<int>();
            try
            {
                agentIds = _agentService.GetSonsListFromRedis(currentAgentId);
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error("获取当前代理人所有下级发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new { AgentIds = agentIds, baseViewModel.BusinessStatus, baseViewModel.StatusMessage }.ResponseToJson();
        }
        /// <summary>
        /// 添加新的代理人到redis
        /// </summary>
        /// <param name="addOrDeleteAgentGroup">添加或删除模型</param>

        [HttpPost]
        public HttpResponseMessage AddAgentGroupToRedis([FromBody]AddOrDeleteAgentGroupViewModel addOrDeleteAgentGroup)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "添加成功" };

            try
            {
                _agentService.AddAgentGroupToRedis(addOrDeleteAgentGroup.CurrentAgentId, addOrDeleteAgentGroup.ParentAgentId, addOrDeleteAgentGroup.TopAgentId, addOrDeleteAgentGroup.AgentLevel);
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error("添加新的代理人到redis发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 从redis中删除此代理人
        /// </summary>
        /// <param name="addOrDeleteAgentGroup">添加或删除模型</param>
        [HttpPost]
        public HttpResponseMessage DeleteAgentGroupFromRedis(AddOrDeleteAgentGroupViewModel addOrDeleteAgentGroup)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "删除成功" };

            try
            {
                _agentService.DeleteAgentGroupFromRedis(addOrDeleteAgentGroup.CurrentAgentId, addOrDeleteAgentGroup.ParentAgentId, addOrDeleteAgentGroup.TopAgentId, addOrDeleteAgentGroup.AgentLevel);
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error("从redis中删除此代理人发生异常: " + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return baseViewModel.ResponseToJson();
        }

        /// <summary>
        /// 更新代理人
        /// </summary>
        /// <param name="updateAgentGroupModel">更新模型</param>
        [HttpPost]
        public HttpResponseMessage UpdateAgentGroupInRedis(UpdateAgentGroupViewModel updateAgentGroupModel)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "更新成功" };

            try
            {
                _agentService.UpdateAgentGroupInRedis(updateAgentGroupModel.CurrentAgentId, updateAgentGroupModel.FromParentAgentIdKeys, updateAgentGroupModel.ToAgentIdKey, updateAgentGroupModel.OperationType);
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error("更新代理人发生异常: " + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return baseViewModel.ResponseToJson();
        }

        /// <summary>
        /// 查询代理人手机号
        /// </summary>
        /// <param name="agentId">代理人Id</param>
        /// <returns>Mobile</returns>
        [HttpPost]
        public HttpResponseMessage GetAgentMobile(int agentId)
        {
            QueryAgentViewModel item = new QueryAgentViewModel();
            try
            {
                var agent = _agentService.GetAgent(agentId);
                if (agent != null)
                {
                    item.Mobile = _agentService.GetAgent(agentId).Mobile;
                    item.BusinessStatus = 1;
                    item.StatusMessage = "查询成功";
                }
                else
                {
                    item.BusinessStatus = -1;
                    item.StatusMessage = "代理人不存在";
                }
                return item.ResponseToJson();
            }
            catch (Exception ex)
            {
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务器异常";
                logError.Error("根据Id查询代理人发生异常: " + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return item.ResponseToJson();
            }
        }

        /// <summary>
        /// 获取代理人的所有下级代理人
        /// 这个在客户列表搜索业务员时使用过
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSonAgents([FromUri]BaseVerifyRequest request)
        {
            if (request.ToPostSecCode() == request.SecCode)
            {
                return _agentService.GetSonAgents2(request.Agent, request.ChildAgent).ResponseToJson();
            }
            else
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamVerifyError).ResponseToJson();
            }
        }

        /// <summary>
        /// 更新代理人层级时通知redis接口
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <param name="parentAgentShareCode"></param>
        /// <param name="oldParentAgentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateAgentParentShareCodeNotifyRedis(int currentAgentId, int parentAgentShareCode, int oldParentAgentId)
        {
            try
            {
                _agentService.UpdateAgentParentShareCodeNotifyRedis(currentAgentId, parentAgentShareCode, oldParentAgentId);
                return BaseViewModel.GetBaseViewModel(Models.BusinessStatusType.OK).ResponseToJson();
            }
            catch (Exception ex)
            {
                logError.Error("更新代理人发生异常: " + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

                return BaseViewModel.GetBaseViewModel(Models.BusinessStatusType.SystemError).ResponseToJson();
            }
        }

        #region 设置代理手机号与微信同号  李金友 2017-09-08 /PC
        /// <summary>
        /// 设置代理手机号与微信同号  李金友 2017-09-08 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SetAgentPhoneIsWechat([FromUri]SetPhoneAndWechatAgentRequest request)
        {

            logInfo.Info("添加优惠活动接口:" + Request.RequestUri + "\n参数为：" + request.ToJson());
            request.ToPostSecCode();
            var viewModel = new BaseViewModel();
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

            var strUrl = string.Format("PhoneAndWechat={0}&ChildAgent={1}&Agent={2}", request.PhoneAndWechat, request.ChildAgent, request.Agent);
            if (strUrl.GetUrl().GetMd5() == request.SecCode)
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                return viewModel.ResponseToJson();
            }

            try
            {
                bool BL = _agentService.SetAgentsPhoneIsWechat(request);
                if (BL)
                {
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "设置成功";
                }
                else
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "设置失败";
                }
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return viewModel.ResponseToJson();
        }
        #endregion


        /// <summary>
        /// 获取source
        /// </summary>
        /// <param name="source">source值</param>
        /// <param name="sourceType"> 0:传递新source  1：传递老source</param>
        /// <returns></returns>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetSource(long source, int sourceType)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            //模型
            companyRelationModel companyRelationModel = new companyRelationModel();
            try
            {
                companyRelationModel = _agentService.GetSource(source, sourceType);
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error("获取source发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            //返回
            return new { companyRelationModel = companyRelationModel, baseViewModel.BusinessStatus, baseViewModel.StatusMessage }.ResponseToJson();
        }

        /// <summary>
        /// 获取代理人设置的表头信息
        /// </summary>
        /// <returns></returns>
        [CustomizedRequestAuthorize]
        [HttpGet, ModelVerify]
        public async Task<HttpResponseMessage> GetTableHeader([FromUri] GetTableHeaderRequest request)
        {
            return (await _tableHeaderService.GetTableHeaderAsync(request)).ResponseToJson();
        }

        /// <summary>
        /// 设置代理人设置的表头信息
        /// </summary>
        /// <returns></returns>
        [CustomizedRequestAuthorize]
        [HttpPost, ModelVerify]
        public async Task<HttpResponseMessage> SetTableHeader([FromBody]SetTableHeaderRequest request)
        {
            return (await _tableHeaderService.SetTableHeaderAsync(request)).ResponseToJson();
        }
        /// <summary>
        /// 获取深分人保集团下所有可用代理人
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAgentIdAndNameByGroupId(int groupId)
        {
            //groupId = Convert.ToInt32(ConfigurationManager.AppSettings["sfGroupId"]);
            return _agentService.GetAgentIdAndNameByGroupId(groupId.ToString()).Select(x => new { TopAgentId = x.Id, AgentName = x.AgentName }).ToList().ResponseToJson();

        }       

        #region 短信设置:微信同号、短信抬头展示门店名称、展示免责提示

        /// <summary>
        /// 获取短信设置
        /// </summary>
        /// <param name="agentId">经济人ID</param>
        /// <returns></returns>
        [CustomizedRequestAuthorize]
        [HttpGet]
        public ShortMsgSettingResponse GetShortMsgSetting(int agentId)
        {
            return _agentService.GetShortMsgSetting(agentId);
        }

        /// <summary>
        /// 设置短信设置
        /// </summary>
        /// <param name="request">短信设置数据模型</param>
        /// <returns></returns>
        [CustomizedRequestAuthorize]
        [HttpPost]
        public bool SetShortMsgSetting(ShortMsgSettingRequest request)
        {
            return _agentService.SetShortMsgSetting(request);
        }
        #endregion
    }
}