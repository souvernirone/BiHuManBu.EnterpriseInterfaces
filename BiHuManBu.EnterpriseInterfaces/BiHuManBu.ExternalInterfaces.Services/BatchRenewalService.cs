using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
namespace BiHuManBu.ExternalInterfaces.Services
{
    public class BatchRenewalService : IBatchRenewalService
    {
        readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        readonly int dataGrowNum = ApplicationSettingsFactory.GetApplicationSettings().DataGrowNum;
        IBatchRenewalRepository _batchRenewalRepository;
        ICompanyrelationRepository _companyrelationRepository;
        private ICacheHelper _cacheHelper;
        static int renewType = 0;
        public BatchRenewalService(IBatchRenewalRepository batchRenewalRepository, ICacheHelper cacheHelper, ICompanyrelationRepository companyrelationRepository)
        {
            _batchRenewalRepository = batchRenewalRepository;
            _cacheHelper = cacheHelper;
            _companyrelationRepository = companyrelationRepository;
        }
        public int UpdateBatchRenewalItem(int buId, int itemStatus)
        {

            return _batchRenewalRepository.UpdateBatchRenewalItem(buId, itemStatus);

        }

        //public int UpdateBatchRenewalItem(List<UpdateBatchRenewalItemModel> itemModels)
        //{

        //    return Convert.ToInt32(MySqlHelper.BulkUpdateByList<UpdateBatchRenewalItemModel>(itemModels, DbConfig, "bx_batchrenewal_item", "Id"));

        //}

        public bool ResettinStatus(long id)
        {
            return _batchRenewalRepository.ResettinStatus(id);
        }

        public bool ExcuteOldBatchrenewalData()
        {
            return _batchRenewalRepository.ExcuteOldBatchrenewalData();
        }
        public int UpdateItemStatus(int buId, int itemStatus)
        {
            return _batchRenewalRepository.UpdateItemStatus(buId, itemStatus);
        }


        /// <summary>
        /// 获取批续对应的数据
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        public bx_batchrenewal_item GetItemByBuId(long buId)
        {
            return _batchRenewalRepository.GetItemByBuId(buId);
        }
        //}
        public IList<CheckBackModel> CheckUserInfo(List<BatchRenewalItemViewModel> batchRenewalItems, string agentId, int renewalCarType, int topagentId)
        {
            var checkUserModels = _batchRenewalRepository.CheckUserInfo(batchRenewalItems, agentId, renewalCarType, topagentId);
            return checkUserModels;
        }
        public bool BulkInsertBatchRenewaErrorlItem(List<ExcelErrorData> excelErrorDataList, long batchId)
        {
            List<bx_batchrenewal_erroritem> batchRenewalErrorItemList = new List<bx_batchrenewal_erroritem>();
            foreach (var item in excelErrorDataList)
            {
                bx_batchrenewal_erroritem batchRenewalErrorItem = new bx_batchrenewal_erroritem();
                batchRenewalErrorItem.BatchId = batchId;
                batchRenewalErrorItem.CreateTime = item.CreateTime;
                batchRenewalErrorItem.RowIndex = item.RowIndex;
                if (!string.IsNullOrWhiteSpace(item.ErrorMsg))
                {
                    batchRenewalErrorItem.ErrorMssage = item.ErrorMsg.Length > 100 ? item.ErrorMsg.Substring(0, 100) : item.ErrorMsg;
                }
                //batchRenewalErrorItem.ErrorMssage = item.ErrorMsg;
                batchRenewalErrorItem.CustomerName = item.CustomerName;
                batchRenewalErrorItem.EngineNo = item.EngineNo;
                batchRenewalErrorItem.LicenseNo = item.LicenseNo;
                batchRenewalErrorItem.Mobile = item.Mobile;
                batchRenewalErrorItem.VinNo = item.VinNo;
                batchRenewalErrorItem.MobileOther = item.MobileOther;
                batchRenewalErrorItem.Remark = item.Remark;
                //备注2
                batchRenewalErrorItem.Intention_Remark = item.Intention_Remark;
                batchRenewalErrorItem.BizEndDate = item.BizEndDate;
                batchRenewalErrorItem.ForceEndDate = item.ForceEndDate;
                batchRenewalErrorItem.LastYearSource = item.LastYearSource;
                batchRenewalErrorItem.MoldName = item.MoldName;
                batchRenewalErrorItem.SalesManAccount = item.SalesManAccount;
                batchRenewalErrorItem.SalesManName = item.SalesManName;
                batchRenewalErrorItem.CategoryInfo = item.CategoryInfo;

                batchRenewalErrorItemList.Add(batchRenewalErrorItem);
            }
            return _batchRenewalRepository.BulkInsertBatchRenewaErrorlItem(batchRenewalErrorItemList);
        }
        public IList<DownLoadExcel> GetBatchRenewalTable(long batchRenewalId)
        {
            return _batchRenewalRepository.GetBatchRenewalTable(batchRenewalId);
        }

        public bool UpdateHistoryStatus(int BatchrenewalId, out List<BatchRenewalUserInfoModel> needUpdateStatus)
        {
            return _batchRenewalRepository.UpdateHistoryStatus(BatchrenewalId, out needUpdateStatus);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="BatchrenewalIdList"></param>
        /// <returns></returns>
        public bool SelectBatchrenewal(List<int> BatchrenewalIdList)
        {
            return _batchRenewalRepository.SelectBatchrenewal(BatchrenewalIdList);
        }

        public string GetFileNameByBatchId(long batchId)
        {
            return _batchRenewalRepository.GetFileNameByBatchId(batchId);
        }

        public List<BatchRenewalSource> GetCacheBatchRenewalSource(int cityId)
        {
            return _batchRenewalRepository.GetCacheBatchRenewalSource(cityId);

        }

        /// <summary>
        /// 获取续保时间段
        /// </summary>
        /// <returns></returns>
        public List<string> GetTimeSetting()
        {
            var getTimeSetting = _batchRenewalRepository.GetTimeSetting();
            return getTimeSetting;
        }


        /// <summary>
        /// 操作UserInfo的函数
        /// </summary>
        /// <param name="batchRenewalItems">Excel中读取的数据</param>
        /// <param name="agentId"></param>
        /// <param name="agent">当前登陆人顶级ID</param>
        /// <param name="childAgent">当前登陆人</param>
        /// <param name="needInsert"></param>
        /// <param name="needUpdate"></param>
        /// <param name="cityId"></param>
        /// <param name="renewalCarType"></param>
        /// <param name="checkUserModels">数据库中存在的老数据</param>
        /// <param name="timeSetting"></param>
        /// <param name="isAuthorization"></param>
        /// <param name="batchRenewalType"></param>
        /// <param name="firstBuid"></param>
        /// <returns></returns>
        public bool BulkInsertUserInfo(List<BatchRenewalItemViewModel> batchRenewalItems, string agentId, int agent,int childAgent,
            List<BatchRenewalItemViewModel> needInsert, List<BatchRenewalItemViewModel> needUpdate, int cityId,
            int renewalCarType, IList<CheckBackModel> checkUserModels, List<string> timeSetting, bool isAuthorization,
            int batchRenewalType, out long firstBuid)
        {

            bool isSuccess = false;
            firstBuid = 0;
            var userInfoList = new List<UserInfoModel>();
            //测试
            var checkUserModelsForLicenseno = checkUserModels.OrderByDescending(x => x.UpdateTime).DistinctBy(x => x.LicenseNo);
            var checkUserModelsForCarVin = checkUserModelsForLicenseno.Where(x => !string.IsNullOrWhiteSpace(x.CarVin));
            checkUserModels = checkUserModelsForLicenseno.Except(checkUserModelsForCarVin).Union(checkUserModelsForCarVin.DistinctBy(x => x.CarVin)).ToList();
            var isRenewaledLicenseNo = checkUserModels.Select(x => x.LicenseNo);
            var isRenewaledCarVin = checkUserModels.Select(x => x.CarVin);
            //  var categoriesInfo = _batchRenewalRepository.SelectCategories(Convert.ToInt32(agentId));
            //需要修改Item的id
            var upItemBuids = new List<long>();
            //修改修改IsNew的id
            var upNewBuids = new List<long>();
            foreach (var item in batchRenewalItems)
            {
                var newLicenseNo = string.IsNullOrWhiteSpace(item.LicenseNo) ? item.VinNo.Trim() : item.LicenseNo.Trim();
                var newCarVin = string.IsNullOrWhiteSpace(item.VinNo) ? "" : item.VinNo.Trim();
                if ((!string.IsNullOrWhiteSpace(newCarVin) && isRenewaledCarVin.Contains(newCarVin)) || isRenewaledLicenseNo.Contains(newLicenseNo))
                {
                    var checkModel = checkUserModels.FirstOrDefault(x => x.LicenseNo == newLicenseNo ||
                    (!string.IsNullOrWhiteSpace(newCarVin) && x.CarVin == newCarVin));
                    if (checkModel.LastBizEndDate != null || checkModel.LastForceEndDate != null)
                    {
                        if (checkModel.LastBizEndDate == null)
                        {
                            if ((checkModel.LastForceEndDate.Value - DateTime.Now).Days <= 90)
                            {
                                //需要续保待中心处理
                                item.ItemStatus = checkModel.RenewalStatus == -1 ? -1 : 0;
                                upItemBuids.Add(checkModel.BuId);
                            }
                            else
                            {
                                item.ItemStatus = checkModel.RenewalStatus == 0 ? 0 : checkModel.Status;
                            }
                        }
                        else
                        {
                            if ((checkModel.LastBizEndDate.Value - DateTime.Now).Days <= 90)
                            {
                                item.ItemStatus = checkModel.RenewalStatus == -1 ? -1 : 0;
                                upItemBuids.Add(checkModel.BuId);
                            }
                            else
                            {
                                item.ItemStatus = checkModel.RenewalStatus == 0 ? 0 : checkModel.Status;
                            }
                        }
                        upNewBuids.Add(checkModel.BuId);
                    }
                    else
                    {
                        //如果当前时间大于今天的下午4点
                        item.ItemStatus = checkModel.RenewalStatus != 0 ? -1 : 0;
                        upItemBuids.Add(checkModel.BuId);
                        upNewBuids.Add(checkModel.BuId);
                    }
                    item.BuId = checkModel.BuId;
                    if (string.IsNullOrWhiteSpace(item.CategoryInfo) || item.CategoryInfo == "0")
                    {
                        item.CategoryinfoId = checkModel.CategoryInfoId;
                    }
                    else
                    {
                        item.CategoryinfoId = Convert.ToInt32(item.CategoryInfo);
                    }
                    #region 新增修改-如果续保类型是摄像头-3，修改为批量续保-5
                    if (!string.IsNullOrEmpty(checkModel.RenewalType))
                    {
                        renewType = Convert.ToInt32(checkModel.RenewalType);
                    }
                    #endregion
                    if (string.IsNullOrEmpty(item.Remark))
                    {
                        item.Remark = StringHandleHelper.FilterBatchRenewalString(checkModel.Remark);
                    }
                    if (string.IsNullOrEmpty(item.Client_mobile_other))
                    {
                        item.Client_mobile_other = StringHandleHelper.FilterBatchRenewalString(checkModel.client_mobile_other);
                    }
                    if (string.IsNullOrEmpty(item.Mobile))
                    {
                        item.Mobile = StringHandleHelper.FilterBatchRenewalString(checkModel.client_mobile);
                    }
                    //品牌型号
                    if (string.IsNullOrEmpty(item.MoldName) && !string.IsNullOrWhiteSpace(checkModel.MoldName))
                        item.MoldName = StringHandleHelper.FilterBatchRenewalString(checkModel.MoldName);
                    //注册日期
                    if (string.IsNullOrEmpty(item.RegisterDate) && !string.IsNullOrWhiteSpace(checkModel.RegisterDate))
                        item.RegisterDate = checkModel.RegisterDate;
                    //发动机号
                    if (string.IsNullOrEmpty(item.EngineNo) && !string.IsNullOrWhiteSpace(checkModel.EngineNo))
                        item.EngineNo = StringHandleHelper.FilterBatchRenewalString(checkModel.EngineNo);
                    //车架号
                    if (string.IsNullOrEmpty(item.VinNo) && !string.IsNullOrWhiteSpace(checkModel.CarVin))
                        item.VinNo = StringHandleHelper.FilterBatchRenewalString(checkModel.CarVin);

                    if (string.IsNullOrEmpty(item.CustomerName))
                        item.CustomerName = StringHandleHelper.FilterBatchRenewalString(checkModel.client_name);
              

                    item.BuId = checkModel.BuId;
                    //如果存在分配的数据
                    if (!string.IsNullOrWhiteSpace(item.SalesManAccount) || !string.IsNullOrWhiteSpace(item.SalesManName))
                    {
                        if (!string.IsNullOrWhiteSpace(item.SalesManAccount) && !string.IsNullOrWhiteSpace(item.SalesManName))
                        {
                            item.OpenId = CommonHelper.GetMd5(item.SalesManAccount.Split(',')[1]).ToUpper();
                            item.Agent = item.SalesManAccount.Split(',')[1];
                        }
                        else if (!string.IsNullOrWhiteSpace(item.SalesManAccount) && string.IsNullOrWhiteSpace(item.SalesManName))
                        {
                            item.OpenId = CommonHelper.GetMd5(item.SalesManAccount.Split(',')[1]).ToUpper();
                            item.Agent = item.SalesManAccount.Split(',')[1];
                        }
                        else
                        {
                            item.OpenId = CommonHelper.GetMd5(item.SalesManName.Split(',')[1]).ToUpper();
                            item.Agent = item.SalesManName.Split(',')[1];
                        }

                        //时间
                        item.DistributedTime = DateTime.Now;
                        //上级分配
                        item.IsDistributed = 3;
                    }
                    else if (string.IsNullOrWhiteSpace(item.SalesManAccount) && string.IsNullOrWhiteSpace(item.SalesManName))
                    {
                        //如果不修改，模型不可以修改数据，赋值为原先数据
                        item.OpenId = checkModel.OpenId;
                        item.Agent = checkModel.Agent;
                        //如果老数据未上传业务员
                        item.DistributedTime = checkModel.DistributedTime;
                        item.IsDistributed = checkModel.IsDistributed;
                    }

                    needUpdate.Add(item);
                    continue;
                }

                #region 光鹏杰使用  2018-04-28
                item.top_agent_id = agent;
                item.agent_id = childAgent; 
                #endregion

                needInsert.Add(item);
                //组合录入userinfo集合的函数
                InsertUserInfoListMethod(userInfoList, item, cityId, renewalCarType, agentId, isAuthorization);
            }

            if (upNewBuids.Any())
            {
                _batchRenewalRepository.UpdateIsNew(upNewBuids);
            }
            if (userInfoList.Any())
            {
                firstBuid = _batchRenewalRepository.BulkInsertUserInfo(userInfoList);
                if (firstBuid != 0)
                {
                    isSuccess = true;
                }
            }
            int isTimeSetting = 0;
            //是否配置
            LogHelper.Info("批量续保" + timeSetting.Count());
            isTimeSetting = IsConfiguration(timeSetting);
            if (needUpdate.Any())
            {
                // 操作数据库的方法
                isSuccess = OperationDataMethod(needUpdate, isTimeSetting, renewalCarType, upItemBuids, batchRenewalType);
            }
            else
            {
                isSuccess = true;
            }
            return isSuccess;
        }

        /// <summary>
        /// 操作数据库的方法
        /// </summary>
        /// <param name="needUpdate"></param>
        /// <param name="isTimeSetting"></param>
        /// <param name="renewalCarType"></param>
        /// <param name="upItemBuids"></param>
        /// <param name="batchRenewalType">0是需要续保的批次  1是不需要续保的批次</param>
        /// <returns></returns>
        public bool OperationDataMethod(List<BatchRenewalItemViewModel> needUpdate, int isTimeSetting, int renewalCarType, List<long> upItemBuids, int batchRenewalType)
        {
            bool isOperationScuess = false;
            //0是需要续保的批次  1是不需要续保的批次
            if (batchRenewalType == 1)
            {
                #region 不需要续保的批次
                var updateUserTimeInfoOnlyUp =
                           needUpdate.Where(x => !string.IsNullOrEmpty(x.Agent)).Select(x => new UpdateUserInfoTimeModel
                           {
                               Id = x.BuId,
                               CategoryInfoId = x.CategoryinfoId ?? 0,
                               RenewalType = renewType,
                               RenewalCarType = renewalCarType,
                               OpenId = x.OpenId,
                               Agent = x.Agent,
                               agent_id=int.Parse(x.Agent),//2018-09-07 同步agent_id和Agent两个字段
                               IsDistributed = x.IsDistributed,
                               //RegisterDate = x.RegisterDate,
                               //MoldName = x.MoldName,
                               IsBatchRenewalData = 1
                           }).ToList();
                if (updateUserTimeInfoOnlyUp.Any())
                {
                    isOperationScuess = _batchRenewalRepository.BulkUpdateUserInfo(updateUserTimeInfoOnlyUp.ToList()) != 0;
                }
                #endregion
            }
            else
            {
                //如果等于0 代表不在执行续保范围内
                if (isTimeSetting == 0)
                {
                    #region 不在执行续保范围内
                    var initUserInfoModelNew =
                                   needUpdate.Where(x => x.ItemStatus == 0 && !string.IsNullOrEmpty(x.Agent))
                                       .Select(x => new initUserInfoModelNew
                                       {
                                           Id = x.BuId,
                                           CategoryInfoId = x.CategoryinfoId ?? 0,
                                           RenewalType = renewType,
                                           RenewalCarType = renewalCarType,
                                           OpenId = x.OpenId,
                                           Agent = x.Agent,
                                           agent_id = int.Parse(x.Agent),//2018-09-07 同步agent_id和Agent两个字段
                                           IsDistributed = x.IsDistributed,
                                           DistributedTime = x.DistributedTime,
                                           //RegisterDate = x.RegisterDate,
                                           //MoldName = x.MoldName,
                                           IsBatchRenewalData = 1
                                       }).ToList();
                    if (initUserInfoModelNew.Any())
                    {
                        isOperationScuess = _batchRenewalRepository.InitUserInfoNew(initUserInfoModelNew.ToList()) != 0;
                    }
                    #endregion
                }
                else
                {
                    #region 续保失败的数据
                    var initUserInfoModel =
                                    needUpdate.Where(x => x.ItemStatus == 0 && !string.IsNullOrEmpty(x.Agent))
                                        .Select(x => new InitUserInfoModel
                                        {
                                            Id = x.BuId,
                                            CategoryInfoId = x.CategoryinfoId ?? 0,
                                            RenewalType = renewType,
                                            RenewalCarType = renewalCarType,
                                            OpenId = x.OpenId,
                                            Agent = x.Agent,
                                            agent_id = int.Parse(x.Agent),//2018-09-07 同步agent_id和Agent两个字段
                                            IsDistributed = x.IsDistributed,
                                            DistributedTime = x.DistributedTime,
                                            RegisterDate = x.RegisterDate,
                                            MoldName = x.MoldName,
                                            IsBatchRenewalData = 1,
                                            EngineNo = x.EngineNo,
                                            CarVIN = x.VinNo,
                                            LicenseNo = string.IsNullOrWhiteSpace(x.LicenseNo) ? x.VinNo : x.LicenseNo
                                        }).ToList();
                    if (initUserInfoModel.Any())
                    {
                        isOperationScuess = _batchRenewalRepository.InitUserInfo(initUserInfoModel.ToList()) != 0;
                    }
                    #endregion
                }
                #region 不需要续保的数据
                var updateUserTimeInfo =
                           needUpdate.Where(x => x.ItemStatus != 0 && !string.IsNullOrEmpty(x.Agent))
                               .Select(x => new UpdateUserInfoTimeModel
                               {

                                   Id = x.BuId,
                                   CategoryInfoId = x.CategoryinfoId ?? 0,
                                   RenewalType = renewType,
                                   RenewalCarType = renewalCarType,
                                   OpenId = x.OpenId,
                                   Agent = x.Agent,
                                   agent_id = int.Parse(x.Agent),//2018-09-07 同步agent_id和Agent两个字段
                                   IsDistributed = x.IsDistributed,
                                   //RegisterDate = x.RegisterDate,
                                   //MoldName = x.MoldName,
                                   IsBatchRenewalData = 1
                               }).ToList();
                if (updateUserTimeInfo.Any())
                {
                    isOperationScuess = _batchRenewalRepository.BulkUpdateUserInfo(updateUserTimeInfo.ToList()) != 0;
                }
                #endregion

            }

            //如果大于0 代表在执行续保范围内
            if (isTimeSetting > 0)
            {
                if (batchRenewalType != 0) return isOperationScuess;
            }
            else
            {
                isOperationScuess = true;
            }
            return isOperationScuess;
        }

        /// <summary>
        /// 组合userInfo的集合方法
        /// </summary>
        /// <param name="userInfoList"></param>
        /// <param name="item"></param>
        /// <param name="cityId"></param>
        /// <param name="renewalCarType"></param>
        /// <param name="agentId"></param>
        public void InsertUserInfoListMethod(List<UserInfoModel> userInfoList, BatchRenewalItemViewModel item, int cityId, int renewalCarType, string agentId, bool isAuthorization)
        {
            UserInfoModel userInfo = new UserInfoModel();
            userInfo.CityCode = cityId.ToString();
            userInfo.Source = -1;
            userInfo.LicenseNo = string.IsNullOrWhiteSpace(item.LicenseNo) ? item.VinNo : item.LicenseNo;
            userInfo.Mobile = StringHandleHelper.FilterBatchRenewalString(item.Mobile);
            userInfo.EngineNo = StringHandleHelper.FilterBatchRenewalString(item.EngineNo);
            userInfo.CarVIN = StringHandleHelper.FilterBatchRenewalString(item.VinNo);
            userInfo.MoldName = StringHandleHelper.FilterBatchRenewalString(item.MoldName);
            userInfo.CreateTime = DateTime.Now;
            userInfo.UpdateTime = DateTime.Now;
            userInfo.RegisterDate = item.RegisterDate;

            //如果存在分配的数据
            if (!string.IsNullOrWhiteSpace(item.SalesManAccount) || !string.IsNullOrWhiteSpace(item.SalesManName))
            {
                if (!string.IsNullOrWhiteSpace(item.SalesManAccount) && !string.IsNullOrWhiteSpace(item.SalesManName))
                {
                    userInfo.OpenId = CommonHelper.GetMd5(item.SalesManAccount.Split(',')[1]).ToUpper();
                    userInfo.Agent = item.SalesManAccount.Split(',')[1];
                }
                else if (!string.IsNullOrWhiteSpace(item.SalesManAccount) && string.IsNullOrWhiteSpace(item.SalesManName))
                {
                    userInfo.OpenId = CommonHelper.GetMd5(item.SalesManAccount.Split(',')[1]).ToUpper();
                    userInfo.Agent = item.SalesManAccount.Split(',')[1];
                }
                else
                {
                    userInfo.OpenId = CommonHelper.GetMd5(item.SalesManName.Split(',')[1]).ToUpper();
                    userInfo.Agent = item.SalesManName.Split(',')[1];
                }
                //时间
                userInfo.DistributedTime = DateTime.Now;
                //上级分配
                userInfo.IsDistributed = 3;
            }
            else if (string.IsNullOrWhiteSpace(item.SalesManAccount) && string.IsNullOrWhiteSpace(item.SalesManName))
            {
                userInfo.OpenId = CommonHelper.GetMd5(agentId).ToUpper();
                userInfo.Agent = agentId;
                if (!isAuthorization)
                {
                    userInfo.DistributedTime = DateTime.Now;
                    //上级分配
                    userInfo.IsDistributed = 3;
                }
            }

            /*如果有车架号，NeedEngineNo=0  反之，NeedEngineNo=1*/
            if (string.IsNullOrWhiteSpace(userInfo.CarVIN))
            {
                userInfo.NeedEngineNo = 1;
            }
            else
            {
                userInfo.NeedEngineNo = 0;
            }
            userInfo.RenewalType = 5;
            userInfo.LastYearSource = -1;
            userInfo.QuoteStatus = -1;
            //是否是批量续保数据
            userInfo.IsBatchRenewalData = 1;
            //增加客户类别
            userInfo.CategoryInfoId = Convert.ToInt32(item.CategoryInfo);
            //新增车牌类型
            userInfo.RenewalCarType = renewalCarType;
            //新增是否回访
            userInfo.IsReView = 0;
            //身份证后六位
            userInfo.SixDigitsAfterIdCard = string.IsNullOrWhiteSpace(item.SixDigitsAfterIdCard) ? "" :
                        item.SixDigitsAfterIdCard.Length > 6 ? item.SixDigitsAfterIdCard.Substring(item.SixDigitsAfterIdCard.Length - 6) : item.SixDigitsAfterIdCard;

            userInfo.agent_id = item.agent_id;
            userInfo.top_agent_id = item.top_agent_id;
            userInfoList.Add(userInfo);
        }
        /// <summary>
        /// 是否在续保时间段内
        /// </summary>
        /// <param name="timeSetting"></param>
        /// <returns></returns>
        public int IsConfiguration(List<string> timeSetting)
        {
            #region 时间配置
            int retuennum = 0;
            //动态根据时间执行
            for (int i = 0; i < timeSetting.Count; i++)
            {
                string firsttime = "";
                string sencondtime = "";
                firsttime = timeSetting[i].Split('-')[0].ToString();
                sencondtime = timeSetting[i].Split('-')[1].ToString();
                if (DateTime.Parse(DateTime.Now.ToShortDateString() + " " + firsttime) < DateTime.Now & DateTime.Now < DateTime.Parse(DateTime.Now.ToShortDateString() + " " + sencondtime))
                {
                    retuennum++;
                }
            }
            #endregion
            return retuennum;
        }

        /// <summary>
        /// ef-转-sql
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="batchRenewalItemViewModelList"></param>
        /// <param name="agentId"></param>
        /// <param name="errorDataCount"></param> 
        /// <returns></returns>
        public long InsertBatchRenewal(string fileName, string channelPattern, int batchRenewalItemViewModelListCount, int agentId, long errorDataCount, int TopAgentId, int cityId, int batchRenewalType, string filePath, int isDelete, bool isCompleted)
        {
            List<bx_batchrenewal> lbx_batch = new List<bx_batchrenewal>();
            bx_batchrenewal batchRenewal = new bx_batchrenewal();
            batchRenewal.AgentId = agentId;
            batchRenewal.CreateTime = DateTime.Now;
            batchRenewal.FileName = fileName;
            batchRenewal.IsCompleted = isCompleted;
            batchRenewal.StartExecuteTime = DateTime.Now;
            batchRenewal.TopAgentId = TopAgentId;
            batchRenewal.TotalCount = batchRenewalItemViewModelListCount;
            //如果是只上传不续保返回0
            batchRenewal.UntreatedCount = batchRenewalType == 1 ? 0 : batchRenewalItemViewModelListCount;
            batchRenewal.ErrorDataCount = errorDataCount;
            batchRenewal.IsDistributed = false;
            batchRenewal.ChannelPattern = channelPattern;
            batchRenewal.CityId = cityId;
            batchRenewal.BatchRenewalType = batchRenewalType;
            batchRenewal.FilePath = filePath;
            batchRenewal.IsDelete = isDelete;
            lbx_batch.Add(batchRenewal);
            return _batchRenewalRepository.InsertBatchRenewal(lbx_batch);
        }

        /// <summary>
        //批量插入Item表 、QuotereqCarinfo表 、UserRenewalInfo表维护
        /// </summary>
        /// <param name="needUpdate"></param>
        /// <param name="batchRenewalId"></param>
        /// <param name="agentId"></param>
        /// <param name="firstBuid"></param>
        /// <returns></returns>
        public bool BulkMaintainBatchRenewalItem(List<BatchRenewalItemViewModel> needInsert, List<BatchRenewalItemViewModel> lsItemModels, long batchRenewalId, string agentId, long firstBuid, int batchRenewalType)
        {

            var batchQuotereqCarinfoList = new List<BatchQuotereqCarinfoModel>();
            var batchrenewalItemList = new List<BatchRenewalItemModel>();
            var batchrenewalItemNewList = new List<BatchRenewalItemModelNew>();
            lsItemModels.AddRange(needInsert);
            if (lsItemModels.Any())
            {
                foreach (var item in lsItemModels)
                {
                    var batchQuotereqCarinfo = new BatchQuotereqCarinfoModel();
                    var batchrenewalItem = new BatchRenewalItemModel();
                    var batchrenewalItemnew = new BatchRenewalItemModelNew();
                    batchrenewalItem.BatchId = batchRenewalId;
                    long buidNew = firstBuid;
                    if (item.BuId == 0)
                    {
                        //数据库自增数，线下是2 线上是1 每次上线时必须更改配置
                        firstBuid = firstBuid + dataGrowNum;
                    }
                    //如果Buid>0  证明是数据修改
                    batchrenewalItem.BUId = item.BuId > 0 ? item.BuId : buidNew;
                    batchrenewalItem.InitBuId = batchrenewalItem.BUId;
                    batchrenewalItem.CreateTime = DateTime.Now;
                    batchrenewalItem.EngineNo = item.EngineNo;
                    //是否最新
                    batchrenewalItem.IsNew = 1;
                    if (item.BuId > 0)
                    {
                        if (batchRenewalType == 0)
                        {
                            batchrenewalItem.ItemStatus = item.ItemStatus == 0 ? -1 : item.ItemStatus;
                        }
                        else
                        {
                            batchrenewalItem.ItemStatus = 5;

                        }

                    }
                    else
                    {
                        if (batchRenewalType == 1)
                        {
                            //5代表的是 只上传不批续的数据
                            batchrenewalItem.ItemStatus = 5;
                        }
                        else
                        {
                            batchrenewalItem.ItemStatus = -1;
                        }

                    }
                    //历史状态
                    batchrenewalItem.HistoryItemStatus = batchrenewalItem.ItemStatus;
                    //  batchrenewalItem.ItemStatus = 0;
                    batchrenewalItem.LicenseNo = string.IsNullOrWhiteSpace(item.LicenseNo) ? null : item.LicenseNo.Trim();
                    batchrenewalItem.Mobile = StringHandleHelper.FilterBatchRenewalString(item.Mobile);
                    batchrenewalItem.CustomerName = StringHandleHelper.FilterBatchRenewalString(item.CustomerName);
                    batchrenewalItem.UpdateTime = DateTime.Now;
                    batchrenewalItem.VinNo = StringHandleHelper.FilterBatchRenewalString(item.VinNo);
                    batchrenewalItem.MoldName = StringHandleHelper.FilterBatchRenewalString(item.MoldName);
                    if (!string.IsNullOrEmpty(item.LastYearSource))
                    {
                        batchrenewalItem.LastYearSource = Convert.ToInt32(item.LastYearSource);
                    }
                    else
                    {
                        batchrenewalItem.LastYearSource = -1;
                    }
                    if (item.ForceEndDate != null)
                    {
                        batchrenewalItem.ForceEndDate = item.ForceEndDate.Value;
                    }
                    else
                    {
                        batchrenewalItem.ForceEndDate = Convert.ToDateTime("1900-01-01 01:01:01");
                    }

                    if (item.BizEndDate != null)
                    {
                        batchrenewalItem.BizEndDate = item.BizEndDate.Value;
                    }
                    else
                    {
                        batchrenewalItem.BizEndDate = Convert.ToDateTime("1900-01-01 01:01:01");
                    }

                    batchrenewalItem.RegisterDate = item.RegisterDate;


                    batchrenewalItem.SalesManAccount = !string.IsNullOrEmpty(item.SalesManAccount) ? item.SalesManAccount.Split(',')[0] : "";
                    batchrenewalItem.SalesManName = !string.IsNullOrEmpty(item.SalesManName) ? item.SalesManName.Split(',')[0] : "";
                    batchrenewalItem.SixDigitsAfterIdCard = string.IsNullOrWhiteSpace(item.SixDigitsAfterIdCard) ? "" :
                        item.SixDigitsAfterIdCard.Length > 6 ? item.SixDigitsAfterIdCard.Substring(item.SixDigitsAfterIdCard.Length - 6) : item.SixDigitsAfterIdCard;
                    batchrenewalItemList.Add(batchrenewalItem);
                    //增加新model
                    //如果Buid>0  证明是数据修改
                    batchrenewalItemnew.BUId = item.BuId > 0 ? item.BuId : buidNew;
                    batchrenewalItemnew.Mobile = StringHandleHelper.FilterBatchRenewalString(item.Mobile);
                    batchrenewalItemnew.CustomerName = StringHandleHelper.FilterBatchRenewalString(item.CustomerName);
                    batchrenewalItemnew.remark = StringHandleHelper.FilterBatchRenewalString(item.Remark);
                    //备注2
                    batchrenewalItemnew.Intention_Remark = StringHandleHelper.FilterBatchRenewalString(item.Intention_Remark);
                    batchrenewalItemnew.UpdateTime = DateTime.Now;
                    batchrenewalItemnew.client_mobile_other = StringHandleHelper.FilterBatchRenewalString(item.Client_mobile_other);
                    batchrenewalItemNewList.Add(batchrenewalItemnew);
                    batchQuotereqCarinfo.b_uid = batchrenewalItem.BUId;
                    batchQuotereqCarinfo.licenseNo = StringHandleHelper.FilterBatchRenewalString(batchrenewalItem.LicenseNo);
                    batchQuotereqCarinfo.is_lastnewcar = item.IsLastYearNewCar;
                    batchQuotereqCarinfo.create_time = DateTime.Now;
                    batchQuotereqCarinfo.update_time = DateTime.Now;
                    batchQuotereqCarinfoList.Add(batchQuotereqCarinfo);
                }
            }
            // QuotereqCarinfo表 UserRenewalInfo表、Item表维护    
            CommonMethod(batchQuotereqCarinfoList, batchrenewalItemList, batchrenewalItemNewList);
            return true;
        }

        #region 共通方法
        /// <summary>
        /// QuotereqCarinfo表 UserRenewalInfo表维护
        /// </summary>
        /// <param name="batchQuotereqCarinfoList"></param>
        /// <param name="batchrenewalItemList"></param>
        /// <returns></returns>
        public int CommonMethod(List<BatchQuotereqCarinfoModel> batchQuotereqCarinfoList, List<BatchRenewalItemModel> batchrenewalItemList, List<BatchRenewalItemModelNew> batchrenewalItemNewList)
        {

            //测试
            batchrenewalItemList = batchrenewalItemList.OrderByDescending(x => x.UpdateTime).DistinctBy(x => x.BUId).ToList();
            batchrenewalItemNewList = batchrenewalItemNewList.OrderByDescending(x => x.UpdateTime).DistinctBy(x => x.BUId).ToList();
            var hasBuids = _batchRenewalRepository.CheckQuotereqCarinfo(batchQuotereqCarinfoList.Select(x => x.b_uid).ToList());
            var updateQuotereqCarinfoList = batchQuotereqCarinfoList.Where(x => hasBuids.Contains(x.b_uid)).ToList();

            var insertQuotereqCarinfoList = batchQuotereqCarinfoList.Except(updateQuotereqCarinfoList).ToList();
            if (insertQuotereqCarinfoList.Any())
            {
                insertQuotereqCarinfoList.ForEach(o =>
                {
                    o.is_lastnewcar = string.IsNullOrWhiteSpace(o.licenseNo) ? 2 : 0;
                });
                _batchRenewalRepository.BatchInsertQuotereqCarinfo(insertQuotereqCarinfoList);
            }
            if (updateQuotereqCarinfoList.Any())
            {
                _batchRenewalRepository.BatchUpdateQuotereqCarinfo(updateQuotereqCarinfoList);
            }
            var hasUserRenewalInfoBuids = _batchRenewalRepository.CheckUserRenewalInfo(batchrenewalItemNewList.Select(x => x.BUId).ToList());
            var updateUserRenewalInfoList = batchrenewalItemNewList.Where(x => hasUserRenewalInfoBuids.Contains(x.BUId)).ToList();
            var insertUserRenewalInfoList = batchrenewalItemNewList.Except(updateUserRenewalInfoList).ToList();
            if (insertUserRenewalInfoList.Any())
            {
                var userRenewalInfoModels = insertUserRenewalInfoList.Select(x => new UserRenewalInfoModel { b_uid = x.BUId, client_mobile = x.Mobile, client_name = x.CustomerName, client_mobile_other = x.client_mobile_other, remark = x.remark, create_time = DateTime.Now, Intention_Remark = x.Intention_Remark }).ToList();
                _batchRenewalRepository.BulkInsertUserRenewalInfo(userRenewalInfoModels);
            }
            if (updateUserRenewalInfoList.Any())
            {
                var userRenewalInfoModels = updateUserRenewalInfoList.Select(x => new UserRenewalInfoModel { b_uid = x.BUId, client_mobile = x.Mobile, client_name = x.CustomerName, client_mobile_other = x.client_mobile_other, remark = x.remark, create_time = DateTime.Now, Intention_Remark = x.Intention_Remark }).ToList();
                _batchRenewalRepository.BatchUpdateUserRenewalInfo(userRenewalInfoModels);
            }
            //var batchRenewalData=  batchrenewalItemList.Select(x => new { BatchId = x.BatchId, CreateTime = x.CreateTime, EngineNo = x.EngineNo, ItemStatus = x.ItemStatus, LicenseNo = x.LicenseNo, Mobile = x.Mobile, MoldName = x.MoldName, RegisterDate = x.RegisterDate, UpdateTime = x.UpdateTime, VinNo = x.VinNo, CustomerName = x.CustomerName, InitBuId = x.InitBuId }).ToList();
            return _batchRenewalRepository.BulkInsertBatchRenewalItem(batchrenewalItemList);
        }
        #endregion

        public bool UpdateBatchRenewal(long batchRenewalId)
        {
            return _batchRenewalRepository.UpdateBatchRenewal(batchRenewalId);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="BatchrenewalIdList"></param>
        /// <returns></returns>
        public bool DeteleRenewalData(List<int> BatchrenewalIdList)
        {

            return _batchRenewalRepository.DeteleRenewalData(BatchrenewalIdList);
        }
        /// <summary>
        /// 删除模板-预留接口传递泛型也许会批量修改
        /// </summary>
        /// <param name="BatchrenewalIdList"></param>
        /// <returns></returns>
        public bool DeleteBatchRenewal(List<int> BatchrenewalIdList)
        {

            return _batchRenewalRepository.DeleteBatchRenewal(BatchrenewalIdList);

        }

        /// <summary>
        /// 根据Buid删除bx_batchrenewal_item表
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        public bool DeleteBatchRenewalItem(int buId)
        {
            return _batchRenewalRepository.DeleteBatchRenewalItem(buId);
        }
        /// <summary>
        /// 根据Buid恢复bx_batchrenewal_item表
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        public bool RevertBatchRenewalItem(int buId)
        {
            return _batchRenewalRepository.RevertBatchRenewalItem(buId);
        }
        /// <summary>
        /// 重查批量续保
        /// </summary>
        /// <param name="BatchId"></param>
        /// <returns></returns>
        public bool AnewBatchRenewal(long BatchId, int operateType, ChannelPatternModel channelPattern)
        {
            return _batchRenewalRepository.AnewBatchRenewal(BatchId, operateType, channelPattern);
        }

        /// <summary>
        /// 获取批量续保选择续保城市接口
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public IList<BatchRenewalSource> GetBatchRenewalSource(int cityId, string agentId)
        {

            return _batchRenewalRepository.GetBatchRenewalSource(cityId, agentId);
        }

        public IList<bx_batchrenewal_erroritem> GetBatchRenewalErrorDataByBathId(long batchId)
        {
            return _batchRenewalRepository.GetBatchRenewalErrorDataByBathId(batchId);
        }

        /// <summary>
        /// 获取批量续保选择续保城市接口
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public string GetBatchRenewalQueueTime(int BatchId)
        {

            return _batchRenewalRepository.GetBatchRenewalQueueTime(BatchId);

        }

        public int GetAgentBatchRenewalCount(int agentId)
        {
            // agentId = Convert.ToInt32(_commonService.GetTopAgent(agentId));
            return _batchRenewalRepository.GetAgentBatchRenewalCount(agentId);
        }
        public int GetSettedCount(int agentId)
        {
            return _batchRenewalRepository.GetSettedCount(agentId);
        }


        public List<bx_batchrenewal_erroritem> GetBatchRenewalErrorItem(long batchId, int pageIndex, int pageSize, out int totalCount)
        {
            return _batchRenewalRepository.GetBatchRenewalErrorItem(batchId, pageIndex, pageSize, out totalCount);
        }
        public IList<BatchRenewalViewModel> GetBatchRenewalList(BatchRenewalListRequest listRequest, List<bx_agent> agentInfos, out int totalCount)
        {
            return _batchRenewalRepository.GetBatchRenewalList(listRequest, agentInfos, out totalCount);
        }

        /// <summary>
        /// 查询所有客户类别信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<bx_customercategories> SelectCategories(int agentId)
        {
            var repetitionDate = _batchRenewalRepository.SelectCategories(agentId);
            return repetitionDate;
        }


        public bool TaskUpdateCount(List<long> batchRenewalIdList)
        {
            return _batchRenewalRepository.TaskUpdateCount(batchRenewalIdList);
        }
        public List<int> GetSonsList(int currentAgent)
        {
            return _batchRenewalRepository.GetSonsList(currentAgent);
        }


        /// <summary>
        /// 根据查询保司id、投保地区、代理人查询相应渠道是否存在
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="city"></param>
        /// <param name="agnetId"></param>
        /// <returns></returns>
        public bool ExitSources(List<int> sources, int city, long agnetId,out string message)
        {
            var exitResult = false;
            message = string.Empty;
            if (sources == null || sources.Count == 0)
            {
                message = "请选择查询保司";
                return exitResult;
            } 
            var result = _batchRenewalRepository.GetBatchRenewalSource(city, agnetId.ToString());
            if (result == null || !result.Any())
            {
                var lstSource = new List<BatchRenewalSource>()
                    {
                        new BatchRenewalSource {Id = 3, isuse = 0, SourceName = "中国人寿财险"},
                        new BatchRenewalSource {Id = 4, isuse = 0, SourceName = "中华联合车险"}
                    };
                result = lstSource;
            }
            else
            {
                if (result.Where(x => x.Id == 4).Count() == 0)
                {
                    BatchRenewalSource batchRenewal = new BatchRenewalSource()
                    {
                        Id = 4,
                        isuse = 0,
                        SourceName = "中华联合车险"
                    };
                    result.Add(batchRenewal);
                }
                if (result.Where(x => x.Id == 3).Count() == 0)
                {
                    BatchRenewalSource batchRenewal = new BatchRenewalSource()
                    {
                        Id = 3,
                        isuse = 0,
                        SourceName = "中国人寿财险"
                    };
                    result.Add(batchRenewal);
                }
            }

            var notList = new List<int>();
            for (int i = 0; i < sources.Count; i++)
            {
                if (result.Where(o => o.Id == sources[i]).Count() == 0)
                    notList.Add(sources[i]);
            }

            if (notList != null && notList.Count > 0)
            {
                var select = _companyrelationRepository.GetCompany(notList);
                if (select != null && select.Count > 0)
                {
                    var companyNameList = select.Select(o => o.name);
                    message = "保司" + string.Join("、", companyNameList) + "未查询到渠道，请重新选择";
                }
            }
            else
            {
                exitResult = true;
            }
            return exitResult;
        }
    }
}
