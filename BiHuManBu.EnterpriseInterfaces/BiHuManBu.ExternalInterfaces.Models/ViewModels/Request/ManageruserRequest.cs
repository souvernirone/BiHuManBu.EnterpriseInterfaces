using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class ManageruserRequest
    {
        public string Mobile { get; set; }
        public string Account { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

    }

    public class ManageruserListViewModel:BaseViewModel
    {
        public IList<ManageruserViewModel> ManageruserList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}
