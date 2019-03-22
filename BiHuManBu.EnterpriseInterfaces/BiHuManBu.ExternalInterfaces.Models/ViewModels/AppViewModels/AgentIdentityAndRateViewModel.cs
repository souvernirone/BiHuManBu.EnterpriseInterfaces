
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
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

            public Rate AgentRate { get; set; }
            public List<Rate> ZhiKeRate { get; set; }


        }

        public class Rate
        {
            public double BizRate { get; set; }
            public double ForceRate { get; set; }
            public double TaxRate { get; set; }

            public long Source { get; set; }
        }
    }
}
