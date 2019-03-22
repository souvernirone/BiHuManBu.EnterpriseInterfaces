using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Mapper;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using System;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using ServiceStack.Text;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.AppIRepository;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 优惠活动
    /// </summary>
    public class PreferentialActivityController : ApiController
    {
        private readonly IPreferentialActivityService _preferentialActivityService;   //优惠活动接口
        private IPreferentialActivityRepository _iPreferentialActivityRepository;   //优惠活动接口
        private readonly IVerifyService _verifyService;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private readonly ILog logErro = LogManager.GetLogger("ERROR");


        public PreferentialActivityController
            (
            IVerifyService verifyService, IPreferentialActivityService preferentialActivityService, IPreferentialActivityRepository iPreferentialActivityRepository)
        {
            _verifyService = verifyService;
            _preferentialActivityService = preferentialActivityService;
            _iPreferentialActivityRepository = iPreferentialActivityRepository;
        }

        #region 获取活动分页列表  注释+姓名+日期+平台
        /// <summary>
        /// 获取活动分页列表 李金友 2017-09-06 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        private HttpResponseMessage GetPreferentialActivityPageList([FromUri]GetActivityPageListRequest request)
        {
            logInfo.Info(string.Format("获取顶级代理的客户列表请求串：{0}", Request.RequestUri));
            var viewModel = new ResponesActivityViewModel();

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

            var response = _preferentialActivityService.GetActivityPageList(request);
            viewModel.BusinessStatus = 1;
            if (response.Any())
            {
                viewModel.ActivitysList = response;
            }

            return viewModel.ResponseToJson();
        }
        #endregion

        #region 通过活动类别查询列表   注释+姓名+日期+平台
        /// <summary>
        /// 通过活动类别查询列表 李金友 2017-09-06 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetPreferentialActivityListByType([FromUri]BaseVerifyRequest request)
        {
            logInfo.Info(string.Format("获取顶级代理的客户列表请求串：{0}", Request.RequestUri));
            var viewModel = new ResponesActivityViewModel();
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
            var strUrl = string.Format("ChildAgent={0}&Agent={1}", request.ChildAgent, request.Agent);
            if (strUrl.GetUrl().GetMd5() != request.SecCode)
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                return viewModel.ResponseToJson();
            }
            var response = _preferentialActivityService.GetActivityByType(request);
            viewModel.BusinessStatus = 1;
            if (response.Any())
            {
                viewModel.TotalCount = response.Count();
                viewModel.ActivitysList = response;
            }
            return viewModel.ResponseToJson();
        }
        
        /// <summary>
        /// 通过当前代理人ID查询上次记录优惠活动信息 李金友 2017-11-15 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetPreferentialActivityByAgentId([FromUri]BaseVerifyRequest request)
        {
            logInfo.Info(string.Format("获取顶级代理的客户列表请求串：{0}", Request.RequestUri));
            var viewModel = new ResponesActivityViewModel();
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
            var strUrl = string.Format("ChildAgent={0}&Agent={1}", request.ChildAgent, request.Agent);
            if (strUrl.GetUrl().GetMd5() != request.SecCode)
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                return viewModel.ResponseToJson();
            }
            var response = _iPreferentialActivityRepository.GetActivityByAgentId(request);
            List<ResponesActivity> list = new List<ResponesActivity>();
            list.Add(response);
            viewModel.BusinessStatus = 1;
            if (list.Any())
            {
                viewModel.TotalCount = list.Count();
                viewModel.ActivitysList = list;
            }
            return viewModel.ResponseToJson();
        }
        #endregion

        #region 新增,编辑，删除优惠活动   注释+姓名+日期+平台
        /// <summary>
        /// 新增,编辑，删除优惠活动 李金友 2017-09-06 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public HttpResponseMessage AddPreferentialActivity([FromBody]RequestActivityViewModel model)
        {
            logInfo.Info("添加优惠活动接口:" + Request.RequestUri + "\n参数为：" + model.ToJson());
            var viewModel = new ResponesActivityViewModel();
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
                Agent = model.Agent,
                SecCode = model.SecCode,
                ChildAgent = model.ChildAgent
            };

            //var strUrl = string.Format("Activitys={0}&ModifyName={1}&ChildAgent={2}&Agent={3}", model.Activitys, model.ModifyName, model.ChildAgent, model.Agent);
            //if (strUrl.GetUrl().GetMd5() != model.SecCode)
            //{
            //    viewModel.BusinessStatus = -10000;
            //    viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
            //    return viewModel.ResponseToJson();
            //} 

            foreach (var item in model.Activitys)
            {
                if (item.Status != "del" && string.IsNullOrEmpty(item.ActivityContent))
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "活动内容不能为空";
                    return viewModel.ResponseToJson();
                }
            }
            

            try
            {
                var ResponseModel = _preferentialActivityService.AddOrEditActivity(model);
                viewModel.TotalCount = ResponseModel.Count;
                viewModel.ActivitysList = ResponseModel;
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "新增成功";

            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logErro.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return viewModel.ResponseToJson();
        }
        #endregion

        #region 新增优惠活动   注释+姓名+日期+平台
        /// <summary>
        /// 新增优惠活动  李金友 2017-09-09 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage AddActivity([FromUri]RequestAddActivityViewModel request)
        {
            logInfo.Info("添加优惠活动接口:" + Request.RequestUri + "\n参数为：" + request.ToJson());
            var viewModel = new ResponesAddActivity();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }

            try
            {
                bx_preferential_activity model = new bx_preferential_activity();
                model.top_agent_id = request.Agent;
                model.agent_id = request.ChildAgent;
                model.activity_type = 4;
                model.activity_name = "";
                model.activity_content = request.ActivityContent;
                model.activity_status = 1;
                model.create_time = DateTime.Now;
                model.create_name = request.CreateName;
                model.modify_time = DateTime.Now;
                model.modify_name = request.CreateName;

                var ResponseModel = _preferentialActivityService.AddActivity(model);
                if (ResponseModel.id > 0)
                {
                    viewModel.Id = ResponseModel.id;
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "新增成功";
                }
                else
                {
                    viewModel.Id = ResponseModel.id;
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "新增失败";
                }
                

            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logErro.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return viewModel.ResponseToJson();
        }
        #endregion

        #region 删除优惠活动   注释+姓名+日期+平台
        /// <summary>
        /// 删除优惠活动 李金友 2017-09-06 /PC
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage DelPreferentialActivity([FromUri]DelPreferentialActivityListRequest model)
        {
            logInfo.Info("添加优惠活动接口:" + Request.RequestUri + "\n参数为：" + model.ToJson());

            var viewModel = new ResponseAddActivityViewModel();
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
                Agent = model.Agent,
                SecCode = model.SecCode,
                ChildAgent = model.ChildAgent
            };

            //校验返回值
            var strUrl = string.Format("Ids={0}&ModifyName={1}&ChildAgent={2}&Agent={3}", model.Ids, model.ModifyName, model.ChildAgent, model.Agent);
            if (strUrl.GetUrl().GetMd5() != model.SecCode)
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                return viewModel.ResponseToJson();
            }

            try
            {
                BaseViewModel ResponseModel = _preferentialActivityService.DelActivity(model);
                return ResponseModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常";
                logErro.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return viewModel.ResponseToJson();
        }
        #endregion


    }
}
