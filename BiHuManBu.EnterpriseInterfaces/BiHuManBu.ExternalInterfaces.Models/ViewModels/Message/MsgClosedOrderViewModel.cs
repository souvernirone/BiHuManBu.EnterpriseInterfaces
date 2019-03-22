
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class MsgClosedOrderViewModel : BaseViewModel
    {
        public string LicenseNo { get; set; }
        public string MoldName { get; set; }
        public string SourceName { get; set; }
        public string ReviewContent { get; set; }

        public int SaAgent { get; set; }
        public string SaAgentName { get; set; }
        public int AdvAgent { get; set; }
        public string AdvAgentName { get; set; }
    }
}
