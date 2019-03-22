using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class FlowDirectionAnalysisVM
    {
        public FlowDirectionAnalysisVM_PropertyClass FlowDirectionLastNewCarAnalysisVM { get; set; }
        public FlowDirectionAnalysisVM_PropertyClass FlowDirectionNotLastNewCarAnalysisVM { get; set; }
    }

    public class FlowDirectionAnalysisVM_PropertyClass
    {
        public List<FlowDirectionAnalysisVM_PropertyClass_PropertyClass> FlowDirectionDayAnalysisVM_PropertyClass { get; set; }
        public List<FlowDirectionAnalysisVM_PropertyClass_PropertyClass> FlowDirectionMonthAnalysisVM_PropertyClass { get; set; }
        public List<FlowDirectionAnalysisVM_PropertyClass_PropertyClass> FlowDirectionYearAnalysisVM_PropertyClass { get; set; }
    }
    public class FlowDirectionAnalysisVM_PropertyClass_PropertyClass
    {
        public int TopAgentId { get; set; }
        public int RenBaoInsureCount { get; set; }
        public int PinganInsureCount { get; set; }
        public int TaipingyangInsureCount { get; set; }
        public int GuoShouCaiInsureCount { get; set; }
        public int OtherSourceInsureCount { get; set; }
    }
}
