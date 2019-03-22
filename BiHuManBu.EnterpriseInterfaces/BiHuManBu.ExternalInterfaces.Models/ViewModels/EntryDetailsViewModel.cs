using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class EntryDetailsViewModel
    {
        public int Id { get; set; }
        public string LicenseNo { get; set; }

        public string InRenewalPeriod { get; set; }

        public string LastYearSource { get; set; }

        public string IsQuote { get; set; }

        public string CameraTime { get; set; }

        public string IsDistributed { get; set; }

        public string IsReView { get; set; }
    }
}
