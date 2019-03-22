
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AgentRateRepository : IAgentRateRepository
    {
        public List<bx_agent_rate> GetAgentRates(List<int> agents, int source)
        {
            var db = DataContextFactory.GetDataContext();

            DataContextFactory.GetDataContext().bx_userinfo.Where(x => new List<long>().Contains(x.Id));
            if (agents.Count == 0) return null;
            var result =
               DataContextFactory.GetDataContext().bx_agent_rate.Where(x => agents.Contains(x.agent_id) && x.company_id == source).ToList();
            return result;
        }

        public List<int> GetAgentOfZhiKeRate(int topAgent)
        {
            return new List<int>();
        }
    }
}
