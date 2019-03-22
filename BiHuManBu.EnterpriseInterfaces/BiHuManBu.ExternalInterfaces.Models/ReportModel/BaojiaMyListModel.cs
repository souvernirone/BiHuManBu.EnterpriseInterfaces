
using System.Collections.Generic;
namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class BaojiaMyListModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string LicenseOwner { get; set; }
        public string LicenseNo { get; set; }
        public string MoldName { get; set; }
        public string last_business_end_date { get; set; }
        public string last_end_date { get; set; }
        public List<QuoteTotal> quoteresult { get; set; }
    }
    public class QuoteTotal
    {
        public int source { get; set; }
        public string BizTotal { get; set; }
        public string ForceTotal { get; set; }
        public string TaxTotal { get; set; }
        public string Total { get; set; }
        public int? submit_status { get; set; }
    }
}
