using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Common;
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
using System.Configuration;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Infrastructure.UploadImg;
using System.Threading;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using Newtonsoft.Json;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using System.IO;
using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using BiHuManBu.ExternalInterfaces.Models.Enums;
using NPOI.SS.Formula.Functions;


namespace BiHuManBu.ExternalInterfaces.API.Controllers
{

    //[CustomizedRequestAuthorize]
    public class BatchRenewalController : ApiController
    {
        private readonly IBatchRenewalService _batchRenewalService;
        // private readonly _authorityService  _authorityService.NoSystemRoleHasDistributeAuth(request.ChildAgent)
        private readonly IAuthorityService _authorityService;
        private readonly IAgentService _agentService;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private readonly ILog logError = LogManager.GetLogger("ERROR");
        private readonly string keyCode = ConfigurationManager.AppSettings["keyCode"].ToString();
        //
        // GET: /BatchRenewal/

        public BatchRenewalController(IBatchRenewalService batchRenewalService, IAgentService agentService, IAuthorityService authorityService)
        {
            _agentService = agentService;
            _batchRenewalService = batchRenewalService;
            _authorityService = authorityService;
        }

        #region 变量
        string message = string.Empty;
        List<ExcelErrorData> excelErrorDataList = null;
        UploadFileResult result = new UploadFileResult();
        List<BatchRenewalItemViewModel> batchRenewalItemViewModels = new List<BatchRenewalItemViewModel>();
        long batchRenewalId = 0;
        bool isSuccess = false;
        static int dataGrowNum = 0;
        //定义队列
        private Queue<QueueInfo> ListQueue = new Queue<QueueInfo>();
        #endregion


        /// <summary>
        /// 第2步插入数据，返回的
        /// </summary>
        /// <param name="request"></param>
        /// <param name="excelErrorDataList"></param>
        /// <param name="ErrorInfo"></param>
        /// <returns></returns>
        public bool FirsrInsertData(BatchRenewalRequest request, string channelPattern, List<ExcelErrorData> excelErrorDataList, string ErrorInfo, string filePath, int isDelete, bool isCompleted, int totalCount)
        {
            bool firstIssuccess = false;
            //批量插入BatchRenewal
            batchRenewalId = _batchRenewalService.InsertBatchRenewal(request.fileName, channelPattern, totalCount, request.ChildAgent, excelErrorDataList == null ? 0 : excelErrorDataList.Count(), request.Agent, request.cityId, request.batchRenewalType, filePath, isDelete, isCompleted);
            logInfo.Info(string.Format("批量续保主表插入的数据id：{0}", batchRenewalId));
            if (batchRenewalId > 0)
                firstIssuccess = true;
            else
            {
                ErrorInfo = "   BatchRenewal录入失败了!";
                ExceptionError(ErrorInfo);
            }
            //批量插入错误Item表   
            if (excelErrorDataList != null && excelErrorDataList.Count() > 0)
            {
                try
                {
                   var result = _batchRenewalService.BulkInsertBatchRenewaErrorlItem(excelErrorDataList, batchRenewalId);
                    logInfo.Info(string.Format("批量续保错误数据插入的结果：{0}", result));
                }
                catch (Exception ex)
                {
                    logInfo.Info(string.Format("批量续保错误数据插入异常：{0}", ex.Message));
                }
            }
            return firstIssuccess;
        }
        static List<string> timeSetting;
        //[OutputCache(Duration = 7200)]
        /// <summary>
        /// 获取续保时间段
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetTimeSetting()
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                if (!ModelState.IsValid)
                {
                    baseViewModel.BusinessStatus = -10000;
                    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    return baseViewModel.ResponseToJson();
                }
                string timeSettingInfo = "";
                timeSetting = _batchRenewalService.GetTimeSetting();
                foreach (string item in timeSetting)
                {
                    timeSettingInfo += item + " ";
                }
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, timeSettingInfo = timeSettingInfo };
                return batchrenewalresult.ResponseToJson();
            }
            catch (Exception ex)
            {

                LogHelper.Error("获取续保时间段:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return batchrenewalresult.ResponseToJson();
            }
        }
        /// <summary>
        /// 第1步插去重错误数据
        /// </summary>
        /// <param name="renewalCarType"></param>
        /// <param name="topAgent">顶级Id</param>
        /// <param name="agentId"></param>
        /// <param name="roleType">3顶级管理员，4管理员</param>
        /// <param name="checkUserModels"></param>
        public void RemoveErrorData(int renewalCarType, int topAgent, int agentId, IList<CheckBackModel> checkUserModels,out string message)
        {
            //回滚判断、定义的变量一处失败,全部回滚
            //获取agentId 
            //检查老数据
            //循环数据
            message = string.Empty;
            if (checkUserModels == null) return;
            var errorCount = 0;
            for (var i = 0; i < batchRenewalItemViewModels.Count; i++)
            {
                //如果检查车牌号
                var checkUserLiceneseModels = checkUserModels.Where(x => x.LicenseNo == batchRenewalItemViewModels[i].LicenseNo || (!string.IsNullOrEmpty(batchRenewalItemViewModels[i].VinNo) && x.CarVin == batchRenewalItemViewModels[i].VinNo)).ToList();
                //如果不存在任何数据
                if (string.IsNullOrEmpty(batchRenewalItemViewModels[i].SalesManName) && string.IsNullOrEmpty(batchRenewalItemViewModels[i].SalesManAccount))
                {
                    continue;
                }
                //定义代理人Id
                var viewModeAgentid = "";
                if (!string.IsNullOrEmpty(batchRenewalItemViewModels[i].SalesManName) || !string.IsNullOrEmpty(batchRenewalItemViewModels[i].SalesManAccount))
                {
                    viewModeAgentid = !string.IsNullOrEmpty(batchRenewalItemViewModels[i].SalesManAccount) ? batchRenewalItemViewModels[i].SalesManAccount.Split(',')[1] : batchRenewalItemViewModels[i].SalesManName.Split(',')[1];
                }


                #region 批续分配权限放开，一级可分配任何人，二级可分配下属任何人
                var batchRenewalNewRole = ConfigurationManager.AppSettings["BatchRenewalNewRole"];
                //济南大友宝和重庆商社
                if (string.IsNullOrEmpty(batchRenewalNewRole) || batchRenewalNewRole.Split(',').Contains(topAgent.ToString()))
                {
                    var agentIds = _batchRenewalService.GetSonsList(agentId);
                    if (agentIds.Contains(Convert.ToInt32(viewModeAgentid)))
                        continue;
                }
                #endregion
                /*
                 * 2018年7月30日 经过产品确认
                 * 1.检查要上传的车牌或车架号是不是已经分配，如果已经分配给了非本次上传的业务员，那么校验不通过，不允许上传。
                 * 2.如果已经分配给了别人，在1的基础上再次检查有没有存在分配给本次上传的业务员，如果有，就让重新上传，没有就不让上传。（就是说，之前已经上传了几个业务员，其中有一个是本次上传的业务员，那么就允许上传，否则不允许。）
                 */
                var checkState = false;
                var oldSalesManName = string.Empty;
                var checkItem = checkUserLiceneseModels.Where(x => x.IsDistributed > 0 && x.Agent != viewModeAgentid).ToList();
                if (checkItem != null && checkItem.Count > 0)
                {
                    checkState = true;
                    var agentName = checkItem.Select(o => o.AgentName).Distinct();
                    if (agentName != null)
                    {
                        oldSalesManName = string.Join("、", agentName);
                    }
                    var already = checkUserLiceneseModels.Where(o => o.IsDistributed > 0 && o.Agent == viewModeAgentid).ToList();
                    if(already != null && already.Count > 0)
                    {
                        checkState = false;
                    }
                }
            if (checkState)
            {
                    //插入错误数据
                    var excelErrorData = new ExcelErrorData
                    {
                        SalesManName =
                            string.IsNullOrEmpty(batchRenewalItemViewModels[i].SalesManName)
                                ? ""
                                : batchRenewalItemViewModels[i].SalesManName.Split(',')[0],
                        SalesManAccount =
                            string.IsNullOrEmpty(batchRenewalItemViewModels[i].SalesManAccount)
                                ? ""
                                : batchRenewalItemViewModels[i].SalesManAccount.Split(',')[0],
                        LicenseNo = batchRenewalItemViewModels[i].LicenseNo,
                        VinNo = batchRenewalItemViewModels[i].VinNo,
                        EngineNo = batchRenewalItemViewModels[i].EngineNo,
                        MoldName = batchRenewalItemViewModels[i].MoldName,
                        LastYearSource =
                            batchRenewalItemViewModels[i].LastYearSource == "-1"
                                ? ""
                                : batchRenewalItemViewModels[i].LastYearSource,
                        ForceEndDate =
                            batchRenewalItemViewModels[i].ForceEndDate == null
                                ? null
                                : batchRenewalItemViewModels[i].ForceEndDate.Value.ToString("yyyy-MM-dd"),
                        BizEndDate =
                            batchRenewalItemViewModels[i].BizEndDate == null
                                ? null
                                : batchRenewalItemViewModels[i].BizEndDate.Value.ToString("yyyy-MM-dd"),
                        CustomerName = batchRenewalItemViewModels[i].CustomerName,
                        Mobile = batchRenewalItemViewModels[i].Mobile,
                        MobileOther = batchRenewalItemViewModels[i].Client_mobile_other,
                        Remark = batchRenewalItemViewModels[i].Remark,
                        CategoryInfo = batchRenewalItemViewModels[i].CategoryInfo,
                        RegisterDate = batchRenewalItemViewModels[i].RegisterDate,
                        Intention_Remark = batchRenewalItemViewModels[i].Intention_Remark,
                        ErrorMsg = !string.IsNullOrWhiteSpace(oldSalesManName) ? "该条数据已分配给业务员" + oldSalesManName + ",请在客户列表查询,避免重复分配!" : "该条数据已被分配给其他业务员,请在客户列表查询,避免重复分配!"
                    };
                //插入到错误数据
                excelErrorDataList.Add(excelErrorData);
                //移除此Item
                batchRenewalItemViewModels.Remove(batchRenewalItemViewModels[i]);
                i--;
                errorCount = errorCount + 1;
            }
          }
            message = errorCount > 0 ? "<font style=\"color:red\">上传失败：</font>数据已被分配给其他业务员,请在客户列表查询,避免重复分配!" : "";
        }

        /// <summary>
        /// 线程开启
        /// </summary>
        private void threadStart()
        {
            while (true)
            {
                if (ListQueue.Count > 0)
                {
                    try
                    {
                        ScanQueue();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("批量续保后台处理堆栈信息：" + ex.StackTrace + "错误信息：" + ex.Message);
                    }
                }
                else
                {
                    //没有任务，休息3秒钟  
                    Thread.Sleep(3000);
                }
            }
        }
        /// <summary>
        /// 要执行的方法  单线程
        /// </summary>
        private void ScanQueue()
        {
            //事物级别
            var option = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.RepeatableRead,
                Timeout = new TimeSpan(0, 15, 0)
            };
            //设置事物超时的时间 15分钟
            using (var ts = new TransactionScope(TransactionScopeOption.Required, option))
            {
                //从队列中取出  
                QueueInfo queueinfo = ListQueue.Dequeue();
                try
                {
                    bool isSuccess = false;
                    string errorInfo = "";
                    logInfo.Info(string.Format("批量续保,开始队列处理{0}", queueinfo.batchrenewalId));
                    for (var i = 1; i <= queueinfo.batchRenewalItemViewModels.Count / 5000 + 1; i++)
                    {
                        var batchRenewalItemViewModelNew = queueinfo.batchRenewalItemViewModels.Skip(5000 * (i - 1)).Take(5000).ToList();
                        long firstBuid = 0;
                        var needInsert = new List<BatchRenewalItemViewModel>();
                        var lsItemModels = new List<BatchRenewalItemViewModel>();
                        isSuccess = _batchRenewalService.BulkInsertUserInfo(batchRenewalItemViewModelNew, queueinfo.agentId, queueinfo.Agent, queueinfo.ChildAgent, needInsert, lsItemModels, queueinfo.cityId, queueinfo.renewalCarType, queueinfo.checkUserModels, queueinfo.timeSetting, queueinfo.isAuthorization, queueinfo.batchRenewalType, out firstBuid);
                        if (!isSuccess)
                        {
                            errorInfo = "UserInfo表-录入失败了!";
                            ExceptionError(errorInfo);
                        }
                        isSuccess = _batchRenewalService.BulkMaintainBatchRenewalItem(needInsert, lsItemModels, queueinfo.batchrenewalId, queueinfo.agentId, firstBuid, queueinfo.batchRenewalType);
                        if (!isSuccess)
                        {
                            errorInfo = "BatchRenewalItem表-更新失败了!";
                            ExceptionError(errorInfo);
                        }
                    }
                    //批量修改BatchRenewal
                    isSuccess = _batchRenewalService.UpdateBatchRenewal(queueinfo.batchrenewalId);
                    logInfo.Info("批量续保后台处理完成:" + queueinfo.batchrenewalId);
                    if (!isSuccess)
                    {
                        logInfo.Info("批量续保后台处理失败:" + queueinfo.batchrenewalId);
                        errorInfo = "BatchRenewal表-更新失败了!";
                        ExceptionError(errorInfo);
                    }
                    //判断
                    if (isSuccess)
                    {
                        //如果全部执行成功
                        ts.Complete();
                        //结束事物
                        ts.Dispose();
                    }
                    else
                    {
                        //结束事物
                        ts.Dispose();
                        //如果失败删除数据
                        _batchRenewalService.DeteleRenewalData(new List<int> { Convert.ToInt32(queueinfo.batchrenewalId) });
                        logInfo.Info("批量续保后台处理失败,删除批次信息:" + queueinfo.batchrenewalId);
                    }
                }
                catch (Exception ex)
                {
                    //结束事物
                    ts.Dispose();
                    logInfo.Info("批量续保后台处理堆栈信息：" + ex.StackTrace + "错误信息：" + ex.Message);
                    //如果失败删除数据
                    _batchRenewalService.DeteleRenewalData(new List<int> { Convert.ToInt32(queueinfo.batchrenewalId) });
                }
            }
        }
        /// <summary>
        /// 错误
        /// </summary>
        void ExceptionError(string ErrorInfo)
        {
            logInfo.Info(string.Format("批量续保错误：{0}", ErrorInfo));
            throw new ArgumentOutOfRangeException(ErrorInfo);
        }
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="categories"></param>
        /// <param name="agentInfos"></param>
        /// <param name="errorDataList"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private List<BatchRenewalItemViewModel> FromExcel(string filePath, string storePath, List<bx_customercategories> categories, List<bx_agent> agentInfos, out List<ExcelErrorData> errorDataList, out string message)
        {
            var excelHelper = new ExcelHelper<BatchRenewalItemViewModel>();
            var renewalItemViewModels = excelHelper.FromExcel(filePath, storePath, categories, agentInfos, out errorDataList, out message);
            return renewalItemViewModels;
        }

        /// <summary>
        /// 导出批量续保结果
        /// </summary>
        /// <param name="exportRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ExportExecuteResultMethod([FromBody]BatchRenewalExportErrorRequest exportRequest)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                if (!ModelState.IsValid)
                {
                    baseViewModel.BusinessStatus = -10000;
                    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    return baseViewModel.ResponseToJson();
                }
                //增加请求记录
                logInfo.Info(string.Format("导出批量续保结果请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(exportRequest)));
                int totalCount = 0;
                var batchRenewalItems = _batchRenewalService.GetBatchRenewalTable(exportRequest.batchRenewalId);
                var batchRenewalErrorItems = _batchRenewalService.GetBatchRenewalErrorItem(exportRequest.batchRenewalId, 1, 100000, out totalCount).Select(x => new ExcelErrorData { CustomerName = x.CustomerName, EngineNo = x.EngineNo, ErrorMsg = x.ErrorMssage, LicenseNo = x.LicenseNo, Mobile = x.Mobile, RegisterDate = x.RegisterDate, VinNo = x.VinNo, MobileOther = x.MobileOther, Remark = x.Remark, MoldName = x.MoldName, BizEndDate = x.BizEndDate, ForceEndDate = x.ForceEndDate, LastYearSource = x.LastYearSource, CategoryInfo = x.CategoryInfo, SalesManName = x.SalesManName, SalesManAccount = x.SalesManAccount, Intention_Remark = x.Intention_Remark }).ToList();
                string downFileName = _batchRenewalService.GetFileNameByBatchId(exportRequest.batchRenewalId);
                //返回Json串
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, batchRenewalItems = batchRenewalItems, batchRenewalErrorItems = batchRenewalErrorItems, downFileName = downFileName };
                return batchrenewalresult.ResponseToJson();

            }
            catch (Exception ex)
            {

                LogHelper.Error("批量导出数据:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return batchrenewalresult.ResponseToJson();
            }

        }
        /// <summary>
        ///  导出批量续保错误数据
        /// </summary>
        /// <param name="exportRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ExportErrorDataMethod([FromBody]BatchRenewalExportErrorRequest exportRequest)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            int totalCount = 0;
            var filePath = string.Empty;
            var downFileName = string.Empty;
            var extensionName = string.Empty;

            try
            {
                if (!ModelState.IsValid)
                {
                    baseViewModel.BusinessStatus = -10000;
                    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    return baseViewModel.ResponseToJson();
                }
                logInfo.Info(string.Format("导出批量续保错误数据请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(exportRequest)));
                var batchRenewalErrorItems = _batchRenewalService.GetBatchRenewalErrorItem(exportRequest.batchRenewalId, 1, 100000, out totalCount).Select(x => new ExcelErrorData { CustomerName = x.CustomerName, EngineNo = x.EngineNo, ErrorMsg = x.ErrorMssage, LicenseNo = x.LicenseNo, Mobile = x.Mobile, RegisterDate = x.RegisterDate, VinNo = x.VinNo, MobileOther = x.MobileOther, Remark = x.Remark, MoldName = x.MoldName, BizEndDate = x.BizEndDate, ForceEndDate = x.ForceEndDate, LastYearSource = x.LastYearSource, CategoryInfo = x.CategoryInfo, SalesManName = x.SalesManName, SalesManAccount = x.SalesManAccount, Intention_Remark = x.Intention_Remark }).ToList();
                //, CategoryInfo = x.CategoryInfo
                downFileName = _batchRenewalService.GetFileNameByBatchId(exportRequest.batchRenewalId);
                //返回Json串
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, batchRenewalErrorItems = batchRenewalErrorItems, downFileName = downFileName };
                return batchrenewalresult.ResponseToJson();

            }
            catch (Exception ex)
            {
                LogHelper.Error("批量导出错误数据:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return batchrenewalresult.ResponseToJson();
            }
        }
        /// <summary>
        /// 删除批量续保文件
        /// </summary>
        /// <param name="delRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DelBatchRenewalMethod([FromBody]BatchRenewalDeleteDataRequest delRequest)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "删除成功" };
            try
            {
                if (!ModelState.IsValid)
                {
                    baseViewModel.BusinessStatus = -10000;
                    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    return baseViewModel.ResponseToJson();
                }
                logInfo.Info(string.Format("批续删除信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(delRequest)));
                string message = string.Empty;
                bool isSuccess = false;
                //删除
                bool isDelBatchrenewal = _batchRenewalService.DeleteBatchRenewal(delRequest.batchrenewalIdList);
                //查询之后修改
                var isUpdateBuids = _batchRenewalService.SelectBatchrenewal(delRequest.batchrenewalIdList);
                //修改
                List<BatchRenewalUserInfoModel> needUpdateStatus = new List<BatchRenewalUserInfoModel>();
                bool isUpdateItemStatus = _batchRenewalService.UpdateHistoryStatus(delRequest.batchrenewalIdList[0], out needUpdateStatus);
                //修改
                // bool isUpdateUserStatus = _batchRenewalService.BatchUpdateUserStatus(needUpdateStatus);
                if (isDelBatchrenewal && isUpdateBuids && isUpdateItemStatus)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }
                //返回Json串
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, isSuccess = isSuccess, message = isSuccess ? "删除成功" : "删除失败" };
                return batchrenewalresult.ResponseToJson();

            }
            catch (Exception ex)
            {
                LogHelper.Error("删除批量续保:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return batchrenewalresult.ResponseToJson();
            }
        }
        /// <summary>
        /// 根据Buid删除或启用bx_batchrenewal_item表
        /// 目前只为客户列表删除信息使用
        /// </summary>
        /// <param name="buId"></param>
        /// <param name="optionType">0是删除，1是恢复</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateBatchRenewalItemMethod(int buId, OptionType optionType)
        {
            bool success = false;
            //事物级别
            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            //设置事物超时的时间 15分钟
            option.Timeout = new TimeSpan(0, 15, 0);
            using (var ts = new TransactionScope(TransactionScopeOption.Required, option))
            {
                try
                {
                    switch (optionType)
                    {
                        case OptionType.Delete:
                            success = _batchRenewalService.DeleteBatchRenewalItem(buId);
                            break;
                        case OptionType.Revert:
                            success = _batchRenewalService.RevertBatchRenewalItem(buId);
                            break;
                        default:
                            success = false;
                            break;
                    }
                    if (success)
                    {
                        ts.Complete();
                        ts.Dispose();
                    }
                    else
                        ts.Dispose();
                    var result = new { BusinessStatus = 1, StatusMessage = success ? "删除成功" : "删除失败" };
                    return result.ResponseToJson();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    LogHelper.Error("删除批量续保Item:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
                    var result = new { BusinessStatus = "-10003", StatusMessage = "删除错误", };
                    return result.ResponseToJson();
                }
            }
        }


        /// <summary>
        /// 重查批量续保
        /// </summary>
        /// <param name="BatchId"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AnewBatchRenewalMethod([FromBody]UpdateBatchRenewalRequest updateBatchRenewalRequest)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "操作成功" };
            try
            {
                logInfo.Info(string.Format("重查批量续保请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), updateBatchRenewalRequest));
                bool isSuccess = false;
                //删除 
                bool isAnewBatchrenewal = _batchRenewalService.AnewBatchRenewal(updateBatchRenewalRequest.batchRenewalId, updateBatchRenewalRequest.operateType, updateBatchRenewalRequest.channelPattern);

                if (isAnewBatchrenewal)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                    baseViewModel.BusinessStatus = 0;
                    baseViewModel.StatusMessage = "操作失败";
                }
                //返回Json串
                var anewBatchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, isSuccess = isSuccess };
                return anewBatchrenewalresult.ResponseToJson();

            }
            catch (Exception ex)
            {

                LogHelper.Error("重查/修改批量续保:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return batchrenewalresult.ResponseToJson();
            }
        }
        /// <summary>
        /// 获取可设置的数量
        /// </summary>
        /// <param name="countRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetAgentBatchRenewalCountMethod([FromBody]BatchRenewalGetCountRequest countRequest)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                if (!ModelState.IsValid)
                {
                    baseViewModel.BusinessStatus = -10000;
                    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    return baseViewModel.ResponseToJson();
                }
                logInfo.Info(string.Format("获取可设置的数量请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(countRequest)));
                ////验证参数的对象转换
                //var baseRequest = new BaseVerifyRequest()
                //{
                //    Agent = countRequest.Agent,
                //    SecCode = countRequest.SecCode
                //};
                //if (countRequest.ToPostSecCode() == countRequest.SecCode)
                //{
                var batchRenewalCount = _batchRenewalService.GetAgentBatchRenewalCount(countRequest.Agent);
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, count = batchRenewalCount };
                return batchrenewalresult.ResponseToJson();

            }
            catch (Exception ex)
            {
                LogHelper.Error("批量续保获取可执行数:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, count = 0 };
                return batchrenewalresult.ResponseToJson();
            }
        }


        /// <summary>
        /// 获取批量续保选择续保城市接口 
        /// </summary>
        /// <param name="sourceRequest"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public HttpResponseMessage GetBatchRenewalSourceMethod([FromBody]BatchRenewalGetSourceRequest sourceRequest)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "获取成功" };
            try
            {
                logInfo.Info(string.Format("获取批量续保选择续保城市请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(sourceRequest)));
                //获取续保
                var BatchRenewalSource = _batchRenewalService.GetBatchRenewalSource(sourceRequest.cityId, sourceRequest.Agent.ToString());
                if (!BatchRenewalSource.Any())
                {
                    //BatchRenewalSource batchRenewal = new BatchRenewalSource()
                    //{
                    //    Id = 3,
                    //    isuse = 1,
                    //    SourceName = "中国人寿财险"
                    //};
                    //BatchRenewalSource batchRenewal1 = new BatchRenewalSource()
                    //{
                    //    Id = 4,
                    //    isuse = 1,
                    //    SourceName = "中华联合车险"
                    //};
                    var lstSource = new List<BatchRenewalSource>()
                    {
                        new BatchRenewalSource {Id = 3, isuse = 0, SourceName = "中国人寿财险"},
                        new BatchRenewalSource {Id = 4, isuse = 0, SourceName = "中华联合车险"}

                    };
                    BatchRenewalSource = lstSource;
                }
                else
                {
                    if (BatchRenewalSource.Where(x => x.Id == 4).Count() == 0)
                    {
                        BatchRenewalSource batchRenewal = new BatchRenewalSource()
                        {
                            Id = 4,
                            isuse = 0,
                            SourceName = "中华联合车险"
                        };
                        BatchRenewalSource.Add(batchRenewal);
                    }
                    if (BatchRenewalSource.Where(x => x.Id == 3).Count() == 0)
                    {
                        BatchRenewalSource batchRenewal = new BatchRenewalSource()
                        {
                            Id = 3,
                            isuse = 0,
                            SourceName = "中国人寿财险"
                        };
                        BatchRenewalSource.Add(batchRenewal);
                    }
                }
                //获取资源结果
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, BatchRenewalSource = BatchRenewalSource };
                return batchrenewalresult.ResponseToJson();
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取续保城市接口:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return batchrenewalresult.ResponseToJson();
            }
        }


        /// <summary>
        /// 获取批量续保排队剩余时间
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetBatchRenewalQueueTimeMethod([FromBody]BatchRenewalCommonRequest bCommonRequest)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                //获取续保
                var getBatchQueueTime = _batchRenewalService.GetBatchRenewalQueueTime(bCommonRequest.BatchId);
                //获取资源结果
                var getBatchQueueTimeresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, getBatchQueueTime = getBatchQueueTime };
                return getBatchQueueTimeresult.ResponseToJson();
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取批量续保排队剩余时间:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return batchrenewalresult.ResponseToJson();
            }
        }
        /// <summary>
        /// 获取已设置的数量
        /// </summary>
        /// <param name="countRequest">s</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetAgentBatchRenewalSetCountMethod([FromBody]BatchRenewalGetCountRequest countRequest)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                if (!ModelState.IsValid)
                {
                    baseViewModel.BusinessStatus = -10000;
                    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    return baseViewModel.ResponseToJson();
                }
                logInfo.Info(string.Format("批续获取已信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(countRequest)));
                ////验证参数的对象转换
                //var baseRequest = new BaseVerifyRequest()
                //{
                //    Agent = countRequest.Agent,
                //    SecCode = countRequest.SecCode
                //};
                //if (countRequest.ToPostSecCode() == countRequest.SecCode)
                //{
                //var batchRenewalSetCount = _batchRenewalService.GetSettedCount(countRequest.AgentId);
                var batchRenewalSetCount = _batchRenewalService.GetSettedCount(countRequest.Agent);
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, count = batchRenewalSetCount };
                return batchrenewalresult.ResponseToJson();

            }
            catch (Exception ex)
            {
                LogHelper.Error("批量续保已设置数:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, count = 0 };
                return batchrenewalresult.ResponseToJson();
            }
        }
        /// <summary>
        /// 获取批量续保列表
        /// </summary>
        /// <param name="listRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetBatchRenewalListMethod([FromBody]BatchRenewalListRequest listRequest)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "查询成功" };
            try
            {
                if (!ModelState.IsValid)
                {
                    baseViewModel.BusinessStatus = -10000;
                    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                    return baseViewModel.ResponseToJson();
                }
                //logInfo.Info(string.Format("批续导入信息请求url为：{0}；请求参数为：{1}", Request.RequestUri.ToString(), JsonHelper.Serialize(listRequest)));
                ////验证参数的对象转换
                //var baseRequest = new BaseVerifyRequest()
                //{
                //    Agent = listRequest.Agent,
                //    SecCode = listRequest.SecCode
                //};
                //if (listRequest.ToPostSecCode() == listRequest.SecCode)
                //{
                int pageSize = ApplicationSettingsFactory.GetApplicationSettings().BatchRenewalPageSize;
                //定义当前页的batchID集合
                int totalCount;
                List<long> batchRenewalIdList = new List<long>();
                int agent = listRequest.Agent;
                int childAgent = listRequest.agentId;
                var agentids = _agentService.GetSonAgents(agent, childAgent);
                List<bx_agent> agentInfos = new List<bx_agent>();
                if (agentids.BusinessStatus == 1)
                {
                    foreach (var item in agentids.ListAgent)
                    {
                        bx_agent bxagent = new bx_agent();
                        bxagent.Id = item.AgentId;
                        bxagent.AgentName = item.AgentName;
                        bxagent.AgentAccount = item.AgentAccount;
                        agentInfos.Add(bxagent);
                    }
                }
                //首次加载，需要查一遍
                if (listRequest.presentBatchIds != null && listRequest.presentBatchIds.Any())
                {
                    _batchRenewalService.TaskUpdateCount(listRequest.presentBatchIds);
                }
                else
                {
                    batchRenewalIdList = _batchRenewalService.GetBatchRenewalList(listRequest, agentInfos, out totalCount).Where(x => x.IsCompleted == 1).Select(x => x.Id).ToList();
                    _batchRenewalService.TaskUpdateCount(batchRenewalIdList);
                }
                var batchRenewalData = _batchRenewalService.GetBatchRenewalList(listRequest, agentInfos, out totalCount).Select(x => new
                {
                    CreateTime = x.CreateTime,
                    Id = x.Id,
                    FailedCount = x.BatchRenewalType == 1 ? 0 : x.FailedCount,
                    FileName = x.FileName,
                    StartExecuteTime = x.StartExecuteTime,
                    SuccessfullCount = x.BatchRenewalType == 1 ? 0 : x.SuccessfullCount,
                    TotalCount = x.TotalCount,
                    UntreatedCount = x.BatchRenewalType == 1 ? 0 : x.UntreatedCount,
                    TaskStatus = x.TaskStatus,
                    IsCompleted = x.IsCompleted,
                    Progress = x.TotalCount == 0 ? 100 :(Convert.ToDouble(x.FailedCount + x.SuccessfullCount) * 100 / Convert.ToDouble(x.TotalCount) > 0 && Convert.ToDouble(x.FailedCount + x.SuccessfullCount) * 100 / Convert.ToDouble(x.TotalCount) < 1) ? 1 : Math.Floor(Convert.ToDouble(x.FailedCount + x.SuccessfullCount) * 100 / Convert.ToDouble(x.TotalCount)),
                    IsDistributed = x.IsDistributed,
                    ErrorDataCount = x.ErrorDataCount,
                    IsAgainRenewal = x.IsAgainRenewal,
                    CityId = x.CityId,
                    channelPattern = x.ChannelPattern,
                    BatchRenewalType = x.BatchRenewalType,
                    FilePath = string.IsNullOrWhiteSpace(x.FilePath) ? "" : x.FilePath
                });
                //timeSetting = _batchRenewalService.GetTimeSetting();
                //if (timeSetting.Count == 0)
                //{, timeSetting = timeSetting
                //    timeSetting.Add("16:00-22:00");
                //}
                //分页结果集字符串
                string strNav = PageNavifationHelper.ShowPageNavigate(listRequest.pageIndex, pageSize, totalCount);

                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, batchRenewalData = batchRenewalData, totalCount = totalCount };
                return batchrenewalresult.ResponseToJson();

            }
            catch (Exception ex)
            {
                LogHelper.Error("续保列表:" + ex.Message + "；堆栈信息：" + ex.StackTrace);
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                var batchrenewalresult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage };
                return batchrenewalresult.ResponseToJson();
            }
        }
        public class BatchResult
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
        }

        /// <summary>
        /// 批量续保上传
        /// </summary>
        /// <param name="renewalRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage BatchRenewalMethod([FromBody]BatchRenewalRequest renewalRequest)
        {
            var batchresult = new BatchResult();

            #region 初始化
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "操作失败" };
            if (!ModelState.IsValid)
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                batchresult.IsValid = false;
                batchresult.Message = "输入参数错误";
                var batchRenewalResult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, isValid = batchresult.IsValid, name = renewalRequest.fileName, msg = batchresult.Message, errorCount = excelErrorDataList == null ? 0 : excelErrorDataList.Count(), batchRenewalId = batchRenewalId };
                return batchRenewalResult.ResponseToJson();
            }

            if (renewalRequest.ChannelPattern.ChannelType > 1)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "渠道类型参数错误";
                batchresult.IsValid = false;
                batchresult.Message = "渠道类型参数错误";
                var batchRenewalResult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, isValid = batchresult.IsValid, name = renewalRequest.fileName, msg = batchresult.Message, errorCount = excelErrorDataList == null ? 0 : excelErrorDataList.Count(), batchRenewalId = batchRenewalId };
                return batchRenewalResult.ResponseToJson();
            }
            if (renewalRequest.ChannelPattern.SelectedSources.Count == 0)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "保司参数错误";
                batchresult.IsValid = false;
                batchresult.Message = "保司参数错误";
                var batchRenewalResult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, isValid = batchresult.IsValid, name = renewalRequest.fileName, msg = batchresult.Message, errorCount = excelErrorDataList == null ? 0 : excelErrorDataList.Count(), batchRenewalId = batchRenewalId };
                return batchRenewalResult.ResponseToJson();
            }
            if (renewalRequest.cityId < 1)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "投保地区参数错误";
                batchresult.IsValid = false;
                batchresult.Message = "投保地区参数错误";
                var batchRenewalResult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, isValid = batchresult.IsValid, name = renewalRequest.fileName, msg = batchresult.Message, errorCount = excelErrorDataList == null ? 0 : excelErrorDataList.Count(), batchRenewalId = batchRenewalId };
                return batchRenewalResult.ResponseToJson();
            }         
            
            //序列化
            string channelPattern = "";
            try
            {
                channelPattern = JsonConvert.SerializeObject(renewalRequest.ChannelPattern);
            }
            catch (Exception)
            {

                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "输入参数序列化错误";
                batchresult.IsValid = false;
                batchresult.Message = "输入参数序列化错误";
                var batchRenewalResult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, isValid = batchresult.IsValid, name = renewalRequest.fileName, msg = batchresult.Message, errorCount = excelErrorDataList == null ? 0 : excelErrorDataList.Count(), batchRenewalId = batchRenewalId };
                return batchRenewalResult.ResponseToJson();
            }
            logInfo.Info(string.Format("批续上传请求url为：{0}；请求参数为：{1}", Request.RequestUri, JsonHelper.Serialize(renewalRequest)));
            #endregion


            List<bx_customercategories> categories = _batchRenewalService.SelectCategories(renewalRequest.Agent);

            #region 获取当前用户下级

            var agent = renewalRequest.Agent;
            var agentids = _agentService.GetSonAgents(agent, renewalRequest.agentId);
            var agentInfos = new List<bx_agent>();
            if (agentids.BusinessStatus == 1)
            {
                agentInfos.AddRange(agentids.ListAgent.Select(item => new bx_agent
                {
                    Id = item.AgentId,
                    AgentName = item.AgentName,
                    AgentAccount = item.AgentAccount
                }));
            }
            #endregion

            var fileNewName = renewalRequest.fileName.Substring(0, renewalRequest.fileName.LastIndexOf('.')) + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") +
                             renewalRequest.fileName.Substring(renewalRequest.fileName.LastIndexOf('.'));
            var storePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\BatchRenewal\\" + renewalRequest.agentId + "\\" + fileNewName + "";
            logInfo.Info(string.Format("批续上传文件路径url为：{0}", storePath));

            //从EXCEL读取数据
            batchRenewalItemViewModels = FromExcel(renewalRequest.filePath, storePath, categories, agentInfos, out excelErrorDataList, out message);

            //经纪人已设置的数据
            var settedCount = _batchRenewalService.GetSettedCount(renewalRequest.Agent);
            //总数
            var totalCount = _batchRenewalService.GetAgentBatchRenewalCount(renewalRequest.Agent);
            //判断
            if (batchRenewalItemViewModels != null)
            {
                if (renewalRequest.batchRenewalType == 0)
                {
                    if (settedCount + batchRenewalItemViewModels.Count > totalCount)
                    {
                        baseViewModel.BusinessStatus = -10003;
                        baseViewModel.StatusMessage = "<font style=\"color:red\">上传失败：</font>今日可用条数不足";
                        batchresult.IsValid = false;
                        batchresult.Message = "<font style=\"color:red\">上传失败：</font>今日可用条数不足";
                        var batchRenewalResult =
                            new
                            {
                                BusinessStatus = baseViewModel.BusinessStatus,
                                StatusMessage = baseViewModel.StatusMessage,
                                isValid = batchresult.IsValid,
                                name = renewalRequest.fileName,
                                msg = batchresult.Message,
                                errorCount = excelErrorDataList == null ? 0 : excelErrorDataList.Count(),
                                batchRenewalId = batchRenewalId
                            };
                        return batchRenewalResult.ResponseToJson();
                    }
                }
            }
            //错误信息
            string ErrorInfo = "";
            if (!string.IsNullOrEmpty(message) && excelErrorDataList.Any())
            {
                isSuccess = FirsrInsertData(renewalRequest, channelPattern, excelErrorDataList, ErrorInfo, renewalRequest.filePath, 1, true, excelErrorDataList.Count);
            }
            if (string.IsNullOrEmpty(message))
            {
        
                try
                {
                    //获取Excel中在数据库存在的车辆信息
                    var checkUserModels = _batchRenewalService.CheckUserInfo(batchRenewalItemViewModels, renewalRequest.agentId.ToString(), renewalRequest.renewalCarType, renewalRequest.Agent);
                    //去除错误数据
                    var errorMessage = string.Empty;
                    if (checkUserModels != null)
                    {
                        RemoveErrorData(renewalRequest.renewalCarType, renewalRequest.Agent, renewalRequest.agentId, checkUserModels,out errorMessage);
                    }
                    if (batchRenewalItemViewModels.Count == 0)
                    {
                        baseViewModel.BusinessStatus = -10003;
                        var thisMessage = string.IsNullOrWhiteSpace(errorMessage) ? "<font style=\"color:red\">上传失败：</font>请在模板中查看上传规则，并检查数据正确性" : errorMessage;
                        baseViewModel.StatusMessage = thisMessage;
                        batchresult.IsValid = false;
                        batchresult.Message = thisMessage;
                        var batchRenewalResult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, isValid = batchresult.IsValid, name = renewalRequest.fileName, msg = batchresult.Message, errorCount = excelErrorDataList == null ? 0 : excelErrorDataList.Count(), batchRenewalId = batchRenewalId };
                        return batchRenewalResult.ResponseToJson();
                    }
                    //插入数据 batchrenewal表、错误表
                    isSuccess = FirsrInsertData(renewalRequest, channelPattern, excelErrorDataList, ErrorInfo, renewalRequest.filePath, 0, false, batchRenewalItemViewModels.Count);
                    bool isAuthorization = false;

                    isAuthorization = _authorityService.IsSystemAdminOrAdmin(renewalRequest.ChildAgent);
                    //列队类实例化
                    QueueInfo queueinfo = new QueueInfo();
                    queueinfo.batchRenewalItemViewModels = batchRenewalItemViewModels;
                    queueinfo.fileName = renewalRequest.fileName;
                    queueinfo.filePath = renewalRequest.filePath;
                    queueinfo.batchrenewalId = batchRenewalId;
                    queueinfo.agentId = renewalRequest.agentId.ToString();
                    queueinfo.Agent = renewalRequest.Agent;
                    queueinfo.ChildAgent = renewalRequest.ChildAgent;
                    queueinfo.cityId = renewalRequest.cityId;
                    queueinfo.renewalCarType = renewalRequest.renewalCarType;
                    queueinfo.checkUserModels = checkUserModels;
                    queueinfo.timeSetting = renewalRequest.timeSetting;
                    queueinfo.dataGrowNum = dataGrowNum;
                    queueinfo.isAuthorization = isAuthorization;
                    queueinfo.batchRenewalType = renewalRequest.batchRenewalType;
                    ListQueue.Enqueue(queueinfo);
                    //判断
                    logInfo.Info("线程开始处理1:");
                    if (isSuccess)
                    {
                        baseViewModel.BusinessStatus = 1;
                        baseViewModel.StatusMessage = "上传成功";
                        //如果全部执行成功
                        batchresult.IsValid = true;
                        batchresult.Message = "上传成功";
                        //开启新线程
                        Thread thread = new Thread(threadStart);
                        thread.IsBackground = true;
                        thread.Start();
                    }
                    else
                    {
                        baseViewModel.BusinessStatus = -10003;
                        baseViewModel.StatusMessage = "上传失败";
                        batchresult.IsValid = false;
                        batchresult.Message = "上传失败;请联系系统管理员";
                    }
                }
                catch (Exception x)
                {

                    baseViewModel.BusinessStatus = -10003;
                    baseViewModel.StatusMessage = "上传失败";
                    batchresult.IsValid = false;
                    batchresult.Message = "上传失败;请联系系统管理员";
                    LogHelper.Error(ErrorInfo + "" + "堆栈信息：" + x.StackTrace + ";文件路径：" + renewalRequest.filePath);
                }
            }
            else
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = message;
                batchresult.IsValid = false;
                batchresult.Message = message;

            }

            var allbatchRenewalResult = new { BusinessStatus = baseViewModel.BusinessStatus, StatusMessage = baseViewModel.StatusMessage, isValid = batchresult.IsValid, name = renewalRequest.fileName, msg = batchresult.Message, errorCount = excelErrorDataList == null ? 0 : excelErrorDataList.Count(), batchRenewalId = batchRenewalId };
            return allbatchRenewalResult.ResponseToJson();

        }
    }
}
