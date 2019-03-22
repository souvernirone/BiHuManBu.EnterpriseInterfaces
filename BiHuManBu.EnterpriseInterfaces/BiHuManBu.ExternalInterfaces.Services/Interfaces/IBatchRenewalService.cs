
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IBatchRenewalService
    {
        //    int UpdateBatchRenewalItem(List<UpdateBatchRenewalItemModel> itemModels);
        /// <summary>
        ///  中心更改item表状态接口
        /// </summary>
        /// <param name="Buid">需要更新的BUID</param>
        /// <param name="ItemStatus">状态</param>
        /// <returns></returns>
        int UpdateBatchRenewalItem(int Buid, int ItemStatus);
        /// <summary>
        /// 重置状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ResettinStatus(long id);
        /// <summary>
        /// 根据Buid修改批量续保子表状态
        /// </summary>
        /// <param name="Buid">bx_userino.id</param>
        /// <param name="ItemStatus">1：处理成功（车辆信息，险种全部取回） 2：失败  4：处理成功（有车辆信息未获取到险种）</param>
        /// <returns>修改影响行数</returns>
        int UpdateItemStatus(int buId, int itemStatus);
        /// <summary>
        /// 执行批量续保遗留的老数据
        /// </summary>
        /// <returns></returns>

        bool ExcuteOldBatchrenewalData();
        /// <summary>
        /// 根据buid获取item
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        bx_batchrenewal_item GetItemByBuId(long buId);


        /// <summary>
        /// 上传主表
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="channelPattern"></param>
        /// <param name="batchRenewalItemViewModelList"></param>
        /// <param name="agentId"></param>
        /// <param name="errorDataCountt"></param>
        /// <param name="TopAgentId"></param>
        /// <param name="cityId"></param>
        /// <returns></returns>
        long InsertBatchRenewal(string fileName, string channelPattern, int batchRenewalItemViewModelListCount, int agentId, long errorDataCount, int TopAgentId, int cityId, int batchRenewalType, string filePath, int isDelete, bool isCompleted);

        IList<CheckBackModel> CheckUserInfo(List<BatchRenewalItemViewModel> batchRenewalItems, string agentId, int renewalCarType, int topagentId);


        /// <summary>
        /// 批量修改BatchRenewalItem
        /// </summary>
        /// <param name="needInsert"></param>
        /// <param name="lsItemModels"></param>
        /// <param name="batchRenewalId"></param>
        /// <param name="agentId"></param>
        /// <param name="firstBuid"></param>
        /// <returns></returns>
        bool BulkMaintainBatchRenewalItem(List<BatchRenewalItemViewModel> needInsert, List<BatchRenewalItemViewModel> lsItemModels, long batchRenewalId, string agentId, long firstBuid,int batchRenewalType);
        /// <summary>
        /// 批量插入未通过数据
        /// </summary>
        /// <param name="excelErrorDataList"></param>
        /// <param name="batchId"></param>
        /// <returns></returns>
        bool BulkInsertBatchRenewaErrorlItem(List<ExcelErrorData> excelErrorDataList, long batchId);
        /// <summary>
        /// 根据批次编号获得批次数据集合
        /// </summary>
        /// <param name="batchRenewalId"></param>
        /// <returns></returns>
        /// <summary>
        /// 根据批次编号更新此批次状态
        /// </summary>
        /// <returns></returns>X
        bool UpdateBatchRenewal(long batchRenewalId);
        /// <summary>
        /// 查询客户类别
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        List<bx_customercategories> SelectCategories(int agentId);
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="BatchrenewalIdList"></param>
        /// <returns></returns>
        bool DeteleRenewalData(List<int> BatchrenewalIdList);
        /// <summary>
        /// 获得已设置但尚未处理的条数
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        int GetSettedCount(int agentId);
        /// <summary>
        /// 根据经纪人编号获取上传列表
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        IList<BatchRenewalViewModel> GetBatchRenewalList(BatchRenewalListRequest listRequest,List<bx_agent> agentInfos, out int totalCount);
        /// <summary>
        /// 更新总表中成功和失败条数
        /// </summary>
        bool TaskUpdateCount(List<long> batchRenewalIdList);
        /// <summary>
        /// 获取代理子集
        /// </summary>
        /// <param name="currentAgent"></param>
        /// <returns></returns>
        List<int> GetSonsList(int currentAgent);
        /// <summary>
        /// 根据批次编号获取未能上传的数据
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        List<bx_batchrenewal_erroritem> GetBatchRenewalErrorItem(long batchId, int pageIndex, int pageSize, out int totalCount);
        /// <summary>
        /// 获得当前业务员可上传条数
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        int GetAgentBatchRenewalCount(int agentId);


        /// <summary>
        /// 批量插入userInfo
        /// </summary>
        /// <param name="batchRenewalItems"></param>
        /// <param name="agentId"></param>
        /// <param name="agent"></param>
        /// <param name="childAgent"></param>
        /// <param name="needInsert"></param>
        /// <param name="needUpdate"></param>
        /// <param name="cityId"></param>
        /// <param name="renewalCarType"></param>
        /// <param name="checkUserModels"></param>
        /// <param name="timeSetting"></param>
        /// <param name="isAuthorization"></param>
        /// <param name="batchRenewalType"></param>
        /// <param name="firstBuid"></param>
        /// <returns></returns>
        bool BulkInsertUserInfo(List<BatchRenewalItemViewModel> batchRenewalItems, string agentId, int agent,
            int childAgent, List<BatchRenewalItemViewModel> needInsert, List<BatchRenewalItemViewModel> needUpdate,
            int cityId, int renewalCarType, IList<CheckBackModel> checkUserModels, List<string> timeSetting,
            bool isAuthorization, int batchRenewalType, out long firstBuid);

        /// <summary>
        /// 获取续保时间段
        /// </summary>
        /// <returns></returns>
        List<string> GetTimeSetting();

        /// <summary>
        /// 根据批次编号获得需要下载数据
        /// </summary>
        /// <param name="batchRenewalId"></param>
        /// <returns></returns>
        IList<DownLoadExcel> GetBatchRenewalTable(long batchRenewalId);
        //#endregion
        /// <summary>
        /// 根据批次编号获得文件名
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        string GetFileNameByBatchId(long batchId);

        /// <summary>
        /// 重查批量续保 
        /// </summary>
        /// <param name="BatchId"></param>
        /// <returns></returns>
        bool AnewBatchRenewal(long BatchId, int operateType, ChannelPatternModel channelPattern);
        /// <summary>
        /// 删除模板-预留传递泛型也许会批量修改
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

        IList<bx_batchrenewal_erroritem> GetBatchRenewalErrorDataByBathId(long batchId);

        IList<BatchRenewalSource> GetBatchRenewalSource(int cityId, string agentId);
        /// <summary>
        /// 获取批量续保排队剩余时间
        /// </summary>
        /// <returns></returns>
        string GetBatchRenewalQueueTime(int BatchId);

        bool UpdateHistoryStatus(int BatchrenewalId, out List<BatchRenewalUserInfoModel> needUpdateStatus);
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="BatchrenewalIdList"></param>
        /// <returns></returns>
        bool SelectBatchrenewal(List<int> BatchrenewalIdList);

        List<BatchRenewalSource> GetCacheBatchRenewalSource(int cityId);

        bool ExitSources(List<int> sources, int city, long agnetId, out string message);

    }
}
