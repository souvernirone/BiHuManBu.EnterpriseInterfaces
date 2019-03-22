using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class FindByTnoQueueInfoViewModel:BaseViewModel
    {
        /// <summary>
        /// 保费信息
        /// </summary>
        public OrderQuoteresult Quoteresult { get; set; }
        /// <summary>
        /// 关系人信息
        /// </summary>
        public OrderRelatedInfo Related { get; set; }
        /// <summary>
        /// 保额信息
        /// </summary>
        public OrderSavequote Savequote { get; set; }
        /// <summary>
        /// 核保单号和订单支付信息
        /// </summary>
        public OrderInfo Order { get; set; }
    }
}
