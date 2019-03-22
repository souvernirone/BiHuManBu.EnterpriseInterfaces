using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class UpdateOrderRequest
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        //[Range(1,2100000000)]
        public long OrderId { get; set; }
        /// <summary>
        /// 订单状态，0取消订单，1下单成功，-2已出单，-3已收单
        /// </summary>
        [Range(-1000,1000)]
        public int OrderStatus { get; set; }
        /// <summary>
        /// 付款状态, 0未付款，1已付款，2已退款
        /// </summary>
        [Range(-1000, 1000)]
        public int PayStatus { get; set; }
        /// <summary>
        /// 当前代理人id
        /// </summary>
        //[Range(1, 1000000)]
        public int AgentId { get; set; }
        /// <summary>
        /// 校验码
        /// </summary>
        //[Required]
        //[StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }
        /// <summary>
        /// 代理人的openid
        /// </summary>
        //[Required]
        public string OpenId { get; set; }
    }
}
