using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
    public class BxUserinfoRenewalViewModel
    {
        public DateTime? LastForceEndDate { get; set; }
        public DateTime? LastBizEndDate { get; set; }
        public long b_uid { get; set; }
    }
}
