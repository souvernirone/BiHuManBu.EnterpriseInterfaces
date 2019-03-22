using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
  public  class CreateWCahtAgentAccountRequest
    {
      public string Account { get; set; }
      public string PassWord { get; set; }
  
      public string OpenId { get; set; }
      public int AgentId { get; set; }
      public string SecCode { get; set; }
      public string CustKey { get; set; }
    }
}
