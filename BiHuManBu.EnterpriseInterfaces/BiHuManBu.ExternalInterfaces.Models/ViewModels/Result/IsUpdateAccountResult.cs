using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
   public class IsUpdateAccountResult:BaseViewModel
    {
       public bool IsNeedUpdateAccount { get; set; }
       public int AgentId { get; set; }
    }
}
