
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class CreateSureOrderNewRequest
    {
        /// <summary>
        /// bx_userinfo表的id
        /// </summary>
        public long BuId { get; set; }
        /// <summary>
        /// dd_order表的id
        /// </summary>
        public long DDOrderId { get; set; }
        /// <summary>
        /// 订单所属代理人
        /// </summary>
        public int OrderAgent { get; set; }
        /// <summary>
        /// 老的source值
        /// </summary>
        public int Source { get; set; }
    }
}
