using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IAgentUkeyRepository
    {
        IQueryable<bx_agent_ukey> GetList(Expression<Func<bx_agent_ukey, bool>> where);
        /// <summary>
        /// 根据UKId到bx_agent_ukey获取数据
        /// </summary>
        /// <param name="ukId"></param>
        /// <returns></returns>
        bx_agent_ukey GetAgentUKeyModel(int ukId);
        bx_agent_ukey GetUkeyCityByAgentId(int agentId);

        /// <summary>
        /// 根据报价渠道Id获取工号（抓单渠道Id）
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        bx_agent_ukey GetUKeyByConfigId(int configId);
    }
}
