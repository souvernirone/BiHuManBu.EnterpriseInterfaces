using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class AgentDistributedViewModel : BaseViewModel
    {
        public int? ParentAgentId { get; set; }
        public int? ShareCode { get; set; }
        public string SecCode { get; set; }
        public string CustKey { get; set; }
        public List<QueryAgentInfo> AgentDistributedInfo { get; set; }
    }
}
