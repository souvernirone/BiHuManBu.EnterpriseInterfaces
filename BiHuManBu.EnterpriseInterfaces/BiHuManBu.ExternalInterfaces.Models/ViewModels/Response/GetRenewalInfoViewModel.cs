using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
    public class GetRenewalInfoViewModel : BaseViewModel
    {
        public int RenewalStatus { get; set; }

        public CurDataUser CurDataUser { get; set; }

        public RenewalInfo RenewalInfo { get; set; }
    }

    public class CurDataUser
    {
        public string CurOpenId { get; set; }

        public string CurAgent { get; set; }

        public int IsUsed { get; set; }

        public int IsChangeRelation { get; set; }
    }

    public class RenewalInfo
    {

        public CustomerInfo CustomerInfo { get; set; }

        public CarInfo CarInfo { get; set; }

        public PreRenewalInfo PreRenewalInfo { get; set; }
        public XianZhong XianZhong { get; set; }
    }
}
