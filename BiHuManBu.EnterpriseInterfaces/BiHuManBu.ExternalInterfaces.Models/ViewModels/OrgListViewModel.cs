using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class OrgListViewModel:BaseViewModel
    {
        public IList<OrgListDto> OrgList { get; set; }
        public IList<OrgListDto> IdNameList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }

    }

    public class OrgListDto
    {
        public int AgentId { get; set; }
        public int AuthenId { get; set; }
        public string Mobile { get; set; }
        public string AgentName { get; set; }
        public string CityName { get; set; }
        public string AuthenState { get; set; }
        public string Camera { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string ChargePerson { get; set; }
    }
}
