using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
   public class AccidentMessageModel
    {
       public List<AccidentNoticeMessageModel> MessageList { get; set; }

        public List<RobbingModel> RobbingMessageList { get; set; }

        public int PageIndex { get; set; }

       public int PageSize { get; set; }

       public int TotalCount { get; set; }
    }

    public class AccidentMessageResponse
    {
        public AccidentMessageModel Data { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }
    }

    public class AccidentClueDataModel
    {
        public List<AccidentClueModel> ClueList { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }
    }

    public class AccidentClueResponse
    {
        public AccidentClueDataModel Data { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }
    }
}
