using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class BankListViewModel : BaseViewModel
    {
        public IList<BankModel> BankList { get; set; }
    }

    public class BankModel
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
    }
}
