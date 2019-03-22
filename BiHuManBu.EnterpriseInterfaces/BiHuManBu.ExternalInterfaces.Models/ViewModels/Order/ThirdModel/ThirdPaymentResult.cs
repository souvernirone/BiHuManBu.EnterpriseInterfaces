using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order.ThirdModel
{
    public class ThirdPaymentResult
    {
        /// <summary>
        /// 支付 实收金额，表对应money字段
        /// </summary>
        public double PurchaseAmount { get; set; }
        /// <summary>
        /// 支付 备注，表对应remarks字段
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 支付 凭据图片,多张以逗号分隔
        /// </summary>
        public string CredentialImg { get; set; }
        /// <summary>
        /// 支付 时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 商业险保单号
        /// </summary>
        public string BizNo { get; set; }
        /// <summary>
        /// 交强险保单号
        /// </summary>
        public string ForceNo { get; set; }
        /// <summary>
        /// 出单时间（即核保时间）
        /// </summary>
        public string IsPaymentTime { get; set; }
        public string ForceTno { get; set; }
        public string BizTno { get; set; }
        public string OrderNum { get; set; }
    }
}
