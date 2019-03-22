using System;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class AgentPointViewModel : BaseViewModel
    {
        public bx_agentpoint agentpoint { get; set; }
    }
    public class AgentPointsViewModel : BaseViewModel
    {
        public List<bx_agentpoint> agentpoints { get; set; }
    }

    public class AgentPoint
    {
        public int id { get; set; }
        public Nullable<int> AgentId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Nullable<DateTime> create_time { get; set; }
    }
}
