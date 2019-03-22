
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order
{
    public class GetOrderDetailRequest : BaseRequest2
    {
        public string OrderNum { get; set; }
    }

    public class UpdateOrderStatusAndPnoRequest : BaseRequest2
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNum { get; set; }

        /// <summary>
        /// 核保状态
        /// </summary>
        public int SubmitStatus { get; set; }

        /// <summary>
        /// 商业保单号
        /// </summary>
        public string BizTno { get; set; }

        /// <summary>
        /// 交强保单号
        /// </summary>
        public string ForceTno { get; set; }
    }
}
