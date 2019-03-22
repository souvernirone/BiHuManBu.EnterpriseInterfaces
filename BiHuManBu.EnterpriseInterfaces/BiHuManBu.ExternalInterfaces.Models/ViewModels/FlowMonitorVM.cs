using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class FlowMonitorVM
    {
        public List<FlowMonitorVM_PropertyClass> FlowMonitorDayVM { get; set; }
        public List<FlowMonitorVM_PropertyClass> FlowMonitorMonthVM { get; set; }
        public List<FlowMonitorVM_PropertyClass> FlowMonitorYearVM { get; set; }
    }
    public class FlowMonitorVM_PropertyClass
    {
        public int TopAgentId { get; set; }
        public int ImportDataCount { get; set; }
        public int DataInterchangeCount { get; set; }
        public int QuoteCount { get; set; }
        public int ConclusionCount { get; set; }
    }
}
