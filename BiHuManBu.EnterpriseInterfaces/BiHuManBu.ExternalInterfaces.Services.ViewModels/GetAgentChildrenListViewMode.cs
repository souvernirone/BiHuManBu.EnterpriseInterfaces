using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class GetAgentChildrenListViewMode : BaseViewModel
    {
        public List<AgentViewModel> Items { get; set; } 
    }
    public class AgentViewModel
    {
        public int Id { set; get; }
        public string Agentname { set; get; }
        public string ShareCode { set; get; }
        public string ShowText { get { return string.Format("{0}({1})", Agentname, ShareCode); } }
    }
}
