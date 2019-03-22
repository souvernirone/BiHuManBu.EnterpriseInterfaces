using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System.Transactions;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class TempUserInfoController : ApiController
    {
        readonly ILog logError;
        readonly ILog logInfo;
        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        private readonly ILog _logError = LogManager.GetLogger("ERROR");
        readonly ITempUserService _tempUserService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public TempUserInfoController(ITempUserService tempUserService)
        {
            this._tempUserService = tempUserService;
            this.logError = LogManager.GetLogger("ERROR");
            this.logInfo = LogManager.GetLogger("INFO");
        }


        
        ///// <summary>
        ///// 获取临时车主信息
        ///// </summary>
        ///// <param name="agentId">bx_agent.id</param>
        ///// <param name="buId">bx_userinfo.Id</param>
        /////  <param name="TempUserType">TempUserType</param>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<HttpResponseMessage> GetTempUserInfoAsync(long buId = -1, int agentId = -1, bool? TempUserType = null)
        //{
        //    var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
        //    try
        //    {
        //        var tempUserInfoAsync = _tempUserService.GetTempUserInfoAsync(agentId, buId, TempUserType);
        //        logInfo.Info(string.Format("获取临时车主信息请求url为：{0}；请求参数为：{1}，{2}", Request.RequestUri.ToString(), agentId, buId));

        //        var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, tempUserInfo = await tempUserInfoAsync };
        //        return result.ResponseToJson();
        //    }
        //    catch (Exception ex)
        //    {
        //        baseViewModel.BusinessStatus = -10003;
        //        baseViewModel.StatusMessage = "服务器异常";
        //        logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
        //        return baseViewModel.ResponseToJson();
        //    }
        //}
        /// <summary>
        /// 获取临时关系人信息 
        /// </summary>
        /// <param name="agentId">代理人主键编号</param>
        /// <param name="buId">userinfo主键编号</param>
        /// <param name="TempUserType">临时关系人类型查询方式-默认null:查询全部,false:查询个人,true：查询公户</param>
        /// <param name="temptype">1：临时车主 2：临时保险人</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetTempRelationAsync(long buId = -1, int agentId = -1, bool? tempUserType = null, int tempType = 0)
        {

            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {

                var tempUserInfoAsync = _tempUserService.GetTempRelationAsync(agentId, buId, tempUserType, tempType);
                logInfo.Info(string.Format("获取临时关系人信息请求url为：{0}；请求参数为：{1}，{2}", Request.RequestUri.ToString(), agentId, buId));

                var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, tempUserInfo = tempUserInfoAsync };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException, Request.RequestUri.ToString(), agentId, buId, tempType));
                return baseViewModel.ResponseToJson();
            }
        }

        /// <summary>
        /// 老数据转新数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage OldDataChange()
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            //事物级别
            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            //引用事物
            //using (var ts = new TransactionScope(TransactionScopeOption.Required, option))
            //{
            try
            {
                //var tempUserInfoAsync = _tempUserService.DataChange();
                //var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, tempUserInfo = tempUserInfoAsync };
                //if (tempUserInfoAsync)
                //{
                //    ts.Complete();
                //    logInfo.Info(string.Format("数据转换-url为：{0}；", Request.RequestUri.ToString()));

                //}

                baseViewModel.BusinessStatus = 1;
                baseViewModel.StatusMessage = "接口已关闭!!";
                return baseViewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常 ";
                logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
            finally
            {
                //结束事物
                //ts.Dispose();
            }
            //}

        }
        //        /// <summary>
        //        /// 保存临时车主信息
        //        /// </summary>
        //        /// <param name="tempUserViewModel">临时车主信息模型</param>
        //        /// <param name="isEdit"></param>
        //        /// <returns></returns>
        //        [HttpPost]
        //        public async Task<HttpResponseMessage> SaveTempUserInfoAsync([FromBody]List<TempUserViewModel> tempUserViewModel, bool isEdit = false)
        //        {
        //            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "保存失败" };
        //            if (!ModelState.IsValid)
        //            {
        //                baseViewModel.BusinessStatus = -10000;
        //                baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
        //                return baseViewModel.ResponseToJson();
        //            }
        //            try
        //            {
        //                var isSuccess = _tempUserService.SaveTempUserInfoAsync(tempUserViewModel, isEdit);
        //                logInfo.Info(string.Format("保存临时车主信息请求url为：{0}；请求参数为：{1}，{2}", Request.RequestUri.ToString(), JsonHelper.Serialize(tempUserViewModel), isEdit
        //));
        //                if (await isSuccess)
        //                {
        //                    baseViewModel.BusinessStatus = 1;
        //                    baseViewModel.StatusMessage = "保存成功";
        //                }
        //                return baseViewModel.ResponseToJson();
        //            }
        //            catch (Exception ex)
        //            {
        //                baseViewModel.BusinessStatus = -10003;
        //                baseViewModel.StatusMessage = "服务器异常 ";
        //                logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
        //                return baseViewModel.ResponseToJson();
        //            }
        //        }



        /// <summary>
        /// 保存临时关系人信息
        /// </summary>
        /// <param name="tempUserViewModel">实体模型：TempUser：临时关系人类  RelationDetailInfo：中间表关系类</param>
        /// <param name="step">1：临时车主 2：临时被保险人</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveTempRelationAsync(TempUserViewModel tempRelationViewModel, int step = 1)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "保存失败" };
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                return baseViewModel.ResponseToJson();
            }
            try
            {

                var isSuccess = _tempUserService.SaveTempRelationAsync(tempRelationViewModel, step);
                logInfo.Info(string.Format("保存临时关系人信息请求url为：{0}；请求参数为：{1}，{2}", Request.RequestUri.ToString(), JsonHelper.Serialize(tempRelationViewModel), step
));
                if (isSuccess)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "保存成功";
                }
                return baseViewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常 ";
                logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException, Request.RequestUri.ToString(), JsonHelper.Serialize(tempRelationViewModel), step));
                return baseViewModel.ResponseToJson();
            }
        }



    }
}
