using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
  public   interface ITXCluesAgentRelationshipRepository
    {

        bool Add(tx_clues_agent_relationship model);
    }
}
