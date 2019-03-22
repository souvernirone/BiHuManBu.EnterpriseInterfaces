using System;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{

    /// <summary>
    /// 顶级客户列表页面相关接口定义
    /// houxianjun
    /// </summary>
    public interface ICustomerBusinessRepository
    {
        /// <summary>
        /// 请求业务员列表方法
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <returns></returns>
        List<CTopLevelAgentViewModel> GetBusinessAgentList(int currentAgentId);

        /// <summary>
        /// 获取可分配代理人集合信息
        /// </summary>
        /// <param name="agentIds">业务员id集合</param>
        /// <returns></returns>
        List<AgentDistributedViewModel> GetDistributAgentList(List<int> agentIds);
        // public List<int> GetAgentIds(int currentAgentId);
        /// <summary>
        /// 批量更新需要分配的车辆信息
        /// </summary>
        /// <param name="userInfoModels"></param>
        /// <returns></returns>
        Boolean BulkUpdateByList(List<UpdateUserInfoModel> userInfoModels);

        /// <summary>
        /// 已有数据更新Istest=1
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        int UpdateIstest(string buids);
        /// <summary>
        /// 更新是否分配
        /// </summary>
        /// <param name="bathcId"></param>
        /// <returns></returns>
        bool UpdateIsDistributeByBatchId(long bathcId);
        /// <summary>
        /// 检查是否全部分发完成
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        bool IsAllDistributed(long batchId);

        List<BxUserinfoViewModel> GetUserinfosForBuids(List<long> buidsList);

        List<DistributeUserinfoDto> GetUserinfosForagents(List<string> agentList);

        IEnumerable<BxUserinfoRenewalViewModel> GetBxUserinfoRenewal(List<long> buidsList);

        int InsertBxMessage(bx_message bxMessages);

        List<BxMessageViewModel> GetBxMessages(int agentId);
        int UpdateBxMessage(string messageIds);

        /// <summary>
        /// 回收功能更新bx_userinfo
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="toAgent">收据回收到的代理人</param>
        /// <param name="openid"></param>
        /// <param name="isDistributed">bx_userinfo.IsDistributed</param>
        /// <returns></returns>
        int UpdateDistributeRecycle(string buids, int toAgent, string openid,int isDistributed);
        /// <summary>
        /// 不需要更新批续的buid集合
        /// </summary>
        /// <param name="updateBratch"></param>
        /// <param name="noBuidList"></param>
        /// <returns></returns>
        int UpdatebratchItem(Dictionary<long, long> updateBratch, List<long> noBuidList);
        List<long> GetBatchRenewalItemBuidList(Dictionary<long, long> updateBratch);
        /// <summary>
        /// 判断是否存在没有续保完成的数据（续保中、排队中）
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        List<bx_batchrefreshrenewal> GetUnfinishBatchRefreshRenewal(int topAgentId, int childAgent);
        int GetTodayBatchRefRenewal(int topAgentId, int childAgent);

        List<bx_batchrefreshrenewal> GetTodayBatchRefRenewalList(int topAgentId, int childAgent);
        /// <summary>
        /// 判断今天是否存在数据，如果存在则更新
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        List<long> GetAndUpdateBatchRefreshRenewal(List<long> buids, int topAgentId, int agentId, string date);
        List<bx_batchrefreshrenewal> GetBatchRefreshRenewalByAllBuid(List<long> buids, int topAgentId, int agentId, string date);
        bx_batchrefreshrenewal BatchRefreshRenewalDetail(int topAgentId, int operateAgent, long Buid);
        /// <summary>
        /// 将批量刷新续保数据添加到BatchRefreshRenewal表
        /// </summary>
        /// <param name="userList"></param>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool AddBatchRefreshRenewal(List<bx_userinfo> userList, int topAgentId, int childAgent, string date);

        List<BatchRefreshRenewalModel> GetBatchRefreshRenewalList(List<long> buids, int topAgentId, int childAgent);
        /// <summary>
        /// 获得存在排队中的数据
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool DeleteLineUpRenewal(int buid, int topAgentId, int operateAgent);
        List<bx_batchrefreshrenewal> GetBatchRefreshRenewalByAgentList();
        List<bx_batchrefreshrenewal> GetBatchRefreshRenewalLimit(int rows);
        List<bx_batchrefreshrenewal> GetBatchRefreshRenewalByTimes();
        /// <summary>
        /// 更新该表中续保状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool UpdateBatchRefreshRenewalStatus(bx_batchrefreshrenewal model);
        List<DistributeTimeTagModel> GetDistributedTimeTag(int agentId);
        /// <summary>
        /// 客户信息
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="agentId">代理人编号</param>
        /// <returns></returns>

        List<CustomerInfoesVM> CustomerInfoes(string mobile, int agentId);

    
    }
}
