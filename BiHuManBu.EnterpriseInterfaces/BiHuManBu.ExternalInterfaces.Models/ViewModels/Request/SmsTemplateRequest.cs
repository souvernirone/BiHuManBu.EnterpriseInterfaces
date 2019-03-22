using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class SmsTemplateRequest
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string SmsTemplateName { get; set; }
        public string SmsTemplateContent { get; set; }

        public int TopAgentId { get; set; }
    }
}
