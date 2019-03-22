using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class OverNoticeSettingRequest
    {
        public int AgentId { get; set; }
        public int RoleType { get; set; }
        public int OverTime { get; set; }
        public int PollingCycle { get; set; }
        public int TopAgentId { get; set; }
    }
}
