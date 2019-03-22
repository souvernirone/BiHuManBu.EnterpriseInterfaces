
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using System.Collections.Generic;
using ReportModel = BiHuManBu.ExternalInterfaces.Models.ReportModel;

namespace BiHuManBu.ExternalInterfaces.Models.AppIRepository
{
    public interface IMessageRepository
    {
        #region bx_message
        int Add(bx_message bxMessage);
        int Update(bx_message bxMessage);
        bx_message Find(int msgId);
        List<bx_message> FindNoReadList(int agentId, out int total);
        List<ReportModel.TableMessage> FindNoReadAllList(int agentId, out int total);
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
        bool CheckXgAccount(int agentId, int deviceType);

        #endregion

        #region bx_system_message
        /// <summary>
        /// 获取消息历史
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="readStatus"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<ViewModels.AppViewModels.MessageHistory> GetMessageHistory(int agentId, int readStatus, int pageIndex, int pageSize, out int totalCount);

        #endregion
        /// <summary>
        /// 获取可分配的代理人
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<SettedAgent> GetSettedAgents(int agentId, int pageIndex, int pageSize, out int totalCount);
        /// <summary>
        /// 获取代理人短信可用条数
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <param name="agentAccount">短信账户</param>
        /// <returns></returns>
        long GetUsableSmsCount(int agentId, int topAgentId,out string agentAccount);
    }
}
