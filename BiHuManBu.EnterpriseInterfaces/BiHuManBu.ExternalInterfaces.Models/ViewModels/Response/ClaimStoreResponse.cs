using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{

    public class PcOrderPageResponse
    {
        public PageModel Data { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }
    }

    public class PageModel
    {
        public List<PcClaimOrderModel> OrderList { get; set; }

        public int TotalCount { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
