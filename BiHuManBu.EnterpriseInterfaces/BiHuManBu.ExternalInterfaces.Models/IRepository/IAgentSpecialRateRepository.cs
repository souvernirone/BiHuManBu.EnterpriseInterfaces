using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IAgentSpecialRateRepository
    {
        bx_agent_special_rate GetAgentSpecialRate(int agentid, int source);

        List<bx_agent_special_rate> GeAgentSpecialRates(List<int> agents, int source);

        IList<BxAgentSpecialRate> GetAgentSpecialRate(int agentId, int companyId, int isQudao, int qudaoId);

        /// <summary>
        /// 下级经纪人特殊点位
        /// </summary>
        /// <param name="list"></param>
        /// <param name="companyId"></param>
        /// <param name="agentId"></param>
        /// <param name="createPeopleId"></param>
        /// <param name="createPeopleName"></param>
        /// <param name="isQudao"></param>
        /// <param name="qudaoId"></param>
        /// <returns></returns>
        int InsertBxAgentSpecialRateLow(List<NameValue> list, int companyId, int agentId, int createPeopleId, string createPeopleName, int isQudao, int qudaoId);

        /// <summary>
        /// 更新经纪人费率
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="companyId"></param>
        /// <param name="agentRate"></param>
        /// <param name="three"></param>
        /// <param name="four"></param>
        /// <param name="isQudao"></param>
        /// <param name="qudaoId"></param>
        /// <returns></returns>

        int UpdateAgentRate(int agentId, int companyId, double agentRate, double three, double four, int isQudao, int qudaoId);

        /// <summary>
        /// 添加经纪人费率
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="companyId"></param>
        /// <param name="agentRate"></param>
        /// <param name="three"></param>
        /// <param name="four"></param>
        /// <param name="createPeople"></param>
        /// <param name="createPeopleName"></param>
        /// <param name="isQudao"></param>
        /// <param name="qudaoId"></param>
        /// <returns></returns>
        int InsertAgentRate(int agentId, int companyId, double agentRate, double three, double four, int createPeople, string createPeopleName, int isQudao, int qudaoId);

        int CheckAgentRate(int agentId, int companyId, int isQudao, int qudaoId);

        BxAgentRate GetAgentRate(int agentId, int companyId, int isQudao, int qudaoId);
    }
}
