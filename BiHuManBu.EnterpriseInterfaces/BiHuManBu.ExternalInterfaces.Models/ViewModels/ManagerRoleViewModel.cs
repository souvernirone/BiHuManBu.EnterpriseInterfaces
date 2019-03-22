using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class ManagerRoleListViewModel : BaseViewModel
    {
        public List<ManagerRoleViewModel> roleInfo { get; set; }
    }

    public class ManagerRoleViewModel
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public Nullable<double> RoleStatus { get; set; }
        public Nullable<int> TopAgentId { get; set; }
        public Nullable<int> RoleType { get; set; }
        public string CreatorName { get; set; }
        public string CreateorTime { get; set; }
        public string AgentName { get; set; }
        public Nullable<int> isRequote { get; set; }
        /// <summary>
        /// 自定义排序字段
        /// </summary>
        public int customOrder { get; set; }
    }
}
