using BiHuManBu.ExternalInterfaces.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models
{
    public interface IBatchRenewalRepository
    {
        /// <summary>
        /// 重置状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ResettinStatus(long id);
        /// <summary>
        /// 执行批量续保遗留的老数据
        /// </summary>
        /// <returns></returns>

        bool ExcuteOldBatchrenewalData();

        int UpdateBatchRenewalItem(int buId, int itemStatus);
        int UpdateItemStatus(int buId, int itemStatus);
        /// <summary>
        /// 根据buid获取item
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        bx_batchrenewal_item GetItemByBuId(long buId);
        IList<CheckBackModel> CheckUserInfo(List<BatchRenewalItemViewModel> batchRenewalItemModelList, string agentId, int renewalCarType, int topagentId);
        /// <summary>
        /// 获取代理子集
        /// </summary>
        /// <param name="currentAgent"></param>
        /// <returns></returns>
        List<int> GetSonsList(int currentAgent);
        /// <summary>
        /// 批量Buid修改批量续保子表状态
        /// </summary>
        /// <param name="Buid"></param>
        /// <returns></returns>
        bool UpdateIsNew(List<long> Buid);
        /// <summary>
        /// 批量插入userInfo
        /// </summary>
        /// <param name="insertSql"></param>
        /// <returns></returns>b
        int BulkInsertUserInfo(List<UserInfoModel> userInfoes);
        /// <summary>
        /// 批量插入未通过数据
        /// </summary>
        /// <param name="excelErrorDataList"></param>
        /// <param name="batchId"></param>
        /// <returns></returns>
        bool BulkInsertBatchRenewaErrorlItem(List<bx_batchrenewal_erroritem> batchRenewalErrorItemList);
        /// <summary>
        /// 根据批次编号获得需要下载数据
        /// </summary>
        /// <param name="batchRenewalId"></param>
        /// <returns></returns>
        IList<DownLoadExcel> GetBatchRenewalTable(long batchRenewalId);
        /// <summary>
        /// 根据批次编号获得文件名
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        string GetFileNameByBatchId(long batchId);
        List<BatchRenewalSource> GetCacheBatchRenewalSource(int cityId);
        /// <summary>
        /// 获取续保时间段
        /// </summary>
        /// <returns></returns>
        List<string> GetTimeSetting();
        /// <returns></returns>
    
        /// <summary>
        /// 批量更新userinfo中NeedEngineNo,LastYearSource,RenewalStatus
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        int InitUserInfo(List<InitUserInfoModel> updateUserinfoModelList);
        int InitUserInfoNew(List<initUserInfoModelNew> updateUserinfoModelList);
        int BulkUpdateUserInfo(List<UpdateUserInfoTimeModel> updateUserInfoModel);
    
        /// <summary>
        /// 批量Buid修改批量续保子表状态
        /// </summary>
        /// <param name="Buid"></param>
        /// <returns></returns>
        bool UpdateItemStatus(List<long> Buid);

        /// <summary>
        /// 保存上传续保信息EF转SQL
        /// </summary>
        /// <param name="batchRenewalViewModel"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        long InsertBatchRenewal(List<bx_batchrenewal> batchRenewalItemViewModelList);

        /// <summary>
        /// 批量插入QuotereqCarinfo
        /// </summary>
        /// <param name="batchQuotereqCarinfoModels"></param>
        /// <returns></returns>
        int BatchInsertQuotereqCarinfo(List<BatchQuotereqCarinfoModel> batchQuotereqCarinfoModels);
        /// <summary>
        /// 批量更新QuotereqCarinfo
        /// </summary>
        /// <param name="batchQuotereqCarinfoModels"></param>
        /// <returns></returns>
        int BatchUpdateQuotereqCarinfo(List<BatchQuotereqCarinfoModel> batchQuotereqCarinfoModels);

        /// <summary>
        /// 批量更新UserRenewalInfo
        /// </summary>
        /// <param name="userRenewalInfoModels"></param>
        /// <returns></returns>
        int BatchUpdateUserRenewalInfo(List<UserRenewalInfoModel> userRenewalInfoModels);
        /// <summary>
        /// 检查QuotereqCarinfo是否存在
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<long> CheckQuotereqCarinfo(List<long> buids);
        /// <summary>
        /// 检查客户信息是否存在
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<long> CheckUserRenewalInfo(List<long> buids);

        /// <summary>
        /// 批量插入bx_userinfo_renewalinfo
        /// </summary>
        /// <param name="userRenewalInfoModels"></param>
        /// <returns></returns>
        int BulkInsertUserRenewalInfo(List<UserRenewalInfoModel> userRenewalInfoModels);
        /// <summary>
        /// 批量插入BatchRenewalItem
        /// </summary>
        /// <param name="insertSql"></param>
        /// <returns></returns>
        int BulkInsertBatchRenewalItem(List<BatchRenewalItemModel> batchRenewalItems);
        /// <summary>
        /// 根据批次编号更新此批次状态
        /// </summary>
        /// <returns></returns>
        bool UpdateBatchRenewal(long batchRenewalId);

        bool UpdateHistoryStatus(int BatchrenewalId, out List<BatchRenewalUserInfoModel> needUpdateStatus);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="BatchrenewalIdList"></param>
        /// <returns></returns>
        bool DeteleRenewalData(List<int> BatchrenewalIdList);
        /// <summary>
        /// 删除模板-预留接口传递泛型也许会批量修改
        /// </summary>
        /// <param name="BatchrenewalIdList"></param>
        /// <returns></returns>
        bool DeleteBatchRenewal(List<int> BatchrenewalIdList);
        /// <summary>
        /// 根据Buid删除bx_batchrenewal_item表
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        bool DeleteBatchRenewalItem(int buId);
        /// <summary>
        /// 根据Buid恢复bx_batchrenewal_item表
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        bool RevertBatchRenewalItem(int buId);

        /// <summary>
        /// 根据buid集合恢复bx_batchrenewal_item表
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        bool RevertBatchRenewalItemByBuIdList(List<long> buId);

        /// <summary>
        /// 重查批量续保
        /// </summary>
        /// <param name="BatchId"></param>
        /// <returns></returns>
        bool AnewBatchRenewal(long BatchId, int operateType, ChannelPatternModel channelPattern);
        /// <summary>
        /// 获取批量续保选择续保城市接口
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        IList<BatchRenewalSource> GetBatchRenewalSource(int cityId, string agentId);
        string GetBatchRenewalQueueTime(int BatchId);
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="BatchrenewalIdList"></param>
        /// <returns></returns>
        bool SelectBatchrenewal(List<int> BatchrenewalIdList);
        /// <summary>
        /// 获得当前业务员可上传条数
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        int GetAgentBatchRenewalCount(int agentId);
        /// <summary>
        /// 获得已设置但尚未处理的条数
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        int GetSettedCount(int agentId);
        /// <summary>
        /// 根据批次编号获取未能上传的数据
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        List<bx_batchrenewal_erroritem> GetBatchRenewalErrorItem(long batchId, int pageIndex, int pageSize, out int totalCount);


        /// <summary>
        /// 根据批次id获取错误信息
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        IList<bx_batchrenewal_erroritem> GetBatchRenewalErrorDataByBathId(long batchId);


        /// <summary>
        /// 查询客户类别
        /// </summary>
        /// 
        /// <returns></returns>
        List<bx_customercategories> SelectCategories(int agentId);
        /// <summary>
        /// 根据经纪人编号获取上传列表
        /// </summary>
        /// <param name="listRequest"></param>
        /// <returns></returns>
        IList<BatchRenewalViewModel> GetBatchRenewalList(BatchRenewalListRequest listRequest, List<bx_agent> agentInfos, out int totalCount);
        //#region 定时任务
        /// <summary>
        /// 更新总表中成功和失败条数
        /// </summary>
        bool TaskUpdateCount(List<long> batchRenewalIdList);

        /// <summary>
        /// 根据批次id获取buid，只判断IsDelete，ItemStatus，其他不判断
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        List<long> GetBuidByBatchId(long batchId, int isDelete = 0);


        /// <summary>
        /// 获取批续主表个数
        /// </summary>
        /// <returns></returns>
       int GetBatchRenewalCompleteCount();

        bool RefreshBatchRenewalStatistics();

        bool UpdateBatchRenewalItemByBuid(int buid, long source, string ForceEndTime, string BizEndTime);

    }
}
