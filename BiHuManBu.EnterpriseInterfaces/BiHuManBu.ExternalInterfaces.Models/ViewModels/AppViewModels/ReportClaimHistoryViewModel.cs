using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public  class ReportClaimHistoryViewModel:BaseViewModel
    {
        public string LicenseNo { get; set; }
        public int TotalCount { get; set; }
        public int UsedCount { get; set; }
        public List<ReportClaim> ReportClaims { get; set; }
        public List<HistoryContract> HistoryContracts { get; set; } 
    }

    public class HistoryContract
    {
        public string Enddate { get; set; }
        public string InsureCompanyName { get; set; }
        public int IsCommerce { get; set; }
        public string PolicyNo { get; set; }
        public string Strdate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
    }

    public class ReportClaim
    {
        public string LicenseNo { get; set; }
        public string AccidentPlace { get; set; }
        public string AccidentPsss { get; set; }
        public string DriverName { get; set; }
        public int IsCommerce { get; set; }
        public int IsOwners { get; set; }
        public int IsThreecCar { get; set; }
        public string ReportDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
    }

}
