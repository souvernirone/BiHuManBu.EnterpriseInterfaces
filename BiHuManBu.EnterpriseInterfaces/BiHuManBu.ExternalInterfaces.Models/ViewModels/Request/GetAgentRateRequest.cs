using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class GetAgentRateRequest
    {
        [Range(1,100000000)]
        public int AgentId { get; set; }
        public int CompanyId { get; set; }
        public int IsQuDao { get; set; }
        public int QuDaoId { get; set; }
    }
}
