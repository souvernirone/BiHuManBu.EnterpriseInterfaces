using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        public CarOrderModel CarOrder { get; set; }
    }

    public class OrdersViewModel : BaseViewModel
    {
        public int TotalCount { get; set; }
        public List<CarOrderModel> CarOrders { get; set; }
    }

}
