using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ICustomerBusinessService
    {
        /// <summary>
        /// 获取顶级客户账号下，业务员列表信息
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <param name="isHas"></param>
        /// <returns></returns>
        CTopLevelAgentListViewModel GetBusinessAgentList(int currentAgentId,int isHas);

        ///// <summary>
        ///// 根据特定筛选条件查询业务员需要操作的车辆信息集合
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="totalCount"></param>
        ///// <returns></returns>
        //List<GetCustomerViewModel> GetCarInfoList(GetCustomerListRequest request, out DistributedCountViewModel totalCount);

        ///// <summary>
        ///// 获取匹配的车辆信息（添加了label的版本使用）
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //List<GetCustomerViewModel> GetCarInfoListLabel(GetCustomerListRequest request);

        ///// <summary>
        ///// 快捷查询车辆信息集合
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="totalCount"></param>
        ///// <returns></returns>
        //List<GetCustomerViewModel> GettQuickCarInfoList(GetCustomerListRequest request,  out DistributedCountViewModel totalCount);
        ///// <summary>
        ///// 今日回访快捷查询车辆信息集合
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="totalCount"></param>
        ///// <returns></returns>
        //List<GetCustomerViewModel> GetTodayReviewCarInfoList(GetCustomerListRequest request, out DistributedCountViewModel totalCount);

        /// <summary>
        /// 查询没有读取的分配通知
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DistributedDataVm GetDistributeForAgent(GetDistributeForAgentRequest request);
        int UpdateBxMessage(string messageIds);

        /// <summary>
        /// 分配回收
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="topAgent"></param>
        /// <param name="childAgent"></param>
        /// <returns></returns>
        Task<bool> UpdateDistributeRecycle(List<long> buids, int topAgent, int childAgent);

        /// <summary>
        /// 获取标签下客户数据的数量
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        LabelCountViewModel GetLabelCount(GetCustomerListRequest request);

        /// <summary>
        /// 获取今日计划回访下面的子标签的数量
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<GetTodayReviewCountViewModel> GetTodayReviewCountAsync(GetTodayReviewCountRequest request);

        /// <summary>
        /// 业务员列表批量审核 zky 2017-08-31 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool AgentBatchAudit(Messages.Request.AgentBatchAuditRequest request);

        /// <summary>
        /// 更新业务员信息 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="request"></param>
        BaseViewModel UpdateCustomerInfo(UpdateCustomerRequest request);




        /// <summary> 
        /// 查询代理人级别接口  zky 2017-12-11 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        GetAgentViewModel GetAgentLevel(int agentId);

        /// <summary>
        /// 删除代理人检查代理人状态  zky 2017-12-11 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        CheckAgentStatusViewModel CheckAgentStatus(int agentId);

        BaseViewModel EditAgentRate(int agentId, int companyId, double agentRate, double three, double four, int createPeople, string createPeopleName, List<NameValue> list, int isQudao, int qudaoId);

        QueryAgentViewModel GetAgentNameByShareCode(string shareCode, int topAgentId);
        QueryAgentViewModel GetShareCodeByAgentName(string agentName, int topAgentId);
        CheckAgentStatusViewModel IsCanEditParentAgent(IsCanEditParentAgentRequest request);

        /// <summary>
        /// 振邦机构添加用户 zky 2017-12-26 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel ZhengBangAddUser(AddZhenBangUserRequest request);

        /// <summary>
        /// 启用禁用代理人账号
        /// </summary>
        /// <param name="agentId">代理人id</param>
        /// <param name="isUsed">1:启用 2：禁用</param>
        /// <returns></returns>
        BaseViewModel EidtAgentIsUsed(List<int> agentId, int isUsed);

        BaseViewModel CanEditParentShareCode(IsCanEditParentAgentRequest request);
        /// <summary>
        /// 判断是否存在未完成刷新续保的数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CheckRefreshRenewalViewModel CheckRefreshRenewal(CheckRefreshRenewalRequest request);
        /// <summary>
        /// 将批量刷新续保数据添加到表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BatchRefreshRenewalViewModel BatchRefreshRenewal(BatchRefreshRenewalRequest request);
        BatchRefreshRenewalDetailViewModel BatchRefreshRenewalDetail(BatchRefreshRenewalDetailRequest request);
        /// <summary>
        /// 删除排队中的数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        LineUpRenewalViewModel DeleteLineUpRenewal(LineUpRenewalRequest request);
        /// <summary>
        /// 获得业务员通知标签
        /// </summary>
        /// <param name="ChildAgent">当前业务员</param>
        /// <returns></returns>
        DistributeTimeTagViewModel GetDistributedTimeTag(int ChildAgent);
        /// <summary>
        /// 客户信息
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="agentId">代理人编号</param>
        /// <returns></returns>

        List<CustomerInfoesVM> CustomerInfoes(string mobile,int agentId);
        CorrectRepeatViewModel GetCorrectRepeatList(CorrectRepeatRequest request);
    }
}
