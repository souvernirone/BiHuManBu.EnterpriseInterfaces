using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class DailyWorkViewModel
    {
        public int TopAgentId { get; set; }
        public string AgentName { get; set; }
        public int QuoteCount { get; set; }
        public int ReviewCount { get; set; }
        public int CallCount { get; set; }
    }
}
