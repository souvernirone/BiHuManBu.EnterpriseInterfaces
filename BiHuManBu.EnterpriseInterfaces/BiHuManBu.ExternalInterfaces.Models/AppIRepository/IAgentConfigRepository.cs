using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.AppIRepository
{
    public interface IAgentConfigRepository
    {
        List<bx_agent_config> Find(int agentid);
        List<bx_agent_config> FindNewCity(int agentid);
        List<long> FindSource(int agentid);
        List<int> FindSourceOld(int agentid);
        List<bx_agent_config> FindBy(int agentid, int citycode);
        List<bx_agent_config> FindCities(int agentid, int cityId);

        /// <summary>
        /// 获取城市集合
        /// </summary>
        /// <param name="agentId">顶级代理人Id</param>
        /// <returns></returns>
        List<int> FindCity(int agentId);

        List<CitySourceViewModel> FindAgentConfigByAgent(int agentid);
    }
}
