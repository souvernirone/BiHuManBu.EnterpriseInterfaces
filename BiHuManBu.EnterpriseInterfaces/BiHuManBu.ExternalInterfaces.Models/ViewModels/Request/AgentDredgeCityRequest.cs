using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.Model;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 
    /// </summary>
    public class AgentDredgeCityRequest:BaseViewModel
    {
        public List<AgentDredgeCity> AgentDredgeCities { get; set; }
    }
}
