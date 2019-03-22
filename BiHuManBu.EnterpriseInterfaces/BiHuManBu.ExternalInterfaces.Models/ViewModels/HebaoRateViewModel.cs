
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class HebaoRateViewModel:BaseViewModel
    {
        /// <summary>
        /// 是否显示费率/计算器 0 展示，1不展示
        /// </summary>
        public int IsShowCalc { get; set; }

        /// <summary>
        /// 商业系统费率
        /// </summary>
        public decimal BizSysRate { get; set; }
        /// <summary>
        /// 交强系统费率
        /// </summary>
        public decimal ForceSysRate { get; set; }
        /// <summary>
        /// 优惠费率
        /// </summary>
        public decimal BenefitRate { get; set; }
    }
}
