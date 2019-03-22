using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class QueryAgentRequest
    {
        [Range(1,100000000)]
        public int TopAgentId { get; set; }
        public string AgentName { get; set; }
        public string ShareCode { get; set; }
    }
}
