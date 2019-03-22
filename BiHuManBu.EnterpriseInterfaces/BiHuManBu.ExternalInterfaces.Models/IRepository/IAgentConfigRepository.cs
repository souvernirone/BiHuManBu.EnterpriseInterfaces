using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Linq.Expressions;
using System;

namespace BiHuManBu.ExternalInterfaces.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAgentConfigRepository: IRepositoryBase<bx_agent_config>
    {
        List<bx_agent_config> FindNewCity(int agentid);

        /// <summary>
        /// 根据代理Id查代理人渠道
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        List<bx_agent_config> Find(int agentid);
        List<int> FindCity(int agentId, int isBj);
        List<bx_agent_config> FindFullCity(int agentId, int isBj);

        /// <summary>
        /// 分页获取ukey
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="quDaoName"></param>
        /// <param name="cityId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        GetPageUKeyViewModel GetPageUKey(int pageIndex, int pageSize, string quDaoName, int cityId, int agentId);


        /// <summary>
        /// 查询ukey
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bx_agent_ukey GetUkeyModel(int id);
        /// <summary>
        /// 通过ID查找
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bx_agent_config GetAgentConfigById(long id);

        /// <summary>
        /// 多渠道报价添加渠道数量
        /// </summary>
         int AddSouceCount(int agentId, int cityId, Dictionary<int, int> dic);

        // <summary>
        /// 获取代理人多渠道报价数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IList<bx_agent_config_count> GetAgentConfigCountList(Expression<Func<bx_agent_config_count, bool>> where);
    }
}
