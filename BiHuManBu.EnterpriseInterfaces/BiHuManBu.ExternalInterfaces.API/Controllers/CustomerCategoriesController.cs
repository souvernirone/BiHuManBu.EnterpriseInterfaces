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
    /// <summary>
    /// 客户类别信息
    /// </summary>
    public class CustomerCategoriesController : ApiController
    {
        readonly ILog logError;
        readonly ILog logInfo;
        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        private readonly ILog _logError = LogManager.GetLogger("ERROR");
        readonly ICustomerCategories _customerCategoriesService;
        //
        // GET: /CustomerCategories/

        public CustomerCategoriesController(ICustomerCategories customerCategoriesService)
        {
            this._customerCategoriesService = customerCategoriesService;
            this.logError = LogManager.GetLogger("ERROR");
            this.logInfo = LogManager.GetLogger("INFO");
        }


        /// <summary>
        /// 保存客户类别信息
        /// </summary>
        /// <param name="customerCategoriesModel"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveCustomerCategories(CustomerCategoriesModel customerCategoriesModel)
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

                var isSuccess = _customerCategoriesService.SaveCustomerCategories(customerCategoriesModel);
                logInfo.Info(string.Format("保存客户类别信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(customerCategoriesModel)));
                if (isSuccess > 0)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "保存成功";
                }
                else if (isSuccess == 10002)
                {
                    baseViewModel.BusinessStatus = 10002;
                    baseViewModel.StatusMessage = "数据已存在,无法继续添加!";
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
                logError.Error(string.Format("保存客户类别信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }



        /// <summary>
        /// 获取客户类别信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCustomerCategories(int agentId = 0)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                var customerCategoriesInfo = _customerCategoriesService.GetCustomerCategories(agentId);
                logInfo.Info(string.Format("获取客户类别信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), agentId));
                var IsStart = false;
                if (customerCategoriesInfo.Any())
                {
                    var ls = customerCategoriesInfo.Where(x => x.IsStart == 1).ToList();
                    //如果相等
                    if (customerCategoriesInfo.Count == ls.Count)
                    {
                        IsStart = true;
                    }
                }
                else
                {
                    IsStart = true;
                }
                var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, customerCategoriesInfo = customerCategoriesInfo, IsStart = IsStart };
                 return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("获取客户类别信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }

        /// <summary>
        /// 刷数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage MakeCustomerCategories()
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "刷库失败" };
            try
            {
                var customerCategoriesInfo = _customerCategoriesService.MakeCustomerCategories();
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
        /// 客户类别设置接口
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SetCustomerCategories(int agentId = 0)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "设置失败" };
            try
            {
                var customerCategoriesInfo = _customerCategoriesService.SetCustomerCategories(agentId);
                logInfo.Info(string.Format("设置客户类别信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), agentId));
                if (customerCategoriesInfo)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "设置客户类别成功";
                }
                return baseViewModel.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("设置客户类别信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }



        /// <summary>
        /// 修改客户类别信息
        /// </summary>
        /// <param name="customercategoriesModel"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UpdateCustomerCategories(UpdateCustomerCategoriesModel customercategoriesModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "修改失败" };
            try
            {
                var customerCategoriesInfo = _customerCategoriesService.UpdateCustomerCategories(customercategoriesModel);
                logInfo.Info(string.Format("修改客户类别信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), customercategoriesModel));
                if (customerCategoriesInfo)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "修改客户类别成功";

                }
                var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("修改客户类别信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }

        /// <summary>
        /// 删除客户类别信息
        /// </summary>
        /// <param name="customercategoriesModel"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DeleteCustomerCategories(RemoveModel removeModel)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "删除失败" };
            try
            {
                var customerCategoriesInfo = _customerCategoriesService.DeleteCustomerCategories(removeModel);
                logInfo.Info(string.Format("删除客户类别信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), removeModel));
                if (customerCategoriesInfo)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "删除客户类别成功";

                }
                var result = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return result.ResponseToJson();
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                logError.Error(string.Format("删除客户类别信息发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel.ResponseToJson();
            }
        }
        // bool UpdateCustomerCategories(bx_customercategories customercategories);




        /// <summary>
        /// 修改客户类别信息
        /// </summary>
        /// <param name="customercategoriesModel"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage BatchUpdateCustomerCategories(BatchUpdateCustomerCategoriesModel model)
        {

            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "更新客户类别失败" };
            if (_customerCategoriesService.BatchUpdateCustomerCategories(model)) 
            {
                baseViewModel.BusinessStatus = 1;
                baseViewModel.StatusMessage = "更新客户类别成功";
            } 
            return   baseViewModel.ResponseToJson();
        }


    }
}
