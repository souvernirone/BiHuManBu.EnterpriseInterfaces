using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class ListCommissionsWithdrawalViewModel : BaseViewModel
    {
        /// <summary>
        /// 佣金
        /// </summary>
        public SunMoneys ToMoney { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public SunMoneys ToCredit { get; set; }
    }

    public class SunMoneys
    {
        /// <summary>
        /// 佣金总额
        /// </summary>
        public double Money { get; set; }
        /// <summary>
        /// 佣金所属订单ID集合
        /// </summary>
        public string MoneyIds { get; set; }

        /// <summary>
        /// 积分总额
        /// </summary>
        public double Credit { get; set; }
    }
}
