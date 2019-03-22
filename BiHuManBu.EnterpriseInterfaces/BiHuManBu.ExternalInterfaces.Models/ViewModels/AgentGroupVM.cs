using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class AgentGroupVM
    {
        public int AgentIdKey { get; set; }
        public IEnumerable<int> AgentIdsValue { get; set; }

    }
}
