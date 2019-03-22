using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using System.Collections.Generic;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppResponse = BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces
{
    public interface IMessageService
    {
        AppResponse.AddMessageResponse AddMessage(AppRequest.AddMessageRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        AppResponse.ReadMessageResponse ReadMessage(AppRequest.ReadMessageRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        AppResponse.MessageListResponse MessageList(AppRequest.MessageListRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        AppResponse.MessageListResponse MessageListNew(AppRequest.MessageListRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 添加消息
        /// 0:系统消息,1:到期通知,2:回访通知,3:工单提醒,4:账号审核提醒,5管理日报，6:分配通知,7:出单通知
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        int AddMessage(AppRequest.AddMessageRequest request);

        /// <summary>
        /// 已出单详情
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppResponse.MsgClosedOrderResponse MsgClosedOrder(AppRequest.MsgClosedOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        
        /// <summary>
        /// 保存代理人和信鸽推送应用关系
        /// </summary>
        /// <param name="agentXGAccountRelationShip"></param>
        /// <returns></returns>
        int SaveAgent_XGAccount_RelationShip(bx_agent_xgaccount_relationship agentXGAccountRelationShip);
        /// <summary>
        /// 更新消息状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="readStatus"></param>
        /// <returns></returns>
        int UpdateMessageStatus(long id, int readStatus);
        /// <summary>
        /// 获取消息历史
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="readStatus"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>

        List<Models.ViewModels.AppViewModels.MessageHistory> GetMessageHistory(int roletype, int agentId, int readStatus, int pageIndex, int pageSize, out int totalCount);
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int DeleteMessage(long id);
        /// <summary>
        /// 检查代理人是否已注册信鸽账号
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        bool CheckXgAccount(int agentId,int deviceType);
        /// <summary>
        /// 获取可分配的代理人
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<SettedAgent> GetSettedAgents(int agentId,int pageIndex,int pageSize,out int totalCount);
        /// <summary>
        /// 获取可用短信条数
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <param name="agentAccount">短信账号</param>
        /// <returns></returns>
        long GetUsableSmsCount(int agentId, int topAgentId, out string agentAccount);
    }
}
