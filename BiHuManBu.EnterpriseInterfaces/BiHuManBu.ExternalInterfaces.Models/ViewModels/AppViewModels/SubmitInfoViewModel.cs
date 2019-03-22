namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class SubmitInfoViewModel:BaseViewModel
    {
        public SubmitInfoDetail Item { get; set; }
        public string CustKey { get; set; }
        public string OrderId { get; set; }
        public string CheckCode { get; set; }
       
    }
    public class SubmitInfoNewViewModel : AppBaseViewModel
    {
        public SubmitInfoDetail Item { get; set; }
        public string OrderId { get; set; }
        public string CheckCode { get; set; }

    }

    public class SubmitInfoDetail
    {
        public int Source { get; set; }
        /// <summary>
        /// 核保状态
        /// </summary>
        public int SubmitStatus { get; set; }
        /// <summary>
        /// 核保状态描述
        /// </summary>
        public string SubmitResult { get; set; }
        /// <summary>
        /// 商业险投保单号
        /// </summary>
        public string BizNo { get; set; }
        /// <summary>
        /// 交强险投保单号
        /// </summary>
        public string ForceNo { get; set; }
        /// <summary>
        /// 商业险费率
        /// </summary>
        public double BizRate { get; set; }
        /// <summary>
        /// 交强车船费率
        /// </summary>
        public double ForceRate { get; set; }

    }
}