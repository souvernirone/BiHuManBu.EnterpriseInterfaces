using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCPersonal
{
    public class GetMyWalletAmountInfoViewModel:BaseViewModel
    {
        /// <summary>
        /// 余额
        /// </summary>
        public double MyBalance { get; set; }
        /// <summary>
        /// 个人佣金
        /// </summary>
        public double MyCommision { get; set; }
        /// <summary>
        /// 团队收益
        /// </summary>
        public double TeamInComing { get; set; }
    }
}
