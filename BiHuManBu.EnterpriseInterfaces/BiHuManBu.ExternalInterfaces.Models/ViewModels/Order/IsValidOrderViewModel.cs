
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order
{
    public class IsValidOrderViewModel : BaseViewModel
    {
        //public long OrderId { get; set; }
        public string OrderNum { get; set; }
        /// <summary>
        /// *订单状态 0暂存、1已过期、2废弃(取消)、3被踢回、41待支付待承保（进行中）、42已支付待承保、5已支付已承保（已完成）
        /// 暂存传0，提交订单传41
        /// </summary>
        public int OrderType { get; set; }
        /// <summary>
        /// 订单状态内容
        /// </summary>
        public string OrderStatus { get; set; }
    }
}
