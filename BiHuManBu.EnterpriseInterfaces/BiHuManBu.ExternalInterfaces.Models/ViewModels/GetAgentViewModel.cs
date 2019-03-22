using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GetAgentViewModel : BaseViewModel
    {
        public int AgentId { get; set; }
        public int? IsUsed { get; set; }
        public int RoleId { get; set; }
        public int TopAgentId { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public int TotalNum { get; set; }
        public string Search { get; set; }
        public List<QueryAgentInfo> AgentInfo { get; set; }
        public string SecCode { get; set; }
        public string CustKey { get; set; }
        public string token { get; set; }
        public int AgentLevel { get; set; }
    }
    public class QueryAgentInfo
    {
        public int? IsUsed { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string Mobile { get; set; }
        public string IsUsedName { get; set; }
        public string RoleName { get; set; }
        public string Time { get; set; }
        public int? RoleId { get; set; }
        public string AgentAccount { get; set; }
        public int AgentLevel { get; set; }
    }
}
