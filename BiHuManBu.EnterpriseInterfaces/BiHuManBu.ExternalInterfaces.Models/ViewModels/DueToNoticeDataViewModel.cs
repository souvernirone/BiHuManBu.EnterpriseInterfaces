using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class DueToNoticeDataViewModel
    {
        public int AgentId { set; get; }

        public List<CompositeBuldLicenseNoDays> Data { get; set; }

        public string BuidsString { get; set; }
    }
    public class CompositeBuldLicenseNoDays
    {

        public long BuId { get; set; }

        public string LicenseNo { get; set; }

        public int Days { get; set; }

    }
}
