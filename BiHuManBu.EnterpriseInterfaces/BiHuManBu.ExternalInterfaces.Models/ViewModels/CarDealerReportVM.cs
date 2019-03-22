using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CarDealerReportVM
    {
        public int CarDealerCount { get; set; }

        public List<CarDealerReportData> CarDealerReportDatas { get; set; }
    }

    public class CarDealerReportBaseVM
    {
        public int EntryCount { get; set; }
        public int QuoteCount { get; set; }
    }

    public class CarDealerReportData : CarDealerReportBaseVM
    {
        public string EntryTime { get; set; }
    }

    public class CarDealerReportDetails : CarDealerReportBaseVM
    {
        public int TopAgentId { get; set; }
        public string AgentName { get; set; }
        public int RenewalPeriodCount { get; set; }
        public int DistributedCount { get; set; }
        public int ReviewCount { get; set; }
    }
}
