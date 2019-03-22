
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GetQuoteStatusViewModel : BaseViewModel
    {
        public long Buid { get; set; }
        public long Source { get; set; }
        public string QuoteStatus { get; set; }
        public string QuoteResult { get; set; }
        public string QuoteResultToc { get; set; }
        public string SubmitStatus { get; set; }
        public string SubmitResult { get; set; }
        public string SubmitResultToc { get; set; }

        /// <summary>
        /// 为app新加的 商业险总额
        /// </summary>
        public double BizTotal { get; set; }
        /// <summary>
        /// 为app新加的 交强+车船总额
        /// </summary>
        public double ForceTotal { get; set; }
    }

    public class GetQuoteStatusForAppViewModel
    {
        public long Buid { get; set; }
        public long Source { get; set; }
        public int QuoteStatus { get; set; }
        public string QuoteResult { get; set; }
        //public string QuoteResultToc { get; set; }
        public int SubmitStatus { get; set; }
        public string SubmitResult { get; set; }
        //public string SubmitResultToc { get; set; }
        /// <summary>
        /// 为app新加的 商业险总额
        /// </summary>
        public double BizTotal { get; set; }
        /// <summary>
        /// 为app新加的 交强+车船总额
        /// </summary>
        public double ForceTotal { get; set; }

    }
}
