
using System;

namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class InsuranceStartDate
    {
        public DateTime? BizStartDate { get; set; }
        public DateTime? ForceStartDate { get; set; }
    }


    public class InsuranceStartDateBuid
    {
        public long Buid { get; set; }
        public DateTime? BizStartDate { get; set; }
        public DateTime? ForceStartDate { get; set; }
    }
}
