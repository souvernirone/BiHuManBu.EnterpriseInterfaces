using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class ConsumerDetailCrmTlViewModel
    {
  
        public string JsonContent { get; set; }
        public int AgentId { get; set; }
        public int Type { get; set; }
        public long BUid { get; set; }
    }
}
