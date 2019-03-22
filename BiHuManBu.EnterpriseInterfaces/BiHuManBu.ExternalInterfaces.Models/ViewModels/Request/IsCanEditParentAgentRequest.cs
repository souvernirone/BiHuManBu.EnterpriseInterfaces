using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class IsCanEditParentAgentRequest
    {
        [Range(1, 100000000)]
        public int AgentId { get; set; }
        [Range(1, 100000000)]
        public int TopAgentId { get; set; }
        [Range(0,1)]
        public int IsDaiLi { get; set; }
        public int ParentShareCode { get; set; }
    }
}
