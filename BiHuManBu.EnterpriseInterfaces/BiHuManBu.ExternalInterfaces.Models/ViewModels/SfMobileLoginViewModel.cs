using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SfMobileLoginViewModel : BaseViewModel
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentAccount { get; set; }
        public int IsViewAllData { get; set; }
    }
}
