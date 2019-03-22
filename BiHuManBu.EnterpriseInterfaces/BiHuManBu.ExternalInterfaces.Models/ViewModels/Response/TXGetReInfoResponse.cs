using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
   public  class TXGetReInfoResponse:TXBaseResponse
    {

       public GetReInfoNewViewModel Data { get; set; }
    }
}
