using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order
{
    public class CommissionIntegralViewModel : BaseViewModel
    {/// <summary>
        /// 佣金总额
        /// </summary>
        public double Commission { get; set; }
        /// <summary>
        /// 积分总和
        /// </summary>
        public double Integral { get; set; }

        /// <summary>
        /// 交强佣金
        /// </summary>
        public double ForceCommission { get; set; }
        /// <summary>
        /// 交强积分
        /// </summary>
        public double ForceIntegral { get; set; }

        /// <summary>
        /// 商业佣金
        /// </summary>
        public double BizCommission { get; set; }
        /// <summary>
        /// 商业积分
        /// </summary>
        public double BizIntegral { get; set; }
    }
}
