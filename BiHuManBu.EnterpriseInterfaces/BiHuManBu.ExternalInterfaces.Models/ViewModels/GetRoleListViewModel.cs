using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GetRoleListViewModel:BaseViewModel
    {
        public IList<string> RoleList { get; set; }
    }
}
