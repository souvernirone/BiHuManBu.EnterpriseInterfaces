using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SettlementViewModel
    {
        public int Id { get; set; }

        public string BatchNum { get; set; }

        public string SettledStart { get; set; }

        public string SettledEnd { get; set; }

        public int OrderCount { get; set; }

        public decimal ExptectAmount { get; set; }

        public int SettledState { get; set; }

        public string SettledTime { get; set; }

        public int InvoiceType { get; set; }

        public decimal Cost { get; set; }

        public decimal ActualAmount { get; set; }
    }
}
