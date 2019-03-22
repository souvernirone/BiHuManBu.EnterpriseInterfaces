using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
     public interface IAgentRateRepository
    {
         List<bx_agent_rate> GetAgentRates(List<int> agents,int source); 
    }
}
