using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class LastDayReInfoTotalViewModel : BaseViewModel
    {
        public int InStoreNum { get; set; }
        public int ExpireNum { get; set; }
        public int IntentionNum { get; set; }
        public int OrderNum { get; set; }
        public List<ReportReInfo> ReInfo { get; set; }
    }
    public class ReportReInfo
    {
        public string LicenseNo { get; set; }
        public string AdvAgentName { get; set; }
    }
}
