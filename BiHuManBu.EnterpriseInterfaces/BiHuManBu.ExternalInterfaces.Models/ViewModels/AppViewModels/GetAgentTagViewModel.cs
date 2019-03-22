using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class GetAgentTagViewModel : BaseViewModel
    {
        public List<TagFlag> Content { get; set; }
    }
    public class TagFlag
    {
        public long TagId { get; set; }
        public int AgentId { get; set; }
        public string Content { get; set; }
        public int ValidFlag { get; set; }
        public System.DateTime CreateTime { get; set; }
    }
}
