using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SfH5AverageViewModel : BaseViewModel
    {
        public SfH5AverageViewModel()
        {
            this.AverageInsureCount = 0;
            this.AverageRenewalCount = 0;
            this.AverageRatio = 0;
        }
        public decimal AverageInsureCount { get; set; }
        public decimal AverageRenewalCount { get; set; }
        public decimal AverageRatio { get; set; }
    }
}
