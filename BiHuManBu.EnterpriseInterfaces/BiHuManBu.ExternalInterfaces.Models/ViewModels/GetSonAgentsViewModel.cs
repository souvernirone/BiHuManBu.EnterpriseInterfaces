using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 获取下级代理人
    /// </summary>
    public class GetSonAgentsViewModel : BaseViewModel<GetSonAgentsViewModel>
    {
        /// <summary>
        /// 
        /// </summary>
        public List<AgentIdAndAgentName> ListAgent { get; set; }
        public List<int> AgentIds { get; set; }
    }
}
