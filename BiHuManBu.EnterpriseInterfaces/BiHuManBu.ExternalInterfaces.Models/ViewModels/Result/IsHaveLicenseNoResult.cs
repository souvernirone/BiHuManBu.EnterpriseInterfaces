using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
   public class IsHaveLicenseNoResult:BaseViewModel
    {
       public int Type { get; set; }
       public long Buid { get; set; }
       public int AgentId { get; set; }
       public int TopAgentId { get; set; }
       public string AgentName { get; set; }
    }
}
