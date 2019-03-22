using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order
{
    public class ReBackOrderRequest : BaseRequest2
    {
        public string OrderNum { get; set; }
        public string Remark { get; set; }
    }

    public class OrderRequest : BaseRequest2
    {
        public string OrderNum { get; set; }
    }

    public class PayWayOrderRequest : BaseRequest2
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [Required]
        public string OrderNum { get; set; }
        /// <summary>
        /// 合作银行ID
        /// </summary>
        public int PayWayId { get; set; }
    }

    public class PayWayOrTypeOrderRequest : BaseRequest2
    {
        private int _payType = -1;
        /// <summary>
        /// 支付方式
        /// </summary>
        public int PayType { get { return _payType; } set { _payType = value; } }

        /// <summary>
        /// 订单编号
        /// </summary>
        [Required]
        public string OrderNum { get; set; }

        private int _payWayId = -1;
        /// <summary>
        /// 合作银行ID
        /// </summary>
        public int PayWayId { get { return _payWayId; } set { _payWayId = value; } }

        /// <summary>
        /// 付款说明
        /// </summary>
        public string PayMentRemark { get; set; }

        private int _payMent = 1;
        /// <summary>
        /// 付款方
        /// </summary>
        public int PayMent { get { return _payMent; } set { _payMent = value; } }
    }


    public class TnoAndAmountOrderRequest : BaseRequest2
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 交强险投保单号
        /// </summary>
        public string ForceTno { get; set; }
        /// <summary>
        /// 商业险投保单号
        /// </summary>
        public string BizTno { get; set; }
        /// <summary>
        /// 应收金额
        /// </summary>
        public double ReceivableAmount { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public double PurchaseAmount { get; set; }
        /// <summary>
        /// 总计金额
        /// </summary>
        public double TotalAmount { get; set; }
    }
}
