using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.Model;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IMessageRepository
    {
        long AddMsgIdx(bx_msgindex model);

        /// <summary>
        /// 批量插入bx_msgindex表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<bool> AddMsgIndexListAsync(List<bx_msgindex> list);

        int MsgCount(int agentId, int msgMethod);
        List<bx_msgindex> MsgList(int agentId, int msgMethod, int pageSize, int curPage, out int total);
        bx_message GetMsgDetail(long indexId);
        bx_message MsgLastDetail(long childagent, int msgMethod);
        bx_msgindex GetMsgIndex(long indexId, int msgMethod);
        bx_msgindex GetMsgIndex(long agentId, int msgId, int msgMethod);
        int UpdateMsgIndex(bx_msgindex model);
        int UpdateChildAgentMsg(long childAgent, int msgMethod);

        /// <summary>
        /// 判断消息是否存在
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        bool IsExist(int msgId);

        /// <summary>
        /// 获取信鸽账号
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bx_agent_xgaccount_relationship GetXgAccount(int agentId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentIds"></param>
        /// <param name="deviceType">4为CRM机器人，5推修</param>
        /// <returns></returns>

        List<bx_agent_xgaccount_relationship> GetXgAccounts(int[] agentIds,int deviceType=4);

        #region bx_message
        int Add(bx_message bxMessage);

        /// <summary>
        /// 批量添加消息
        /// </summary>
        /// <param name="listMsg"></param>
        /// <returns></returns>
        Task<bool> AddListAsync(List<bx_message> listMsg);

        /// <summary>
        /// 添加bx_msgIndex,由于添加的量很大，所以直接写sql添加
        /// </summary>
        /// <param name="insertSql"></param>
        /// <returns></returns>
        bool Add(string insertSql);

        int Update(bx_message bxMessage);
        bx_message Find(int msgId);
        bx_message FindById(int msgId);
        List<bx_message> FindMsgList(int agentId, int pageSize, int curPage, out int total);
        List<bx_message> FindNoReadList(int agentId, out int total);
        List<TableMessage> FindNoReadAllList(int agentId, out int total);
        #endregion

        #region bx_system_message
        int AddSysMessage(bx_system_message bxSystemMessage);
        int UpdateSysMessage(bx_system_message bxSystemMessage);
        List<bx_system_message> FindSysMessage();

        #endregion
    }
}
