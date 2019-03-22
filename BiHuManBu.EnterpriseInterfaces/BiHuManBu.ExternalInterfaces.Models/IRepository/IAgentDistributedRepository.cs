using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAgentDistributedRepository : IRepositoryBase<bx_agent_distributed>
    {

        List<bx_agent_distributed> FindByParentAgent(int parentAgentId);

        /// <summary>
        /// 获取出单员
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        List<OrderAgentDto> GetOrderAgent(int topAgentId);

        /// <summary>
        /// 只获取出单员对应的代理人id
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        Task<List<int>> GetOrderAgentId(int topAgentId);

        /// <summary>
        /// 获取业务员，包含顶级代理人
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        List<BriefAgentDto> GetAgent(int topAgentId);

        /// <summary>
        /// 根据父级代理人和代理人类型删除
        /// </summary>
        /// <param name="parentAgentId"></param>
        /// <param name="agentType"></param>
        /// <returns></returns>
        bool DeleteByParentAgentIdAgentType(int parentAgentId,int agentType);
        /// <summary>
        ///设置出单员可用渠道
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        bool AddDistributedSource(string sql);
        /// <summary>
        /// 获取渠道
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<OrderAgentSourceDto> GetOrderAgentSource(GetOrderAgentRequest request);
    }
}
