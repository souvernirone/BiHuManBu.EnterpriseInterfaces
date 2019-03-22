
namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class InsuranceEndDate
    {
        public string LastBusinessEndDdate { get; set; }
        public string LastForceEndDdate { get; set; }
    }

    public class InsuranceEndDateWithBuid
    {
        public long Buid { get; set; }
        public string LastBusinessEndDdate { get; set; }
        public string LastForceEndDdate { get; set; }
    }
}
