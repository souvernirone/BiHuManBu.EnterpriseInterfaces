using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class GetAgentTokenViewModel : BaseViewModel
    {
        public int? AgentId { get; set; }
        public string SecCode { get; set; }
        public string CustKey { get; set; }
        public string token { get; set; }
    }
}
