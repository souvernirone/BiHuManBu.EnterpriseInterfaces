using BiHuManBu.ExternalInterfaces.Services.Interfaces;

using log4net;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;


namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class EnterpriseBatchRenewalController : ApiController
    {
        private ILog _logInfo;
        private ILog _logError;
        //public EnterpriseBatchRenewalController() { }
        IBatchRenewalService _batchRenewalService;

        public EnterpriseBatchRenewalController(IBatchRenewalService batchRenewalService)
        {
            _batchRenewalService = batchRenewalService;
            _logInfo = LogManager.GetLogger("INFO");
            _logError = LogManager.GetLogger("ERROR");
        }

        /// <summary>
        /// 中心调用-根据Buid修改批量续保子表状态
        /// </summary>
        /// <param name="Buid">bx_userino.id</param>
        /// <param name="ItemStatus">1：处理成功（车辆信息，险种全部取回） 2：失败  4：处理成功（有车辆信息未获取到险种）</param>
        /// <returns>修改影响行数</returns>
        [HttpPost]
        public HttpResponseMessage UpdateRenewalResult(int buId, int itemStatus)
        {
            int isSuccess = 0;
            var baseViewMdeol = new BaseViewModel();
            try
            {
                List<int> li = new List<int>() { 1, 2, 4 };
                if (!li.Contains(itemStatus))
                {
                    //参数错误
                    baseViewMdeol.BusinessStatus = 10002;
                    baseViewMdeol.StatusMessage = "参数错误,itemStatus只能是1,2,4";
                    return new { BusinessStatus = baseViewMdeol.BusinessStatus, StatusMessage = baseViewMdeol.StatusMessage }.ResponseToJson(); ;
                }
                isSuccess = _batchRenewalService.UpdateBatchRenewalItem(buId, itemStatus);
                if (isSuccess >= 0)
                {
                    baseViewMdeol.BusinessStatus = 1;
                    baseViewMdeol.StatusMessage = "操作成功";
                    _logInfo.Info("操作成功!" + "参数Buid:" + buId + "参数ItemStatus:" + itemStatus);
                }
                else
                {
                    //如果等于-1 表示传入的BUID 数据库不存在
                    if (isSuccess == -1)
                    {
                        baseViewMdeol.BusinessStatus = 0;
                        baseViewMdeol.StatusMessage = "参数Buid数据库内未检索出!参数无效";
                        _logInfo.Info("操作失败!" + "参数Buid:" + buId + "参数ItemStatus:" + itemStatus);
                    }
                    else
                    {
                        baseViewMdeol.BusinessStatus = 0;
                        baseViewMdeol.StatusMessage = "操作失败";
                        _logInfo.Info("操作失败!" + "参数Buid:" + buId + "参数ItemStatus:" + itemStatus);
                    }

                }

            }
            catch (Exception ex)
            {

                baseViewMdeol.BusinessStatus = 0;
                baseViewMdeol.StatusMessage = "操作失败";
                _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException + "参数Buid:" + buId + "参数ItemStatus:" + itemStatus);
            }
            return baseViewMdeol.ResponseToJson();
            //var baseViewMdeol = new BaseViewModel();

            //var backUpdateBatchRenewalItem = new UpdateBatchRenewalItemModel();
            //try
            //{

            //    _batchRenewalService.UpdateBatchRenewalItem(itemModels);
            //    backUpdateBatchRenewalItem.Id = itemModels[0].Id;
            //    baseViewMdeol.BusinessStatus = 1;
            //    baseViewMdeol.StatusMessage = "操作成功";
            //    _logInfo.Info("批量更新状态Id:" + itemModels[0].Id + ";状态:" + itemModels[0].ItemStatus);
            //}
            //catch (Exception ex)
            //{
            //    backUpdateBatchRenewalItem.Id = itemModels[0].Id;
            //    baseViewMdeol.BusinessStatus = 0;
            //    baseViewMdeol.StatusMessage = "操作失败";
            //    _logInfo.Error("批量更新状态Id:" + itemModels[0].Id + ";状态:" + itemModels[0].ItemStatus + ";错误信息:" + ex.Message);
            //}
            //return new { Id = itemModels[0].Id, BusinessStatus = baseViewMdeol.BusinessStatus, StatusMessage = baseViewMdeol.StatusMessage }.ResponseToJson();

        }
        /// <summary>
        /// 执行批量续保遗留数据
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage ExcuteOldBatchrenewalData()
        {
            var baseViewMdeol = new BaseViewModel();
            bool isSuccess = false;
            try
            {
                isSuccess = _batchRenewalService.ExcuteOldBatchrenewalData();
                baseViewMdeol.BusinessStatus = isSuccess ? 1 : 0;
                baseViewMdeol.StatusMessage = isSuccess ? "执行成功" : "执行失败";
            }
            catch (Exception ex)
            {
                baseViewMdeol.BusinessStatus = 0;
                baseViewMdeol.StatusMessage = "操作失败";
                _logInfo.Error("批量更新状态Id:错误信息:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
            }

            return baseViewMdeol.ResponseToJson();
        }
        /// <summary>
        /// 提供给师傅接口-根据Buid修改批量续保子表状态
        /// </summary>
        /// <param name="Buid">bx_userino.id</param>
        /// <param name="ItemStatus">1：处理成功（车辆信息，险种全部取回） 2：失败  4：处理成功（有车辆信息未获取到险种）</param>
        /// <returns>修改影响行数</returns>
        [HttpPost]
        public HttpResponseMessage UpdateItemStatus(int buId, int itemStatus)
        {
            int isSuccess = 0;
            var baseViewMdeol = new BaseViewModel();
            try
            {
                List<int> li = new List<int>() { 1, 2, 4 };
                if (!li.Contains(itemStatus))
                {
                    //参数错误
                    baseViewMdeol.BusinessStatus = 10002;
                    baseViewMdeol.StatusMessage = "参数错误,itemStatus只能是1,2,4";
                    return new { BusinessStatus = baseViewMdeol.BusinessStatus, StatusMessage = baseViewMdeol.StatusMessage }.ResponseToJson(); ;
                }
                isSuccess = _batchRenewalService.UpdateItemStatus(buId, itemStatus);
                if (isSuccess >= 0)
                {
                    baseViewMdeol.BusinessStatus = 1;
                    baseViewMdeol.StatusMessage = "操作成功";
                    _logInfo.Info("操作成功!" + "参数Buid:" + buId + "参数ItemStatus:" + itemStatus);
                }
                else
                {
                    //如果等于-1 表示传入的BUID 数据库不存在
                    if (isSuccess == -1)
                    {
                        baseViewMdeol.BusinessStatus = 0;
                        baseViewMdeol.StatusMessage = "参数Buid数据库内未检索出!参数无效";
                        _logInfo.Info("操作失败!" + "参数Buid:" + buId + "参数ItemStatus:" + itemStatus);
                    }
                    else
                    {
                        baseViewMdeol.BusinessStatus = 0;
                        baseViewMdeol.StatusMessage = "操作失败";
                        _logInfo.Info("操作失败!" + "参数Buid:" + buId + "参数ItemStatus:" + itemStatus);
                    }
                }
            }
            catch (Exception ex)
            {

                baseViewMdeol.BusinessStatus = 0;
                baseViewMdeol.StatusMessage = "操作失败";
                _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException + "参数Buid:" + buId + "参数ItemStatus:" + itemStatus);
            }
            return new { BusinessStatus = baseViewMdeol.BusinessStatus, StatusMessage = baseViewMdeol.StatusMessage }.ResponseToJson();
        }


    }
}
