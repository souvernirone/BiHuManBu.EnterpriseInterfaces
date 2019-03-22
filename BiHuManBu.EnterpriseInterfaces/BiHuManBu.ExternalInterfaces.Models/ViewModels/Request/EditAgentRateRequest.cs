using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class EditAgentRateRequest
    {
        [Range(1, 100000000)]
        public int AgentId { get; set; }
        [Range(0, 100000000)]
        public double AgentRate { get; set; }
        public int CompanyId { get; set; }
        public double Three { get; set; }
        public double Four { get; set; }
        public string TableJson { get; set; }
        [Range(0, 3)]
        public int IsQuDao { get; set; }
        public int QuDaoId { get; set; }
        [Range(1, 100000000)]
        public int CreateUserId { get; set; }
        [Required]
        public string CreateUserName { get; set; }
    }
}
