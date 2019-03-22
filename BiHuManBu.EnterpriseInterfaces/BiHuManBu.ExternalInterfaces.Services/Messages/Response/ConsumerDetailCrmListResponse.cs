using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public  class ConsumerDetailCrmListResponse:BaseViewModel
    {
        public List<ConsumerDetailCrmListResponsedeatil> BxCrmStepses { get; set; }
    }


}
