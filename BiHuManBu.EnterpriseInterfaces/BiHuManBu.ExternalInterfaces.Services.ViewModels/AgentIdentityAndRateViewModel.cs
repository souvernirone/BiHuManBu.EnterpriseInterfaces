
namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class AgentIdentityAndRateViewModel : BaseViewModel
    {
        public AgentIdentityAndRate Item { get; set; }
        public class AgentIdentityAndRate
        {
            /// <summary>
            /// 是否是经纪人 1:经纪人 0：直客
            /// </summary>
            public int IsAgent { get; set; }

            public double BizRate { get; set; }
            public double ForceRate { get; set; }
            public double TaxRate { get; set; }
        }
    }
}
