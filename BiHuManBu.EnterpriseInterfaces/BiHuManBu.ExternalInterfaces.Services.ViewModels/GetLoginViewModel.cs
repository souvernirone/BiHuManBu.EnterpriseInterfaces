using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class GetLoginViewModel : BaseViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int agentId { get; set; }
        public string agentName { get; set; }
        public string agentMobile { get; set; }
        public string mobile { get; set; }
        public int? agentType { get; set; }

        public int? isShow { get; set; }

        public int managerRoleId { get; set; }

        public string secretKey { get; set; }

        public int? isDaiLi { get; set; }
        public int? roleType { get; set; }

        public int? topAgentId { get; set; }
        public ManagerRole roleInfo { get; set; }
        public List<ManagerModule> module { get; set; }
        public List<ParentAgent> parentAgent { get; set; }

        private int _isDistribute = 1;
        public int isDistribute
        {
            get { return _isDistribute; }
            set { _isDistribute = value; }
        }
        public int? regType { get; set; }
        public string parentSecretKey { get; set; }

        public string token { get; set; }
    }

    public class ManagerRole
    {
        public int id { get; set; }
        public string role_name { get; set; }
        public Nullable<double> role_status { get; set; }
        public Nullable<int> top_agent_id { get; set; }
        public Nullable<int> role_type { get; set; }
    }

    public class ManagerModule
    {
        public string module_code { get; set; }
        public string module_name { get; set; }
        public string pater_code { get; set; }
        public Nullable<double> module_level { get; set; }
        public Nullable<double> is_menu { get; set; }
        public Nullable<double> is_action { get; set; }
        public string action_url { get; set; }
        public Nullable<double> module_status { get; set; }
        public List<ManagerModule> child { get; set; }
    }
    public class ParentAgent
    {
        public int id { get; set; }
        public string agentName { get; set; }
        public string shareCode { get; set; }
        public string secretKey { get; set; }
        public int parentAgent { get; set; }
        public int? isUsed { get; set; }
        public int? isDaiLi { get; set; }
        public int ManagerRoleId { get; set; }
    }
}
