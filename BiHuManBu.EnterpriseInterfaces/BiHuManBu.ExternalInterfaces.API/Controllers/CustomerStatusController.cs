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
using BiHuManBu.ExternalInterfaces.Models;


namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class CustomerStatusController : ApiController
    {
        readonly ILog logError;
        readonly ILog logInfo;
        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        private readonly ILog _logError = LogManager.GetLogger("ERROR");
        private readonly IConsumerDetailService _consumerDetailService;
        readonly ICustomerStatusService _customerStatusService;
        private readonly ICustomerTopLevelService _customerTopLevelService;

        //
        // GET: /CustomerCategories/

        public CustomerStatusController(ICustomerStatusService customerStatusService, IConsumerDetailService consumerDetailService, ICustomerTopLevelService customerTopLevelService)
        {
            this._customerStatusService = customerStatusService;
            this._consumerDetailService = consumerDetailService;
            _customerTopLevelService = customerTopLevelService;
            this.logError = LogManager.GetLogger("ERROR");
            this.logInfo = LogManager.GetLogger("INFO");
        }


        /// <summary>
        ///  保存客户状态信息
        /// </summary>
        /// <param name="addCustomerStatusModel"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveCustomerStatus(CustomerStatusModel addCustomerStatusModel)
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

                var isSuccess = _customerStatusService.SaveCustomerStatus(addCustomerStatusModel);
                logInfo.Info(string.Format("保存客户状态信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(addCustomerStatusModel)));
                if (isSuccess > 0)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "保存成功";
                }
                else if (isSuccess == -1)
                {
                    baseViewModel.BusinessStatus = -10001;
                    baseViewModel.StatusMessage = "已存在该客户状态!";
                }
                var result = new
                {
                    BusinessStatus = baseViewModel.BusinessStatus,
                    StatusMessage = baseViewModel.StatusMessage,
                    Id = isSuccess
                };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常 ";
                logError.Error(string.Format("保存客户状态信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }




        /// <summary>
        /// 获取客户状态信息
        /// </summary>
        /// <param name="agentId">顶级代理人编号</param>
        /// <param name="t_Id">前端原编号</param>
        /// <param name="isDeleteData">是否查询出带删除的数据false：不查 true：查</param>
        [HttpGet]
        public HttpResponseMessage GetCustomerStatus(int agentId = 0, int t_Id = -1, bool isDeleteData = false, bool isGetReView = true)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                var customerStatusInfo = _customerStatusService.GetCustomerStatus(agentId, t_Id, isDeleteData, isGetReView);
                logInfo.Info(string.Format("获取客户状态信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), agentId, t_Id, isDeleteData));
                var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, customerStatusInfo = customerStatusInfo };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("获取客户状态信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }

        /// <summary>
        /// 刷数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage MakeCustomerStatus()
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "刷库失败" };
            try
            {
                var customerCategoriesInfo = _customerStatusService.MakeCustomerStatus();
                baseViewModel.BusinessStatus = 1;
                baseViewModel.StatusMessage = "刷库成功";
                return baseViewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "刷库异常";
                logError.Error(string.Format("刷库信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }

        /// <summary>
        /// 获取客户状态信息-名称
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCustomerStatusInfo(int agentId = 0, int t_Id = -1)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                var customerStatusInfo = _customerStatusService.GetCustomerStatusInfo(agentId, t_Id);
                logInfo.Info(string.Format("获取客户状态信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), agentId));
                var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, customerStatusInfo = customerStatusInfo };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("获取客户状态信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }

        ///// <summary>
        ///// 客户状态设置接口
        ///// </summary>
        ///// <param name="agentId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public HttpResponseMessage SetCustomerStatus(int agentId = 0)
        //{
        //    var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "设置失败" };
        //    try
        //    {
        //        var customerCategoriesInfo = _customerStatusService.SetCustomerStatus(agentId);
        //        logInfo.Info(string.Format("设置客户状态信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), agentId));
        //        if (customerCategoriesInfo)
        //        {
        //            baseViewModel.BusinessStatus = 1;
        //            baseViewModel.StatusMessage = "设置客户状态成功";
        //        }
        //        return baseViewModel.ResponseToJson();
        //    }
        //    catch (Exception ex)
        //    {
        //        baseViewModel.BusinessStatus = -10003;
        //        baseViewModel.StatusMessage = "服务器异常";
        //        logError.Error(string.Format("设置客户状态信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
        //        return baseViewModel.ResponseToJson();
        //    }
        //}

        /// <summary>
        /// 修改客户状态信息
        /// </summary>
        /// <param name="updateCustomerStatusModel"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UpdateCustomerStatus(UpdateCustomerStatusModel updateCustomerStatusModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "修改失败" };
            try
            {
                var customerCategoriesInfo = _customerStatusService.UpdateCustomerStatus(updateCustomerStatusModel);
                logInfo.Info(string.Format("修改客户状态信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), updateCustomerStatusModel));
                if (customerCategoriesInfo)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "修改客户状态成功";

                }
                var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("修改客户状态信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }

        
        /// <summary>
        /// 批量修改客户状态信息
        /// </summary>
        /// <param name="updateCustomerStatusModel"></param>
        /// <returns></returns>
        [HttpPost]
        public   HttpResponseMessage BatchUpdateCustomerStatusAndCategories(BatchUpdateCustomerStatusAndCategoriesModel updateCustomerStatusModel)
        {
           
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "更新客户状态和类别失败" };
            if ( _customerTopLevelService.BatchUpdateCustomerStatusAndCategories(updateCustomerStatusModel))
            {
                baseViewModel.BusinessStatus = 1;
                baseViewModel.StatusMessage = "更新客户状态和类别成功";
            }
            return baseViewModel.ResponseToJson();
        }

      


        /// <summary>
        /// 删除客户状态信息
        /// </summary>
        /// <param name="removeCustomerStatusModel"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DeleteCustomerStatusModel(RemoveCustomerStatusModel removeCustomerStatusModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "删除失败" };
            try
            {
                var customerCategoriesInfo = _customerStatusService.DeleteCustomerStatus(removeCustomerStatusModel);
                logInfo.Info(string.Format("删除客户状态信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), removeCustomerStatusModel));
                if (customerCategoriesInfo)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "删除客户状态成功";
                }
                var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("删除客户状态信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }

    }
}
