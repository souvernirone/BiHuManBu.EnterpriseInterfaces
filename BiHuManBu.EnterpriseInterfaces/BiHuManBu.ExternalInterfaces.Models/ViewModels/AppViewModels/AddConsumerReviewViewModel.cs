using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
  public  class AddConsumerReviewViewModel: BaseViewModel
    {
        public int CategoryId { get; set; }
        public bool IsDelete { get; set; }
    }
}
