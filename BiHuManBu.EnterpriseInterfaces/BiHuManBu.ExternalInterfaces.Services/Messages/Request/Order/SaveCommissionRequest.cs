using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order
{
    public class SaveCommissionRequest : BaseRequest2
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        public double Commission { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Integral { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string LincenseNo { get; set; }

        /// <summary>
        /// 保单号
        /// </summary>
        public string InsurancePolicyNo { get; set; }

        /// <summary>
        /// 保单类型，1:交强 2:商业
        /// </summary>
        public sbyte InsurancePolicyType { get; set; }

        /// <summary>
        /// 佣金类型，1:个人收益 2:资金提现 3:团队收益
        /// </summary>
        public sbyte CommissionType { get; set; }

        /// <summary>
        /// 佣金状态，1:生效 2:未生效
        /// </summary>
        public sbyte Status { get; set; }
    }

    public class SaveCommissionIntegralRequest
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 佣金状态，1:生效 2:未生效
        /// </summary>
        public int Status { get; set; }
    }
}
