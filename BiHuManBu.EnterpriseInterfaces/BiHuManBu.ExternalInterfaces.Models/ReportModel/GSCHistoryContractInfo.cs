
namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class GSCHistoryContractInfo
    {
        public string Enddate { get; set; }   //保险止期
        public string InsureCompanyName { get; set; }     //保险公司
        public int IsCommerce { get; set; }     //是否商业
        public string PolicyNo { get; set; }   //保单号
        public string Strdate { get; set; }   //保险起期
    } 
}
