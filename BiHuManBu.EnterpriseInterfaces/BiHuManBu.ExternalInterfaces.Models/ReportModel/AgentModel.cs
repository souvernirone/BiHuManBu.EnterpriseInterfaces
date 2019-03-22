

using System;

namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class AgentModel:bx_agent
    {
        public int AgentLevel { get; set; }
        public string ParentName { get; set; }
        public string ParentMobile { get; set; }
        public string TopAgentName { get; set; }
        public string TopAgentMobile { get; set; }
        public Nullable<long> TotalTimes { get; set; }
        public Nullable<long> AvailTimes { get; set; }
        public string SmsAccount { get; set; }
    }
}
