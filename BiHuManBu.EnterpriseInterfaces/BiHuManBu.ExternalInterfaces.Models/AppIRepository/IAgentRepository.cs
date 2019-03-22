using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent;

namespace BiHuManBu.ExternalInterfaces.Models.AppIRepository
{
    public interface IAgentRepository
    {
        long Add(bx_agent agent);
        int Update(long agentId);
        int UpdateModel(bx_agent bxAgent);
        bx_agent GetAgentByMobile(string mobile);
        bx_agent GetAgent(int agentId);
        bx_agent GetAgentIsTop(int agentId);
        bx_agent GetAgent(string openId);
        List<bx_agent> GetAllAgent(string openId);
        bx_agent GetAgentByParentAgent(string openId, int parentAgent);
        bx_agent GetAgentByTopParentAgent(string openId, int topParentAgent);
        /// <summary>
        /// 根据OpenId和顶级Agent获取当前AgentId
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="topAgent"></param>
        /// <returns></returns>
        int GetAgentId(string openId, string topAgent);
        bx_agent GetAgentByPhoneTopAgent(string mobile, int topParentAgent);
        string GetSonId(int currentAgent);
        /// <summary>
        /// 根据AgentId获取所有的子集代理
        /// </summary>
        /// <param name="currentAgent"></param>
        /// <param name="isContainSelf">是否包含自己</param>
        /// <returns></returns>
        List<string> GetSonsList(int currentAgent, bool isContainSelf);

        /// <summary>
        /// 查询子集，返回子集字符串 demo：'1','2','3'
        /// </summary>
        /// <param name="curAgent"></param>
        /// <param name="isContainSelf">是否包含自己</param>
        /// <returns></returns>
        string GetSonsIdToString(int curAgent, bool isContainSelf);
        /// <summary>
        /// 查询子集，返回子集字符串 demo：1,2,3
        /// </summary>
        /// <param name="curAgent"></param>
        /// <param name="isContainSelf">是否包含自己</param>
        /// <returns></returns>
        string GetSonsIdToInt(int curAgent, bool isContainSelf);
        

        string GetTopAgentId(int currentAgent);
        int GetAgentLevel(int parentAgent);
        List<bx_agent> GetSonsAndSelfList(bx_agent bxAgent);
        IEnumerable<bx_agent> GetSonsAgent(int curAgentId);
        List<bx_agent> GetAgentList(int curAgent, int topAgent, string search, int agentStatus, int pageSize,
            int curPage, out int totalCount);

        bool IsContainSon(int topAgent, int sonAgent);
    }
}
