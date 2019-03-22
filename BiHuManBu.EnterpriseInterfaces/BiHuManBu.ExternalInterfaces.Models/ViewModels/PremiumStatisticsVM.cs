using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class PremiumStatisticsVM
    {
        public int OrderTotal { get; set; }
        public decimal PremiumAmount { get; set; }
        public List<CustomerType> CustomerTypes { get; set; }
        public List<InsuranceCompany> InsuranceCompanies { get; set; }
        public List<InsuranceType> InsuranceTypes { get; set; }
    }

    public class InsuranceCompany
    {
        public int Company { get; set; }
        public decimal Premium { get; set; }
        public decimal Ratio { get; set; }
    }

    public class InsuranceType
    {
        public int Insurance { get; set; }
        public decimal Premium { get; set; }
        public decimal Ratio { get; set; }
    }

    public class CustomerType
    {
        public int Customer { get; set; }
        public int OrderCount { get; set; }
        public decimal Premium { get; set; }
        public decimal Ratio { get; set; }
    }

    public class DailyPaper
    {
        public int OrderTotal { get; set; }
        public decimal PremiumAmount { get; set; }
        public List<DailyOrder> DailyOrders { get; set; }
        public List<DailyPremium> DailyPremia { get; set; }
    }

    public class DailyOrder
    {
        public int Customer { get; set; }
        public int OrderCount { get; set; }
        public decimal Ratio { get; set; }
    }

    public class DailyPremium
    {
        public int Customer { get; set; }
        public decimal Premium { get; set; }
        public decimal Ratio { get; set; }
    }
}
