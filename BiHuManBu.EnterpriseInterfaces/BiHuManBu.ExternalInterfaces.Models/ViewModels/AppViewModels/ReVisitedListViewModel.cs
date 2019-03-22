
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class ReVisitedListViewModel:BaseViewModel
    {
        public VisitedInfo VisitedInfo { get; set; }
        public List<VisitedDetail> VisitedList { get; set; }
    }

    public class VisitedInfo
    {
        public string CustomerName { get; set; }
        public string Phone { get; set; }
    }

    public class VisitedDetail
    {
        public int Id { get; set; }
        public int? ResultStatus { get; set; }
        public string RecallTime { get; set; }
        public string CreateTime { get; set; }
        public string Remark { get; set; }
    }
}
