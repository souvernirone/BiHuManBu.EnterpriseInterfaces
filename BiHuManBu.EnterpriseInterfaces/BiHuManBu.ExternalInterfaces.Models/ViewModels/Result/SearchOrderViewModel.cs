using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.Model;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
    /// <summary>
    /// 
    /// </summary>
    public class SearchOrderViewModel:BaseViewModel<SearchOrderViewModel>
    {
        private List<DDOrder> listOrder = new List<DDOrder>();

        public List<DDOrder> ListOrder { get { return listOrder; } set { listOrder = value; } }

        public int TotalCount { get; set; }
    }
}
