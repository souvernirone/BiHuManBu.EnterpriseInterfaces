using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class GetSettedAgentViewModel : BaseViewModel
    {
        public int TotalCount { get; set; }

        public List<SettedAgent> SettedAgents { get; set; }
    }
   public class SettedAgent
    {
        /// <summary>
        /// 代理人id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 代理人姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// bx_agent_distributed.id
        /// </summary>
        public int Id { get; set; }
    }
}
