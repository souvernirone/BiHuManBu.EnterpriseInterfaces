using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order
{
    public class CommissionIntegralRequest : BaseRequest2
    {
        /// <summary>
        /// 车辆使用性质
        /// </summary>
        public int CarUsedType { get; set; }
        /// <summary>
        /// 人保精算口径
        /// </summary>
        public string RbJSKJ { get; set; }
        /// <summary>
        /// 交强总金额
        /// </summary>
        public double ForceTotal { get; set; }
        /// <summary>
        /// 商业总金额
        /// </summary>
        public double BizTotal { get; set; }
    }
}
